using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMDestinationAdapter
{
    public class Mapping
    {
        public class MappingItem
        { 
            string externalColumnName = "";
            DataType externalColumn;
            string externalColumnTypeName = "";
            string internalColumnName = "";
            string internalColumnTypeName = "";
            AttributeTypeCode? internalColumn;
            string defaultValue;
            bool map = false;

            
            public string ExternalColumnName
            {
                get
                {
                    return externalColumnName;
                }

                set
                {
                    externalColumnName = value;
                }
            }

            public DataType ExternalColumnType
            {
                get
                {
                    return externalColumn;
                }

                set
                {
                    externalColumn = value;
                }
            }

            public string InternalColumnName
            {
                get
                {
                    return internalColumnName;
                }

                set
                {
                    internalColumnName = value;
                }
            }

            public AttributeTypeCode? InternalColumnType
            {
                get
                {
                    return internalColumn;
                }

                set
                {
                    internalColumn = value;
                }
            }

            public string DefaultValue
            {
                get
                {
                    return defaultValue;
                }

                set
                {
                    defaultValue = value;
                }
            }

            public bool Map
            {
                get
                {
                    return map;
                }

                set
                {
                    map = value;
                }
            }

            public string InternalColumnTypeName
            {
                get
                {
                    return internalColumnTypeName;
                }

                set
                {
                    internalColumnTypeName = value;
                }
            }

            public string ExternalColumnTypeName
            {
                get
                {
                    return externalColumnTypeName;
                }

                set
                {
                    externalColumnTypeName = value;
                }
            }
        }

        private List<MappingItem> columnList = new List<MappingItem>();

        public List<MappingItem> ColumnList
        {
            get
            {
                return columnList;
            }

            set
            {
                columnList = value;
            }
        }

        public Mapping()
        {
            
        }

        public Mapping(AttributeMetadata[] metadata, IDTSInput100 Input)
        {
            MappingItem mi;


            int i = 0;

            foreach (AttributeMetadata attribute in metadata)
            {

                IDTSInputColumn100 inputCol = Input.InputColumnCollection.New();
                inputCol.Name = attribute.LogicalName;
                inputCol.LineageID = i++;
                

                mi = new MappingItem();

                mi.InternalColumnName = attribute.LogicalName;
                mi.InternalColumnTypeName = attribute.AttributeType.ToString();
                mi.InternalColumnType = attribute.AttributeType;

                columnList.Add(mi);
           
            //IDTSVirtualInputColumn100 external = findByName(attribute.LogicalName.ToString(), Input);
            //if (external != null)
            //{
            //    mi.ExternalColumnName = external.Name;
            //    mi.ExternalColumnType = external.DataType;
            //}
        }
           

           
        }

        private IDTSVirtualInputColumn100 findByName(string attributename, IDTSInput100 Input)
        {

            IDTSVirtualInput100 vInput = Input.GetVirtualInput();



            foreach (IDTSInputColumn100 column in Input.InputColumnCollection)
            {
                IDTSVirtualInputColumn100 vColumn = vInput.VirtualInputColumnCollection.GetVirtualInputColumnByName(attributename, attributename);
                
                if (vColumn.Name == attributename)
                {
                  
                    return vColumn;
                }
            }
            return null;
        }

    }
}
