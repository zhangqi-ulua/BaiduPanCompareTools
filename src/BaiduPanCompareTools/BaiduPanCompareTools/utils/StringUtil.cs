using System.Text;

namespace BaiduPanCompareTools.utils
{
    internal class StringUtil
    {
        /// <summary>
        /// 将List中的所有数据用指定分隔符连接为一个新字符串
        /// </summary>
        public static string CombineString<T>(IList<T> list, string separateString)
        {
            if (list == null || list.Count < 1)
                return "";
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(list[0]);
                for (int i = 1; i < list.Count; ++i)
                    builder.Append(separateString).Append(list[i]);

                return builder.ToString();
            }
        }
    }
}
