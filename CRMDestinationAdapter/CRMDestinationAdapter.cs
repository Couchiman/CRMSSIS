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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using System.Globalization;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using CRMSSIS.CRMCommon.Enumerators;

namespace CRMSSIS.CRMDestinationAdapter
{
    [DtsPipelineComponent(DisplayName = "Dynamics CRM Destination",
      ComponentType = ComponentType.DestinationAdapter,
      Description = "Connection destination for CRM Dynamics",
      IconResource = "CRMSSIS.CRMDestinationAdapter.Icon2.ico",
      UITypeName = "CRMSSIS.CRMDestinationAdapter.CRMDestinationAdapterUI, CRMSSIS.CRMDestinationAdapter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5d9126043847bebf")]
    public class CRMDestinationAdapter : PipelineComponent
    {

        #region properties

        public IOrganizationService service { get; set; }
        public Mapping mapping { get; set; }
        #endregion

        #region variables
        public int[] mapInputColsToBufferCols;
        int bchCnt = 0;
        List<int> rowIndexList = new List<int>();

        int ir = 0;
        int batchSize = 1;
        int operation = 0;
        int culture = 127;
        Guid currentUserId;
        string EntityName = "";
        string EntityId = "";
        //int responseColumn = 0;
        string WorkflowId;
        int errorOutputId = 0;
        int defaultOuputId = 0;


        #endregion

        #region strucutures
        private struct CRMIntegrate
        {
            public ExecuteMultipleRequest Req;
            public ExecuteMultipleResponse Resp;
            public string ExceptionMessage;
            public List<int> DataTableRowsIndex;
           
        }
        #endregion

        #region design-time methods

        public override void PerformUpgrade(int pipelineVersion)
        {

            ComponentMetaData.CustomPropertyCollection["UserComponentTypeName"].Value = this.GetType().AssemblyQualifiedName;

        }
        /// <summary>
        /// Connects to Dynamics instance using connection string.
        /// </summary>
        /// <param name="transaction"></param>
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
                        service = CRMCommon.CRM.Connect(_connectionstring);
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

        /// <summary>
        /// Add user and hidden properties to configure the control
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();
            ComponentMetaData.UsesDispositions = true;


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

            IDTSCustomProperty100 CultureInfo = ComponentMetaData.CustomPropertyCollection.New();
            CultureInfo.Description = "Forces specific culture info";
            CultureInfo.Name = "CultureInfo";
            CultureInfo.Value = 127;

            IDTSCustomProperty100 Entity = ComponentMetaData.CustomPropertyCollection.New();
            Entity.Description = "Entity";
            Entity.Name = "Entity";
            Entity.TypeConverter = "NOTBROWSABLE";


            IDTSCustomProperty100 Workflow = ComponentMetaData.CustomPropertyCollection.New();
            Workflow.Description = "Workflow";
            Workflow.Name = "Workflow";
            Workflow.Value = "";
            Workflow.TypeConverter = "NOTBROWSABLE";

            IDTSCustomProperty100 Mapping = ComponentMetaData.CustomPropertyCollection.New();
            Mapping.Description = "Mapping";
            Mapping.Name = "Mapping";
            Mapping.TypeConverter = "NOTBROWSABLE";


            IDTSInput100 input = ComponentMetaData.InputCollection.New();
            input.Name = "Input";
            input.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;

            

            IDTSOutput100 CRMErrors = ComponentMetaData.OutputCollection.New();
            CRMErrors.Name = "CRMErrors";
            CRMErrors.SynchronousInputID = input.ID;
            CRMErrors.ExclusionGroup = 1;
            CRMErrors.IsErrorOut = true;
           


            IDTSOutput100 CRMOK = ComponentMetaData.OutputCollection.New();
            CRMOK.Name = "CRMOK";
            CRMOK.SynchronousInputID = input.ID;
            CRMOK.ExclusionGroup = 1;

            //AD CRM GUID Column
            IDTSOutputColumn100 outputcol = CRMOK.OutputColumnCollection.New();
            outputcol.Name = "_Response_";
            outputcol.Description = "_Response_";
            outputcol.SetDataTypeProperties(DataType.DT_STR, 60, 0, 0, 1252);


            

            IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "CRMSSIS";
            connection.ConnectionManagerID = "CRMSSIS";

        }

