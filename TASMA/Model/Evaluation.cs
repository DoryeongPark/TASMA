using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMA.Model
{
    public class Evaluation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string key;
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged("Key"); }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        private int ratio;
        public int Ratio
        {
            get { return ratio; }
            set { ratio = value;  OnPropertyChanged("Ratio"); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
