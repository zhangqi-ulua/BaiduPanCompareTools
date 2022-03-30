namespace BaiduPanCompareTools
{
    partial class SnapshootViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshootViewerForm));
            this.ImgIcon = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.LblSnapshootFilePath = new System.Windows.Forms.Label();
            this.TvwDiff = new System.Windows.Forms.TreeView();
            this.TxtDirOrFileDetail = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtBaiduPanUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtBaiduPanAccessCode = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ImgIcon
            // 
            this.ImgIcon.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ImgIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgIcon.ImageStream")));
            this.ImgIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgIcon.Images.SetKeyName(0, "NoneDir.ico");
            this.ImgIcon.Images.SetKeyName(1, "NoneFile.ico");
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
            // LblSnapshootFilePath
            // 
            this.LblSnapshootFilePath.AutoSize = true;
            this.LblSnapshootFilePath.Location = new System.Drawing.Point(122, 26);
            this.LblSnapshootFilePath.Name = "LblSnapshootFilePath";
            this.LblSnapshootFilePath.Size = new System.Drawing.Size(36, 17);
            this.LblSnapshootFilePath.TabIndex = 4;
            this.LblSnapshootFilePath.Text = "label";
            // 
            // TvwDiff
            // 
            this.TvwDiff.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TvwDiff.ImageIndex = 0;
            this.TvwDiff.ImageList = this.ImgIcon;
            this.TvwDiff.Location = new System.Drawing.Point(24, 128);
            this.TvwDiff.Name = "TvwDiff";
            this.TvwDiff.SelectedImageIndex = 0;
            this.TvwDiff.Size = new System.Drawing.Size(1226, 659);
            this.TvwDiff.TabIndex = 8;
            this.TvwDiff.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvwDiff_AfterSelect);
            // 
            // TxtDirOrFileDetail
            // 
            this.TxtDirOrFileDetail.Location = new System.Drawing.Point(868, 16);
            this.TxtDirOrFileDetail.Multiline = true;
            this.TxtDirOrFileDetail.Name = "TxtDirOrFileDetail";
            this.TxtDirOrFileDetail.Size = new System.Drawing.Size(382, 93);
            this.TxtDirOrFileDetail.TabIndex = 9;
            this.TxtDirOrFileDetail.Text = "fsId：12345678901234\r\n文件大小：66.66MB\r\n修改时间：2022-02-01 16:00:00\r\n百度MD5：8e23f7635tb641" +
    "36eddb0719602bc477\r\n本地MD5：d41d8cd98f00b204e9800998ecf8427e";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "百度网盘链接：";
            // 
            // TxtBaiduPanUrl
            // 
            this.TxtBaiduPanUrl.Location = new System.Drawing.Point(122, 55);
            this.TxtBaiduPanUrl.Name = "TxtBaiduPanUrl";
            this.TxtBaiduPanUrl.Size = new System.Drawing.Size(457, 23);
            this.TxtBaiduPanUrl.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(604, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "提取码：";
            // 
            // TxtBaiduPanAccessCode
            // 
            this.TxtBaiduPanAccessCode.Location = new System.Drawing.Point(666, 55);
            this.TxtBaiduPanAccessCode.Name = "TxtBaiduPanAccessCode";
            this.TxtBaiduPanAccessCode.Size = new System.Drawing.Size(100, 23);
            this.TxtBaiduPanAccessCode.TabIndex = 13;
            // 
            // SnapshootViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 810);
            this.Controls.Add(this.TxtBaiduPanAccessCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtBaiduPanUrl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtDirOrFileDetail);
            this.Controls.Add(this.TvwDiff);
            this.Controls.Add(this.LblSnapshootFilePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SnapshootViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "快照浏览器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageList ImgIcon;
        private Label label1;
        private Label LblSnapshootFilePath;
        private TreeView TvwDiff;
        private TextBox TxtDirOrFileDetail;
        private Label label2;
        private TextBox TxtBaiduPanUrl;
        private Label label3;
        private TextBox TxtBaiduPanAccessCode;
    }
}