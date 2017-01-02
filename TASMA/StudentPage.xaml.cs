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

namespace TASMA
{
    /// <summary>
    /// StudentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StudentPage : Page
    {
        private AdminDAO adminDAO;

        private DataTable dataTable;

        private bool isEditing = false;
        
        public StudentPage(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            dataTable = adminDAO.GetStudentDataTable();

            var comboBoxColumn = StudentDataTable.Columns[2] as DataGridComboBoxColumn;
            var items = new ArrayList();
            items.Add("M");
            items.Add("F");
            comboBoxColumn.ItemsSource = items;

            StudentDataTable.PreviewKeyDown += OnPreviewKeyDown;
            StudentDataTable.CellEditEnding += OnCellEditEnding;
        
            StudentDataTable.ItemsSource = dataTable.AsDataView();
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string valueEdited = "";

            var textBox = e.EditingElement as TextBox;
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

            if (columnIndex == 0)
            {
                //Exception - 번호 외 다른 입력
                try
                {
                    var studentNumberEdited = long.Parse(valueEdited);

                    for (int i = 0; i < dataTable.Rows.Count; ++i)
                    {
                        var studentNumber = (long)dataTable.Rows[i][2];
                        if (studentNumberEdited == studentNumber)
                        {
                            MessageBox.Show("Student numbers are duplicated");
                            textBox.Text = GetAvailableStudentNumber().ToString();
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("You should input number");
                    textBox.Text = GetAvailableStudentNumber().ToString();
                    return;
                }
            }
                
        }

        /// <summary>
        /// 데이터 조작에 대한 키보드 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                dataTable.Rows.Add(
                    new object[] { adminDAO.CurrentClass, adminDAO.CurrentGrade, GetAvailableStudentNumber(), "New Student", "M", null, null });
                StudentDataTable.ItemsSource = dataTable.AsDataView();
                StudentDataTable.CurrentCell = new DataGridCellInfo(StudentDataTable.Items[StudentDataTable.Items.Count - 1], StudentDataTable.Columns[3]);
                StudentDataTable.BeginEdit();
                ReflectDataTable();
            }

            if (e.Key == Key.Up)
            {
                var selectedIndex = StudentDataTable.SelectedIndex;
                if (selectedIndex == -1)
                    return;
 
                var drv = (DataRowView)StudentDataTable.SelectedItem;
                var selectedStudentNumber = (long)drv["SNUM"];
                var selectedStudentName = drv["SNAME"];

                var messageBoxResult = MessageBox.Show("Are you sure delete student? - " + selectedStudentName, "Delete Student", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
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
