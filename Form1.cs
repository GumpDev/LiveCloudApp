using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace LiveCloud
{
    public partial class Form1 : Form
    {
        string path;
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LiveCloud\\";
        string pathM;
        string url = "http://localhost/LiveCloud";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            path = defaultPath;
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }
            label1.Text = "";
            label2.Text = "";
            GetFilesInDirectory();
        }

        private void GetFilesInDirectory()
        {
            textBox1.Text = path;
            listView1.Items.Clear();
            if(path != defaultPath)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = 0;
                item.Text = "...";
                listView1.Items.Add(item);
            }
            string[] folders = Directory.GetDirectories(path);
            for (int i = 0; i < folders.Length; i++)
            {
                string name = folders[i].Split('\\')[folders[i].Split('\\').Length - 1];
                string pathfl = folders[i];
                ListViewItem item = new ListViewItem();
                item.ImageIndex = 1;
                item.Text = name;
                listView1.Items.Add(item);
            }
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string name = files[i].Split('\\')[files[i].Split('\\').Length - 1];
                string pathfl = files[i];
                ListViewItem item = new ListViewItem();
                item.ImageIndex = 4;
                item.Text = name;
                listView1.Items.Add(item);
            }
            WebClient wc = new WebClient();
            string wFiles = wc.DownloadString(url + "/getFiles.php?loc=" + pathM);
            for(int i = 0; i < wFiles.Split('|').Length - 2; i++)
            {
                string name = wFiles.Split('|')[i];
                i++;
                int size = int.Parse(wFiles.Split('|')[i]);
                int status = 0;
                int id = 0;
                for(int x = 0; x < listView1.Items.Count; x++)
                {
                    if(listView1.Items[x].Text == name)
                    {
                        MessageBox.Show(File.ReadAllBytes(path + listView1.Items[i].Text).Length.ToString());
                        MessageBox.Show(size.ToString());
                        if(size != File.ReadAllBytes(path + listView1.Items[i].Text).Length)
                        {
                            status = 1;
                        }
                        else
                        {
                            status = 2;
                        }
                    }
                    id = x;
                    break;
                }
                if(status == 0)
                {
                    string namen = name;
                    ListViewItem item = new ListViewItem();
                    if (size == 0)
                        item.ImageIndex = 2;
                    else
                        item.ImageIndex = 5;
                    item.Text = namen;
                    listView1.Items.Add(item);
                }
                else if (status == 1)
                {
                    string namen = name;
                    ListViewItem item = new ListViewItem();
                    if (size == 0)
                        item.ImageIndex = 3;
                    else
                        item.ImageIndex = 6;
                    item.Text = namen;
                    listView1.Items[id] = item;
                }else if(status == 2)
                {
                    string namen = name;
                    ListViewItem item = new ListViewItem();
                    if (size == 0)
                        item.ImageIndex = 7;
                    else
                        item.ImageIndex = 8;
                    item.Text = namen;
                    listView1.Items[id] = item;
                }
            }
        }

        static readonly string[] SizeSuffixes =
                  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(value, 1024);

            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                string selected = listView1.SelectedItems[0].Text;
                if(File.Exists(path + selected))
                {
                    Process.Start(path + selected);
                }
                else
                {
                    if (selected == "...")
                    {
                        string x = "";
                        pathM = "";
                        for(int i = 0; i < path.Split('\\').Length -2; i++)
                        {
                            x += path.Split('\\')[i] + "\\";
                            if (i > 5)
                                pathM += path.Split('\\')[i] + "\\";
                        }
                        path = x;
                        GetFilesInDirectory();
                    }
                    else
                    {
                        path += selected + "\\";
                        pathM += selected + "\\";
                        GetFilesInDirectory();
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selected = listView1.SelectedItems[0].Text;
                if (selected == "...")
                {
                    label1.Text = "...";
                    label2.Text = "Back to last Dircctory";
                }
                else
                {
                    if(listView1.SelectedItems[0].ImageIndex == 2 || listView1.SelectedItems[0].ImageIndex == 5)
                    {
                        label1.Text = selected;
                        label2.Text = "in Server";
                    }else if (listView1.SelectedItems[0].ImageIndex == 6 || listView1.SelectedItems[0].ImageIndex == 3)
                    {
                        label1.Text = selected;
                        label2.Text = "Different From Server";
                    }
                    else if (File.Exists(path + selected))
                    {
                        label1.Text = selected;
                        label2.Text = "Size: " + SizeSuffix(File.ReadAllBytes(path + selected).Length);
                    }
                    else
                    {
                        label1.Text = selected;
                        label2.Text = "Folder";
                    }
                }
            }
        }
    }
}
