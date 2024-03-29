using System;

namespace SubloaderAvalonia.Models;

public class User
{
    public int AllowedDownloads { get; set; }
    public string BaseUrl { get; set; }
    public bool IsVIP { get; set; }
    public string Level { get; set; }
    public int RemainingDownloads { get; set; }
    public DateTime? ResetTime { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
}
