namespace Configuration;

public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 25;
    public string UserName { get; set; }
    public string Password { get; set; }

    public override string ToString()
    {
        return $"Host: {Host} Port: {Port} Username: {UserName} Password: {Password}";
    }
}