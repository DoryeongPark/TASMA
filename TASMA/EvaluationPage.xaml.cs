using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TASMA.Database;
using TASMA.Model;

namespace TASMA
{
    /// <summary>
    /// 과목에 반과 평가항목을 등록하는 페이지 입니다.
    /// </summary>
    public partial class EvaluationPage : Page, INotifyPropertyChanged
    {
        private AdminDAO adminDAO;
        private string subjectName;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<SubjectTreeViewItem> subjectTreeViewItems;
        public ObservableCollection<SubjectTreeViewItem> SubjectTreeViewItems
        {
            get { return subjectTreeViewItems; }
            set { subjectTreeViewItems = value; OnPropertyChanged("SubjectTreeViewItems"); }
        }

        private ObservableCollection<EvaluationListBoxItem> evaluationListBoxItems;
        public ObservableCollection<EvaluationListBoxItem> EvaluationListBoxItems
        {
            get { return evaluationListBoxItems; }
            set { evaluationListBoxItems = value; OnPropertyChanged("EvaluationListBoxItems"); }
        }

        private EvaluationListBoxItem selectedListBoxItem;
        public EvaluationListBoxItem SelectedListBoxItem
        {
            get { return selectedListBoxItem; }
            set { selectedListBoxItem = value; OnPropertyChanged("SelectedListBoxItem"); }
        }

        /// <summary>
        /// 현재 데이터베이스의 상태를 컨트롤에 반영합니다.
        /// </summary>
        /// <param name="adminDAO">Data Access Object</param>
        /// <param name="subjectName">현재 과목</param>
        public EvaluationPage(AdminDAO adminDAO, string subjectName)
        {
            this.adminDAO = adminDAO;
            this.subjectName = subjectName;

            //트리 뷰 데이터 로드(ViewModel)
            subjectTreeViewItems = new ObservableCollection<SubjectTreeViewItem>();
            var gradeList = adminDAO.GetGradeList();

            foreach (var grade in gradeList)
            {
                adminDAO.SelectGrade(grade);

                var gradeItem = new SubjectTreeViewItem()
                {
                    Name = grade,
                    Type = SubjectTreeViewItemType.Grade,
                    IsChecked = false,
                    Children = null,
                    Parent = null
                };

                var classList = adminDAO.GetClassList();
                var classItems = new ObservableCollection<SubjectTreeViewItem>();

                foreach (var classData in classList)
                {
                    var isRegistered = false;
                    var subjectList = adminDAO.GetClassSubjects(grade, classData);
                    foreach (var subjectData in subjectList)
                        if (subjectData == subjectName)
                        {
                            isRegistered = true;
                            break;
                        }

                    var classItem = new SubjectTreeViewItem()
                    {
                        Name = classData,
                        Type = SubjectTreeViewItemType.Class,
                        IsChecked = isRegistered,
                        Children = null,
                        Parent = gradeItem
                    };
                    classItems.Add(classItem);

                    gradeItem.Children = classItems;
                }

                subjectTreeViewItems.Add(gradeItem);
                adminDAO.MovePrevious();
            }


            //리스트박스 데이터 로드(ViewModel)
            var evaluationList = adminDAO.GetEvaluationList(subjectName);
            evaluationListBoxItems = new ObservableCollection<EvaluationListBoxItem>();

            foreach (var evaluationData in evaluationList)
            {
                var evaluationItem = new EvaluationListBoxItem()
                {
                    Name = evaluationData
                };
                evaluationListBoxItems.Add(evaluationItem);
            }

            foreach (var gradeItem in subjectTreeViewItems)
            {
                gradeItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                foreach (var classItem in gradeItem.Children)
                {
                    classItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                }
            }

            foreach (var evaluationItem in evaluationListBoxItems)
            {
                evaluationItem.PropertyChanged += OnEvaluationListBoxItemPropertyChanged;
            }

            DataContext = this;
            InitializeComponent();

            EvaluationPage_Subject.Content = subjectName;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigating += OnClosing;
        }


        private void OnEvaluationListBoxItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// TreeViewItem의 클릭 이벤트 루틴을 실행합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSubjectTreeViewItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SubjectTreeViewItem;

