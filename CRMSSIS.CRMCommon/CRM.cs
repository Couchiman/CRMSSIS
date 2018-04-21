using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace CRMSSIS.CRMCommon
{
    public static class CRM
    {
        public static IOrganizationService Connect(string connectionString)
        {
            try {  
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CrmServiceClient conn = new CrmServiceClient(connectionString);

            // Cast the proxy client to the IOrganizationService interface.
            IOrganizationService service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

           
                if (!conn.IsReady)
                    throw new Exception("Cannot connect to Dynamics Instance");

                return service;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static RetrieveAllEntitiesResponse RetrieveAllEntityMetatada(IOrganizationService service)
        {

            RetrieveAllEntitiesRequest mdRequest = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };
            RetrieveAllEntitiesResponse metaDataResponse = new RetrieveAllEntitiesResponse();

            return (RetrieveAllEntitiesResponse)service.Execute(mdRequest);

                    }

        public static RetrieveEntityResponse RetrieveEntityRequestMetadata(IOrganizationService service, string entityName)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityName,
                RetrieveAsIfPublished = true
            };

            return  (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);


        }

        public static EntityCollection GetWorkflowList(IOrganizationService service, string entityName)
        {

            var QEworkflow = new QueryExpression("workflow");

            QEworkflow.ColumnSet.AddColumns("workflowid", "primaryentity", "name", "statecode", "type");

            QEworkflow.Criteria.AddCondition("type", ConditionOperator.Equal, 1);

            QEworkflow.Criteria.AddCondition("primaryentity", ConditionOperator.Equal, entityName);   

            return service.RetrieveMultiple(QEworkflow);


        }
    }
}
