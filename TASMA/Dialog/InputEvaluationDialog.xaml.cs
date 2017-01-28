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
using TASMA.MessageBox;

namespace TASMA.Dialog
{
    /// <summary>
    /// InputEvaluationDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputEvaluationDialog : Window
    {
        private bool modificationMode = false;

        private bool isDetermined = false;
        public bool IsDetermined
        {
            get { return isDetermined; }
            set { isDetermined = value; }
        }

        private string evaluation = null;
        public string Evaluation
        {
            get { return evaluation; }
            set { evaluation = value; }
        }

        private int ratio = -1;
        public int Ratio
        {
            get { return ratio; }
            set { ratio = value; }
        }

        private List<string> evaluationList;

        public InputEvaluationDialog(List<string> evaluationList, string evaluation = null, int ratio = -1)
        {
            InitializeComponent();
            this.evaluationList = evaluationList;

            InputEvaluationDialog_EvaluationName.Focus();

            if (evaluation != null && ratio != -1)
            {
                InputEvaluationDialog_EvaluationName.Text = evaluation;
                InputEvaluationDialog_Ratio.Text = ratio.ToString();
                InputEvaluationDialog_EvaluationName.SelectAll();
                modificationMode = true;
            }

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            isDetermined = false;
            Close();
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            var evaluationUnchecked = InputEvaluationDialog_EvaluationName.Text;
            var ratioUnchecked = InputEvaluationDialog_Ratio.Text;

            if (!modificationMode)
            {
                /* Check Duplication */
                foreach (var evaluationName in evaluationList)
                {
                    if (evaluationUnchecked.ToUpper() == evaluationName.ToUpper())
                    {
                        var alert = new TasmaAlertMessageBox("Alert Duplication",
                            "Evaluation name is duplicated");
                        alert.ShowDialog();
                        InputEvaluationDialog_EvaluationName.Focus();
                        InputEvaluationDialog_EvaluationName.SelectAll();
                        return;
                    }
                }
            }

            int ratioConverted;

            /* Check number */
            try
            {
                ratioConverted = int.Parse(ratioUnchecked);
            }
            catch (Exception)
            {
                var alert = new TasmaAlertMessageBox("Incorrect value",
                                                    "Ratio should be number");
                alert.ShowDialog();
                InputEvaluationDialog_Ratio.Focus();
                InputEvaluationDialog_Ratio.SelectAll();
                return;
            }
                
            if(ratioConverted < 0 || 100 < ratioConverted)
            {
                var alert = new TasmaAlertMessageBox("Incorrect number",
                                                "Ratio should be number between 0 ~ 100");
                alert.ShowDialog();
                InputEvaluationDialog_Ratio.Focus();
                InputEvaluationDialog_Ratio.SelectAll();
                return;
            }

      
            isDetermined = true;
            evaluation = evaluationUnchecked;
            ratio = ratioConverted;
            Close();
        }
    }
}
