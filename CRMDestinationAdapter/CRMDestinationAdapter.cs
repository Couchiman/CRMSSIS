using System;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.Xrm.Sdk;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.Xrm.Sdk.Metadata;

using CRMSSIS.CRMCommon.Controls;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel;

namespace CRMSSIS.CRMDestinationAdapter
{
    [DtsPipelineComponent(DisplayName = "Dynamics CRM Destination",
      ComponentType = ComponentType.DestinationAdapter,
      Description = "Connection destination for CRM Dynamics",
      IconResource = "CRMSSIS.CRMDestinationAdapter.Icon2.ico",
      UITypeName = "CRMSSIS.CRMDestinationAdapter.CRMDestinationAdapterUI, CRMSSIS.CRMDestinationAdapter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5d9126043847bebf")]
    public class CRMDestinationAdapter : PipelineComponent
    {

        public IOrganizationService service { get; set; }
        public Mapping mapping { get; set; }


        public override void PerformUpgrade(int pipelineVersion)
        {

            ComponentMetaData.CustomPropertyCollection["UserComponentTypeName"].Value = this.GetType().AssemblyQualifiedName;

        }

        public override void AcquireConnections(object transaction)
        {

            if (ComponentMetaData.RuntimeConnectionCollection.Count > 0)
            {
                if (ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
                {

                    var connectionManager = ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager;


                    string _connectionstring = (string)connectionManager.AcquireConnection(null);

                    if (connectionManager == null)
                        throw new Exception("Could not get connection manager");



                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        CrmServiceClient conn = new CrmServiceClient(_connectionstring);

                        // Cast the proxy client to the IOrganizationService interface.
                        this.service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;



                        if (!conn.IsReady)
                            throw new Exception("Cannot connect to Dynamics Instance");

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                else
                {
                    throw new Exception("Couldn't get the Connection Manager");
                }
            }

        }


        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            ComponentMetaData.Name = "Dynamics CRM Destination Adapter";
            ComponentMetaData.ContactInfo = "couchiman@gmail.com";
            ComponentMetaData.Description = "Allows to connect to Dynamics CRM Destination";

            IDTSCustomProperty100 Operation = ComponentMetaData.CustomPropertyCollection.New();
            Operation.Description = "The action code for the operation to take place over an Entity";
            Operation.Name = "Operation";
            Operation.Value = 0;

            IDTSCustomProperty100 BatchSize = ComponentMetaData.CustomPropertyCollection.New();
            BatchSize.Description = "Data block limit";
            BatchSize.Name = "BatchSize";
            BatchSize.Value = 1;

            IDTSCustomProperty100 Entity = ComponentMetaData.CustomPropertyCollection.New();
            Entity.Description = "Entity";
            Entity.Name = "Entity";
            


            IDTSCustomProperty100 Mapping = ComponentMetaData.CustomPropertyCollection.New();
            Mapping.Description = "Mapping";
            Mapping.Name = "Mapping";
            
             

            IDTSInput100 input = ComponentMetaData.InputCollection.New();
            input.Name = "Input";
           
            IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "CRMSSIS";
            connection.ConnectionManagerID = "CRMSSIS";

        }

        public override DTSValidationStatus Validate()
        {


            bool cancel = false;


            if (ComponentMetaData.CustomPropertyCollection["Entity"].Value ==null)
            {

                ComponentMetaData.FireError(0, ComponentMetaData.Name, "Entity must be set", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }


            if ((ComponentMetaData.CustomPropertyCollection["Mapping"].Value ==null))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "Column mapping has not been set", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            return base.Validate();
        }
      

public override void OnInputPathAttached(int inputID)
        {
            base.OnInputPathAttached(inputID);

          
            for (int i = 0; i < ComponentMetaData.InputCollection.Count; i++)
            {
                ComponentMetaData.InputCollection[i].InputColumnCollection.RemoveAll();
                IDTSVirtualInput100 input = ComponentMetaData.InputCollection[i].GetVirtualInput();
                foreach (IDTSVirtualInputColumn100 vcol in input.VirtualInputColumnCollection)
                {
                    input.SetUsageType(vcol.LineageID, DTSUsageType.UT_READONLY);
                }
            }
        }
        //public override void OnInputPathDetached(int inputID)
        //{
        //    input.InputColumnCollection.RemoveAll();
        //}

        //private void CreateExternalMetaDataColumn(IDTSInput100 input, IDTSInputColumn100 inputColumn)
        //{
        //    IDTSExternalMetadataColumn100 externalColumn = input.ExternalMetadataColumnCollection.New();
        //    externalColumn.Name = inputColumn.Name;
        //    externalColumn.Precision = inputColumn.Precision;
        //    externalColumn.Length = inputColumn.Length;
        //    externalColumn.DataType = inputColumn.DataType;
        //    externalColumn.Scale = inputColumn.Scale;
        //    inputColumn.ExternalMetadataColumnID = externalColumn.ID;
        //}

        public int[] mapInputColsToBufferCols;

        public override void PreExecute()
        {


            base.PreExecute();

            IDTSInput100 input = ComponentMetaData.InputCollection[0];
            mapInputColsToBufferCols = new int[input.InputColumnCollection.Count];

            for (int i = 0; i < ComponentMetaData.InputCollection[0].InputColumnCollection.Count; i++)
            {

                mapInputColsToBufferCols[i] = BufferManager.FindColumnByLineageID(input.Buffer, input.InputColumnCollection[i].LineageID);
            }

            mapping = (Mapping)ComponentMetaData.CustomPropertyCollection["Mapping"].Value;
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            EntityCollection newEntityCollection = new EntityCollection();

            while ((buffer.NextRow()))
            {
                Entity newEntity = new Entity(((Item)ComponentMetaData.CustomPropertyCollection["Entity"].Value).Text);

                foreach (int col in mapInputColsToBufferCols)
                {
                    if (buffer.IsNull(col) == false)
                    {
                        IDTSInput100 input = ComponentMetaData.InputCollection[0];

                        Mapping.MappingItem mappedColumn = mapping.ColumnList.Find(x => x.ExternalColumnName == input.InputColumnCollection[col].Name && x.Map == true);

                        if (mappedColumn != null)
                        {
                            newEntity.Attributes[mappedColumn.InternalColumnName] = buffer[col];
                        }
                        //TODO From here 
                        //Build Entity - Upsert Operation
                        //Add Operation Type for Status Operation/Association, etc...
                        // 
                    }
                    newEntityCollection.Entities.Add(newEntity);
                }
                
            }

                ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
                {
                    // Assign settings that define execution behavior: continue on error, return responses. 
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = true,
                        ReturnResponses = true
                    },
                    // Create an empty organization request collection.
                    Requests = new OrganizationRequestCollection()
                };


                foreach (var entity in newEntityCollection.Entities)
                {
                    CreateRequest createRequest = new CreateRequest { Target = entity };
                    requestWithResults.Requests.Add(createRequest);
                }

                try
                {
                    var responseRecords =
                        (ExecuteTransactionResponse)service.Execute(requestWithResults);

                    int i = 0;
                    // Display the results returned in the responses.
                    foreach (var responseItem in responseRecords.Responses)
                    {
                        if (responseItem != null)
                            //TODO
                            //DisplayResponse(requestWithResults.Requests[i], responseItem);
                        i++;
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    Console.WriteLine("Create request failed for the account{0} and the reason being: {1}",
                        ((ExecuteTransactionFault)(ex.Detail)).FaultedRequestIndex + 1, ex.Detail.Message);
                    throw;
                }

            }
        }

    }
}
