using OccultWatcher.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Occulwatcher.ACPExporter.Resources
{
    public partial class configDialog : Form
    {

        private ACPExporterConfig intConfig;
        public configDialog(ACPExporterConfig config)
        {
            InitializeComponent();
            intConfig = config;
            FilePathBox.Text = intConfig.ConfigFilePath;
            textBox2.Text = intConfig.ConfigHeader;
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FilePathBox.Text = saveFileDialog1.FileName;
                }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void configDialog_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            intConfig.ConfigFilePath = FilePathBox.Text;
            intConfig.ConfigHeader = textBox2.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
