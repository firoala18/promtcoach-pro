using ProjectsWebApp.DataAccsess.Services.Interfaces;
using System.Text;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public class PdfChunkingService : IPdfChunkingService
    {
        private const int CharsPerToken = 4;

        public List<DocumentChunk> ChunkDocument(PdfExtractionResult pdf, int maxTokensPerChunk = 6000)
        {
            var chunks = new List<DocumentChunk>();
            if (pdf.Pages.Count == 0 || string.IsNullOrWhiteSpace(pdf.FullText))
                return chunks;

            int maxCharsPerChunk = maxTokensPerChunk * CharsPerToken;

            var currentChunk = new StringBuilder();
            int currentCharCount = 0;
            int startPage = 0;
            int endPage = 0;

            foreach (var page in pdf.Pages)
            {
                if (string.IsNullOrWhiteSpace(page.Text))
                    continue;

                var pageText = page.Text.Trim();
                var pageCharCount = pageText.Length;

                if (pageCharCount > maxCharsPerChunk)
                {
                    if (currentChunk.Length > 0)
                    {
                        chunks.Add(CreateChunk(currentChunk.ToString(), startPage, endPage));
                        currentChunk.Clear();
                        currentCharCount = 0;
                    }

                    var pageChunks = SplitLargePage(pageText, page.PageNumber, maxCharsPerChunk);
                    chunks.AddRange(pageChunks);
                    startPage = 0;
                    continue;
                }

                int separatorLength = currentChunk.Length > 0 ? 4 : 0;
                if (currentCharCount + separatorLength + pageCharCount > maxCharsPerChunk)
                {
                    if (currentChunk.Length > 0)
                    {
                        chunks.Add(CreateChunk(currentChunk.ToString(), startPage, endPage));
                        currentChunk.Clear();
                        currentCharCount = 0;
                    }
                    startPage = 0;
                }

                if (currentChunk.Length > 0)
                {
                    currentChunk.AppendLine().AppendLine();
                    currentCharCount += 4;
                }
                else
                {
                    startPage = page.PageNumber;
                }

                currentChunk.Append(pageText);
                currentCharCount += pageCharCount;
                endPage = page.PageNumber;
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(CreateChunk(currentChunk.ToString(), startPage, endPage));
            }

            return chunks;
        }

        private static DocumentChunk CreateChunk(string content, int startPage, int endPage)
        {
            var trimmedContent = content.Trim();
            var sourceInfo = startPage == endPage
                ? $"Seite {startPage}"
                : $"Seiten {startPage}-{endPage}";

            return new DocumentChunk
            {
                Content = trimmedContent,
                SourceInfo = sourceInfo,
                ApproxTokens = trimmedContent.Length / CharsPerToken
            };
        }

        private static List<DocumentChunk> SplitLargePage(string pageText, int pageNumber, int maxCharsPerChunk)
        {
            var chunks = new List<DocumentChunk>();
            var paragraphs = pageText.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);
            var currentChunk = new StringBuilder();
            int partNumber = 1;

            foreach (var para in paragraphs)
            {
                var trimmedPara = para.Trim();
                if (string.IsNullOrWhiteSpace(trimmedPara))
                    continue;

                if (trimmedPara.Length > maxCharsPerChunk)
                {
                    if (currentChunk.Length > 0)
                    {
                        chunks.Add(new DocumentChunk
                        {
                            Content = currentChunk.ToString().Trim(),
                            SourceInfo = $"Seite {pageNumber} (Teil {partNumber})",
                            ApproxTokens = currentChunk.Length / CharsPerToken
                        });
                        currentChunk.Clear();
                        partNumber++;
                    }

                    for (int i = 0; i < trimmedPara.Length; i += maxCharsPerChunk)
                    {
                        var part = trimmedPara.Substring(i, Math.Min(maxCharsPerChunk, trimmedPara.Length - i));
                        chunks.Add(new DocumentChunk
                        {
                            Content = part,
                            SourceInfo = $"Seite {pageNumber} (Teil {partNumber})",
                            ApproxTokens = part.Length / CharsPerToken
                        });
                        partNumber++;
                    }
                    continue;
                }

                int separatorLength = currentChunk.Length > 0 ? 4 : 0;
                if (currentChunk.Length + separatorLength + trimmedPara.Length > maxCharsPerChunk)
                {
                    if (currentChunk.Length > 0)
                    {
                        chunks.Add(new DocumentChunk
                        {
                            Content = currentChunk.ToString().Trim(),
                            SourceInfo = $"Seite {pageNumber} (Teil {partNumber})",
                            ApproxTokens = currentChunk.Length / CharsPerToken
                        });
                        currentChunk.Clear();
                        partNumber++;
                    }
                }

                if (currentChunk.Length > 0)
                    currentChunk.AppendLine().AppendLine();
                currentChunk.Append(trimmedPara);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(new DocumentChunk
                {
                    Content = currentChunk.ToString().Trim(),
                    SourceInfo = partNumber == 1 ? $"Seite {pageNumber}" : $"Seite {pageNumber} (Teil {partNumber})",
                    ApproxTokens = currentChunk.Length / CharsPerToken
                });
            }

            return chunks;
        }
    }
}
