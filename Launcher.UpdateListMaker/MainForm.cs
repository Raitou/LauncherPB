using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace Launcher.UpdateListMaker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void lvUpdateList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void lvUpdateList_DragDrop(object sender, DragEventArgs e)
        {
            string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(string Filename in Files)
            {
                if (Directory.Exists(Filename))
                {
                    foreach (string DirFilename in Directory.GetFiles(Filename, "*.*", SearchOption.AllDirectories))
                        AddDirFilename(Filename, DirFilename);
                }
                else
                    AddFilename(Filename);
            }
        }

        private void AddDirFilename(string Directory, string Filename)
        {
            string Hash = string.Empty;
            using (MD5 hash = MD5.Create())
            {
                using (BufferedStream Stream = new BufferedStream(File.OpenRead(Filename), 1200000))
                {
                    Hash = BitConverter.ToString(hash.ComputeHash(Stream)).Replace('-', '.');
                }
            }
            CreateListViewItem(Filename.Replace(Path.GetDirectoryName(Directory) + Path.DirectorySeparatorChar, string.Empty), Hash, new FileInfo(Filename).Length);
        }

        private void AddFilename(string Filename)
        {
            string Hash = string.Empty;
            using (MD5 hash = MD5.Create())
            {
                using (BufferedStream Stream = new BufferedStream(File.OpenRead(Filename), 1200000))
                {
                    Hash = BitConverter.ToString(hash.ComputeHash(Stream)).Replace('-', '.');
                }
            }
            CreateListViewItem(Path.GetFileName(Filename), Hash, new FileInfo(Filename).Length);
        }

        private void CreateListViewItem(string Filename, string Hash, long FileSize)
        {
            ListViewItem Item = lvUpdateList.Items.Add(Filename);
            Item.SubItems.Add(Hash);
            Item.SubItems.Add(FileSize.ToString());
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog Dialog = new SaveFileDialog())
            {
                Dialog.Filter = "UpdateList XML|UpdateListConfiguration.xml";
                Dialog.FileName = "UpdateListConfiguration.xml";

                if(Dialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream Stream = File.Open(Dialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        XElement RootElement = new XElement(XName.Get("root"));
                        foreach (ListViewItem Item in lvUpdateList.Items)
                        {
                            string Filename = Item.Text;
                            string Hash = Item.SubItems[1].Text;
                            string Size = Item.SubItems[2].Text;

                            XElement PropertyElement = new XElement(XName.Get("Property"));
                            PropertyElement.SetAttributeValue(XName.Get("Section"), Filename);
                            PropertyElement.SetAttributeValue(XName.Get("Value"), Hash + "|" + Size);
                            RootElement.Add(PropertyElement);
                        }
                        RootElement.Save(Stream);
                    }
                }
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            lvUpdateList.Items.Clear();
        }

        private void lvUpdateList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lvUpdateList.SelectedIndices.Count > 0)
                for (int i = 0; i < lvUpdateList.SelectedIndices.Count + 0; i++)
                    lvUpdateList.Items.RemoveAt(lvUpdateList.SelectedIndices[i]);
        }
    }
}
