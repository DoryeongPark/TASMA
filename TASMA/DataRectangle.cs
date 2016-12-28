using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace TASMA
{
    namespace DataInterfaces
    {
        class DataRectangle : DockPanel { 

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
                Height = 120;
                Margin = new Thickness(10);
                Background = Brushes.Black;

                var btnArea = new DockPanel();
                btnArea.Width = 120;
                btnArea.Height = 15;
                    
                var deleteBtn = new Button();
                deleteBtn.Width = 15;
                deleteBtn.Height = 15;
                deleteBtn.Click += OnDeleteButtonClicked;
                
                var modifyBtn = new Button();
                modifyBtn.Width = 15;
                modifyBtn.Height = 15;
                modifyBtn.Click += OnModifyButtonClicked;

                var blank = new StackPanel();

                textArea = new Grid();
                textArea.Width = 120;
                textArea.Height = 105;
                textArea.HorizontalAlignment = HorizontalAlignment.Center;
                textArea.VerticalAlignment = VerticalAlignment.Center;
                textArea.Background = Brushes.Brown;
                
                var viewBox = new Viewbox();
                viewBox.Width = 120;
                viewBox.Height = 105;
                viewBox.HorizontalAlignment = HorizontalAlignment.Center;
                viewBox.VerticalAlignment = VerticalAlignment.Center;
                viewBox.Stretch = Stretch.Uniform;
                viewBox.StretchDirection = StretchDirection.DownOnly;

                textBlock = new TextBlock();
                textBlock.FontFamily = new FontFamily("Segoe UI");
                textBlock.FontSize = 15;
                textBlock.Foreground = Brushes.Yellow;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Text = data;

                btnArea.Children.Add(deleteBtn);
                btnArea.Children.Add(modifyBtn);
                btnArea.Children.Add(blank);
                SetDock(deleteBtn, Dock.Right);
                SetDock(modifyBtn, Dock.Right);
                SetDock(blank, Dock.Top);
                viewBox.Child = textBlock;
                textArea.Children.Add(viewBox);

                this.Children.Add(btnArea);
                this.Children.Add(textArea);

                SetDock(btnArea, Dock.Top);
                SetDock(textArea, Dock.Bottom);
                
            } 

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
                textBox.Foreground = Brushes.White;
                textBox.FontSize = 15;
                textBox.FontFamily = new FontFamily("Segoe UI");
                textBox.SelectedText = data;

                //이벤트 - 텍스트박스가 포커스를 잃어버렸을 시 현재의 객체 참조를 보내고 알려준다.
                textBox.LostKeyboardFocus += (s, ea) =>
                {
                    
                    if(OnCheckDuplication.Invoke(textBox.Text))
                    {
                        MessageBox.Show("Grade already exists");
                        return;
                    }

                    var oldData = data;
                    data = textBox.Text;
                    textBlock.Text = textBox.Text;
                    textArea.Children.Remove((TextBox)s);
                    OnModificationComplete?.Invoke(oldData, data);
                    DataRectangleManager.IsModified = true;
                };

                textArea.Children.Add(textBox);
            }
        }
    }
}
