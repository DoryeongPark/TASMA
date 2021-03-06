﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using TASMA.MessageBox;

namespace TASMA
{
    namespace DataInterfaces
    {
        class DataRectangle : Border { 

            private string data;

            private Grid textArea;
            private TextBlock textBlock;
            
            public string Data
            {
                get { return data;  }
            }

            public delegate void NotifyModification(string oldData, string newData);
            public delegate bool CheckModificationPossible(string newData);

            public event NotifyModification OnModificationComplete;
            public event CheckModificationPossible OnCheckDuplication;
            public event EventHandler OnDeleteData;
            
            /// <summary>
            /// 데이터 박스를 생성합니다.
            /// </summary>
            /// <param name="data">데이터</param>
            public DataRectangle(string data)
            {
                this.data = data;

                Width = 120;
                Height = 60;
                Margin = new Thickness(10);
                CornerRadius = new CornerRadius(5);
                Background = new SolidColorBrush(Color.FromRgb(197, 224, 206));

                var background = new DockPanel();
                background.Width = 120;
                background.Height = 60;
                
                var layout = new DockPanel();
                layout.Width = 120;
                layout.Height = 50;
              
                var btnArea = new DockPanel();
                btnArea.Width = 120;
                btnArea.Height = 15;
                    
                var deleteBtn = new Button();
                deleteBtn.Width = 15;
                deleteBtn.Height = 15;
                deleteBtn.Content = Application.Current.TryFindResource("tasmaDataDelete");
                deleteBtn.Background = Brushes.Transparent;
                deleteBtn.Margin = new Thickness(0, 2, 2, 0);
                deleteBtn.BorderThickness = new Thickness(0);
                deleteBtn.Click += OnDeleteButtonClicked;
                
                var modifyBtn = new Button();
                modifyBtn.Width = 15;
                modifyBtn.Height = 15;
                modifyBtn.Content = Application.Current.TryFindResource("tasmaDataModify");
                modifyBtn.Background = Brushes.Transparent;
                modifyBtn.Margin = new Thickness(0, 2, 0, 0);
                modifyBtn.BorderThickness = new Thickness(0);
                modifyBtn.Click += OnModifyButtonClicked;

                var blank = new StackPanel();

                //텍스트 박스가 디스플레이 되는 영역
                textArea = new Grid();
                textArea.Width = 120;
                textArea.Height = 35;
                textArea.HorizontalAlignment = HorizontalAlignment.Center;
                textArea.VerticalAlignment = VerticalAlignment.Center;
                
                var viewBox = new Viewbox();
                viewBox.Width = 120;
                viewBox.Height = 35;
                viewBox.HorizontalAlignment = HorizontalAlignment.Center;
                viewBox.VerticalAlignment = VerticalAlignment.Center;
                viewBox.Stretch = Stretch.Uniform;
                viewBox.StretchDirection = StretchDirection.DownOnly;

                //데이터 이름
                textBlock = new TextBlock();
                textBlock.FontFamily = new FontFamily("Segoe UI");
                textBlock.FontSize = 15;
                textBlock.Foreground = Brushes.Black;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Text = data;

                btnArea.Children.Add(deleteBtn);
                btnArea.Children.Add(modifyBtn);
                btnArea.Children.Add(blank);
                DockPanel.SetDock(deleteBtn, Dock.Right);
                DockPanel.SetDock(modifyBtn, Dock.Right);
                DockPanel.SetDock(blank, Dock.Top);
                viewBox.Child = textBlock;
                textArea.Children.Add(viewBox);

                background.Children.Add(btnArea);
                background.Children.Add(textArea);

                DockPanel.SetDock(btnArea, Dock.Top);
                DockPanel.SetDock(textArea, Dock.Bottom);

                this.Child = background;
                
            } 

            /// <summary>
            /// 삭제 버튼 클릭 시 연결된 델리게이트를 호출합니다.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnDeleteButtonClicked(object sender, EventArgs e)
            {
                if(DataRectangleManager.IsModified)
                    OnDeleteData?.Invoke(this, e);
            }

            /// <summary>
            /// 수정 버튼 클릭 시 컴포넌트 위에 텍스트 박스를 생성합니다.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnModifyButtonClicked(object sender, EventArgs e)
            {
                if (DataRectangleManager.IsModified)
                    DataRectangleManager.IsModified = false;
                else
                    return;

                //텍스트박스 생성 및 정의 부분
                var textBox = new TextBox();
                textBox.Width = 100;
                textBox.Height = 25;
                textBox.MaxLength = 15;
                textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                textBox.Background = this.Background;
                textBox.Foreground = Brushes.Indigo;
                textBox.FontSize = 15;
                textBox.FontFamily = new FontFamily("Segoe UI");
                textBox.FontWeight = FontWeights.SemiBold;
                textBox.Focus();
                textBox.Text = data;
                textBox.Select(0, data.Length);
               

                //이벤트 등록 - 텍스트박스가 포커스를 잃어버리면 현재의 객체 참조를 보내고 알려준다.
                textBox.LostFocus += (s, ea) =>
                {
                    //수정하지 않았을 시 동작
                    if (textBox.Text == data)
                    {
                        textArea.Children.Remove((TextBox)s);
                        DataRectangleManager.IsModified = true;
                        return;
                    }

                    //아이템 중복 여부 체크
                    if(OnCheckDuplication.Invoke(textBox.Text))
                    {
                        var alert = new TasmaAlertMessageBox("Duplication", "Data already exists");
                        alert.ShowDialog();
                        return;
                    }

                    var oldData = data;
                    data = textBox.Text;
                    textBlock.Text = textBox.Text;
                    textArea.Children.Remove((TextBox)s);
                    OnModificationComplete?.Invoke(oldData, data);
                    DataRectangleManager.IsModified = true;
                };

                textBox.KeyDown += (s, ea) =>
                {
                    if(ea.Key == Key.Enter)
                    {
                        textBox.RaiseEvent(new RoutedEventArgs(TextBox.LostFocusEvent));
                    }
                };

                
                textArea.Children.Add(textBox);
            }
        }
    }
}
