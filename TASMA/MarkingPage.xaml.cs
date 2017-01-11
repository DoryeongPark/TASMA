using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 점수 입력 페이지입니다.
    /// </summary>
    public partial class MarkingPage : Page
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

        private Grade selectedGradeItem;
        public Grade SelectedGradeItem
        {
            get { return selectedGradeItem; }
            set { selectedGradeItem = value; }
        }

        private Class selectedClassItem;
        public Class SelectedClassItem
        {
            get { return selectedClassItem; }
            set { selectedClassItem = value; }
        }

        private Subject selectedSubjectItem;
        public Subject SelectedSubjectItem
        {
            get { return selectedSubjectItem; }
            set { selectedSubjectItem = value; }
        }

        public MarkingPage(AdminDAO adminDAO)
        {
            this.adminDAO = adminDAO;
            adminDAO.ReturnToInitialLoginState();
            var gradeList = adminDAO.GetGradeList();

            gradeListBoxItems = new ObservableCollection<Grade>();
            classListBoxItems = new ObservableCollection<Class>();
            subjectListBoxItems = new ObservableCollection<Subject>();
            
            foreach(var gradeName in gradeList)
                gradeListBoxItems.Add(new Grade(){ GradeName = gradeName });
            
            DataContext = this;
            InitializeComponent();
        }

        private void OnGradeItemClicked(object sender, MouseButtonEventArgs e)
        {
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

        private void OnClassItemClicked(object sender, MouseButtonEventArgs e)
        {
            subjectListBoxItems.Clear();

            adminDAO.SelectClass(selectedClassItem.ClassName);
            var subjectList = adminDAO.GetClassSubjects(selectedGradeItem.GradeName, selectedClassItem.ClassName);
            foreach(var subjectName in subjectList)
            {
                subjectListBoxItems.Add(new Subject { SubjectName = subjectName });
            }
        }

        private void OnSubjectItemClicked(object sender, MouseButtonEventArgs e)
        {
            var nav = NavigationService.GetNavigationService(this);
            //nav.Navigate(new MarkingViewPage(adminDAO, 
            //                                selectedGradeItem.GradeName, 
            //                                selectedClassItem.ClassName, 
            //                                selectedSubjectItem.SubjectName));
        }
    }
}
