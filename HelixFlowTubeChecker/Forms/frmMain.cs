using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using HidWatcher2;


namespace HelixFlowTubeChecker
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            DataInterface di = new DataInterface(Global.SAMPLE_CONNECTION);
            this.tubeValidationControl1.Bind(di);
            this.flowTubeListDisplayControl1.Bind(di);

            HidWatcher2.BarCodeWatcher bcw = new BarCodeWatcher(Global.USB_SCANNER_CONFIG);
            bcw.CodeRead += new BarCodeWatcher.CodeReadHandler(bcw_CodeRead);
            // tubeValidationControl1.ShowMessage("Welcome to the tube checker. Scan first tube of a rack/manifest to begin.");
            this.flowTubeListDisplayControl1.RefreshData();
            this.tubeValidationControl1.DatabaseUpdated += new TubeValidationControl.DatabaseUpdatedEventHandler(tubeValidationControl1_DatabaseUpdated);
            UpdateTubeLimits();

           // System.IO.FileSystemWatcher fsw = new FileSystemWatcher(@"\\plmfs1\plmlabs\Helix Automation\PROD\Flow Cytometry");
        //  fsw.IncludeSubdirectories = true;
    //    fsw.Created += new FileSystemEventHandler(fsw_Created);
   //      fsw.EnableRaisingEvents = true;
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.FullPath);
        }

        void tubeValidationControl1_DatabaseUpdated()
        {
            this.flowTubeListDisplayControl1.RefreshData();
        }

        public delegate void SetCodeReadCallback(Code barcode);

        void bcw_CodeRead(Code barcode)
        {
            if (base.InvokeRequired)
            {
                SetCodeReadCallback method = new SetCodeReadCallback(this.HandleBarCodeScan);
                base.Invoke(method, new object[] { barcode });
            }
            else
                this.HandleBarCodeScan(barcode);
        }


        //  private string _connectionString = @"Server=spidrsql3\spidrsql3;Database=HelixWorkflow;User Id=HelixWorkflow_ServiceAccount;Password=k&ye_j2ZzmE4^e;";
        //private string _connectionString = @"Server=LISW8558495;Database=HelixWorkflow;User Id=HelixWorkflow_ServiceAccount;Password=k&ye_j2ZzmE4^e;";

        public string FormattedToUnformattedAccession(string FormattedAccession)
        {
            return FormattedAccession.Replace("-", "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataInterface di = new DataInterface(Global.SAMPLE_CONNECTION);
            di.DeleteAllFlowData("YESIREALLYWANTTODOTHIS");
            this.flowTubeListDisplayControl1.RefreshData();
            MessageBox.Show("All tube data has been deleted");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Code barcode = new Code(this.txtBarcode.Text, Symbology.DataMatrix);
            this.HandleBarCodeScan(barcode);
        }

        public void HandleBarCodeScan(Code barcode)
        {
            if (barcode.Symbology == Symbology.DataMatrix)
                tubeValidationControl1.HandleAccessionScan(barcode.TextData);
            else
            {
                Sound.PlayWaveResource("StateError.wav");
                MessageBox.Show("Unhandled barcode symbology for this form");
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            this.tubeValidationControl1.Clear();
        }

        private void UpdateTubeLimits()
        {
            this.tubeValidationControl1.LimitTubeCount = this.chkLimitTubes.Checked;
            this.tubeValidationControl1.LimitTubeMax = Convert.ToInt32(this.txtLimit.Text);
        }


        private void chkLimitTubes_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTubeLimits();

        }

        private void txtLimit_TextChanged(object sender, EventArgs e)
        {
            UpdateTubeLimits();
        }

        private void showTestTrainOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Show test/train options", "Enter administrative password");
            if (input != "mdl") return;
            this.pnlTest.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataInterface di = new DataInterface(Global.SAMPLE_CONNECTION);
            int UpdateCount = di.ParseDirectory(this.textBox2.Text).Count;
            this.flowTubeListDisplayControl1.RefreshData();
            MessageBox.Show(UpdateCount.ToString() + " new files parsed and uploaded to database");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataInterface di = new DataInterface(Global.SAMPLE_CONNECTION);
            bool Success = di.ParseFile(this.textBox1.Text);
            this.flowTubeListDisplayControl1.RefreshData();
            if (Success)
                MessageBox.Show("File" + this.textBox1.Text + " parsed and uploaded to the database");
            else
                MessageBox.Show("Not parsed.  May already be in database or may not be a valid XML file");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void chkShowTest_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlTest.Visible = chkShowTest.Checked;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            this.flowTubeListDisplayControl1.RefreshData();
        }







    }
}
