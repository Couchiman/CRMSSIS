using System;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using CRMSSIS.CRMCommon.Enumerators;
using CRMSSIS.CRMCommon.Controls;

namespace CRMSSIS.CRMConnectionManager
{
    [DtsConnection(ConnectionType = "CRMSSIS",
      DisplayName = "Dynamics CRM Connection Manager",
      Description = "Connection Manager for CRM Dynamics 365",
      ConnectionContact = "Pablo J. Cósimo",
      IconResource = "CRMSSIS.CRMConnectionManager.Icon1.ico",
      UITypeName = "CRMSSIS.CRMConnectionManager.CRMConnectionManagerUI, CRMSSIS.CRMConnectionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1445bdffd791cede")]
    public class CRMConnectionManager : ConnectionManagerBase, IDTSComponentPersist
    {

        #region properties
        public bool UsesSSL { get; set; }
        public string Port { get; set; }

        public string CallerId { get; set; }
         public int TimeOut { get; set; }
        public string OrganizationUri { get; set; }
        public string HomeRealmUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        private AuthenticationProviderTypeDescriptive _AuthType = AuthenticationProviderTypeDescriptive.Office365;
        public AuthenticationProviderTypeDescriptive AuthType { get { return _AuthType; } set { _AuthType = value; } }

        private string _connectionString;

        
        public override string ConnectionString
        {
            get
            {
                UpdateConnectionString();
                return _connectionString;
            }
            set
            {
                base.ConnectionString = value;
                _connectionString = value;
            }
        }

        #endregion

        public CRMConnectionManager()
        {
            OrganizationUri = "";
            HomeRealmUri = "";
            UserName = "";
            Password = "";
            Domain = "";
            AuthType = AuthenticationProviderTypeDescriptive.Office365;
            CallerId = "";
            UsesSSL = true;
            Port = "443";
            TimeOut =0;

        }
        /// <summary>
        /// Updates the connection string when user changes it.
        /// </summary>
        private void UpdateConnectionString()
        {


            UriBuilder uriBuilder = new UriBuilder();
            if (UsesSSL)
            uriBuilder.Scheme = "https://";
            else
                uriBuilder.Scheme = "http://";
            uriBuilder.Host = OrganizationUri;
            if (!string.IsNullOrEmpty(Port))
            {
                int sPort;
                int.TryParse(Port, out sPort);
                uriBuilder.Port = sPort;

            }


            string temporaryString;
            

            switch (AuthType)
            {
                case AuthenticationProviderTypeDescriptive.ActiveDirectory:
                    

                    temporaryString = "AuthType=AD;Url=" + uriBuilder.Uri.ToString()+ ";Domain=" + Domain.ToString() + ";Username=" + UserName.ToString() +"; Password=" + Password.ToString() +";";
                    break;


                case AuthenticationProviderTypeDescriptive.IFD:
                    temporaryString = "AuthType=IFD;Url =" + uriBuilder.Uri.ToString() + ";HomeRealmUri =" + HomeRealmUri.ToString() + ";Domain=" + Domain.ToString() + "; Username=" + UserName.ToString() +";Password=" + Password.ToString() +";";
                    break;


                case AuthenticationProviderTypeDescriptive.Office365:
                    // return AuthenticationProviderType.OnlineFederation;
                    temporaryString = "AuthType=Office365;Url =" + uriBuilder.Uri.ToString() + ";Username=" + UserName.ToString() + ";Password=" + Password.ToString() +";";
                    break;

                default:
                    throw new ArgumentException(String.Format("{0} is not a valid authentication type", AuthType.ToString()));
            }

            if (TimeOut > 0)
            {
                TimeSpan t = new TimeSpan(0, 0, TimeOut, 0, 0);
                temporaryString = temporaryString + string.Format("Timeout={0};", t.ToString(@"hh\:mm\:ss"));
            }
            

            if (!string.IsNullOrEmpty(CallerId))
            {
                temporaryString = temporaryString + string.Format("ClientId={0};", CallerId);
            }

            _connectionString = temporaryString.Trim();
        }

        /// <summary>
        /// Validates required properties. 
        /// </summary>
        /// <param name="infoEvents"></param>
        /// <returns></returns>
        public override Microsoft.SqlServer.Dts.Runtime.DTSExecResult Validate(Microsoft.SqlServer.Dts.Runtime.IDTSInfoEvents infoEvents)
        {
         

            if (String.IsNullOrEmpty(_connectionString))
            {
                infoEvents.FireError(0, "CRM Connection Manager", "No connection specified", String.Empty, 0);
                return DTSExecResult.Failure;
            }
            else
            {
                return DTSExecResult.Success;
            }

        }
        /// <summary>
        /// Gets connection string to be used in Destination and Source adapters.
        /// </summary>
        /// <param name="txn"></param>
        /// <returns></returns>
        public override object AcquireConnection(object txn)
        {

            try
            {
                  
                return _connectionString;
                
            }

            catch (Exception ex)
            {//IMPROVE
                throw new Exception("Cannot connect to Dynamics Instance " + ex.Message);
            }

        }

        /// <summary>
        /// Liberates connection object
        /// </summary>
        /// <param name="connection"></param>
        public override void ReleaseConnection(object connection)
        {
            base.ReleaseConnection(connection);
            connection = null;
        }

