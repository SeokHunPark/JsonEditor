using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Json;
using System.IO;
using System.Windows.Forms;

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

        public TreeNode JsonObjectToTreeNode(JsonObject json)
        {
            TreeNode treeNode = new TreeNode();
            string key = json.Name;
            treeNode.Tag = json;

            if (json.GetType() == typeof(JsonArrayCollection))
            {
                treeNode.Text = key;
                List<JsonObject> jsonList = json.GetValue() as List<JsonObject>;
                for (int i = 0; i < jsonList.Count; i++)
                {
                    TreeNode chlidNode = JsonObjectToTreeNode(jsonList[i]);
                    treeNode.Nodes.Add(chlidNode);
                }
            }
            else if (json.GetType() == typeof(JsonObjectCollection))
            {
                treeNode.Text = key;
                List<JsonObject> jsonList = json.GetValue() as List<JsonObject>;
                for (int i = 0; i < jsonList.Count; i++)
                {
                    TreeNode chlidNode = JsonObjectToTreeNode(jsonList[i]);
                    treeNode.Nodes.Add(chlidNode);
                }
            }
            else if (json.GetType() == typeof(JsonStringValue))
            {
                string value = json.GetValue().ToString();
                treeNode.Text = key + " : " + value;
            }
            else if (json.GetType() == typeof(JsonNumericValue))
            {
                string value = json.GetValue().ToString();
                treeNode.Text = key + " : " + value;
            }
            else if (json.GetType() == typeof(JsonBooleanValue))
            {
                string value = json.GetValue().ToString();
                treeNode.Text = key + " : " + value;
            }

            return treeNode;
        }

        public JsonObjectCollection JsonTreeNodeToJsonCollection(TreeNode treeNode)
        {
            JsonObject nodeJsonObject = treeNode.Tag as JsonObject;
            JsonObjectCollection resultCollection = new JsonObjectCollection(nodeJsonObject.Name);

            for (int i = 0; i < treeNode.Nodes.Count; i++)
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
