using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Text;
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
using TASMA.Database;

namespace TASMA.Window
{
    /// <summary>
    /// PrintDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportDialog : System.Windows.Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AdminDAO adminDAO;
        private DataTable originalDataTable;

        private string schoolName;
        private string year;
        private string region;
        private string address;

        /* ReportListBox Properties */
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


        /* NameSheetOption Properties */
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

        /* SubjectOption Properties */
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

            /* 메타 데이터 획득 루틴 */
            var metaData = adminDAO.GetDBInfo();
            schoolName = metaData[0];
            year = metaData[1];
            region = metaData[2];
            address = metaData[3];

            DataContext = this;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            /* Datatable 예외처리 */
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

            SubjectComboBoxItems = new ObservableCollection<string>();

            /* Report 리스트박스 초기화 */
            ReportListBoxItems = new ObservableCollection<string>();
            ReportListBoxItems.Add("NameSheet");
            ReportListBoxItems.Add("Subject");
            ReportListBoxItems.Add("Student");

            /* Semester 초기화 */
            if(1 < DateTime.Now.Month && DateTime.Now.Month <= 7)
            {
                SelectedSubjectSemesterComboBoxItem = SubjectSemesterComboBoxItems[0];
            }else
            {
                SelectedSubjectSemesterComboBoxItem = SubjectSemesterComboBoxItems[1];
            }

            SelectedReportListBoxItem = "NameSheet";

            PrintDialog_DocumentViewer.FitToWidth();
        }

        private void OnReportListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedReportListBoxItem == "NameSheet")
            {
                NameSheetOption.Visibility = Visibility.Visible;
                SubjectReportOption.Visibility = Visibility.Hidden;
                StudentReportOption.Visibility = Visibility.Hidden;

                /* 바인딩 상태 초기화 */
                NameSheetGradeComboBoxItems.Clear();
                SelectedNameSheetGradeComboBoxItem = null;

                NameSheetClassComboBoxItems.Clear();
                SelectedNameSheetClassComboBoxItem = null;

                /* Grade ComboBox 초기화 */
                var gradeList = originalDataTable.AsEnumerable().Select(r => r.Field<string>("GRADE")).Distinct().ToList();
                foreach (var gradeName in gradeList)
                    NameSheetGradeComboBoxItems.Add(gradeName);
                if (gradeList.Count != 0)
                    SelectedNameSheetGradeComboBoxItem = gradeList[0];

            }
            else if (SelectedReportListBoxItem == "Subject")
            {
                NameSheetOption.Visibility = Visibility.Hidden;
                SubjectReportOption.Visibility = Visibility.Visible;
                StudentReportOption.Visibility = Visibility.Hidden;

                /* 바인딩 상태 초기화 */
                SubjectComboBoxItems.Clear();
               
                /* Subject ComboBox 초기화 */
                var allClasses = originalDataTable.AsEnumerable().Select
                                                   (row => new {
                                                       GradeName = row.Field<string>("GRADE"),
                                                       ClassName = row.Field<string>("CLASS") }).Distinct();

                var finalSubjectList = new List<string>();
                foreach (var pair in allClasses)
                {
                    var subjectList = adminDAO.GetClassSubjects(pair.GradeName, pair.ClassName);
                    finalSubjectList = finalSubjectList.Union(subjectList).ToList();
                }

                

            }
            else if(SelectedReportListBoxItem == "Student")
            {
                NameSheetOption.Visibility = Visibility.Hidden;
                SubjectReportOption.Visibility = Visibility.Hidden;
                StudentReportOption.Visibility = Visibility.Visible;
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
            /* Make & Show FixedDocuments */
        }

        private void SubjectComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* 바인딩 상태 초기화 */

            /* Make & Show FixedDocuments */
        }


        private void DisplayNameSheet()
        {
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
