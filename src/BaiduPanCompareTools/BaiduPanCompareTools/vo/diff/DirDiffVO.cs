namespace BaiduPanCompareTools.vo.diff
{
    internal class DirDiffVO : DirOrFileDiffVO
    {
        public DirDiffVO()
        {
            isDir = true;
        }

        // 当DiffState为Delete或Add时，记录较老或较新快照文件中该文件夹的信息（注意是本级目录的DirInfoVO）
        public DirInfoVO addOrDeleteDirInfo { get; set; }
        // 当DiffState为None时，记录子文件夹、子文件的比对信息
        public List<DirOrFileDiffVO> childsDiff { get; set; }

        // 当较老、较新快照中某文件夹都存在时，将它们下属的子文件夹、子文件分别遍历整理为可通过文件夹名或文件名进行快速索引的字典结构
        public Dictionary<string, FileInfoVO> filesInOldSnapshoot { get; set; }
        public Dictionary<string, FileInfoVO> filesInNewSnapshoot { get; set; }
        public Dictionary<string, DirInfoVO> dirsInOldSnapshoot { get; set; }
        public Dictionary<string, DirInfoVO> dirsInNewSnapshoot { get; set; }

        /// <summary>
        /// 判断子文件夹或子文件是否有差异
        /// </summary>
        public bool IsExistDiffInChilds()
        {
            if (childsDiff != null)
            {
                for (int i = childsDiff.Count - 1; i >= 0; i--)
                {
                    DirOrFileDiffVO childDiff = childsDiff[i];
                    if (childDiff.isDir == true)
                    {
                        DirDiffVO childDirDiff = childDiff as DirDiffVO;
                        if (childDirDiff.diffState == DiffStateEnum.Add || childDirDiff.diffState == DiffStateEnum.Delete)
                            return true;
                        else if (childDirDiff.IsExistDiffInChilds() == true)
                            return true;
                    }
                    else
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清除掉childsDiff中实际无差别的子文件夹（也会对子文件夹下属的进行清理）
        /// </summary>
        public void CleanNotDiffChildsDir()
        {
            if (childsDiff == null)
                return;

            for (int i = childsDiff.Count - 1; i >= 0; i--)
            {
                DirOrFileDiffVO childDiff = childsDiff[i];
                if (childDiff.isDir == true && childDiff.diffState == DiffStateEnum.None)
                {
                    DirDiffVO childDirDiff = childDiff as DirDiffVO;
                    if (childDirDiff.IsExistDiffInChilds() == false)
                        childsDiff.RemoveAt(i);
                    else
                        childDirDiff.CleanNotDiffChildsDir();
                }
            }
        }
    }
}
