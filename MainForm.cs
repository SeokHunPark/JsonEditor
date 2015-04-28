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
            _rootNode = null;
            _selectedNode = null;

            AddNodeForm addNodeForm = AddNodeForm.GetInstance();
            addNodeForm.sendNodeInfo += new AddNodeForm.sendNodeInfoDelegate(AddNode);

            //string[] comboBoxList = { "Json", "int", "float", "double", "string" };
            string[] comboBoxList = new string[(int)ValueType.COUNT];
            comboBoxList[(int)ValueType.Json] = ValueType.Json.ToString();
            comboBoxList[(int)ValueType.Integer] = ValueType.Integer.ToString();
            comboBoxList[(int)ValueType.Float] = ValueType.Float.ToString();
            comboBoxList[(int)ValueType.Double] = ValueType.Double.ToString();
            comboBoxList[(int)ValueType.String] = ValueType.String.ToString();
            comboBoxList[(int)ValueType.Boolean] = ValueType.Boolean.ToString();
            comboBoxList[(int)ValueType.List] = ValueType.List.ToString();
            typeComboBox.Items.AddRange(comboBoxList);
            typeComboBox.SelectedIndex = 0;

            openFileDialog1.Filter = "Json 파일(*.json)|*.json";
            openFileDialog1.FileName = "";
        }

        private void AddNode(string key, string valueType, string value)
        {
            //Console.WriteLine(key + " " + valueType + " " + value);

            TreeNode treeNode = new TreeNode();
            string textString = key;
            JsonObject jsonObject = null;
            if (valueType == "Json")
            {
                jsonObject = new JsonObjectCollection();
            }
            else if (valueType == "List")
            {
                jsonObject = new JsonArrayCollection();
            }
            else if (valueType == "String")
            {
                jsonObject = new JsonStringValue(key, value);
                textString += " : ";
                textString += value;
            }
            else if (valueType == "Integer")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToInt32(value));
                textString += " : ";
                textString += value;
            }
            else if (valueType == "Float")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToSingle(value));
                textString += " : ";
                textString += value;
            }
            else if (valueType == "Double")
            {
                jsonObject = new JsonNumericValue(key, Convert.ToDouble(value));
                textString += " : ";
                textString += value;
            }
            else if (valueType == "Boolean")
            {
                jsonObject = new JsonBooleanValue(key, Convert.ToBoolean(value));
                textString += " : ";
                textString += value;
            }
            treeNode.Text = textString;
            treeNode.Tag = jsonObject;

            _selectedNode.Nodes.Add(treeNode);

            JsonObjectCollection json = _jsonEditor.JsonTreeNodeToJsonCollection(_rootNode);
            ViewOnTextBox(json);
            //ViewOnTreeView(json);

            //_jsonEditor.SaveJson(json, _filePath);
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
                if (node == null)
                    return;

                _selectedNode = null;
                _selectedNode = node;

                JsonObject jsonObject = node.Tag as JsonObject;
                keyTextBox.Text = jsonObject.Name;
                valueTextBox.Enabled = true;
                //Console.WriteLine(jsonObject.GetType().ToString());

                if (jsonObject.GetType() == typeof(JsonArrayCollection))
                {
                    valueTextBox.Text = "";
                    typeComboBox.SelectedIndex = (int)ValueType.List;
                }
                if (jsonObject.GetType() == typeof(JsonObjectCollection))
                {
                    valueTextBox.Text = "";
                    typeComboBox.SelectedIndex = (int)ValueType.Json;
                }
                else if (jsonObject.GetType() == typeof(JsonNumericValue))
                {
                    if (jsonObject.GetValue().GetType() == typeof(System.Double))
                    {
                        valueTextBox.Text = jsonObject.GetValue().ToString();
                        typeComboBox.SelectedIndex = (int)ValueType.Double;
                    }
                    else if (jsonObject.GetValue().GetType() == typeof(System.Int32))
                    {
                        valueTextBox.Text = jsonObject.GetValue().ToString();
                        typeComboBox.SelectedIndex = (int)ValueType.Integer;
                    }
                    else if (jsonObject.GetValue().GetType() == typeof(System.Single))
                    {
                        valueTextBox.Text = jsonObject.GetValue().ToString();
                        typeComboBox.SelectedIndex = (int)ValueType.Float;
                    }
                }
                else if (jsonObject.GetType() == typeof(JsonStringValue))
                {
                    valueTextBox.Text = jsonObject.GetValue().ToString();
                    typeComboBox.SelectedIndex = (int)ValueType.String;
                }
                else if (jsonObject.GetType() == typeof(JsonBooleanValue))
                {
                    valueTextBox.Text = jsonObject.GetValue().ToString();
                    typeComboBox.SelectedIndex = (int)ValueType.Boolean;
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (_selectedNode == null)
                return;

            string key = keyTextBox.Text;
            string value = "";

            if (_selectedNode.Tag.GetType() != typeof(JsonObjectCollection) || _selectedNode.Tag.GetType() != typeof(JsonArrayCollection))
            {
                value = valueTextBox.Text;
            }

            try
            {
                //UpdateTreeNodeJson(_selectedNode, key, value);
                _selectedNode.Tag = JsonManager.CreateJsonObject(key, value, typeComboBox.SelectedItem.ToString());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "예외 발생", MessageBoxButtons.OK);
                return;
            }

            JsonObjectCollection json = _jsonEditor.JsonTreeNodeToJsonCollection(_rootNode);
            ViewOnTextBox(json);
            ViewOnTreeView(json);

            _jsonEditor.SaveJson(json, _filePath);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {

            if (_rootNode == null)
                return;

            if (_selectedNode == null)
                return;

            _rootNode.Nodes.Remove(_selectedNode);

            JsonObjectCollection json = _jsonEditor.JsonTreeNodeToJsonCollection(_rootNode);
            ViewOnTextBox(json);
            ViewOnTreeView(json);

            _jsonEditor.SaveJson(json, _filePath);

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

        private void addButton_Click(object sender, EventArgs e)
        {
            if (_rootNode == null)
                return;

            if (_selectedNode == null)
                return;

            string key = keyTextBox.Text;
            var value = valueTextBox.Text;

            TreeNode childNode = new TreeNode();
            childNode.Text = key + " : " + value;
            childNode.Tag = JsonManager.CreateJsonObject(keyTextBox.Text, valueTextBox.Text, typeComboBox.SelectedItem.ToString());
            _selectedNode.Nodes.Add(childNode);

            JsonObjectCollection json = _jsonEditor.JsonTreeNodeToJsonCollection(_rootNode);
            ViewOnTextBox(json);
            ViewOnTreeView(json);

            _jsonEditor.SaveJson(json, _filePath);

            //AddNodeForm addNodeWindow = AddNodeForm.GetInstance();
            //addNodeWindow.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //_loadedJson = null;
                //_loadedJson = _jsonEditor.LoadJson("D:\\WORK\\JsonEditor\\game_item.json");
                JsonObjectCollection loadedJson = _jsonEditor.LoadJson(openFileDialog1.FileName);

                if (loadedJson == null)
                {
                    MessageBox.Show("잘못된 Json 형식입니다.", "Error", MessageBoxButtons.OK);
                    return;
                }

                _filePath = openFileDialog1.FileName;

                ViewOnTextBox(loadedJson);
                ViewOnTreeView(loadedJson);

            }
        }

        private void ViewOnTextBox(JsonObjectCollection json)
        {
            string jsonString = "";
            try
            {
                jsonString = JsonManager.JsonToString(json);
            }
            catch (Exception exception)
            {
                MessageBox.Show("잘못된 Json 형식입니다.", "Error", MessageBoxButtons.OK);
                Console.WriteLine(exception.Message);
                //jsonTextBox.Text = "잘못된 Json 형식 입니다.";
                return;
            }
            jsonTextBox.Text = jsonString;
        }

        private void ViewOnTreeView(JsonObjectCollection json)
        {
            jsonTreeView.Nodes.Clear();

            TreeNode tree = _jsonEditor.JsonObjectToTreeNode(json);
            jsonTreeView.Nodes.Add(tree);
            jsonTreeView.ExpandAll();
            jsonTreeView.SelectedNode = jsonTreeView.Nodes[0];

            _rootNode = tree;
        }

        private void ViewOnTreeView(TreeNode tree)
        {
            jsonTreeView.Nodes.Clear();

            jsonTreeView.Nodes.Add(tree);
            jsonTreeView.ExpandAll();
            jsonTreeView.SelectedNode = jsonTreeView.Nodes[0];

            _rootNode = tree;
        }
    }
}
