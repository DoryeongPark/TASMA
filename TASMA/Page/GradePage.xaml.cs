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
    /// 학년 페이지 입니다.
    /// </summary>
    public partial class GradePage : Page
    {
        private AdminDAO adminDAO;
        private List<StackPanel> columns;
        private List<string> gradeList;

        private int columnIndex;
       
        /// <summary>
        /// 현재 학년을 조회하는 페이지를 생성합니다.
        /// </summary>
        /// <param name="adminDAO">Data Access Object</param>
        public GradePage(AdminDAO adminDAO)
        {
            InitializeComponent();
           
            this.adminDAO = adminDAO;
            adminDAO.ReturnToInitialState();

            columns = new List<StackPanel>();
            columns.Add(GradePage_Column0);
            columns.Add(GradePage_Column1);
            columns.Add(GradePage_Column2);

            GradePage_AddButton.Click += OnAddButtonClicked;

            Invalidate();
        }

        /// <summary>
        /// Navigation의 이벤트 처리 루틴입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigating += (s, ea) =>
            {
                if (!DataRectangleManager.IsModified)
                {
                    DataRectangleManager.IsModified = true;
                }
            };
        }

        /// <summary>
        /// 데이터베이스의 상태를 페이지에 반영합니다.
        /// </summary>
        private void Invalidate()
        {
            //모든 데이터 박스 제거
            foreach (var column in columns)
                column.Children.Clear();
                
            columnIndex = 0;
            gradeList = adminDAO.GetGradeList();

            foreach (var data in gradeList)
            {
                var dataRect = new DataRectangle(data);

                //이벤트 등록
                dataRect.OnCheckDuplication += OnCheckDuplication;
                dataRect.OnModificationComplete += OnModificationComplete;
                dataRect.OnDeleteData += OnDeleteData;
                dataRect.MouseLeftButtonUp += OnClickGrade;

                //데이터 박스 추가
                columns[columnIndex].Children.Add(dataRect);
                if (++columnIndex == 3)
                    columnIndex = 0;
            }
        }

        /// <summary>
        /// 변경할 학년 데이터가 다른 데이터와 중복되는지 확인합니다. 
        /// </summary>
        /// <param name="newData">변경할 데이터</param>
        /// <returns>중복 여부</returns>
        private bool OnCheckDuplication(string newData)
        {
            foreach (var str in gradeList)
                if (str.ToUpper() == newData.ToUpper())
                    return true;
                   
            return false;
        }

        /// <summary>
        /// 학년 데이터를 변경하고 페이지에 반영합니다.
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        private void OnModificationComplete(string oldData, string newData)
        {
            adminDAO.UpdateGrade(oldData, newData);
            Invalidate();
        }

        /// <summary>
        /// 학년 데이터를 지우고 페이지에 반영합니다.
        /// </summary>
        /// <param name="sender">데이터 박스 객체</param>
        /// <param name="e">NULL</param>
        private void OnDeleteData(object sender, EventArgs e)
        {
            if (DataRectangleManager.IsModified == false)
                return;

            var dataRect = sender as DataRectangle;

            var passwordConfirm = new TasmaPasswordMessageBox("Input password", "Input password to delete grade");
            passwordConfirm.ShowDialog();

            if (passwordConfirm.IsDetermined) {

                if (adminDAO.Authenticate(passwordConfirm.Input))
                {
                    adminDAO.DeleteGrade(dataRect.Data);
                    Invalidate();
                }else
                {
                    var alert = new TasmaAlertMessageBox("Incorrect password", "Password doesn't match");
                    alert.ShowDialog();
                }
            }            
        }

        /// <summary>
        /// 학년 데이터를 추가하고 페이지에 반영합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var promptWindow = new TasmaPromptMessageBox("Create grade", "Please input grade name");
            promptWindow.ShowDialog();

            if (promptWindow.IsDetermined)
            {
                var newGrade = promptWindow.Input;
                if (!OnCheckDuplication(newGrade))
                {
                    adminDAO.CreateGrade(newGrade);
                    Invalidate();
                }else
                {
                    var alert = new TasmaAlertMessageBox("Duplication", "Grade already exists");
                    alert.ShowDialog();
                    return;
                }
            }
        }

        /// <summary>
        /// 학년을 선택하여 반 페이지로 넘어갑니다.
        /// </summary>
        /// <param name="sender">DataRectangle 객체</param>
        /// <param name="e"></param>
        private void OnClickGrade(object sender, RoutedEventArgs e)
        {
            if (!DataRectangleManager.IsModified)
                return;

            var gradeSelected = (sender as DataRectangle).Data;
            adminDAO.SelectGrade(gradeSelected);
            NavigationService.Navigate(new ClassPage(adminDAO));         
        }        
    }
}
