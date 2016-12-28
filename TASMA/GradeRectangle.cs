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
        class GradeRectangle : Border { 

            private string grade;
          
            public GradeRectangle(string grade)
            {
                this.grade = grade;
                Width = 120;
                Height = 120;
                Background = Brushes.Black;

                var viewBox = new Viewbox();
                viewBox.Width = 120;
                viewBox.Height = 120;
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
               
                viewBox.Child = textBlock;
                this.Child = viewBox;
            } 
        }
    }
}
