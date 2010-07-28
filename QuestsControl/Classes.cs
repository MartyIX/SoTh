using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sokoban.Model
{
    public class Round
    {
        public Round()
        {

        }

        public Round(int id, string name, string variant, TimeSpan bestTime)
        {
            Name = name;
            ID = id;
            BestTime = bestTime;
            Variant = variant;
        }

        public string Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public string Variant
        {
            get;
            set;
        }


        public TimeSpan BestTime
        {
            get;
            set;
        }

        public override string ToString()
        {
            TimeSpan ts = BestTime;
            string formattedTimeSpan;

            if (ts == TimeSpan.Zero)
            {
                formattedTimeSpan = "Not available";
            }
            else if (ts.Hours > 0)
            {
                formattedTimeSpan = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
            }
            else
            {
                formattedTimeSpan = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
            }

            return string.Format("{0} (Variant: {1} | Best time: {2})", Name, Variant, formattedTimeSpan);
        }
    }

    public class Rounds : ObservableCollection<Round> { }

    public class League : INotifyPropertyChanged
    {
        public League()
        {
            Rounds = new Rounds();
        }

        public League(int id, string name, bool loaded, Rounds rounds)
        {
            Name = name;
            Rounds = rounds;
            ID = id;
            Loaded = loaded;
        }

        private bool loaded;

        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; Notify("Loaded"); Notify("Name"); }
        }

        private string name;

        public string Name
        {
            get
            {
                if (loaded == true)
                {
                    return name;
                }
                else
                {
                    return name + " [not loaded]";
                }
            }
            set { name = value; Notify("Name"); }
        }

        private Rounds rounds;
        public Rounds Rounds
        {
            get {return rounds;}
            set { rounds = value; Notify("Rounds"); }
        }


        public int ID
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class Leagues : ObservableCollection<League> { }

    public class Category : INotifyPropertyChanged
    {
        public Category()
        {
            Leagues = new Leagues();
        }

        public Category(int id, string name, bool loaded, Leagues leagues)
        {
            ID = id;
            Name = name;
            Leagues = leagues;
            Loaded = loaded;
        }

        public int ID
        {
            get;
            set;
        }

        private bool loaded;

        public bool Loaded
        {
            get { return loaded; }
            set { loaded = value; Notify("Loaded"); Notify("Name"); }
        }

        private string name;

        public string Name
        {
            get
            {
                if (loaded == true)
                {
                    return name;
                }
                else
                {
                    return name + " [not loaded]";
                }
            }
            set { name = value; Notify("Name"); }
        }

        private Leagues leagues;
        public Leagues Leagues
        {
            get { return leagues; }
            set { leagues = value; Notify("Leagues"); }        
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class Categories : ObservableCollection<Category> { }
}
