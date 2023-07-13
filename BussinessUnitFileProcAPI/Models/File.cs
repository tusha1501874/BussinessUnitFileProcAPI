namespace BussinessUnitFileProcAPI.Models;
public class File
{
    public string Filename { get; set; }
    public int FileSize { get; set; }
    public string MimeType { get; set; }
    public string Hash { get; set; }
    public List<Attributes> Attributes { get; set; }
}