using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Json;

namespace JsonEditor
{
    public partial class mainForm : Form
    {
        private JsonEditor _jsonEditor;
        private string _filePath;
        //private JsonObjectCollection _loadedJson;
        private TreeNode _rootNode;
        private TreeNode _selectedNode;

        // constructor
        public mainForm()
        {
            InitializeComponent();
            
        }

        private void Initialize()
        {
            _jsonEditor = new JsonEditor();
            _filePath = "";
            //_loadedJson = null;
            _rootNode = null;
            _selectedNode = null;

            //string[] comboBoxList = { "Json", "int", "float", "double", "string" };
            string[] comboBoxList = new string[(int)ValueType.COUNT];
            comboBoxList[(int)ValueType.Json] = "Json";
            comboBoxList[(int)ValueType.Integer] = "Integer";
            comboBoxList[(int)ValueType.Float] = "Float";
            comboBoxList[(int)ValueType.Double] = "Double";
            comboBoxList[(int)ValueType.String] = "String";
            comboBoxList[(int)ValueType.Boolean] = "Boolean";
            typeComboBox.Items.AddRange(comboBoxList);
            typeComboBox.SelectedIndex = 0;

            openFileDialog1.Filter = "Json 파일(*.json)|*.json";
            openFileDialog1.FileName = "";
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            Initialize();

            // @Test
            //_loadedJson = null;
            //_loadedJson = _jsonEditor.LoadJson("D:\\WORK\\JsonEditor\\game_item.json");
            //_filePath = "D:\\WORK\\JsonEditor\\game_item.json";

            //ViewOnTextBox(_loadedJson);
            //ViewOnTreeView(_loadedJson);
        }

        private void jsonTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = jsonTreeView.GetNodeAt(e.Location);
                _selectedNode = null;
                _selectedNode = node;

                JsonObject jsonObject = node.Tag as JsonObject;
                keyTextBox.Text = jsonObject.Name;
                //Console.WriteLine(jsonObject.GetType().ToString());

                if (jsonObject.GetValue().GetType() == typeof(List<JsonObject>))
                {
                    valueTextBox.Text = "JsonObject";
                    typeComboBox.SelectedIndex = (int)ValueType.Json;
                }
                else if (jsonObject.GetValue().GetType() == typeof(System.Int32))
                {
                    valueTextBox.Text = jsonObject.GetValue().ToString();
                    typeComboBox.SelectedIndex = (int)ValueType.Integer;
                }
                else if (jsonObject.GetValue().GetType() == typeof(System.Double))
                {
                    valueTextBox.Text = jsonObject.GetValue().ToString();
                    typeComboBox.SelectedIndex = (int)ValueType.Double;
                }
                else if (jsonObject.GetValue().GetType() == typeof(System.String))
                {
                    valueTextBox.Text = jsonObject.GetValue().ToString();
                    typeComboBox.SelectedIndex = (int)ValueType.String;
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {

            if (_selectedNode == null)
                return;

            if (_selectedNode.Tag.GetType() == typeof(JsonObjectCollection))
                return;

            string key = keyTextBox.Text;
            var value = valueTextBox.Text;

            try
            {
                UpdateTreeNode(key, value);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "예외 발생", MessageBoxButtons.OK);
                return;
            }

            JsonObjectCollection json = JsonManager.JsonTreeNodeToJsonCollection(_rootNode);
            ViewOnTextBox(json);
            ViewOnTreeView(json);

            _jsonEditor.SaveJson(json, _filePath);
        }

        private void UpdateTreeNode(string key, string value)
        {
            JsonObject jsonObject = null;
            if (typeComboBox.SelectedItem.ToString() == "String")
            {
                jsonObject = new JsonStringValue(key, value);
            }
            else if (typeComboBox.SelectedItem.ToString() == "Integer")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToInt32(value));
            }
            else if (typeComboBox.SelectedItem.ToString() == "Float")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToSingle(value));
            }
            else if (typeComboBox.SelectedItem.ToString() == "Double")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToDouble(value));
            }
            else if (typeComboBox.SelectedItem.ToString() == "Boolean")
            {
                jsonObject = new JsonBooleanValue(key, Convert.ToBoolean(value));
            }
            _selectedNode.Tag = jsonObject;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // @Test
            //JsonObjectCollection collection = new JsonObjectCollection();

            //JsonStringValue stringValue = new JsonStringValue("first", "첫번째");
            //collection.Add(stringValue);

            //JsonArrayCollection arrayValue = new JsonArrayCollection("JsonArrayCollection");
            //JsonStringValue stringValue2 = new JsonStringValue("second", "두번째");
            //arrayValue.Add(stringValue2);
            //collection.Add(arrayValue);

            //JsonObjectCollection collectionValue = new JsonObjectCollection("JsonObjectCollection");
            //JsonNumericValue numericValue = new JsonNumericValue("third", 3.0);
            //collectionValue.Add(numericValue);
            //collection.Add(collectionValue);

            //ViewOnTextBox(collection);
            //ViewOnTreeView(collection);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //_loadedJson = null;
                //_loadedJson = _jsonEditor.LoadJson("D:\\WORK\\JsonEditor\\game_item.json");
                JsonObjectCollection loadedJson = _jsonEditor.LoadJson(openFileDialog1.FileName);
                _filePath = openFileDialog1.FileName;

                ViewOnTextBox(loadedJson);
                ViewOnTreeView(loadedJson);
            }
        }

        private void ViewOnTextBox(JsonObjectCollection json)
        {
            string jsonString = JsonManager.JsonToString(json);
            jsonTextBox.Text = jsonString;
        }

        private void ViewOnTreeView(JsonObjectCollection json)
        {
            jsonTreeView.Nodes.Clear();

            TreeNode tree = JsonManager.JsonToTreeNode(json);
            jsonTreeView.Nodes.Add(tree);
            jsonTreeView.ExpandAll();
            jsonTreeView.SelectedNode = jsonTreeView.Nodes[0];

            _rootNode = tree;
        }
    }
}
