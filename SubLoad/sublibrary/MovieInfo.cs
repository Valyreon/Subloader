using System;
using System.Collections.Generic;
using System.Text;

namespace SubLib
{
    public class MovieInfo
    {
        public string sublanguageid;
        public string moviehash;
        public double moviebytesize;

        public MovieInfo(string path, string languages)
        {
            moviehash = GetHash.Main.ToHexadecimal(GetHash.Main.ComputeHash(path));
            sublanguageid = languages;
            moviebytesize = new System.IO.FileInfo(path).Length;
        }
    }
}
