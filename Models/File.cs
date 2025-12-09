using System.ComponentModel.DataAnnotations;

namespace apiBozzi.Models;

public class File
{
    [Key] public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required] public string IdStorage { get; set; } = string.Empty;

    [Required] public string Url { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long FileSize { get; set; }
}