using System.Text;

namespace BaiduPanCompareTools
{
    internal class AppConsts
    {
        // 本工具保存的百度网盘快照文件扩展名
        public const string SNAPSHOOT_FILE_EXTENSION = "bdps";
        // 请求百度网盘时的User-Agent
        public const string USER_AGENT = "netdisk";
        // 连续发出请求百度网盘服务器的时间间隔（毫秒）
        public const int REQ_BAIDU_PAN_API_INTERVAL = 1000;

        // 百度网盘链接地址的开头字符串
        public const string BAIDU_PAN_URL_PREFIX = "https://pan.baidu.com/s/";
        // 带有提取码的百度网盘链接中的提取码参数名
        public const string BAIDU_PAN_URL_ACCESS_CODE_PARAM_NAME = "?pwd=";
        // 百度网盘提取码的位数
        public const int BAIDU_PAN_ACCESS_CODE_LENGTH = 4;
        // 百度网盘请求share/list时，每页固定返回的文件夹或文件最大数量
        public const int SHARE_LIST_ONE_PAGE_MAX_NUM = 100;
        // 百度网盘链接不存在时的提示
        public const string BAIDU_PAN_URL_NOT_FOUND_TIPS = "啊哦，你所访问的页面不存在了。";
        // 百度网盘输入提取码页面的提示
        public const string BAIDU_PAN_URL_INPUT_ACCESS_CODE_TIPS = "请输入提取码";
        // 链接错误码对应的描述文本
        public static readonly Dictionary<int, string> ERROR_TYPE_TO_DESC = new Dictionary<int, string>()
        {
            {0, "啊哦，你来晚了，分享的文件已经被删除了，下次要早点哟。"},
            {1, "啊哦，你来晚了，分享的文件已经被取消了，下次要早点哟。"},
            {2, "此链接分享内容暂时不可访问"},
            {3, "此链接分享内容可能因为涉及侵权、色情、反动、低俗等信息，无法访问！"},
            {5, "啊哦！链接错误没找到文件，请打开正确的分享链接!"},
            {10, "啊哦，来晚了，该分享文件已过期"},
            {11, "由于访问次数过多，该分享链接已失效"},
            {12, "因该分享含有自动备份文件夹，暂无法查看"},
            {15, "系统升级，链接暂时无法查看，升级完成后恢复正常。"},
            {17, "该链接访问范围受限，请使用正常的访问方式"},
            {123, "该链接已超过访问人数上限，可联系分享者重新分享"},
            {124, "您访问的链接已被冻结，可联系分享者进行激活"},
        };

        /// <summary>
        /// 判断输入的百度网盘提取码格式是否正确，必须为4位英文字母（不区分大小写）或数字
        /// </summary>
        /// <param name="accessCode">要验证的提取码</param>
        /// <param name="errorString">如果校验不通过，返回错误信息</param>
        /// <returns></returns>
        public static bool CheckBaiduPanAccessCodeFormat(string accessCode, out string errorString)
        {
            if (accessCode.Length != BAIDU_PAN_ACCESS_CODE_LENGTH)
            {
                errorString = $"提取码错应为{BAIDU_PAN_ACCESS_CODE_LENGTH}位";
                return false;
            }
            foreach (char c in accessCode)
            {
                if (!(c >= 'a' && c <= 'z') && !(c >= 'A' && c <= 'Z') && !(c >= '0' && c <= '9'))
                {
                    errorString = $"提取码必须由英文字母和数字组成";
                    return false;
                }
            }

            errorString = null;
            return true;
        }

        /// <summary>
        /// 将本地文件的MD5值转为百度服务器上记录的百度MD5
        /// </summary>
        public static string Md5ToBaiduMd5(string md5)
        {
            string i = string.Concat(md5.Substring(8, 8), md5.Substring(0, 8), md5.Substring(24, 8), md5.Substring(16, 8));
            StringBuilder oSb = new StringBuilder();
            for (int a = 0; a < i.Length; a++)
                oSb.Append((Convert.ToInt32(i[a].ToString(), 16) ^ (15 & a)).ToString("x"));

            string o = oSb.ToString();
            char s = (char)((int)'g' + Convert.ToInt32(o[9].ToString(), 16));
            return string.Concat(o.Substring(0, 9), s, o.Substring(10));
        }

        /// <summary>
        /// 将百度服务器上记录的百度MD5转为本地文件的MD5值
        /// </summary>
        public static string BaiduMd5ToMd5(string baiduMd5)
        {
            StringBuilder iSb = new StringBuilder();
            for (int a = 0; a < baiduMd5.Length; a++)
            {
                char c = baiduMd5[a];
                if (a == 9)
                    c = ((int)(c - 'g')).ToString("x")[0];

                iSb.Append((Convert.ToInt32(c.ToString(), 16) ^ (15 & a)).ToString("x"));
            }
            string i = iSb.ToString();
            return string.Concat(i.Substring(8, 8), i.Substring(0, 8), i.Substring(24, 8), i.Substring(16, 8));
        }
    }
}
