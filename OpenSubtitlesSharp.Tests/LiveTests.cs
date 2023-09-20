namespace OpenSubtitlesSharp.Tests
{
    public class LiveTests
    {
        [Fact]
        public async Task LoginTest()
        {
            var client = new OpenSubtitlesClient("idMNeWNRIKVKlGiP8zjNyG80a4AqKYBd");
            var langs = await client.GetLanguagesAsync();
            var formats = await client.GetSubtitleFormatsAsync();
            var loginInfo = await client.LoginAsync("Valyreon", "OpenSubtitles!111");

            Console.WriteLine("DONE");
        }
    }
}
