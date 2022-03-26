namespace BaiduPanCompareTools
{
    /// <summary>
    /// 百度网盘中一个文件夹的信息
    /// </summary>
    internal class DirInfoVO : DirOrFileInfoVO
    {
        // 该文件夹中子文件或子文件夹的信息
        public List<DirOrFileInfoVO> childs;

        public DirInfoVO()
        {
            isDir = true;
            childs = new List<DirOrFileInfoVO>();
        }
    }
}
