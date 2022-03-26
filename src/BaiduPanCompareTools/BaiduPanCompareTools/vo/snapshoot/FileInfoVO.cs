namespace BaiduPanCompareTools
{
    /// <summary>
    /// 百度网盘中一个文件的信息
    /// </summary>
    internal class FileInfoVO : DirOrFileInfoVO
    {
        // 百度服务器为文件记录的MD5值（英文字母全小写，是将本地文件MD5经过算法运算得到的另外的字符串）
        public string baiduMd5 { get; set; }
        // 本地文件的MD5值（英文字母全小写）
        public string localMd5 { get; set; }
        // 文件大小（字节）
        public long fileSize { get; set; }

        public FileInfoVO()
        {
            isDir = false;
        }
    }
}
