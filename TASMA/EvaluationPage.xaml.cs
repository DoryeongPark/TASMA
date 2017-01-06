using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TASMA.Database;
using TASMA.Model;

namespace TASMA
{
    /// <summary>
    /// EvaluationPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EvaluationPage : Page
    {
        private AdminDAO adminDAO;

        private string subjectName;

        ObservableCollection<SubjectTreeViewItem> subjectTreeViewItems
        {
            get; set;
        }
       
        public EvaluationPage(AdminDAO adminDAO, string subjectName)
        {
            this.adminDAO = adminDAO;
            this.subjectName = subjectName;

            subjectTreeViewItems = new ObservableCollection<SubjectTreeViewItem>();
            var gradeList = adminDAO.GetGradeList();
           
            foreach(var grade in gradeList)
            {
                adminDAO.SelectGrade(grade);

                var gradeItem = new SubjectTreeViewItem()
                {
                    Name = grade,
                    Type = SubjectTreeViewItemType.Grade,
                    Checked = false,
                    Children = null,
                    Parent = null
                };

                gradeItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;

                var classList = adminDAO.GetClassList();
                var classItems = new ObservableCollection<SubjectTreeViewItem>();
               
                foreach(var classData in classList)
                {
                    var isRegistered = false;
                    var subjectList = adminDAO.GetClassSubjects(grade, classData);
                    foreach (var subjectData in subjectList)
                        if (subjectData == subjectName) {
                            isRegistered = true;
                            break;
                        }

                    var classItem = new SubjectTreeViewItem()
                    {
                        Name = classData,
                        Type = SubjectTreeViewItemType.Class,
                        Checked = isRegistered,
                        Children = null,
                        Parent = gradeItem
                    };
                    classItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                    classItems.Add(classItem);

                    gradeItem.Children = classItems;
                }

                subjectTreeViewItems.Add(gradeItem);
                adminDAO.MovePrevious();
            }  


            //subjectTreeViewItems = new ObservableCollection<SubjectTreeViewItem>()
            //{
            //    new SubjectTreeViewItem(){Name="Item1", Checked=true,
            //      Children = new ObservableCollection<SubjectTreeViewItem>()
            //      {
            //            new SubjectTreeViewItem(){Name="SubItem11", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem12", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem13", Checked=false}
            //      }
            //    },
            //    new SubjectTreeViewItem(){Name="Item2", Checked=true,
            //        Children = new ObservableCollection<SubjectTreeViewItem>()
            //        {
            //            new SubjectTreeViewItem(){Name="SubItem21", Checked=true},
            //            new SubjectTreeViewItem(){Name="SubItem22", Checked=true},
            //            new SubjectTreeViewItem(){Name="SubItem23", Checked=true}
            //        }
            //    },
            //    new SubjectTreeViewItem(){Name="Item3", Checked=true,
            //          Children = new ObservableCollection<SubjectTreeViewItem>()
            //          {
            //            new SubjectTreeViewItem(){Name="SubItem31", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem32", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem33", Checked=false}
            //          }
            //    },
            //    new SubjectTreeViewItem(){Name="Item4", Checked=true,
            //        Children = new ObservableCollection<SubjectTreeViewItem>()
            //        {
            //            new SubjectTreeViewItem(){Name="SubItem41", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem42", Checked=false},
            //            new SubjectTreeViewItem(){Name="SubItem43", Checked=false}
            //        }
            //    }
            //};

            InitializeComponent();
            
            this.DataContext = subjectTreeViewItems;
                
        }

        private void OnSubjectTreeViewItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SubjectTreeViewItem;

            if(item.Type == SubjectTreeViewItemType.Grade)
            {
                if (item.Checked)
                {
                    foreach (var classItem in item.Children)
                    {
                        classItem.Checked = item.Checked;
                    }
                }
            }else
            {
                if (item.Parent != null)
                    MessageBox.Show(item.Parent.Name);
            }
                
        }

        public void OnClickCheckBox(object sender, RoutedEventArgs e)
        {
            
        }

        public void OnCheckBoxLoaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
