using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMA
{
    namespace Model
    {
        public class Semester : INotifyPropertyChanged
        {
            private int value;
            public int Value
            {
                get { return value; }
                set { this.value = value; OnPropertyChanged("Value"); }
            }

            private string text;
            public string Text
            {
                get { return text; }
                set { text = value;  OnPropertyChanged("Text"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
