namespace Domain.Dtos;

public class AuthResponseDto
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public UserDto user { get; set; }
}