namespace sso_back.Dtos.RequestDtos;

public class CreateClientReq
{
    public string ClientId { get; set; }=String.Empty;
    public string ClientUrl { get; set; }=String.Empty;
}