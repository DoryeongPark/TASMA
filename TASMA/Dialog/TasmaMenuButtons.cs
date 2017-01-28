using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TASMA.Dialog
{
    public static class TasmaMenuButtons
    {
        private static List<Button> tasmaMenuButtons;
        public static List<Button> Buttons 
        {
            get { return tasmaMenuButtons;  }
            set { tasmaMenuButtons = value; }
        }
        
        public static void UpdateButtonsState (Button buttonClicked)
        { 
            if (Exists(buttonClicked))
            {
                foreach(var menuButton in tasmaMenuButtons)
                {
                    if(menuButton == buttonClicked)
                    {
                        menuButton.IsDefault = true;
                        menuButton.IsEnabled = false;
                    }else
                    {
                        menuButton.IsDefault = false;
                        menuButton.IsEnabled = true;  
                    }
                }

            }
        }

        public static bool Exists(Button button)
        {
            foreach(var menuButton in tasmaMenuButtons)
            {
                if (menuButton == button)
                    return true;
            }

            return false;
        }
    }
}
