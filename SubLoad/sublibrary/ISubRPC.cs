namespace SubLib
{
    using CookComputing.XmlRpc;

    [XmlRpcUrl("http://api.opensubtitles.org/xml-rpc")]
    public interface ISubRPC : IXmlRpcProxy
    {
        [XmlRpcMethod("SearchSubtitles")]//endpoint name
        SearchSubtitlesResponse SearchSubtitles(string token, MovieInfo[] req);

        [XmlRpcMethod("LogIn")]//endpoint name
        LogInResponse LogIn(string username, string password, string language, string useragent);

        [XmlRpcMethod("LogOut")]//endpoint name
        LogOutResponse LogOut(string token);

        [XmlRpcMethod("DownloadSubtitles")]//endpoint name
        DownloadSubtitleResponse DownloadSubtitles(string token, int[] subtitle_ids);
    }
}