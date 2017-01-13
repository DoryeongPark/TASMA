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

        private string gradeName;
        private string className;
        private string subjectName;

        private DataTable scoreTable;
        public DataTable ScoreTable
        {
            get { return scoreTable; }
            set { scoreTable = value; OnPropertyChanged("ScoreTable"); }
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

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 현재 선택된 반의 ViewModel을 초기화합니다.
        /// </summary>
        /// <param name="adminDAO"></param>
        /// <param name="gradeName"></param>
        /// <param name="className"></param>
        /// <param name="subjectName"></param>
        public MarkingViewPage(AdminDAO adminDAO, string gradeName, string className, string subjectName)
        {
            this.adminDAO = adminDAO;
            this.gradeName = gradeName;
            this.className = className;
            this.subjectName = subjectName;

            adminDAO.SelectClass(gradeName);
            adminDAO.SelectClass(className);
            var studentList = adminDAO.GetStudentList();
            var studentListFromSubject = adminDAO.GetStudentNumberFromSubject(subjectName);

            //Table creation routine
            scoreTable = new DataTable();
            scoreTable.Columns.Add("No", typeof(int));
            scoreTable.Columns.Add("Student name", typeof(string));

            foreach (var student in studentList)
            {
                var row = scoreTable.NewRow();
                row["No"] = student.Item1;
                row["Student name"] = student.Item2;
                scoreTable.Rows.Add(row);
            }

            //과목 테이블에서 학생 정보를 가져온 뒤 비교한다.
            //Update와 Delete는 자동으로 되므로 새 학생 데이터를 과목 테이블에 삽입만 하면 된다.
            foreach (var student in studentList)
                if (!studentListFromSubject.Contains(student.Item1))
                    adminDAO.CreateStudentInSubject(subjectName, student.Item1);

            var evaluationList = adminDAO.GetEvaluationList(subjectName);
            foreach (var evaluationName in evaluationList)
                scoreTable.Columns.Add(evaluationName, typeof(float));

            var scoreTableFromSubject = adminDAO.GetScoreTable(subjectName);
            for (var i = 0; i < scoreTable.Rows.Count; ++i)
            {
                var rowFound = scoreTableFromSubject.Select("SNUM = " + (int)scoreTable.Rows[i]["No"]);
                for (var j = 2; j < scoreTable.Columns.Count; ++j)
                    scoreTable.Rows[i][j] = rowFound[0][j - 1];
            }

            for (var j = 2; j < scoreTable.Columns.Count; ++j)
                scoreTable.Columns[2].AllowDBNull = true;

            //ComboBox creation routine
            var subjectList = adminDAO.GetClassSubjects(gradeName, className);
            subjectComboBoxItems = new ObservableCollection<string>();

            foreach (var subject in subjectList)
                subjectComboBoxItems.Add(subject);
            
            foreach (var subjectComboBoxItem in subjectComboBoxItems)
                if (subjectName == subjectComboBoxItem)
                {
                    selectedSubjectComboBoxItem = subjectComboBoxItem;
                    break;
                }

            scoreTable.ColumnChanged += OnValueChanged;
            scoreTable.AcceptChanges();

            DataContext = this;
            InitializeComponent();
        }
        
        /// <summary>
        /// ComboBox에 이벤트를 추가합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MarkingViewPage_ComboBox.SelectionChanged += OnSubjectComboBoxSelected;
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
            
            adminDAO.UpdateScore(subjectName, sNum, columnName, newValue);              
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

        /// <summary>
        /// 이전 반 선택 페이지로 돌아갑니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MarkingPage(adminDAO, gradeName, className, subjectName));
        }

        /// <summary>
        /// 콤보 박스 선택 시 호출되는 루틴입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubjectComboBoxSelected(object sender, SelectionChangedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new MarkingViewPage(adminDAO, gradeName, className, selectedSubjectComboBoxItem));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
