using System;
using System.Collections.Generic;
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
    /// 학생 채점 테이블 페이지 입니다.
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

        public event PropertyChangedEventHandler PropertyChanged;

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

            scoreTable = new DataTable();
            scoreTable.Columns.Add("Number", typeof(int));
            scoreTable.Columns.Add("Student name", typeof(string));
            
            foreach (var student in studentList)
            {
                var row = scoreTable.NewRow();
                row["Number"] = student.Item1;
                row["Student name"] = student.Item2;
                scoreTable.Rows.Add(row);
            }

            //과목 테이블에서 학생 정보를 가져온 뒤 비교한다.
            //Update와 Delete는 자동으로 되므로 새 학생 데이터를 과목 테이블에 삽입만 하면 된다.
            foreach (var student in studentList)
                if (!studentListFromSubject.Contains(student.Item1))
                    adminDAO.CreateStudentInSubject(subjectName, student.Item1);
            
            
           
            
            DataContext = this;
            InitializeComponent();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnGenerateColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.CanUserResize = false;
            e.Column.CanUserSort = false;

            if (e.PropertyName == "Number")
            {
                e.Column.IsReadOnly = true;
                e.Column.Width = new DataGridLength(25);
            }else if(e.PropertyName == "Student name")
            {
                e.Column.IsReadOnly = true;
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);               
            }
            else
            {
                e.Column.Width = new DataGridLength(80, DataGridLengthUnitType.Auto);
            }
        }
    }
}