        /// <summary>
        /// Validates properties required by the component
        /// </summary>
        /// <returns></returns>
        public override DTSValidationStatus Validate()
        {


            bool cancel;


            if (ComponentMetaData.CustomPropertyCollection["Entity"].Value == null)
            {

                ComponentMetaData.FireError(0, ComponentMetaData.Name, "Entity must be set", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }


            if ((ComponentMetaData.CustomPropertyCollection["Mapping"].Value == null))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "Column mapping has not been set", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            IDTSInput100 input = ComponentMetaData.InputCollection[0];
            IDTSOutput100 outputError = ComponentMetaData.OutputCollection[0];
            IDTSOutput100 outputOK = ComponentMetaData.OutputCollection[1];
            ValidateOutputCollumns(input, outputError);
            ValidateOutputCollumns(input, outputOK);

            return base.Validate();
        }

        /// <summary>
        /// Adds CRMOK and CRMError Columns
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void ValidateOutputCollumns(IDTSInput100 input, IDTSOutput100 output)
        {
            foreach (IDTSInputColumn100 inputColumn in input.InputColumnCollection)
            {
                bool IsPresent = false;
                foreach (IDTSOutputColumn100 outputColumn in output.OutputColumnCollection)
                {
                    if (outputColumn.Name == inputColumn.Name)
                    {
                        IsPresent = true;
                    }
                }

                if (!IsPresent)
                {
                    IDTSOutputColumn100 outputcol = output.OutputColumnCollection.New();
                    outputcol.Name = inputColumn.Name;
                    outputcol.Description = String.Format("{0} contains valid data", inputColumn.Name);
                    outputcol.SetDataTypeProperties(inputColumn.DataType, inputColumn.Length, inputColumn.Precision, inputColumn.Scale, inputColumn.CodePage);

                   // AddExternalMetaDataColumn(input, inputColumn);
                }
            }

            
        }
        /// <summary>
        /// Allows to edit output columns Datatypes.
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="outputColumnID"></param>
        /// <param name="dataType"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="codePage"></param>
        public override void SetOutputColumnDataTypeProperties(int outputID, int outputColumnID, Microsoft.SqlServer.Dts.Runtime.Wrapper.DataType dataType, int length, int precision, int scale, int codePage)

        {

            IDTSOutputCollection100 outputColl = this.ComponentMetaData.OutputCollection;

            IDTSOutput100 output = outputColl.GetObjectByID(outputID);

            IDTSOutputColumnCollection100 columnColl = output.OutputColumnCollection;

            IDTSOutputColumn100 column = columnColl.GetObjectByID(outputColumnID);

            column.SetDataTypeProperties(dataType, length, precision, scale, codePage);

        }

        public override void ReinitializeMetaData()
        {
            ComponentMetaData.RemoveInvalidInputColumns();
            
            ReinitializeMetaData();
        }

        /// <summary>
        /// Prevents user adding external output columns manually
        /// </summary>
        /// <param name="outputID"></param>
        /// <param name="outputColumnIndex"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public override IDTSOutputColumn100 InsertOutputColumnAt(int outputID, int outputColumnIndex, string name, string description)
        {
           
            throw new Exception(string.Format("Fail to add output column name to {0} ", ComponentMetaData.Name), null);
        }

        /// <summary>
        /// Capture when dataflow component is attached
        /// </summary>
        /// <param name="inputID"></param>
        public override void OnInputPathAttached(int inputID)
        {
            base.OnInputPathAttached(inputID);

            ///Create input collection from virtual inputs
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

        //private void AddExternalMetaDataColumn(IDTSInput100 input, IDTSInputColumn100 inputColumn)
        //{
        //    // Set the properties of the external metadata column.  
        //    IDTSExternalMetadataColumn100 externalColumn = input.ExternalMetadataColumnCollection.New();
        //    externalColumn.Name = inputColumn.Name;
        //    externalColumn.Precision = inputColumn.Precision;
        //    externalColumn.Length = inputColumn.Length;
        //    externalColumn.DataType = inputColumn.DataType;
        //    externalColumn.Scale = inputColumn.Scale;

        //    // Map the external column to the input column.  
        //    inputColumn.ExternalMetadataColumnID = externalColumn.ID;
        //}

        #endregion

        #region runtime methods


        /// <summary>
        /// setups the operation with required values and maps buffer with inputcolumns
        /// </summary>
        public override void PreExecute()
        {


            base.PreExecute();

            IDTSInput100 input = ComponentMetaData.InputCollection[0];
            mapInputColsToBufferCols = new int[input.InputColumnCollection.Count];



            for (int i = 0; i < ComponentMetaData.InputCollection[0].InputColumnCollection.Count; i++)
            {

                mapInputColsToBufferCols[i] = BufferManager.FindColumnByLineageID(input.Buffer, input.InputColumnCollection[i].LineageID);
            }

          

            mapping = CRMCommon.JSONSerialization.Deserialize<Mapping>(ComponentMetaData.CustomPropertyCollection["Mapping"].Value.ToString());


            batchSize = Convert.ToInt32(ComponentMetaData.CustomPropertyCollection["BatchSize"].Value.ToString());

            operation = Convert.ToInt32(ComponentMetaData.CustomPropertyCollection["Operation"].Value.ToString());
            culture = Convert.ToInt32(ComponentMetaData.CustomPropertyCollection["CultureInfo"].Value.ToString());

            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);

            currentUserId = userResponse.UserId;

            EntityName = (CRMCommon.JSONSerialization.Deserialize<Item>(ComponentMetaData.CustomPropertyCollection["Entity"].Value.ToString())).Text;
             
            errorOutputId = ComponentMetaData.OutputCollection[0].ID;
            defaultOuputId = ComponentMetaData.OutputCollection[1].ID;

            if (!string.IsNullOrEmpty(ComponentMetaData.CustomPropertyCollection["Workflow"].Value.ToString()))
                WorkflowId = (CRMCommon.JSONSerialization.Deserialize<Item>(ComponentMetaData.CustomPropertyCollection["Workflow"].Value.ToString())).Value;
            EntityId = (CRMCommon.JSONSerialization.Deserialize<Item>(ComponentMetaData.CustomPropertyCollection["Entity"].Value.ToString())).Value;


        }

        /// <summary>
        /// Process the rows from the datasource
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="buffer"></param>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            EntityCollection newEntityCollection = new EntityCollection();
            List<OrganizationRequest> Rqs = new List<OrganizationRequest>();


            Mapping.MappingItem mappedColumn;
            IDTSInputColumn100 inputcolumn;

            IDTSInput100 input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
             

           Entity newEntity;

            while (buffer.NextRow())
            {
                try
                {
                    newEntity = new Entity(EntityName);


                    bchCnt++;
                    //adds the row to output buffer for futher processing.

                  

                    foreach (int col in mapInputColsToBufferCols)
                    {
                        inputcolumn = ComponentMetaData.InputCollection[0].InputColumnCollection[col];

                        mappedColumn = mapping.ColumnList.Find(x => x.ExternalColumnName == inputcolumn.Name && x.Map == true);

                        if (mappedColumn != null)
                        {
                            if (buffer.IsNull(col) == false)
                                AttributesBuilder(mappedColumn, buffer[col], ref newEntity);
                            else
                                AttributesBuilder(mappedColumn, mappedColumn.DefaultValue, ref newEntity);
                        }


                    }


                    switch ((Operations)operation)
                    {    //Create  
                        case Operations.Create:
                            Rqs.Add(new CreateRequest { Target = newEntity });
                            newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                            break;
                        //Update
                        case Operations.Update:
                            Rqs.Add(new UpdateRequest { Target = newEntity });
                            newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                            break;
                        //Delete
                        case Operations.Delete:
                            Rqs.Add(new DeleteRequest { Target = newEntity.ToEntityReference() });
                            break;
                        //status
                        case Operations.Status:
                            Rqs.Add(new SetStateRequest
                            {
                                EntityMoniker = newEntity.ToEntityReference(),
                                State = new OptionSetValue((int)newEntity.Attributes["statecode"]),
                                Status = new OptionSetValue((int)newEntity.Attributes["statuscode"])
                            });
                            break;
                        case Operations.Upsert:
                            Rqs.Add(new UpsertRequest { Target = newEntity });
                            newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                            break;
                        case Operations.Workflow:
                            newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                            Rqs.Add(new ExecuteWorkflowRequest { EntityId= newEntity.Id, WorkflowId= Guid.Parse(WorkflowId) });
                            
                            break;
                    }
                    newEntityCollection.Entities.Add(newEntity);
                    rowIndexList.Add(ir);

                     

                    if (bchCnt == batchSize * 2 || (buffer.CurrentRow == buffer.RowCount || (buffer.RowCount % 2 !=0 && buffer.CurrentRow == buffer.RowCount-1)))
                    {
                        int startBuffIndex = buffer.CurrentRow - (bchCnt-1);
                        CRMIntegrate[] IntegrationRows = SendRowsToCRM(newEntityCollection, EntityName, Rqs);

                        sendOutputResults(IntegrationRows, buffer, startBuffIndex);
                    }

                    ir++;
                   
                }
               
                catch (Exception ex)
                {
                    switch(input.ErrorRowDisposition)
                    {
                                                
                        case DTSRowDisposition.RD_RedirectRow:
                            buffer.DirectErrorRow(errorOutputId, 0,buffer.CurrentRow);
                            break;
                        case DTSRowDisposition.RD_IgnoreFailure:
                            buffer.DirectRow(defaultOuputId);
                            break;
                        case DTSRowDisposition.RD_FailComponent:
                            throw new Exception("There was and error processing rows. " + ex.Message);
                    }
                }
            }

            //if (buffer.EndOfRowset)
            //{
            //    int startBuffIndex = rowCount - bchCnt;

            //    CRMIntegrate[] IntegrationRows = SendRowsToCRM(newEntityCollection, EntityName, Rqs);

            //    sendOutputResults(IntegrationRows, buffer, startBuffIndex);
            //}

        }

