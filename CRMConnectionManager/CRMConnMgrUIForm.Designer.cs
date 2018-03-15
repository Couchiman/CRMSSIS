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
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lblAuthType = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cboAuthType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(256, 197);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(71, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(400, 197);
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
            this.lblOrganizationUri.Location = new System.Drawing.Point(21, 53);
            this.lblOrganizationUri.Name = "lblOrganizationUri";
            this.lblOrganizationUri.Size = new System.Drawing.Size(79, 13);
            this.lblOrganizationUri.TabIndex = 2;
            this.lblOrganizationUri.Text = "OrganizationUri";
            // 
            // txtOrganizationUri
            // 
            this.txtOrganizationUri.Location = new System.Drawing.Point(125, 48);
            this.txtOrganizationUri.Name = "txtOrganizationUri";
            this.txtOrganizationUri.Size = new System.Drawing.Size(346, 20);
            this.txtOrganizationUri.TabIndex = 3;
            // 
            // lblHomeRealmUri
            // 
            this.lblHomeRealmUri.AutoSize = true;
            this.lblHomeRealmUri.Location = new System.Drawing.Point(21, 81);
            this.lblHomeRealmUri.Name = "lblHomeRealmUri";
            this.lblHomeRealmUri.Size = new System.Drawing.Size(78, 13);
            this.lblHomeRealmUri.TabIndex = 4;
            this.lblHomeRealmUri.Text = "HomeRealmUri";
            // 
            // txtHomeRealUri
            // 
            this.txtHomeRealUri.Location = new System.Drawing.Point(124, 78);
            this.txtHomeRealUri.Name = "txtHomeRealUri";
            this.txtHomeRealUri.Size = new System.Drawing.Size(350, 20);
            this.txtHomeRealUri.TabIndex = 5;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(21, 137);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(57, 13);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "UserName";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(21, 164);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "Password";
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(21, 110);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(43, 13);
            this.lblDomain.TabIndex = 8;
            this.lblDomain.Text = "Domain";
            // 
            // lblAuthType
            // 
            this.lblAuthType.AutoSize = true;
            this.lblAuthType.Location = new System.Drawing.Point(21, 26);
            this.lblAuthType.Name = "lblAuthType";
            this.lblAuthType.Size = new System.Drawing.Size(102, 13);
            this.lblAuthType.TabIndex = 9;
            this.lblAuthType.Text = "Authentication Type";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(124, 107);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(133, 20);
            this.txtDomain.TabIndex = 10;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(124, 134);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(203, 20);
            this.txtUserName.TabIndex = 11;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(124, 160);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(133, 20);
            this.txtPassword.TabIndex = 12;
            // 
            // cboAuthType
            // 
            this.cboAuthType.FormattingEnabled = true;
            this.cboAuthType.Location = new System.Drawing.Point(125, 18);
            this.cboAuthType.Name = "cboAuthType";
            this.cboAuthType.Size = new System.Drawing.Size(132, 21);
            this.cboAuthType.TabIndex = 13;
            this.cboAuthType.SelectedIndexChanged += new System.EventHandler(this.cboAuthType_SelectedIndexChanged);
            // 
            // CRMConnMgrUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 236);
            this.Controls.Add(this.cboAuthType);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtDomain);
            this.Controls.Add(this.lblAuthType);
            this.Controls.Add(this.lblDomain);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtHomeRealUri);
            this.Controls.Add(this.lblHomeRealmUri);
            this.Controls.Add(this.txtOrganizationUri);
            this.Controls.Add(this.lblOrganizationUri);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CRMConnMgrUIForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CRM Dynamics Connection Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblOrganizationUri;
        private System.Windows.Forms.TextBox txtOrganizationUri;
        private System.Windows.Forms.Label lblHomeRealmUri;
        private System.Windows.Forms.TextBox txtHomeRealUri;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Label lblAuthType;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.ComboBox cboAuthType;
    }
}