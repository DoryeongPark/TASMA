using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TASMA.Database;

namespace TASMA.Dialog
{
    /// <summary>
    /// PrintDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private AdminDAO adminDAO;
        private DataTable originalDataTable;

        private List<Tuple<string, string>> classList;
        private List<Tuple<string, string, long, string>> studentList;

        private string schoolName;
        private string year;
        private string region;
        private string address;

        /* ProgressBar Properties */
        private int progressValue = 0;
        public int ProgressValue
        {
            get { return progressValue; }
            set { progressValue = value;  OnPropertyChanged("ProgressValue"); }
        }

        /* Report ListBox Properties */
        private ObservableCollection<string> reportListBoxItems;
        public ObservableCollection<string> ReportListBoxItems
        {
            get { return reportListBoxItems; }
            set { reportListBoxItems = value;
                OnPropertyChanged("ReportListBoxItems"); }
        }

        private string selectedReportListBoxItem;
        public string SelectedReportListBoxItem
        {
            get { return selectedReportListBoxItem; }
            set { selectedReportListBoxItem = value;
                OnPropertyChanged("SelectedReportListBoxItem"); }
        }


        /* NameSheet Option Properties */
        private ObservableCollection<string> nameSheetGradeComboBoxItems;
        public ObservableCollection<string> NameSheetGradeComboBoxItems
        {
            get { return nameSheetGradeComboBoxItems; }
            set
            {
                nameSheetGradeComboBoxItems = value;
                OnPropertyChanged("NameSheetGradeComboBoxItems");
            }
        }

        private string selectedNameSheetGradeComboBoxItem;
        public string SelectedNameSheetGradeComboBoxItem
        {
            get { return selectedNameSheetGradeComboBoxItem; }
            set
            {
                selectedNameSheetGradeComboBoxItem = value;
                OnPropertyChanged("SelectedNameSheetGradeComboBoxItem");
            }
        }

        private ObservableCollection<string> nameSheetClassComboBoxItems;
        public ObservableCollection<string> NameSheetClassComboBoxItems
        {
            get { return nameSheetClassComboBoxItems; }
            set
            {
                nameSheetClassComboBoxItems = value;
                OnPropertyChanged("NameSheetClassComboBoxItems");
            }
        }

        private string selectedNameSheetClassComboBoxItem;
        public string SelectedNameSheetClassComboBoxItem
        {
            get { return selectedNameSheetClassComboBoxItem; }
            set
            {
                selectedNameSheetClassComboBoxItem = value;
                OnPropertyChanged("SelectedNameSheetClassComboBoxItem");
            }
        }

        /* Subject Option Properties */
        private ObservableCollection<KeyValuePair<int, string>> subjectSemesterComboBoxItems;
        public ObservableCollection<KeyValuePair<int, string>> SubjectSemesterComboBoxItems
        {
            get { return subjectSemesterComboBoxItems; }
            set { subjectSemesterComboBoxItems = value; OnPropertyChanged("SubjectSemesterComboBoxItems"); }
        }

        private KeyValuePair<int, string> selectedSubjectSemesterComboBoxItem;
        public KeyValuePair<int, string> SelectedSubjectSemesterComboBoxItem
        {
            get { return selectedSubjectSemesterComboBoxItem; }
            set { selectedSubjectSemesterComboBoxItem = value; OnPropertyChanged("SelectedSubjectSemesterComboBoxItem"); }
        }

        private ObservableCollection<string> subjectComboBoxItems;
        public ObservableCollection<string> SubjectComboBoxItems
        {
            get { return subjectComboBoxItems; }
            set { subjectComboBoxItems = value; OnPropertyChanged("SubjectComboBoxItems"); }
        }

        private string selectedSubjectComboBoxItem;
        public string SelectedSubjectComboBoxItem
        {
            get { return selectedSubjectComboBoxItem; }
            set { selectedSubjectComboBoxItem = value; OnPropertyChanged("SelectedSubjectComboBoxItem"); }
        }

        /* Student Option Properties */
        private ObservableCollection<KeyValuePair<int, string>> studentSemesterComboBoxItems;
        public ObservableCollection<KeyValuePair<int, string>> StudentSemesterComboBoxItems
        {
            get { return studentSemesterComboBoxItems; }
            set { studentSemesterComboBoxItems = value; OnPropertyChanged("StudentSemesterComboBoxItems"); }
        }

        private KeyValuePair<int, string> selectedStudentSemesterComboBoxItem;
        public KeyValuePair<int, string> SelectedStudentSemesterComboBoxItem
        {
            get { return selectedStudentSemesterComboBoxItem; }
            set { selectedStudentSemesterComboBoxItem = value; OnPropertyChanged("SelectedStudentSemesterComboBoxItem"); }
        }

