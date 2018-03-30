using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRMSSIS.CRMCommon
{
   public static class JSONSerialization
    {
        public static T Deserialize<T>(this string toDeserialize)
        {
            
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(toDeserialize));
            Type typeParameterType = typeof(T);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeParameterType);
           
            return (T)ser.ReadObject(ms);
          
        }

        public static string Serialize<T>(this T toSerialize)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream1, toSerialize);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();
        }
    }
}
