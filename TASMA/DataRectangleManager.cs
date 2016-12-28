using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASMA
{
    namespace DataInterfaces
    {
        /// <summary>
        /// 현제 데이터박스가 수정 중 여부를 반환한다.
        /// </summary>
        static class DataRectangleManager
        {
            private static bool isModified = true;
            private static List<string> gradeList;
            
            public static bool IsModified
            {
                get { return isModified; }
                set { isModified = value; }
            }
        }
    }
}
