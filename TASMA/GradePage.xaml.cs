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

            var testRect = new GradeRectangle(gradeList[0]);
            testRect.OnDeleteGrade += (sender, e) => { MessageBox.Show("Delete - " + (sender as GradeRectangle).Grade); };
            columns[0].Children.Add(testRect);
            columns[1].Children.Add(new GradeRectangle(gradeList[1]));
            columns[0].Children.Add(new GradeRectangle(gradeList[2]));
            
            
            
        }

        public void ResizeContents()
        {
            
        }
    }
}
