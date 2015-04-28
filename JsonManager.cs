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
                
                //string[] stringArray = filePath.Split('\\');
                //json.Name = stringArray[stringArray.Length - 1];
                //Console.WriteLine(stringArray[stringArray.Length - 1]);

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

        public static JsonObject CreateJsonObject(string key, string value, string valueType)
        {
            JsonObject jsonObject = null;
            if (valueType == "Json")
            {
                jsonObject = new JsonObjectCollection();
                jsonObject.Name = key;
            }
            else if (valueType == "List")
            {
                jsonObject = new JsonArrayCollection();
                jsonObject.Name = key;
            }
            else if (valueType == "String")
            {
                jsonObject = new JsonStringValue(key, value);
            }
            else if (valueType == "Integer")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToInt32(value));
            }
            else if (valueType == "Float")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToSingle(value));
            }
            else if (valueType == "Double")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToDouble(value));
            }
            else if (valueType == "Boolean")
            {
                jsonObject = new JsonBooleanValue(key, Convert.ToBoolean(value));
            }

            return jsonObject;
        }
    }
}
