using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    /// 선생님 계정의 데이터베이스 리스트를 불러옵니다.
    /// </summary>
    public partial class DatabaseListWindow : Window, INotifyPropertyChanged
    {

        private AdminDAO adminDAO;
        private string accountName;

        private string determinedDBPath = null;
        public string DeterminedDBPath
        {
            get { return determinedDBPath; }
            set { determinedDBPath = value; }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> dbListBoxItems;
        public ObservableCollection<string> DBListBoxItems
        {
            get { return dbListBoxItems; }
            set { dbListBoxItems = value; OnPropertyChanged("DBListBoxItems"); }
        }

        private string selectedDBListBoxItem;
        public string SelectedDBListBoxItem
        {
            get { return selectedDBListBoxItem; }
            set { selectedDBListBoxItem = value; OnPropertyChanged("SelectedDBListBoxItem"); }
        }

        public DatabaseListWindow(AdminDAO adminDAO, string accountName)
        {
            this.adminDAO = adminDAO;
            this.accountName = accountName;

            var dir = new DirectoryInfo(accountName);
            var dbList = dir.GetFiles();

            dbListBoxItems = new ObservableCollection<string>();
            foreach(var dbFile in dbList)
            {
                var dbName = dbFile.Name;

                if (dbName == "Authentication.db")
                    continue;

                dbName = dbName.Remove(dbName.Length - 3);
                dbListBoxItems.Add(dbName);
            }

            DataContext = this;
            InitializeComponent();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            var cdd = new InputDatabaseWindow();
            cdd.ShowDialog();

            /* 데이터베이스 생성 루틴 */
            if (cdd.IsDetermined)
            {   
                if(dbListBoxItems.Any(item => item == cdd.DBName) ||
                    cdd.DBName == "Authentication")
                {
                    MessageBox.Show("Database already exists");
                    return;
                }

                var path = accountName + "/" + cdd.DBName;
                var metaData = new string[4] { cdd.SchoolName,
                                               cdd.Year,
                                               cdd.Region,
                                               cdd.Address };
                
                if(adminDAO.CreateDatabase(path, metaData))
                {
                    DBListBoxItems.Add(cdd.DBName);
                }
                
            }
        }

        private void OnModifyButtonClicked(object sender, RoutedEventArgs e)
        {
            if (SelectedDBListBoxItem == null)
                return;

            var dbPath = accountName + "/" + SelectedDBListBoxItem;
            string[] dbInfo = adminDAO.GetDBInfo(dbPath);
            var cdd = new InputDatabaseWindow();
            cdd.DBName = SelectedDBListBoxItem;
            cdd.SchoolName = dbInfo[0]; cdd.Year = dbInfo[1];
            cdd.Region = dbInfo[2]; cdd.Address = dbInfo[3];
             
            cdd.ShowDialog();

            /* 파일 이름, 데이터베이스 정보 수정 루틴 */
            if (cdd.IsDetermined)
            {
                var newDBPath = accountName + "/" + cdd.DBName;
                File.Move(dbPath + ".db", newDBPath + ".db");
                adminDAO.ModifyDBInfo(newDBPath, new string[] { cdd.SchoolName,
                                                              cdd.Year,
                                                              cdd.Region,
                                                              cdd.Address });
                DBListBoxItems.Remove(SelectedDBListBoxItem);
                DBListBoxItems.Add(cdd.DBName);
                SelectedDBListBoxItem = cdd.DBName;
            }
        }

        private void OnDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            /* 인증 루틴 */
            var confirm = new TasmaPromptMessageBox("Delete database", "Please input password to delete");
            confirm.ShowDialog();
            var authenticationPath = accountName + "/" + "Authentication";
       
            /* 인증 여부 확인 */
            if(!adminDAO.Authenticate(authenticationPath, confirm.Input))
            {
                MessageBox.Show("Password doesn't match");
                return;
            }
           
            /* 데이터베이스 삭제 루틴 */
            var dbPath = accountName + "/" + SelectedDBListBoxItem;
            File.Delete(dbPath + ".db");
            DBListBoxItems.Remove(SelectedDBListBoxItem);
            SelectedDBListBoxItem = null;
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnExitButtonClicked(object sender, RoutedEventArgs e)
        {
            determinedDBPath = null;
            Close();
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            determinedDBPath = accountName + "/" + SelectedDBListBoxItem;
            Close();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
