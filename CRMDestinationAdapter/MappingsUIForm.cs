using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRMSSIS.CRMCommon.Controls;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.Xrm.Sdk.Metadata;
using CRMSSIS.CRMCommon.Enumerators;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace CRMSSIS.CRMDestinationAdapter
{
    public partial class MappingsUIForm : Form
    {

        private Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData;
        private int operation;
        private IDtsConnectionService connectionService;
        private Item entityItem;
        private Mapping m;
        ComboBox cbm;
        DataGridViewCell currentCell;


        public Mapping mapping
        {
            get
            {
                return m;
            }

            set
            {
                m = value;
            }
        }

        public MappingsUIForm()
        {
            InitializeComponent();
        }
        public MappingsUIForm(Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100 metaData, IDtsConnectionService connectionService, int Operation, Item EntityItem, Mapping Mapping)
      : this()
            {
            this.metaData = metaData;
            this.connectionService = connectionService;
            this.operation = Operation;
            this.entityItem = EntityItem;
            this.mapping = Mapping;

       
        }
          


     
      

        private void MappingsUIForm_Load(object sender, EventArgs e)
        {

            dgAtributeMap.DataError += new DataGridViewDataErrorEventHandler(dgAtributeMap_DataError);

            if (entityItem !=null)
                loadMappingGrid(entityItem);
                      
            
        }

        void dgAtributeMap_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // (No need to write anything in here)
        }
        

        private void loadMappingGrid(Item entityItem)
        {

            Item entity = (Item)entityItem;

            IDTSInput100 input = this.metaData.InputCollection[0];

            

            if (m == null)
            {
                m = new Mapping(entity.Metadata, input, operation);
                dgAtributeMap.DataSource = null;
                dgAtributeMap.Rows.Clear();
                dgAtributeMap.Columns.Clear();
                dgAtributeMap.Refresh();

            }


            dgAtributeMap.Enabled = true;
            ConfigureMappingGrid(input);
            dgAtributeMap.DataSource = m.ColumnList;


            //dgAtributeMap.CellFormatting += new DataGridViewCellFormattingEventHandler(dgAttributemap_CellFormatting);
            dgAtributeMap.CellEndEdit += new DataGridViewCellEventHandler(dgAttributemap_CellEndEdit);
            dgAtributeMap.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgAtributeMap_EditingControlShowing);


        }

       
        
        private void  dgAttributemap_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (cbm != null)
            {
                // Here we will remove the subscription for selected index changed
                cbm.SelectedIndexChanged -= new EventHandler(cbm_SelectedIndexChanged);
            }

        }

        void dgAtributeMap_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Here try to add subscription for selected index changed event
            if (e.Control is ComboBox)
            {
                cbm = (ComboBox)e.Control;
                if (cbm != null)
                {
                    cbm.SelectedIndexChanged += new EventHandler(cbm_SelectedIndexChanged);
                }
                currentCell = this.dgAtributeMap.CurrentCell;
            }
        }

        void cbm_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Invoke method if the selection changed event occurs
            BeginInvoke(new MethodInvoker(EndEdit));
        }

        void EndEdit()
        {
            // Change the content of appropriate cell when selected index changes
            if (cbm != null)
            {
                

                if (cbm.SelectedItem != null)
                {
                     if(currentCell.ColumnIndex == 0)
                    {
                        bool isPresent = false;
                        foreach (IDTSInputColumn100 column in this.metaData.InputCollection[0].InputColumnCollection)
                        {
                            if (column.Name== cbm.SelectedItem.ToString()) { 
                                this.dgAtributeMap[currentCell.ColumnIndex + 1, currentCell.RowIndex].Value = column.DataType.ToString();
                                isPresent = true;
                            }
                        }
                        if (!isPresent) this.dgAtributeMap[currentCell.ColumnIndex + 1, currentCell.RowIndex].Value = "";

                    }
                   

                    if (currentCell.ColumnIndex == 2)
                        this.dgAtributeMap[currentCell.ColumnIndex + 1, currentCell.RowIndex].Value = m.ColumnList[currentCell.RowIndex + 1].InternalColumnTypeName;
                                      

                    this.dgAtributeMap.EndEdit();
                }
            }
        }
        private void ConfigureMappingGrid(IDTSInput100 Input)
        {


            dgAtributeMap.AutoGenerateColumns = false;

            dgAtributeMap.RowHeadersWidth = 20;

            /// External Columns from Source (Input Columns)
            DataGridViewComboBoxColumn cmbExternalColumnName = new DataGridViewComboBoxColumn();
            cmbExternalColumnName.HeaderText = "External Column";
            cmbExternalColumnName.Name = "ExternalColumnName";
            cmbExternalColumnName.DisplayMember = "ExternalColumnName";
            cmbExternalColumnName.ValueMember = "ExternalColumnName";
            cmbExternalColumnName.Width = 120;

            foreach (IDTSInputColumn100 column in this.metaData.InputCollection[0].InputColumnCollection)
                cmbExternalColumnName.Items.Add(column.Name.ToString());

            cmbExternalColumnName.Items.Add("");
            dgAtributeMap.Columns.Add(cmbExternalColumnName);


            DataGridViewTextBoxColumn cmbExternalColumnTypeName = new DataGridViewTextBoxColumn();
            cmbExternalColumnTypeName.HeaderText = "Column Type";
            cmbExternalColumnTypeName.Name = "ExternalColumnTypeName";          
            cmbExternalColumnTypeName.Width = 75;
            cmbExternalColumnTypeName.ReadOnly = true;
            

            dgAtributeMap.Columns.Add(cmbExternalColumnTypeName);


            /// Destination Columns
            /// 
            DataGridViewComboBoxColumn cmbInternalColumnName = new DataGridViewComboBoxColumn();
            cmbInternalColumnName.HeaderText = "Internal Column";
            cmbInternalColumnName.Name = "InternalColumnName";
            cmbInternalColumnName.DisplayMember = "InternalColumnName";
            cmbInternalColumnName.ValueMember = "InternalColumnName";
            cmbInternalColumnName.Width = 120;

            foreach (Mapping.MappingItem column in m.ColumnList)
                cmbInternalColumnName.Items.Add(column.InternalColumnName);

            dgAtributeMap.Columns.Add(cmbInternalColumnName);

            DataGridViewTextBoxColumn cmbInternalColumnTypeName = new DataGridViewTextBoxColumn();
            cmbInternalColumnTypeName.HeaderText = "Column Type";
            cmbInternalColumnTypeName.Name = "InternalColumnTypeName";
            cmbInternalColumnTypeName.Width = 75;
            cmbInternalColumnTypeName.ReadOnly = true;

            dgAtributeMap.Columns.Add(cmbInternalColumnTypeName);


            //Default Values Column


            //TODO Determinate if and picklist load it. Lookup value,etc.
            DataGridViewTextBoxColumn defaultValues = new DataGridViewTextBoxColumn();
            defaultValues.HeaderText = "Default Value";
            defaultValues.Name = "DefaultValue";
            dgAtributeMap.Columns.Add(defaultValues);


            DataGridViewCheckBoxColumn isRequired = new DataGridViewCheckBoxColumn();
            isRequired.HeaderText = "isRequired";
            isRequired.Width = 55;
            isRequired.ReadOnly = true;
            dgAtributeMap.Columns.Add(isRequired);

            DataGridViewCheckBoxColumn isPrimary = new DataGridViewCheckBoxColumn();
            isPrimary.HeaderText = "isPrimary";
            isPrimary.Width = 55;
            isPrimary.ReadOnly = true;
            dgAtributeMap.Columns.Add(isPrimary);


            DataGridViewComboBoxColumn TargetEntity = new DataGridViewComboBoxColumn();
            TargetEntity.HeaderText = "TargetEntity";
            TargetEntity.Name = "TargetEntity";
            TargetEntity.DisplayMember = "TargetEntity";
            TargetEntity.ValueMember = "TargetEntity";
            dgAtributeMap.Columns.Add(TargetEntity);

            //Map the External with internal attribute
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Map";
            checkColumn.HeaderText = "Map";
            checkColumn.Width = 50;
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10;

            dgAtributeMap.Columns.Add(checkColumn);



            dgAtributeMap.Columns[0].DataPropertyName = "ExternalColumnName";
            dgAtributeMap.Columns[1].DataPropertyName = "ExternalColumnTypeName";
            dgAtributeMap.Columns[2].DataPropertyName = "InternalColumnName";
            dgAtributeMap.Columns[3].DataPropertyName = "InternalColumnTypeName";

            dgAtributeMap.Columns[4].DataPropertyName = "DefaultValue";
            dgAtributeMap.Columns[5].DataPropertyName = "isRequired";
            dgAtributeMap.Columns[6].DataPropertyName = "isPrimary";
            dgAtributeMap.Columns[7].DataPropertyName = "TargetEntity";
            dgAtributeMap.Columns[8].DataPropertyName = "Map";

            dgAtributeMap.DataBindingComplete += dgAtributeMap_DataBindingComplete;
        }

        private void dgAtributeMap_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string colName = "TargetEntity";


            Item Entity = (Item)CRMSSIS.CRMCommon.JSONSerialization.Deserialize<Item>(this.metaData.CustomPropertyCollection["Entity"].Value.ToString());

            foreach (DataGridViewRow row in dgAtributeMap.Rows)
            {
                var mappingitem = (Mapping.MappingItem)row.DataBoundItem;
                var cell = row.Cells[colName] as DataGridViewComboBoxCell;


                foreach (AttributeMetadata attribute in Entity.Metadata)
                {
                    if (mappingitem.InternalColumnName == attribute.LogicalName)
                    {
                        switch (attribute.AttributeType.Value)
                        {
                            case AttributeTypeCode.Lookup:
                            case AttributeTypeCode.Customer:
                            case AttributeTypeCode.PartyList:
                            case AttributeTypeCode.Owner:

                                cell.Items.AddRange(((Microsoft.Xrm.Sdk.Metadata.LookupAttributeMetadata)attribute).Targets);
                                if (mappingitem.TargetEntity != string.Empty)
                                    cell.Value = mappingitem.TargetEntity;

                                row.DefaultCellStyle.BackColor = Color.Aquamarine;
                                break;
                            case AttributeTypeCode.Uniqueidentifier:
                                row.DefaultCellStyle.BackColor = Color.YellowGreen;
                                break;
                        }
                    }
                }

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
