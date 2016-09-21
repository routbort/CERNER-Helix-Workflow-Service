
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.DirectoryServices.AccountManagement;
using System.Deployment.Application;


public class frmLogin : Form
{
    private Button btnLogin;
    private CheckBox chkTest;
    private IContainer components = null;
    private const bool DEBUG = false;
    private Label label1;
    private Label label2;
    private Label lblFail;
    private Label lblMain;
    private TextBox txtPassword;
    private Label lblContributing;
    private TextBox txtUsername;

    public frmLogin()
    {
        this.InitializeComponent();
        if (ApplicationDeployment.IsNetworkDeployed)
            this.Text += " " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);

        this.txtPassword.Focus();

    }

    private void btnLogin_Click(object sender, EventArgs e)
    {

        Global.CURRENT_USERNAME = this.txtUsername.Text;

        AuthenticationResult result = this.IsAuthenticated(Global.DIRECTORY, this.txtUsername.Text, Global.AD_GROUP, this.txtPassword.Text);

        if (Global.AD_GROUP != null)
        {
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Global.DIRECTORY);

            // find a user
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, this.txtUsername.Text);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, Global.AD_GROUP);

            if (user == null || !user.IsMemberOf(group))
            {
                this.lblFail.Text = "Error:  User must be member of  " + Global.AD_GROUP + " to use this application." + Environment.NewLine + Environment.NewLine + result.ErrorMessage;
                this.txtPassword.Text = "";
                this.lblMain.Visible = false;
                return;
            }
        }

        if (result.Success)
        {
            this.SetApplicationGlobals();

            HelixFlowTubeChecker.frmMain form = new HelixFlowTubeChecker.frmMain();
            form.Show();


            base.Hide();
        }
        else
        {
            this.lblFail.Text = "Login failed: " + Environment.NewLine + Environment.NewLine + result.ErrorMessage;
            this.txtPassword.Text = "";
            this.lblMain.Visible = false;
        }
    }

    private void chkTest_CheckedChanged(object sender, EventArgs e)
    {
        Global.IS_TEST_SYSTEM = this.chkTest.Checked;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (this.components != null))
        {
            this.components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void frmLogin_Load(object sender, EventArgs e)
    {
        string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        this.lblMain.Text = this.lblMain.Text.Replace("{BUILD}", productVersion);
        if (Environment.UserDomainName != Global.DIRECTORY)
        {
            MessageBox.Show("Access to this application requires that the computer be logged in to the " + Global.DIRECTORY + " domain." + Environment.NewLine + Environment.NewLine + "Exiting application . . .", "Error");
            Application.Exit();
        }
        this.txtUsername.Text = Environment.UserName;
        this.txtPassword.Focus();
    }

    private void InitializeComponent()
    {
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblFail = new System.Windows.Forms.Label();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.lblMain = new System.Windows.Forms.Label();
            this.lblContributing = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(96, 24);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(240, 26);
            this.txtUsername.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(96, 56);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(240, 26);
            this.txtPassword.TabIndex = 0;
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.Location = new System.Drawing.Point(131, 123);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(102, 36);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblFail
            // 
            this.lblFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFail.ForeColor = System.Drawing.Color.Red;
            this.lblFail.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblFail.Location = new System.Drawing.Point(17, 169);
            this.lblFail.Name = "lblFail";
            this.lblFail.Size = new System.Drawing.Size(317, 70);
            this.lblFail.TabIndex = 5;
            this.lblFail.Click += new System.EventHandler(this.lblFail_Click);
            // 
            // chkTest
            // 
            this.chkTest.AutoSize = true;
            this.chkTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTest.Location = new System.Drawing.Point(204, 88);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(130, 19);
            this.chkTest.TabIndex = 6;
            this.chkTest.Text = "Use Test Database";
            this.chkTest.UseVisualStyleBackColor = true;
            this.chkTest.CheckedChanged += new System.EventHandler(this.chkTest_CheckedChanged);
            // 
            // lblMain
            // 
            this.lblMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.Location = new System.Drawing.Point(26, 239);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(207, 51);
            this.lblMain.TabIndex = 10;
            this.lblMain.Text = "UT MD Anderson \r\nFlow Cytometry Lab";
            this.lblMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblContributing
            // 
            this.lblContributing.AutoSize = true;
            this.lblContributing.Location = new System.Drawing.Point(254, 260);
            this.lblContributing.Name = "lblContributing";
            this.lblContributing.Size = new System.Drawing.Size(82, 26);
            this.lblContributing.TabIndex = 12;
            this.lblContributing.Text = "Mark Routbort\r\nDanielle Cooper\r";
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 300);
            this.Controls.Add(this.lblContributing);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.chkTest);
            this.Controls.Add(this.lblFail);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUsername);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Helix Flow Tube Checker Login";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    public AuthenticationResult IsAuthenticated(string domain, string username, string groupname, string pwd)
    {
        string str = domain + @"\" + username;
        DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + domain, str, pwd);
        AuthenticationResult result = new AuthenticationResult
        {
            Success = false
        };
        try
        {
            object nativeObject = searchRoot.NativeObject;
            DirectorySearcher searcher = new DirectorySearcher(searchRoot)
            {
                Filter = "(SAMAccountName=" + username + ")"
            };
            searcher.PropertiesToLoad.Add("cn");
            SearchResult result2 = searcher.FindOne();
            if (null == result2)
            {
                result.Success = false;
                result.ErrorMessage = "User/password match not found.";
                return result;
            }
        }
        catch (Exception exception)
        {
            result.Success = false;
            result.ErrorMessage = "Error authenticating user:" + Environment.NewLine + exception.Message;
            return result;
        }

        if (groupname == null)
        {
            result.Success = true;
            return result;
        }

        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);

        // find a user
        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);

        // find the group in question
        GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupname);

        if (user != null && group != null)
        {
            // check if user is member of that group
            if (user.IsMemberOf(group))
            {
                result.Success = true;
                return result;
            }
        }

        result.Success = false;
        result.ErrorMessage = "Could not verify membership in group " + groupname;
        return result;



    }

    private void lblFail_Click(object sender, EventArgs e)
    {
    }

    private void SetApplicationGlobals()
    {
        Global.SAMPLE_CONNECTION = this.chkTest.Checked ? Global.CONNECTION_TEST : Global.CONNECTION_PROD;

    }








}


