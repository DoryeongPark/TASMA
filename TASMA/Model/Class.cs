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
        public class Class : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string className;
            public string ClassName
            {
                get { return className; }
                set { className = value; }
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
