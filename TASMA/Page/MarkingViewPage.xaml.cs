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
using TASMA.Model;

namespace TASMA
{
    /// <summary>
    /// 반 채점 테이블 페이지 입니다.
    /// </summary>
    public partial class MarkingViewPage : Page, INotifyPropertyChanged
    {
        private AdminDAO adminDAO;

        private DataTable scoreTable;
        public DataTable ScoreTable
        {
            get { return scoreTable; }
            set { scoreTable = value; OnPropertyChanged("ScoreTable"); }
        }

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
            set { selectedGradeComboBoxItem = value; OnPropertyChanged("SelectedGradeComboBoxItem"); }
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

        private ObservableCollection<string> subjectComboBoxItems;
        public ObservableCollection<string> SubjectComboBoxItems
        {
            get { return subjectComboBoxItems; }
            set { subjectComboBoxItems = value; OnPropertyChanged("SubjectComboBoxItems"); }   
        }

        private string selectedSubjectComboBoxItem;
        public string SelectedSubjectComboBoxItem
        {
            get { return selectedSubjectComboBoxItem; }
            set { selectedSubjectComboBoxItem = value; OnPropertyChanged("SelectedSubjectComboBoxItem"); }
        }

        private ObservableCollection<KeyValuePair<int, string>> semesterComboBoxItems;
        public ObservableCollection<KeyValuePair<int, string>> SemesterComboBoxItems
        {
            get { return semesterComboBoxItems; }
            set { semesterComboBoxItems = value; OnPropertyChanged("SemesterComboBoxItems"); }
        }

        private KeyValuePair<int, string> selectedSemesterComboBoxItem;
        public KeyValuePair<int, string> SelectedSemesterComboBoxItem
        {
            get { return selectedSemesterComboBoxItem; }
            set { selectedSemesterComboBoxItem = value; OnPropertyChanged("SelectedSemesterComboBoxItem"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 현재 선택된 반의 ViewModel을 초기화합니다.
        /// </summary>
        /// <param name="adminDAO"></param>
        /// <param name="gradeName"></param>
        /// <param name="className"></param>
        /// <param name="subjectName"></param>
        public MarkingViewPage(AdminDAO adminDAO, string currentGradeName = null, string currentClassName = null)
        {
            
            this.adminDAO = adminDAO;
            adminDAO.ReturnToInitialState();

            gradeComboBoxItems = new ObservableCollection<string>();
            var gradeList = adminDAO.GetGradeList();

            foreach(var gradeName in gradeList)
            {
                gradeComboBoxItems.Add(gradeName);
            }

            classComboBoxItems = new ObservableCollection<string>();
            subjectComboBoxItems = new ObservableCollection<string>();
            
            semesterComboBoxItems = new ObservableCollection<KeyValuePair<int, string>>();
            semesterComboBoxItems.Add(new KeyValuePair<int, string>(0, "1st"));
            semesterComboBoxItems.Add(new KeyValuePair<int, string>(1, "2nd"));

            if (DateTime.Today.Month <= 6)
                selectedSemesterComboBoxItem = semesterComboBoxItems[0];
            else
                selectedSemesterComboBoxItem = semesterComboBoxItems[1];

            //Add Event
            scoreTable = new DataTable(); 
            scoreTable.ColumnChanged += OnValueChanged;
            
            DataContext = this;
            InitializeComponent();

            if(currentGradeName != null &&
               currentClassName != null)
            {
                SelectedGradeComboBoxItem = currentGradeName;
                OnGradeComboBoxSelectionChanged(null, null);
                SelectedClassComboBoxItem = currentClassName;
                OnClassComboBoxSelectionChanged(null, null);
                adminDAO.ReturnToInitialState();
                var subjectList = adminDAO.GetClassSubjects(SelectedGradeComboBoxItem,
                                                            SelectedClassComboBoxItem);
                if (subjectList == null || subjectList.Count == 0)
                    return;
                SelectedSubjectComboBoxItem = subjectList[0];
                OnSubjectComboBoxSelectionChanged(null, null);
            }
        }
        
        /// <summary>
        /// ComboBox에 이벤트를 추가합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MarkingViewPage_SubjectComboBox.SelectionChanged += OnSubjectComboBoxSelectionChanged;
            MarkingViewPage_SemesterComboBox.SelectionChanged += OnSemesterComboBoxSelected;
        }

       
        /// <summary>
        /// 셀 편집 종료 시의 호출 루틴입니다. 데이터의 유효성을 검사합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellEditFinished(object sender, DataGridCellEditEndingEventArgs e)
        { 
            var textBox = e.EditingElement as TextBox;
            var rowIndex = e.Row.GetIndex();
            var columnIndex = e.Column.DisplayIndex;
            var newValueText = textBox.Text;
            float? oldValue = null;

            if (scoreTable.Rows[rowIndex][columnIndex] != DBNull.Value)
            { 
                oldValue = (float)scoreTable.Rows[rowIndex][columnIndex];
            }

            float newValue;

            if(newValueText == "")
            {
                textBox.Text = "";
                scoreTable.Rows[rowIndex][columnIndex] = DBNull.Value;
                return;
            }

            try {

                newValue = float.Parse(newValueText);

            } catch
            {
                if (oldValue == null)
                {
                    textBox.Text = "";
                    scoreTable.Rows[rowIndex][columnIndex] = DBNull.Value;
                }
                else
                    textBox.Text = oldValue.ToString();

                return;
            }
                  
            if (0 > newValue)
            {
                textBox.Text = (-1.0f * float.Parse(newValueText)).ToString();
            }
           
        }

        /// <summary>
        /// 셀 편집을 통해 바뀐 데이터를 실제 데이터베이스에 반영합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, DataColumnChangeEventArgs e)
        {
            var columnName = e.Column.ColumnName;
            var sNum = (int)e.Row[0];
            float? newValue;

            if (e.Row[columnName] == DBNull.Value)
            {
                newValue = null;
            }
            else
            {
                newValue = (float)e.Row[columnName];
            }

            adminDAO.SelectGrade(SelectedGradeComboBoxItem);
            adminDAO.SelectClass(SelectedClassComboBoxItem);
            adminDAO.UpdateScore(SelectedSubjectComboBoxItem,
                                SelectedSemesterComboBoxItem.Key, 
                                sNum, 
                                columnName, 
                                newValue);
            adminDAO.ReturnToInitialState();          
        }


        /// <summary>
        /// 컬럼 생성 시 호출되는 루틴입니다. 컬럼 헤더에 대한 설정을 초기화합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGenerateColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;

            if (column != null)
            {
                Style elementStyle = new Style(typeof(TextBlock));
                elementStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow));
                column.ElementStyle = elementStyle;

                column.CanUserResize = false;
                column.CanUserSort = false;

                if (e.PropertyName == "No")
                {
                    column.IsReadOnly = true;
                    column.Width = new DataGridLength(25);
                }
                else if (e.PropertyName == "Student name")
                {
                    column.IsReadOnly = true;
                    column.Width = new DataGridLength(200);
                }
                else
                {
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                }
            }
        }


