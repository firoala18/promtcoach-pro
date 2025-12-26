using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using OpenAI.Embeddings;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public sealed class SemanticIndexService : ISemanticIndexService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly IAiProviderResolver _resolver;

        public SemanticIndexService(ApplicationDbContext db, IConfiguration cfg, IAiProviderResolver resolver)
        {
            _db = db;
            _cfg = cfg;
            _resolver = resolver;
        }

        public async Task UpsertFilterItemAsync(FilterItem item, string? group, string? ownerUserId, CancellationToken ct)
        {
            var cat = await _db.Set<FilterCategory>()
                               .AsNoTracking()
                               .Where(c => c.Id == item.FilterCategoryId)
                               .Select(c => new { c.Type, c.UserId })
                               .FirstOrDefaultAsync(ct);

            var title = StripHtml(item.Title ?? string.Empty).Trim();
            var info = StripHtml(item.Info ?? string.Empty).Trim();
            var instr = StripHtml(item.Instruction ?? string.Empty).Trim();
            var content = string.Join("\n", new[] { title, info, instr }.Where(s => !string.IsNullOrWhiteSpace(s)));

            var entityType = "FilterItem";
            var entityId = item.Id.ToString();

            var existingList = await _db.SemanticIndexEntries
                                        .Where(e => e.EntityType == entityType && e.EntityId == entityId)
                                        .OrderByDescending(e => e.UpdatedAt)
                                        .ToListAsync(ct);
            if (existingList.Count > 0)
            {
                var eq = existingList.FirstOrDefault(e => ContentEquivalent(e.Content, content));
                if (eq != null)
                {
                    // Update metadata without re-embedding; remove duplicates
                    eq.Title = title;
                    eq.Group = group;
                    eq.OwnerUserId = ownerUserId;
                    eq.UpdatedAt = DateTime.UtcNow;

                    var toRemove = existingList.Where(e => e.Id != eq.Id).ToList();
                    if (toRemove.Count > 0)
                        _db.SemanticIndexEntries.RemoveRange(toRemove);

                    await _db.SaveChangesAsync(ct);
                    return; // up-to-date, no embedding call
                }
            }

            var apiKey = await _resolver.ResolveEmbeddingsKeyAsync(ct);
            var embClient = new EmbeddingClient("text-embedding-3-small", apiKey);
            var resp = await embClient.GenerateEmbeddingAsync(content, cancellationToken: ct);
            var embeddingMem = resp.Value.ToFloats();
            var vectorArray = embeddingMem.ToArray(); // avoid Span in async method (C# 12 compatible)
            double sumSq = 0;
            for (int i = 0; i < vectorArray.Length; i++)
            {
                var v = vectorArray[i];
                sumSq += v * v;
            }
            var norm = Math.Sqrt(sumSq);
            var json = System.Text.Json.JsonSerializer.Serialize(vectorArray);

            var existing = existingList.FirstOrDefault();
            if (existing == null)
            {
                existing = new SemanticIndexEntry
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Title = title,
                    Content = content,
                    EmbeddingJson = json,
                    VectorNorm = norm,
                    Group = group,
                    OwnerUserId = ownerUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.SemanticIndexEntries.Add(existing);
            }
            else
            {
                existing.Title = title;
                existing.Content = content;
                existing.EmbeddingJson = json;
                existing.VectorNorm = norm;
                existing.Group = group;
                existing.OwnerUserId = ownerUserId;
                existing.UpdatedAt = DateTime.UtcNow;

                // Remove other duplicates after updating the newest
                var toRemove = existingList.Where(e => e.Id != existing.Id).ToList();
                if (toRemove.Count > 0)
                    _db.SemanticIndexEntries.RemoveRange(toRemove);
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveForFilterItemAsync(int itemId, CancellationToken ct)
        {
            var entityType = "FilterItem";
            var entityId = itemId.ToString();
            var rows = await _db.SemanticIndexEntries
                                 .Where(e => e.EntityType == entityType && e.EntityId == entityId)
                                 .ToListAsync(ct);
            if (rows.Count > 0)
            {
                _db.SemanticIndexEntries.RemoveRange(rows);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<int> BackfillAllFilterItemsAsync(PromptType? type, CancellationToken ct)
        {
            var q = _db.Set<FilterItem>()
                        .AsNoTracking()
                        .Join(_db.Set<FilterCategory>().AsNoTracking(), i => i.FilterCategoryId, c => c.Id,
                              (i, c) => new { Item = i, Cat = c })
                        .Where(x => x.Cat.UserId == null);

            if (type.HasValue)
                q = q.Where(x => x.Cat.Type == type.Value);

            // Only process items that do NOT yet have a semantic index entry
            var items = await q.Select(x => x.Item).ToListAsync(ct);
            var idStrings = items.Select(i => i.Id.ToString()).ToList();
            if (idStrings.Count == 0) return 0;

            var existingIds = await _db.SemanticIndexEntries
                                       .AsNoTracking()
                                       .Where(e => e.EntityType == "FilterItem" && idStrings.Contains(e.EntityId))
                                       .Select(e => e.EntityId)
                                       .ToListAsync(ct);
            var existingSet = existingIds.ToHashSet(StringComparer.Ordinal);
            var missing = items.Where(i => !existingSet.Contains(i.Id.ToString())).ToList();

            int count = 0;
            foreach (var item in missing)
            {
                try
                {
                    await UpsertFilterItemAsync(item, group: null, ownerUserId: null, ct);
                    count++;
                }
                catch
                {
                }
            }
            return count;
        }

        public async Task<int> BackfillUserFilterItemsAsync(string ownerUserId, PromptType? type, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ownerUserId)) return 0;

            var q = _db.Set<FilterItem>()
                        .AsNoTracking()
                        .Join(_db.Set<FilterCategory>().AsNoTracking(), i => i.FilterCategoryId, c => c.Id,
                              (i, c) => new { Item = i, Cat = c })
                        .Where(x => x.Cat.UserId == ownerUserId);

            if (type.HasValue)
                q = q.Where(x => x.Cat.Type == type.Value);

            var items = await q.Select(x => x.Item).ToListAsync(ct);
            var idStrings = items.Select(i => i.Id.ToString()).ToList();
            if (idStrings.Count == 0) return 0;

            var existingIds = await _db.SemanticIndexEntries
                                       .AsNoTracking()
                                       .Where(e => e.EntityType == "FilterItem" && idStrings.Contains(e.EntityId))
                                       .Select(e => e.EntityId)
                                       .ToListAsync(ct);
            var existingSet = existingIds.ToHashSet(StringComparer.Ordinal);
            var missing = items.Where(i => !existingSet.Contains(i.Id.ToString())).ToList();

            int count = 0;
            foreach (var item in missing)
            {
                try
                {
                    await UpsertFilterItemAsync(item, group: null, ownerUserId: ownerUserId, ct);
                    count++;
                }
                catch
                {
                }
            }
            return count;
        }

        private static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        private static string Canonicalize(string? s)
        {
            s ??= string.Empty;
            // unify newlines (handle CRLF, CR, literal backslash-n) and collapse whitespace
            var t = s.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\\n", "\n");
            t = Regex.Replace(t, "\n+", "\n");
            t = Regex.Replace(t, "[\t ]+", " ");
            t = t.Trim();
            return t;
        }

        private static bool ContentEquivalent(string? a, string? b)
        {
            return string.Equals(Canonicalize(a), Canonicalize(b), StringComparison.Ordinal);
        }

        
    }
}
