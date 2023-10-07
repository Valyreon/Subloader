OpenSubtitlesSharp is a .NET library I developed for the Subloader application. It enables the developer to send requests to most useful OpenSubtitles API endpoints specified **[here](https://opensubtitles.stoplight.io/docs/opensubtitles-api/e3750fd63a100-getting-started)**.

The implemented endpoints include `Login`, `Logout`, `Search`(by file or query), `Download`, `Get Languages`, `Get Formats` and `Get User Info`.

### Usage

Example of usage:

```csharp
using var osClient = new OpenSubtitlesClient(Constants.APIKey);
var result = await osClient.GetLanguagesAsync();
```

The client has 2 more possible parameters for the constructors and those are the **JWT token** (you get it on login) and specifying whether to use the **Default** or **VIP** API Url. When you login the user there will be a bool field indicating whether that user is VIP, in that case the user can use the VIP endpoint.

For any non-OK status return from OS API, the client will throw `RequestFailureException`. The exception will contain the returned status code and the message from the API. These messages are generally very descriptive.

For example if you don't specify enough parameters for your search, OS API returns `406 Not Acceptable`:
```csharp
using var osClient = new OpenSubtitlesClient(Constants.APIKey, jwtToken);

try
{
    var searchParams = new SearchParameters
    {
        SeasonNumber = 5
    };

    var result = await osClient.SearchAsync(searchParams);
}
catch(RequestFailureException ex)
{
    Console.WriteLine($"API returned {ex.StatusCode} with message '{ex.Message}'"); // not enough parameters
}
```




