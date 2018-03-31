namespace CRMSSIS.CRMDestinationAdapter
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbEntity = new System.Windows.Forms.ComboBox();
            this.lblOperation = new System.Windows.Forms.Label();
            this.cbOperation = new System.Windows.Forms.ComboBox();
            this.dgAtributeMap = new System.Windows.Forms.DataGridView();
            this.backgroundWorkerLoadEntities = new System.ComponentModel.BackgroundWorker();
            this.pbLoader = new System.Windows.Forms.PictureBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAtributeMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoader)).BeginInit();
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
            this.btnCancel.Location = new System.Drawing.Point(322, 441);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(240, 441);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            // txtBatchSize
            // 
            this.txtBatchSize.Location = new System.Drawing.Point(120, 90);
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
            // dgAtributeMap
            // 
            this.dgAtributeMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAtributeMap.Location = new System.Drawing.Point(12, 245);
            this.dgAtributeMap.Name = "dgAtributeMap";
            this.dgAtributeMap.Size = new System.Drawing.Size(803, 179);
            this.dgAtributeMap.TabIndex = 22;
            // 
            // backgroundWorkerLoadEntities
            // 
            this.backgroundWorkerLoadEntities.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // pbLoader
            // 
            this.pbLoader.Location = new System.Drawing.Point(464, 14);
            this.pbLoader.Name = "pbLoader";
            this.pbLoader.Size = new System.Drawing.Size(161, 82);
            this.pbLoader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbLoader.TabIndex = 23;
            this.pbLoader.TabStop = false;
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
            // CRMDestinationAdapterUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 476);
            this.Controls.Add(this.pbLoader);
            this.Controls.Add(this.dgAtributeMap);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CRMDestinationAdapterUIForm";
            this.Text = "CRM Destination Adapter";
            this.Load += new System.EventHandler(this.CRMDestinationAdapterUIForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAtributeMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoader)).EndInit();
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
        private System.Windows.Forms.DataGridView dgAtributeMap;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadEntities;
        private System.Windows.Forms.PictureBox pbLoader;
        private System.Windows.Forms.Button btnRefresh;
    }
}