using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TASMA.MessageBox;

namespace TASMA.Dialog
{
    /// <summary>
    /// 데이터베이스 생성 다이얼로그입니다.
    /// </summary>
    public partial class InputDatabaseWindow : System.Windows.Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
            set { dbName = value; OnPropertyChanged("DBName"); }
        }

        private string schoolName = "";
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value; OnPropertyChanged("SchoolName"); }
        }

        private string year = "";
        public string Year
        {
            get { return year; }
            set { schoolName = year; OnPropertyChanged("Year"); }
        }

        private string region = "";
        public string Region
        {
            get { return region; }
            set { region = value; OnPropertyChanged("Region"); }
        }

        private string address = "";

        public string Address
        {
            get { return address; }
            set { address = value; OnPropertyChanged("Address"); }
        }

        public InputDatabaseWindow()
        {
            if(year == "")
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
                var alert = new TasmaAlertMessageBox("Alert", "Please input DBName");
                alert.ShowDialog();
                return false;
            }

            if(SchoolName == "")
            {
                var alert = new TasmaAlertMessageBox("Alert", "Please input School Name");
                alert.ShowDialog();
                return false;
            }

            if (Year == "")
            {
                var alert = new TasmaAlertMessageBox("Alert", "Please input Year");
                alert.ShowDialog();
                return false;
            }

            if (Region == "")
            {
                var alert = new TasmaAlertMessageBox("Alert", "Please input Region");
                alert.ShowDialog();
                return false;
            }

            if (Address == "")
            {
                var alert = new TasmaAlertMessageBox("Alert", "Please input Address");
                alert.ShowDialog();
                return false;
            }

            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
