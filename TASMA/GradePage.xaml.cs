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

namespace TASMA
{
    /// <summary>
    /// Grade 리스트
    /// </summary>
    public partial class GradePage : Page
    {
        private AdminDAO adminDAO;

        private List<StackPanel> columns;

        private int columnIndex = 0;
        
        public GradePage(AdminDAO adminDAO)
        {
            InitializeComponent();

            this.adminDAO = adminDAO;
            columns = new List<StackPanel>();
            columns.Add(GradePage_Column0);
            columns.Add(GradePage_Column1);
            columns.Add(GradePage_Column2);
            
            var gradeList = adminDAO.GetGradeList();

            var tb1 = new Border();
            tb1.Width = 150;
            tb1.Height = 150;
            tb1.Background = Brushes.Red;


            var tb2 = new Border();
            tb2.Width = 150;
            tb2.Height = 150;
            tb2.Background = Brushes.Yellow;


            var tb3 = new Border();
            tb3.Width = 150;
            tb3.Height = 150;
            tb3.Background = Brushes.Blue;

            columns[0].Children.Add(tb1);
            columns[1].Children.Add(tb2);
            columns[0].Children.Add(tb3);
            

        }

        public void ResizeContents()
        {
            
        }
    }
}
