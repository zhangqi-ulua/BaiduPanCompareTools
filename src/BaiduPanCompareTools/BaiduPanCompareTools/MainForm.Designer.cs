namespace BaiduPanCompareTools
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GrpGenerateSnapshoot = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.TxtWaitServerMaxMillisecond = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.BtnGenerateSnapshoot = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.TxtRequestInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtTargetDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtBaiduPanAccessCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtBaiduPanUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GrpCompareSnapshoot = new System.Windows.Forms.GroupBox();
            this.BtnViewNewSnapshoot = new System.Windows.Forms.Button();
            this.BtnViewOldSnapshoot = new System.Windows.Forms.Button();
            this.BtnCompareSnapshoot = new System.Windows.Forms.Button();
            this.TxtCompareNewSnapshootDir = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TxtCompareOldSnapshootDir = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.BtnChooseNewSnapshootFile = new System.Windows.Forms.Button();
            this.BtnChooseOldSnapshootFile = new System.Windows.Forms.Button();
            this.TxtNewSnapshootFilePath = new System.Windows.Forms.TextBox();
            this.TxtOldSnapshootFilePath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.RtxLog = new System.Windows.Forms.RichTextBox();
            this.GrpCompareSnapshootAndLocalDir = new System.Windows.Forms.GroupBox();
            this.GrpFileMD5Config = new System.Windows.Forms.GroupBox();
            this.TxtIgnoreFileExtension = new System.Windows.Forms.TextBox();
            this.ChkIgnoreFileExtension = new System.Windows.Forms.CheckBox();
            this.Cbo = new System.Windows.Forms.ComboBox();
            this.TxtIgnoreBigFileSize = new System.Windows.Forms.TextBox();
            this.ChkIgnoreBigFile = new System.Windows.Forms.CheckBox();
            this.BtnCompareSnapshootAndLocalDir = new System.Windows.Forms.Button();
            this.BtnChooseLocalDir = new System.Windows.Forms.Button();
            this.TxtLocalDirPath = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.TxtCompareSnapshootDir = new System.Windows.Forms.TextBox();
            this.TxtSnapshootFilePath = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.BtnChooseSnapshootFile = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.GrpGenerateSnapshoot.SuspendLayout();
            this.GrpCompareSnapshoot.SuspendLayout();
            this.GrpCompareSnapshootAndLocalDir.SuspendLayout();
            this.GrpFileMD5Config.SuspendLayout();
            this.SuspendLayout();
            // 
            // GrpGenerateSnapshoot
            // 
            this.GrpGenerateSnapshoot.Controls.Add(this.label12);
            this.GrpGenerateSnapshoot.Controls.Add(this.TxtWaitServerMaxMillisecond);
            this.GrpGenerateSnapshoot.Controls.Add(this.label11);
            this.GrpGenerateSnapshoot.Controls.Add(this.BtnGenerateSnapshoot);
            this.GrpGenerateSnapshoot.Controls.Add(this.label6);
            this.GrpGenerateSnapshoot.Controls.Add(this.TxtRequestInterval);
            this.GrpGenerateSnapshoot.Controls.Add(this.label5);
            this.GrpGenerateSnapshoot.Controls.Add(this.TxtTargetDir);
            this.GrpGenerateSnapshoot.Controls.Add(this.label3);
            this.GrpGenerateSnapshoot.Controls.Add(this.TxtBaiduPanAccessCode);
            this.GrpGenerateSnapshoot.Controls.Add(this.label2);
            this.GrpGenerateSnapshoot.Controls.Add(this.TxtBaiduPanUrl);
            this.GrpGenerateSnapshoot.Controls.Add(this.label1);
            this.GrpGenerateSnapshoot.Location = new System.Drawing.Point(24, 24);
            this.GrpGenerateSnapshoot.Name = "GrpGenerateSnapshoot";
            this.GrpGenerateSnapshoot.Size = new System.Drawing.Size(902, 214);
            this.GrpGenerateSnapshoot.TabIndex = 0;
            this.GrpGenerateSnapshoot.TabStop = false;
            this.GrpGenerateSnapshoot.Text = "生成百度网盘某目录的快照，供日后对比变化";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(576, 178);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 17);
            this.label12.TabIndex = 15;
            this.label12.Text = "毫秒";
            // 
            // TxtWaitServerMaxMillisecond
            // 
            this.TxtWaitServerMaxMillisecond.Location = new System.Drawing.Point(470, 175);
            this.TxtWaitServerMaxMillisecond.Name = "TxtWaitServerMaxMillisecond";
            this.TxtWaitServerMaxMillisecond.Size = new System.Drawing.Size(100, 23);
            this.TxtWaitServerMaxMillisecond.TabIndex = 14;
            this.TxtWaitServerMaxMillisecond.Text = "5000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 178);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(428, 17);
            this.label11.TabIndex = 13;
            this.label11.Text = "发出请求后等待百度网盘服务器响应的最长时间（网速较差时，可适当调大）：";
            // 
            // BtnGenerateSnapshoot
            // 
            this.BtnGenerateSnapshoot.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BtnGenerateSnapshoot.Location = new System.Drawing.Point(727, 144);
            this.BtnGenerateSnapshoot.Name = "BtnGenerateSnapshoot";
            this.BtnGenerateSnapshoot.Size = new System.Drawing.Size(144, 51);
            this.BtnGenerateSnapshoot.TabIndex = 12;
            this.BtnGenerateSnapshoot.Text = "生成快照";
            this.BtnGenerateSnapshoot.UseVisualStyleBackColor = true;
            this.BtnGenerateSnapshoot.Click += new System.EventHandler(this.BtnGenerateSnapshoot_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(636, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "毫秒";
            // 
            // TxtRequestInterval
            // 
            this.TxtRequestInterval.Location = new System.Drawing.Point(530, 144);
            this.TxtRequestInterval.Name = "TxtRequestInterval";
            this.TxtRequestInterval.Size = new System.Drawing.Size(100, 23);
            this.TxtRequestInterval.TabIndex = 10;
            this.TxtRequestInterval.Text = "1000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(500, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "打开子文件夹间隔的时间（建议不要设置过短，以免请求过快对百度网盘服务器带来压力）：";
            // 
            // TxtTargetDir
            // 
            this.TxtTargetDir.Location = new System.Drawing.Point(242, 100);
            this.TxtTargetDir.Name = "TxtTargetDir";
            this.TxtTargetDir.Size = new System.Drawing.Size(629, 23);
            this.TxtTargetDir.TabIndex = 5;
            this.TxtTargetDir.Text = "/";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(193, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "要建立快照的目录（根目录为/）：";
            // 
            // TxtBaiduPanAccessCode
            // 
            this.TxtBaiduPanAccessCode.Location = new System.Drawing.Point(568, 66);
            this.TxtBaiduPanAccessCode.MaxLength = 4;
            this.TxtBaiduPanAccessCode.Name = "TxtBaiduPanAccessCode";
            this.TxtBaiduPanAccessCode.Size = new System.Drawing.Size(100, 23);
            this.TxtBaiduPanAccessCode.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(536, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "对应的提取码（若上面的连接地址是带有?pwd=XXXX这样自动填充提取码的，则这里不必输入）：";
            // 
            // TxtBaiduPanUrl
            // 
            this.TxtBaiduPanUrl.Location = new System.Drawing.Point(414, 35);
            this.TxtBaiduPanUrl.Name = "TxtBaiduPanUrl";
            this.TxtBaiduPanUrl.Size = new System.Drawing.Size(457, 23);
            this.TxtBaiduPanUrl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "分享的百度网盘链接地址（形如https://pan.baidu.com/s/XXXXX）：";
            // 
            // GrpCompareSnapshoot
            // 
            this.GrpCompareSnapshoot.Controls.Add(this.BtnViewNewSnapshoot);
            this.GrpCompareSnapshoot.Controls.Add(this.BtnViewOldSnapshoot);
            this.GrpCompareSnapshoot.Controls.Add(this.BtnCompareSnapshoot);
            this.GrpCompareSnapshoot.Controls.Add(this.TxtCompareNewSnapshootDir);
            this.GrpCompareSnapshoot.Controls.Add(this.label10);
            this.GrpCompareSnapshoot.Controls.Add(this.TxtCompareOldSnapshootDir);
            this.GrpCompareSnapshoot.Controls.Add(this.label9);
            this.GrpCompareSnapshoot.Controls.Add(this.BtnChooseNewSnapshootFile);
            this.GrpCompareSnapshoot.Controls.Add(this.BtnChooseOldSnapshootFile);
            this.GrpCompareSnapshoot.Controls.Add(this.TxtNewSnapshootFilePath);
            this.GrpCompareSnapshoot.Controls.Add(this.TxtOldSnapshootFilePath);
            this.GrpCompareSnapshoot.Controls.Add(this.label8);
            this.GrpCompareSnapshoot.Controls.Add(this.label7);
            this.GrpCompareSnapshoot.Location = new System.Drawing.Point(24, 257);
            this.GrpCompareSnapshoot.Name = "GrpCompareSnapshoot";
            this.GrpCompareSnapshoot.Size = new System.Drawing.Size(902, 249);
            this.GrpCompareSnapshoot.TabIndex = 1;
            this.GrpCompareSnapshoot.TabStop = false;
            this.GrpCompareSnapshoot.Text = "对比百度网盘快照文件，找出发生的变化";
            // 
            // BtnViewNewSnapshoot
            // 
            this.BtnViewNewSnapshoot.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnViewNewSnapshoot.Location = new System.Drawing.Point(720, 193);
            this.BtnViewNewSnapshoot.Name = "BtnViewNewSnapshoot";
            this.BtnViewNewSnapshoot.Size = new System.Drawing.Size(151, 35);
            this.BtnViewNewSnapshoot.TabIndex = 20;
            this.BtnViewNewSnapshoot.Text = "浏览较新的快照文件";
            this.BtnViewNewSnapshoot.UseVisualStyleBackColor = true;
            this.BtnViewNewSnapshoot.Click += new System.EventHandler(this.BtnViewNewSnapshoot_Click);
            // 
            // BtnViewOldSnapshoot
            // 
            this.BtnViewOldSnapshoot.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnViewOldSnapshoot.Location = new System.Drawing.Point(554, 193);
            this.BtnViewOldSnapshoot.Name = "BtnViewOldSnapshoot";
            this.BtnViewOldSnapshoot.Size = new System.Drawing.Size(151, 35);
            this.BtnViewOldSnapshoot.TabIndex = 19;
            this.BtnViewOldSnapshoot.Text = "浏览较老的快照文件";
            this.BtnViewOldSnapshoot.UseVisualStyleBackColor = true;
            this.BtnViewOldSnapshoot.Click += new System.EventHandler(this.BtnViewOldSnapshoot_Click);
            // 
            // BtnCompareSnapshoot
            // 
            this.BtnCompareSnapshoot.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BtnCompareSnapshoot.Location = new System.Drawing.Point(380, 177);
            this.BtnCompareSnapshoot.Name = "BtnCompareSnapshoot";
            this.BtnCompareSnapshoot.Size = new System.Drawing.Size(144, 51);
            this.BtnCompareSnapshoot.TabIndex = 13;
            this.BtnCompareSnapshoot.Text = "对比快照";
            this.BtnCompareSnapshoot.UseVisualStyleBackColor = true;
            this.BtnCompareSnapshoot.Click += new System.EventHandler(this.BtnCompareSnapshoot_Click);
            // 
            // TxtCompareNewSnapshootDir
            // 
            this.TxtCompareNewSnapshootDir.Location = new System.Drawing.Point(296, 134);
            this.TxtCompareNewSnapshootDir.Name = "TxtCompareNewSnapshootDir";
            this.TxtCompareNewSnapshootDir.Size = new System.Drawing.Size(575, 23);
            this.TxtCompareNewSnapshootDir.TabIndex = 18;
            this.TxtCompareNewSnapshootDir.Text = "/";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(54, 137);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(236, 17);
            this.label10.TabIndex = 17;
            this.label10.Text = "对比哪个目录（从快照的根目录开始写）：";
            // 
            // TxtCompareOldSnapshootDir
            // 
            this.TxtCompareOldSnapshootDir.Location = new System.Drawing.Point(296, 65);
            this.TxtCompareOldSnapshootDir.Name = "TxtCompareOldSnapshootDir";
            this.TxtCompareOldSnapshootDir.Size = new System.Drawing.Size(575, 23);
            this.TxtCompareOldSnapshootDir.TabIndex = 16;
            this.TxtCompareOldSnapshootDir.Text = "/";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(54, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(236, 17);
            this.label9.TabIndex = 15;
            this.label9.Text = "对比哪个目录（从快照的根目录开始写）：";
            // 
            // BtnChooseNewSnapshootFile
            // 
            this.BtnChooseNewSnapshootFile.Location = new System.Drawing.Point(796, 105);
            this.BtnChooseNewSnapshootFile.Name = "BtnChooseNewSnapshootFile";
            this.BtnChooseNewSnapshootFile.Size = new System.Drawing.Size(75, 23);
            this.BtnChooseNewSnapshootFile.TabIndex = 14;
            this.BtnChooseNewSnapshootFile.Text = "选择";
            this.BtnChooseNewSnapshootFile.UseVisualStyleBackColor = true;
            this.BtnChooseNewSnapshootFile.Click += new System.EventHandler(this.BtnChooseNewSnapshootFile_Click);
            // 
            // BtnChooseOldSnapshootFile
            // 
            this.BtnChooseOldSnapshootFile.Location = new System.Drawing.Point(796, 36);
            this.BtnChooseOldSnapshootFile.Name = "BtnChooseOldSnapshootFile";
            this.BtnChooseOldSnapshootFile.Size = new System.Drawing.Size(75, 23);
            this.BtnChooseOldSnapshootFile.TabIndex = 13;
            this.BtnChooseOldSnapshootFile.Text = "选择";
            this.BtnChooseOldSnapshootFile.UseVisualStyleBackColor = true;
            this.BtnChooseOldSnapshootFile.Click += new System.EventHandler(this.BtnChooseOldSnapshootFile_Click);
            // 
            // TxtNewSnapshootFilePath
            // 
            this.TxtNewSnapshootFilePath.AllowDrop = true;
            this.TxtNewSnapshootFilePath.Location = new System.Drawing.Point(158, 105);
            this.TxtNewSnapshootFilePath.Name = "TxtNewSnapshootFilePath";
            this.TxtNewSnapshootFilePath.Size = new System.Drawing.Size(609, 23);
            this.TxtNewSnapshootFilePath.TabIndex = 3;
            // 
            // TxtOldSnapshootFilePath
            // 
            this.TxtOldSnapshootFilePath.AllowDrop = true;
            this.TxtOldSnapshootFilePath.Location = new System.Drawing.Point(158, 36);
            this.TxtOldSnapshootFilePath.Name = "TxtOldSnapshootFilePath";
            this.TxtOldSnapshootFilePath.Size = new System.Drawing.Size(609, 23);
            this.TxtOldSnapshootFilePath.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "较新的快照文件路径：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "较老的快照文件路径：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(950, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "运行日志：";
            // 
            // RtxLog
            // 
            this.RtxLog.Location = new System.Drawing.Point(950, 59);
            this.RtxLog.Name = "RtxLog";
            this.RtxLog.Size = new System.Drawing.Size(506, 777);
            this.RtxLog.TabIndex = 3;
            this.RtxLog.Text = "";
            // 
            // GrpCompareSnapshootAndLocalDir
            // 
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.GrpFileMD5Config);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.BtnCompareSnapshootAndLocalDir);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.BtnChooseLocalDir);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.TxtLocalDirPath);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.label15);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.TxtCompareSnapshootDir);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.TxtSnapshootFilePath);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.label13);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.label14);
            this.GrpCompareSnapshootAndLocalDir.Controls.Add(this.BtnChooseSnapshootFile);
            this.GrpCompareSnapshootAndLocalDir.Location = new System.Drawing.Point(24, 527);
            this.GrpCompareSnapshootAndLocalDir.Name = "GrpCompareSnapshootAndLocalDir";
            this.GrpCompareSnapshootAndLocalDir.Size = new System.Drawing.Size(902, 309);
            this.GrpCompareSnapshootAndLocalDir.TabIndex = 4;
            this.GrpCompareSnapshootAndLocalDir.TabStop = false;
            this.GrpCompareSnapshootAndLocalDir.Text = "将本地目录与百度网盘快照文件对比，找出差异";
            // 
            // GrpFileMD5Config
            // 
            this.GrpFileMD5Config.Controls.Add(this.TxtIgnoreFileExtension);
            this.GrpFileMD5Config.Controls.Add(this.ChkIgnoreFileExtension);
            this.GrpFileMD5Config.Controls.Add(this.Cbo);
            this.GrpFileMD5Config.Controls.Add(this.TxtIgnoreBigFileSize);
            this.GrpFileMD5Config.Controls.Add(this.ChkIgnoreBigFile);
            this.GrpFileMD5Config.Location = new System.Drawing.Point(24, 155);
            this.GrpFileMD5Config.Name = "GrpFileMD5Config";
            this.GrpFileMD5Config.Size = new System.Drawing.Size(460, 132);
            this.GrpFileMD5Config.TabIndex = 28;
            this.GrpFileMD5Config.TabStop = false;
            this.GrpFileMD5Config.Text = "计算本地文件MD5选项（不建议对所有文件计算MD5，会占用大量时间）";
            // 
            // TxtIgnoreFileExtension
            // 
            this.TxtIgnoreFileExtension.Location = new System.Drawing.Point(61, 88);
            this.TxtIgnoreFileExtension.Name = "TxtIgnoreFileExtension";
            this.TxtIgnoreFileExtension.Size = new System.Drawing.Size(375, 23);
            this.TxtIgnoreFileExtension.TabIndex = 4;
            // 
            // ChkIgnoreFileExtension
            // 
            this.ChkIgnoreFileExtension.AutoSize = true;
            this.ChkIgnoreFileExtension.Location = new System.Drawing.Point(30, 61);
            this.ChkIgnoreFileExtension.Name = "ChkIgnoreFileExtension";
            this.ChkIgnoreFileExtension.Size = new System.Drawing.Size(354, 21);
            this.ChkIgnoreFileExtension.TabIndex = 3;
            this.ChkIgnoreFileExtension.Text = "忽略以下扩展名的文件（用|分隔，扩展名不必带前面的点号）";
            this.ChkIgnoreFileExtension.UseVisualStyleBackColor = true;
            // 
            // Cbo
            // 
            this.Cbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cbo.FormattingEnabled = true;
            this.Cbo.Items.AddRange(new object[] {
            "B",
            "KB",
            "MB",
            "GB"});
            this.Cbo.Location = new System.Drawing.Point(375, 28);
            this.Cbo.Name = "Cbo";
            this.Cbo.Size = new System.Drawing.Size(61, 25);
            this.Cbo.TabIndex = 2;
            // 
            // TxtIgnoreBigFileSize
            // 
            this.TxtIgnoreBigFileSize.Location = new System.Drawing.Point(195, 28);
            this.TxtIgnoreBigFileSize.Name = "TxtIgnoreBigFileSize";
            this.TxtIgnoreBigFileSize.Size = new System.Drawing.Size(165, 23);
            this.TxtIgnoreBigFileSize.TabIndex = 1;
            // 
            // ChkIgnoreBigFile
            // 
            this.ChkIgnoreBigFile.AutoSize = true;
            this.ChkIgnoreBigFile.Location = new System.Drawing.Point(30, 30);
            this.ChkIgnoreBigFile.Name = "ChkIgnoreBigFile";
            this.ChkIgnoreBigFile.Size = new System.Drawing.Size(159, 21);
            this.ChkIgnoreBigFile.TabIndex = 0;
            this.ChkIgnoreBigFile.Text = "忽略达到以下大小的文件";
            this.ChkIgnoreBigFile.UseVisualStyleBackColor = true;
            // 
            // BtnCompareSnapshootAndLocalDir
            // 
            this.BtnCompareSnapshootAndLocalDir.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BtnCompareSnapshootAndLocalDir.Location = new System.Drawing.Point(727, 198);
            this.BtnCompareSnapshootAndLocalDir.Name = "BtnCompareSnapshootAndLocalDir";
            this.BtnCompareSnapshootAndLocalDir.Size = new System.Drawing.Size(144, 51);
            this.BtnCompareSnapshootAndLocalDir.TabIndex = 27;
            this.BtnCompareSnapshootAndLocalDir.Text = "对比快照\r\n与本地目录";
            this.BtnCompareSnapshootAndLocalDir.UseVisualStyleBackColor = true;
            this.BtnCompareSnapshootAndLocalDir.Click += new System.EventHandler(this.BtnCompareSnapshootAndLocalDir_Click);
            // 
            // BtnChooseLocalDir
            // 
            this.BtnChooseLocalDir.Location = new System.Drawing.Point(796, 111);
            this.BtnChooseLocalDir.Name = "BtnChooseLocalDir";
            this.BtnChooseLocalDir.Size = new System.Drawing.Size(75, 23);
            this.BtnChooseLocalDir.TabIndex = 26;
            this.BtnChooseLocalDir.Text = "选择";
            this.BtnChooseLocalDir.UseVisualStyleBackColor = true;
            this.BtnChooseLocalDir.Click += new System.EventHandler(this.BtnChooseLocalDir_Click);
            // 
            // TxtLocalDirPath
            // 
            this.TxtLocalDirPath.AllowDrop = true;
            this.TxtLocalDirPath.Location = new System.Drawing.Point(122, 111);
            this.TxtLocalDirPath.Name = "TxtLocalDirPath";
            this.TxtLocalDirPath.Size = new System.Drawing.Size(645, 23);
            this.TxtLocalDirPath.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 114);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(92, 17);
            this.label15.TabIndex = 24;
            this.label15.Text = "本地目录路径：";
            // 
            // TxtCompareSnapshootDir
            // 
            this.TxtCompareSnapshootDir.Location = new System.Drawing.Point(296, 63);
            this.TxtCompareSnapshootDir.Name = "TxtCompareSnapshootDir";
            this.TxtCompareSnapshootDir.Size = new System.Drawing.Size(575, 23);
            this.TxtCompareSnapshootDir.TabIndex = 23;
            this.TxtCompareSnapshootDir.Text = "/";
            // 
            // TxtSnapshootFilePath
            // 
            this.TxtSnapshootFilePath.AllowDrop = true;
            this.TxtSnapshootFilePath.Location = new System.Drawing.Point(122, 34);
            this.TxtSnapshootFilePath.Name = "TxtSnapshootFilePath";
            this.TxtSnapshootFilePath.Size = new System.Drawing.Size(645, 23);
            this.TxtSnapshootFilePath.TabIndex = 20;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(54, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(236, 17);
            this.label13.TabIndex = 22;
            this.label13.Text = "对比哪个目录（从快照的根目录开始写）：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(24, 37);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(92, 17);
            this.label14.TabIndex = 19;
            this.label14.Text = "快照文件路径：";
            // 
            // BtnChooseSnapshootFile
            // 
            this.BtnChooseSnapshootFile.Location = new System.Drawing.Point(796, 34);
            this.BtnChooseSnapshootFile.Name = "BtnChooseSnapshootFile";
            this.BtnChooseSnapshootFile.Size = new System.Drawing.Size(75, 23);
            this.BtnChooseSnapshootFile.TabIndex = 21;
            this.BtnChooseSnapshootFile.Text = "选择";
            this.BtnChooseSnapshootFile.UseVisualStyleBackColor = true;
            this.BtnChooseSnapshootFile.Click += new System.EventHandler(this.BtnChooseSnapshootFile_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(24, 852);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(909, 34);
            this.label16.TabIndex = 5;
            this.label16.Text = "注意：1、百度网盘服务器记录的MD5不是文件本身的MD5，而是将本身的MD5经过算法运算得到32位字符串。为了加以区分，本工具分别称为“百度MD5”和“本地MD5" +
    "”\r\n         2、如果分享的链接中仅包含单个文件，百度网盘服务器返回的文件信息中无MD5（个人认为是百度网盘的BUG），故这种情况下无法对比文件";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1478, 901);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.GrpCompareSnapshootAndLocalDir);
            this.Controls.Add(this.RtxLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.GrpCompareSnapshoot);
            this.Controls.Add(this.GrpGenerateSnapshoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "百度网盘对比工具 v1.1.0 by 张齐 （https://github.com/zhangqi-ulua）";
            this.GrpGenerateSnapshoot.ResumeLayout(false);
            this.GrpGenerateSnapshoot.PerformLayout();
            this.GrpCompareSnapshoot.ResumeLayout(false);
            this.GrpCompareSnapshoot.PerformLayout();
            this.GrpCompareSnapshootAndLocalDir.ResumeLayout(false);
            this.GrpCompareSnapshootAndLocalDir.PerformLayout();
            this.GrpFileMD5Config.ResumeLayout(false);
            this.GrpFileMD5Config.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox GrpGenerateSnapshoot;
        private Button BtnGenerateSnapshoot;
        private Label label6;
        private TextBox TxtRequestInterval;
        private Label label5;
        private TextBox TxtTargetDir;
        private Label label3;
        private TextBox TxtBaiduPanAccessCode;
        private Label label2;
        private TextBox TxtBaiduPanUrl;
        private Label label1;
        private GroupBox GrpCompareSnapshoot;
        private Button BtnCompareSnapshoot;
        private TextBox TxtCompareNewSnapshootDir;
        private Label label10;
        private TextBox TxtCompareOldSnapshootDir;
        private Label label9;
        private Button BtnChooseNewSnapshootFile;
        private Button BtnChooseOldSnapshootFile;
        private TextBox TxtNewSnapshootFilePath;
        private TextBox TxtOldSnapshootFilePath;
        private Label label8;
        private Label label7;
        private Label label4;
        private RichTextBox RtxLog;
        private Label label12;
        private TextBox TxtWaitServerMaxMillisecond;
        private Label label11;
        private GroupBox GrpCompareSnapshootAndLocalDir;
        private Button BtnCompareSnapshootAndLocalDir;
        private Button BtnChooseLocalDir;
        private TextBox TxtLocalDirPath;
        private Label label15;
        private TextBox TxtCompareSnapshootDir;
        private TextBox TxtSnapshootFilePath;
        private Label label13;
        private Label label14;
        private Button BtnChooseSnapshootFile;
        private GroupBox GrpFileMD5Config;
        private ComboBox Cbo;
        private TextBox TxtIgnoreBigFileSize;
        private CheckBox ChkIgnoreBigFile;
        private TextBox TxtIgnoreFileExtension;
        private CheckBox ChkIgnoreFileExtension;
        private Label label16;
        private Button BtnViewNewSnapshoot;
        private Button BtnViewOldSnapshoot;
    }
}