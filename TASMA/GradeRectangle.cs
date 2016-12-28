using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TASMA
{
    namespace DataInterfaces
    {
        class GradeRectangle : DockPanel { 

            private string grade;

            public string Grade
            {
                get { return grade;  }
            }

            public event EventHandler OnDeleteGrade;
            public event EventHandler OnModifyGrade;
          
            public GradeRectangle(string grade)
            {
                this.grade = grade;
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
                
                var viewBox = new Viewbox();
                viewBox.Width = 15;
                viewBox.Height = 105;
                viewBox.HorizontalAlignment = HorizontalAlignment.Center;
                viewBox.VerticalAlignment = VerticalAlignment.Center;
                viewBox.Stretch = Stretch.Uniform;
                viewBox.StretchDirection = StretchDirection.DownOnly;

                var textBlock = new TextBlock();
                textBlock.FontFamily = new FontFamily("Segoe UI");
                textBlock.FontSize = 15;
                textBlock.Foreground = Brushes.Yellow;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Text = grade;

                btnArea.Children.Add(deleteBtn);
                btnArea.Children.Add(modifyBtn);
                btnArea.Children.Add(blank);
                SetDock(deleteBtn, Dock.Right);
                SetDock(modifyBtn, Dock.Right);
                SetDock(blank, Dock.Top);
                viewBox.Child = textBlock;

                this.Children.Add(btnArea);
                this.Children.Add(viewBox);

                SetDock(btnArea, Dock.Top);
                SetDock(viewBox, Dock.Bottom);
                
            } 

            private void OnDeleteButtonClicked(object sender, EventArgs e)
            {
                OnDeleteGrade?.Invoke(this, e);
            }

            private void OnModifyButtonClicked(object sender, EventArgs e)
            {
                OnModifyGrade?.Invoke(this, e);
            }

        }
    }
}
