using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ZdezClientForWP.Themes
{
    /// <summary>
    /// 主题相关的附加功能，适用与无法动态绑定的各种情况
    /// </summary>
    internal static class ThemeAddition
    {

        /// <summary>
        /// 定义主题橙色
        /// </summary>
        public static Color OrangeColor { get; private set; }

        static ThemeAddition()
        {
            
            // #FFF09609
            OrangeColor = Color.FromArgb(0xFF, 0xF0, 0x96, 0x09);

        }

    }
}
