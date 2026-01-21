namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public interface IPdfTextExtractor
    {
        Task<PdfExtractionResult> ExtractTextAsync(Stream pdfStream, CancellationToken ct = default);
    }

    public class PdfExtractionResult
    {
        public List<PageContent> Pages { get; set; } = new();
        public int TotalPages { get; set; }
        public string FullText { get; set; } = string.Empty;
    }

    public class PageContent
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
