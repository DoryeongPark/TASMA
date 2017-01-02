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

            //ColumnIndex = 0 정수로 변경
                
        }



        /// <summary>
        /// 새 학생 데이터를 만듭니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            dataTable.Rows.Add(new object[] { adminDAO.CurrentClass, adminDAO.CurrentGrade, GetAvailableStudentNumber(), "New Student", "M", null, null });
            StudentDataTable.ItemsSource = dataTable.AsDataView();
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
                if(studentNumber != numberAllocated)
                {
                    return numberAllocated;
                }else
                {
                    ++numberAllocated;
                }
            }

            return numberAllocated;
        }

        
        
    }
}
