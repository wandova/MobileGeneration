using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileGeneration
{
    public partial class MainForm : Form
    {
        private readonly MainService mainService;
        public MainForm()
        {
            InitializeComponent();
            this.Text += $" v{Assembly.GetExecutingAssembly().GetName().Version}";
            mainService = new MainService();
            this.treeView1.AfterCheck += TreeView_AllCheck;
            this.treeView2.AfterCheck += TreeView_AllCheck;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.treeView1.CheckBoxes = true;
            this.treeView1.Nodes.AddRange(mainService.GetIspPrefixTreeNodes());
            this.treeView2.CheckBoxes = true;
            this.treeView2.Nodes.AddRange(mainService.GetProvinceCityTreeNodes());
        }

        private void TreeView_AllCheck(object sender, TreeViewEventArgs e)
        {
            Task.Run(() =>
            {
                CalcFileSize();
            });
            if (e.Action == TreeViewAction.Unknown)
            {
                return;
            }
            var parentNode = e.Node.Parent;
            if (parentNode != null)
            {
                var i = 0;
                foreach (TreeNode item in parentNode.Nodes)
                {
                    if (item.Checked)
                    {
                        i++;
                    }
                }
                if (i == parentNode.Nodes.Count)
                {
                    parentNode.Checked = true;
                }
                else
                {
                    parentNode.Checked = false;
                }
            }
            else
            {
                foreach (TreeNode item in e.Node.Nodes)
                {
                    item.Checked = e.Node.Checked;
                }
            }
        }

        private CityCode[] GetCheckedCityCodes()
        {
            var allCheckedNodes = new List<string>();
            TreeNodeCollection treeView1Nodes = null;
            TreeNodeCollection treeView2Nodes = null;
            this.treeView1.Invoke(new Action(() => {
                treeView1Nodes = this.treeView1.Nodes;
                treeView2Nodes = this.treeView2.Nodes;
            }));
            foreach (TreeNode nodes in treeView1Nodes)
            {
                foreach (TreeNode node in nodes.Nodes)
                {
                    if (node.Checked)
                    {
                        allCheckedNodes.Add(node.Name);
                    }
                }
            }
            foreach (TreeNode nodes in treeView2Nodes)
            {
                foreach (TreeNode node in nodes.Nodes)
                {
                    if (node.Checked)
                    {
                        allCheckedNodes.Add(node.Name);
                    }
                }
            }
            return mainService.CityCodes.Where(a => allCheckedNodes.Contains(a.Prefix) && allCheckedNodes.Contains(a.City)).ToArray();
        }

        private void CalcFileSize()
        {
            var cityCodes = GetCheckedCityCodes();
            this.label1.BeginInvoke(new Action(() => {
                var totalByte = cityCodes.Length * 129998L;
                this.label1.Text = $"{totalByte}B";
                var size = totalByte / (float)1024;
                if (size > 1)
                {
                    this.label1.Text = $"{size.ToString("0.##")}KB";
                }
                size = totalByte / (float)1024 / 1024;
                if (size > 1)
                {
                    this.label1.Text = $"{size.ToString("0.##")}MB";
                }
                size = totalByte / (float)1024 / 1024 / 1024;
                if (size > 1)
                {
                    this.label1.Text = $"{size.ToString("0.##")}GB";
                }
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "保存";
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Task.Run(() => {
                var cityCodes = GetCheckedCityCodes();
                if (cityCodes.Length > 0)
                {
                    using (var streamWriter = new StreamWriter(saveFileDialog.FileName, true, Encoding.UTF8))
                    {
                        for (int i = 0; i < cityCodes.Length; i++)
                        {
                            for (int j = 0; j < 10000; j++)
                            {
                                var sup = "";
                                if (j < 1000)
                                {
                                    sup = (4 - j.ToString().Length).ToString();
                                    sup = sup.Replace("1", "0").Replace("2", "00").Replace("3", "000");
                                }
                                sup = $"{cityCodes[i].Code}{j}{sup}\r\n";
                                streamWriter.Write(sup);
                            }
                            this.progressBar1.BeginInvoke(new Action(() => {
                                this.progressBar1.Value = (int)(i / (float)cityCodes.Length * 100);
                            }));
                        }
                    }
                    this.BeginInvoke(new Action(() => {
                        MessageBox.Show("导出成功");
                    }));
                }
            });
        }
    }
}