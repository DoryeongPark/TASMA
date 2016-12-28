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
       
        /// <summary>
        /// 현재 학년을 조회하는 페이지를 생성합니다.
        /// </summary>
        /// <param name="adminDAO"></param>
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

        /// <summary>
        /// 데이터베이스의 상태를 페이지에 반영합니다
        /// </summary>
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

        /// <summary>
        /// 변경할 데이터가 다른 데이터와 중복되는지 확인합니다. 
        /// </summary>
        /// <param name="newData">변경할 데이터</param>
        /// <returns>변경 가능 여부</returns>
        private bool OnCheckModificationPossible(string newData)
        {
            foreach (var str in gradeList)
                if (str == newData)
                    return false;
                   
            return true;
        }

        /// <summary>
        /// 데이터를 변경하고 인터페이스에 반영합니다.
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateGrade(oldData, newData);
            Invalidate();
        }

        /// <summary>
        /// 데이터를 지우고 인터페이스에 반영합니다.
        /// </summary>
        /// <param name="sender">데이터 박스 객체</param>
        /// <param name="e">NULL</param>
        private void OnDeleteData(object sender, EventArgs e)
        {
            var dataRect = sender as DataRectangle;
            adminDAO.DeleteGrade(dataRect.Data);
            Invalidate();           
        }
    }
}
