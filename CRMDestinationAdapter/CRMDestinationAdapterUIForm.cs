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

        private class ConnectionManagerItem
        {
            public string ID;
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }

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
                var item = (ConnectionManagerItem)cbConnectionList.SelectedItem;


                this.metaData.RuntimeConnectionCollection[0].Description = "Dynamics CRM Connection";
                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.ID;
                this.metaData.RuntimeConnectionCollection[0].Name = item.Name;

            }
        }

        private void CRMDestinationAdapterUIForm_Load(object sender, EventArgs e)
        {
            loadOperationsCombobox();
            cbEntity.Enabled = false;
                                 

        }
        private void loadOperationsCombobox()
        {

            cbOperation.Items.Add(new ComboBoxItem("Upsert", "1"));
            cbOperation.Items.Add(new ComboBoxItem("Delete", "2"));
           
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
            if (cbConnectionList.SelectedIndex != -1)
            {
                loadEntityCombobox();

            }
        }

        private void loadEntityCombobox()
        {


        }
    }

    public class ComboBoxItem
    {
        public string Value;
        public string Text;

        public ComboBoxItem(string val, string text)
        {
            Value = val;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }


}
