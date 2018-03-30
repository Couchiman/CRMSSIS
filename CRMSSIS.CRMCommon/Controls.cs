using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMCommon.Controls
{


    #region Items for Combos

    [DataContract(Name = "Item")]
    public class Item
        {
       
        private string value;
        private string text;
       
        private AttributeMetadata[] metadata;
        [DataMember(Name = "Value")]
        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
        [DataMember(Name = "Text")]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }
        [DataMember]
        public AttributeMetadata[] Metadata
        {
            get
            {
                return metadata;
            }

            set
            {
                metadata = value;
            }
        }

        public Item(string text, string val)
            {
                Value = val;
                Text = text;
            }
       
        public Item(string text, string val, AttributeMetadata[] array)
            {
                Value = val;
                Text = text;
                Metadata = array;

            }
        
            public override string ToString()
            {
                return Text;
            }
         
        #endregion
    }
}
