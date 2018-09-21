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

            if (cboWorkflows.SelectedItem != null)
            {
                Item item = (Item)cboWorkflows.SelectedItem;
                designTimeInstance.SetComponentProperty("Workflow", CRMSSIS.CRMCommon.JSONSerialization.Serialize<Item>(item));

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
            cboWorkflows.Enabled = false;
            btnRefresh.Enabled = false;
            btnRefreshWorkflows.Enabled = false;
            btnRefreshMetadata.Enabled = false;

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
            // User already selected an Entity 
            if (this.metaData.CustomPropertyCollection["Entity"].Value != null)
            {
                btnRefresh.Enabled = true;
                btnRefreshMetadata.Enabled = true;
                btnMappings.Enabled = true;

                Item Entity = (Item)CRMSSIS.CRMCommon.JSONSerialization.Deserialize<Item>(this.metaData.CustomPropertyCollection["Entity"].Value.ToString());

                cbEntity.Items.Add(Entity);
                cbEntity.SelectedIndex = 0;
                cbEntity.Enabled = false;
                
                this.cbEntity.SelectedIndexChanged += new System.EventHandler(this.cbEntity_SelectedIndexChanged);
                if (this.metaData.CustomPropertyCollection["Mapping"].Value !=null)
                m = CRMCommon.JSONSerialization.Deserialize<Mapping>(this.metaData.CustomPropertyCollection["Mapping"].Value.ToString());

               
                // Check if the operation is a workflow, if not lets user add mapping
                if ((Operations)this.metaData.CustomPropertyCollection["Operation"].Value == Operations.Workflow)
                {
                    
                    btnRefreshWorkflows.Enabled = true;

                    if (this.metaData.CustomPropertyCollection["Workflow"].Value.ToString() == "")
                    {
                        backgroundWorkerLoadWorkflows.RunWorkerAsync();
                        cboWorkflows.Enabled = true;
                    }
                    else if (this.metaData.CustomPropertyCollection["Workflow"].Value.ToString() != "")
                    {
                        Item Workflow = (Item)CRMSSIS.CRMCommon.JSONSerialization.Deserialize<Item>(this.metaData.CustomPropertyCollection["Workflow"].Value.ToString());

                        cboWorkflows.Items.Add(Workflow);
                        cboWorkflows.SelectedIndex = 0;

                    }
                }
                


            }
            else
            {
                if (cbConnectionList.SelectedItem != null)
                {
                    SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
                    pbLoader.Dock = DockStyle.Fill;
                    btnRefresh.Enabled = true;
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

        /// <summary>
        /// Load workflow list for an entity
        /// </summary>
        /// <param name="entityName"></param>
        private void loadWorkflows(string entityName)
        {

            try
            {
                List<Item> wfList = new List<Item>();
                pbLoader.Visible = true;

                int connectionId = findConnectionId();

                if (connectionId > -1)
                {
                    string _connectionstring = (string)connectionService.GetConnections()[connectionId].AcquireConnection(null);

                    IOrganizationService service = CRMCommon.CRM.Connect(_connectionstring);

                    EntityCollection enWorkflows = CRMCommon.CRM.GetWorkflowList(service, entityName);

                    foreach (Entity workflow in enWorkflows.Entities)
                    {
                        wfList.Add(new Item(workflow.Attributes["name"].ToString(), workflow.Attributes["workflowid"].ToString()));

                    }

                    cbOperation.DisplayMember = "Text";
                    cbOperation.ValueMember = "Value";
                    cboWorkflows.DataSource = wfList;
                    cboWorkflows.Enabled = true;

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

            int cboValue = Convert.ToInt32(this.metaData.CustomPropertyCollection["Operation"].Value.ToString());

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

            int cboValue = Convert.ToInt32(this.metaData.CustomPropertyCollection["CultureInfo"].Value.ToString()); 


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

        /// <summary>
        /// Loads Entities
        /// </summary>
        private void loadEntityCombobox()
        {
            try
            {
                pbLoader.Visible = true;

                int connectionId = findConnectionId();
                List<Item> eList = new List<Item>();

                if (connectionId > -1)
                {
                    string _connectionstring = (string)connectionService.GetConnections()[connectionId].AcquireConnection(null);

                    IOrganizationService service = CRMCommon.CRM.Connect(_connectionstring);

                    RetrieveAllEntitiesResponse allentityResponse  = CRMCommon.CRM.RetrieveAllEntityMetatada(service);
                    
                    foreach (EntityMetadata Entity in allentityResponse.EntityMetadata)
                    {
                        eList.Add(new Item(Entity.LogicalName, Entity.PrimaryIdAttribute, Entity.Attributes));
                       
                    }
                    cbEntity.DisplayMember = "Text";
                    cbEntity.ValueMember = "Value";
                    cbEntity.DataSource = eList;

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
                this.metaData.CustomPropertyCollection["Workflow"].Value = "";


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
        /// Refresh workflow list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshWorkflows_Click(object sender, EventArgs e)
        {
            if(cbEntity.SelectedItem !=null)
            { 
                if (cbOperation.SelectedItem != null && (Operations)cbOperation.SelectedValue == Operations.Workflow)
                {
                    SetPictureBoxFromResource(pbLoader, "CRMSSIS.CRMDestinationAdapter.loading.gif");
                    pbLoader.Dock = DockStyle.Fill;

                    backgroundWorkerLoadWorkflows.RunWorkerAsync();
                }
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

                if ((Operations)cbOperation.SelectedValue == Operations.Workflow)
                {
                    btnRefreshWorkflows.Enabled = true;
                    btnRefresh.Enabled = true;
                }
                else
                {
                   
                    cboWorkflows.Items.Clear();
                    cboWorkflows.ResetText();
                    btnRefresh.Enabled = true;

                }
            }
        }

        private void btnMappings_Click(object sender, EventArgs e)
        {

            int operation = (int)EnumEx.GetValueFromDescription<Operations>(cbOperation.SelectedValue.ToString());


            MappingsUIForm Mappings = new MappingsUIForm(metaData,connectionService, operation, (Item)cbEntity.SelectedItem, m);
            if (Mappings.ShowDialog() == DialogResult.OK)
                m = Mappings.mapping;

        }

        private void backgroundWorkerLoadWorkflows_DoWork(object sender, DoWorkEventArgs e)
        {
            loadWorkflows(cbEntity.SelectedItem.ToString());
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            CRMCommon.AboutBox box = new CRMCommon.AboutBox();
            box.Show();
        }


        // NOT SUPPORTED
        //private void btnExpressions_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Create an expression builder popup and make sure the expressions can be evaluated as a string:
        //        // Or change it if you want boolean to System.Boolean, etc. Last property is the textbox containing
        //        // the expression that you want to edit.
        //        using (var expressionBuilder = ExpressionBuilder.Instantiate(_taskHost.Variables,
        //                       _taskHost.VariableDispenser,
        //                       Type.GetType("System.String"),
        //                       txtExpression.Text))
        //        {
        //            // Open the window / dialog with expression builder
        //            if (expressionBuilder.ShowDialog() == DialogResult.OK)
        //            {
        //                // If pressed OK then get the created expression
        //                // and put it in a textbox.
        //                txtExpression.Text = expressionBuilder.Expression;
        //                lblExpressionEvaluated.Text = "";

        //                // Create object to evaluate the expression
        //                Wrapper.ExpressionEvaluator evalutor = new Wrapper.ExpressionEvaluator();

        //                // Add the expression
        //                evalutor.Expression = txtExpression.Text;

        //                // Object for storing the evaluated expression
        //                object result = null;

        //                try
        //                {
        //                    // Evalute the expression and store it in the result object
        //                    evalutor.Evaluate(DtsConvert.GetExtendedInterface(_taskHost.VariableDispenser), out result, false);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Store error message in label
        //                    // Perhaps a little useless in this example because the expression builder window
        //                    // already validated the expression. But you could also make the textbox readable
        //                    // and change the expression there (without opening the expression builder window)
        //                    lblExpressionEvaluated.Text = ex.Message;
        //                }

        //                // If the Expression contains some error, the "result" will be <null>.
        //                if (result != null)
        //                {
        //                    // Add evaluated expression to label
        //                    lblExpressionEvaluated.Text = result.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    }
}
