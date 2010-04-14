using System.Windows.Forms;

namespace Sokoban.Dialogs
{
    partial class LoggingIn
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
            System.Windows.Forms.Label lStaticName;
            System.Windows.Forms.Label LStaticPass;
            this.tJmeno = new System.Windows.Forms.TextBox();
            this.bOK = new System.Windows.Forms.Button();
            this.tPassword = new System.Windows.Forms.TextBox();
            this.lStatus = new System.Windows.Forms.Label();
            lStaticName = new System.Windows.Forms.Label();
            LStaticPass = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lStaticName
            // 
            lStaticName.AutoSize = true;
            lStaticName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            lStaticName.Location = new System.Drawing.Point(37, 39);
            lStaticName.Name = "lStaticName";
            lStaticName.Size = new System.Drawing.Size(52, 16);
            lStaticName.TabIndex = 0;
            lStaticName.Text = "Jméno:";
            // 
            // LStaticPass
            // 
            LStaticPass.AutoSize = true;
            LStaticPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            LStaticPass.Location = new System.Drawing.Point(37, 64);
            LStaticPass.Name = "LStaticPass";
            LStaticPass.Size = new System.Drawing.Size(47, 16);
            LStaticPass.TabIndex = 3;
            LStaticPass.Text = "Heslo:";
            // 
            // tJmeno
            // 
            this.tJmeno.Location = new System.Drawing.Point(95, 38);
            this.tJmeno.MaxLength = 15;
            this.tJmeno.Name = "tJmeno";
            this.tJmeno.Size = new System.Drawing.Size(213, 20);
            this.tJmeno.TabIndex = 1;
            this.tJmeno.Text = "Anonymous";
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(314, 38);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(40, 20);
            this.bOK.TabIndex = 3;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // tPassword
            // 
            this.tPassword.Location = new System.Drawing.Point(95, 63);
            this.tPassword.Name = "tPassword";
            this.tPassword.Size = new System.Drawing.Size(213, 20);
            this.tPassword.TabIndex = 2;
            this.tPassword.UseSystemPasswordChar = true;
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(37, 87);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(43, 13);
            this.lStatus.TabIndex = 4;
            this.lStatus.Text = "[Status]";
            // 
            // LogInDialog
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 105);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.tPassword);
            this.Controls.Add(LStaticPass);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.tJmeno);
            this.Controls.Add(lStaticName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LogInDialog";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Fdialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogInDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tJmeno;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.TextBox tPassword;
        private System.Windows.Forms.Label lStatus;
    }
}