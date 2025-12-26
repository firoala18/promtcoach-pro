// Models/PortalVideo.cs
public class PortalVideo
{
    public int Id { get; set; }
    public string VideoPath { get; set; }
    public DateTime? UploadDate { get; set; } = DateTime.Now;

    public string? ImagePath { get; set; }
    public bool ShowImageInsteadOfVideo { get; set; }  // new toggle flag
}