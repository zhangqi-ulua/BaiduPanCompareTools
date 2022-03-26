namespace BaiduPanCompareTools.vo.diff
{
    internal class DirOrFileDiffVO
    {
        public bool isDir { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public DiffStateEnum diffState { get; set; }
    }
}
