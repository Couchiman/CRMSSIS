﻿using System;
using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Data;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Security;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;


namespace CRMSSIS.CRMSourceAdapter

{
    [DtsPipelineComponent(DisplayName = "Dynamics CRM Source",
        ComponentType = ComponentType.SourceAdapter,
        Description = "Connection source for CRM Dynamics",
        IconResource = "CRMSSIS.CRMSourceAdapter.Icon2.ico",
        //CurrentVersion = 2,
        UITypeName = "CRMSSIS.CRMSourceAdapter.CRMSourceAdapterUI, CRMSSIS.CRMSourceAdapter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=342aeb18c30e3bab")]
    public class CRMSourceAdapter : PipelineComponent
    {

        #region properties
        private IOrganizationService service { get; set; }
       private EntityMetadata entMetadata { get; set; }
        #endregion
        #region design time methods

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

     
        /// <summary>
        /// Validates properties required by the component
        /// </summary>
        /// <returns></returns>
        public override DTSValidationStatus Validate()
        {

            bool cancel;
            string qFetchxml = ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString();

            if (string.IsNullOrWhiteSpace(qFetchxml))
            {
                //Validate that the Fetchxml property is set
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "FetchXML Query must be set", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            // TODO : Improve querying. Right now, retrieves all(Max 5000k)
            if (qFetchxml.IndexOf("fetch") > 0)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "Use FetchXML without fetch tag: ex <entity name='accounts' ><attribute name='accountid' /></entity>", "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }



            if ((ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count == 0))
            {
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

          
            return base.Validate();
        }

       /// <summary>
       /// When metadata changes, changes re adds output columns
       /// </summary>
        public override void ReinitializeMetaData()
        {
            AddOutputColumns(ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString());
            base.ReinitializeMetaData();
        }
        /// <summary>
        /// Add output columns
        /// </summary>
        /// <param name="propertyValue"></param>
        public void AddOutputColumns(String propertyValue)
        {
            if (!String.IsNullOrEmpty(propertyValue))
            {

                if (service == null)
                    AcquireConnections(null); 

                if (service != null)
                {


                    DataTable dt = GetData(propertyValue, true);

                    IDTSOutput100 output = ComponentMetaData.OutputCollection[0];

                    output.OutputColumnCollection.RemoveAll();
                    output.ExternalMetadataColumnCollection.RemoveAll();

                    if (dt != null)
                    {
                        //Check if there are any rows in the datatable
                        if (dt.Rows != null && dt.Rows.Count > 0)
                        {

                            DataTable schemaDT = dt.CreateDataReader().GetSchemaTable();
                            foreach (DataRow row in schemaDT.Rows)
                            {

                                IDTSOutputColumn100 outputCol = output.OutputColumnCollection.New();

                              


                                AttributeMetadata mdta = entMetadata.Attributes.FirstOrDefault(m => m.LogicalName == row["ColumnName"].ToString());

                                bool isLong = false; 
                                DataType dType = DataRecordTypeToBufferType((Type)row["DataType"]);
                                dType = ConvertBufferDataTypeToFitManaged(dType, ref isLong);
                                int length = ((int)row["ColumnSize"]) == -1 ? 4000 : (int)row["ColumnSize"];
                                int precision = row["NumericPrecision"] is System.DBNull ? 0 : (short)row["NumericPrecision"];
                                int scale = row["NumericScale"] is System.DBNull ? 0 : (short)row["NumericScale"];
                                int codePage = schemaDT.Locale.TextInfo.ANSICodePage;

                                switch (dType)
                                {
                                    //case DataType.DT_DATE:
                                      
                                    //    precision = 0;
                                    //    scale = 0;
                                    //    break;
                                    case DataType.DT_STR:
                                    case DataType.DT_TEXT:
                                        MemoAttributeMetadata att = (MemoAttributeMetadata)mdta;
                                        if(att.MaxLength.HasValue) length = (int)att.MaxLength;
                                        else 
                                        length = 1048576;

                                        precision = 0;
                                        scale = 0;
                                        break;
                                    case DataType.DT_CY:
                                        MoneyAttributeMetadata attMoney = (MoneyAttributeMetadata)mdta;
                                        if (attMoney.PrecisionSource == 0) //TODO get the other types of precision sources
                                        {
                                            if (attMoney.Precision.HasValue)
                                                scale = (int)attMoney.Precision;
                                            else
                                                scale = 2;
                                        }
                                        else scale = 4;

                                        int precision1 = 0, precision2 = 0;

                                        if (attMoney.MaxValue.HasValue)
                                        {
                                            precision1 = attMoney.MaxValue.Value.ToString().Length;
                                        }
                                       
                                        if (attMoney.MinValue.HasValue)
                                        {
                                             precision2 = attMoney.MinValue.Value.ToString().Length;
                                        }
                                        if (precision1 > precision2) precision = precision1;
                                        else precision = precision2;

                                        length = 0;
                                        codePage = 0;
                                        if (precision == 0) precision = 23;
                                        
                                        if (precision > 38)
                                            precision = 38;
                                        if (scale > precision)
                                            scale = precision;
                                        break;
                                    case DataType.DT_NUMERIC:
                                    case DataType.DT_DECIMAL:
                                        DecimalAttributeMetadata attDecimal = (DecimalAttributeMetadata)mdta;
                                        
                                            if (attDecimal.Precision.HasValue)
                                                scale = (int)attDecimal.Precision;
                                            else
                                                scale = 2;


                                        int precisiondec1 = 0, precisiondec2 = 0;

                                        if (attDecimal.MaxValue.HasValue)
                                        {
                                            precisiondec1 = attDecimal.MaxValue.Value.ToString().Length;
                                        }

                                        if (attDecimal.MinValue.HasValue)
                                        {
                                            precisiondec2 = attDecimal.MinValue.Value.ToString().Length;
                                        }
                                        if (precisiondec1 > precisiondec2) precision = precisiondec1;
                                        else precision = precisiondec2;

                                        length = 0;
                                        codePage = 0;
                                        if (precision == 0) precision = 23;

                                        if (precision > 38)
                                            precision = 38;
                                        if (scale > precision)
                                            scale = precision;
                                        break;
                                    
                                    
                                    case DataType.DT_WSTR:

                                        

                                        if (mdta.GetType() == typeof(StringAttributeMetadata))
                                        { 
                                         StringAttributeMetadata attstring = (StringAttributeMetadata)mdta;
                                            if (attstring.MaxLength.HasValue) length = (int)attstring.MaxLength;
                                            else length = 4000;
                                        }
                                        else
                                        { 
                                        MemoAttributeMetadata attmemo = (MemoAttributeMetadata)mdta;
                                        length = (int)attmemo.MaxLength;
                                        }

                                        
                                         
                                        precision = 0;
                                        scale = 0;
                                        codePage = 0;
                                        break;
                                    default:
                                        length = 0;
                                        precision = 0;
                                        scale = 0;
                                        codePage = 0;
                                        break;
                                }

                                outputCol.Name = row["ColumnName"].ToString();
                                outputCol.SetDataTypeProperties(dType, length, precision, scale, codePage);

                                CreateExternalMetaDataColumn(output, outputCol);

                            }


                        }

                         
                    }
                }
            }
        }
        /// <summary>
        /// Add external output columns for offline support
        /// </summary>
        /// <param name="output"></param>
        /// <param name="outputColumn"></param>
        private void CreateExternalMetaDataColumn(IDTSOutput100 output, IDTSOutputColumn100 outputColumn)
        {
            IDTSExternalMetadataColumn100 externalColumn = output.ExternalMetadataColumnCollection.New();
            externalColumn.Name = outputColumn.Name;
            externalColumn.Precision = outputColumn.Precision;
            externalColumn.Length = outputColumn.Length;
            externalColumn.DataType = outputColumn.DataType;
            externalColumn.Scale = outputColumn.Scale;
            outputColumn.ExternalMetadataColumnID = externalColumn.ID;
        }
        /// <summary>
        /// Add user and hidden properties to configure the control
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            
            ComponentMetaData.Name = "Dynamics CRM Source Adapter";
            ComponentMetaData.ContactInfo = "couchiman@gmail.com";
            ComponentMetaData.Description = "Allows to connect to Dynamics CRM Source";

            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "Output";

            IDTSCustomProperty100 FetchXML = ComponentMetaData.CustomPropertyCollection.New();
            FetchXML.Description = "FetchXML query to get information from Dynamics";
            FetchXML.Name = "FetchXML";
            FetchXML.Value = String.Empty;

            IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "CRMSSIS";
            connection.ConnectionManagerID = "CRMSSIS";
             
        }
        /// <summary>
        /// Gets information from Dynamics entity based on FetchXML query
        /// </summary>
        /// <param name="FetchXML"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public DataTable GetData(String FetchXML, Boolean top)
        {

            if (string.IsNullOrEmpty(FetchXML))
            {
                throw new Exception("FetchXML query is empty");
            }


            try
            {

                if (service == null) { throw new Exception("No Organization Service Available"); }

                DataTable dTable = new DataTable();
                EntityCollection result = new EntityCollection();
                bool AddCol = true;
                int page = 1;
                
                AttributeMetadata mdta;

                do
                {

                    if (top)
                    {
                        result = service.RetrieveMultiple(new FetchExpression("<fetch version=\"1.0\" count=\"200\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" + FetchXML + "</fetch>"));

                        result.MoreRecords = false;
                    }
                    else
                        result = service.RetrieveMultiple(new FetchExpression(string.Format("<fetch version=\"1.0\" page=\"{1}\" paging-cookie=\"{0}\" count=\"5000\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" + FetchXML + "</fetch>", SecurityElement.Escape(result.PagingCookie), page++)));



                    RetrieveEntityRequest mdRequest = new RetrieveEntityRequest()
                    {
                        EntityFilters = EntityFilters.Attributes,
                        LogicalName = result.EntityName,
                        RetrieveAsIfPublished = false
                    };

                    RetrieveEntityResponse entityResponse = (RetrieveEntityResponse)service.Execute(mdRequest);

                    entMetadata = entityResponse.EntityMetadata;


                    if (AddCol)
                    {
                        foreach (Entity entity in result.Entities)
                        {
                            for (int iElement = 0; iElement <= entity.Attributes.Count - 1; iElement++)
                            {

                                string columnName = entity.Attributes.Keys.ElementAt(iElement);

                                if (!dTable.Columns.Contains(columnName))
                                {
                                    mdta = entMetadata.Attributes.FirstOrDefault(m => m.LogicalName == columnName);
                                   


                                    switch (mdta.AttributeType.Value)
                                    {
                                        //    break;
                                        case AttributeTypeCode.BigInt:
                                            dTable.Columns.Add(columnName, typeof(Int64));
                                            
                                            break;
                                        case AttributeTypeCode.Boolean:
                                            dTable.Columns.Add(columnName, typeof(bool));
                                            break;
                                        case AttributeTypeCode.DateTime:
                                            dTable.Columns.Add(columnName, typeof(DateTime));

                                            break;
                                        case AttributeTypeCode.Decimal:
                                            dTable.Columns.Add(columnName, typeof(decimal));
                                            
                                            break;
                                        case AttributeTypeCode.Double:
                                        case AttributeTypeCode.Money:
                                           
                                            dTable.Columns.Add(columnName, typeof(float));
                                            break;
                                        case AttributeTypeCode.Integer:
                                        case AttributeTypeCode.Picklist:
                                            dTable.Columns.Add(columnName, typeof(Int32));
                                            break;
                                        case AttributeTypeCode.Uniqueidentifier:
                                        case AttributeTypeCode.Customer:
                                        case AttributeTypeCode.Lookup:
                                        case AttributeTypeCode.PartyList:
                                        case AttributeTypeCode.Owner:
                                            dTable.Columns.Add(columnName, typeof(Guid));
                                            break;
                                        
                                         default:
                                            
                                           
                                            dTable.Columns.Add(columnName,typeof(string));
                                            break;
                                    }
                                   
                                }
                            }
                        }
                    }
                    else AddCol = false;




                    foreach (Entity entity in result.Entities)
                    {


                        DataRow dRow = dTable.NewRow();
                        for (int i = 0; i <= entity.Attributes.Count - 1; i++)
                        {
                            string colName = entity.Attributes.Keys.ElementAt(i);



                            mdta = entMetadata.Attributes.FirstOrDefault(m => m.LogicalName == colName);

                            switch (mdta.AttributeType.Value)
                            {


                                //case AttributeTypeCode.Boolean:
                                //    dRow[colName] = entity.Attributes.Values.ElementAt(i).ToString() == "1" || entity.Attributes.Values.ElementAt(i).ToString().Trim().ToLower() == "true";
                                //    break;
                                case AttributeTypeCode.Picklist:
                                    dRow[colName] = ((Microsoft.Xrm.Sdk.OptionSetValue)entity.Attributes.Values.ElementAt(i)).Value;
                                    break;

                                case AttributeTypeCode.Customer:
                                case AttributeTypeCode.Lookup:
                                case AttributeTypeCode.PartyList:
                                case AttributeTypeCode.Owner:

                                    
                                    dRow[colName] = (Guid)((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes.Values.ElementAt(i)).Id;
                                    break;
                                case AttributeTypeCode.BigInt:
                                    dRow[colName] = (Int64?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Decimal:
                                 
                                    dRow[colName] = (decimal?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Double:
                                    dRow[colName] = (double?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Integer:
                                    dRow[colName] = (int?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Money:
                                    dRow[colName] = (float?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.DateTime:
                                    dRow[colName] = (DateTime?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Uniqueidentifier:
                                    dRow[colName] = (Guid?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                case AttributeTypeCode.Boolean:
                                    dRow[colName] = (bool?)entity.Attributes.Values.ElementAt(i);
                                    break;
                                default:
                                    dRow[colName] = (string)entity.Attributes.Values.ElementAt(i);
                                    
                                    break;
                            }
                        }
                        dTable.Rows.Add(dRow);
                       
                    }
                }
                while (result.MoreRecords);
                
                return dTable;

            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion
        #region runtime methods
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {

            //System.Diagnostics.Debugger.Launch();

            base.PrimeOutput(outputs, outputIDs, buffers);

            IDTSOutput100 output = ComponentMetaData.OutputCollection.FindObjectByID(outputIDs[0]);
            PipelineBuffer buffer = buffers[0];

            DataTable dt = new DataTable();

            dt = GetData(ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString(), false);

            foreach (DataRow row in dt.Rows)
            {
                buffer.AddRow();

                for (int x = 0; x < mapOutputColsToBufferCols.Length; x++)
                {
                    if (row.IsNull(x))
                        buffer.SetNull(mapOutputColsToBufferCols[x]);
                    else
                        buffer[mapOutputColsToBufferCols[x]] = row[x];
                }
            }

            buffer.SetEndOfRowset();
        }

        public int[] mapOutputColsToBufferCols;


        public override void PreExecute()
        {


            base.PreExecute();

            IDTSOutput100 output = ComponentMetaData.OutputCollection[0];
            mapOutputColsToBufferCols = new int[output.OutputColumnCollection.Count];

            for (int i = 0; i < ComponentMetaData.OutputCollection[0].OutputColumnCollection.Count; i++)
            {
                // Here, "i" is the column count in the component's outputcolumncollection
                // and the value of mapOutputColsToBufferCols[i] is the index of the corresponding column in the
                // buffer.
                mapOutputColsToBufferCols[i] = BufferManager.FindColumnByLineageID(output.Buffer, output.OutputColumnCollection[i].LineageID);
            }
        }
        #endregion
    }
}