        /// <summary>
        /// Sends Outputs to files.
        /// </summary>
        /// <param name="Integ"></param>
        /// <param name="buffer"></param>
        /// <param name="startBuffIndex"></param>
        private void sendOutputResults(CRMIntegrate[] Integ, PipelineBuffer buffer, int startBuffIndex)
        {
            IEnumerable<ExecuteMultipleResponseItem> FltResp;
            IEnumerable<ExecuteMultipleResponseItem> OkResp;

            int current = buffer.CurrentRow;

            buffer.CurrentRow = startBuffIndex;

            foreach (CRMIntegrate irsp in Integ)
            {
                if (irsp.Resp != null)
                {
                    if (irsp.Resp.IsFaulted)
                    {
                        FltResp = irsp.Resp.Responses.Where(r => r.Fault != null);

                        foreach (ExecuteMultipleResponseItem itm in FltResp)
                        {  
                            buffer.DirectErrorRow(errorOutputId, itm.Fault.ErrorCode, buffer.CurrentRow);

                           
                            if (buffer.CurrentRow < buffer.RowCount)
                                buffer.NextRow();
                        }
                    }

                    OkResp = irsp.Resp.Responses.Where(r => r.Fault == null);

                    int ResponseColumn = ComponentMetaData.InputCollection[0].InputColumnCollection.Count + ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count;

                    foreach (ExecuteMultipleResponseItem itm in OkResp)
                    {

                        
                        //Add the inserted GUID for Create Operation
                        switch ((Operations)operation)
                        {
                            case Operations.Create:
                                
                                buffer.SetString(ResponseColumn, ((CreateResponse)itm.Response).id.ToString());                              
                                break;
                                                      
                            case Operations.Update:
                                buffer.SetString(ResponseColumn, ((UpdateResponse)itm.Response).Results.FirstOrDefault().Value.ToString());
                                break;
                            case Operations.Delete:
                                buffer.SetString(ResponseColumn, ((DeleteResponse)itm.Response).Results.FirstOrDefault().Value.ToString());
                                break;
                            case Operations.Upsert:
                                buffer.SetString(ResponseColumn, ((UpsertResponse)itm.Response).Results.FirstOrDefault().Value.ToString());
                                break;
                            case Operations.Status:
                                buffer.SetString(ResponseColumn, ((SetStateResponse)itm.Response).Results.FirstOrDefault().Value.ToString());
                                break;
                            case Operations.Workflow:
                                buffer.SetString(ResponseColumn, ((ExecuteWorkflowResponse)itm.Response).Results.FirstOrDefault().Value.ToString());
                                break;

                        }

           
                    buffer.DirectRow(defaultOuputId);
                    if (buffer.CurrentRow < buffer.RowCount)
                        buffer.NextRow();

                }
                    

                }
                else if (irsp.ExceptionMessage != "")                  
                    {
                       
                        buffer.DirectErrorRow(errorOutputId, -1, buffer.CurrentRow);
                        if (buffer.CurrentRow < buffer.RowCount)
                            buffer.NextRow();

                    }
                
            }

            buffer.CurrentRow = current;
        }
        /// <summary>
        /// Fill attributes in the entity to be sent to Dynamics CRM
        /// </summary>
        /// <param name="mappedColumn"></param>
        /// <param name="value"></param>
        /// <param name="newEntity"></param>
        private void AttributesBuilder(Mapping.MappingItem mappedColumn, object value, ref Entity newEntity)
        {
            switch (mappedColumn.InternalColumnType.Value)
            {
                //    break;
                case AttributeTypeCode.BigInt:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToInt64(value, new CultureInfo(culture));

                    break;
                case AttributeTypeCode.Boolean:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToBoolean(value, new CultureInfo(culture));
                    break;
                case AttributeTypeCode.DateTime:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDateTime(value, new CultureInfo(culture));

                    break;
                case AttributeTypeCode.Decimal:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDecimal(value, new CultureInfo(culture));

                    break;
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Money:

                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDouble(value, new CultureInfo(culture));
                    break;
                case AttributeTypeCode.Integer:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToInt32(value, new CultureInfo(culture));
                    break;
                case AttributeTypeCode.Picklist:

                    newEntity.Attributes[mappedColumn.InternalColumnName] = new OptionSetValue(Convert.ToInt32(value, new CultureInfo(culture)));
                    break;
                case AttributeTypeCode.Uniqueidentifier:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = new Guid(Convert.ToString(value));
                    newEntity.Id = new Guid(Convert.ToString(value));
                    break;
                case AttributeTypeCode.Owner:
                    break;
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.PartyList:

                    newEntity.Attributes[mappedColumn.InternalColumnName] = new EntityReference(mappedColumn.TargetEntity, new Guid(Convert.ToString(value, new CultureInfo(culture))));
                    break;

                default:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToString(value, new CultureInfo(culture));
                    break;
            }
        }

