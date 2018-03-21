using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using CRMSSIS.DestinationAdapter;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CRMSSIS.DestinationAdapter
{
    public partial class CRMDestinationAdapterUIForm : Form
    {

        private Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData;

        private IDtsConnectionService connectionService;
        private CManagedComponentWrapper designTimeInstance;

         

        public CRMDestinationAdapterUIForm()
        {
            InitializeComponent();
        }
 
        #region Items for Combos
             
        public class Item
        {
            public string Value;
            public string Text;
            public AttributeMetadata[] Metadata;

            public Item(string val, string text)
            {
                Value = val;
                Text = text;
            }

            public Item(string val, string text, AttributeMetadata[] array)
            {
                Value = val;
                Text = text;
                Metadata = array;

            }

            public override string ToString()
            {
                return Text;
            }
        }
        #endregion

        public CRMDestinationAdapterUIForm(Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData, IDtsConnectionService connectionService)
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
                var item = (Item)cbConnectionList.SelectedItem;


                this.metaData.RuntimeConnectionCollection[0].Description = "Dynamics CRM Connection";
                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.Value;
                this.metaData.RuntimeConnectionCollection[0].Name = item.Text;

            }
        }

        private void CRMDestinationAdapterUIForm_Load(object sender, EventArgs e)
        {

            var connections = connectionService.GetConnections();

            
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
                    if (conn.GetType().ToString() == "CRMSSIS.CRMConnectionManager.CRMConnectionManager")
                    {
                        var item = new Item(connections[i].Name, connections[i].ID);
                       
                        cbConnectionList.Items.Add(item);

                        if (connections[i].ID.Equals(connectionManagerId))
                        {
                            cbConnectionList.SelectedIndex = i;
                        }
                    }
                }
            }

            loadOperationsCombobox();
            dgAtributeMap.Enabled = false;
            cbEntity.Enabled = false;
                                 

        }
        private void loadOperationsCombobox()
        {

            cbOperation.Items.Add(new Item("Upsert", "1"));
            cbOperation.Items.Add(new Item("Delete", "2"));
           
        }
        private void btnNewConnectionManager_Click(object sender, EventArgs e)
        {
      
            System.Collections.ArrayList created = connectionService.CreateConnection("CRMSSIS");


            foreach (ConnectionManager cm in created)
            {


                var item = new Item(cm.ID, cm.Name);
                

                cbConnectionList.Items.Insert(0, item);
            }

           

        
    }

        private void cbConnectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbConnectionList.SelectedItem != null)
            {
                var item = (Item)cbConnectionList.SelectedItem;


                this.metaData.RuntimeConnectionCollection[0].Description = "Dynamics CRM Connection";
                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.Value;
                this.metaData.RuntimeConnectionCollection[0].Name = item.Text;

                loadEntityCombobox();
            }
        }

        private void loadEntityCombobox()
        {
            try
            {
                CRMDestinationAdapter da = new CRMDestinationAdapter();

                da.AcquireConnections(null);

                RetrieveAllEntitiesRequest mdRequest = new RetrieveAllEntitiesRequest()
                {
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = false
                };
                RetrieveAllEntitiesResponse metaDataResponse = new RetrieveAllEntitiesResponse();

                RetrieveAllEntitiesResponse allentityResponse = (RetrieveAllEntitiesResponse)da.service.Execute(mdRequest);

                
                foreach (EntityMetadata Entity in allentityResponse.EntityMetadata)
                {
                    cbEntity.Items.Add(new Item(Entity.LogicalName, Entity.LogicalName, Entity.Attributes));
                }

                cbEntity.Enabled = true;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void loadMappingGrid(Item entityItem)
        {

            Item entity = (Item)entityItem;
            
 
            Mapping m = new Mapping(entity.Metadata,this.metaData.InputCollection);

            ConfigureMappingGrid(m);
            dgAtributeMap.DataSource = m;
            
             


        }
        private void ConfigureMappingGrid(Mapping m)
        {
            //TODO
        }

        private void cbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEntity.SelectedItem != null)
            {
                loadMappingGrid((Item)cbEntity.SelectedItem);
            }
            
        }
    }



}
