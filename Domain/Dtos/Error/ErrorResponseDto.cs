namespace Domain.Dtos.Error;

public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; }
    public List<ErrorDto>? Errors { get; set; }
}