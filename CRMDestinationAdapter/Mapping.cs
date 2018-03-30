using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMDestinationAdapter
{
    public class Mapping
    {
        [DataContract (Name = "MappingItem")]
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

            [DataMember(Name = "ExternalColumnName")]
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

            [DataMember(Name = "ExternalColumnType")]
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
            [DataMember(Name = "InternalColumnName")]
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
            [DataMember(Name = "InternalColumnType")]
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
            [DataMember(Name = "DefaultValue")]
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
            [DataMember(Name = "Map")]
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
            [DataMember(Name = "InternalColumnTypeName")]
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
            [DataMember(Name = "ExternalColumnTypeName")]
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

        public Mapping(AttributeMetadata[] metadata, IDTSInput100 input)
        {
            MappingItem mi;


           

            foreach (AttributeMetadata attribute in metadata)
            {

                             

                mi = new MappingItem();

                mi.InternalColumnName = attribute.LogicalName;
                mi.InternalColumnTypeName = attribute.AttributeType.ToString();
                mi.InternalColumnType = attribute.AttributeType;

                //Maps by name the Input collection with Dynamics CRM collection
                foreach (IDTSInputColumn100 inputcol in input.InputColumnCollection)
                {
                    if (inputcol.Name == attribute.LogicalName)
                    {
                        mi.ExternalColumnName = inputcol.Name;
                        mi.ExternalColumnType = inputcol.DataType;
                        mi.ExternalColumnTypeName = inputcol.DataType.ToString();
                    }
                }
                         

                columnList.Add(mi);
            
        }
           

           
        }

      

    }
}
