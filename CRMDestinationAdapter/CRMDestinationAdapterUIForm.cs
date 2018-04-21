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
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using CRMSSIS.CRMCommon.Enumerators;
using CRMSSIS.CRMCommon.Controls;
using CRMSSIS.CRMCommon;
using System.Collections;
using System.Globalization;

namespace CRMSSIS.CRMDestinationAdapter
{
    public partial class CRMDestinationAdapterUIForm : Form
    {

        private Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData;

        private IDtsConnectionService connectionService;
        private CManagedComponentWrapper designTimeInstance;
        private Mapping m;
        CheckBox lastChecked;

        public CRMDestinationAdapterUIForm()
        {
            InitializeComponent();
        }


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
                Item item = (Item)cbEntity.SelectedItem;
                //First Serialize the object because complex objects are not supported by SSIS
          
                designTimeInstance.SetComponentProperty("Entity", CRMSSIS.CRMCommon.JSONSerialization.Serialize<Item>(item));
              
            }

            if (!string.IsNullOrEmpty(txtBatchSize.Text))
            {
                designTimeInstance.SetComponentProperty("BatchSize", txtBatchSize.Text);

            }

            if (m !=null)
            {
                designTimeInstance.SetComponentProperty("Mapping", CRMCommon.JSONSerialization.Serialize<Mapping>(m));

            }
            
            if (cboLocales.SelectedItem != null)
            {
                designTimeInstance.SetComponentProperty("CultureInfo", EnumEx.GetValueFromDescription<SupportedLanguages>(cboLocales.SelectedValue.ToString()));
            }

            if (chkErrorFail.Checked) this.metaData.InputCollection[0].ErrorRowDisposition = DTSRowDisposition.RD_FailComponent;
            if (chkIgnoreError.Checked) this.metaData.InputCollection[0].ErrorRowDisposition = DTSRowDisposition.RD_IgnoreFailure;
            if (chkRedirect.Checked) this.metaData.InputCollection[0].ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;


            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void CRMDestinationAdapterUIForm_Load(object sender, EventArgs e)
        {



         
            btnMappings.Enabled = false;


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



            loadCultureInfo();
                      

            loadOperationsCombobox();

            

           

            txtBatchSize.Text = Convert.ToString(this.metaData.CustomPropertyCollection["BatchSize"].Value);

            if (this.metaData.CustomPropertyCollection["Entity"].Value != null)
            {

                Item Entity = (Item)CRMSSIS.CRMCommon.JSONSerialization.Deserialize<Item>(this.metaData.CustomPropertyCollection["Entity"].Value.ToString());

                cbEntity.Items.Add(Entity);
                cbEntity.SelectedIndex = 0;

             
                this.cbEntity.SelectedIndexChanged += new System.EventHandler(this.cbEntity_SelectedIndexChanged);
                if (this.metaData.CustomPropertyCollection["Mapping"].Value !=null)
                m = CRMCommon.JSONSerialization.Deserialize<Mapping>(this.metaData.CustomPropertyCollection["Mapping"].Value.ToString());

                btnMappings.Enabled = true;
                
               
            }
            else
            {
                if (cbConnectionList.SelectedItem != null)
                {
                    SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
                    pbLoader.Dock = DockStyle.Fill;

                    backgroundWorkerLoadEntities.RunWorkerAsync();
                  
                }
                

            }
           
         
            cbConnectionList.SelectedIndexChanged += new System.EventHandler(this.cbConnectionList_SelectedIndexChanged);
            cbOperation.SelectedIndexChanged += new System.EventHandler(this.cbOperation_SelectedIndexChanged);
            chkErrorFail.CheckedChanged += new System.EventHandler(this.chk_Click);
            chkIgnoreError.CheckedChanged += new System.EventHandler(this.chk_Click);
            chkRedirect.CheckedChanged += new System.EventHandler(this.chk_Click);

            loadErrorHandlingCheckboxes();

        }

        private void loadErrorHandlingCheckboxes()
        {

             
            switch (this.metaData.InputCollection[0].ErrorRowDisposition)

             {
                case DTSRowDisposition.RD_FailComponent:
                    chkIgnoreError.Checked = false;
                    chkRedirect.Checked = false;
                    chkErrorFail.Checked = true;
                    break;
                case DTSRowDisposition.RD_RedirectRow:
                    chkIgnoreError.Checked = false;
                    chkRedirect.Checked = true;
                    chkErrorFail.Checked = false;
                    break;
                case DTSRowDisposition.RD_IgnoreFailure:
                    chkIgnoreError.Checked = true;
                    chkRedirect.Checked = false;
                    chkErrorFail.Checked = false;
                    break;

             }
        }

        private void chk_Click(object sender, EventArgs e)
        {
            CheckBox activeCheckBox = sender as CheckBox;
            if (activeCheckBox != lastChecked && lastChecked != null) lastChecked.Checked = false;
            lastChecked = activeCheckBox.Checked ? activeCheckBox : null;
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

            int cboValue = (int)(Operations)this.metaData.CustomPropertyCollection["Operation"].Value;

            if (cboValue >= 0)
                cbOperation.SelectedIndex = cboValue;

        }


