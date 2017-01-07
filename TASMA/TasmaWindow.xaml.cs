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
                btn.MouseEnter += OnMouseEnter;
                btn.MouseLeave += OnMouseLeave;
                btn.Click += OnClickMenuButton;
            }

            OnClickMenuButton(TasmaWindow_Student, null);

        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var menuBtn = sender as Button;

            if (menuBtn.Equals(btnClicked))
                return;

            SetUnClickedStyle(menuBtn);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var menuBtn = sender as Button;

            if (menuBtn.Equals(btnClicked))
                return;

            SetClickedStyle(menuBtn);
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
            
            for(int i = 0; i < menuButtons.Count; ++i)
            {
                if (btnClicked.Equals(menuButtons[i]))
                    SetClickedStyle(menuButtons[i]);
                else
                    SetUnClickedStyle(menuButtons[i]);
            }

            //Event for each buttons
            if (menuBtn.Equals(TasmaWindow_Student))
                TasmaWindow_Frame.Navigate(new GradePage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Subject))
                TasmaWindow_Frame.Navigate(new SubjectPage(adminDAO));
            else if (menuBtn.Equals(TasmaWindow_Score))
                return;
            else if (menuBtn.Equals(TasmaWindow_Print))
                return;
            else if (menuBtn.Equals(TasmaWindow_Export))
                return;
            else
                return; //Error
        }

        /// <summary>
        /// 버튼 클릭 스타일을 적용합니다.
        /// </summary>
        /// <param name="btn">스타일을 적용할 버튼</param>
        private void SetClickedStyle(Button btn)
        {
            btn.Foreground = Brushes.Black;
            btn.Background = Brushes.Azure;
        }

        /// <summary>
        /// 버튼을 클릭하지 않은 스타일을 적용합니다.
        /// </summary>
        /// <param name="btn">스타일을 적용할 버튼</param>
        private void SetUnClickedStyle(Button btn)
        {
            btn.Foreground = Brushes.White;
            btn.Background = Brushes.Indigo;
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
