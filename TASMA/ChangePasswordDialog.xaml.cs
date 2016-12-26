﻿using System;
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

namespace TASMA
{
    /// <summary>
    /// ChangePasswordDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChangePasswordDialog : Window
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
                MessageBox.Show("You should input username");
                return;
            }


            if(!adminDAO.Authenticate(username, password))
            {
                MessageBox.Show("Username and password are not matching");
                return;
            }

            if(newPassword != confirmNewPassword)
            {
                MessageBox.Show("Passwords are not matching");
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