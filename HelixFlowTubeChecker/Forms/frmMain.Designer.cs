namespace HelixFlowTubeChecker
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.cmdReset = new System.Windows.Forms.Button();
            this.pnlTest = new System.Windows.Forms.Panel();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.chkLimitTubes = new System.Windows.Forms.CheckBox();
            this.pnlReset = new System.Windows.Forms.Panel();
            this.pblDebugOption = new System.Windows.Forms.Panel();
            this.chkShowTest = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.tubeValidationControl1 = new HelixFlowTubeChecker.TubeValidationControl();
            this.flowTubeListDisplayControl1 = new HelixFlowTubeChecker.FlowTubeListDisplayControl();
            this.pnlTest.SuspendLayout();
            this.pnlReset.SuspendLayout();
            this.pblDebugOption.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load XML from File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(156, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(270, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "C:\\Temp\\Flow\\Example Manifest.xml";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(621, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 21);
            this.button2.TabIndex = 3;
            this.button2.Text = "Delete existing data (testing)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(432, 12);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(176, 20);
            this.txtBarcode.TabIndex = 4;
            this.txtBarcode.Text = "BM15000940B";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(432, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(176, 21);
            this.button3.TabIndex = 5;
            this.button3.Text = "Simulate barcode scan (testing)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(157, 40);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(270, 20);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "C:\\Temp\\Flow";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1, 38);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(150, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Recurse Directory";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmdReset
            // 
            this.cmdReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdReset.Location = new System.Drawing.Point(0, 0);
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(785, 41);
            this.cmdReset.TabIndex = 9;
            this.cmdReset.Text = "Reset";
            this.cmdReset.UseVisualStyleBackColor = true;
            this.cmdReset.Click += new System.EventHandler(this.button5_Click);
            // 
            // pnlTest
            // 
            this.pnlTest.Controls.Add(this.txtLimit);
            this.pnlTest.Controls.Add(this.chkLimitTubes);
            this.pnlTest.Controls.Add(this.textBox2);
            this.pnlTest.Controls.Add(this.button4);
            this.pnlTest.Controls.Add(this.button3);
            this.pnlTest.Controls.Add(this.button2);
            this.pnlTest.Controls.Add(this.txtBarcode);
            this.pnlTest.Controls.Add(this.textBox1);
            this.pnlTest.Controls.Add(this.button1);
            this.pnlTest.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTest.Location = new System.Drawing.Point(3, 3);
            this.pnlTest.Name = "pnlTest";
            this.pnlTest.Size = new System.Drawing.Size(901, 68);
            this.pnlTest.TabIndex = 10;
            this.pnlTest.Visible = false;
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(709, 40);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(34, 20);
            this.txtLimit.TabIndex = 9;
            this.txtLimit.Text = "4";
            this.txtLimit.TextChanged += new System.EventHandler(this.txtLimit_TextChanged);
            // 
            // chkLimitTubes
            // 
            this.chkLimitTubes.AutoSize = true;
            this.chkLimitTubes.Location = new System.Drawing.Point(621, 42);
            this.chkLimitTubes.Name = "chkLimitTubes";
            this.chkLimitTubes.Size = new System.Drawing.Size(91, 17);
            this.chkLimitTubes.TabIndex = 8;
            this.chkLimitTubes.Text = "Limit tubes to ";
            this.chkLimitTubes.UseVisualStyleBackColor = true;
            this.chkLimitTubes.CheckedChanged += new System.EventHandler(this.chkLimitTubes_CheckedChanged);
            // 
            // pnlReset
            // 
            this.pnlReset.Controls.Add(this.cmdReset);
            this.pnlReset.Controls.Add(this.pblDebugOption);
            this.pnlReset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlReset.Location = new System.Drawing.Point(3, 463);
            this.pnlReset.Name = "pnlReset";
            this.pnlReset.Size = new System.Drawing.Size(901, 41);
            this.pnlReset.TabIndex = 11;
            // 
            // pblDebugOption
            // 
            this.pblDebugOption.Controls.Add(this.chkShowTest);
            this.pblDebugOption.Dock = System.Windows.Forms.DockStyle.Right;
            this.pblDebugOption.Location = new System.Drawing.Point(785, 0);
            this.pblDebugOption.Name = "pblDebugOption";
            this.pblDebugOption.Size = new System.Drawing.Size(116, 41);
            this.pblDebugOption.TabIndex = 11;
            // 
            // chkShowTest
            // 
            this.chkShowTest.AutoSize = true;
            this.chkShowTest.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkShowTest.Location = new System.Drawing.Point(32, 0);
            this.chkShowTest.Name = "chkShowTest";
            this.chkShowTest.Size = new System.Drawing.Size(84, 41);
            this.chkShowTest.TabIndex = 10;
            this.chkShowTest.Text = "Debug/Test";
            this.chkShowTest.UseVisualStyleBackColor = true;
            this.chkShowTest.CheckedChanged += new System.EventHandler(this.chkShowTest_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(915, 533);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tubeValidationControl1);
            this.tabPage1.Controls.Add(this.pnlTest);
            this.tabPage1.Controls.Add(this.pnlReset);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(907, 507);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Rack validation";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flowTubeListDisplayControl1);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(907, 507);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "View recent racks";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(901, 27);
            this.panel1.TabIndex = 1;
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.Location = new System.Drawing.Point(4, 1);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(75, 23);
            this.cmdRefresh.TabIndex = 0;
            this.cmdRefresh.Text = "Refresh";
            this.cmdRefresh.UseVisualStyleBackColor = true;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // tubeValidationControl1
            // 
            this.tubeValidationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tubeValidationControl1.LimitTubeCount = false;
            this.tubeValidationControl1.LimitTubeMax = 0;
            this.tubeValidationControl1.Location = new System.Drawing.Point(3, 71);
            this.tubeValidationControl1.Name = "tubeValidationControl1";
            this.tubeValidationControl1.Size = new System.Drawing.Size(901, 392);
            this.tubeValidationControl1.TabIndex = 0;
            this.tubeValidationControl1.VisibleColumnDefinition = "SeqNbr=Tube;AccessionNumber=Container;TubeLabel=Label;Name=Name;MRN=MRN;Confirmat" +
    "ionIcon=Confirmed?";
            // 
            // flowTubeListDisplayControl1
            // 
            this.flowTubeListDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTubeListDisplayControl1.Location = new System.Drawing.Point(3, 30);
            this.flowTubeListDisplayControl1.Name = "flowTubeListDisplayControl1";
            this.flowTubeListDisplayControl1.Size = new System.Drawing.Size(901, 474);
            this.flowTubeListDisplayControl1.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 533);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UT MDACC Helix Flow Tube Checker";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.pnlTest.ResumeLayout(false);
            this.pnlTest.PerformLayout();
            this.pnlReset.ResumeLayout(false);
            this.pblDebugOption.ResumeLayout(false);
            this.pblDebugOption.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button4;
        private TubeValidationControl tubeValidationControl1;
        private System.Windows.Forms.Button cmdReset;
        private System.Windows.Forms.Panel pnlTest;
        private System.Windows.Forms.TextBox txtLimit;
        private System.Windows.Forms.CheckBox chkLimitTubes;
        private System.Windows.Forms.Panel pnlReset;
        private System.Windows.Forms.Panel pblDebugOption;
        private System.Windows.Forms.CheckBox chkShowTest;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private FlowTubeListDisplayControl flowTubeListDisplayControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdRefresh;
    }
}

