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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ExitRoutine(object sender, EventArgs e)
        {
            Close();
        }

        private void LoginTest(object sender, RoutedEventArgs e)
        {
            var dao = AdminDAO.GetDAO();
            dao.RegisterAdmin("FelixPark", "");
            dao.LoginAs("FelixPark", "");
            dao.CreateGrade("1");
            dao.CreateGrade("2");
            
        }
    }
}
