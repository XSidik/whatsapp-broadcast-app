using System.ComponentModel.DataAnnotations;

namespace web_app.DTOs;

public class SendMessageDto
{    
    public required string[] Numbers { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s\.,!?]*$", ErrorMessage = "Message contains invalid characters.")]
    public required string Message { get; set; }       
}