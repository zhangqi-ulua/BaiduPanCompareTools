namespace BaiduPanCompareTools
{
    partial class SnapshootDiffResultForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshootDiffResultForm));
            this.ImgIcon = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LblOldSnapshootFilePath = new System.Windows.Forms.Label();
            this.LblOldTargetDir = new System.Windows.Forms.Label();
            this.LblNewSnapshootFilePath = new System.Windows.Forms.Label();
            this.LblNewTargetDir = new System.Windows.Forms.Label();
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
            this.label1.Size = new System.Drawing.Size(128, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "较老的快照文件路径：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "对比目录：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "对比目录：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "较老的快照文件路径：";
            // 
            // LblOldSnapshootFilePath
            // 
            this.LblOldSnapshootFilePath.AutoSize = true;
            this.LblOldSnapshootFilePath.Location = new System.Drawing.Point(158, 26);
            this.LblOldSnapshootFilePath.Name = "LblOldSnapshootFilePath";
            this.LblOldSnapshootFilePath.Size = new System.Drawing.Size(36, 17);
            this.LblOldSnapshootFilePath.TabIndex = 4;
            this.LblOldSnapshootFilePath.Text = "label";
            // 
            // LblOldTargetDir
            // 
            this.LblOldTargetDir.AutoSize = true;
            this.LblOldTargetDir.Location = new System.Drawing.Point(158, 52);
            this.LblOldTargetDir.Name = "LblOldTargetDir";
            this.LblOldTargetDir.Size = new System.Drawing.Size(36, 17);
            this.LblOldTargetDir.TabIndex = 5;
            this.LblOldTargetDir.Text = "label";
            // 
            // LblNewSnapshootFilePath
            // 
            this.LblNewSnapshootFilePath.AutoSize = true;
            this.LblNewSnapshootFilePath.Location = new System.Drawing.Point(158, 104);
            this.LblNewSnapshootFilePath.Name = "LblNewSnapshootFilePath";
            this.LblNewSnapshootFilePath.Size = new System.Drawing.Size(36, 17);
            this.LblNewSnapshootFilePath.TabIndex = 6;
            this.LblNewSnapshootFilePath.Text = "label";
            // 
            // LblNewTargetDir
            // 
            this.LblNewTargetDir.AutoSize = true;
            this.LblNewTargetDir.Location = new System.Drawing.Point(158, 130);
            this.LblNewTargetDir.Name = "LblNewTargetDir";
            this.LblNewTargetDir.Size = new System.Drawing.Size(36, 17);
            this.LblNewTargetDir.TabIndex = 7;
            this.LblNewTargetDir.Text = "label";
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
            this.TxtFileDiffDetail.Text = "较老快照中：\r\n文件大小：[较大]77.77MB\r\n修改时间：2022-01-01 12:00:00\r\n百度MD5：8e23f7635tb64136eddb071" +
    "9602bc477\r\n较新快照中：\r\n文件大小：66.66MB\r\n修改时间：[较新]2022-02-01 16:00:00\r\n百度MD5：c4ca4238a0b" +
    "923820dcc509a6f75849b";
            // 
            // SnapshootDiffResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 865);
            this.Controls.Add(this.TxtFileDiffDetail);
            this.Controls.Add(this.TvwDiff);
            this.Controls.Add(this.LblNewTargetDir);
            this.Controls.Add(this.LblNewSnapshootFilePath);
            this.Controls.Add(this.LblOldTargetDir);
            this.Controls.Add(this.LblOldSnapshootFilePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SnapshootDiffResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新老快照对比结果";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageList ImgIcon;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label LblOldSnapshootFilePath;
        private Label LblOldTargetDir;
        private Label LblNewSnapshootFilePath;
        private Label LblNewTargetDir;
        private TreeView TvwDiff;
        private TextBox TxtFileDiffDetail;
    }
}