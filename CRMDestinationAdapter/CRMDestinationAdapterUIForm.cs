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
using CRMSSIS.CRMDestinationAdapter;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace CRMSSIS.CRMDestinationAdapter
{
    public partial class CRMDestinationAdapterUIForm : Form
    {

        private Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData;

        private IDtsConnectionService connectionService;
        private CManagedComponentWrapper designTimeInstance;
        private Mapping m;
 

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

            public Item(string text, string val)
            {
                Value = val;
                Text = text;
            }

            public Item(string text, string val, AttributeMetadata[] array)
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

            if (cbOperation.SelectedItem != null)
            {
                designTimeInstance.SetComponentProperty("Operation", EnumEx.GetValueFromDescription<Operations>(cbOperation.SelectedValue.ToString()));

            }

            if (cbEntity.SelectedItem != null)
            {
                var item = (Item)cbEntity.SelectedItem;

                designTimeInstance.SetComponentProperty("Entity", item.Text);

            }

            if (!string.IsNullOrEmpty(txtBatchSize.Text))
            {
                designTimeInstance.SetComponentProperty("BatchSize", txtBatchSize.Text);

            }

            if (m.ColumnList.Count > 0)
            {
                designTimeInstance.SetComponentProperty("Mapping", m);

            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void CRMDestinationAdapterUIForm_Load(object sender, EventArgs e)
        {

            SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
            pbLoader.Dock = DockStyle.Fill;

            dgAtributeMap.Enabled = false;
            cbEntity.Enabled = false;

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
                            cbEntity.Enabled = true;


                        }
                    }
                }
            }

            
           

            m = (Mapping)this.metaData.CustomPropertyCollection["Mapping"].Value;
            loadOperationsCombobox();
            int cboValue = (int)(Operations)this.metaData.CustomPropertyCollection["Operation"].Value;

            if (cboValue > 0)
                cbOperation.SelectedIndex = cboValue;

            backgroundWorkerLoadEntities.RunWorkerAsync();
            dgAtributeMap.DataError += new DataGridViewDataErrorEventHandler(dgAtributeMap_DataError);

          
             



        }
        /// <summary>
        /// Basic operations
        /// </summary>
        private void loadOperationsCombobox()
        {

            cbOperation.DataSource = Enum.GetValues(typeof(Operations))
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
            cbOperation.DisplayMember = "Description";
            cbOperation.ValueMember = "value";



        }

        void dgAtributeMap_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // (No need to write anything in here)
        }



        /// <summary>
        /// New Button to create a new connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewConnectionManager_Click(object sender, EventArgs e)
        {

            System.Collections.ArrayList created = connectionService.CreateConnection("CRMSSIS");


            foreach (ConnectionManager cm in created)
            {


                var item = new Item(cm.Name, cm.ID);


                cbConnectionList.Items.Insert(0, item);
            }




        }
        /// <summary>
        /// sets the connection and loadEntities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbConnectionList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbConnectionList.SelectedItem != null)
            {
                var item = (Item)cbConnectionList.SelectedItem;


                this.metaData.RuntimeConnectionCollection[0].Description = "Dynamics CRM Connection";
                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.Value;
                this.metaData.RuntimeConnectionCollection[0].Name = item.Text;

               
                backgroundWorkerLoadEntities.RunWorkerAsync();
              
            }
        }

        /// <summary>
        /// Gets the current connecion
        /// </summary>
        /// <returns></returns>
        private int findConnectionId()
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

                        if (connections[i].ID.Equals(connectionManagerId))
                        {
                            return i;

                        }
                    }
                }
            }

            return -1;
        }

        private void loadEntityCombobox()
        {
            try
            {
                pbLoader.Visible = true;

                int connectionId = findConnectionId();
                //  var conn = connectionService.GetDataSource(Connection.);
                if (connectionId > -1)
                {
                    string _connectionstring = (string)connectionService.GetConnections()[connectionId].AcquireConnection(null);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    CrmServiceClient conn = new CrmServiceClient(_connectionstring);

                    // Cast the proxy client to the IOrganizationService interface.
                    IOrganizationService service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;


                    RetrieveAllEntitiesRequest mdRequest = new RetrieveAllEntitiesRequest()
                    {
                        EntityFilters = EntityFilters.Attributes,

                        RetrieveAsIfPublished = false
                    };
                    RetrieveAllEntitiesResponse metaDataResponse = new RetrieveAllEntitiesResponse();

                    RetrieveAllEntitiesResponse allentityResponse = (RetrieveAllEntitiesResponse)service.Execute(mdRequest);

                    int i = 0;
                    foreach (EntityMetadata Entity in allentityResponse.EntityMetadata)
                    {
                        cbEntity.Items.Add(new Item(Entity.LogicalName, Entity.LogicalName, Entity.Attributes));

                        if (metaData.CustomPropertyCollection["Entity"].ToString() == Entity.LogicalName)
                        {
                            cbEntity.SelectedIndex = i;
                        }
                        i++;
                    }


                    cbEntity.Enabled = true;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                
                pbLoader.Visible = false;
            }

        }

        private void loadMappingGrid(Item entityItem)
        {

            Item entity = (Item)entityItem;

            IDTSInput100 input = this.metaData.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();


            if (m == null)
                m = new Mapping(entity.Metadata, this.metaData.InputCollection[0]);


            //foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            //{
            //    //  Call the SetUsageType method of the destination  
            //    //   to add each available virtual input column as an input column.  
            //    designTimeInstance.SetUsageType(input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
            //}


            dgAtributeMap.Enabled = true;
            dgAtributeMap.AutoGenerateColumns = false;
            ConfigureMappingGrid(this.metaData.InputCollection[0]);
            dgAtributeMap.DataSource = m.ColumnList;
            




        }

        
        private void ConfigureMappingGrid(IDTSInput100 Input)
        {




            /// External Columns from Source (Virtual Columns)
            DataGridViewComboBoxColumn cmbExternalColumnName = new DataGridViewComboBoxColumn();
            cmbExternalColumnName.HeaderText = "External Column";
            cmbExternalColumnName.Name = "ExternalColumnName";

            IDTSVirtualInput100 vInput = Input.GetVirtualInput();

            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            {

                cmbExternalColumnName.Items.Add(vColumn.Name);
            }


            dgAtributeMap.Columns.Add(cmbExternalColumnName);

            DataGridViewComboBoxColumn cmbExternalColumnType = new DataGridViewComboBoxColumn();
            cmbExternalColumnType.HeaderText = "External Column Type";
            cmbExternalColumnType.Name = "ExternalColumnTypeName";
            cmbExternalColumnType.DisplayMember = "ExternalColumnTypeName";
            cmbExternalColumnType.ValueMember = "ExternalColumnTypeName";



            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
      
                cmbExternalColumnType.Items.Add(vColumn.DataType.ToString());
        
            dgAtributeMap.Columns.Add(cmbExternalColumnType);


            /// Destination Columns
            /// 
            DataGridViewComboBoxColumn cmbInternalColumnName = new DataGridViewComboBoxColumn();
            cmbInternalColumnName.HeaderText = "Internal Column";
            cmbInternalColumnName.Name = "InternalColumnName";
            cmbInternalColumnName.DisplayMember = "InternalColumnName";
            cmbInternalColumnName.ValueMember = "InternalColumnName";


            foreach (Mapping.MappingItem column in m.ColumnList)
                cmbInternalColumnName.Items.Add(column.InternalColumnName);

            dgAtributeMap.Columns.Add(cmbInternalColumnName);

            DataGridViewComboBoxColumn cmbInternalColumnType = new DataGridViewComboBoxColumn();
            cmbInternalColumnType.HeaderText = "Internal Column Type";
            cmbInternalColumnType.Name = "InternalColumnTypeName";
            cmbInternalColumnName.DisplayMember = "InternalColumnTypeName";
            cmbInternalColumnName.ValueMember = "InternalColumnTypeName";

            IEnumerable<string> filteredAttributeTypes = m.ColumnList.Select(x => x.InternalColumnTypeName).Distinct();

            foreach (string column in filteredAttributeTypes)
                cmbInternalColumnType.Items.Add(column.ToString());

            dgAtributeMap.Columns.Add(cmbInternalColumnType);

            //Default Values Column


            //TODO Determinate if and picklist load it. Lookup value,etc.
            DataGridViewTextBoxColumn defaultValues = new DataGridViewTextBoxColumn();
            defaultValues.HeaderText = "Default Value";
            defaultValues.Name = "DefaultValue";
            dgAtributeMap.Columns.Add(defaultValues);

            //Map the External with internal attribute
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Map";
            checkColumn.HeaderText = "Map";
            checkColumn.Width = 50;
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10;

            dgAtributeMap.Columns.Add(checkColumn);


            dgAtributeMap.Columns[0].DataPropertyName = "ExternalColumnName";
            dgAtributeMap.Columns[1].DataPropertyName = "ExternalColumnType";
            dgAtributeMap.Columns[2].DataPropertyName = "InternalColumnName";
            dgAtributeMap.Columns[3].DataPropertyName = "InternalColumnType";
            dgAtributeMap.Columns[4].DataPropertyName = "DefaultValue";
            dgAtributeMap.Columns[5].DataPropertyName = "Map";
        }


        private void cbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEntity.SelectedItem != null)
            {
                m = null;
                loadMappingGrid((Item)cbEntity.SelectedItem);
            }

        }


        private void SetPictureBoxFromResource(PictureBox picBox, string bmpName)
        {
            System.IO.Stream stream = this.GetType().Assembly.GetManifestResourceStream(bmpName);
            //  if the stream is found...
            if (!(stream == null))
            {
                Bitmap bmp = new Bitmap(stream);
                //  and the bitmpat is loaded...
                if (!(bmp == null))
                {
                    //  load it in the PictureBox
                    picBox.Image = bmp;
                }

            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            loadEntityCombobox();

           
        }
    }
}
