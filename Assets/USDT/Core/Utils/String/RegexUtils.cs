using System.Text.RegularExpressions;


namespace USDT.Utils {

    public static class RegexUtils {
        public static bool HasChinese(string text) {
            return text != null && RegexConst.RegexChs.IsMatch(text);
        }
    }

    public static class RegexConst {
        public static Regex RegexChs = new Regex("[\u4e00-\u9fa5]");
        /// <summary>
        /// Namespace�м�����
        /// </summary>
        public static Regex RegexNamespaceMiddleContent = new Regex(@"namespace\s+[\w\.]+\s*\{");
    }
}