using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Json;
using System.Windows.Forms;

namespace JsonEditor
{
    enum ValueType
    {
        Json = 0,
        Integer,
        Float,
        Double,
        String,
        Boolean,
        List,
        COUNT
    }

    class JsonManager
    {
        public static JsonObjectCollection LoadJsonFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string jsonString = streamReader.ReadToEnd();
                JsonObjectCollection json = StringToJson(jsonString);

                return json;
            }
        }

        public static JsonObjectCollection StringToJson(string jsonString)
        {
            JsonTextParser parser = new JsonTextParser();
            JsonObject jsonObject = null;
            try 
            {
                jsonObject = parser.Parse(jsonString);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
            JsonObjectCollection json = jsonObject as JsonObjectCollection;

            return json;
        }

        public static string JsonToString(JsonObjectCollection json)
        {
            return json.ToString();
        }
    }
}
