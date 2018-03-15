using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
 


namespace CRMSSIS.CRMSourceAdapter
{
    public partial class CRMSourceAdapterUIForm : Form
    {

        private Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData;
       
        private IDtsConnectionService connectionService;
        private CManagedComponentWrapper designTimeInstance;

               
        private class ConnectionManagerItem
        {
            public string ID;
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }

        }

        public CRMSourceAdapterUIForm()
        {
            InitializeComponent();
        }

        public CRMSourceAdapterUIForm(Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData, IDtsConnectionService connectionService)
      : this()
        {
            this.metaData = metaData;
            this.connectionService = connectionService;
          
            this.designTimeInstance = metaData.Instantiate();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
          

            if (cbConnectionList.SelectedItem != null)
            {
                var item = (ConnectionManagerItem)cbConnectionList.SelectedItem;

             
                this.metaData.RuntimeConnectionCollection[0].Description = "Dynamics CRM Connection";
                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.ID;
                this.metaData.RuntimeConnectionCollection[0].Name = item.Name;
               
            }

            

            if (!string.IsNullOrWhiteSpace(txtFetchXML.Text))
            {
                designTimeInstance.SetComponentProperty("FetchXML", txtFetchXML.Text.Trim());
            }
                           
            

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close(); 
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           
        }

        private void CRMSourceAdapterUIForm_Load(object sender, EventArgs e)
        {
            var connections = connectionService.GetConnections();

            var FetchXML = metaData.CustomPropertyCollection[0];
            txtFetchXML.Text = (string)FetchXML.Value;

            string connectionManagerId = string.Empty;

            var currentConnectionManager = this.metaData.RuntimeConnectionCollection[0];
            if (currentConnectionManager != null)
            {
                connectionManagerId = currentConnectionManager.ConnectionManagerID;
            }

            for (int i = 0; i < connections.Count; i++)
            {
                var conn = connections[i].InnerObject;

                if (conn != null)
                {
                   // if (conn.CreationName =="CRMSSIS")
                    var item = new ConnectionManagerItem()
                    {
                        Name = connections[i].Name,
                      
                        ID = connections[i].ID
                    };
                    cbConnectionList.Items.Add(item);

                    if (connections[i].ID.Equals(connectionManagerId))
                    {
                        cbConnectionList.SelectedIndex = i;
                    }
                }
            }
        }

        private void btnNewConnectionManager_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList created = connectionService.CreateConnection("CRMSSIS");
            

            foreach (ConnectionManager cm in created)
            {

                
                var item = new ConnectionManagerItem()
                {
                    Name = cm.Name,
                    ID = cm.ID
                };

                cbConnectionList.Items.Insert(0, item);
            }

        }

        private void cbConnectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
