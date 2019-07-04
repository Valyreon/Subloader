using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLoad.Models
{
    public class SubtitleLanguage
    {
        public int Id { get; }
        public string Name { get; }
        public string Code { get; }

        public SubtitleLanguage(int id, string name, string code)
        {
            this.Id = id;
            this.Name = name;
            this.Code = code;
        }
    }
}
