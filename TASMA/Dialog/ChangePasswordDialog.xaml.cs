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
using System.Windows.Shapes;
using TASMA.Database;
using TASMA.MessageBox;

namespace TASMA
{
    /// <summary>
    /// 선생님 계정 비밀번호 수정 다이얼로그
    /// </summary>
    public partial class ChangePasswordDialog : System.Windows.Window
    {
        private AdminDAO adminDAO;
        private bool isDetermined = false;

        public bool IsDetermined
        {
            get { return isDetermined; }
        }

        private string username;
        
        public string Username
        {
            get { return username;  }
        }

        private string password;
        
        public string Password
        {
            get { return password; }
        }

        private string newPassword;
        
        public string NewPassword
        {
            get { return newPassword; }
        }

        private string confirmNewPassword;
 
        public ChangePasswordDialog(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;
        }

        public void OnClickOKButton(object sender, RoutedEventArgs e)
        {
            username = ChangePasswordDialog_Username.Text;
            password = ChangePasswordDialog_Password.Password;
            newPassword = ChangePasswordDialog_NewPassword.Password;
            confirmNewPassword = ChangePasswordDialog_ConfirmNewPassword.Password;
            
            if(username == "")
            {
                var alert = new TasmaAlertMessageBox("Alert","You should input username");
                alert.ShowDialog();
                return;
            }

            if(!adminDAO.Authenticate(username, password))
            {
                var alert = new TasmaAlertMessageBox("Alert","Username and password doesn't match");
                alert.ShowDialog();
                return;
            }

            adminDAO.LogoutAccount();

            if(newPassword != confirmNewPassword)
            {
                var alert = new TasmaAlertMessageBox("Alert","Passwords are not matching");
                alert.ShowDialog();
                return;
            }

            isDetermined = true;
            Close();
        }

        public void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            isDetermined = false;
            Close();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
