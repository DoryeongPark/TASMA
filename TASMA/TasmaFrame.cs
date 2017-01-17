using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TASMA
{
    public static class TasmaFrame
    {
        private static Frame frame;
        public static Frame Frame
        {
            get { return frame; }
            set { frame = value; }
        }
    }
}
