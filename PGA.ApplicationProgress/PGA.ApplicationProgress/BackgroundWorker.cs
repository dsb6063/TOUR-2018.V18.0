using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.ApplicationProgress
{


    public class BackgroundWorker
    {


        private int _total;
        public int total
        {
            get
            {
                return _total;
            }

            set
            {
                if (_total == value)
                {
                    return;
                }

                _total = value;
                RaisePropertyChanged("total");
            }
        }

        private int _stageone;
        public int stageone
        {
            get
            {
                return _stageone;
            }

            set
            {
                if (_stageone == value)
                {
                    return;
                }

                _stageone = value;
                RaisePropertyChanged("stageone");
            }
        }
        private int _stagetwo;

        public int stagetwo
        {
            get
            {
                return _stagetwo;
            }

            set
            {
                if (_stagetwo == value)
                {
                    return;
                }

                _stagetwo = value;
                RaisePropertyChanged("stagetwo");
            }
        }
        private int _stagethree;

        public int stagethree
        {
            get
            {
                return _stagethree;
            }

            set
            {
                if (_stagethree == value)
                {
                    return;
                }

                _stagethree = value;
                RaisePropertyChanged("stagethree");
            }
        }

        public  System.Windows.Forms.Timer myTimer = null;
        static int alarmCounter = 1;
        static bool exitFlag = false;

        public BackgroundWorker()
        {
            myTimer = new System.Windows.Forms.Timer();
            alarmCounter = 1;
            exitFlag = false;
        }

        // This is the method to run when the timer is raised.


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
