namespace BaiduPanCompareTools
{
    /// <summary>
    /// 百度网盘一个文件或文件夹共有的信息
    /// </summary>
    internal class DirOrFileInfoVO
    {
        // 文件（含扩展名）或文件夹名
        public string name { get; set; }
        // 百度网盘为每个文件或文件夹编号的唯一id
        public long fsId { get; set; }
        // 是否为文件夹
        public bool isDir { get; set; }
        //// 其本身修改时间戳（秒），担心可能文件本身的时间戳被故意修改为超出int范围的
        //public long localModifyTimestamp { get; set; }
        // 其在百度网盘修改时间戳（秒），对于本地文件则是本地修改时间
        public int serverModifyTimestamp { get; set; }
    }
}
