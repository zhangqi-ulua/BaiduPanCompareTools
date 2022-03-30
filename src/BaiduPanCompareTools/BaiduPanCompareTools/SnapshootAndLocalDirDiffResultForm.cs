using BaiduPanCompareTools.utils;
using BaiduPanCompareTools.vo.diff;
using System.Diagnostics;
using System.Text;

namespace BaiduPanCompareTools
{
    internal partial class SnapshootAndLocalDirDiffResultForm : Form
    {
        private const string DEFAULT_FILE_DIFF_DETAIL_TEXTBOX_TEXT = "点击文件，在这里显示文件详情";
        private const string FILE_SIZE_BIGGER_TEXT = "[较大]";
        private const string FILE_MODIFY_TIME_LATER_TEXT = "[较新]";
        private const string IGNORE_CALCULATE_FILE_MD5_TEXT = "[忽略对此文件计算MD5]";

        private ToolStripMenuItem _TsmiDir;
        private ToolStripMenuItem _TsmiFile;
        // 当前鼠标右键点击的节点
        private TreeNode _CurrentRightClickTreeNode;

        private string _LocalDirPath;

        public SnapshootAndLocalDirDiffResultForm(string snapShootFilePath, string targetDir,
            string localDirPath, DirDiffVO diff)
        {
            _LocalDirPath = localDirPath;

            InitializeComponent();

            LblSnapshootFilePath.Text = snapShootFilePath;
            LblTargetDir.Text = targetDir;
            LblLocalDirPath.Text = localDirPath;

            TxtFileDiffDetail.Text = DEFAULT_FILE_DIFF_DETAIL_TEXTBOX_TEXT;

            _TsmiDir = new ToolStripMenuItem("打开本地文件夹");
            _TsmiDir.Click += TsmiDir_Click;
            _TsmiFile = new ToolStripMenuItem("打开本地文件");
            _TsmiFile.Click += TsmiFile_Click;

            TreeNode diffRootTreeNode = GetDiffTreeNode(diff);
            diffRootTreeNode.Text = "[对比的根目录]";
            TvwDiff.ShowNodeToolTips = true;
            TvwDiff.Nodes.Add(diffRootTreeNode);
            TvwDiff.ExpandAll();
        }

