using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathHotfix
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.comboBox2.SelectedIndex = 0;
            this.WalkDir();
            this.treeView1.ExpandAll();
        }

        /// <summary>
        /// 构建文件树
        /// </summary>
        private void WalkDir()
        {
            TreeNode treeNode = new TreeNode("文件");
            treeNode.ImageIndex = 0;
            treeNode.SelectedImageIndex = 1;
            this.treeView1.Nodes.Add(treeNode);
            this.GetFile(Application.StartupPath, treeNode);
        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="parent"></param>
        private void GetFile(string dir, TreeNode parent)
        {
            Array.ForEach<string>(Directory.GetDirectories(dir), delegate (string x)
            {
                int num = x.LastIndexOf('\\');
                TreeNode treeNode = new TreeNode(x.Substring(num + 1));
                treeNode.ImageIndex = 0;
                treeNode.SelectedImageIndex = 1;
                parent.Nodes.Add(treeNode);
                this.GetFile(x, treeNode);
            });
            Array.ForEach<string>(Directory.GetFiles(dir), delegate (string x)
            {
                string extension = Path.GetExtension(x);
                int num = x.LastIndexOf('\\');
                TreeNode treeNode = new TreeNode(x.Substring(num + 1));
                Form1.FileInfo fileInfo = new Form1.FileInfo();
                fileInfo.FileName = x.Replace(Application.StartupPath + "\\", "");
                treeNode.Tag = fileInfo;
                //if (fileInfo.FileName == AppDomain.CurrentDomain.SetupInformation.ApplicationName || fileInfo.FileName == FileName.Version || fileInfo.FileName == FileName.FileJson)
                //{
                //    return;
                //}
                parent.Nodes.Add(treeNode);
                if (this.imageList1.Images.ContainsKey(extension))
                {
                    treeNode.ImageIndex = this.imageList1.Images.IndexOfKey(extension);
                    TreeNode treeNode2 = treeNode;
                    treeNode2.SelectedImageIndex = treeNode2.ImageIndex;
                    return;
                }
                Icon fileIcon = this.GetFileIcon(x, false);
                this.imageList1.Images.Add(extension, fileIcon);
                int num2 = this.imageList1.Images.IndexOfKey(extension);
                if (num2 < 0)
                {
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex = 2;
                    return;
                }
                treeNode.ImageIndex = num2;
                TreeNode treeNode3 = treeNode;
                treeNode3.SelectedImageIndex = treeNode3.ImageIndex;
            });
        }

        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo(string FileName, uint FileAttributes, out Form1.SHFILEINFO FileInfo, uint fileInfo, Form1.SHGFI uFlags);

        /// <summary>
        /// 获取文件图标
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="largeIcon"></param>
        /// <returns></returns>
        private Icon GetFileIcon(string fileName, bool largeIcon)
        {
            Form1.SHFILEINFO shfileinfo = new Form1.SHFILEINFO(true);
            int fileInfo = Marshal.SizeOf(shfileinfo);
            Form1.SHGFI uFlags;
            if (largeIcon)
            {
                uFlags = (Form1.SHGFI)272;
            }
            else
            {
                uFlags = (Form1.SHGFI)273;
            }
            Form1.SHGetFileInfo(fileName, 256U, out shfileinfo, (uint)fileInfo, uFlags);
            return Icon.FromHandle(shfileinfo.hIcon);
        }

        /// <summary>
        /// 开始制作文件更新模式信息
        /// </summary>
        private void CreateFile()
        {
            int num = 0;
            this.treeNodes.Clear();
            this.GetInfo(this.treeView1.Nodes[0]);
            if (File.Exists(FileName.FileJson))
            {
                File.Delete(FileName.FileJson);
            }
            using (FileStream fileStream = new FileStream(FileName.FileJson, FileMode.CreateNew))
            {
                fileStream.Write(Encoding.UTF8.GetBytes("["), 0, 1);
                foreach (string text in this.treeNodes)
                {
                    num++;
                    if (num == this.treeNodes.Count)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(text.Replace("\\", "\\\\"));
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        byte[] bytes2 = Encoding.UTF8.GetBytes(text.Replace("\\", "\\\\") + ",");
                        fileStream.Write(bytes2, 0, bytes2.Length);
                    }
                }
                fileStream.Write(Encoding.UTF8.GetBytes("]"), 0, 1);
                fileStream.Flush();
                fileStream.Dispose();
            }
        }

        private void GetInfo(TreeNode parent)
        {
            foreach (object obj in parent.Nodes)
            {
                TreeNode treeNode = (TreeNode)obj;
                Form1.FileInfo fileInfo = treeNode.Tag as Form1.FileInfo;
                if (fileInfo != null)
                {
                    this.treeNodes.Add(string.Concat(new object[]
                    {
                        "{\"FileName\":\"",
                        fileInfo.FileName,
                        "\",\"OperatorType\":",
                        fileInfo.Operator,
                        "}"
                    }));
                }
                this.GetInfo(treeNode);
            }
        }

        /// <summary>
        /// 生成版本信息
        /// </summary>
        /// <returns></returns>
        private string CreateUpdateFile()
        {
            int num = this.radioButton1.Checked ? 1 : 0;
            string text = string.Concat(new string[]
            {
                Path.GetFileName(this.textBox7.Text).Replace(".exe", ""),
                "-",
                this.textBox5.Text,
                "-",
                this.textBox6.Text,
                "-",
                DateTime.Now.ToString("yyyyMMdd"),
                ".ph"
            });
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\"Version\":");
            stringBuilder.Append(string.Concat(new string[]
            {
                "\"",
                this.textBox5.Text.Trim(),
                "-",
                this.textBox6.Text.Trim(),
                "\","
            }));
            stringBuilder.Append("\"App\":");
            stringBuilder.Append("\"" + Path.GetFileName(this.textBox7.Text).Replace(".exe", "") + "\",");
            stringBuilder.Append("\"Date\":");
            stringBuilder.Append("\"" + DateTime.Now.ToString("yyyyMMdd") + "\",");
            stringBuilder.Append("\"MD5\":");
            stringBuilder.Append("\"" + string.Empty + "\",");
            stringBuilder.Append("\"PatchName\":");
            stringBuilder.Append("\"" + text + "\",");
            stringBuilder.Append("\"ClientName\":");
            stringBuilder.Append("\"" + this.textBox1.Text + "\",");
            stringBuilder.Append("\"Reason\":");
            stringBuilder.Append("\"" + textBox4.Text + ";" + this.textBox2.Text.Replace('\r', ' ').Replace('\n', '@') + "\",");
            stringBuilder.Append("\"UpdateDesc\":");
            stringBuilder.Append("\"" + this.textBox3.Text.Replace('\r', ' ').Replace('\n', '@') + "\",");
            stringBuilder.Append("\"Model\":");
            stringBuilder.Append(num + ",");
            stringBuilder.Append("\"UpdateApp\":");
            stringBuilder.Append("false,");
            stringBuilder.Append("\"PatchSize\":");
            stringBuilder.Append("0,");
            stringBuilder.Append("\"Server\":");
            stringBuilder.Append("\"" + string.Empty + "\",");
            stringBuilder.Append("\"UserName\":");
            stringBuilder.Append("\"" + string.Empty + "\",");
            stringBuilder.Append("\"Passwd\":");
            stringBuilder.Append("\"" + string.Empty + "\"}");
            if (File.Exists(FileName.Version))
            {
                File.Delete(FileName.Version);
            }
            using (FileStream fileStream = new FileStream(FileName.Version, FileMode.CreateNew))
            {
                string s = stringBuilder.ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            return text;
        }

        #region(事件)
        private void button1_Click(object sender, EventArgs e)
        {
            this.CreateFile();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Form1.FileInfo fileInfo = e.Node.Tag as Form1.FileInfo;
            if (fileInfo != null)
            {
                this.comboBox2.Enabled = true;
                this.comboBox2.SelectedIndex = fileInfo.Operator;
                return;
            }
            this.comboBox2.Enabled = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                Form1.FileInfo fileInfo = this.treeView1.SelectedNode.Tag as Form1.FileInfo;
                if (fileInfo != null)
                {
                    fileInfo.Operator = this.comboBox2.SelectedIndex;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();
            if (string.IsNullOrEmpty(this.textBox7.Text))
            {
                this.errorProvider1.SetError(this.textBox7, "请选择程序可执行文件");
                return;
            }
            if (string.IsNullOrEmpty(this.textBox1.Text.Trim()))
            {
                this.errorProvider1.SetError(this.textBox1, "请输入程序名称");
                return;
            }
            if (string.IsNullOrEmpty(this.textBox2.Text.Trim()))
            {
                this.errorProvider1.SetError(this.textBox2, "请输入更新原因");
                return;
            }
            if (string.IsNullOrEmpty(this.textBox3.Text.Trim()))
            {
                this.errorProvider1.SetError(this.textBox3, "请输入更新描述");
                return;
            }
        
            if (string.IsNullOrEmpty(this.textBox5.Text.Trim()))
            {
                this.errorProvider1.SetError(this.textBox5, "请输入大版本号");
                return;
            }
            if (string.IsNullOrEmpty(this.textBox6.Text.Trim()))
            {
                this.errorProvider1.SetError(this.textBox6, "请输入补丁号");
                return;
            }
            this.CreateFile();
            string text = this.CreateUpdateFile();
            int length = Application.StartupPath.LastIndexOf('\\');
            text = Application.StartupPath.Substring(0, length) + "\\" + text;

            MathZip mathZip = new MathZip();
            if (mathZip.MathByIOZip(Application.StartupPath, text))
            {
                MessageBox.Show("补丁制作完成");
                return;
            }
            MessageBox.Show("补丁制作失败");

            //使用hotfix.exe制作
            //UpdateService updateService = new UpdateService();
            //if (updateService.MakePatch(Application.StartupPath, text))
            //{
            //    MessageBox.Show("补丁制作完成");
            //    return;
            //}
            //MessageBox.Show("补丁制作失败");
        }

            private void button4_Click(object sender, EventArgs e)
        {
            base.Close();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "|*.exe";
            openFileDialog.Multiselect = false;
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                this.textBox7.Text = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }
        #endregion


        /// <summary>
        /// 文件更新模式
        /// </summary>
        private class FileInfo
        {
            public string FileName;

            public int Operator;
        }

        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                this.hIcon = IntPtr.Zero;
                this.iIcon = 0;
                this.dwAttributes = 0U;
                this.szDisplayName = "";
                this.szTypeName = "";
            }

            public IntPtr hIcon;

            public int iIcon;

            public uint dwAttributes;

            [MarshalAs(UnmanagedType.LPStr)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string szTypeName;
        }

        private enum SHGFI
        {
            SmallIcon = 1,
            LargeIcon = 0,
            Icon = 256,
            DisplayName = 512,
            Typename = 1024,
            SysIconIndex = 16384,
            UseFileAttributes = 16
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }

 
}
