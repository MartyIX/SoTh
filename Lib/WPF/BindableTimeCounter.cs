using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;

namespace Sokoban.WPF
{
    public class BindableTimeCounter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _now;
        private DateTime start;
        private bool isInitialized = false;
        private DispatcherTimer timer = null;
        private int updatePeriod;

        public BindableTimeCounter()
        {
            this.updatePeriod = 1000;
        }
        
        public BindableTimeCounter(int updatePeriod)
        {
            this.updatePeriod = updatePeriod;
        }

        public BindableTimeCounter(int updatePeriod, DateTime start)
        {
            this.updatePeriod = updatePeriod;
            Initialize(start);
        }

        public void Initialize(DateTime start)
        {
            this.start = start;
            isInitialized = true;

            _now = DateTime.Now;

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(updatePeriod);
                timer.Tick += new EventHandler(timer_Tick);
            }
        }

        public void Start()
        {
            if (timer != null)
            {
                timer.Start();
            }
        }

        public void Pause()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        public void Clear()
        {
            if (timer != null)
            {
                timer.Stop();
            }

            isInitialized = false;
            NotifyChange();
        }

        public TimeSpan Time
        {
            get {

                if (isInitialized == false)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return _now.Subtract(this.start);
                }
            }
        }

        public string FormattedTime
        {
            get {

                TimeSpan ts = this.Time;

                if (ts.Hours > 0)
                {
                    string formattedTimeSpan = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
                    return formattedTimeSpan;
                }
                else
                {
                    string formattedTimeSpan = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                    return formattedTimeSpan;
                }                
            }
        }

        private DateTime now
        {
            set
            {
                _now = value;
                NotifyChange();
            }
        }

        private void NotifyChange()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Time"));
                PropertyChanged(this, new PropertyChangedEventArgs("FormattedTime"));
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            now = DateTime.Now;
        }
    }
}
