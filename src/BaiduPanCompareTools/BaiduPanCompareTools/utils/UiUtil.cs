namespace BaiduPanCompareTools.utils
{
    internal class UiUtil
    {
        /// <summary>
        /// 想拖拽接收文件或文件夹路径的文本框，需要注册DragEnter事件回调
        /// </summary>
        public static void TextBoxDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        /// <summary>
        /// 要求拖拽一个文件到文本框的DragDrop事件回调
        /// </summary>
        public static void TextBoxOneFileDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                Array dragDropFileArray = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (dragDropFileArray.Length != 1)
                {
                    MessageBox.Show("只允许拖入一个指定的文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string path = dragDropFileArray.GetValue(0).ToString();
                if (Directory.Exists(path) == true)
                {
                    MessageBox.Show("请拖入一个指定的文件而不是文件夹", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (File.Exists(path))
                    {
                        TextBox textBox = sender as TextBox;
                        textBox.Text = path;
                    }
                    else
                    {
                        MessageBox.Show("请拖入一个指定的文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Path.GetExtension(path) != $".{AppConsts.SNAPSHOOT_FILE_EXTENSION}")
                    {
                        MessageBox.Show($"请拖入一个扩展名为{AppConsts.SNAPSHOOT_FILE_EXTENSION}的文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("请拖入一个指定的文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// 要求拖拽一个文件夹到文本框的DragDrop事件回调
        /// </summary>
        public static void TextBoxOneDirDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                Array dragDropDirArray = e.Data.GetData(DataFormats.FileDrop) as Array;
                if (dragDropDirArray.Length != 1)
                {
                    MessageBox.Show("只允许拖入一个指定的文件夹", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string path = dragDropDirArray.GetValue(0).ToString();
                if (File.Exists(path) == true)
                {
                    MessageBox.Show("请拖入一个指定的文件夹而不是文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        TextBox textBox = sender as TextBox;
                        textBox.Text = path;
                    }
                    else
                    {
                        MessageBox.Show("请拖入一个指定的文件夹", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("请拖入一个指定的文件夹", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
