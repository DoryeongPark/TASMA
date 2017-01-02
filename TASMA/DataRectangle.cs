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
                Background = Brushes.Honeydew; //Background
               
                var layout = new DockPanel();
                layout.Width = 120;
                layout.Height = 120;
              
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

                //텍스트 박스가 디스플레이 되는 영역
                textArea = new Grid();
                textArea.Width = 120;
                textArea.Height = 105;
                textArea.HorizontalAlignment = HorizontalAlignment.Center;
                textArea.VerticalAlignment = VerticalAlignment.Center;
                
                var viewBox = new Viewbox();
                viewBox.Width = 120;
                viewBox.Height = 105;
                viewBox.HorizontalAlignment = HorizontalAlignment.Center;
                viewBox.VerticalAlignment = VerticalAlignment.Center;
                viewBox.Stretch = Stretch.Uniform;
                viewBox.StretchDirection = StretchDirection.DownOnly;

                //데이터 이름
                textBlock = new TextBlock();
                textBlock.FontFamily = new FontFamily("Segoe UI");
                textBlock.FontSize = 20;
                textBlock.Foreground = Brushes.Indigo; //Font Color
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
                textBox.Foreground = Brushes.Honeydew;
                textBox.FontSize = 20;
                textBox.FontFamily = new FontFamily("Segoe UI");
                textBox.SelectedText = data;
                textBox.SelectAll();

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
                        MessageBox.Show("Data already exists");
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
