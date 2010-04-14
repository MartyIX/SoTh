
// #pragma warning disable 1591,1592,1573,1571,1570,1572  -- disables XML comments warnings

namespace Sokoban
{
    /// <summary>
    /// Main form
    /// </summary>
    partial class GameDeskView
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
            System.Windows.Forms.Label lStaticHrac;
            System.Windows.Forms.Label lStaticKroky;
            System.Windows.Forms.Label lStaticCas;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameDeskView));
            this.bStart = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StL = new System.Windows.Forms.ToolStripStatusLabel();
            this.bRestart = new System.Windows.Forms.Button();
            this.lUser = new System.Windows.Forms.Label();
            this.pSettings = new System.Windows.Forms.Panel();
            this.cbRecord = new System.Windows.Forms.CheckBox();
            this.cbSounds = new System.Windows.Forms.CheckBox();
            this.cbCharts = new System.Windows.Forms.CheckBox();
            this.lSteps = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.pMain = new System.Windows.Forms.Panel();
            this.bLoad = new System.Windows.Forms.Button();
            this.lStaticRound = new System.Windows.Forms.Label();
            this.lRound = new System.Windows.Forms.Label();
            this.bConnect = new System.Windows.Forms.Button();
            this.bListen = new System.Windows.Forms.Button();
            this.cbLeague = new System.Windows.Forms.ComboBox();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.bContinue = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.bSave = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.bExtend = new System.Windows.Forms.Button();
            this.tIPAddress = new System.Windows.Forms.TextBox();
            this.tPort = new System.Windows.Forms.TextBox();
            this.rbDebug = new System.Windows.Forms.RichTextBox();
            this.tDelay = new System.Windows.Forms.TextBox();
            this.lPozice1 = new System.Windows.Forms.Label();
            this.lPozice2 = new System.Windows.Forms.Label();
            this.lIP = new System.Windows.Forms.Label();
            this.lDelay = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.pRight = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            lStaticHrac = new System.Windows.Forms.Label();
            lStaticKroky = new System.Windows.Forms.Label();
            lStaticCas = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.pSettings.SuspendLayout();
            this.pMain.SuspendLayout();
            this.pRight.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lStaticHrac
            // 
            lStaticHrac.AutoSize = true;
            lStaticHrac.Location = new System.Drawing.Point(287, 27);
            lStaticHrac.Name = "lStaticHrac";
            lStaticHrac.Size = new System.Drawing.Size(42, 13);
            lStaticHrac.TabIndex = 5;
            lStaticHrac.Text = "Player: ";
            // 
            // lStaticKroky
            // 
            lStaticKroky.AutoSize = true;
            lStaticKroky.Location = new System.Drawing.Point(338, 10);
            lStaticKroky.Name = "lStaticKroky";
            lStaticKroky.Size = new System.Drawing.Size(37, 13);
            lStaticKroky.TabIndex = 18;
            lStaticKroky.Text = "Steps:";
            // 
            // lStaticCas
            // 
            lStaticCas.AutoSize = true;
            lStaticCas.Location = new System.Drawing.Point(253, 10);
            lStaticCas.Name = "lStaticCas";
            lStaticCas.Size = new System.Drawing.Size(33, 13);
            lStaticCas.TabIndex = 16;
            lStaticCas.Text = "Time:";
            // 
            // bStart
            // 
            this.bStart.Location = new System.Drawing.Point(0, 23);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(38, 20);
            this.bStart.TabIndex = 0;
            this.bStart.Tag = "0";
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.bStart_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StL});
            this.statusStrip1.Location = new System.Drawing.Point(0, 550);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(869, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "Autor: Martin Všetička, 2008";
            // 
            // StL
            // 
            this.StL.Name = "StL";
            this.StL.Size = new System.Drawing.Size(0, 17);
            // 
            // bRestart
            // 
            this.bRestart.Location = new System.Drawing.Point(41, 23);
            this.bRestart.Name = "bRestart";
            this.bRestart.Size = new System.Drawing.Size(47, 20);
            this.bRestart.TabIndex = 10;
            this.bRestart.Text = "Again";
            this.bRestart.UseVisualStyleBackColor = true;
            this.bRestart.Click += new System.EventHandler(this.bRestart_Click);
            // 
            // lUser
            // 
            this.lUser.AutoSize = true;
            this.lUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lUser.Location = new System.Drawing.Point(323, 27);
            this.lUser.Name = "lUser";
            this.lUser.Size = new System.Drawing.Size(61, 13);
            this.lUser.TabIndex = 8;
            this.lUser.Text = "No player";
            // 
            // pSettings
            // 
            this.pSettings.Controls.Add(this.cbRecord);
            this.pSettings.Controls.Add(this.cbSounds);
            this.pSettings.Controls.Add(this.cbCharts);
            this.pSettings.Controls.Add(this.lSteps);
            this.pSettings.Controls.Add(lStaticKroky);
            this.pSettings.Controls.Add(this.lTime);
            this.pSettings.Controls.Add(lStaticCas);
            this.pSettings.Location = new System.Drawing.Point(12, 462);
            this.pSettings.Name = "pSettings";
            this.pSettings.Size = new System.Drawing.Size(408, 38);
            this.pSettings.TabIndex = 16;
            // 
            // cbRecord
            // 
            this.cbRecord.AutoSize = true;
            this.cbRecord.Location = new System.Drawing.Point(119, 0);
            this.cbRecord.Name = "cbRecord";
            this.cbRecord.Size = new System.Drawing.Size(90, 17);
            this.cbRecord.TabIndex = 22;
            this.cbRecord.TabStop = false;
            this.cbRecord.Text = "Record game";
            this.cbRecord.UseVisualStyleBackColor = true;
            // 
            // cbSounds
            // 
            this.cbSounds.AutoSize = true;
            this.cbSounds.Checked = true;
            this.cbSounds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSounds.Location = new System.Drawing.Point(3, 16);
            this.cbSounds.Name = "cbSounds";
            this.cbSounds.Size = new System.Drawing.Size(62, 17);
            this.cbSounds.TabIndex = 21;
            this.cbSounds.TabStop = false;
            this.cbSounds.Text = "Sounds";
            this.cbSounds.UseVisualStyleBackColor = true;
            // 
            // cbCharts
            // 
            this.cbCharts.AutoSize = true;
            this.cbCharts.Location = new System.Drawing.Point(3, 0);
            this.cbCharts.Name = "cbCharts";
            this.cbCharts.Size = new System.Drawing.Size(85, 17);
            this.cbCharts.TabIndex = 20;
            this.cbCharts.TabStop = false;
            this.cbCharts.Text = "Show charts";
            this.cbCharts.UseVisualStyleBackColor = true;
            // 
            // lSteps
            // 
            this.lSteps.AutoSize = true;
            this.lSteps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lSteps.Location = new System.Drawing.Point(381, 10);
            this.lSteps.Name = "lSteps";
            this.lSteps.Size = new System.Drawing.Size(14, 13);
            this.lSteps.TabIndex = 19;
            this.lSteps.Text = "0";
            // 
            // lTime
            // 
            this.lTime.AutoSize = true;
            this.lTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lTime.Location = new System.Drawing.Point(287, 10);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(32, 13);
            this.lTime.TabIndex = 17;
            this.lTime.Text = "0:00";
            // 
            // pMain
            // 
            this.pMain.Controls.Add(this.bLoad);
            this.pMain.Controls.Add(this.lStaticRound);
            this.pMain.Controls.Add(this.lRound);
            this.pMain.Controls.Add(this.lUser);
            this.pMain.Controls.Add(lStaticHrac);
            this.pMain.Controls.Add(this.bRestart);
            this.pMain.Controls.Add(this.bStart);
            this.pMain.Location = new System.Drawing.Point(12, 8);
            this.pMain.Name = "pMain";
            this.pMain.Size = new System.Drawing.Size(410, 57);
            this.pMain.TabIndex = 17;
            // 
            // bLoad
            // 
            this.bLoad.Location = new System.Drawing.Point(90, 23);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(45, 20);
            this.bLoad.TabIndex = 12;
            this.bLoad.Text = "Load";
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // lStaticRound
            // 
            this.lStaticRound.AutoSize = true;
            this.lStaticRound.Location = new System.Drawing.Point(141, 27);
            this.lStaticRound.Name = "lStaticRound";
            this.lStaticRound.Size = new System.Drawing.Size(42, 13);
            this.lStaticRound.TabIndex = 11;
            this.lStaticRound.Text = "Round:";
            // 
            // lRound
            // 
            this.lRound.AutoSize = true;
            this.lRound.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lRound.Location = new System.Drawing.Point(180, 27);
            this.lRound.Name = "lRound";
            this.lRound.Size = new System.Drawing.Size(58, 13);
            this.lRound.TabIndex = 10;
            this.lRound.Text = "<Round>";
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(3, 23);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(71, 25);
            this.bConnect.TabIndex = 25;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // bListen
            // 
            this.bListen.Location = new System.Drawing.Point(3, 1);
            this.bListen.Name = "bListen";
            this.bListen.Size = new System.Drawing.Size(71, 21);
            this.bListen.TabIndex = 23;
            this.bListen.Text = "Listen";
            this.bListen.UseVisualStyleBackColor = true;
            this.bListen.Click += new System.EventHandler(this.bListen_Click);
            // 
            // cbLeague
            // 
            this.cbLeague.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLeague.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cbLeague.FormattingEnabled = true;
            this.cbLeague.Location = new System.Drawing.Point(12, 8);
            this.cbLeague.Name = "cbLeague";
            this.cbLeague.Size = new System.Drawing.Size(410, 23);
            this.cbLeague.TabIndex = 20;
            this.cbLeague.TabStop = false;
            this.cbLeague.SelectedIndexChanged += new System.EventHandler(this.cbLiga_SelectedIndexChanged);
            this.cbLeague.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbLeague_KeyDown);
            // 
            // lbLog
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.IntegralHeight = false;
            this.lbLog.Location = new System.Drawing.Point(3, 57);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(71, 381);
            this.lbLog.TabIndex = 19;
            this.lbLog.SelectedIndexChanged += new System.EventHandler(this.LBlog_SelectedIndexChanged);
            // 
            // bContinue
            // 
            this.bContinue.Location = new System.Drawing.Point(3, 466);
            this.bContinue.Name = "bContinue";
            this.bContinue.Size = new System.Drawing.Size(71, 23);
            this.bContinue.TabIndex = 21;
            this.bContinue.Text = "Continue";
            this.bContinue.UseVisualStyleBackColor = true;
            this.bContinue.Click += new System.EventHandler(this.bContinue_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "All Supported Files (*.srf)|*.srf";
            this.openFileDialog1.Title = "Load record";
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(3, 444);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(71, 23);
            this.bSave.TabIndex = 22;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Sokoban replay file (*.srf)|*.srf";
            // 
            // bExtend
            // 
            this.bExtend.Location = new System.Drawing.Point(423, 191);
            this.bExtend.Name = "bExtend";
            this.bExtend.Size = new System.Drawing.Size(10, 173);
            this.bExtend.TabIndex = 26;
            this.bExtend.TabStop = false;
            this.bExtend.Tag = "Closed";
            this.bExtend.Text = ">";
            this.bExtend.UseVisualStyleBackColor = true;
            this.bExtend.Click += new System.EventHandler(this.bExtend_Click);
            // 
            // tIPAddress
            // 
            this.tIPAddress.Location = new System.Drawing.Point(31, 1);
            this.tIPAddress.Name = "tIPAddress";
            this.tIPAddress.Size = new System.Drawing.Size(71, 20);
            this.tIPAddress.TabIndex = 27;
            this.tIPAddress.Text = "127.0.0.1";
            // 
            // tPort
            // 
            this.tPort.Location = new System.Drawing.Point(108, 1);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(40, 20);
            this.tPort.TabIndex = 28;
            this.tPort.Text = "49773";
            // 
            // rbDebug
            // 
            this.rbDebug.Location = new System.Drawing.Point(12, 80);
            this.rbDebug.Name = "rbDebug";
            this.rbDebug.Size = new System.Drawing.Size(402, 370);
            this.rbDebug.TabIndex = 29;
            this.rbDebug.Text = "";
            this.rbDebug.Visible = false;
            // 
            // tDelay
            // 
            this.tDelay.Location = new System.Drawing.Point(202, 1);
            this.tDelay.Name = "tDelay";
            this.tDelay.Size = new System.Drawing.Size(36, 20);
            this.tDelay.TabIndex = 32;
            this.tDelay.Text = "50";
            // 
            // lPozice1
            // 
            this.lPozice1.AutoSize = true;
            this.lPozice1.Location = new System.Drawing.Point(16, 504);
            this.lPozice1.Name = "lPozice1";
            this.lPozice1.Size = new System.Drawing.Size(35, 13);
            this.lPozice1.TabIndex = 33;
            this.lPozice1.Text = "label1";
            this.lPozice1.Visible = false;
            // 
            // lPozice2
            // 
            this.lPozice2.AutoSize = true;
            this.lPozice2.Location = new System.Drawing.Point(534, 507);
            this.lPozice2.Name = "lPozice2";
            this.lPozice2.Size = new System.Drawing.Size(35, 13);
            this.lPozice2.TabIndex = 34;
            this.lPozice2.Text = "label1";
            this.lPozice2.Visible = false;
            // 
            // lIP
            // 
            this.lIP.AutoSize = true;
            this.lIP.Location = new System.Drawing.Point(5, 4);
            this.lIP.Name = "lIP";
            this.lIP.Size = new System.Drawing.Size(20, 13);
            this.lIP.TabIndex = 35;
            this.lIP.Text = "IP:";
            // 
            // lDelay
            // 
            this.lDelay.AutoSize = true;
            this.lDelay.Location = new System.Drawing.Point(161, 4);
            this.lDelay.Name = "lDelay";
            this.lDelay.Size = new System.Drawing.Size(37, 13);
            this.lDelay.TabIndex = 36;
            this.lDelay.Text = "Delay:";
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(28, 35);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(99, 13);
            this.lStatus.TabIndex = 37;
            this.lStatus.Text = "Network status: OK";
            // 
            // pRight
            // 
            this.pRight.Controls.Add(this.bConnect);
            this.pRight.Controls.Add(this.bSave);
            this.pRight.Controls.Add(this.bContinue);
            this.pRight.Controls.Add(this.lbLog);
            this.pRight.Controls.Add(this.bListen);
            this.pRight.Location = new System.Drawing.Point(437, 8);
            this.pRight.Name = "pRight";
            this.pRight.Size = new System.Drawing.Size(77, 500);
            this.pRight.TabIndex = 38;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lStatus);
            this.panel1.Controls.Add(this.lDelay);
            this.panel1.Controls.Add(this.lIP);
            this.panel1.Controls.Add(this.tDelay);
            this.panel1.Controls.Add(this.tPort);
            this.panel1.Controls.Add(this.tIPAddress);
            this.panel1.Location = new System.Drawing.Point(529, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(252, 55);
            this.panel1.TabIndex = 39;
            // 
            // GameDeskView
            // 
            this.AcceptButton = this.bStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 572);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pRight);
            this.Controls.Add(this.lPozice2);
            this.Controls.Add(this.lPozice1);
            this.Controls.Add(this.rbDebug);
            this.Controls.Add(this.bExtend);
            this.Controls.Add(this.cbLeague);
            this.Controls.Add(this.pMain);
            this.Controls.Add(this.pSettings);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(445, 580);
            this.Name = "GameDeskView";
            this.Text = "Sokoban";
            this.Load += new System.EventHandler(this.GameDeskView_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameDeskView_Paint);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameDeskView_FormClosed_1);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameDeskView_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameDeskView_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pSettings.ResumeLayout(false);
            this.pSettings.PerformLayout();
            this.pMain.ResumeLayout(false);
            this.pMain.PerformLayout();
            this.pRight.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// "Start/Pause" button
        /// </summary>
        public System.Windows.Forms.Button bStart;
        public System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel StL;
        public System.Windows.Forms.Button bRestart;
        public System.Windows.Forms.Label lUser;
        public System.Windows.Forms.Panel pSettings;
        public System.Windows.Forms.CheckBox cbSounds;
        public System.Windows.Forms.CheckBox cbCharts;
        public System.Windows.Forms.Label lSteps;

        /// <summary>
        /// Displays time that elapsed from start of playing actual round
        /// </summary>
        public System.Windows.Forms.Label lTime;
        public System.Windows.Forms.Panel pMain;

        /// <summary>
        /// Checkbox: Record game
        /// </summary>
        public System.Windows.Forms.CheckBox cbRecord;

        /// <summary>
        /// ListBox: Steps of Sokoban in a round
        /// </summary>
        public System.Windows.Forms.ListBox lbLog;

        /// <summary>
        /// Actual round name
        /// </summary>
        public System.Windows.Forms.Label lRound;
        private System.Windows.Forms.Label lStaticRound;

        /// <summary>
        /// List of available leagues
        /// </summary>
        public System.Windows.Forms.ComboBox cbLeague;
        private System.Windows.Forms.Button bContinue;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        public System.Windows.Forms.Button bListen;
        public System.Windows.Forms.Button bConnect;
        public System.Windows.Forms.Button bExtend;
        public System.Windows.Forms.TextBox tIPAddress;
        public System.Windows.Forms.TextBox tPort;
        private System.Windows.Forms.RichTextBox rbDebug;
        public System.Windows.Forms.TextBox tDelay;
        private System.Windows.Forms.Label lPozice1;
        private System.Windows.Forms.Label lPozice2;
        private System.Windows.Forms.Label lIP;
        private System.Windows.Forms.Label lDelay;
        public System.Windows.Forms.Label lStatus;
        public System.Windows.Forms.Panel pRight;
        private System.Windows.Forms.Panel panel1;
    }
}

