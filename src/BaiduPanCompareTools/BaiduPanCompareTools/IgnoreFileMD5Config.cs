namespace BaiduPanCompareTools
{
    internal class IgnoreFileMD5Config
    {
        // 忽略文件大于多少字节的文件，0表示均不忽略，-1表示全都忽略
        public long IgnoreFileSizeByte { get; set; }
        // 忽略哪些扩展名（带前面的点号）的文件
        public string[] IgnoreFileExtensions { get; set; }
    }
}
