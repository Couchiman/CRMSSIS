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
using CRMSSIS.CRMCommon.Enumerators;
 

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

                txtCallerId.Text = _connectionManager.Properties["CallerId"].GetValue(_connectionManager).ToString();
                txtPort.Text = _connectionManager.Properties["Port"].GetValue(_connectionManager).ToString();

                bool cbhttpValue = (bool)_connectionManager.Properties["UsesSSL"].GetValue(_connectionManager);
                                
                chkSSL.Checked = cbhttpValue;

                txtTimeout.Text = _connectionManager.Properties["TimeOut"].GetValue(_connectionManager).ToString();

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

                _connectionManager.Properties["CallerId"].SetValue(_connectionManager, txtCallerId.Text);
                _connectionManager.Properties["Port"].SetValue(_connectionManager, txtPort.Text);
                _connectionManager.Properties["UsesSSL"].SetValue(_connectionManager, chkSSL.Checked);
                if (!string.IsNullOrEmpty(txtTimeout.Text))
                {
                    int sTimeout;
                    int.TryParse(txtTimeout.Text, out sTimeout);
                _connectionManager.Properties["TimeOut"].SetValue(_connectionManager, sTimeout);
                }
                else
                    _connectionManager.Properties["TimeOut"].SetValue(_connectionManager, 0);


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
