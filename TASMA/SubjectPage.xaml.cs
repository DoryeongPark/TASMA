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
    /// 과목 등록을 위한 페이지입니다.
    /// </summary>
    public partial class SubjectPage : Page
    {
        private AdminDAO adminDAO;
        private List<StackPanel> columns;
        private List<string> subjectList;

        private int columnIndex;

        public SubjectPage(AdminDAO adminDAO)
        {
            InitializeComponent();

            this.adminDAO = adminDAO;

            columns = new List<StackPanel>();

            columns.Add(SubjectPage_Column0);
            columns.Add(SubjectPage_Column1);
            columns.Add(SubjectPage_Column2);

            SubjectPage_AddButton.Click += OnAddButtonClicked;

            Invalidate();
        }

        private void Invalidate()
        {
            foreach (var column in columns)
                column.Children.Clear();

            columnIndex = 0;
            subjectList = adminDAO.GetSubjectList();

            foreach (var data in subjectList)
            {
                var dataRect = new DataRectangle(data);

                dataRect.OnCheckDuplication += OnCheckDuplication;
                dataRect.OnModificationComplete += OnModificationComplete;
                dataRect.OnDeleteData += OnDeleteData;
                dataRect.MouseLeftButtonUp += OnClickSubject;
                
                columns[columnIndex].Children.Add(dataRect);
                if (++columnIndex == 3)
                    columnIndex = 0;
            }
        }

        private void OnClickSubject(object sender, MouseButtonEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var subjectSelected = (sender as DataRectangle).Data;

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new EvaluationPage(adminDAO, subjectSelected));
        }

        private void OnDeleteData(object sender, EventArgs e)
        {
            var dataRect = sender as DataRectangle;
            adminDAO.DeleteSubject(dataRect.Data);
            Invalidate();
        }

        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateSubject(oldData, newData);
            Invalidate();
        }

        private bool OnCheckDuplication(string newData)
        {
            foreach (var data in subjectList)
                if (data.ToUpper() == newData.ToUpper())
                    return true;

            return false;
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            var promptWindow = new TasmaPromptWindow("Create subject", "Please input subject name");
            promptWindow.ShowDialog();

            if (promptWindow.IsDetermined)
            {
                var newSubject = promptWindow.Input;
                if (!OnCheckDuplication(newSubject))
                {
                    adminDAO.CreateSubject(newSubject);
                    Invalidate();
                }
                else
                {
                    MessageBox.Show("Subject already exists");
                    return;
                }
            }
        }
    }
}
