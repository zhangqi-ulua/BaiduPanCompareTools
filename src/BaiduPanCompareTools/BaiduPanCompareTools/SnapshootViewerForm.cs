using BaiduPanCompareTools.utils;
using System.Text;

namespace BaiduPanCompareTools
{
    internal enum ViewerIconEnum
    {
        NoneDir = 0,
        NoneFile,
    }

    internal partial class SnapshootViewerForm : Form
    {
        public SnapshootViewerForm(string snapshootFilePath, SnapshootInfoVO snapshoot)
        {
            InitializeComponent();

            LblSnapshootFilePath.Text = snapshootFilePath;
            TxtBaiduPanUrl.Text = snapshoot.baiduPanUrl;
            TxtBaiduPanAccessCode.Text = snapshoot.baiduPanAccessCode;

            DirInfoVO dirInfo = new DirInfoVO();
            dirInfo.childs = snapshoot.childs;
            dirInfo.name = "[根目录]";
            long totalChildFileSize;
            TreeNode rootTreeNode = GetTreeNode(dirInfo, "/", out totalChildFileSize);
            TvwDiff.ShowNodeToolTips = true;
            TvwDiff.Nodes.Add(rootTreeNode);
            TvwDiff.ExpandAll();
            TvwDiff.SelectedNode = rootTreeNode;
        }

        private TreeNode GetTreeNode(DirInfoVO dirInfo, string dirPath, out long totalChildFileSize)
        {
            TreeNode dirRootNode = new TreeNode();
            dirRootNode.Text = dirInfo.name;
            dirRootNode.Name = dirPath;
            dirRootNode.ToolTipText = dirRootNode.Name;
            dirRootNode.ImageIndex = (int)ViewerIconEnum.NoneDir;
            dirRootNode.SelectedImageIndex = (int)ViewerIconEnum.NoneDir;
            StringBuilder dirDetailBuilder = new StringBuilder();
            if (dirPath != "/")
            {
                dirDetailBuilder.AppendLine($"fsId：{dirInfo.fsId}");
                dirDetailBuilder.AppendLine($"修改时间：{DateTimeUtil.TimestampSecondToLongDateString(dirInfo.serverModifyTimestamp)}");
            }
            int directChildDirCount = 0;
            int directChildFileCount = 0;
            totalChildFileSize = 0;
            long tempOneChildFolderAllChildFileSize;

            // 生成下属的所有子文件夹、子文件
            foreach (DirOrFileInfoVO dirOrFileInfo in dirInfo.childs)
            {
                if (dirOrFileInfo.isDir == true)
                {
                    directChildDirCount++;
                    DirInfoVO childDir = dirOrFileInfo as DirInfoVO;
                    dirRootNode.Nodes.Add(GetTreeNode(childDir, CombineChildPath(dirPath, childDir.name), out tempOneChildFolderAllChildFileSize));
                    totalChildFileSize += tempOneChildFolderAllChildFileSize;
                }
                else
                {
                    directChildFileCount++;
                    FileInfoVO childFile = dirOrFileInfo as FileInfoVO;
                    TreeNode childFileNode = new TreeNode();
                    childFileNode.Text = childFile.name;
                    childFileNode.Name = CombineChildPath(dirPath, childFile.name);
                    childFileNode.ToolTipText = childFileNode.Name;
                    childFileNode.ImageIndex = (int)ViewerIconEnum.NoneFile;
                    childFileNode.SelectedImageIndex = (int)ViewerIconEnum.NoneFile;
                    StringBuilder fileDetailBuilder = new StringBuilder();
                    fileDetailBuilder.AppendLine($"fsId：{childFile.fsId}");
                    fileDetailBuilder.AppendLine($"文件大小：{IoUtil.GetFileLengthString(childFile.fileSize)}");
                    totalChildFileSize += childFile.fileSize;
                    fileDetailBuilder.AppendLine($"修改时间：{DateTimeUtil.TimestampSecondToLongDateString(childFile.serverModifyTimestamp)}");
                    fileDetailBuilder.AppendLine($"百度MD5：{childFile.baiduMd5}");
                    fileDetailBuilder.Append($"本地MD5：{AppConsts.BaiduMd5ToMd5(childFile.baiduMd5)}");
                    childFileNode.Tag = fileDetailBuilder.ToString();

                    dirRootNode.Nodes.Add(childFileNode);
                }
            }

            dirDetailBuilder.AppendLine($"直属子文件个数：{directChildDirCount}， 直属子文件个数：{directChildFileCount}");
            dirDetailBuilder.Append($"下属各级文件总大小：{IoUtil.GetFileLengthString(totalChildFileSize)} （{totalChildFileSize}B）");
            dirRootNode.Tag = dirDetailBuilder.ToString();

            return dirRootNode;
        }

        private string CombineChildPath(string parentPath, string childName)
        {
            if (parentPath == "/")
                return string.Concat(parentPath, childName);
            else
                return string.Concat(parentPath, "/", childName);
        }

        private void TvwDiff_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TxtDirOrFileDetail.Text = e.Node.Tag.ToString();
        }
    }
}