            if (item.Type == SubjectTreeViewItemType.Grade)
            {
                //Grade에 체크 - 하위 Class들을 모두 체크 상태로 전환합니다.
                if (item.IsChecked)
                {
                    foreach (var classItem in item.Children)
                    {
                        classItem.IsChecked = item.IsChecked;
                    }
                }
                else//Grade에 체크 해제 - 하위 클래스들이 모두 체크 상태인 경우 모두 체크 해제 상태로 전환합니다.
                {
                    foreach (var child in item.Children)
                    {
                        if (!child.IsChecked)
                            break;
                    }

                    foreach (var classItem in item.Children)
                    {
                        classItem.IsChecked = false;
                    }
                }
            }
            else
            {
                if (!item.IsChecked)//Class에 체크 해제 - 상위 Grade가 체크 상태일 시 체크 해제 상태로 전환합니다.
                {
                    if (item.Parent != null && item.Parent.IsChecked)
                    {
                        item.Parent.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// 평가 항목을 리스트에 추가합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddListBoxItem(object sender, RoutedEventArgs e)
        {
            var dialog = new TasmaPromptMessageBox("Add evaluation", "Input evaluation name");
            dialog.ShowDialog();

            if (dialog.IsDetermined)
            {
                //Check Duplication
                foreach (var evaluationItem in evaluationListBoxItems)
                    if (evaluationItem.Name.ToUpper() == dialog.Input.ToUpper())
                    {
                        MessageBox.Show("Evaluations are duplicated");
                        return;
                    }
                evaluationListBoxItems.Add(new EvaluationListBoxItem { Name = dialog.Input });
            }
        }

        /// <summary>
        /// 선택한 평가 항목을 수정하고 데이터베이스에 반영합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModifyListBoxItem(object sender, RoutedEventArgs e)
        {
            if (selectedListBoxItem == null)
                return;

            var dialog = new TasmaPromptMessageBox("Modify evaluation", "Input evaluation name", selectedListBoxItem.Name);
            dialog.ShowDialog();

            if (dialog.IsDetermined)
            {
                //Check Duplication
                foreach (var evaluationItem in evaluationListBoxItems)
                    if (evaluationItem.Name.ToUpper() == dialog.Input.ToUpper())
                    {
                        MessageBox.Show("Evaluations are duplicated");
                        return;
                    }

                adminDAO.UpdateEvaluation(subjectName, selectedListBoxItem.Name, dialog.Input);
                selectedListBoxItem.Name = dialog.Input;
            }
        }

        /// <summary>
        /// 선택한 평가 항목을 리스트에서 지웁니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteListBoxItem(object sender, RoutedEventArgs e)
        {
            if (selectedListBoxItem == null)
                return;

            var dialog = new TasmaConfirmationMessageBox("Delete evaluation", "Are you sure delete evaluation? - " + selectedListBoxItem.Name);
            dialog.ShowDialog();

            if (dialog.Yes)
                evaluationListBoxItems.Remove(selectedListBoxItem);
        }



        /// <summary>
        /// 현재 ViewModel의 상태를 데이터베이스에 반영합니다.
        /// </summary>
        private void SaveRoutine()
        {
            var currentClasses = adminDAO.GetSubjectClasses(subjectName);
            var currentEvaluations = adminDAO.GetEvaluationList(subjectName);

            var newEvaluations = new List<string>();
            foreach (var evaluationItem in evaluationListBoxItems)
                newEvaluations.Add(evaluationItem.Name);

            //반 등록 - 현재 데이터베이스에 ViewModel 상태 반영
            foreach (var gradeItem in subjectTreeViewItems)
            {
                var gradeName = gradeItem.Name;
                foreach (var classItem in gradeItem.Children)
                {
                    var className = classItem.Name;
                    var contains = currentClasses.AsEnumerable().Any(row => gradeName == row.Field<string>("Grade")) &&
                                        currentClasses.AsEnumerable().Any(row => className == row.Field<string>("Class"));

                    if (classItem.IsChecked)
                    {
                        if (!contains)//현재 테이블에 존재하지 않지만 체크가 되어 있는 반 -> 삽입
                        {
                            adminDAO.RegisterClassOnSubject(subjectName, gradeName, className);
                        }

                    }
                    else
                    {
                        if (contains)//현재 테이블에 존재하지만 체크가 되어 있지 않는 반 -> 삭제
                        {
                            adminDAO.UnRegisterClassOnSubject(subjectName, gradeName, className);
                        }
                    }
                }
            }

            //평가 항목 등록 - 현재 데이터베이스에 ViewModel 상태 반영
            foreach (var newEvaluation in newEvaluations)
            {
                //현재 리스트에 존재하지 않지만, 새 리스트에 포함 -> 추가
                if (!currentEvaluations.Contains(newEvaluation))
                {
                    adminDAO.CreateEvaluation(subjectName, newEvaluation);
                }
            }
            foreach (var currentEvaluation in currentEvaluations)
            {
                //현재 리스트에 존재하지만, 새 리스트에 포함되지 않음 -> 삭제
                if (!newEvaluations.Contains(currentEvaluation))
                {
                    adminDAO.DeleteEvaluation(subjectName, currentEvaluation);
                }
            }

        }

        /// <summary>
        /// 이전 페이지로 이동합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            var nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new GradePage(adminDAO));
        }

        /// <summary>
        /// 현재 페이지를 빠져나가기 전에 호출되는 루틴입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, NavigatingCancelEventArgs e)
        {
            var dialog = new TasmaConfirmationMessageBox("Save confirmation", "Do you want to save changes?");
            dialog.ShowDialog();

            if (dialog.Yes)
            {
                SaveRoutine();
            }

            var nav = NavigationService.GetNavigationService(this);
            nav.Navigating -= OnClosing;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
    }
}


