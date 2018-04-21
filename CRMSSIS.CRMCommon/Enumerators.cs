using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRMSSIS.CRMCommon.Enumerators
{

    public enum Operations
    {

        //Upsert Operation
        [Description("Create")]
        Create = 0,
        //
        //Update Operation
        [Description("Update")]
        Update = 1,
        //Delete Operation
        [Description("Delete")]
        Delete = 2,
        [Description("Status")]
        Status = 3,
        [Description("Upsert")]
        Upsert = 4,
        //Execute workflow operation
        [Description("Workflow")]
        Workflow = 5


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

    public enum SupportedLanguages
    {
        [Description("Invariant")]
        Invariant = 127,
        [Description("Arabic")]
        Arabic = 1025,
        [Description("Basque (Basque)")]
        Basque = 1069,
        [Description("Bulgarian (Bulgaria)")]
        Bulgarian = 1026,
        [Description("Catalan (Catalan)")]
        Catalan = 1027,
        [Description("Chinese (Hong Kong S.A.R.)")]
        ChineseSAR = 3076,
        [Description("Chinese (PRC)")]
        ChinesePRC = 2052,
        [Description("Chinese (Taiwan)")]
        ChineseTW = 1028,
        [Description("Croatian (Croatia)")]
        Croatian = 1050,
        [Description("Czech")]
        Czech = 1029,
        [Description("Danish")]
        Danish = 1030,
        [Description("Dutch")]
        Dutch = 1043,
        [Description("English")]
        English = 1033,
        [Description("Estonian (Estonia)")]
        Estonian = 1061,
        [Description("Finnish")]
        Finnish = 1035,
        [Description("French")]
        French = 1036,
        [Description("Galician (Galician)")]
        Galician = 1110,
        [Description("German")]
        German = 1031,
        [Description("Greek")]
        Greek = 1032,
        [Description("Hebrew")]
        Hebrew = 1037,
        [Description("Hindi (India)")]
        Hindi = 1081,
        [Description("Hungarian")]
        Hungarian = 1038,
        [Description("Indonesian")]
        Indonesian = 1057,
        [Description("Italian")]
        Italian = 1040,
        [Description("Japanese")]
        Japanese = 1041,
        [Description("Kazakh (Kazakhstan)")]
        Kazakh = 1087,
        [Description("Korean")]
        Korean = 1042,
        [Description("Latvian (Latvia)")]
        Latvian = 1062,
        [Description("Lithuanian (Lithuania)")]
        Lithuanian = 1063,
        [Description("Malay")]
        Malay = 1086,
        [Description("Norwegian (Bokmal)")]
        Norwegian = 1044,
        [Description("Polish")]
        Polish = 1045,
        [Description("Portuguese (Brazil)")]
        PortugueseBR = 1046,
        [Description("Portuguese (Portugal)")]
        PortuguesePT = 2070,
        [Description("Romanian (Romania)")]
        Romanian = 1048,
        [Description("Russian")]
        Russian = 1049,
        [Description("Serbian (Cyrillic)")]
        SerbianCY = 3098,
        [Description("Serbian (Latin, Serbia)")]
        SerbianLT = 2074,
        [Description("Slovak (Slovakia)")]
        Slovak = 1051,
        [Description("Slovenian (Slovenia)")]
        Slovenian = 1060,
        [Description("Spanish")]
        Spanish = 3082,
        [Description("Swedish")]
        Swedish = 1053,
        [Description("Thai")]
        Thai = 1054,
        [Description("Turkish")]
        Turkish = 1055,
        [Description("Ukrainian (Ukraine)")]
        Ukrainian = 1058,
        [Description("Vietnamese")]
        Vietnamese = 1066,

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

        public static string GetDescriptionFromValue<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }

}
