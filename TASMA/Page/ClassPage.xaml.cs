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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TASMA.Database;
using TASMA.DataInterfaces;
using TASMA.MessageBox;

namespace TASMA.Pages
{
    /// <summary>
    /// 반에 대한 페이지 입니다.
    /// </summary>
    public partial class ClassPage : Page
    {
        private AdminDAO adminDAO;
        private string gradeName;
        private List<StackPanel> columns;
        private List<string> classList;
        
        private int columnIndex;

        public ClassPage(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;

            gradeName = adminDAO.CurrentGrade;
            ClassPage_Grade.Text = "GRADE: " + gradeName;

            columns = new List<StackPanel>();
            columns.Add(ClassPage_Column0);
            columns.Add(ClassPage_Column1);
            columns.Add(ClassPage_Column2);

            ClassPage_PreviousButton.Click += OnPreviousButtonClicked;
            ClassPage_AddButton.Click += OnAddButtonClicked;

            Invalidate();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
             
        }

        /// <summary>
        /// 데이터베이스의 상태를 페이지에 반영합니다.
        /// </summary>
        private void Invalidate()
        {
            adminDAO.ReturnToInitialState();
            adminDAO.SelectGrade(gradeName);
            classList = adminDAO.GetClassList();

            //모든 데이터 박스 제거
            foreach (var column in columns)
                column.Children.Clear();

            columnIndex = 0;
            classList = adminDAO.GetClassList();

            foreach (var data in classList)
            {
                var dataRect = new DataRectangle(data);

                //이벤트 등록
                dataRect.OnCheckDuplication += OnCheckDuplication;
                dataRect.OnModificationComplete += OnModificationComplete;
                dataRect.OnDeleteData += OnDeleteData;
                dataRect.MouseLeftButtonUp += OnClickClass;

                //데이터 박스 추가
                columns[columnIndex].Children.Add(dataRect);
                if (++columnIndex == 3)
                    columnIndex = 0;
            }
        }

        /// <summary>
        /// 변경할 반 데이터가 다른 데이터와 중복되는지 확인합니다
        /// </summary>
        /// <param name="newData">변경할 데이터</param>
        /// <returns>중복 여부</returns>
        private bool OnCheckDuplication(string newData)
        {
            foreach (var data in classList)
                if (data.ToUpper() == newData.ToUpper())
                    return true;

            return false;
        }

        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateClass(oldData, newData);
            Invalidate();
        }

        private void OnDeleteData(object sender, EventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var dataRect = sender as DataRectangle;

            var passwordConfirm = new TasmaPasswordMessageBox("Input password", "Input password to delete class");
            passwordConfirm.ShowDialog();

            if (passwordConfirm.IsDetermined)
            {
                if (adminDAO.Authenticate(passwordConfirm.Input))
                {
                    adminDAO.DeleteClass(dataRect.Data);
                    Invalidate();
                }
                else
                {
                    var alert = new TasmaAlertMessageBox("Incorrect password", "Password doesn't match");
                    alert.ShowDialog();
                }
            }
        }

        private void OnClickClass(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var classSelected = (sender as DataRectangle).Data;
            adminDAO.SelectClass(classSelected);
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StudentPage(adminDAO));
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var promptWindow = new TasmaPromptMessageBox("Create class", "Please input class name");
            promptWindow.ShowDialog();

            if (promptWindow.IsDetermined)
            {
                var newClass = promptWindow.Input;
                if (!OnCheckDuplication(newClass))
                {
                    adminDAO.CreateClass(newClass);
                    Invalidate();
                }
                else
                {
                    var alert = new TasmaAlertMessageBox("Duplication", "Class already exists");
                    alert.ShowDialog();
                    return;
                }
            }
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            adminDAO.MovePrevious();
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new GradePage(adminDAO));
        }   
    }
}
