using ProjectsWebApp.DataAccsess.Services.Interfaces;
using UglyToad.PdfPig;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public class PdfTextExtractor : IPdfTextExtractor
    {
        public Task<PdfExtractionResult> ExtractTextAsync(Stream pdfStream, CancellationToken ct = default)
        {
            var result = new PdfExtractionResult();
            var fullTextBuilder = new System.Text.StringBuilder();

            using var document = PdfDocument.Open(pdfStream);
            result.TotalPages = document.NumberOfPages;

            foreach (var page in document.GetPages())
            {
                ct.ThrowIfCancellationRequested();

                var pageText = page.Text ?? string.Empty;
                pageText = NormalizeText(pageText);

                result.Pages.Add(new PageContent
                {
                    PageNumber = page.Number,
                    Text = pageText
                });

                if (fullTextBuilder.Length > 0 && !string.IsNullOrWhiteSpace(pageText))
                {
                    fullTextBuilder.AppendLine();
                    fullTextBuilder.AppendLine();
                }
                fullTextBuilder.Append(pageText);
            }

            result.FullText = fullTextBuilder.ToString().Trim();
            return Task.FromResult(result);
        }

        private static string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = System.Text.RegularExpressions.Regex.Replace(text, @"[\u00A0\u2000-\u200B\u202F\u205F\u3000]", " ");
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\n{3,}", "\n\n");

            var lines = text.Split('\n')
                           .Select(l => l.Trim())
                           .Where(l => !string.IsNullOrWhiteSpace(l) || l == string.Empty);

            return string.Join("\n", lines).Trim();
        }
    }
}
