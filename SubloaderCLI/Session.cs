namespace SubloaderCLI;
public class Session
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string BaseUrl { get; set; }
    public string Level { get; set; }
    public int AllowedDownloads { get; set; }
    public int RemainingDownloads { get; set; }
    public bool IsVIP { get; set; }
}