        private void TsmiDir_Click(object sender, EventArgs e)
        {
            string dirOrFilePath = string.Concat(_LocalDirPath, _CurrentRightClickTreeNode.Name).Replace('/', '\\').Replace("\\\\", "\\");
            try
            {
                if (File.Exists(dirOrFilePath) == true)
                    Process.Start("explorer.exe", Path.GetDirectoryName(dirOrFilePath));
                else
                    Process.Start("explorer.exe", dirOrFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"打开本地文件夹失败，路径为：{dirOrFilePath}\n异常信息为：{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiFile_Click(object sender, EventArgs e)
        {
            string filePath = string.Concat(_LocalDirPath, _CurrentRightClickTreeNode.Name).Replace('/', '\\').Replace("\\\\", "\\");
            try
            {
                Process.Start("explorer.exe", filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"打开本地文件失败，路径为：{filePath}\n异常信息为：{ex.ToString()}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<TreeNode> GetAllChildsTreeNode(string parentDirPath, List<DirOrFileInfoVO> childs, bool isAdd)
        {
            List<TreeNode> list = new List<TreeNode>();

            foreach (DirOrFileInfoVO child in childs)
            {
                if (child.isDir == true)
                {
                    TreeNode childDirNode = new TreeNode();
                    childDirNode.Text = child.name;
                    childDirNode.Name = string.Concat(parentDirPath, "/", child.name);
                    childDirNode.ToolTipText = childDirNode.Name;
                    childDirNode.ImageIndex = (isAdd == true ? (int)IconEnum.AddDir : (int)IconEnum.DeleteDir);
                    childDirNode.SelectedImageIndex = (isAdd == true ? (int)IconEnum.AddDir : (int)IconEnum.DeleteDir);
                    DirInfoVO childDir = child as DirInfoVO;
                    childDirNode.Nodes.AddRange(GetAllChildsTreeNode(string.Concat(parentDirPath, "/", child.name), childDir.childs, isAdd).ToArray());
                    list.Add(childDirNode);
                }
                else
                {
                    FileInfoVO childFile = child as FileInfoVO;
                    TreeNode childFileNode = new TreeNode();
                    childFileNode.Text = childFile.name;
                    childFileNode.Name = string.Concat(parentDirPath, "/", child.name);
                    childFileNode.ToolTipText = childFileNode.Name;
                    childFileNode.ImageIndex = (isAdd == true ? (int)IconEnum.AddFile : (int)IconEnum.DeleteFile);
                    childFileNode.SelectedImageIndex = (isAdd == true ? (int)IconEnum.AddFile : (int)IconEnum.DeleteFile);
                    StringBuilder fileDetailBuilder = new StringBuilder();
                    fileDetailBuilder.AppendLine($"文件大小：{IoUtil.GetFileLengthString(childFile.fileSize)}");
                    fileDetailBuilder.AppendLine($"修改时间：{DateTimeUtil.TimestampSecondToLongDateString(childFile.serverModifyTimestamp)}");
                    if (isAdd == true)
                        fileDetailBuilder.Append($"百度MD5：{childFile.baiduMd5}");
                    else
                    {
                        if (childFile.baiduMd5 != null)
                        {
                            fileDetailBuilder.AppendLine($"百度MD5：{childFile.baiduMd5}");
                            fileDetailBuilder.Append($"本地MD5：{childFile.localMd5}");
                        }
                        else
                        {
                            fileDetailBuilder.AppendLine($"百度MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                            fileDetailBuilder.Append($"本地MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                        }
                    }

                    childFileNode.Tag = fileDetailBuilder.ToString();
                    list.Add(childFileNode);
                }
            }

            return list;
        }

        private TreeNode GetDiffTreeNode(DirDiffVO diff)
        {
            TreeNode dirRootNode = new TreeNode();
            dirRootNode.Text = diff.name;
            dirRootNode.Name = diff.path;
            dirRootNode.ToolTipText = dirRootNode.Name;

            if (diff.diffState == DiffStateEnum.Add)
            {
                dirRootNode.ImageIndex = (int)IconEnum.AddDir;
                dirRootNode.SelectedImageIndex = (int)IconEnum.AddDir;
                // 生成下属的所有子文件夹、子文件
                dirRootNode.Nodes.AddRange(GetAllChildsTreeNode(diff.path, diff.addOrDeleteDirInfo.childs, true).ToArray());
            }
            else if (diff.diffState == DiffStateEnum.Delete)
            {
                dirRootNode.ImageIndex = (int)IconEnum.DeleteDir;
                dirRootNode.SelectedImageIndex = (int)IconEnum.DeleteDir;
                // 生成下属的所有子文件夹、子文件
                dirRootNode.Nodes.AddRange(GetAllChildsTreeNode(diff.path, diff.addOrDeleteDirInfo.childs, false).ToArray());
            }
            else
            {
                dirRootNode.ImageIndex = (int)IconEnum.NoneDir;
                dirRootNode.SelectedImageIndex = (int)IconEnum.NoneDir;
                // 生成下属的所有子文件夹、子文件
                if (diff.childsDiff != null)
                {
                    foreach (DirOrFileDiffVO childDiff in diff.childsDiff)
                    {
                        if (childDiff.isDir == true)
                        {
                            DirDiffVO childDirDiff = childDiff as DirDiffVO;
                            dirRootNode.Nodes.Add(GetDiffTreeNode(childDirDiff));
                        }
                        else
                        {
                            FileDiffVO childFileDiff = childDiff as FileDiffVO;
                            TreeNode childFileNode = new TreeNode();
                            childFileNode.Text = childFileDiff.name;
                            childFileNode.Name = childFileDiff.path;
                            childFileNode.ToolTipText = childFileNode.Name;
                            if (childFileDiff.diffState == DiffStateEnum.Add)
                            {
                                childFileNode.ImageIndex = (int)IconEnum.AddFile;
                                childFileNode.SelectedImageIndex = (int)IconEnum.AddFile;
                                StringBuilder fileDetailBuilder = new StringBuilder();
                                fileDetailBuilder.AppendLine($"文件大小：{IoUtil.GetFileLengthString(childFileDiff.newFileInfo.fileSize)}");
                                fileDetailBuilder.AppendLine($"修改时间：{DateTimeUtil.TimestampSecondToLongDateString(childFileDiff.newFileInfo.serverModifyTimestamp)}");
                                fileDetailBuilder.Append($"百度MD5：{childFileDiff.newFileInfo.baiduMd5}");
                                childFileNode.Tag = fileDetailBuilder.ToString();
                            }
                            else if (childFileDiff.diffState == DiffStateEnum.Delete)
                            {
                                childFileNode.ImageIndex = (int)IconEnum.DeleteFile;
                                childFileNode.SelectedImageIndex = (int)IconEnum.DeleteFile;
                                StringBuilder fileDetailBuilder = new StringBuilder();
                                fileDetailBuilder.AppendLine($"文件大小：{IoUtil.GetFileLengthString(childFileDiff.oldFileInfo.fileSize)}");
                                fileDetailBuilder.AppendLine($"修改时间：{DateTimeUtil.TimestampSecondToLongDateString(childFileDiff.oldFileInfo.serverModifyTimestamp)}");
                                if (childFileDiff.oldFileInfo.baiduMd5 != null)
                                {
                                    fileDetailBuilder.AppendLine($"百度MD5：{childFileDiff.oldFileInfo.baiduMd5}");
                                    fileDetailBuilder.Append($"本地MD5：{childFileDiff.oldFileInfo.localMd5}");
                                }
                                else
                                {
                                    fileDetailBuilder.AppendLine($"百度MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                                    fileDetailBuilder.Append($"本地MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                                }
                                childFileNode.Tag = fileDetailBuilder.ToString();
                            }
                            else if (childFileDiff.diffState == DiffStateEnum.Modity)
                            {
                                childFileNode.ImageIndex = (int)IconEnum.ModifyFile;
                                childFileNode.SelectedImageIndex = (int)IconEnum.ModifyFile;
                                StringBuilder fileDetailBuilder = new StringBuilder();
                                fileDetailBuilder.AppendLine("本地文件中：");
                                bool isLocalFileSizeBigger = (childFileDiff.oldFileInfo.fileSize > childFileDiff.newFileInfo.fileSize);
                                fileDetailBuilder.AppendLine($"文件大小：{(isLocalFileSizeBigger ? FILE_SIZE_BIGGER_TEXT : "")}{IoUtil.GetFileLengthString(childFileDiff.oldFileInfo.fileSize)}");
                                bool isOldFileModifyLater = (childFileDiff.oldFileInfo.serverModifyTimestamp > childFileDiff.newFileInfo.serverModifyTimestamp);
                                fileDetailBuilder.AppendLine($"修改时间：{(isOldFileModifyLater ? FILE_MODIFY_TIME_LATER_TEXT : "")}{DateTimeUtil.TimestampSecondToLongDateString(childFileDiff.oldFileInfo.serverModifyTimestamp)}");
                                if (childFileDiff.oldFileInfo.baiduMd5 != null)
                                {
                                    fileDetailBuilder.AppendLine($"百度MD5：{childFileDiff.oldFileInfo.baiduMd5}");
                                    fileDetailBuilder.AppendLine($"本地MD5：{childFileDiff.oldFileInfo.localMd5}");
                                }
                                else
                                {
                                    fileDetailBuilder.AppendLine($"百度MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                                    fileDetailBuilder.AppendLine($"本地MD5：{IGNORE_CALCULATE_FILE_MD5_TEXT}");
                                }
                                fileDetailBuilder.AppendLine("快照中：");
                                bool isSnapshootFileSizeBigger = (childFileDiff.oldFileInfo.fileSize < childFileDiff.newFileInfo.fileSize);
                                fileDetailBuilder.AppendLine($"文件大小：{(isSnapshootFileSizeBigger ? FILE_SIZE_BIGGER_TEXT : "")}{IoUtil.GetFileLengthString(childFileDiff.newFileInfo.fileSize)}");
                                bool isNewFileModifyLater = (childFileDiff.oldFileInfo.serverModifyTimestamp < childFileDiff.newFileInfo.serverModifyTimestamp);
                                fileDetailBuilder.AppendLine($"修改时间：{(isNewFileModifyLater ? FILE_MODIFY_TIME_LATER_TEXT : "")}{DateTimeUtil.TimestampSecondToLongDateString(childFileDiff.newFileInfo.serverModifyTimestamp)}");
                                fileDetailBuilder.Append($"百度MD5：{childFileDiff.newFileInfo.baiduMd5}");
                                childFileNode.Tag = fileDetailBuilder.ToString();
                            }

                            dirRootNode.Nodes.Add(childFileNode);
                        }
                    }
                }
            }

            return dirRootNode;
        }

        private void TvwDiff_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                TxtFileDiffDetail.Text = DEFAULT_FILE_DIFF_DETAIL_TEXTBOX_TEXT;
            else
                TxtFileDiffDetail.Text = e.Node.Tag.ToString();
        }

        private void TvwDiff_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode treeNode = e.Node;
                ContextMenuStrip cms = new ContextMenuStrip();
                if (treeNode.ImageIndex == (int)IconEnum.NoneDir || treeNode.ImageIndex == (int)IconEnum.DeleteDir)
                    cms.Items.Add(_TsmiDir);
                else if (treeNode.ImageIndex == (int)IconEnum.DeleteFile || treeNode.ImageIndex == (int)IconEnum.ModifyFile)
                {
                    cms.Items.Add(_TsmiDir);
                    cms.Items.Add(_TsmiFile);
                }

                TvwDiff.SelectedNode = treeNode;
                _CurrentRightClickTreeNode = treeNode;
                cms.Show(TvwDiff, e.X, e.Y);
            }
        }
    }
}
