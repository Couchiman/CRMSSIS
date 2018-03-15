using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.SqlServer.Dts.Design;

namespace CRMSSIS.CRMConnectionManager
{
    
    public class CRMConnectionManagerUI: IDtsConnectionManagerUI
    
    {
        private ConnectionManager _connectionManager { get; set; }
        private System.IServiceProvider _serviceProvider { get; set; }

       
        void IDtsConnectionManagerUI.Initialize(ConnectionManager connectionManager, IServiceProvider serviceProvider)
        {
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        bool IDtsConnectionManagerUI.New(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            IDtsClipboardService clipboardService;

            clipboardService = (IDtsClipboardService)_serviceProvider.GetService(typeof(IDtsClipboardService));
            if (clipboardService != null)
            // If connection manager has been copied and pasted, take no action.  
            {
                if (clipboardService.IsPasteActive)
                {
                    return true;
                }
            }

            return EditCRMConnection(parentWindow, connections);
        }

        void IDtsConnectionManagerUI.Delete(IWin32Window parentWindow)
        {
            throw new NotImplementedException();
        }

        bool IDtsConnectionManagerUI.Edit(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            return EditCRMConnection(parentWindow, connections);
        }

        private bool EditCRMConnection(IWin32Window parentWindow, Connections connections)
        {

            CRMConnMgrUIForm CMUIForm = new CRMConnMgrUIForm();

           CMUIForm.Initialize(_connectionManager, _serviceProvider);
            if (CMUIForm.ShowDialog(parentWindow) == DialogResult.OK)
                return true;
            return false;


        }


    }
}
