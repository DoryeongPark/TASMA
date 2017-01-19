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

namespace TASMA
{
    /// <summary>
    /// 데이터베이스 생성 다이얼로그입니다.
    /// </summary>
    public partial class CreateDatabaseWindow : Window
    {
        private bool isDetermined = false;
        public bool IsDetermined
        {
            get { return isDetermined; }
            set { isDetermined = value; }
        }

        private string dbName = "";
        public string DBName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        private string schoolName = "";
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value; }
        }

        private string year = "";
        public string Year
        {
            get { return year; }
            set { schoolName = year; }
        }

        private string region = "";
        public string Region
        {
            get { return region; }
            set { region = value; }
        }

        private string address = "";
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public CreateDatabaseWindow()
        {
            year = DateTime.Now.Year.ToString();

            DataContext = this;
            InitializeComponent();
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CheckTextBox())
            {
                IsDetermined = true;
                Close();
            }
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            IsDetermined = false;
            Close();
        }
        
        private bool CheckTextBox()
        {
            if(DBName == "")
            {
                MessageBox.Show("Please input DBName");
                return false;
            }

            if(SchoolName == "")
            {
                MessageBox.Show("Please input School Name");
                return false;
            }

            if (Year == "")
            {
                MessageBox.Show("Please input Year");
                return false;
            }

            if (Region == "")
            {
                MessageBox.Show("Please input Region");
                return false;
            }

            if (Address == "")
            {
                MessageBox.Show("Please input Address");
                return false;
            }

            return true;
        }
    }
}
