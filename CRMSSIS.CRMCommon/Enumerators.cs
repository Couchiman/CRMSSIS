using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMCommon.Enumerators
{
    
        public enum Operations
        {
       
         //Upsert Operation
         [Description("Bulk Create")]
        Create = 0,
        //
        //Delete Operation
        [Description("Bulk Update")]
        Update = 1,
        //Delete Operation
        [Description("Bulk Delete")]
        Delete = 2
      
        
        }


    public enum AuthenticationProviderTypeDescriptive
    {

        //
        // Resumen:
        //     An Active Directory identity provider. Value = 1.
        [Description("Active Directory")]
        ActiveDirectory = 0,
        //
        // Resumen:
        //     A federated claims identity provider. Value = 2.
        [Description("IFD")]
        IFD = 1,
        //
        // Resumen:
        //     A Microsoft account identity provider. Value = 3.
        [Description("Office 365")]
        Office365 = 2,

    }

    public static class EnumEx
    {
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }
    }

}
