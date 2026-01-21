namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public interface IPdfChunkingService
    {
        List<DocumentChunk> ChunkDocument(PdfExtractionResult pdf, int maxTokensPerChunk = 6000);
    }

    public class DocumentChunk
    {
        public string Content { get; set; } = string.Empty;
        public string SourceInfo { get; set; } = string.Empty;
        public int ApproxTokens { get; set; }
    }
}
