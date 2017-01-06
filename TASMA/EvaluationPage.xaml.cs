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
                    IsChecked = false,
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
                        IsChecked = isRegistered,
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

            InitializeComponent();
            
            this.DataContext = subjectTreeViewItems;
                
        }

        private void OnSubjectTreeViewItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SubjectTreeViewItem;

            if(item.Type == SubjectTreeViewItemType.Grade)
            {
                if (item.IsChecked)
                {
                    foreach (var classItem in item.Children)
                    {
                        classItem.IsChecked = item.IsChecked;
                    }
                }else
                {
                    var areChildrenChecked = true;

                    foreach (var child in item.Children)
                    {
                        if (!child.IsChecked)
                        {
                            areChildrenChecked = false;
                            break;
                        }
                    }

                    if (areChildrenChecked)
                    {
                        foreach(var classItem in item.Children)
                        {
                            classItem.IsChecked = false;
                        }
                    }
                }
            }else
            {
                if (item.IsChecked)
                {

                }else
                {
                    if (item.Parent != null && item.Parent.IsChecked)
                    {
                        item.Parent.IsChecked = false;
                    } 
                }
            }
        }

        

    }
}
