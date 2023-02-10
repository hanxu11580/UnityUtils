using System.Text.RegularExpressions;


namespace Utility {

    public static class RegexUtils {
        public static bool HasChinese(string text) {
            return text != null && RegexConst.RegexChs.IsMatch(text);
        }
    }

    public static class RegexConst {
        public static Regex RegexChs = new Regex("[\u4e00-\u9fa5]");
    }
}