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
    public partial class EvaluationPage : Page, INotifyPropertyChanged
    {
        private AdminDAO adminDAO;
        private string subjectName;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<SubjectTreeViewItem> subjectTreeViewItems;
        public ObservableCollection<SubjectTreeViewItem> SubjectTreeViewItems
        {
            get { return subjectTreeViewItems; }
            set { subjectTreeViewItems = value;  OnPropertyChanged("SubjectTreeViewItems"); }
        }

        private ObservableCollection<EvaluationListBoxItem> evaluationListBoxItems;
        public ObservableCollection<EvaluationListBoxItem> EvaluationListBoxItems
        {
            get { return evaluationListBoxItems; }
            set { evaluationListBoxItems = value; OnPropertyChanged("EvaluationListBoxItems"); }
        }

        private EvaluationListBoxItem selectedListBoxItem;
        public EvaluationListBoxItem SelectedListBoxItem
        {
            get { return selectedListBoxItem; }
            set { selectedListBoxItem = value;  OnPropertyChanged("SelectedListBoxItem"); }
        }


        public EvaluationPage(AdminDAO adminDAO, string subjectName)
        {
            
            this.adminDAO = adminDAO;
            this.subjectName = subjectName;

            //트리뷰 데이터 로드(Model View 역할)
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
                    classItems.Add(classItem);

                    gradeItem.Children = classItems;
                }

                subjectTreeViewItems.Add(gradeItem);
                adminDAO.MovePrevious();
            }

            
            //리스트박스 데이터 로드(ModelView 역할)
            var evaluationList = adminDAO.GetEvaluationList(subjectName);
            evaluationListBoxItems = new ObservableCollection<EvaluationListBoxItem>();

            foreach (var evaluationData in evaluationList)
            {
                var evaluationItem = new EvaluationListBoxItem()
                {
                    Name = evaluationData
                };
                evaluationListBoxItems.Add(evaluationItem);
            }

            foreach(var gradeItem in subjectTreeViewItems)
            {
                gradeItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                foreach(var classItem in gradeItem.Children)
                {
                    classItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                }
            }

            foreach(var evaluationItem in evaluationListBoxItems)
            {
                evaluationItem.PropertyChanged += OnEvaluationListBoxItemPropertyChanged;         
            }

            DataContext = this;
            InitializeComponent();
        }

        private void OnEvaluationListBoxItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
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

        private void OnAddListBoxItem(object sender, RoutedEventArgs e)
        {
            var dialog = new TasmaPromptWindow("Add evaluation", "Input evaluation name");
            dialog.ShowDialog();

            if (dialog.IsDetermined)
                selectedListBoxItem.Name = dialog.Input;
        }

        private void OnModifyListBoxItem(object sender, RoutedEventArgs e)
        {
            var dialog = new TasmaPromptWindow("Modify evaluation", "Input evaluation name");
            dialog.ShowDialog();

            if (dialog.IsDetermined)
                selectedListBoxItem.Name = dialog.Input;   
        }

        private void OnDeleteListBoxItem(object sender, RoutedEventArgs e)
        {
            evaluationListBoxItems.Remove(selectedListBoxItem);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));   
        }

    }
}
