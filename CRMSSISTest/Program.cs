﻿// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

//<snippetAuthenticateWithNoHelp>
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Crm.Sdk.Messages;
using System.Net;
using CRMSSIS.CRMSourceAdapter;
using CRMSSIS.CRMConnectionManager;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Microsoft.Crm.Sdk.Samples
{
    /// <summary>
    /// Demonstrate how to do basic authentication using IServiceManagement and SecurityTokenResponse.
    /// </summary>
    class AuthenticateWithNoHelp
    {
        #region Class Level Members
        // To get discovery service address and organization unique name, 
        // Sign in to your CRM org and click Settings, Customization, Developer Resources.
        // On Developer Resource page, find the discovery service address under Service Endpoints and organization unique name under Your Organization Information.
        private String _discoveryServiceAddress = "https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc";
        private String _organizationUniqueName = "org82cd5538";
        // Provide your user name and password.
        private String _userName = "pcosimo@axxonconsulting.com";
        private String _password = "Axxon2020";

        // Provide domain name for the On-Premises org.
        private String _domain = "ar";

        #endregion Class Level Members

        #region How To Sample Code
        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            //<snippetAuthenticateWithNoHelp1>
            IServiceManagement<IDiscoveryService> serviceManagement =
                        ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(
                        new Uri(_discoveryServiceAddress));
            AuthenticationProviderType endpointType = serviceManagement.AuthenticationType;

            // Set the credentials.
            AuthenticationCredentials authCredentials = GetCredentials(serviceManagement, endpointType);


            String organizationUri = String.Empty;
            // Get the discovery service proxy.
            using (DiscoveryServiceProxy discoveryProxy =
                GetProxy<IDiscoveryService, DiscoveryServiceProxy>(serviceManagement, authCredentials))
            {
                // Obtain organization information from the Discovery service. 
                if (discoveryProxy != null)
                {
                    // Obtain information about the organizations that the system user belongs to.
                    OrganizationDetailCollection orgs = DiscoverOrganizations(discoveryProxy);
                    // Obtains the Web address (Uri) of the target organization.
                    organizationUri = FindOrganization(_organizationUniqueName,
                        orgs.ToArray()).Endpoints[EndpointType.OrganizationService];

                }
            }
            //</snippetAuthenticateWithNoHelp1>


            if (!String.IsNullOrWhiteSpace(organizationUri))
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //<snippetAuthenticateWithNoHelp3>
                IServiceManagement<IOrganizationService> orgServiceManagement =
                    ServiceConfigurationFactory.CreateManagement<IOrganizationService>(
                    new Uri(organizationUri));

                // Set the credentials.
                AuthenticationCredentials credentials = GetCredentials(orgServiceManagement, endpointType);

                // Get the organization service proxy.
                using (OrganizationServiceProxy organizationProxy =
                    GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, credentials))
                {
                    // This statement is required to enable early-bound type support.
                    organizationProxy.EnableProxyTypes();
                     
                    
                    // Now make an SDK call with the organization service proxy.
                    // Display information about the logged on user.
                    //Guid userid = ((WhoAmIResponse)organizationProxy.Execute(
                    //    new WhoAmIRequest())).UserId;
                    //SystemUser systemUser = organizationProxy.Retrieve("systemuser", userid,
                    //   new ColumnSet(new string[] { "firstname", "lastname" })).ToEntity<SystemUser>();
                    //Console.WriteLine("Logged on user is {0} {1}.",
                    //    systemUser.FirstName, systemUser.LastName);
                }
                //</snippetAuthenticateWithNoHelp3>
            }

        }

        //<snippetAuthenticateWithNoHelp2>
        /// <summary>
        /// Obtain the AuthenticationCredentials based on AuthenticationProviderType.
        /// </summary>
        /// <param name="service">A service management object.</param>
        /// <param name="endpointType">An AuthenticationProviderType of the CRM environment.</param>
        /// <returns>Get filled credentials.</returns>
        private AuthenticationCredentials GetCredentials<TService>(IServiceManagement<TService> service, AuthenticationProviderType endpointType)
        {
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();

            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential =
                        new System.Net.NetworkCredential(_userName,
                            _password,
                            _domain);
                    break;
                case AuthenticationProviderType.LiveId:
                    authCredentials.ClientCredentials.UserName.UserName = _userName;
                    authCredentials.ClientCredentials.UserName.Password = _password;
                    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                    authCredentials.SupportingCredentials.ClientCredentials =
                        Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
                    break;
                default: // For Federated and OnlineFederated environments.                    
                    authCredentials.ClientCredentials.UserName.UserName = _userName;
                    authCredentials.ClientCredentials.UserName.Password = _password;
                    // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
                    // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  // Windows Kerberos

                    // The service is configured for User Id authentication, but the user might provide Microsoft
                    // account credentials. If so, the supporting credentials must contain the device credentials.
                    if (endpointType == AuthenticationProviderType.OnlineFederation)
                    {
                        IdentityProvider provider = service.GetIdentityProvider(authCredentials.ClientCredentials.UserName.UserName);
                        if (provider != null && provider.IdentityProviderType == IdentityProviderType.LiveId)
                        {
                            authCredentials.SupportingCredentials = new AuthenticationCredentials();
                            authCredentials.SupportingCredentials.ClientCredentials =
                                Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
                        }
                    }

                    break;
            }

            return authCredentials;
        }
        //</snippetAuthenticateWithNoHelp2>

        /// <summary>
        /// Discovers the organizations that the calling user belongs to.
        /// </summary>
        /// <param name="service">A Discovery service proxy instance.</param>
        /// <returns>Array containing detailed information on each organization that 
        /// the user belongs to.</returns>
        public OrganizationDetailCollection DiscoverOrganizations(
            IDiscoveryService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse orgResponse =
                (RetrieveOrganizationsResponse)service.Execute(orgRequest);

            return orgResponse.Details;
        }

        /// <summary>
        /// Finds a specific organization detail in the array of organization details
        /// returned from the Discovery service.
        /// </summary>
        /// <param name="orgUniqueName">The unique name of the organization to find.</param>
        /// <param name="orgDetails">Array of organization detail object returned from the discovery service.</param>
        /// <returns>Organization details or null if the organization was not found.</returns>
        /// <seealso cref="DiscoveryOrganizations"/>
        public OrganizationDetail FindOrganization(string orgUniqueName,
            OrganizationDetail[] orgDetails)
        {
            if (String.IsNullOrWhiteSpace(orgUniqueName))
                throw new ArgumentNullException("orgUniqueName");
            if (orgDetails == null)
                throw new ArgumentNullException("orgDetails");
            OrganizationDetail orgDetail = null;

            foreach (OrganizationDetail detail in orgDetails)
            {
                if (String.Compare(detail.UniqueName, orgUniqueName,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    orgDetail = detail;
                    break;
                }
            }
            return orgDetail;
        }

        /// <summary>
        /// Generic method to obtain discovery/organization service proxy instance.
        /// </summary>
        /// <typeparam name="TService">
        /// Set IDiscoveryService or IOrganizationService type to request respective service proxy instance.
        /// </typeparam>
        /// <typeparam name="TProxy">
        /// Set the return type to either DiscoveryServiceProxy or OrganizationServiceProxy type based on TService type.
        /// </typeparam>
        /// <param name="serviceManagement">An instance of IServiceManagement</param>
        /// <param name="authCredentials">The user's Microsoft Dynamics CRM logon credentials.</param>
        /// <returns></returns>
        /// <snippetAuthenticateWithNoHelp4>
        private TProxy GetProxy<TService, TProxy>(
            IServiceManagement<TService> serviceManagement,
            AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Type classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType !=
                AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(authCredentials);
                // Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
                // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }
        /// </snippetAuthenticateWithNoHelp4>

        #endregion How To Sample Code

        #region Main method

        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        /// <param name="args"></param>
        static public void Main(string[] args)
        {
            try
            {

                Prueba();
                //AuthenticateWithNoHelp app = new AuthenticateWithNoHelp();
                //app.Run();
                CRMConnectionManager conn = new CRMConnectionManager();
                IOrganizationService service  = (IOrganizationService)conn.AcquireConnection(null);
                 

                CRMSourceAdapter crm = new CRMSourceAdapter();
                 
                string str = @"<entity name='account'><attribute name='primarycontactid' /></entity>";
                crm.GetData(str, false);


            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
        #endregion Main method
   


    public static void Prueba()
    {
            // The package is the ExecuteProcess package sample 
            // that is installed with the SSIS samples.
            string pkg = @"C:\CRMSSIS\CRMIngration\CRMIngration\Package2.dtsx";

        Application app = new Application();
         Package p1 = app.LoadPackage(pkg, null);

    // Show the properties inherited from DtsContainer.
    Console.WriteLine("CreationName:    {0}", p1.CreationName);
            Console.WriteLine("DebugMode:       {0}", p1.DebugMode);
            Console.WriteLine("DelayValidation: {0}", p1.DelayValidation);
            Console.WriteLine("Description:     {0}", p1.Description);
            Console.WriteLine("Disable:         {0}", p1.Disable);
        Console.WriteLine("Paquete {0}", p1.Connections.Count.ToString());

            // Description is not set for this sample, so set it.
            p1.Description = "This is the Execute Process Package Sample";
            Console.WriteLine("Description after modification: {0}", p1.Description);

            p1.DelayValidation = true;
            p1.DebugMode = true;


            Connections myConns = p1.Connections;
            int connCount = myConns.Count;
            Console.WriteLine("The number of connections in the package is: {0}", connCount);
             

            // Enumerate over the collection, printing out
            // the values for various properties.
            Console.WriteLine("Package Validation {0}", p1.Validate(myConns, null, null, null)); 
            foreach (ConnectionManager connMgr in myConns)
            {
                Console.WriteLine("ConnectionString:        {0}", connMgr.ConnectionString);
                Console.WriteLine("CreationName:            {0}", connMgr.CreationName);
                Console.WriteLine("DelayValidation:         {0}", connMgr.DelayValidation);
                Console.WriteLine("Description:             {0}", connMgr.Description);
                Console.WriteLine("HostType:                {0}", connMgr.HostType);
                Console.WriteLine("ID:                      {0}", connMgr.ID);
                Console.WriteLine("InnerObject:             {0}", connMgr.InnerObject);
                Console.WriteLine("Name:                    {0}", connMgr.Name);
                Console.WriteLine("ProtectionLevel:         {0}", connMgr.ProtectionLevel);
                Console.WriteLine("SupportsDTCTransactions: {0}", connMgr.SupportsDTCTransactions);
                Console.WriteLine("Executon Path {0}", connMgr.GetExecutionPath());
               
            }
            
            //foreach (Executable e in p1.Executables)
            //{
            //    MainPipe mp = ((TaskHost)e).InnerObject as MainPipe;
            //    if (e != null)
            //    {
            //        // Walk the components.
            //        foreach (IDTSComponentMetaData100 md in mp.ComponentMetaDataCollection)
            //        {
            //            // Walk the RuntimeConnectionCollection.
            //            foreach (IDTSRuntimeConnection100 rc in md.RuntimeConnectionCollection)
            //            {
            //                // Check to see if the package's connections collection contains the 
            //                // Connectionmanager stored in the RuntimeConnection.
            //                if (p1.Connections.Contains(rc.ConnectionManagerID)) { 
            //                    rc.ConnectionManager = DtsConvert.ToConnectionManage100(p1.Connections[rc.ConnectionManagerID]);
            //                Console.Write(rc.ConnectionManagerID);
            //                }
            //                else
            //                    Console.WriteLine("The ConnectionManager " + rc.ConnectionManagerID + " was not found in the Package's Connections collection.");
            //            }
            //        }
            //    }
           // }
            DTSExecResult r = p1.Execute();
            Console.WriteLine(r);
            Console.ReadKey();
        }
}
}
//</snippetAuthenticateWithNoHelp>