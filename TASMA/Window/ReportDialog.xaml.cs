using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        
        public ReportDialog(AdminDAO adminDAO, DataTable originalDataTable)
        {
            this.adminDAO = adminDAO;

            /* 데이터 테이블 복사 및 필터링 처리 루틴 */
            object copiedDataTable = originalDataTable.Clone() as object;
            originalDataTable = copiedDataTable as DataTable;
            
            for (int i = 0; i < originalDataTable.Rows.Count; ++i)
            {
                if((bool)originalDataTable.Rows[i]["Print"] == false)
                {
                    originalDataTable.Rows.RemoveAt(i);
                    i = -1;
                }
            }
                        
            /* 메타 데이터 루틴 */
            var metaData = adminDAO.GetDBInfo();
            
            
            
            
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PrintDialog_DocumentViewer.FitToWidth();
            OnStudentListReportSelected();
        }

        private void OnStudentListReportSelected()
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
            background.Children.Add(layout);
            
            //타이틀 영역
            var titleArea = new StackPanel();
            titleArea.HorizontalAlignment = HorizontalAlignment.Center;
            titleArea.VerticalAlignment = VerticalAlignment.Center;
            titleArea.Height = 120;
            titleArea.Background = Brushes.Transparent;
            layout.Children.Add(titleArea);

            //분급 영역
            var descArea = new StackPanel();
            descArea.Height = 30;
            descArea.HorizontalAlignment = HorizontalAlignment.Center;
            layout.Children.Add(descArea);

            //표 영역
            var tableArea = new StackPanel();
            tableArea.HorizontalAlignment = HorizontalAlignment.Center;
            layout.Children.Add(tableArea);
            
            var title = new TextBlock();
            title.Text = "TITLE";
            titleArea.Children.Add(title);

            var desc = new TextBlock();
            desc.Text = "DESC";
            descArea.Children.Add(desc);

            var table = new TextBlock();
            table.Text = "TABLE";
            tableArea.Children.Add(table);
            
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


    }
}
