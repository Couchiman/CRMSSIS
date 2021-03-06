﻿namespace CRMSSIS.CRMDestinationAdapter
{
    partial class CRMDestinationAdapterUIForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CRMDestinationAdapterUIForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbConnectionList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNewConnectionManager = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnRefreshMetadata = new System.Windows.Forms.Button();
            this.btnMappings = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbEntity = new System.Windows.Forms.ComboBox();
            this.lblOperation = new System.Windows.Forms.Label();
            this.cbOperation = new System.Windows.Forms.ComboBox();
            this.cboLocales = new System.Windows.Forms.ComboBox();
            this.lblCultureInfo = new System.Windows.Forms.Label();
            this.chkErrorFail = new System.Windows.Forms.CheckBox();
            this.chkRedirect = new System.Windows.Forms.CheckBox();
            this.chkIgnoreError = new System.Windows.Forms.CheckBox();
            this.gpError = new System.Windows.Forms.GroupBox();
            this.backgroundWorkerLoadEntities = new System.ComponentModel.BackgroundWorker();
            this.pbLoader = new System.Windows.Forms.PictureBox();
            this.gpExecuteWF = new System.Windows.Forms.GroupBox();
            this.btnRefreshWorkflows = new System.Windows.Forms.Button();
            this.lblWorkflow = new System.Windows.Forms.Label();
            this.cboWorkflows = new System.Windows.Forms.ComboBox();
            this.backgroundWorkerLoadWorkflows = new System.ComponentModel.BackgroundWorker();
            this.btnAbout = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gpError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoader)).BeginInit();
            this.gpExecuteWF.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.cbConnectionList);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnNewConnectionManager);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 88);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Settings";
            // 
            // cbConnectionList
            // 
            this.cbConnectionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConnectionList.FormattingEnabled = true;
            this.cbConnectionList.Location = new System.Drawing.Point(120, 35);
            this.cbConnectionList.Name = "cbConnectionList";
            this.cbConnectionList.Size = new System.Drawing.Size(188, 21);
            this.cbConnectionList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection Manager";
            // 
            // btnNewConnectionManager
            // 
            this.btnNewConnectionManager.Location = new System.Drawing.Point(314, 33);
            this.btnNewConnectionManager.Name = "btnNewConnectionManager";
            this.btnNewConnectionManager.Size = new System.Drawing.Size(75, 23);
            this.btnNewConnectionManager.TabIndex = 2;
            this.btnNewConnectionManager.Text = "New";
            this.btnNewConnectionManager.UseVisualStyleBackColor = true;
            this.btnNewConnectionManager.Click += new System.EventHandler(this.btnNewConnectionManager_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(528, 329);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(418, 329);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnRefreshMetadata
            // 
            this.btnRefreshMetadata.Location = new System.Drawing.Point(162, 329);
            this.btnRefreshMetadata.Name = "btnRefreshMetadata";
            this.btnRefreshMetadata.Size = new System.Drawing.Size(107, 23);
            this.btnRefreshMetadata.TabIndex = 13;
            this.btnRefreshMetadata.Text = "Refresh Metadata";
            this.btnRefreshMetadata.UseVisualStyleBackColor = true;
            this.btnRefreshMetadata.Click += new System.EventHandler(this.btnRefreshMetadata_Click);
            // 
            // btnMappings
            // 
            this.btnMappings.Enabled = false;
            this.btnMappings.Location = new System.Drawing.Point(300, 329);
            this.btnMappings.Name = "btnMappings";
            this.btnMappings.Size = new System.Drawing.Size(80, 23);
            this.btnMappings.TabIndex = 100;
            this.btnMappings.Text = "Mappings";
            this.btnMappings.UseVisualStyleBackColor = true;
            this.btnMappings.Click += new System.EventHandler(this.btnMappings_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRefresh);
            this.groupBox2.Controls.Add(this.txtBatchSize);
            this.groupBox2.Controls.Add(this.lblBatchSize);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbEntity);
            this.groupBox2.Controls.Add(this.lblOperation);
            this.groupBox2.Controls.Add(this.cbOperation);
            this.groupBox2.Location = new System.Drawing.Point(8, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(807, 137);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configure Operation";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(518, 56);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(116, 23);
            this.btnRefresh.TabIndex = 25;
            this.btnRefresh.Text = "Refresh Entities";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.Location = new System.Drawing.Point(120, 90);
            this.txtBatchSize.MaxLength = 3;
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(100, 20);
            this.txtBatchSize.TabIndex = 24;
            this.txtBatchSize.Text = "1";
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(15, 97);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(58, 13);
            this.lblBatchSize.TabIndex = 23;
            this.lblBatchSize.Text = "Batch Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Entity";
            // 
            // cbEntity
            // 
            this.cbEntity.FormattingEnabled = true;
            this.cbEntity.Location = new System.Drawing.Point(120, 56);
            this.cbEntity.Name = "cbEntity";
            this.cbEntity.Size = new System.Drawing.Size(374, 21);
            this.cbEntity.TabIndex = 21;
            // 
            // lblOperation
            // 
            this.lblOperation.AutoSize = true;
            this.lblOperation.Location = new System.Drawing.Point(15, 33);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.Size = new System.Drawing.Size(53, 13);
            this.lblOperation.TabIndex = 20;
            this.lblOperation.Text = "Operation";
            // 
            // cbOperation
            // 
            this.cbOperation.FormattingEnabled = true;
            this.cbOperation.Location = new System.Drawing.Point(120, 25);
            this.cbOperation.Name = "cbOperation";
            this.cbOperation.Size = new System.Drawing.Size(187, 21);
            this.cbOperation.TabIndex = 19;
            // 
            // cboLocales
            // 
            this.cboLocales.FormattingEnabled = true;
            this.cboLocales.Location = new System.Drawing.Point(648, 30);
            this.cboLocales.Name = "cboLocales";
            this.cboLocales.Size = new System.Drawing.Size(121, 21);
            this.cboLocales.TabIndex = 101;
            // 
            // lblCultureInfo
            // 
            this.lblCultureInfo.AutoSize = true;
            this.lblCultureInfo.Location = new System.Drawing.Point(645, 14);
            this.lblCultureInfo.Name = "lblCultureInfo";
            this.lblCultureInfo.Size = new System.Drawing.Size(61, 13);
            this.lblCultureInfo.TabIndex = 3;
            this.lblCultureInfo.Text = "Culture Info";
            // 
            // chkErrorFail
            // 
            this.chkErrorFail.AutoSize = true;
            this.chkErrorFail.Location = new System.Drawing.Point(16, 38);
            this.chkErrorFail.Name = "chkErrorFail";
            this.chkErrorFail.Size = new System.Drawing.Size(82, 17);
            this.chkErrorFail.TabIndex = 102;
            this.chkErrorFail.Text = "Fail on Error";
            this.chkErrorFail.UseVisualStyleBackColor = true;
            // 
            // chkRedirect
            // 
            this.chkRedirect.AutoSize = true;
            this.chkRedirect.Location = new System.Drawing.Point(16, 19);
            this.chkRedirect.Name = "chkRedirect";
            this.chkRedirect.Size = new System.Drawing.Size(66, 17);
            this.chkRedirect.TabIndex = 103;
            this.chkRedirect.Text = "Redirect";
            this.chkRedirect.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreError
            // 
            this.chkIgnoreError.AutoSize = true;
            this.chkIgnoreError.Location = new System.Drawing.Point(16, 58);
            this.chkIgnoreError.Name = "chkIgnoreError";
            this.chkIgnoreError.Size = new System.Drawing.Size(56, 17);
            this.chkIgnoreError.TabIndex = 104;
            this.chkIgnoreError.Text = "Ignore";
            this.chkIgnoreError.UseVisualStyleBackColor = true;
            // 
            // gpError
            // 
            this.gpError.Controls.Add(this.chkRedirect);
            this.gpError.Controls.Add(this.chkErrorFail);
            this.gpError.Controls.Add(this.chkIgnoreError);
            this.gpError.Location = new System.Drawing.Point(526, 14);
            this.gpError.Name = "gpError";
            this.gpError.Size = new System.Drawing.Size(116, 82);
            this.gpError.TabIndex = 105;
            this.gpError.TabStop = false;
            this.gpError.Text = "Input Error Handling";
            // 
            // backgroundWorkerLoadEntities
            // 
            this.backgroundWorkerLoadEntities.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // pbLoader
            // 
            this.pbLoader.Location = new System.Drawing.Point(464, 14);
            this.pbLoader.Name = "pbLoader";
            this.pbLoader.Size = new System.Drawing.Size(54, 50);
            this.pbLoader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbLoader.TabIndex = 99;
            this.pbLoader.TabStop = false;
            // 
            // gpExecuteWF
            // 
            this.gpExecuteWF.Controls.Add(this.btnRefreshWorkflows);
            this.gpExecuteWF.Controls.Add(this.lblWorkflow);
            this.gpExecuteWF.Controls.Add(this.cboWorkflows);
            this.gpExecuteWF.Location = new System.Drawing.Point(13, 246);
            this.gpExecuteWF.Name = "gpExecuteWF";
            this.gpExecuteWF.Size = new System.Drawing.Size(802, 66);
            this.gpExecuteWF.TabIndex = 106;
            this.gpExecuteWF.TabStop = false;
            this.gpExecuteWF.Text = "Execute Workflow";
            // 
            // btnRefreshWorkflows
            // 
            this.btnRefreshWorkflows.Location = new System.Drawing.Point(515, 24);
            this.btnRefreshWorkflows.Name = "btnRefreshWorkflows";
            this.btnRefreshWorkflows.Size = new System.Drawing.Size(116, 23);
            this.btnRefreshWorkflows.TabIndex = 26;
            this.btnRefreshWorkflows.Text = "Refresh Workflows";
            this.btnRefreshWorkflows.UseVisualStyleBackColor = true;
            this.btnRefreshWorkflows.Click += new System.EventHandler(this.btnRefreshWorkflows_Click);
            // 
            // lblWorkflow
            // 
            this.lblWorkflow.AutoSize = true;
            this.lblWorkflow.Location = new System.Drawing.Point(10, 34);
            this.lblWorkflow.Name = "lblWorkflow";
            this.lblWorkflow.Size = new System.Drawing.Size(52, 13);
            this.lblWorkflow.TabIndex = 24;
            this.lblWorkflow.Text = "Workflow";
            // 
            // cboWorkflows
            // 
            this.cboWorkflows.FormattingEnabled = true;
            this.cboWorkflows.Location = new System.Drawing.Point(115, 26);
            this.cboWorkflows.Name = "cboWorkflows";
            this.cboWorkflows.Size = new System.Drawing.Size(374, 21);
            this.cboWorkflows.TabIndex = 23;
            // 
            // backgroundWorkerLoadWorkflows
            // 
            this.backgroundWorkerLoadWorkflows.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoadWorkflows_DoWork);
            // 
            // btnAbout
            // 
            this.btnAbout.BackColor = System.Drawing.SystemColors.Info;
            this.btnAbout.ForeColor = System.Drawing.SystemColors.Info;
            this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
            this.btnAbout.Location = new System.Drawing.Point(791, 12);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(24, 23);
            this.btnAbout.TabIndex = 107;
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // CRMDestinationAdapterUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 363);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.pbLoader);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRefreshMetadata);
            this.Controls.Add(this.btnMappings);
            this.Controls.Add(this.lblCultureInfo);
            this.Controls.Add(this.cboLocales);
            this.Controls.Add(this.gpError);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gpExecuteWF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CRMDestinationAdapterUIForm";
            this.Text = "CRM Destination Adapter";
            this.Load += new System.EventHandler(this.CRMDestinationAdapterUIForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gpError.ResumeLayout(false);
            this.gpError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoader)).EndInit();
            this.gpExecuteWF.ResumeLayout(false);
            this.gpExecuteWF.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbConnectionList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNewConnectionManager;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBatchSize;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbEntity;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.ComboBox cbOperation;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadEntities;
        private System.Windows.Forms.PictureBox pbLoader;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnRefreshMetadata;
        private System.Windows.Forms.Button btnMappings;
        private System.Windows.Forms.ComboBox cboLocales;
        private System.Windows.Forms.Label lblCultureInfo;
        private System.Windows.Forms.CheckBox chkErrorFail;
        private System.Windows.Forms.CheckBox chkRedirect;
        private System.Windows.Forms.CheckBox chkIgnoreError;
        private System.Windows.Forms.GroupBox gpError;
        private System.Windows.Forms.GroupBox gpExecuteWF;
        private System.Windows.Forms.Label lblWorkflow;
        private System.Windows.Forms.ComboBox cboWorkflows;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadWorkflows;
        private System.Windows.Forms.Button btnRefreshWorkflows;
        private System.Windows.Forms.Button btnAbout;
    }
}