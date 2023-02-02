using System.Collections;
using System.Collections.Generic;

namespace Utility {
    public static class MathUtils
    {
        /// <summary> 是否为偶数 </summary>
        public static bool IsEvennumber(int val)
        {
            return (val & 1) == 0; //与1进行二进制 二进制偶数最后一位为 0
        }
    }

}
