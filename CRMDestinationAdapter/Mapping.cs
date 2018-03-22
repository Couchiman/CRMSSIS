using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
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
            DTSObjectType externalColumn;
            string internalColumnName = "";
            AttributeTypeCode? internalColumn;
            string defaultValue;
            
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

            public DTSObjectType ExternalColumn
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

            public AttributeTypeCode? InternalColumn
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

        public Mapping(AttributeMetadata[] metadata, IDTSInputCollection100 InputCollection)
        {
            MappingItem mi;

            foreach (AttributeMetadata attribute in metadata)
            {
                mi = new MappingItem();

                mi.InternalColumnName = attribute.LogicalName;
                mi.InternalColumn = attribute.AttributeType;

                IDTSInput100 external = findByName(attribute.LogicalName.ToString(), InputCollection);
                mi.ExternalColumnName = external.Name;
                mi.ExternalColumn = external.ObjectType;

            }   
        }

        private IDTSInput100 findByName(string attributename, IDTSInputCollection100 InputCollection)
        {

            foreach (IDTSInput100 input in InputCollection)
            {
                if (input.Name == attributename)
                {
                    return input;
                }
            }
            return null;
        }

    }
}