        /// <summary>
        /// Send information to IOrganization service in bulk way
        /// </summary>
        /// <param name="EntityList"></param>
        /// <param name="EntityName"></param>
        /// <param name="Rqs"></param>
        /// <param name="RowCount"></param>
        private CRMIntegrate[] SendRowsToCRM(EntityCollection EntityList, string EntityName, List<OrganizationRequest> Rqs)
        {

            ExecuteMultipleRequest Req;
            CRMIntegrate[] Integ = new CRMIntegrate[2];

            bchCnt = 0;
            Req = new ExecuteMultipleRequest();
            Req.Settings = new ExecuteMultipleSettings { ContinueOnError = true, ReturnResponses = true };
            Req.Requests = new OrganizationRequestCollection();
            Req.Requests.AddRange(Rqs.Take(Rqs.Count / 2));
            Integ[0] = new CRMIntegrate
            {
                ExceptionMessage = "",
                Req = Req,
                Resp = null,
                DataTableRowsIndex = rowIndexList.Take(rowIndexList.Count / 2).ToList<int>(),

            };

            Req = new ExecuteMultipleRequest();
            Req.Settings = new ExecuteMultipleSettings { ContinueOnError = true, ReturnResponses = true };
            Req.Requests = new OrganizationRequestCollection();
            Req.Requests.AddRange(Rqs.Skip(Rqs.Count / 2).Take(Rqs.Count - Rqs.Count / 2));
            Integ[1] = new CRMIntegrate
            {
                ExceptionMessage = "",
                Req = Req,
                Resp = null,
                DataTableRowsIndex = rowIndexList.Skip(rowIndexList.Count / 2).Take(rowIndexList.Count - rowIndexList.Count / 2).ToList<int>(),

            };


            Parallel.Invoke(
                                           () =>
                                           {
                                               try
                                               {
                                                   if (Integ[0].Req.Requests.Count > 0)
                                                   {
                                                       Integ[0].Resp = (ExecuteMultipleResponse)service.Execute(Integ[0].Req);
                                                   }
                                                   else
                                                   {
                                                       Integ[0].Resp = null;
                                                       Integ[0].ExceptionMessage = "";
                                                   }
                                               }
                                               catch (Exception ex)
                                               {
                                                   Integ[0].Resp = null;
                                                   Integ[0].ExceptionMessage = ex.Message;
                                               }
                                           },
                                           () =>
                                           {
                                               try
                                               {
                                                   if (Integ[1].Req.Requests.Count > 0)
                                                   {
                                                       Integ[1].Resp = (ExecuteMultipleResponse)service.Execute(Integ[1].Req);
                                                   }
                                                   else
                                                   {
                                                       Integ[1].Resp = null;
                                                       Integ[1].ExceptionMessage = "";
                                                   }
                                               }
                                               catch (Exception ex)
                                               {
                                                   Integ[1].Resp = null;
                                                   Integ[1].ExceptionMessage = ex.Message;
                                               }
                                           }
                           );


            Rqs.Clear();
            rowIndexList.Clear();

            return Integ;

        }

    }
    #endregion
}
