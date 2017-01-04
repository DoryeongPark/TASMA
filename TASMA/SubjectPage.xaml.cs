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
    /// 과목 등록을 위한 페이지입니다.
    /// </summary>
    public partial class SubjectPage : Page
    {
        private AdminDAO adminDAO;

        public SubjectPage(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;
        }
    }
}
