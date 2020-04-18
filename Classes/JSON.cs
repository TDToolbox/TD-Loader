using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TD_Loader.Classes
{
    class JSON
    {
        public static bool IsJsonValid(string json)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                dynamic result = serializer.DeserializeObject(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
