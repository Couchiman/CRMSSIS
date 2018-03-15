using System;
using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Data;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Security;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;

namespace CRMSSIS.CRMSourceAdapter

{
    [DtsPipelineComponent(DisplayName = "Dynamics CRM Source",
        ComponentType = ComponentType.SourceAdapter,
        Description = "Connection source for CRM Dynamics",
        IconResource = "CRMSSIS.CRMSourceAdapter.Icon2.ico",
        CurrentVersion = 2,
        UITypeName = "CRMSSIS.CRMSourceAdapter.CRMSourceAdapterUI, CRMSSIS.CRMSourceAdapter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=342aeb18c30e3bab")]
    public class CRMSourceAdapter : PipelineComponent
    {

        //[SSIS.Pipeline]
        //Error: Cannot find the connection manager with ID "{B116844B-768A-4A52-B3DE-EDD303605854}" in the connection manager 
        //    collection due to error code 0xC0010009. That connection manager is needed by "CRM Dynamics Source.Connections[CRMSSIS]"
        //    in the connection manager collection of "CRM Dynamics Source". Verify that a connection manager in the connection manager collection,
        //    Connections, has been created with that ID.


        private IOrganizationService service { get; set; }
       


        // Define the set of possible values for the new custom property.  

        private enum InvalidValueHandling
        {
            Ignore,
            FireInformation,
            FireWarning,
            FireError
        };

        public override void PerformUpgrade(int pipelineVersion)
        {
            
            ComponentMetaData.CustomPropertyCollection["UserComponentTypeName"].Value = this.GetType().AssemblyQualifiedName;

        }
        //public override void PerformUpgrade(int pipelineVersion)
        //{

        //    // Obtain the current component version from the attribute.  
        //    DtsPipelineComponentAttribute componentAttribute =
        //      (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
        //    int currentVersion = componentAttribute.CurrentVersion;

        //    // If the component version saved in the package is less than  
        //    //  the current version, Version 2, perform the upgrade.  
        //    if (ComponentMetaData.Version < currentVersion)

        //    // Get the current value of the old custom property, RaiseErrorOnInvalidValue,   
        //    // and then remove the property from the custom property collection.  
        //    {
        //        bool oldValue = false;

        //        IDTSCustomProperty100 oldProperty =
        //          ComponentMetaData.CustomPropertyCollection["RaiseErrorOnInvalidValue"];
        //        oldValue = (bool)oldProperty.Value;
        //        ComponentMetaData.CustomPropertyCollection.RemoveObjectByIndex("RaiseErrorOnInvalidValue");


        //        // Set the value of the new custom property, InvalidValueHandling,  
        //        //  by using the appropriate enumeration value.  
        //        IDTSCustomProperty100 newProperty =
        //           ComponentMetaData.CustomPropertyCollection["InvalidValueHandling"];
        //        if (oldValue == true)
        //        {
        //            newProperty.Value = InvalidValueHandling.FireError;
        //        }
        //        else
        //        {
        //            newProperty.Value = InvalidValueHandling.Ignore;
        //        }

        //    }

        //    // Update the saved component version metadata to the current version.  
        //    ComponentMetaData.Version = currentVersion;

        //}

