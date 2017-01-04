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

namespace TASMA
{
    /// <summary>
    /// TASMA Main Window
    /// </summary>
    public partial class TasmaWindow : Window
    {
        private AdminDAO adminDAO;

        private List<Button> menuButtons;

        public TasmaWindow(AdminDAO adminDAO)
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            this.adminDAO = adminDAO;

            menuButtons = new List<Button>();

            menuButtons.Add(TasmaWindow_Student);
            menuButtons.Add(TasmaWindow_Subject);
            menuButtons.Add(TasmaWindow_Score);
            menuButtons.Add(TasmaWindow_Print);
            menuButtons.Add(TasmaWindow_Export);

            foreach(var btn in menuButtons)
            {
                btn.MouseLeave += new MouseEventHandler(OnMenuButtonMouseLeave);
                btn.MouseEnter += new MouseEventHandler(OnMenuButtonMouseEnter);
            }

            TasmaWindow_Frame.Navigate(new GradePage(adminDAO));
            
 
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeRoutine(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitRoutine(object sender, EventArgs e)
        {
            Close();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Owner.Close();
        }

        private void OnMenuButtonMouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.Background = Brushes.Azure;
            button.Foreground = Brushes.Black;
        }
       
        private void OnMenuButtonMouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.Background = Brushes.Indigo;
            button.Foreground = Brushes.White;
        }
    }
}
