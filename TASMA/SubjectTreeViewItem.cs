using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMA
{
    namespace Model
    {
        public class SubjectTreeViewItem : INotifyPropertyChanged
        {
            private string name;
            private SubjectTreeViewItemType type;
            private bool isChecked;
            private ObservableCollection<SubjectTreeViewItem> children;
            private SubjectTreeViewItem parent;

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged("Name"); }
            }

            public SubjectTreeViewItemType Type
            {
                get { return type; }
                set { type = value; OnPropertyChanged("Type"); }
            }

            public bool IsChecked
            {
                get { return isChecked; }
                set { isChecked = value; OnPropertyChanged("IsChecked"); }
            }

            public SubjectTreeViewItem Parent
            {
                get { return parent; }
                set { parent = value; }
            }
            
            public ObservableCollection<SubjectTreeViewItem> Children
            {
                get { return children; }
                set {
                    children = value;
                    OnPropertyChanged("Children");
                }
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
