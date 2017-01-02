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
    /// 반에 대한 페이지 입니다.
    /// </summary>
    public partial class ClassPage : Page
    {
        private AdminDAO adminDAO;
        private List<StackPanel> columns;
        private List<string> classList;
        
        private int columnIndex;

        public ClassPage(AdminDAO adminDAO)
        {
            InitializeComponent();

            this.adminDAO = adminDAO;

            ClassPage_Class.Content = "GRADE " + adminDAO.CurrentGrade + " - CLASSES";

            columns = new List<StackPanel>();
            columns.Add(ClassPage_Column0);
            columns.Add(ClassPage_Column1);
            columns.Add(ClassPage_Column2);

            ClassPage_PreviousButton.Click += OnPreviousButtonClicked;
            ClassPage_AddButton.Click += OnAddButtonClicked;

            Invalidate();
        }

        /// <summary>
        /// 데이터베이스의 상태를 페이지에 반영합니다.
        /// </summary>
        private void Invalidate()
        {
            classList = adminDAO.GetClassList();

            //모든 데이터 박스 제거
            foreach (var column in columns)
                column.Children.Clear();

            columnIndex = 0;
            classList = adminDAO.GetClassList();

            foreach (var data in classList)
            {
                var dataRect = new DataRectangle(data);

                //이벤트 등록
                dataRect.OnCheckDuplication += OnCheckDuplication;
                dataRect.OnModificationComplete += OnModificationComplete;
                dataRect.OnDeleteData += OnDeleteData;
                dataRect.MouseLeftButtonUp += OnClickClass;

                //데이터 박스 추가
                columns[columnIndex].Children.Add(dataRect);
                if (++columnIndex == 3)
                    columnIndex = 0;
            }
        }

        /// <summary>
        /// 변경할 반 데이터가 다른 데이터와 중복되는지 확인합니다
        /// </summary>
        /// <param name="newData">변경할 데이터</param>
        /// <returns>중복 여부</returns>
        private bool OnCheckDuplication(string newData)
        {
            foreach (var data in classList)
                if (data == newData)
                    return true;

            return false;
        }

        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateClass(oldData, newData);
            Invalidate();
        }

        private void OnDeleteData(object sender, EventArgs e)
        {
            var dataRect = sender as DataRectangle;
            adminDAO.DeleteClass(dataRect.Data);
            Invalidate();
        }

        private void OnClickClass(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var classSelected = (sender as DataRectangle).Data;
            adminDAO.SelectClass(classSelected);
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StudentPage(adminDAO));
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            var promptWindow = new TasmaPromptWindow("Create class", "Please input class name");
            promptWindow.ShowDialog();

            if (promptWindow.IsDetermined)
            {
                var newClass = promptWindow.Input;
                if (!OnCheckDuplication(newClass))
                {
                    adminDAO.CreateClass(newClass);
                    Invalidate();
                }
                else
                {
                    MessageBox.Show("Class already exists");
                    return;
                }
            }
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            adminDAO.MovePrevious();
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new GradePage(adminDAO));
        }
    }
}