        /// <summary>
        /// Persists Connection String information
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="infoEvents"></param>
        void IDTSComponentPersist.SaveToXML(XmlDocument doc, IDTSInfoEvents infoEvents)
        {
            // Create a root node for the data
            XmlElement rootElement = doc.CreateElement(String.Empty, "CRMConnectionManager", String.Empty);
            
         
            doc.AppendChild(rootElement);

            XmlNode node = doc.CreateNode(XmlNodeType.Element, "AuthType", String.Empty);
            XmlElement XElement = node as XmlElement;
            XElement.InnerText = (Attribute.GetCustomAttribute(AuthType.GetType().GetField(AuthType.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
            rootElement.AppendChild(node);


            if (!String.IsNullOrEmpty(CallerId))
            {
                XmlNode node3 = doc.CreateNode(XmlNodeType.Element, "CallerId", String.Empty);
                XmlElement XElementCallerId = node3 as XmlElement;
                XElementCallerId.InnerText = CallerId;
                rootElement.AppendChild(node3);
            }


            if (TimeOut >0)
            {
                XmlNode node3 = doc.CreateNode(XmlNodeType.Element, "TimeOut", String.Empty);
                XmlElement XElementTimeOut = node3 as XmlElement;
                XElementTimeOut.InnerText = TimeOut.ToString();
                rootElement.AppendChild(node3);
            }

            if (!string.IsNullOrEmpty(Port))
            {
                XmlNode nodeport = doc.CreateNode(XmlNodeType.Element, "Port", String.Empty);
                XmlElement XElementPort = nodeport as XmlElement;
                XElementPort.InnerText = Port.ToString();
                rootElement.AppendChild(nodeport);
            }

            
                XmlNode nodessl = doc.CreateNode(XmlNodeType.Element, "UsesSSL", String.Empty);
                XmlElement XElementSSL = nodessl as XmlElement;
                XElementSSL.InnerText = UsesSSL.ToString();
                rootElement.AppendChild(nodessl);
            

            if (!String.IsNullOrEmpty(Domain))
            {
                XmlNode node2 = doc.CreateNode(XmlNodeType.Element, "Domain", String.Empty);
                XmlElement XElementDomain = node2 as XmlElement;
                XElementDomain.InnerText = Domain;
                rootElement.AppendChild(node2);
            }

            if (!String.IsNullOrEmpty(OrganizationUri))
            {
                XmlNode node3 = doc.CreateNode(XmlNodeType.Element, "OrganizationUri", String.Empty);
                XmlElement XElementOrganizationUri = node3 as XmlElement;
                XElementOrganizationUri.InnerText = OrganizationUri;
                rootElement.AppendChild(node3);
            }

            if (!String.IsNullOrEmpty(HomeRealmUri))
            {
                XmlNode node4 = doc.CreateNode(XmlNodeType.Element, "HomeRealmUri", String.Empty);
                XmlElement XmlElementHomeRealmUri = node4 as XmlElement;
                XmlElementHomeRealmUri.InnerText = HomeRealmUri;
                rootElement.AppendChild(node4);
            }



            if (!String.IsNullOrEmpty(UserName))
            {
                XmlNode node5 = doc.CreateNode(XmlNodeType.Element, "UserName", String.Empty);
                XmlElement userName = node5 as XmlElement;
                userName.InnerText = UserName;
                rootElement.AppendChild(node5);
            }
            // Persist the Proxy password separately
            if (!String.IsNullOrEmpty(Password))
            {
                XmlNode node6 = doc.CreateNode(XmlNodeType.Element, "Password", String.Empty);
                XmlElement pswdElement = node6 as XmlElement;
                rootElement.AppendChild(node6);

                pswdElement.InnerText = Password;
                XmlAttribute pwAttr = doc.CreateAttribute("Sensitive");
                pwAttr.Value = "1";
                pswdElement.Attributes.Append(pwAttr);
            }




        }
        /// <summary>
        /// Loads Connection String persisted information
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="infoEvents"></param>
        void IDTSComponentPersist.LoadFromXML(XmlElement rootNode, IDTSInfoEvents infoEvents)
        {
            // Create a root node for the data
            if (rootNode.Name != "CRMConnectionManager")
                throw new ArgumentException("Unexpected element");

            // Unpersist the elements 
            foreach (XmlNode childNode in rootNode.ChildNodes)
            {

               // _connectionString = rootNode.GetAttribute("ConnectionString").ToString();
                if (childNode.Name == "AuthType")


                    AuthType = EnumEx.GetValueFromDescription<AuthenticationProviderTypeDescriptive>(childNode.InnerText);

                if (childNode.Name == "Domain")
                    Domain = childNode.InnerText;
                if (childNode.Name == "OrganizationUri")
                    OrganizationUri = childNode.InnerText;
                if (childNode.Name == "HomeRealmUri")
                    HomeRealmUri = childNode.InnerText;


                if (childNode.Name == "UserName")
                    UserName = childNode.InnerText;
                if (childNode.Name == "Password")
                    Password = childNode.InnerText; // The SSIS runtime will already have decrypted it for us and you don't need to do it again

                if (childNode.Name == "UsesSSL")
                    UsesSSL = Boolean.Parse(childNode.InnerText.ToString());

                if (childNode.Name == "TimeOut")
                {
                    
                    int sTime;

                    int.TryParse(childNode.InnerText, out sTime);
                    TimeOut = sTime;
                }
                if (childNode.Name == "CallerId")
                    CallerId = childNode.InnerText.ToString();
                if (childNode.Name == "Port")
                {
                    Port = childNode.InnerText.ToString();
                }
            }

            UpdateConnectionString();
        }


    }



}
