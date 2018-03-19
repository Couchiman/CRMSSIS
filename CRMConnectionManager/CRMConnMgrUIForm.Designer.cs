namespace CRMSSIS.CRMConnectionManager
{
    partial class CRMConnMgrUIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CRMConnMgrUIForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblOrganizationUri = new System.Windows.Forms.Label();
            this.txtOrganizationUri = new System.Windows.Forms.TextBox();
            this.lblHomeRealmUri = new System.Windows.Forms.Label();
            this.txtHomeRealUri = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lblAuthType = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.cboAuthType = new System.Windows.Forms.ComboBox();
            this.txtCallerId = new System.Windows.Forms.TextBox();
            this.lblCallerId = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSSL = new System.Windows.Forms.CheckBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblminutes = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(198, 309);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(71, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(297, 309);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblOrganizationUri
            // 
            this.lblOrganizationUri.AutoSize = true;
            this.lblOrganizationUri.Location = new System.Drawing.Point(8, 53);
            this.lblOrganizationUri.Name = "lblOrganizationUri";
            this.lblOrganizationUri.Size = new System.Drawing.Size(79, 13);
            this.lblOrganizationUri.TabIndex = 2;
            this.lblOrganizationUri.Text = "OrganizationUri";
            // 
            // txtOrganizationUri
            // 
            this.txtOrganizationUri.Location = new System.Drawing.Point(112, 45);
            this.txtOrganizationUri.Name = "txtOrganizationUri";
            this.txtOrganizationUri.Size = new System.Drawing.Size(251, 20);
            this.txtOrganizationUri.TabIndex = 3;
            // 
            // lblHomeRealmUri
            // 
            this.lblHomeRealmUri.AutoSize = true;
            this.lblHomeRealmUri.Location = new System.Drawing.Point(9, 22);
            this.lblHomeRealmUri.Name = "lblHomeRealmUri";
            this.lblHomeRealmUri.Size = new System.Drawing.Size(78, 13);
            this.lblHomeRealmUri.TabIndex = 4;
            this.lblHomeRealmUri.Text = "HomeRealmUri";
            // 
            // txtHomeRealUri
            // 
            this.txtHomeRealUri.Location = new System.Drawing.Point(112, 19);
            this.txtHomeRealUri.Name = "txtHomeRealUri";
            this.txtHomeRealUri.Size = new System.Drawing.Size(350, 20);
            this.txtHomeRealUri.TabIndex = 5;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(8, 105);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(57, 13);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "UserName";
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(9, 49);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(45, 13);
            this.lblTimeout.TabIndex = 7;
            this.lblTimeout.Text = "Timeout";
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(8, 78);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(43, 13);
            this.lblDomain.TabIndex = 8;
            this.lblDomain.Text = "Domain";
            // 
            // lblAuthType
            // 
            this.lblAuthType.AutoSize = true;
            this.lblAuthType.Location = new System.Drawing.Point(8, 26);
            this.lblAuthType.Name = "lblAuthType";
            this.lblAuthType.Size = new System.Drawing.Size(102, 13);
            this.lblAuthType.TabIndex = 9;
            this.lblAuthType.Text = "Authentication Type";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(111, 75);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(133, 20);
            this.txtDomain.TabIndex = 10;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(111, 102);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(203, 20);
            this.txtUserName.TabIndex = 11;
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(112, 45);
            this.txtTimeout.MaxLength = 5;
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(40, 20);
            this.txtTimeout.TabIndex = 12;
            // 
            // cboAuthType
            // 
            this.cboAuthType.FormattingEnabled = true;
            this.cboAuthType.Location = new System.Drawing.Point(112, 18);
            this.cboAuthType.Name = "cboAuthType";
            this.cboAuthType.Size = new System.Drawing.Size(132, 21);
            this.cboAuthType.TabIndex = 13;
            this.cboAuthType.SelectedIndexChanged += new System.EventHandler(this.cboAuthType_SelectedIndexChanged);
            // 
            // txtCallerId
            // 
            this.txtCallerId.Location = new System.Drawing.Point(112, 71);
            this.txtCallerId.MaxLength = 5;
            this.txtCallerId.Name = "txtCallerId";
            this.txtCallerId.Size = new System.Drawing.Size(251, 20);
            this.txtCallerId.TabIndex = 15;
            // 
            // lblCallerId
            // 
            this.lblCallerId.AutoSize = true;
            this.lblCallerId.Location = new System.Drawing.Point(9, 75);
            this.lblCallerId.Name = "lblCallerId";
            this.lblCallerId.Size = new System.Drawing.Size(72, 13);
            this.lblCallerId.TabIndex = 14;
            this.lblCallerId.Text = "CallerId GUID";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(423, 49);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 17;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(455, 44);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(48, 20);
            this.txtPort.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblminutes);
            this.groupBox1.Controls.Add(this.txtHomeRealUri);
            this.groupBox1.Controls.Add(this.lblHomeRealmUri);
            this.groupBox1.Controls.Add(this.lblTimeout);
            this.groupBox1.Controls.Add(this.txtTimeout);
            this.groupBox1.Controls.Add(this.txtCallerId);
            this.groupBox1.Controls.Add(this.lblCallerId);
            this.groupBox1.Location = new System.Drawing.Point(24, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(520, 119);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.lblPassword);
            this.groupBox2.Controls.Add(this.chkSSL);
            this.groupBox2.Controls.Add(this.cboAuthType);
            this.groupBox2.Controls.Add(this.lblOrganizationUri);
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.txtOrganizationUri);
            this.groupBox2.Controls.Add(this.lblPort);
            this.groupBox2.Controls.Add(this.lblUsername);
            this.groupBox2.Controls.Add(this.lblDomain);
            this.groupBox2.Controls.Add(this.lblAuthType);
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.txtDomain);
            this.groupBox2.Location = new System.Drawing.Point(24, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(520, 157);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Basic";
            // 
            // chkSSL
            // 
            this.chkSSL.AutoSize = true;
            this.chkSSL.Location = new System.Drawing.Point(371, 49);
            this.chkSSL.Name = "chkSSL";
            this.chkSSL.Size = new System.Drawing.Size(46, 17);
            this.chkSSL.TabIndex = 19;
            this.chkSSL.Text = "SSL";
            this.chkSSL.UseVisualStyleBackColor = true;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(9, 131);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 20;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(112, 128);
            this.txtPassword.MaxLength = 20;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(133, 20);
            this.txtPassword.TabIndex = 16;
            // 
            // lblminutes
            // 
            this.lblminutes.AutoSize = true;
            this.lblminutes.Location = new System.Drawing.Point(158, 48);
            this.lblminutes.Name = "lblminutes";
            this.lblminutes.Size = new System.Drawing.Size(111, 13);
            this.lblminutes.TabIndex = 16;
            this.lblminutes.Text = "minutes. (0 for default)";
            // 
            // CRMConnMgrUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 348);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CRMConnMgrUIForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CRM Dynamics Connection Manager";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblOrganizationUri;
        private System.Windows.Forms.TextBox txtOrganizationUri;
        private System.Windows.Forms.Label lblHomeRealmUri;
        private System.Windows.Forms.TextBox txtHomeRealUri;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Label lblAuthType;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.ComboBox cboAuthType;
        private System.Windows.Forms.TextBox txtCallerId;
        private System.Windows.Forms.Label lblCallerId;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkSSL;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblminutes;
    }
}