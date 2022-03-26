using System.Security.Cryptography;
using System.Text;

namespace BaiduPanCompareTools.utils
{
    internal class IoUtil
    {
        private static MD5 _MD5 = new MD5CryptoServiceProvider();

        /// <summary>
        /// 获取文件字节数对应的文件大小显示，最大单位为GB，依次为MB、KB、B
        /// </summary>
        public static string GetFileLengthString(long fileLength)
        {
            if (fileLength < 1024)
                return fileLength + " B";
            else if (fileLength < 1048576)
                return Math.Round((double)fileLength / 1024, 2) + " KB";
            else if (fileLength < 1073741824)
                return Math.Round((double)fileLength / 1048576, 2) + " MB";
            else
                return Math.Round((double)fileLength / 1073741824, 2) + " GB";
        }

        public static string CalculateFileMd5(string filePath, out string errorString)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] byteArray = _MD5.ComputeHash(fs);
                fs.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < byteArray.Length; i++)
                    sb.Append(byteArray[i].ToString("x2"));

                errorString = null;
                return sb.ToString();
            }
            catch (Exception ex)
            {
                errorString = $"计算此文件MD5失败：{filePath}\n请检查文件是否有权限访问且未被其他程序占用，异常信息为：\n{ex.ToString()}";
                return null;
            }
        }
    }
}
