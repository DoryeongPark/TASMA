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

        private Button btnClicked;

        public TasmaWindow(AdminDAO adminDAO)
        {
            InitializeComponent();
            this.adminDAO = adminDAO;

            menuButtons = new List<Button>();

            menuButtons.Add(TasmaWindow_Student);
            menuButtons.Add(TasmaWindow_Subject);
            menuButtons.Add(TasmaWindow_Marking);
            menuButtons.Add(TasmaWindow_Search);
            menuButtons.Add(TasmaWindow_Export);

            foreach(var btn in menuButtons)
            {
                btn.Click += OnClickMenuButton;
            }

            TasmaFrame.Frame = TasmaWindow_Frame;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnClickMenuButton(TasmaWindow_Student, null);
        }


        /// <summary>
        /// 메뉴 버튼 클릭 이벤트 루틴을 수행합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickMenuButton(object sender, RoutedEventArgs e)
        {
            
            adminDAO.ReturnToInitialLoginState();

            var menuBtn = sender as Button;
            btnClicked = menuBtn;

            foreach (var buttons in menuButtons)
            {
                buttons.IsDefault = false;
                buttons.IsEnabled = true;
            }
            
            btnClicked.IsDefault = true;
            btnClicked.IsEnabled = false;

            //Event for each buttons
            if (menuBtn.Equals(TasmaWindow_Student))
                TasmaWindow_Frame.Navigate(new GradePage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Subject))
                TasmaWindow_Frame.Navigate(new SubjectPage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Marking))
                TasmaWindow_Frame.Navigate(new MarkingPage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Search))
                TasmaWindow_Frame.Navigate(new SearchPage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Export))
                return;
            else
                return; //Error
        }

        /// <summary>
        /// 창 끌기 이벤트를 추가합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// 창 최소화 이벤트를 추가합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeRoutine(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 로그아웃 루틴을 수행합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            adminDAO.Logout();
            Close();
        }

        /// <summary>
        /// 프로그램 종료 루틴을 수행합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, EventArgs e)
        {
            Owner.Close();
        }

       
    }
}