        public ReportDialog(AdminDAO adminDAO, DataTable dataTable)
        {
            this.adminDAO = adminDAO;
            
            /* 데이터 테이블 복사 및 필터링 처리 루틴 */
            object copiedDataTable = dataTable.Copy() as object;
            this.originalDataTable = copiedDataTable as DataTable;

            for (int i = 0; i < originalDataTable.Rows.Count; ++i)
            {
                if ((bool)originalDataTable.Rows[i]["Print"] == false)
                {
                    originalDataTable.Rows.RemoveAt(i);
                    i = -1;
                }
            }

            
            /* Class 리스트 초기화 */
            var allClasses = originalDataTable.AsEnumerable().Select
                                               (row => new {
                                                   GradeName = row.Field<string>("GRADE"),
                                                   ClassName = row.Field<string>("CLASS")
                                               }).Distinct();

            classList = new List<Tuple<string, string>>();
            foreach(var classItem in allClasses)
                classList.Add(new Tuple<string, string>(classItem.GradeName, classItem.ClassName));

            /* Student 리스트 초기화 */
            var allStudents = originalDataTable.AsEnumerable().Select
                                                (row => new
                                                {
                                                    GradeName = row.Field<string>("GRADE"),
                                                    ClassName = row.Field<string>("CLASS"),
                                                    StudentNum = row.Field<long>("SNUM"),
                                                    StudentName = row.Field<string>("SNAME")
                                                }).Distinct();

            studentList = new List<Tuple<string, string, long, string>>();
            foreach (var studentItem in allStudents)
                studentList.Add(new Tuple<string, string, long, string>(studentItem.GradeName, 
                                                                studentItem.ClassName, 
                                                                studentItem.StudentNum,
                                                                studentItem.StudentName));            

            /* 메타 데이터 획득 루틴 */
            var metaData = adminDAO.GetDBInfo();
            schoolName = metaData[0].ToUpper();
            year = metaData[1];
            region = metaData[2].ToUpper();
            address = metaData[3].ToUpper();

            DataContext = this;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PrintDialog_DocumentViewer.FitToWidth();

            /* Datatable 예외 처리 */
            if (originalDataTable == null ||
                originalDataTable.Rows.Count == 0)
            {
                var alert = new TasmaAlertMessageBox("Alert", "There are no records to print");
                alert.ShowDialog();
                Close();
            }

            /* 데이터 바인딩 객체 초기화 */
            NameSheetGradeComboBoxItems = new ObservableCollection<string>();
            NameSheetClassComboBoxItems = new ObservableCollection<string>();

            SubjectSemesterComboBoxItems = new ObservableCollection<KeyValuePair<int, string>>();
            SubjectSemesterComboBoxItems.Add(new KeyValuePair<int, string>(0, "1st"));
            SubjectSemesterComboBoxItems.Add(new KeyValuePair<int, string>(1, "2nd"));

            StudentSemesterComboBoxItems = new ObservableCollection<KeyValuePair<int, string>>();
            StudentSemesterComboBoxItems.Add(new KeyValuePair<int, string>(0, "1st"));
            StudentSemesterComboBoxItems.Add(new KeyValuePair<int, string>(1, "2nd"));

            SubjectComboBoxItems = new ObservableCollection<string>();
            var tempSubjectList = new List<string>();
            foreach(var classItem in classList)
            {
                var subjectList = adminDAO.GetClassSubjects(classItem.Item1, classItem.Item2);
                foreach (var subject in subjectList)
                    tempSubjectList.Add(subject);
            }
            tempSubjectList = tempSubjectList.Distinct().ToList();
            foreach(var subjectName in tempSubjectList)
                SubjectComboBoxItems.Add(subjectName);

            /* Grade 리스트 초기화 */
            var gradeList = originalDataTable.AsEnumerable().Select(r => r.Field<string>("GRADE")).Distinct().ToList();
            foreach (var gradeName in gradeList)
                NameSheetGradeComboBoxItems.Add(gradeName);


            /* Report 리스트박스 초기화 */
            ReportListBoxItems = new ObservableCollection<string>();
            ReportListBoxItems.Add("NameSheet");
            ReportListBoxItems.Add("Subject");
            ReportListBoxItems.Add("Student");
        }

