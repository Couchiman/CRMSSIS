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
        Guid currentUserId;
        string retError = string.Empty;
        string retOK = string.Empty;
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
                        CRMCommon.CRM.Connect(_connectionstring);
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
            Entity.TypeConverter = "NOTBROWSABLE";



            IDTSCustomProperty100 Mapping = ComponentMetaData.CustomPropertyCollection.New();
            Mapping.Description = "Mapping";
            Mapping.Name = "Mapping";
            Mapping.TypeConverter = "NOTBROWSABLE";


            IDTSInput100 input = ComponentMetaData.InputCollection.New();
            input.Name = "Input";

            IDTSCustomProperty100 CRMErrors = ComponentMetaData.CustomPropertyCollection.New();
            CRMErrors.Name = "CRMErrors";
            CRMErrors.Description = "Errors during integration process";



            IDTSCustomProperty100 CRMOK = ComponentMetaData.CustomPropertyCollection.New();
            CRMOK.Name = "CRMOK";
            CRMOK.Description = "Valid GUIDs integrated registers";
           

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

            return base.Validate();
        }
 


        /// <summary>
        /// Capture when dataflow component is attached
        /// </summary>
        /// <param name="inputID"></param>
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
           
             

            var userRequest = new WhoAmIRequest();
            var userResponse = (WhoAmIResponse)service.Execute(userRequest);
            
            currentUserId = userResponse.UserId;

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

            string EntityName = (CRMCommon.JSONSerialization.Deserialize<Item>(ComponentMetaData.CustomPropertyCollection["Entity"].Value.ToString())).Text;
            Mapping.MappingItem mappedColumn;
            IDTSInputColumn100 input;


            Entity newEntity;

            while ((buffer.NextRow()))
            {
                newEntity = new Entity(EntityName);
                bchCnt++;

                foreach (int col in mapInputColsToBufferCols)
                {
                    input = ComponentMetaData.InputCollection[0].InputColumnCollection[col];

                    mappedColumn = mapping.ColumnList.Find(x => x.ExternalColumnName == input.Name && x.Map == true);

                    if(mappedColumn != null)
                    {
                        if (buffer.IsNull(col) == false)
                              AttributesBuilder(mappedColumn, buffer[col], ref newEntity);
                     else
                             AttributesBuilder(mappedColumn, mappedColumn.DefaultValue, ref newEntity);
                    }
                }
                             

                switch (operation)
                {
                    case 0:
                        Rqs.Add(new CreateRequest { Target = newEntity });
                        newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                        break;
                    case 1:
                        Rqs.Add(new UpdateRequest { Target = newEntity });
                        newEntity.Attributes["ownerid"] = new EntityReference("systemuser", currentUserId);
                        break;
                    case 2:
                        Rqs.Add(new DeleteRequest { Target = newEntity.ToEntityReference() });
                        break;
                    case 3:
                        Rqs.Add(new SetStateRequest {
                            EntityMoniker = newEntity.ToEntityReference(),
                            State = new OptionSetValue((int)newEntity.Attributes["statecode"]),
                            Status = new OptionSetValue((int)newEntity.Attributes["statuscode"])
                        });
                        break;
                }
                newEntityCollection.Entities.Add(newEntity);
                rowIndexList.Add(ir);
                ir++;

                 SendRowsToCRM(newEntityCollection, EntityName, Rqs, buffer.RowCount);
               

            }

          
        }
        private void AttributesBuilder(Mapping.MappingItem mappedColumn, object value, ref Entity newEntity)
        {
            switch (mappedColumn.InternalColumnType.Value)
            {
                //    break;
                case AttributeTypeCode.BigInt:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToInt64(value, CultureInfo.CreateSpecificCulture("en-US"));
                    
                    break;
                case AttributeTypeCode.Boolean:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToBoolean(value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
                case AttributeTypeCode.DateTime:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDateTime(value, CultureInfo.CreateSpecificCulture("en-US"));

                    break;
                case AttributeTypeCode.Decimal:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDecimal(value, CultureInfo.CreateSpecificCulture("en-US"));

                    break;
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Money:

                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToDouble(value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
                case AttributeTypeCode.Integer:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToInt32(value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
                case AttributeTypeCode.Picklist:

                    newEntity.Attributes[mappedColumn.InternalColumnName] = new OptionSetValue(Convert.ToInt32(value, CultureInfo.CreateSpecificCulture("en-US")));
                    break;
                case AttributeTypeCode.Uniqueidentifier:
                    newEntity.Attributes[mappedColumn.InternalColumnName] = new Guid(Convert.ToString(value, CultureInfo.CreateSpecificCulture("en-US")));
                    break;
                case AttributeTypeCode.Owner:
                    break;
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.PartyList:
                   
                    newEntity.Attributes[mappedColumn.InternalColumnName] = new EntityReference(mappedColumn.TargetEntity, new Guid(Convert.ToString(value, CultureInfo.CreateSpecificCulture("en-US"))));
                    break;

                default:
                        newEntity.Attributes[mappedColumn.InternalColumnName] = Convert.ToString(value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
            }
        }
      

        private void SendRowsToCRM(EntityCollection EntityList, string EntityName, List<OrganizationRequest> Rqs, int RowCount)
        {

            ExecuteMultipleRequest Req;
            CRMIntegrate[] Integ = new CRMIntegrate[2];
            IEnumerable<ExecuteMultipleResponseItem> FltResp;
            IEnumerable<ExecuteMultipleResponseItem> OkResp;
          
                       

            if (bchCnt == batchSize * 2 || (ir + 1) == RowCount)
            {
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
                    DataTableRowsIndex = rowIndexList.Take(rowIndexList.Count / 2).ToList<int>()
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
                    DataTableRowsIndex = rowIndexList.Skip(rowIndexList.Count / 2).Take(rowIndexList.Count - rowIndexList.Count / 2).ToList<int>()
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

               


                foreach (CRMIntegrate irsp in Integ)
                {
                    if (irsp.Resp != null)
                    {
                        if (irsp.Resp.IsFaulted)
                        {
                            FltResp = irsp.Resp.Responses.Where(r => r.Fault != null);

                            foreach (ExecuteMultipleResponseItem itm in FltResp)
                                retError += string.Format("Error  '{0}' -> {1}\r\n", irsp.DataTableRowsIndex[itm.RequestIndex].ToString(), itm.Fault.Message);

                        }

                        OkResp = irsp.Resp.Responses.Where(r => r.Fault == null);

                        foreach (ExecuteMultipleResponseItem itm in OkResp)
                            if(operation ==0)
                            retOK += string.Format("{0} \r\n", ((CreateResponse)itm.Response).id.ToString());
                        else
                            retOK += string.Format("{0} \r\n", itm.Response.ResponseName);

                    }
                    else if (irsp.ExceptionMessage != "")
                        retError += string.Format("Error at '{0}' integrating '{1}' to '{2}':\r\n", irsp.ExceptionMessage, irsp.DataTableRowsIndex[0].ToString(), irsp.DataTableRowsIndex[irsp.DataTableRowsIndex.Count - 1].ToString());

                }

               
                Rqs.Clear();
                rowIndexList.Clear();
            }

        }

        public override void PostExecute()
        {
            
            ComponentMetaData.CustomPropertyCollection["CRMErrors"].Value =retError;
            ComponentMetaData.CustomPropertyCollection["CRMOK"].Value =  retOK;
            
        }

    }
    #endregion
}
