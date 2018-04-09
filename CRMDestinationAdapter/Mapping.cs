using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CRMSSIS.CRMCommon;
using Microsoft.Xrm.Sdk.Messages;

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
            Boolean IsRequired = false;
            Boolean IsPrimary = false;
            string targetEntity =""; 

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
            [DataMember(Name = "isRequired")]
            public bool isRequired
            {
                get
                {
                    return IsRequired;
                }

                set
                {
                    IsRequired = value;
                }
            }
            [DataMember(Name = "isPrimary")]
            public bool isPrimary
            {
                get
                {
                    return IsPrimary;
                }

                set
                {
                    IsPrimary = value;
                }
            }
            [DataMember(Name = "TargetEntity")]
            public string TargetEntity
            {
                get
                {
                    return targetEntity;
                }

                set
                {
                    targetEntity = value;
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

        /// <summary>
       
        
        public Mapping(AttributeMetadata[] metadata, IDTSInput100 input, int Operation)
        {
           

            foreach (AttributeMetadata attribute in metadata)
            {

                AddSupportedMappingItem(attribute, input, Operation);
                
           }
                    
        }

        /// <summary>
        /// Add an Item to the columnlist 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="Operation"></param>
        private void AddSupportedMappingItem(AttributeMetadata attribute, IDTSInput100 input, int Operation)
        {
            MappingItem mi;

            if (SupportedTypes.isValidAttribute(attribute, Operation))
            {
                mi = new MappingItem();

                mi.InternalColumnName = attribute.LogicalName;
                mi.InternalColumnTypeName = attribute.AttributeType.ToString();
                mi.InternalColumnType = attribute.AttributeType;
                mi.isPrimary = attribute.IsPrimaryId.HasValue ? (bool)attribute.IsPrimaryId : false;
                mi.isRequired = attribute.IsRequiredForForm.HasValue ? (bool)attribute.IsRequiredForForm : false;



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
        /// <summary>
        /// This operation is being called by Refresh Metada button 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentMetadata"></param>
        /// <param name="newMetadata"></param>
        /// <param name="Operation"></param>
        public void RefreshMapping(IDTSInput100 input, AttributeMetadata[] currentMetadata, AttributeMetadata[] newMetadata, int Operation)
        {

            List<MappingItem> removedItemFromMetadata = new List<MappingItem>();
            List<MappingItem> AddedItemFromMetadata = new List<MappingItem>();

            bool bexists;


            //First Check: Removed item from input collection, so remove from mapping
            foreach (MappingItem m in columnList)
            {
                bexists = false;

                foreach (IDTSInputColumn100 inputcol in input.InputColumnCollection)
                {
                    if (inputcol.Name == m.ExternalColumnName)
                    {
                        bexists = true;
                    }

                }
                if (!bexists)
                {
                    m.ExternalColumnName = "";
                    m.ExternalColumnType = DataType.DT_EMPTY;
                    m.ExternalColumnTypeName = "";
                }
            }

            // Second Check:
            // Added External Input Columns should appear automatically when grid renders


            // Third Check:If Internal Columns does not exist anymore

            foreach (MappingItem mi in columnList)
            {
                bexists = false;

                foreach (AttributeMetadata atr in newMetadata)
                {
                    if (mi.InternalColumnName == atr.LogicalName)
                    {
                        bexists = true;
                    }
                }

                if (!bexists)
                {
                    removedItemFromMetadata.Add(mi);
                }
            }

            foreach (MappingItem mi in removedItemFromMetadata) columnList.Remove(mi);

            // Fourth Check: Added Internal Columns

            foreach (AttributeMetadata atr in newMetadata)
            {
                bexists = false;

                foreach (AttributeMetadata atrcurrent in currentMetadata)
                {
                    if (atrcurrent.LogicalName == atr.LogicalName)
                    {
                        bexists = true;
                    }
                }

                if (!bexists)
                {
                    AddSupportedMappingItem(atr, input, Operation);
                }

            }

        }

        }
    }
