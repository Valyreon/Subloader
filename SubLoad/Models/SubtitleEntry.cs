﻿using System.ComponentModel;

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
        }

        public string Language
        {
            get
            {
                return language;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
