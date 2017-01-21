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
        public class EvaluationListBoxItem : INotifyPropertyChanged
        {
            private string name;
            
            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged("Name"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