        /// <summary>
        /// Loads Culture types to enforce integration culture format for data.
        /// </summary>
        private void loadCultureInfo()
        {

            cboLocales.DataSource = Enum.GetValues(typeof(SupportedLanguages))
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
            cboLocales.DisplayMember = "Description";
            cboLocales.ValueMember = "value";

            int cboValue = (int)(SupportedLanguages)this.metaData.CustomPropertyCollection["CultureInfo"].Value;


            cboLocales.SelectedIndex= cboLocales.FindString(CRMSSIS.CRMCommon.Enumerators.EnumEx.GetDescriptionFromValue<SupportedLanguages>((SupportedLanguages)cboValue));
          


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

                SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
                pbLoader.Dock = DockStyle.Fill;

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
                
                if (connectionId > -1)
                {
                    string _connectionstring = (string)connectionService.GetConnections()[connectionId].AcquireConnection(null);

                    IOrganizationService service = CRMCommon.CRM.Connect(_connectionstring);

                    RetrieveAllEntitiesResponse allentityResponse  = CRMCommon.CRM.RetrieveAllEntityMetatada(service);
                    
                    foreach (EntityMetadata Entity in allentityResponse.EntityMetadata)
                    {
                        cbEntity.Items.Add(new Item(Entity.LogicalName, Entity.LogicalName, Entity.Attributes));
                       
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

       

        /// <summary>
        /// Captures the Entity change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEntity.SelectedItem != null)
            {
            
                m = null;
               
            }

        }

        /// <summary>
        /// Sets the image for loading operation
        /// </summary>
        /// <param name="picBox"></param>
        /// <param name="bmpName"></param>
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

        /// <summary>
        /// Loads all entities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            loadEntityCombobox();

           
        }
        /// <summary>
        /// Refresh entities button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if(cbConnectionList.SelectedItem !=null)
            { 
                SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
                pbLoader.Dock = DockStyle.Fill;

                backgroundWorkerLoadEntities.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Refresh Metatada button, corrects inputs and outputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshMetadata_Click(object sender, EventArgs e)
        {
            //Forces meadata refresh
            for (int i = 0; i < this.metaData.InputCollection.Count; i++)
            {
                this.metaData.InputCollection[i].InputColumnCollection.RemoveAll();
                IDTSVirtualInput100 input = this.metaData.InputCollection[i].GetVirtualInput();
                foreach (IDTSVirtualInputColumn100 vcol in input.VirtualInputColumnCollection)
                {
                    input.SetUsageType(vcol.LineageID, DTSUsageType.UT_READONLY);
                }
            }



            if (this.metaData.CustomPropertyCollection["Entity"].Value != null)
                {

                    Item entity = (Item)CRMSSIS.CRMCommon.JSONSerialization.Deserialize<Item>(this.metaData.CustomPropertyCollection["Entity"].Value.ToString());


                    IDTSInput100 input = this.metaData.InputCollection[0];

                    int connectionId = findConnectionId();
                  
                    if (connectionId > -1)
                    {
                        try
                        {
                            string _connectionstring = (string)connectionService.GetConnections()[connectionId].AcquireConnection(null);

                            IOrganizationService service = CRMSSIS.CRMCommon.CRM.Connect(_connectionstring);

                            RetrieveEntityResponse retrieveEntityResponse = CRMSSIS.CRMCommon.CRM.RetrieveEntityRequestMetadata(service, entity.Text);

                            this.metaData.CustomPropertyCollection["Entity"].Value = CRMSSIS.CRMCommon.JSONSerialization.Serialize<Item>(new Item(entity.Text, entity.Text, retrieveEntityResponse.EntityMetadata.Attributes));
                            int operation = (int)EnumEx.GetValueFromDescription<Operations>(cbOperation.SelectedValue.ToString());

                            m.RefreshMapping(input, entity.Metadata, retrieveEntityResponse.EntityMetadata.Attributes, operation);

                        ////Refresh Grid
                        //dgAtributeMap.DataSource = null;
                        //dgAtributeMap.Rows.Clear();
                        //dgAtributeMap.Refresh();

                        //ConfigureMappingGrid(input);
                        //dgAtributeMap.DataSource = m.ColumnList;

                    }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                 
                    }



                }
                else
                {
                    MessageBox.Show("Entity must be set", "Warning", MessageBoxButtons.OK);
                }

            
        }

        private void cbOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOperation.SelectedItem != null && cbEntity.SelectedItem !=null)
            {

                m = null;
                //loadMappingGrid((Item)cbEntity.SelectedItem);
            }
        }

        private void btnMappings_Click(object sender, EventArgs e)
        {

            int operation = (int)EnumEx.GetValueFromDescription<Operations>(cbOperation.SelectedValue.ToString());


            MappingsUIForm Mappings = new MappingsUIForm(metaData,connectionService, operation, (Item)cbEntity.SelectedItem, m);
            if (Mappings.ShowDialog() == DialogResult.OK)
                m = Mappings.mapping;

        }
    }
}
