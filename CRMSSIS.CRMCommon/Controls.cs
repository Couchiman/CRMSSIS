using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMCommon.Controls
{
    

        #region Items for Combos

        public class Item
        {
            public string Value;
            public string Text;
            public AttributeMetadata[] Metadata;

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
