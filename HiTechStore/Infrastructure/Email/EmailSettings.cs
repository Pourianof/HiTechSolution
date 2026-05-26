namespace HiTechStore.Infrastructure.Email;

public class EmailSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool UseSsl { get; set; } = true;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string FromName { get; set; } = "HiTechStore";
    public string FromAddress { get; set; } = "no-reply@hitechstore.local";
}
