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
    /// StudentPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StudentPage : Page
    {
        private AdminDAO adminDAO;

        public StudentPage(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {

        }
    }
}
