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
        public class Subject : INotifyPropertyChanged
        {
            private string subjectName;
            public string SubjectName
            {
                get { return subjectName; }
                set { value = subjectName; OnPropertyChanged("SubjectName"); }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
