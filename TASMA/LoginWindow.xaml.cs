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
        private AdminDAO adminDAO;

        public LoginWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            adminDAO = AdminDAO.GetDAO();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitRoutine(object sender, EventArgs e)
        {
            Close();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Login");
        }

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            var cpd = new ChangePasswordDialog(adminDAO);
            cpd.ShowDialog();

            if (cpd.IsDetermined)
            {
                adminDAO.ChangePassword(cpd.Username, cpd.Password, cpd.NewPassword);
            }
        }

        private void RegisterAdmin(object sender, RoutedEventArgs e)
        {
            var rd = new RegistrationDialog();
            rd.ShowDialog();

            if (rd.IsDetermined)
            {
                adminDAO.RegisterAdmin(rd.UserName, rd.Password);
            }
        }
    }
}
