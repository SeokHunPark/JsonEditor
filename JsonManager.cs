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
        COUNT
    }

    class JsonManager
    {
        public static JsonObjectCollection LoadJsonFile(string filePath)
        {
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
            JsonObject jsonObject = parser.Parse(jsonString);
            JsonObjectCollection json = jsonObject as JsonObjectCollection;

            return json;
        }

        public static string JsonToString(JsonObjectCollection json)
        {
            return json.ToString();
        }

        public static TreeNode JsonToTreeNode(JsonObjectCollection json)
        {
            TreeNode rootNode = new TreeNode("Root");
            rootNode.Tag = json;

            for (int i = 0; i < json.Count; i++)
            {
                string key = json[i].Name.ToString();
                TreeNode childNode = null;
                if (json[i].GetType() == typeof(JsonObjectCollection))
                {
                    childNode = new TreeNode(key + " : ");
                    childNode.Tag = json[i];
                    List<JsonObject> listJsonObject = json[i].GetValue() as List<JsonObject>;
                    foreach (var item in listJsonObject)
                    {
                        string k = item.Name;
                        string v = item.GetValue().ToString();
                        TreeNode cn = new TreeNode(k + " : " + v);
                        cn.Tag = item;
                        childNode.Nodes.Add(cn);
                    }
                }
                else
                {
                    string value = json[i].GetValue().ToString();
                    childNode = new TreeNode(key + " : " + value);
                    childNode.Tag = json[i];
                }
                rootNode.Nodes.Add(childNode);
            }

            return rootNode;
        }

        public static JsonObjectCollection JsonTreeNodeToJsonCollection(TreeNode treeNode)
        {
            JsonObject nodeJsonObject = treeNode.Tag as JsonObject;
            JsonObjectCollection resultCollection = new JsonObjectCollection(nodeJsonObject.Name);

            for (int i = 0; i < treeNode.Nodes.Count; i++ )
            {
                JsonObject childJsonObject = treeNode.Nodes[i].Tag as JsonObject;
                
                if (childJsonObject.GetType() == typeof(JsonObjectCollection))
                {
                    childJsonObject = null;
                    childJsonObject = JsonTreeNodeToJsonCollection(treeNode.Nodes[i]);
                    resultCollection.Add(childJsonObject);
                }
                else
                {
                    resultCollection.Add(childJsonObject);
                }
            }

            return resultCollection;
        }
    }
}
