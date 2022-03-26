namespace BaiduPanCompareTools.vo.diff
{
    internal class FileDiffVO : DirOrFileDiffVO
    {
        public FileDiffVO()
        {
            isDir = false;
        }

        // 当DiffState为Delete或Modity时，为其赋值
        public FileInfoVO oldFileInfo { get; set; }
        // 当DiffState为Add或Modity时，为其赋值
        public FileInfoVO newFileInfo { get; set; }
    }
}
