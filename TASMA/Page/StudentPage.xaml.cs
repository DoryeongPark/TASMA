using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
using TASMA.MessageBox;

namespace TASMA.Pages
{
    /// <summary>
    /// 학생 정보 페이지 입니다.
    /// </summary>
    public partial class StudentPage : Page
    {
        private AdminDAO adminDAO;

        private DataTable dataTable;

        private long preSelectedSnum = -1;
        
        public StudentPage(AdminDAO adminDAO, long sNum = -1)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;

            this.preSelectedSnum = sNum;

            StudentPage_Class.Text = "GRADE: " + adminDAO.CurrentGrade + "  CLASS: " + adminDAO.CurrentClass;

            StudentPage_PreviousButton.Click += OnPreviousButtonClicked;
            StudentPage_SortButton.Click += OnSortButtonClicked;
        }

        /// <summary>
        /// 학생 테이블을 이름으로 오름차순 정렬한 뒤 새 학생 번호를 부여합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSortButtonClicked(object sender, RoutedEventArgs e)
        {
            var confirm = new TasmaConfirmationMessageBox("Auto number allocation", "Are you sure execute auto number allocation?");
            confirm.ShowDialog();
            if (confirm.Yes == false)
                return;

            dataTable = adminDAO.GetStudentDataTable(StudentTableOption.AscByName);

            int tempSnum = 0;

            for (int i = 0; i < dataTable.Rows.Count; ++i)
                adminDAO.UpdateStudent(((int)(long)dataTable.Rows[i][2]), --tempSnum);
            
            dataTable = adminDAO.GetStudentDataTable(StudentTableOption.DescByNumber);
           
            int newSnum = 0;

            for (int i = 0; i < dataTable.Rows.Count; ++i)
                adminDAO.UpdateStudent(((int)(long)dataTable.Rows[i][2]), ++newSnum);

            dataTable = adminDAO.GetStudentDataTable(StudentTableOption.AscByNumber);
            
            StudentDataTable.ItemsSource = dataTable.AsDataView();
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            adminDAO.MovePrevious();
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new ClassPage(adminDAO));
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            dataTable = adminDAO.GetStudentDataTable(StudentTableOption.AscByNumber);
            dataTable.AcceptChanges();

            var comboBoxColumn = StudentDataTable.Columns[2] as DataGridComboBoxColumn;
            var items = new ArrayList();
            items.Add("M");
            items.Add("F");
            comboBoxColumn.ItemsSource = items;

            StudentDataTable.PreviewKeyDown += OnPreviewKeyDown;
            StudentDataTable.CellEditEnding += OnCellEditFinished;
        
            StudentDataTable.ItemsSource = dataTable.AsDataView();

            if(dataTable.Rows.Count == 0)
            {
                dataTable.Rows.Add(
                    new object[] { adminDAO.CurrentGrade, adminDAO.CurrentClass, GetAvailableStudentNumber(), "New Student", "M", null, null });
                StudentDataTable.ItemsSource = dataTable.AsDataView();
            }

            if (preSelectedSnum != -1) {
                for (int i = 0; i < dataTable.Rows.Count; ++i) {
                    if ((long)dataTable.Rows[i]["SNUM"] == preSelectedSnum) {
                        StudentDataTable.Focus();
                        StudentDataTable.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 셀 편집이 끝났을 때 호출되는 로직입니다. 데이터 유효성을 검사한 뒤 실제 데이터베이스에 반영합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellEditFinished(object sender, DataGridCellEditEndingEventArgs e)
        {
            string valueEdited = "";

            var textBox = e.EditingElement as TextBox;
            var rowIndex = e.Row.GetIndex();
            var columnIndex = e.Column.DisplayIndex;
            
            if (textBox == null)
            {
                var comboBox = e.EditingElement as ComboBox;
                valueEdited = comboBox.Text;
            }
            else
            {
                valueEdited = textBox.Text;
            }

            //수정한 데이터가 학생 번호인 경우
            if (columnIndex == 0)
            {
                //Exception - 번호 외 다른 입력
                try
                {
                    var studentNumberEdited = long.Parse(valueEdited);

                    for (int i = 0; i < dataTable.Rows.Count; ++i)
                    {
                        if (i == rowIndex)
                            continue;

                        var studentNumber = (long)dataTable.Rows[i][2];
                        if (studentNumberEdited == studentNumber)
                        {
                            var alert = new TasmaAlertMessageBox("Duplication", "Student numbers are duplicated");
                            alert.ShowDialog();
                            textBox.Text = dataTable.Rows[rowIndex][2].ToString();
                            return;
                        }
                    }
                    dataTable.Rows[rowIndex][columnIndex + 2] = studentNumberEdited;

                }
                catch
                {
                    var alert = new TasmaAlertMessageBox("Input number", "You should input number");
                    alert.ShowDialog();
                    textBox.Text = GetAvailableStudentNumber().ToString();
                    return;
                }
            }else //수정한 데이터가 학생 번호가 아닐 경우
            {
                dataTable.Rows[rowIndex][columnIndex + 2] = valueEdited;
            }

            ReflectDataTable();                
        }

        /// <summary>
        /// 키보드 입력에 대한 로직 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            //학생 데이터 삽입
            if (e.Key == Key.Down && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                dataTable.Rows.Add(
                    new object[] { adminDAO.CurrentGrade, adminDAO.CurrentClass, GetAvailableStudentNumber(), "New Student", "M", null, null });
                StudentDataTable.ItemsSource = dataTable.AsDataView();
                StudentDataTable.CurrentCell = new DataGridCellInfo(StudentDataTable.Items[StudentDataTable.Items.Count - 1], StudentDataTable.Columns[1]);
                StudentDataTable.BeginEdit();
                ReflectDataTable();
            }

            //학생 데이터 삭제
            if (e.Key == Key.Up && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var selectedIndex = StudentDataTable.SelectedIndex;
                if (selectedIndex == -1)
                    return;
 
                var drv = (DataRowView)StudentDataTable.SelectedItem;
                var selectedStudentNumber = (long)drv["SNUM"];
                var selectedStudentName = drv["SNAME"];

                var confirm = new TasmaConfirmationMessageBox("Delete Student", "Are you sure delete student? - " + selectedStudentName);
                confirm.ShowDialog();

                if (confirm.Yes != true)
                    return;
                 
                for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
                {
                    var dr = dataTable.Rows[i];
                    var studentNumber = (long)dr["SNUM"];

                    if (studentNumber == selectedStudentNumber)
                        dr.Delete();                                            
                }

                StudentDataTable.ItemsSource = dataTable.AsDataView();
                ReflectDataTable();
               
            }

        }


        /// <summary>
        /// 새 학생번호를 계산하여 반환합니다.
        /// </summary>
        /// <returns>새 학생번호</returns>
        private int GetAvailableStudentNumber()
        {
            var numberAllocated = 1;

            for (int i = 0; i < dataTable.Rows.Count; ++i)
            {
                var studentNumber = (long)dataTable.Rows[i][2];
                if (studentNumber != numberAllocated)
                {
                    return numberAllocated;
                } else
                {
                    ++numberAllocated;
                }
            }

            return numberAllocated;
        }

        /// <summary>
        /// 현재 데이터 테이블의 상태를 실제 데이터베이스에 반영합니다.
        /// </summary>
        private void ReflectDataTable()
        {
            adminDAO.UpdateStudentDataTable(dataTable);
        }
        
    }
}
