using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace TASMA
{
    /// <summary>
    /// SearchPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AdminDAO adminDAO;

        private ObservableCollection<string> gradeComboBoxItems;
        public ObservableCollection<string> GradeComboBoxItems
        {
            get { return gradeComboBoxItems; }
            set { gradeComboBoxItems = value; OnPropertyChanged("GradeComboBoxItems"); }
        }

        private string selectedGradeComboBoxItem;
        public string SelectedGradeComboBoxItem
        {
            get { return selectedGradeComboBoxItem; }
            set { selectedGradeComboBoxItem = value;  OnPropertyChanged("SelectedGradeComboBoxItem"); }
        }

        private ObservableCollection<string> classComboBoxItems;
        public ObservableCollection<string> ClassComboBoxItems
        {
            get { return classComboBoxItems; }
            set { classComboBoxItems = value; OnPropertyChanged("ClassComboBoxItems"); }
        }

        private string selectedClassComboBoxItem;
        public string SelectedClassComboBoxItem
        {
            get { return selectedClassComboBoxItem; }
            set { selectedClassComboBoxItem = value; OnPropertyChanged("SelectedClassComboBoxItem"); }
        }

        private string studentName;
        public string StudentName
        {
            get { return studentName; }
            set { studentName = value; OnPropertyChanged("StudentName"); }
        }

        private DataTable searchTable;
        public DataTable SearchTable
        {
            get { return searchTable;  }
            set { searchTable = value; OnPropertyChanged("SearchTable"); }
        }


        public SearchPage(AdminDAO adminDAO)
        {
            this.adminDAO = adminDAO;
            adminDAO.ReturnToInitialLoginState();

            /* Init GradeComboBox */ 
            var gradeList = adminDAO.GetGradeList();
            gradeComboBoxItems = new ObservableCollection<string>();
            gradeComboBoxItems.Add("----");
            foreach (var gradeName in gradeList)
                gradeComboBoxItems.Add(gradeName);

            selectedGradeComboBoxItem = gradeComboBoxItems[0];

            /* Init ClassComboBox */
            classComboBoxItems = new ObservableCollection<string>();
            classComboBoxItems.Add("----");
            selectedClassComboBoxItem = classComboBoxItems[0];

            searchTable = adminDAO.SearchStudent(null, null, "");
            
            /* Initialize Student */ 
            studentName = "";
           
            DataContext = this;
            InitializeComponent();

            
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SearchPage_GradeComboBox.SelectionChanged += (s, ea) =>
            {
                adminDAO.ReturnToInitialLoginState();

                if (selectedGradeComboBoxItem == "----")
                {
                    ClassComboBoxItems = new ObservableCollection<string>();
                    ClassComboBoxItems.Add("----");
                    SelectedClassComboBoxItem = classComboBoxItems[0];
                    SearchTable = adminDAO.SearchStudent(null, null, studentName);
                }
                else
                {
                    adminDAO.SelectGrade(selectedGradeComboBoxItem);
                    var classList = adminDAO.GetClassList();
                    adminDAO.ReturnToInitialLoginState();
                    ClassComboBoxItems = new ObservableCollection<string>();
                    ClassComboBoxItems.Add("----");
                    foreach (var className in classList)
                        ClassComboBoxItems.Add(className);
                    SelectedClassComboBoxItem = classComboBoxItems[0];

                    SearchTable = adminDAO.SearchStudent(selectedGradeComboBoxItem, null, studentName);
                }
            };

            SearchPage_ClassComboBox.SelectionChanged += (s, ea) =>
            {
                if(selectedClassComboBoxItem != "----")
                {
                    adminDAO.ReturnToInitialLoginState();
                    SearchTable = adminDAO.SearchStudent(selectedGradeComboBoxItem, selectedClassComboBoxItem, studentName);
                }
            };
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string gradeName = null;
            string className = null;
            
            var textBox = sender as TextBox;
            StudentName = textBox.Text;

            if(selectedGradeComboBoxItem != "----")
            {
                gradeName = selectedGradeComboBoxItem;
            }

            if(selectedClassComboBoxItem != "----")
            {
                className = selectedClassComboBoxItem;
            }

            SearchTable = adminDAO.SearchStudent(gradeName, className, StudentName);
        }

        private void OnInfoButtonClicked(object sender, RoutedEventArgs e)
        {
            var row = SearchPage_DataGrid.SelectedIndex;
            var gradeName = SearchTable.Rows[row]["Grade"] as string;
            var className = SearchTable.Rows[row]["Class"] as string;

            adminDAO.SelectGrade(gradeName);
            adminDAO.SelectClass(className);

            TasmaMenuButtons.UpdateButtonsState(TasmaMenuButtons.Buttons[0]);
            NavigationService.Navigate(new StudentPage(adminDAO));
            
        }

        private void OnScoreButtonClicked(object sender, RoutedEventArgs e)
        {
            var row = SearchPage_DataGrid.SelectedIndex;
            var gradeName = SearchTable.Rows[row]["GRADE"] as string;
            var className = SearchTable.Rows[row]["CLASS"] as string;



        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
