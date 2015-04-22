using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Json;
using System.IO;

namespace JsonEditor
{
    class JsonEditor
    {
        public JsonEditor()
        {
            
        }

        public JsonObjectCollection LoadJson(string filePath)
        {
            JsonObjectCollection json = JsonManager.LoadJsonFile(filePath);

            return json;
        }

        public void SaveJson(JsonObjectCollection json, string filePath)
        {
            if (!File.Exists(filePath))
                return;

            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                json.WriteTo(sw);
                sw.Close();
            }
        }
    }
}
