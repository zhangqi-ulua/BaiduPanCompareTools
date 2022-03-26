namespace BaiduPanCompareTools
{
    partial class SnapshootAndLocalDirDiffResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshootAndLocalDirDiffResultForm));
            this.ImgIcon = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LblSnapshootFilePath = new System.Windows.Forms.Label();
            this.LblTargetDir = new System.Windows.Forms.Label();
            this.LblLocalDirPath = new System.Windows.Forms.Label();
            this.TvwDiff = new System.Windows.Forms.TreeView();
            this.TxtFileDiffDetail = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ImgIcon
            // 
            this.ImgIcon.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ImgIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgIcon.ImageStream")));
            this.ImgIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgIcon.Images.SetKeyName(0, "NoneDir.ico");
            this.ImgIcon.Images.SetKeyName(1, "AddDir.ico");
            this.ImgIcon.Images.SetKeyName(2, "DeleteDir.ico");
            this.ImgIcon.Images.SetKeyName(3, "AddFile.ico");
            this.ImgIcon.Images.SetKeyName(4, "DeleteFile.ico");
            this.ImgIcon.Images.SetKeyName(5, "ModifyFile.ico");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "快照文件路径：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "对比目录：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "本地目录路径：";
            // 
            // LblSnapshootFilePath
            // 
            this.LblSnapshootFilePath.AutoSize = true;
            this.LblSnapshootFilePath.Location = new System.Drawing.Point(122, 26);
            this.LblSnapshootFilePath.Name = "LblSnapshootFilePath";
            this.LblSnapshootFilePath.Size = new System.Drawing.Size(36, 17);
            this.LblSnapshootFilePath.TabIndex = 4;
            this.LblSnapshootFilePath.Text = "label";
            // 
            // LblTargetDir
            // 
            this.LblTargetDir.AutoSize = true;
            this.LblTargetDir.Location = new System.Drawing.Point(122, 52);
            this.LblTargetDir.Name = "LblTargetDir";
            this.LblTargetDir.Size = new System.Drawing.Size(36, 17);
            this.LblTargetDir.TabIndex = 5;
            this.LblTargetDir.Text = "label";
            // 
            // LblLocalDirPath
            // 
            this.LblLocalDirPath.AutoSize = true;
            this.LblLocalDirPath.Location = new System.Drawing.Point(122, 104);
            this.LblLocalDirPath.Name = "LblLocalDirPath";
            this.LblLocalDirPath.Size = new System.Drawing.Size(36, 17);
            this.LblLocalDirPath.TabIndex = 6;
            this.LblLocalDirPath.Text = "label";
            // 
            // TvwDiff
            // 
            this.TvwDiff.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TvwDiff.ImageIndex = 0;
            this.TvwDiff.ImageList = this.ImgIcon;
            this.TvwDiff.Location = new System.Drawing.Point(24, 181);
            this.TvwDiff.Name = "TvwDiff";
            this.TvwDiff.SelectedImageIndex = 0;
            this.TvwDiff.Size = new System.Drawing.Size(1226, 659);
            this.TvwDiff.TabIndex = 8;
            this.TvwDiff.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvwDiff_AfterSelect);
            // 
            // TxtFileDiffDetail
            // 
            this.TxtFileDiffDetail.Location = new System.Drawing.Point(868, 10);
            this.TxtFileDiffDetail.Multiline = true;
            this.TxtFileDiffDetail.Name = "TxtFileDiffDetail";
            this.TxtFileDiffDetail.Size = new System.Drawing.Size(382, 161);
            this.TxtFileDiffDetail.TabIndex = 9;
            this.TxtFileDiffDetail.Text = resources.GetString("TxtFileDiffDetail.Text");
            // 
            // SnapshootAndLocalDirDiffResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 865);
            this.Controls.Add(this.TxtFileDiffDetail);
            this.Controls.Add(this.TvwDiff);
            this.Controls.Add(this.LblLocalDirPath);
            this.Controls.Add(this.LblTargetDir);
            this.Controls.Add(this.LblSnapshootFilePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SnapshootAndLocalDirDiffResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "本地目录与快照对比结果";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageList ImgIcon;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label LblSnapshootFilePath;
        private Label LblTargetDir;
        private Label LblLocalDirPath;
        private TreeView TvwDiff;
        private TextBox TxtFileDiffDetail;
    }
}