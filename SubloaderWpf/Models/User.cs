using System;
using SubloaderWpf.Mvvm;

namespace SubloaderWpf.Models;

public class User : ObservableEntity
{
    public int AllowedDownloads { get; set; }
    public bool IsVIP { get; set; }
    public string Level { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public long TokenExpirationUnixTimestamp { get; set; }
    public string TokenExpirationTimestampString
    {
        get
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(TokenExpirationUnixTimestamp);
            var dateTime = dateTimeOffset.LocalDateTime;
            return dateTime.ToString("u")[..^1];
        }
        set => _ = value;
    }
}
