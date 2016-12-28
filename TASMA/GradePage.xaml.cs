using System;
using System.Collections.Generic;
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
using TASMA.DataInterfaces;

namespace TASMA
{
    /// <summary>
    /// Grade 리스트
    /// </summary>
    public partial class GradePage : Page
    {
        private AdminDAO adminDAO;

        private List<StackPanel> columns;
        private List<string> gradeList;
       
        public GradePage(AdminDAO adminDAO)
        {
            InitializeComponent();

            this.adminDAO = adminDAO;
            columns = new List<StackPanel>();
            columns.Add(GradePage_Column0);
            columns.Add(GradePage_Column1);
            columns.Add(GradePage_Column2);

            Invalidate();
        }

        private void Invalidate()
        {
            foreach (var column in columns)
                column.Children.Clear();
                
            var columnIndex = 0;
            gradeList = adminDAO.GetGradeList();

            foreach (var data in gradeList)
            {
                var dataRect = new DataRectangle(data);

                //이벤트 등록
                dataRect.OnCheckModificationPossible += OnCheckModificationPossible;
                dataRect.OnModificationComplete += OnModificationComplete;
                dataRect.OnDeleteData += OnDeleteData;

                //데이터 박스 추가
                columns[columnIndex].Children.Add(dataRect);
                if (++columnIndex == 3)
                    columnIndex = 0;
            }
        }

        private bool OnCheckModificationPossible(string newData)
        {
            foreach (var str in gradeList)
                if (str == newData)
                    return false;
                   
            return true;
        }

        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateGrade(oldData, newData);
            Invalidate();
        }

        private void OnDeleteData(object sender, EventArgs e)
        {
            var dataRect = sender as DataRectangle;
            adminDAO.DeleteGrade(dataRect.Data);
            Invalidate();           
        }
    }
}
