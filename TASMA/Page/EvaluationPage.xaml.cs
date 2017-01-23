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
using TASMA.Dialog;
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

        private ObservableCollection<Evaluation> evaluationListBoxItems;
        public ObservableCollection<Evaluation> EvaluationListBoxItems
        {
            get { return evaluationListBoxItems; }
            set { evaluationListBoxItems = value; OnPropertyChanged("EvaluationListBoxItems"); }
        }

        private Evaluation selectedListBoxItem;
        public Evaluation SelectedListBoxItem
        {
            get { return selectedListBoxItem; }
            set { selectedListBoxItem = value;
                OnPropertyChanged("SelectedListBoxItem"); }
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
            var evaluationList = adminDAO.GetEvaluationAndRatio(subjectName);
            evaluationListBoxItems = new ObservableCollection<Evaluation>(); 

            foreach (var evaluationData in evaluationList)
            {
                evaluationListBoxItems.Add(new Evaluation
                {
                    Key = evaluationData.Item1,
                    Value = evaluationData.Item1 + "(" + evaluationData.Item2 + "%)",
                    Ratio = evaluationData.Item2
                });
             }

            /* 트리 뷰 이벤트 추가 */
            foreach (var gradeItem in subjectTreeViewItems)
            {
                gradeItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;

                if (gradeItem.Children == null)
                    continue;

                foreach (var classItem in gradeItem.Children)
                {
                    classItem.PropertyChanged += OnSubjectTreeViewItemPropertyChanged;
                }
            }


            DataContext = this;
            InitializeComponent();

            EvaluationPage_Subject.Text = subjectName;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
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
                        if(!classItem.IsChecked)
                            classItem.IsChecked = item.IsChecked;
                    }
                }
                else//Grade에 체크 해제 - 하위 클래스들이 모두 체크 상태인 경우 모두 체크 해제 상태로 전환합니다.
                {
                    foreach (var child in item.Children)
                    {
                        if (!child.IsChecked)
                            return;
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

                    adminDAO.UnRegisterClassOnSubject(this.subjectName, item.Parent.Name, item.Name);
                }
                else
                {
                    adminDAO.RegisterClassOnSubject(this.subjectName, item.Parent.Name, item.Name);
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
            var evaluationList = new List<string>();
            foreach (var evaluationItem in evaluationListBoxItems)
                evaluationList.Add(evaluationItem.Key);

            var ied = new InputEvaluationDialog(evaluationList);
            ied.ShowDialog();

            if (!ied.IsDetermined)
                return;

            var evaluation = ied.Evaluation;
            var ratio = ied.Ratio;

            int currentSum = 0;
            foreach (var evaluationItem in EvaluationListBoxItems)
                currentSum += evaluationItem.Ratio;

            if (currentSum + ratio > 100)
            {
                var ratioCount = EvaluationListBoxItems.Count;
                var newRatio = (100 - ratio) / ratioCount;

                for (int i = 0; i < ratioCount; ++i)
                {
                    EvaluationListBoxItems[i] = new Evaluation { Key = EvaluationListBoxItems[i].Key,
                                                                 Value = EvaluationListBoxItems[i].Key + "(" + newRatio + "%)",
                                                                 Ratio = newRatio
                                                               };
                    adminDAO.UpdateRatio(this.subjectName, EvaluationListBoxItems[i].Key, newRatio);                                     
                }
            }

            EvaluationListBoxItems.Add(new Evaluation{
                                                        Key = ied.Evaluation,
                                                        Value = ied.Evaluation + "(" + ied.Ratio + "%)",
                                                        Ratio = ied.Ratio
                                                     });
            adminDAO.CreateEvaluation(this.subjectName, ied.Evaluation);
            adminDAO.UpdateRatio(this.subjectName, ied.Evaluation, ied.Ratio);
            SelectedListBoxItem = EvaluationListBoxItems[EvaluationListBoxItems.Count - 1];
        }

        /// <summary>
        /// 선택한 평가 항목을 수정하고 데이터베이스에 반영합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModifyListBoxItem(object sender, RoutedEventArgs e)
        {
            if (SelectedListBoxItem == null)
                return;

            var evaluationList = new List<string>();
            foreach (var evaluationItem in evaluationListBoxItems)
                evaluationList.Add(evaluationItem.Key);

            var ied = new InputEvaluationDialog(evaluationList, SelectedListBoxItem.Key, SelectedListBoxItem.Ratio);
            ied.ShowDialog();

            if (!ied.IsDetermined)
                return;

            adminDAO.UpdateEvaluation(this.subjectName, SelectedListBoxItem.Key, ied.Evaluation);
            adminDAO.UpdateRatio(this.subjectName, ied.Evaluation, ied.Ratio);

            int selectedIndex = -1;

            for (int i = 0; i < EvaluationListBoxItems.Count; ++i)
            {
                if(SelectedListBoxItem.Key == EvaluationListBoxItems[i].Key)
                {
                    EvaluationListBoxItems[i] = new Evaluation { Key = ied.Evaluation, Value = ied.Evaluation + "(" + ied.Ratio + "%)", Ratio = ied.Ratio };
                    SelectedListBoxItem = EvaluationListBoxItems[i];
                    selectedIndex = i;
                    break;
                }
            }
            
            var ratio = ied.Ratio;

            int currentSum = 0;
            foreach (var evaluationItem in EvaluationListBoxItems)
            {
                currentSum += evaluationItem.Ratio;
            }

            if (currentSum > 100)
            {
                var ratioCount = EvaluationListBoxItems.Count - 1;
                var newRatio = (100 - ratio) / ratioCount;

                for (int i = 0; i < ratioCount; ++i)
                {
                    if (i == selectedIndex)
                        continue;

                    EvaluationListBoxItems[i] = new Evaluation
                    {
                        Key = EvaluationListBoxItems[i].Key,
                        Value = EvaluationListBoxItems[i].Key + "(" + newRatio + "%)",
                        Ratio = newRatio
                    };
                    adminDAO.UpdateRatio(this.subjectName, EvaluationListBoxItems[i].Key, newRatio);
                }
            }
        }

        /// <summary>
        /// 선택한 평가 항목을 리스트에서 지웁니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteListBoxItem(object sender, RoutedEventArgs e)
        {
            if (SelectedListBoxItem == null)
                return;

            var dialog = new TasmaConfirmationMessageBox("Delete evaluation", "Are you sure delete evaluation?");
            dialog.ShowDialog();


            if (dialog.Yes)
            {
                adminDAO.DeleteEvaluation(this.subjectName, SelectedListBoxItem.Key);
                evaluationListBoxItems.Remove(SelectedListBoxItem);
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
            nav.Navigate(new SubjectPage(adminDAO));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}


