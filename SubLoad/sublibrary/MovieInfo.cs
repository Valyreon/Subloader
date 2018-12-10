namespace SubLib
{
    public class MovieInfo
    {
        public string sublanguageid;
        public string moviehash;
        public double moviebytesize;

        public MovieInfo(string path, string languages)
        {
            this.moviehash = GetHash.Main.ToHexadecimal(GetHash.Main.ComputeHash(path));
            this.sublanguageid = languages;
            this.moviebytesize = new System.IO.FileInfo(path).Length;
        }
    }
}