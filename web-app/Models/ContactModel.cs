namespace web_app.Models;

public class Contact
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? WANumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
