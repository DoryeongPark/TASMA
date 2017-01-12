using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// 점수 입력 페이지입니다.
    /// </summary>
    public partial class MarkingPage : Page, INotifyPropertyChanged
    {
        private AdminDAO adminDAO;

        private ObservableCollection<Grade> gradeListBoxItems;
        public ObservableCollection<Grade> GradeListBoxItems
        {
            get { return gradeListBoxItems; }
            set { gradeListBoxItems = value; }
        }

        private ObservableCollection<Class> classListBoxItems;
        public ObservableCollection<Class> ClassListBoxItems
        {
            get { return classListBoxItems; }
            set { classListBoxItems = value; }
        }

        private ObservableCollection<Subject> subjectListBoxItems;
        public ObservableCollection<Subject> SubjectListBoxItems
        {
            get { return subjectListBoxItems; }
            set { subjectListBoxItems = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Grade selectedGradeItem;
        public Grade SelectedGradeItem
        {
            get { return selectedGradeItem; }
            set { selectedGradeItem = value; OnPropertyChanged("SelectedGradeItem"); }
        }

        private Class selectedClassItem;
        public Class SelectedClassItem
        {
            get { return selectedClassItem; }
            set { selectedClassItem = value;  OnPropertyChanged("SelectedClassItem"); }
        }

        private Subject selectedSubjectItem;
        public Subject SelectedSubjectItem
        {
            get { return selectedSubjectItem; }
            set { selectedSubjectItem = value; OnPropertyChanged("SelectedSubjectItem"); }
        }

        public MarkingPage(AdminDAO adminDAO)
        {
            this.adminDAO = adminDAO;
            InitializeRoutine();
        }
    
        public MarkingPage(AdminDAO adminDAO, string gradeName, string className, string subjectName)
        {
            this.adminDAO = adminDAO;
            InitializeRoutine();

            foreach (var gradeItem in gradeListBoxItems)
                if (gradeItem.GradeName == gradeName)
                {
                    SelectedGradeItem = gradeItem;
                    break;
                }
            
            foreach(var classItem in classListBoxItems)
                if(classItem.ClassName == className)
                {
                    SelectedClassItem = classItem;
                    break;
                }
            
        }

        private void InitializeRoutine()
        {
            adminDAO.ReturnToInitialLoginState();
            var gradeList = adminDAO.GetGradeList();

            gradeListBoxItems = new ObservableCollection<Grade>();
            classListBoxItems = new ObservableCollection<Class>();
            subjectListBoxItems = new ObservableCollection<Subject>();

            foreach (var gradeName in gradeList)
                gradeListBoxItems.Add(new Grade() { GradeName = gradeName });

            DataContext = this;
            InitializeComponent();
        }

        private void OnGradeItemClicked(object sender, SelectionChangedEventArgs e)
        {
            if (selectedGradeItem == null)
                return;

            adminDAO.ReturnToInitialLoginState();
            classListBoxItems.Clear();
            subjectListBoxItems.Clear();

            adminDAO.SelectGrade(selectedGradeItem.GradeName);
            var classList = adminDAO.GetClassList(); 
            foreach(var className in classList)
            {
                classListBoxItems.Add(new Class() { ClassName = className });
            }          
        }

        private void OnClassItemClicked(object sender, SelectionChangedEventArgs e)
        {         
            if (selectedClassItem == null)
                return;

            subjectListBoxItems.Clear();

            adminDAO.SelectClass(selectedClassItem.ClassName);
            var subjectList = adminDAO.GetClassSubjects(selectedGradeItem.GradeName, selectedClassItem.ClassName);
            foreach(var subjectName in subjectList)
            {
                subjectListBoxItems.Add(new Subject { SubjectName = subjectName });
            }
        }

        private void OnSubjectItemClicked(object sender, SelectionChangedEventArgs e)
        {

            if (selectedSubjectItem == null)
                return;

            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MarkingViewPage(adminDAO,
                                            selectedGradeItem.GradeName,
                                            selectedClassItem.ClassName,
                                            selectedSubjectItem.SubjectName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
