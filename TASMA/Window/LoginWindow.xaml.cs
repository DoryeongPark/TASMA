using System;
using System.Collections.Generic;
using System.IO;
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
            LoginWindow_ID.Focus();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeRoutine(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 프로그램을 종료합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitRoutine(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 선생님 계정으로 로그인합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, RoutedEventArgs e)
        {
            if(adminDAO.Authenticate(LoginWindow_ID.Text, LoginWindow_Password.Password))
            {
                var dld = new DatabaseListWindow(adminDAO, LoginWindow_ID.Text);
                dld.ShowDialog();
               
                //데이터베이스 리스트에서 데이터를 선택하고 OK 버튼 클릭
                if(dld.DeterminedDBPath != null)
                {
                    adminDAO.LoginDatabase(dld.DeterminedDBPath);
                    var td = new TasmaWindow(adminDAO);
                    this.Visibility = Visibility.Hidden;
                    td.Owner = this;
                    td.ShowDialog();

                }else//데이터베이스 리스트에서 취소 버튼 클릭
                {
                    adminDAO.LogoutAccount();
                }
                         
            }else
            {
                MessageBox.Show("Password doesn't match");
            }

        }

        /// <summary>
        /// 계정의 비밀번호를 바꿉니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            var cpd = new ChangePasswordDialog(adminDAO);
            cpd.ShowDialog();

            if (cpd.IsDetermined)
            {
                adminDAO.ChangePassword(cpd.Username, cpd.Password, cpd.NewPassword);
            }
        }

        /// <summary>
        /// 선생님 계정을 등록합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterAdmin(object sender, RoutedEventArgs e)
        {
            var rd = new RegistrationDialog();
            rd.ShowDialog();

            if (rd.IsDetermined)
            {
                //계정 중복 검사
                if(new DirectoryInfo(rd.UserName).Exists)
                {
                    MessageBox.Show("User already exists");
                    return;
                }
                adminDAO.RegisterAccount(rd.UserName, rd.Password);
            }
        }

       
    }
}