        public override void AcquireConnections(object transaction)
        {

            if (ComponentMetaData.RuntimeConnectionCollection.Count > 0)
            {
                if (ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
                {

                    IDTSConnectionManager100 connectionManager = ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager;
                    
      
                   //     ConnectionManager connectionManager = DtsConvert.GetWrapper(
                   //ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);
                      
                    string _connectionString = (string)connectionManager.AcquireConnection(null);

                    if (connectionManager == null)
                        throw new Exception("Could not get connection manager");



                    try
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        CrmServiceClient conn = new CrmServiceClient(_connectionString);
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

     
        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {

            //System.Diagnostics.Debugger.Launch();

            base.PrimeOutput(outputs, outputIDs, buffers);

            IDTSOutput100 output = ComponentMetaData.OutputCollection.FindObjectByID(outputIDs[0]);
            PipelineBuffer buffer = buffers[0];

            DataTable dt = new DataTable();
            //dt=GetData(ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString(), false);

            //if (ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString() != string.Empty) {
            dt = GetData(ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString(), false);
            //} else
            //{  
            //DataColumn colId = new DataColumn("accountid", typeof(String));
            //dt.Columns.Add(colId);


            //DataColumn colDate = new DataColumn("primarycontactid", typeof(String));
            //dt.Columns.Add(colDate);
            // }


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

        public override void ReinitializeMetaData()
        {
            AddOutputColumns(ComponentMetaData.CustomPropertyCollection["FetchXML"].Value.ToString());
            base.ReinitializeMetaData();
        }
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


                                bool isLong = false;
                                DataType dType = DataRecordTypeToBufferType((Type)row["DataType"]);
                                dType = ConvertBufferDataTypeToFitManaged(dType, ref isLong);
                                int length = ((int)row["ColumnSize"]) == -1 ? 1000 : (int)row["ColumnSize"];
                                int precision = row["NumericPrecision"] is System.DBNull ? 0 : (short)row["NumericPrecision"];
                                int scale = row["NumericScale"] is System.DBNull ? 0 : (short)row["NumericScale"];
                                int codePage = schemaDT.Locale.TextInfo.ANSICodePage;

                                switch (dType)
                                {
                                    case DataType.DT_STR:
                                    case DataType.DT_TEXT:
                                        precision = 0;
                                        scale = 0;
                                        break;
                                    case DataType.DT_NUMERIC:
                                        length = 0;
                                        codePage = 0;
                                        if (precision > 38)
                                            precision = 38;
                                        if (scale > precision)
                                            scale = precision;
                                        break;
                                    case DataType.DT_DECIMAL:
                                        length = 0;
                                        precision = 0;
                                        codePage = 0;
                                        if (scale > 28)
                                            scale = 28;
                                        break;
                                    case DataType.DT_WSTR:
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

                        //DtsPipelineComponentAttribute componentAttribute = (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
                        //ComponentMetaData.Version = componentAttribute.CurrentVersion;
                    }
                }
            }
        }
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
        public override void ProvideComponentProperties()
        {
            base.RemoveAllInputsOutputsAndCustomProperties();
            ComponentMetaData.RuntimeConnectionCollection.RemoveAll();

            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "Output";

            IDTSCustomProperty100 FetchXML = ComponentMetaData.CustomPropertyCollection.New();
            FetchXML.Description = "FetchXML Query";
            FetchXML.Name = "FetchXML";
            FetchXML.Value = String.Empty;

            IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "CRMSSIS";
            //connection.ConnectionManagerID = "CRMSSIS";
            


        }

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

                     do
                    {

                        if (top)
                        {
                            result = service.RetrieveMultiple(new FetchExpression("<fetch version=\"1.0\" count=\"200\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" + FetchXML + "</fetch>"));
                            result.MoreRecords = false;
                        }
                        else
                            result = service.RetrieveMultiple(new FetchExpression(string.Format("<fetch version=\"1.0\" page=\"{1}\" paging-cookie=\"{0}\" count=\"5000\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" + FetchXML + "</fetch>", SecurityElement.Escape(result.PagingCookie), page++)));

                        if (AddCol)
                        {
                            foreach (Entity entity in result.Entities)
                            {
                                for (int iElement = 0; iElement <= entity.Attributes.Count - 1; iElement++)
                                {

                                    string columnName = entity.Attributes.Keys.ElementAt(iElement);
                                    if (!dTable.Columns.Contains(columnName))
                                        dTable.Columns.Add(columnName);

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
                                dRow[colName] = entity.Attributes.Values.ElementAt(i);
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


    }
}