        private void OnGradeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClassComboBoxItems.Clear();
            SubjectComboBoxItems.Clear();
            adminDAO.ReturnToInitialState();
            adminDAO.SelectGrade(selectedGradeComboBoxItem);
            var classList = adminDAO.GetClassList();
            foreach(var className in classList)
            {
                ClassComboBoxItems.Add(className);
            }
            SelectedClassComboBoxItem = null;
            SelectedSubjectComboBoxItem = null;

            adminDAO.ReturnToInitialState();
        }

        private void OnClassComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SubjectComboBoxItems.Clear();
            var subjectList = adminDAO.GetClassSubjects(SelectedGradeComboBoxItem, SelectedClassComboBoxItem);
            foreach(var subjectName in subjectList)
            {
                SubjectComboBoxItems.Add(subjectName);
            }
            SelectedSubjectComboBoxItem = null;
        }

        /// <summary>
        /// 과목 콤보 박스 선택 시 호출되는 루틴입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubjectComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedClassComboBoxItem == null || 
                SelectedGradeComboBoxItem == null ||
                SelectedSubjectComboBoxItem == null)
                return;

            adminDAO.ReturnToInitialState();

            var dataTable = new DataTable();
           
            adminDAO.SelectGrade(SelectedGradeComboBoxItem);
            adminDAO.SelectClass(SelectedClassComboBoxItem);
            var evaluationList = adminDAO.GetEvaluationList(SelectedSubjectComboBoxItem);
            
            /* Column 추가 */
            dataTable.Columns.Add("No", typeof(int));
            dataTable.Columns.Add("Student name", typeof(string));
            foreach(var evaluationName in evaluationList)
            {
                dataTable.Columns.Add(evaluationName, typeof(float));
            }

            /* Data 삽입 */
            var studentList = adminDAO.GetStudentList();
            foreach(var studentData in studentList)
            {
                dataTable.Rows.Add(new object[]{ studentData.Item1, studentData.Item2 });
            }

            var studentListFromSubject = adminDAO.GetStudentNumberFromSubject(SelectedSubjectComboBoxItem);
            foreach(var studentData in studentList)
            {
                if (!studentListFromSubject.Contains(studentData.Item1))
                {
                    adminDAO.CreateStudentInSubject(SelectedSubjectComboBoxItem, studentData.Item1);
                }
            }

            var scoreDataTable = adminDAO.GetScoreTable(SelectedSubjectComboBoxItem, SelectedSemesterComboBoxItem.Key);
            for(int i = 0; i < dataTable.Rows.Count; ++i)
            {
                for(int j = 2; j < dataTable.Columns.Count; ++j)
                {
                    dataTable.Rows[i][j] = scoreDataTable.Rows[i][j - 1];
                }
            }

            ScoreTable = dataTable;
            ScoreTable.ColumnChanged += OnValueChanged;
            adminDAO.ReturnToInitialState();
        }

        /// <summary>
        /// 학기 콤보 박스 선택 시 호출되는 루틴입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSemesterComboBoxSelected(object sender, SelectionChangedEventArgs e)
        {
            OnSubjectComboBoxSelectionChanged(sender, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
