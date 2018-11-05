using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLoad
{
    public class SubtitleEntry: INotifyPropertyChanged
    {
        private string name;
        private string language;
        private int SubFileID;
        private string SubFormat;

        public int GetSubtitleFileID()
        {
            return SubFileID;
        }

        public string GetFormat()
        {
            return SubFormat;
        }

        public SubtitleEntry(string n, string l, int id, string format)
        {
            name = n;
            language = l;
            SubFileID = id;
            SubFormat = format;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                name = value;
                this.OnPropertyChanged("Language");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
