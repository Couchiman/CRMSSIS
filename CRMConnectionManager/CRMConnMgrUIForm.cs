using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 

namespace CRMSSIS.CRMConnectionManager
{
    public partial class CRMConnMgrUIForm : Form
    {
        public CRMConnMgrUIForm()
        {
            InitializeComponent();

          
        }

        private ConnectionManager _connectionManager { get; set; }
        private System.IServiceProvider _serviceProvider { get; set; }

        public void Initialize (ConnectionManager connectionManager, IServiceProvider serviceProvider) 
        {

            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
            LoadAuthCombo(cboAuthType);
            ConfigureControlsFromConnectionManager();
            

        }

  

        private void ConfigureControlsFromConnectionManager()
        {
 

            {
               
                

                txtOrganizationUri.Text = _connectionManager.Properties["OrganizationUri"].GetValue(_connectionManager).ToString();
                txtHomeRealUri.Text = _connectionManager.Properties["HomeRealmUri"].GetValue(_connectionManager).ToString();
                txtDomain.Text = _connectionManager.Properties["Domain"].GetValue(_connectionManager).ToString();
                txtPassword.Text = _connectionManager.Properties["Password"].GetValue(_connectionManager).ToString();
                txtUserName.Text = _connectionManager.Properties["UserName"].GetValue(_connectionManager).ToString();
                int cboValue = (int)(AuthenticationProviderTypeDescriptive)_connectionManager.Properties["AuthType"].GetValue(_connectionManager);
                
                cboAuthType.SelectedIndex = cboValue;
                 
            }

        }



        private void ConfigureConnectionManagerFromControls()
        {

            {
                _connectionManager.Properties["OrganizationUri"].SetValue(_connectionManager, txtOrganizationUri.Text);
                _connectionManager.Properties["HomeRealmUri"].SetValue(_connectionManager, txtHomeRealUri.Text);
                _connectionManager.Properties["Domain"].SetValue(_connectionManager, txtDomain.Text);
                _connectionManager.Properties["UserName"].SetValue(_connectionManager, txtUserName.Text);
                _connectionManager.Properties["Password"].SetValue(_connectionManager, txtPassword.Text);
                _connectionManager.Properties["AuthType"].SetValue(_connectionManager, cboAuthType.SelectedValue);
                

            }

        }


        public static void LoadAuthCombo(ComboBox cbo)
        {
            cbo.DataSource = Enum.GetValues(typeof(AuthenticationProviderTypeDescriptive))
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
            cbo.DisplayMember = "Description";
            cbo.ValueMember = "value";

           
             
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ConfigureConnectionManagerFromControls();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cboAuthType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
