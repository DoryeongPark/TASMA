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

        private int columnIndex = 0;
        
        public GradePage(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;

            var gradeList = adminDAO.GetGradeList();
            
            
        }
    }
}
