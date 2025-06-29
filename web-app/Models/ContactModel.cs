using System.ComponentModel.DataAnnotations;

namespace web_app.Models;

public class Contact
{
    public int Id { get; set; }
    public required string? Name { get; set; }

    [RegularExpression(@"^(\+62|62|0)8[1-9][0-9]{6,9}$", ErrorMessage = "Invalid Indonesian phone number")]
    public required string? WANumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