        private void OnReportListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedReportListBoxItem == "NameSheet")
            {
                NameSheetOption.Visibility = Visibility.Visible;
                SubjectReportOption.Visibility = Visibility.Hidden;
                StudentReportOption.Visibility = Visibility.Hidden;

                /* 바인딩 상태 초기화 */
                SelectedNameSheetGradeComboBoxItem = null;
                SelectedNameSheetClassComboBoxItem = null;

                /* Grade ComboBox 초기화 */
                SelectedNameSheetGradeComboBoxItem = NameSheetGradeComboBoxItems[0];
                
            }
            else if (SelectedReportListBoxItem == "Subject")
            {
                NameSheetOption.Visibility = Visibility.Hidden;
                SubjectReportOption.Visibility = Visibility.Visible;
                StudentReportOption.Visibility = Visibility.Hidden;

                /* Semester 초기화 */
                if (1 < DateTime.Now.Month && DateTime.Now.Month <= 7)
                {
                    if (SelectedSubjectSemesterComboBoxItem.Key == SubjectSemesterComboBoxItems[0].Key)
                        DisplaySubjectReport();

                    SelectedSubjectSemesterComboBoxItem = SubjectSemesterComboBoxItems[0];
                }
                else
                {
                    if (SelectedSubjectSemesterComboBoxItem.Key == SubjectSemesterComboBoxItems[1].Key)
                        DisplaySubjectReport();

                    SelectedSubjectSemesterComboBoxItem = SubjectSemesterComboBoxItems[1];
                }

            }
            else if(SelectedReportListBoxItem == "Student")
            {
                NameSheetOption.Visibility = Visibility.Hidden;
                SubjectReportOption.Visibility = Visibility.Hidden;
                StudentReportOption.Visibility = Visibility.Visible;

                /* Semester 초기화 */
                if (1 < DateTime.Now.Month && DateTime.Now.Month <= 7)
                {
                    if (SelectedStudentSemesterComboBoxItem.Key == StudentSemesterComboBoxItems[0].Key)
                    {
                        DisplayStudentReport();
                        return;
                    }

                    SelectedStudentSemesterComboBoxItem = SubjectSemesterComboBoxItems[0];
                }
                else
                {
                    if (SelectedStudentSemesterComboBoxItem.Key == StudentSemesterComboBoxItems[1].Key)
                    {
                        DisplayStudentReport();
                        return;
                    }

                    SelectedStudentSemesterComboBoxItem = SubjectSemesterComboBoxItems[1];
                }

            }
        }

        /* Namesheet Option Methods */
        private void NameSheetGradeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* 바인딩 상태 초기화 */
            NameSheetClassComboBoxItems.Clear();
            SelectedNameSheetClassComboBoxItem = null;

            /* Make & Show FixedDocument */
            DisplayNameSheet();

            /* Class ComboBox 초기화 */
            var classList = (from DataRow row in originalDataTable.Rows
                          where (string)row["GRADE"] == SelectedNameSheetGradeComboBoxItem
                          select (string)row["CLASS"]).Distinct().ToList();
            foreach (var className in classList)
                NameSheetClassComboBoxItems.Add(className);
           
        }

        private void NameSheetClassComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Make & Show FixedDocument */
            DisplayNameSheet();
        }

        private void SubjectSemesterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectComboBoxItems.Count != 0)
            {
                if (SelectedSubjectComboBoxItem == SubjectComboBoxItems[0])
                    SubjectComboBoxSelectionChanged(null, null);

                SelectedSubjectComboBoxItem = SubjectComboBoxItems[0];      
            }
        }

        private void SubjectComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Make & Show FixedDocuments */
            DisplaySubjectReport();
        }

        private void StudentSemesterComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Make & Show FixedDocuments */
            DisplayStudentReport();
        }

        private void DisplayNameSheet()
        {

            DocumentProgressBar.Dispatcher.Invoke(() =>
                       DocumentProgressBar.Value = 0, DispatcherPriority.Background);


            /* 전체 레이아웃 */
            var background = new Grid();
            background.Width = 750;
            background.Margin = new Thickness(20);
            background.Background = Brushes.Transparent;

            Image image = new Image();
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri("pack://application:,,,/Tasma;component/Resources/Tasma_DepartmentMark.png");
            bit.EndInit();
            image.Source = bit;
            image.VerticalAlignment = VerticalAlignment.Top;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.Width = 120;
            image.Height = 120;
            background.Children.Add(image);

            var layout = new StackPanel();
            layout.Orientation = Orientation.Vertical;
            layout.HorizontalAlignment = HorizontalAlignment.Center;
            layout.Width = 750;
            background.Children.Add(layout);

            /* 타이틀 영역 */
            var titleArea = new StackPanel();
            titleArea.HorizontalAlignment = HorizontalAlignment.Center;
            titleArea.VerticalAlignment = VerticalAlignment.Center;
            titleArea.Height = 120;
            titleArea.Background = Brushes.Transparent;
            titleArea.Margin = new Thickness(0, 0, 0, 15);
            layout.Children.Add(titleArea);

            /* 분급 영역 */
            var descArea = new Grid();
            descArea.Height = 30;
            layout.Children.Add(descArea);

            /* 표 영역 */
            var tableArea = new StackPanel();
            tableArea.HorizontalAlignment = HorizontalAlignment.Center;
            layout.Children.Add(tableArea);

            /* 타이틀 텍스트 */
            var title = new TextBlock();
            title.TextAlignment = TextAlignment.Center;
            title.FontSize = 20;
            title.FontWeight = FontWeights.Bold;
            title.Text = "\n" + region + " CITY COUNCIL\n"
                       + schoolName + "\n"
                       + "STUDENT NAMESHEET";
            titleArea.Children.Add(title);

            /* 분급 텍스트 */
            var master = new TextBlock();
            master.TextAlignment = TextAlignment.Left;
            master.FontSize = 12;
            master.Text = Space(5) + "MASTER: ";
            descArea.Children.Add(master);

            var year = new TextBlock();
            year.TextAlignment = TextAlignment.Right;
            year.FontSize = 12;
            year.Text = "YEAR:" + Space(5) + this.year + Space(5);
            descArea.Children.Add(year);

            if (SelectedNameSheetClassComboBoxItem == null)
            {
                var grade = new TextBlock();
                grade.TextAlignment = TextAlignment.Right;
                grade.FontSize = 12;
                grade.Text = "GRADE:" + Space(3) + SelectedNameSheetGradeComboBoxItem + Space(40);
                descArea.Children.Add(grade);
            }
            else
            {
                var classText = new TextBlock();
                classText.HorizontalAlignment = HorizontalAlignment.Right;
                classText.TextAlignment = TextAlignment.Right;
                classText.FontSize = 12;
                classText.Text = "CLASS:" + Space(3) + SelectedNameSheetClassComboBoxItem + Space(40);
                descArea.Children.Add(classText);

                var grade = new TextBlock();
                grade.TextAlignment = TextAlignment.Right;
                grade.FontSize = 12;
                grade.Text = "GRADE:" + Space(3) + SelectedNameSheetGradeComboBoxItem + Space(65);
                descArea.Children.Add(grade);
            }

            DocumentProgressBar.Dispatcher.Invoke(() =>
                       DocumentProgressBar.Value = 50, DispatcherPriority.Background);


            /* 학생 테이블 */
            DataGrid dataGrid = new DataGrid();
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            dataGrid.Width = 720;
            dataGrid.BorderBrush = Brushes.Black;
            dataGrid.Foreground = Brushes.Black;
            dataGrid.Background = Brushes.White;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.Visibility = Visibility.Visible;
            var headerStyle = new Style(typeof(DataGridColumnHeader));
            headerStyle.BasedOn = this.TryFindResource("printHeaderStyle") as Style;
            dataGrid.ColumnHeaderStyle = headerStyle;
            var cellStyle = new Style(typeof(DataGridCell));
            cellStyle.BasedOn = this.TryFindResource("printCellStyle") as Style;
            dataGrid.CellStyle = cellStyle;
            
            if(SelectedNameSheetClassComboBoxItem == null)
            {
                var classColumn = new DataGridTextColumn();
                classColumn.Header = "CLASS";
                classColumn.Width = new DataGridLength(80);
                classColumn.Binding = new Binding("CLASS");
                dataGrid.Columns.Add(classColumn);
            }

            var noColumn = new DataGridTextColumn();
            noColumn.Header = "NO";
            noColumn.Width = new DataGridLength(40);
            noColumn.Binding = new Binding("SNUM");
            dataGrid.Columns.Add(noColumn);

            var nameColumn = new DataGridTextColumn();
            nameColumn.Header = "STUDENT NAME";
            nameColumn.Width = new DataGridLength(200);
            nameColumn.Binding = new Binding("SNAME");
            dataGrid.Columns.Add(nameColumn);

            var phoneColumn = new DataGridTextColumn();
            phoneColumn.Header = "PHONE";
            phoneColumn.Width = new DataGridLength(100);
            phoneColumn.Binding = new Binding("PNUM");
            dataGrid.Columns.Add(phoneColumn);

            var adrColumn = new DataGridTextColumn();
            adrColumn.Header = "ADDRESS";
            adrColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            adrColumn.Binding = new Binding("ADDRESS");
            dataGrid.Columns.Add(adrColumn);

            /* 데이터 추출 */
            var studentDataTable = adminDAO.GetFilteredStudentDataTable(originalDataTable, 
                                                                    SelectedNameSheetGradeComboBoxItem, 
                                                                    SelectedNameSheetClassComboBoxItem);
            dataGrid.ItemsSource = studentDataTable.AsDataView();
            
            tableArea.Children.Add(dataGrid);

            var fixedDocument = GetFixedDocument(background, new PrintDialog());            
            PrintDialog_DocumentViewer.Document = fixedDocument;

            DocumentProgressBar.Dispatcher.Invoke(() =>
                       DocumentProgressBar.Value = 100, DispatcherPriority.Background);

        }

        private void DisplaySubjectReport()
        {
            var fixedDocuments = new FixedDocumentSequence();

            double count = 0;
            double max = classList.Count;

            foreach (var classItem in classList)
            {
                DocumentProgressBar.Dispatcher.Invoke(() =>
                       DocumentProgressBar.Value = 100.0 * ++count / max, DispatcherPriority.Background);

                /* 전체 레이아웃 */
                var background = new Grid();
                background.Width = 750;
                background.Margin = new Thickness(20);
                background.Background = Brushes.Transparent;

                Image image = new Image();
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.UriSource = new Uri("pack://application:,,,/Tasma;component/Resources/Tasma_DepartmentMark.png");
                bit.EndInit();
                image.Source = bit;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Width = 120;
                image.Height = 120;
                background.Children.Add(image);

                var layout = new StackPanel();
                layout.Orientation = Orientation.Vertical;
                layout.HorizontalAlignment = HorizontalAlignment.Center;
                layout.Width = 750;
                background.Children.Add(layout);

                /* 타이틀 영역 */
                var titleArea = new StackPanel();
                titleArea.HorizontalAlignment = HorizontalAlignment.Center;
                titleArea.VerticalAlignment = VerticalAlignment.Center;
                titleArea.Height = 120;
                titleArea.Background = Brushes.Transparent;
                titleArea.Margin = new Thickness(0, 0, 0, 15);
                layout.Children.Add(titleArea);

                /* 분급 영역 */
                var descArea = new Grid();
                descArea.Height = 30;
                layout.Children.Add(descArea);

                /* 표 영역 */
                var tableArea = new StackPanel();
                tableArea.HorizontalAlignment = HorizontalAlignment.Center;
                layout.Children.Add(tableArea);

                /* 타이틀 텍스트 */
                var title = new TextBlock();
                title.TextAlignment = TextAlignment.Center;
                title.FontSize = 20;
                title.FontWeight = FontWeights.Bold;
                title.Text = "\n" + region + " CITY COUNCIL\n"
                           + schoolName + "\n"
                           + SelectedSubjectComboBoxItem + " SCORE TABLE";
                titleArea.Children.Add(title);

                /* 분급 텍스트 */
                var master = new TextBlock();
                master.TextAlignment = TextAlignment.Left;
                master.FontSize = 12;
                master.Text = Space(5) + "MASTER:";
                descArea.Children.Add(master);

                var semText = new TextBlock();
                semText.TextAlignment = TextAlignment.Left;
                semText.FontSize = 12;
                semText.Text = Space(50) + "SEMESTER:" + Space(3) + SelectedSubjectSemesterComboBoxItem.Value;
                descArea.Children.Add(semText);

                var gradeText = new TextBlock();
                gradeText.TextAlignment = TextAlignment.Left;
                gradeText.FontSize = 12;
                gradeText.Text = Space(80) + "GRADE:" + Space(3) + classItem.Item1;
                descArea.Children.Add(gradeText);

                var classText = new TextBlock();
                classText.TextAlignment = TextAlignment.Left;
                classText.FontSize = 12;
                classText.Text = Space(110) + "CLASS:" + Space(3) + classItem.Item2;
                descArea.Children.Add(classText);

                var yearText = new TextBlock();
                yearText.TextAlignment = TextAlignment.Left;
                yearText.FontSize = 12;
                yearText.Text = Space(140) + "YEAR:" + Space(3) + year;
                descArea.Children.Add(yearText);

                /* 학생 테이블 */
                DataGrid dataGrid = new DataGrid();
                dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
                dataGrid.Width = 720;
                dataGrid.BorderBrush = Brushes.Black;
                dataGrid.Foreground = Brushes.Black;
                dataGrid.Background = Brushes.White;
                dataGrid.AutoGenerateColumns = false;
                dataGrid.CanUserAddRows = false;
                dataGrid.Visibility = Visibility.Visible;
                var headerStyle = new Style(typeof(DataGridColumnHeader));
                headerStyle.BasedOn = this.TryFindResource("printHeaderStyle") as Style;
                dataGrid.ColumnHeaderStyle = headerStyle;
                var cellStyle = new Style(typeof(DataGridCell));
                cellStyle.BasedOn = this.TryFindResource("printCellStyle") as Style;
                dataGrid.CellStyle = cellStyle;

                var noColumn = new DataGridTextColumn();
                noColumn.Header = "NO";
                noColumn.Width = new DataGridLength(40);
                noColumn.Binding = new Binding("SNUM");
                dataGrid.Columns.Add(noColumn);

                var nameColumn = new DataGridTextColumn();
                nameColumn.Header = "STUDENT NAME";
                nameColumn.Width = new DataGridLength(200);
                nameColumn.Binding = new Binding("SNAME");
                dataGrid.Columns.Add(nameColumn);

                var evaluationList = adminDAO.GetEvaluationList(SelectedSubjectComboBoxItem);
           
                foreach(var evaluation in evaluationList)
                {
                    var evalColumn = new DataGridTextColumn();
                    evalColumn.Header = evaluation.ToUpper();
                    evalColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    evalColumn.Binding = new Binding(evaluation);
                    dataGrid.Columns.Add(evalColumn);
                }

                var avgColumn = new DataGridTextColumn();
                avgColumn.Header = "AVERAGE";
                avgColumn.Width = new DataGridLength(60);
                avgColumn.Binding = new Binding("AVERAGE");
                dataGrid.Columns.Add(avgColumn);

                var ratingColumn = new DataGridTextColumn();
                ratingColumn.Header = "GRADE";
                ratingColumn.Width = new DataGridLength(60);
                ratingColumn.Binding = new Binding("RATING");
                dataGrid.Columns.Add(ratingColumn);

                var posColumn = new DataGridTextColumn();
                posColumn.Header = "POSITION";
                posColumn.Width = new DataGridLength(60);
                posColumn.Binding = new Binding("POSITION");
                dataGrid.Columns.Add(posColumn);

                tableArea.Children.Add(dataGrid);

                var dataTable = adminDAO.GetFilteredSubjectDataTable(
                                                                 SelectedSubjectSemesterComboBoxItem.Key,
                                                                 classItem.Item1,
                                                                 classItem.Item2,
                                                                 SelectedSubjectComboBoxItem,
                                                                 originalDataTable);
                dataGrid.ItemsSource = dataTable.AsDataView();

                /* 문서 생성 */
                var fixedDocument = GetFixedDocument(background, new PrintDialog());
                var documentReference = new DocumentReference();
                documentReference.SetDocument(fixedDocument);
                fixedDocuments.References.Add(documentReference);
            }

            PrintDialog_DocumentViewer.Document = fixedDocuments;

        }

        private void DisplayStudentReport()
        {
            var fixedDocuments = new FixedDocumentSequence();

            double max = studentList.Count;
            double count = 0.0;

            foreach (var studentItem in studentList)
            {
                
                DocumentProgressBar.Dispatcher.Invoke(() => 
                        DocumentProgressBar.Value = 100.0 * ++count / max, DispatcherPriority.Background);

                /* 전체 레이아웃 */
                var background = new Grid();
                background.Width = 750;
                background.Margin = new Thickness(20);
                background.Background = Brushes.Transparent;

                Image image = new Image();
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.UriSource = new Uri("pack://application:,,,/Tasma;component/Resources/Tasma_DepartmentMark.png");
                bit.EndInit();
                image.Source = bit;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Width = 120;
                image.Height = 120;
                background.Children.Add(image);

                var layout = new StackPanel();
                layout.Orientation = Orientation.Vertical;
                layout.HorizontalAlignment = HorizontalAlignment.Center;
                layout.Width = 750;
                background.Children.Add(layout);

                /* 타이틀 영역 */
                var titleArea = new StackPanel();
                titleArea.HorizontalAlignment = HorizontalAlignment.Center;
                titleArea.VerticalAlignment = VerticalAlignment.Center;
                titleArea.Height = 120;
                titleArea.Background = Brushes.Transparent;
                titleArea.Margin = new Thickness(0, 0, 0, 15);
                layout.Children.Add(titleArea);

                /* 분급 영역 */
                var descArea = new Grid();
                descArea.Height = 30;
                layout.Children.Add(descArea);

                /* 표 영역 */
                var tableArea = new StackPanel();
                tableArea.Orientation = Orientation.Vertical;
                layout.Children.Add(tableArea);

                /* 전체 점수 영역 */
                var totalArea = new Grid();
                descArea.Height = 30;
                layout.Children.Add(totalArea);


                /* 타이틀 텍스트 */
                var title = new TextBlock();
                title.TextAlignment = TextAlignment.Center;
                title.FontSize = 16;
                title.FontWeight = FontWeights.Bold;
                title.Text = "\n" + "HALMASHAURI YA WILAYA YA " + region + "\n"
                           + schoolName + "\n"
                           + address + "\n"
                           + "TAARIFA YA MAENDELEO YA MWANAFUNZI";
                titleArea.Children.Add(title);

                /* 분급 텍스트 */
                var nameText = new TextBlock();
                nameText.TextAlignment = TextAlignment.Left;
                nameText.FontSize = 11;
                nameText.Text = Space(5) + "JINA:" + Space(3) + studentItem.Item4;
                descArea.Children.Add(nameText);

                var semText = new TextBlock();
                semText.TextAlignment = TextAlignment.Left;
                semText.FontSize = 11;
                semText.Text = Space(55) + "MUHULA:" + Space(3) + SelectedStudentSemesterComboBoxItem.Value;
                descArea.Children.Add(semText);

                var gradeText = new TextBlock();
                gradeText.TextAlignment = TextAlignment.Left;
                gradeText.FontSize = 11;
                gradeText.Text = Space(82) + "GRADE:" + Space(3) + studentItem.Item1;
                descArea.Children.Add(gradeText);

                var classText = new TextBlock();
                classText.TextAlignment = TextAlignment.Left;
                classText.FontSize = 11;
                classText.Text = Space(109) + "CLASS:" + Space(3) + studentItem.Item2;
                descArea.Children.Add(classText);

                var numberText = new TextBlock();
                numberText.TextAlignment = TextAlignment.Left;
                numberText.FontSize = 11;
                numberText.Text = Space(136) + "NAMBA:" + Space(3) + studentItem.Item3;
                descArea.Children.Add(numberText);

                var yearText = new TextBlock();
                yearText.TextAlignment = TextAlignment.Left;
                yearText.FontSize = 11;
                yearText.Text = Space(163) + "MWAKA:" + Space(3) + year;
                descArea.Children.Add(yearText);

                var subjectList = adminDAO.GetClassSubjects(studentItem.Item1, studentItem.Item2);
                int subjectCount = 1;

                foreach (var subjectName in subjectList)
                {
                    
                    /* 학생 테이블 */
                    var tableDesc = new TextBlock();
                    tableDesc.TextAlignment = TextAlignment.Left;
                    tableDesc.FontWeight = FontWeights.Bold;
                    tableDesc.FontSize = 11;
                    tableDesc.Text = Space(5) + subjectCount++ + "." + Space(3) + subjectName + "\n";
                    tableArea.Children.Add(tableDesc);

                    DataGrid dataGrid = new DataGrid();
                    dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
                    dataGrid.Width = 720;
                    dataGrid.BorderBrush = Brushes.Black;
                    dataGrid.Foreground = Brushes.Black;
                    dataGrid.Background = Brushes.White;
                    dataGrid.AutoGenerateColumns = false;
                    dataGrid.CanUserAddRows = false;
                    dataGrid.Visibility = Visibility.Visible;
                    var headerStyle = new Style(typeof(DataGridColumnHeader));
                    headerStyle.BasedOn = this.TryFindResource("printHeaderStyle") as Style;
                    dataGrid.ColumnHeaderStyle = headerStyle;
                    var cellStyle = new Style(typeof(DataGridCell));
                    cellStyle.BasedOn = this.TryFindResource("printCellStyle") as Style;
                    dataGrid.CellStyle = cellStyle;

                    var evaluationList = adminDAO.GetEvaluationList(subjectName);

                    foreach (var evaluation in evaluationList)
                    {
                        var evalColumn = new DataGridTextColumn();
                        evalColumn.Header = evaluation.ToUpper();
                        evalColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                        evalColumn.Binding = new Binding(evaluation);
                        dataGrid.Columns.Add(evalColumn);
                    }

                    var avgColumn = new DataGridTextColumn();
                    avgColumn.Header = "WASTANI";
                    avgColumn.Width = new DataGridLength(70);
                    avgColumn.Binding = new Binding("AVERAGE");
                    dataGrid.Columns.Add(avgColumn);

                    var ratingColumn = new DataGridTextColumn();
                    ratingColumn.Header = "DARAJA";
                    ratingColumn.Width = new DataGridLength(70);
                    ratingColumn.Binding = new Binding("RATING");
                    dataGrid.Columns.Add(ratingColumn);

                    var posColumn = new DataGridTextColumn();
                    posColumn.Header = "NAFASI";
                    posColumn.Width = new DataGridLength(70);
                    posColumn.Binding = new Binding("POSITION");
                    dataGrid.Columns.Add(posColumn);

                    var dataTable = adminDAO.GetScoreDataTableForStudent(SelectedStudentSemesterComboBoxItem.Key,
                                                                         studentItem.Item1,
                                                                         studentItem.Item2,
                                                                         subjectName,
                                                                         studentItem.Item3);
                    if (dataTable == null)
                        continue;

                    dataGrid.ItemsSource = dataTable.AsDataView();
                    tableArea.Children.Add(dataGrid);

                    var tableNewLine = new TextBlock();
                    tableNewLine.TextAlignment = TextAlignment.Left;
                    tableNewLine.FontSize = 11;
                    tableNewLine.Text = "\n";
                    tableArea.Children.Add(tableNewLine);

                    var total = adminDAO.GetStudentTotalPosition(SelectedStudentSemesterComboBoxItem.Key,
                                                                         studentItem.Item1,
                                                                         studentItem.Item2,
                                                                         studentItem.Item3);

                    var totalText = new TextBlock();
                    totalText.TextAlignment = TextAlignment.Left;
                    totalText.FontSize = 11;
                    totalText.Text = Space(5) + "WASTANI WA ALAMA:" + Space(3) + total.Item1;
                    totalArea.Children.Add(totalText);

                    var avgText = new TextBlock();
                    avgText.TextAlignment = TextAlignment.Left;
                    avgText.FontSize = 11;
                    avgText.Text = Space(50) + "WASTANI WA WANAFUNZI:" + Space(3) + total.Item2;
                    totalArea.Children.Add(avgText);


                }

                /* 문서 생성 */
                var fixedDocument = GetFixedDocument(background, new PrintDialog());
                var documentReference = new DocumentReference();
                documentReference.SetDocument(fixedDocument);
                fixedDocuments.References.Add(documentReference);
            }

            PrintDialog_DocumentViewer.Document = fixedDocuments;
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// UI를 통해 문서를 만들어 냅니다.
        /// </summary>
        /// <param name="toPrint"></param>
        /// <param name="printDialog"></param>
        /// <returns></returns>
        private FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog)
        {
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument fixedDoc = new FixedDocument();
            toPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            toPrint.Arrange(new Rect(new Point(0, 0), toPrint.DesiredSize));
              
            Size size = toPrint.DesiredSize;
            
            double yOffset = 0;
            while (yOffset < size.Height)
            {
                VisualBrush vb = new VisualBrush(toPrint);
                vb.Stretch = Stretch.None;
                vb.AlignmentX = AlignmentX.Left;
                vb.AlignmentY = AlignmentY.Top;
                vb.ViewboxUnits = BrushMappingMode.Absolute;
                vb.TileMode = TileMode.None;
                vb.Viewbox = new Rect(0, yOffset, visibleSize.Width, visibleSize.Height);
                PageContent pageContent = new PageContent();
                FixedPage page = new FixedPage();
                ((IAddChild)pageContent).AddChild(page);
                fixedDoc.Pages.Add(pageContent);
                page.Width = pageSize.Width;
                page.Height = pageSize.Height;
                Canvas canvas = new Canvas();
                FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
                FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
                canvas.Width = visibleSize.Width;
                canvas.Height = visibleSize.Height;
                canvas.Background = vb;
                page.Children.Add(canvas);
                yOffset += visibleSize.Height;
            }
            return fixedDoc;
        }

        private string Space(int count)
        {
            var result = "";
            for (int i = 0; i < count; ++i)
                result += " ";

            return result;
        }

        //private void DocumentProgressAsync(DoWorkEventHandler method)
        //{
        //    backgroundWorker.CancelAsync();
        //    backgroundWorker = null;
        //    backgroundWorker = new BackgroundWorker();
        //    backgroundWorker.WorkerSupportsCancellation = true;
        //    backgroundWorker.WorkerReportsProgress = true;
        //    backgroundWorker.ProgressChanged += (sender, e) =>
        //    {
        //        ProgressValue = e.ProgressPercentage;
        //    };
        //    backgroundWorker.DoWork += method;
        //    backgroundWorker.RunWorkerAsync();
        //}

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
