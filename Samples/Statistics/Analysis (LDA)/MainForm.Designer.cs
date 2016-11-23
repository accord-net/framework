namespace Analysis.LDA
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tscbMethod = new System.Windows.Forms.ToolStripComboBox();
            this.btnRunAnalysis = new System.Windows.Forms.ToolStripButton();
            this.MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabShift = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dgvProjectionSource = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvProjectionResult = new System.Windows.Forms.DataGridView();
            this.btnProject2 = new System.Windows.Forms.Button();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.outputScatterplot = new Accord.Controls.ScatterplotView();
            this.tabOverview = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvPrincipalComponents = new System.Windows.Forms.DataGridView();
            this.colComponent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEigenValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProportion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCumulativeProportion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvFeatureVectors = new System.Windows.Forms.DataGridView();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.distributionView = new Accord.Controls.ComponentView();
            this.cumulativeView = new Accord.Controls.ComponentView();
            this.tabDistribution = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.dgvDistributionMeasures = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.dataHistogramView1 = new Accord.Controls.HistogramView();
            this.tabSamples = new System.Windows.Forms.TabPage();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.btnSampleRunAnalysis = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvAnalysisSource = new System.Windows.Forms.DataGridView();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.inputScatterplot = new Accord.Controls.ScatterplotView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.dgvScatterWithin = new System.Windows.Forms.DataGridView();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.dgvScatterBetween = new System.Windows.Forms.DataGridView();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.dgvScatterTotal = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.dgvScatter = new System.Windows.Forms.DataGridView();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.dgvClasses = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrevalence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.dgvClassData = new System.Windows.Forms.DataGridView();
            this.dgvClassHistogram = new System.Windows.Forms.GroupBox();
            this.dataHistogramView2 = new Accord.Controls.HistogramView();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.tabShift.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectionSource)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectionResult)).BeginInit();
            this.groupBox16.SuspendLayout();
            this.tabOverview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrincipalComponents)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeatureVectors)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.tabDistribution.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistributionMeasures)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.tabSamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnalysisSource)).BeginInit();
            this.groupBox15.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterWithin)).BeginInit();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterBetween)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterTotal)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatter)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClasses)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClassData)).BeginInit();
            this.dgvClassHistogram.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // tscbMethod
            // 
            this.tscbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbMethod.Name = "tscbMethod";
            this.tscbMethod.Size = new System.Drawing.Size(121, 33);
            // 
            // btnRunAnalysis
            // 
            this.btnRunAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRunAnalysis.Name = "btnRunAnalysis";
            this.btnRunAnalysis.Size = new System.Drawing.Size(78, 22);
            this.btnRunAnalysis.Text = "&Run Analysis";
            // 
            // MenuFileOpen
            // 
            this.MenuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileOpen.Image")));
            this.MenuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuFileOpen.Name = "MenuFileOpen";
            this.MenuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuFileOpen.Size = new System.Drawing.Size(146, 22);
            this.MenuFileOpen.Text = "&Open";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem6});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1340, 35);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator3,
            this.toolStripMenuItem5});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(50, 29);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(201, 30);
            this.toolStripMenuItem2.Text = "&Open";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(198, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(201, 30);
            this.toolStripMenuItem5.Text = "E&xit";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(61, 29);
            this.toolStripMenuItem6.Text = "&Help";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(146, 30);
            this.toolStripMenuItem7.Text = "&About...";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // tabShift
            // 
            this.tabShift.Controls.Add(this.tableLayoutPanel5);
            this.tabShift.Location = new System.Drawing.Point(4, 29);
            this.tabShift.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabShift.Name = "tabShift";
            this.tabShift.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabShift.Size = new System.Drawing.Size(1332, 753);
            this.tabShift.TabIndex = 4;
            this.tabShift.Text = "Discriminant Projection";
            this.tabShift.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dgvProjectionSource);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(4, 4);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Size = new System.Drawing.Size(345, 341);
            this.groupBox8.TabIndex = 7;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Original";
            // 
            // dgvProjectionSource
            // 
            this.dgvProjectionSource.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProjectionSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvProjectionSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvProjectionSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProjectionSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProjectionSource.Location = new System.Drawing.Point(4, 23);
            this.dgvProjectionSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvProjectionSource.Name = "dgvProjectionSource";
            this.dgvProjectionSource.Size = new System.Drawing.Size(337, 314);
            this.dgvProjectionSource.TabIndex = 7;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvProjectionResult);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(4, 353);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(345, 341);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Projection";
            // 
            // dgvProjectionResult
            // 
            this.dgvProjectionResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProjectionResult.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvProjectionResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvProjectionResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProjectionResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProjectionResult.Location = new System.Drawing.Point(4, 23);
            this.dgvProjectionResult.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvProjectionResult.Name = "dgvProjectionResult";
            this.dgvProjectionResult.ReadOnly = true;
            this.dgvProjectionResult.Size = new System.Drawing.Size(337, 314);
            this.dgvProjectionResult.TabIndex = 7;
            // 
            // btnProject2
            // 
            this.btnProject2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnProject2.Location = new System.Drawing.Point(4, 702);
            this.btnProject2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnProject2.Name = "btnProject2";
            this.btnProject2.Size = new System.Drawing.Size(345, 39);
            this.btnProject2.TabIndex = 5;
            this.btnProject2.Text = "Project!";
            this.btnProject2.UseVisualStyleBackColor = true;
            this.btnProject2.Click += new System.EventHandler(this.btnProject_Click);
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.outputScatterplot);
            this.groupBox16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox16.Location = new System.Drawing.Point(357, 4);
            this.groupBox16.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel5.SetRowSpan(this.groupBox16, 3);
            this.groupBox16.Size = new System.Drawing.Size(963, 737);
            this.groupBox16.TabIndex = 8;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Scatter Plot";
            // 
            // outputScatterplot
            // 
            this.outputScatterplot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputScatterplot.LinesVisible = false;
            this.outputScatterplot.Location = new System.Drawing.Point(4, 23);
            this.outputScatterplot.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.outputScatterplot.Name = "outputScatterplot";
            this.outputScatterplot.ScaleTight = false;
            this.outputScatterplot.Size = new System.Drawing.Size(955, 710);
            this.outputScatterplot.SymbolSize = 7F;
            this.outputScatterplot.TabIndex = 0;
            // 
            // tabOverview
            // 
            this.tabOverview.Controls.Add(this.tableLayoutPanel2);
            this.tabOverview.Location = new System.Drawing.Point(4, 29);
            this.tabOverview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabOverview.Name = "tabOverview";
            this.tabOverview.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabOverview.Size = new System.Drawing.Size(1332, 753);
            this.tabOverview.TabIndex = 1;
            this.tabOverview.Text = "Linear Discriminant Analysis";
            this.tabOverview.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(4, 4);
            this.splitContainer4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer4.Size = new System.Drawing.Size(1316, 364);
            this.splitContainer4.SplitterDistance = 537;
            this.splitContainer4.SplitterWidth = 6;
            this.splitContainer4.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvPrincipalComponents);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(537, 364);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Components";
            // 
            // dgvPrincipalComponents
            // 
            this.dgvPrincipalComponents.AllowUserToAddRows = false;
            this.dgvPrincipalComponents.AllowUserToDeleteRows = false;
            this.dgvPrincipalComponents.AllowUserToResizeRows = false;
            this.dgvPrincipalComponents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPrincipalComponents.BackgroundColor = System.Drawing.Color.White;
            this.dgvPrincipalComponents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPrincipalComponents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPrincipalComponents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrincipalComponents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colComponent,
            this.colEigenValue,
            this.colProportion,
            this.colCumulativeProportion});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrincipalComponents.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvPrincipalComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrincipalComponents.Location = new System.Drawing.Point(4, 23);
            this.dgvPrincipalComponents.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvPrincipalComponents.Name = "dgvPrincipalComponents";
            this.dgvPrincipalComponents.ReadOnly = true;
            this.dgvPrincipalComponents.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvPrincipalComponents.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPrincipalComponents.Size = new System.Drawing.Size(529, 337);
            this.dgvPrincipalComponents.TabIndex = 1;
            // 
            // colComponent
            // 
            this.colComponent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colComponent.DataPropertyName = "Index";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colComponent.DefaultCellStyle = dataGridViewCellStyle2;
            this.colComponent.FillWeight = 10F;
            this.colComponent.HeaderText = "Component";
            this.colComponent.Name = "colComponent";
            this.colComponent.ReadOnly = true;
            this.colComponent.Width = 105;
            // 
            // colEigenValue
            // 
            this.colEigenValue.DataPropertyName = "Eigenvalue";
            dataGridViewCellStyle3.Format = "N5";
            dataGridViewCellStyle3.NullValue = null;
            this.colEigenValue.DefaultCellStyle = dataGridViewCellStyle3;
            this.colEigenValue.HeaderText = "Eigen Value";
            this.colEigenValue.Name = "colEigenValue";
            this.colEigenValue.ReadOnly = true;
            // 
            // colProportion
            // 
            this.colProportion.DataPropertyName = "Proportion";
            dataGridViewCellStyle4.Format = "N5";
            this.colProportion.DefaultCellStyle = dataGridViewCellStyle4;
            this.colProportion.HeaderText = "Proportion";
            this.colProportion.Name = "colProportion";
            this.colProportion.ReadOnly = true;
            // 
            // colCumulativeProportion
            // 
            this.colCumulativeProportion.DataPropertyName = "CumulativeProportion";
            dataGridViewCellStyle5.Format = "N5";
            this.colCumulativeProportion.DefaultCellStyle = dataGridViewCellStyle5;
            this.colCumulativeProportion.HeaderText = "Cumulative Proportion";
            this.colCumulativeProportion.Name = "colCumulativeProportion";
            this.colCumulativeProportion.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvFeatureVectors);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(773, 364);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Eigenvectors Matrix";
            // 
            // dgvFeatureVectors
            // 
            this.dgvFeatureVectors.AllowUserToAddRows = false;
            this.dgvFeatureVectors.AllowUserToDeleteRows = false;
            this.dgvFeatureVectors.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvFeatureVectors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvFeatureVectors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFeatureVectors.ColumnHeadersVisible = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.Format = "N4";
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFeatureVectors.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvFeatureVectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFeatureVectors.Location = new System.Drawing.Point(4, 23);
            this.dgvFeatureVectors.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvFeatureVectors.Name = "dgvFeatureVectors";
            this.dgvFeatureVectors.ReadOnly = true;
            this.dgvFeatureVectors.RowHeadersVisible = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvFeatureVectors.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvFeatureVectors.Size = new System.Drawing.Size(765, 337);
            this.dgvFeatureVectors.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cumulativeView);
            this.groupBox5.Controls.Add(this.distributionView);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(4, 376);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(1316, 365);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Visualization";
            // 
            // distributionView
            // 
            this.distributionView.Cumulative = false;
            this.distributionView.DataSource = null;
            this.distributionView.Dock = System.Windows.Forms.DockStyle.Left;
            this.distributionView.Location = new System.Drawing.Point(4, 23);
            this.distributionView.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.distributionView.Name = "distributionView";
            this.distributionView.Size = new System.Drawing.Size(390, 338);
            this.distributionView.TabIndex = 4;
            // 
            // cumulativeView
            // 
            this.cumulativeView.Cumulative = true;
            this.cumulativeView.DataSource = null;
            this.cumulativeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cumulativeView.Location = new System.Drawing.Point(394, 23);
            this.cumulativeView.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.cumulativeView.Name = "cumulativeView";
            this.cumulativeView.Size = new System.Drawing.Size(918, 338);
            this.cumulativeView.TabIndex = 4;
            // 
            // tabDistribution
            // 
            this.tabDistribution.Controls.Add(this.tableLayoutPanel1);
            this.tabDistribution.Location = new System.Drawing.Point(4, 29);
            this.tabDistribution.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabDistribution.Name = "tabDistribution";
            this.tabDistribution.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabDistribution.Size = new System.Drawing.Size(1332, 753);
            this.tabDistribution.TabIndex = 7;
            this.tabDistribution.Text = "Univariate Descriptive Statistics";
            this.tabDistribution.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dgvDistributionMeasures);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(4, 4);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Size = new System.Drawing.Size(1316, 364);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Samples";
            // 
            // dgvDistributionMeasures
            // 
            this.dgvDistributionMeasures.AllowUserToAddRows = false;
            this.dgvDistributionMeasures.AllowUserToDeleteRows = false;
            this.dgvDistributionMeasures.AllowUserToOrderColumns = true;
            this.dgvDistributionMeasures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDistributionMeasures.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvDistributionMeasures.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDistributionMeasures.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvDistributionMeasures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDistributionMeasures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.colIndex,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.colMin,
            this.colLength});
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle13.Format = "N5";
            dataGridViewCellStyle13.NullValue = null;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDistributionMeasures.DefaultCellStyle = dataGridViewCellStyle13;
            this.dgvDistributionMeasures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDistributionMeasures.Location = new System.Drawing.Point(4, 23);
            this.dgvDistributionMeasures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvDistributionMeasures.Name = "dgvDistributionMeasures";
            this.dgvDistributionMeasures.ReadOnly = true;
            this.dgvDistributionMeasures.RowHeadersVisible = false;
            this.dgvDistributionMeasures.Size = new System.Drawing.Size(1308, 337);
            this.dgvDistributionMeasures.TabIndex = 1;
            this.dgvDistributionMeasures.CurrentCellChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.DataPropertyName = "Name";
            dataGridViewCellStyle11.Format = "N0";
            dataGridViewCellStyle11.NullValue = null;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn6.HeaderText = "Column";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 80;
            // 
            // colIndex
            // 
            this.colIndex.DataPropertyName = "Index";
            this.colIndex.HeaderText = "Index";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "Mean";
            this.dataGridViewTextBoxColumn7.HeaderText = "Mean";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "StandardDeviation";
            this.dataGridViewTextBoxColumn8.HeaderText = "Standard Deviation";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "Median";
            this.dataGridViewTextBoxColumn9.HeaderText = "Median";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "Mode";
            this.dataGridViewTextBoxColumn10.HeaderText = "Mode";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.DataPropertyName = "Variance";
            this.dataGridViewTextBoxColumn11.HeaderText = "Variance";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "Max";
            this.dataGridViewTextBoxColumn12.HeaderText = "Max";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // colMin
            // 
            this.colMin.DataPropertyName = "Min";
            this.colMin.HeaderText = "Min";
            this.colMin.Name = "colMin";
            this.colMin.ReadOnly = true;
            // 
            // colLength
            // 
            this.colLength.DataPropertyName = "Length";
            dataGridViewCellStyle12.Format = "N";
            dataGridViewCellStyle12.NullValue = null;
            this.colLength.DefaultCellStyle = dataGridViewCellStyle12;
            this.colLength.HeaderText = "Length";
            this.colLength.Name = "colLength";
            this.colLength.ReadOnly = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.dataHistogramView1);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.Location = new System.Drawing.Point(4, 376);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox12.Size = new System.Drawing.Size(1316, 365);
            this.groupBox12.TabIndex = 0;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Histogram";
            // 
            // dataHistogramView1
            // 
            this.dataHistogramView1.BinWidth = null;
            this.dataHistogramView1.DataSource = ((object)(resources.GetObject("dataHistogramView1.DataSource")));
            this.dataHistogramView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataHistogramView1.Histogram = ((Accord.Statistics.Visualizations.Histogram)(resources.GetObject("dataHistogramView1.Histogram")));
            this.dataHistogramView1.Location = new System.Drawing.Point(4, 23);
            this.dataHistogramView1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.dataHistogramView1.Name = "dataHistogramView1";
            this.dataHistogramView1.NumberOfBins = null;
            this.dataHistogramView1.Size = new System.Drawing.Size(1308, 338);
            this.dataHistogramView1.TabIndex = 1;
            // 
            // tabSamples
            // 
            this.tabSamples.Controls.Add(this.splitContainer7);
            this.tabSamples.Location = new System.Drawing.Point(4, 29);
            this.tabSamples.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabSamples.Name = "tabSamples";
            this.tabSamples.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabSamples.Size = new System.Drawing.Size(1332, 753);
            this.tabSamples.TabIndex = 0;
            this.tabSamples.Text = "Samples (Input)";
            this.tabSamples.UseVisualStyleBackColor = true;
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.Location = new System.Drawing.Point(4, 4);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.groupBox7);
            this.splitContainer7.Panel1.Controls.Add(this.btnSampleRunAnalysis);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.groupBox15);
            this.splitContainer7.Size = new System.Drawing.Size(1324, 745);
            this.splitContainer7.SplitterDistance = 332;
            this.splitContainer7.SplitterWidth = 6;
            this.splitContainer7.TabIndex = 8;
            // 
            // btnSampleRunAnalysis
            // 
            this.btnSampleRunAnalysis.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSampleRunAnalysis.Location = new System.Drawing.Point(0, 683);
            this.btnSampleRunAnalysis.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSampleRunAnalysis.Name = "btnSampleRunAnalysis";
            this.btnSampleRunAnalysis.Size = new System.Drawing.Size(332, 62);
            this.btnSampleRunAnalysis.TabIndex = 1;
            this.btnSampleRunAnalysis.Text = "Compute analysis";
            this.btnSampleRunAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSampleRunAnalysis.UseVisualStyleBackColor = true;
            this.btnSampleRunAnalysis.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvAnalysisSource);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Size = new System.Drawing.Size(332, 683);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Samples";
            // 
            // dgvAnalysisSource
            // 
            this.dgvAnalysisSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvAnalysisSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvAnalysisSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAnalysisSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAnalysisSource.Location = new System.Drawing.Point(4, 23);
            this.dgvAnalysisSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvAnalysisSource.Name = "dgvAnalysisSource";
            this.dgvAnalysisSource.Size = new System.Drawing.Size(324, 656);
            this.dgvAnalysisSource.TabIndex = 5;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.inputScatterplot);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(0, 0);
            this.groupBox15.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox15.Size = new System.Drawing.Size(986, 745);
            this.groupBox15.TabIndex = 7;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Scatter Plot";
            // 
            // inputScatterplot
            // 
            this.inputScatterplot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputScatterplot.LinesVisible = false;
            this.inputScatterplot.Location = new System.Drawing.Point(4, 23);
            this.inputScatterplot.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.inputScatterplot.Name = "inputScatterplot";
            this.inputScatterplot.ScaleTight = false;
            this.inputScatterplot.Size = new System.Drawing.Size(978, 718);
            this.inputScatterplot.SymbolSize = 7F;
            this.inputScatterplot.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSamples);
            this.tabControl.Controls.Add(this.tabDistribution);
            this.tabControl.Controls.Add(this.tabOverview);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabShift);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 35);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1340, 786);
            this.tabControl.TabIndex = 9;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel3);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Size = new System.Drawing.Size(1332, 753);
            this.tabPage4.TabIndex = 9;
            this.tabPage4.Text = "Scatter Matrices";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel4);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1332, 753);
            this.tabPage3.TabIndex = 8;
            this.tabPage3.Text = "Classes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 821);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1340, 30);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(222, 25);
            this.lbStatus.Text = "Click File > Open to begin!";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox12, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox11, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1324, 745);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.splitContainer4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox5, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1324, 745);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.48617F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.51383F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox18, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.groupBox17, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox13, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1324, 745);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.dgvScatterWithin);
            this.groupBox18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox18.Location = new System.Drawing.Point(540, 376);
            this.groupBox18.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox18.Size = new System.Drawing.Size(780, 365);
            this.groupBox18.TabIndex = 2;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "Within Class Scatter Matrix";
            // 
            // dgvScatterWithin
            // 
            this.dgvScatterWithin.AllowUserToAddRows = false;
            this.dgvScatterWithin.AllowUserToDeleteRows = false;
            this.dgvScatterWithin.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvScatterWithin.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScatterWithin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScatterWithin.ColumnHeadersVisible = false;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.Format = "N4";
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvScatterWithin.DefaultCellStyle = dataGridViewCellStyle14;
            this.dgvScatterWithin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScatterWithin.Location = new System.Drawing.Point(4, 23);
            this.dgvScatterWithin.Margin = new System.Windows.Forms.Padding(4);
            this.dgvScatterWithin.Name = "dgvScatterWithin";
            this.dgvScatterWithin.ReadOnly = true;
            this.dgvScatterWithin.RowHeadersVisible = false;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvScatterWithin.RowsDefaultCellStyle = dataGridViewCellStyle15;
            this.dgvScatterWithin.Size = new System.Drawing.Size(772, 338);
            this.dgvScatterWithin.TabIndex = 1;
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.dgvScatterBetween);
            this.groupBox17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox17.Location = new System.Drawing.Point(540, 4);
            this.groupBox17.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox17.Size = new System.Drawing.Size(780, 364);
            this.groupBox17.TabIndex = 2;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "Between Class Scatter Matrix";
            // 
            // dgvScatterBetween
            // 
            this.dgvScatterBetween.AllowUserToAddRows = false;
            this.dgvScatterBetween.AllowUserToDeleteRows = false;
            this.dgvScatterBetween.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvScatterBetween.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScatterBetween.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScatterBetween.ColumnHeadersVisible = false;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle16.Format = "N4";
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvScatterBetween.DefaultCellStyle = dataGridViewCellStyle16;
            this.dgvScatterBetween.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScatterBetween.Location = new System.Drawing.Point(4, 23);
            this.dgvScatterBetween.Margin = new System.Windows.Forms.Padding(4);
            this.dgvScatterBetween.Name = "dgvScatterBetween";
            this.dgvScatterBetween.ReadOnly = true;
            this.dgvScatterBetween.RowHeadersVisible = false;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvScatterBetween.RowsDefaultCellStyle = dataGridViewCellStyle17;
            this.dgvScatterBetween.Size = new System.Drawing.Size(772, 337);
            this.dgvScatterBetween.TabIndex = 1;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.dgvScatterTotal);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(4, 4);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Padding = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel3.SetRowSpan(this.groupBox13, 2);
            this.groupBox13.Size = new System.Drawing.Size(528, 737);
            this.groupBox13.TabIndex = 1;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Total Scatter Matrix";
            // 
            // dgvScatterTotal
            // 
            this.dgvScatterTotal.AllowUserToAddRows = false;
            this.dgvScatterTotal.AllowUserToDeleteRows = false;
            this.dgvScatterTotal.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvScatterTotal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScatterTotal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScatterTotal.ColumnHeadersVisible = false;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle18.Format = "N4";
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvScatterTotal.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvScatterTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScatterTotal.Location = new System.Drawing.Point(4, 23);
            this.dgvScatterTotal.Margin = new System.Windows.Forms.Padding(4);
            this.dgvScatterTotal.Name = "dgvScatterTotal";
            this.dgvScatterTotal.ReadOnly = true;
            this.dgvScatterTotal.RowHeadersVisible = false;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvScatterTotal.RowsDefaultCellStyle = dataGridViewCellStyle19;
            this.dgvScatterTotal.Size = new System.Drawing.Size(520, 710);
            this.dgvScatterTotal.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.55704F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.44296F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox14, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.groupBox9, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.groupBox10, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.dgvClassHistogram, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.35602F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.64398F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1332, 753);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.dgvScatter);
            this.groupBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox14.Location = new System.Drawing.Point(4, 398);
            this.groupBox14.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox14.Size = new System.Drawing.Size(412, 351);
            this.groupBox14.TabIndex = 1;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Scatter Matrix";
            // 
            // dgvScatter
            // 
            this.dgvScatter.AllowUserToAddRows = false;
            this.dgvScatter.AllowUserToDeleteRows = false;
            this.dgvScatter.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvScatter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScatter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScatter.ColumnHeadersVisible = false;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle20.Format = "N4";
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvScatter.DefaultCellStyle = dataGridViewCellStyle20;
            this.dgvScatter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScatter.Location = new System.Drawing.Point(4, 23);
            this.dgvScatter.Margin = new System.Windows.Forms.Padding(4);
            this.dgvScatter.Name = "dgvScatter";
            this.dgvScatter.ReadOnly = true;
            this.dgvScatter.RowHeadersVisible = false;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvScatter.RowsDefaultCellStyle = dataGridViewCellStyle21;
            this.dgvScatter.Size = new System.Drawing.Size(404, 324);
            this.dgvScatter.TabIndex = 1;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dgvClasses);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(4, 4);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox9.Size = new System.Drawing.Size(412, 386);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Classes";
            // 
            // dgvClasses
            // 
            this.dgvClasses.AllowUserToAddRows = false;
            this.dgvClasses.AllowUserToDeleteRows = false;
            this.dgvClasses.AllowUserToResizeRows = false;
            this.dgvClasses.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClasses.BackgroundColor = System.Drawing.Color.White;
            this.dgvClasses.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle22.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClasses.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle22;
            this.dgvClasses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClasses.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.colPrevalence,
            this.colCount});
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvClasses.DefaultCellStyle = dataGridViewCellStyle24;
            this.dgvClasses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClasses.Location = new System.Drawing.Point(4, 23);
            this.dgvClasses.Margin = new System.Windows.Forms.Padding(4);
            this.dgvClasses.Name = "dgvClasses";
            this.dgvClasses.ReadOnly = true;
            this.dgvClasses.RowHeadersVisible = false;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvClasses.RowsDefaultCellStyle = dataGridViewCellStyle25;
            this.dgvClasses.Size = new System.Drawing.Size(404, 359);
            this.dgvClasses.TabIndex = 2;
            this.dgvClasses.CurrentCellChanged += new System.EventHandler(this.dgvClasses_CurrentCellChanged);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Number";
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle23;
            this.dataGridViewTextBoxColumn3.FillWeight = 10F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Number";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 83;
            // 
            // colPrevalence
            // 
            this.colPrevalence.DataPropertyName = "Prevalence";
            this.colPrevalence.HeaderText = "Prevalence";
            this.colPrevalence.Name = "colPrevalence";
            this.colPrevalence.ReadOnly = true;
            // 
            // colCount
            // 
            this.colCount.DataPropertyName = "Count";
            this.colCount.HeaderText = "Count";
            this.colCount.Name = "colCount";
            this.colCount.ReadOnly = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.dgvClassData);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(424, 4);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox10.Size = new System.Drawing.Size(904, 386);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Sample Subset";
            // 
            // dgvClassData
            // 
            this.dgvClassData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClassData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClassData.Location = new System.Drawing.Point(4, 23);
            this.dgvClassData.Margin = new System.Windows.Forms.Padding(4);
            this.dgvClassData.Name = "dgvClassData";
            this.dgvClassData.Size = new System.Drawing.Size(896, 359);
            this.dgvClassData.TabIndex = 0;
            this.dgvClassData.CurrentCellChanged += new System.EventHandler(this.dgvClassData_CurrentCellChanged);
            // 
            // dgvClassHistogram
            // 
            this.dgvClassHistogram.Controls.Add(this.dataHistogramView2);
            this.dgvClassHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClassHistogram.Location = new System.Drawing.Point(424, 398);
            this.dgvClassHistogram.Margin = new System.Windows.Forms.Padding(4);
            this.dgvClassHistogram.Name = "dgvClassHistogram";
            this.dgvClassHistogram.Padding = new System.Windows.Forms.Padding(4);
            this.dgvClassHistogram.Size = new System.Drawing.Size(904, 351);
            this.dgvClassHistogram.TabIndex = 1;
            this.dgvClassHistogram.TabStop = false;
            this.dgvClassHistogram.Text = "Histogram";
            // 
            // dataHistogramView2
            // 
            this.dataHistogramView2.BinWidth = null;
            this.dataHistogramView2.DataSource = ((object)(resources.GetObject("dataHistogramView2.DataSource")));
            this.dataHistogramView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataHistogramView2.Histogram = ((Accord.Statistics.Visualizations.Histogram)(resources.GetObject("dataHistogramView2.Histogram")));
            this.dataHistogramView2.Location = new System.Drawing.Point(4, 23);
            this.dataHistogramView2.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.dataHistogramView2.Name = "dataHistogramView2";
            this.dataHistogramView2.NumberOfBins = null;
            this.dataHistogramView2.Size = new System.Drawing.Size(896, 324);
            this.dataHistogramView2.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.73716F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.26284F));
            this.tableLayoutPanel5.Controls.Add(this.btnProject2, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.groupBox16, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.groupBox4, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1324, 745);
            this.tableLayoutPanel5.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1340, 851);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Linear Discriminant Analysis";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabShift.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectionSource)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectionResult)).EndInit();
            this.groupBox16.ResumeLayout(false);
            this.tabOverview.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrincipalComponents)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeatureVectors)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.tabDistribution.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDistributionMeasures)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.tabSamples.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnalysisSource)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox18.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterWithin)).EndInit();
            this.groupBox17.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterBetween)).EndInit();
            this.groupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatterTotal)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScatter)).EndInit();
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClasses)).EndInit();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClassData)).EndInit();
            this.dgvClassHistogram.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripComboBox tscbMethod;
        private System.Windows.Forms.ToolStripButton btnRunAnalysis;
        private System.Windows.Forms.ToolStripMenuItem MenuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.TabPage tabShift;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.DataGridView dgvProjectionSource;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dgvProjectionResult;
        private System.Windows.Forms.TabPage tabOverview;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvPrincipalComponents;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvFeatureVectors;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TabPage tabDistribution;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.DataGridView dgvDistributionMeasures;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.TabPage tabSamples;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridView dgvAnalysisSource;
        private System.Windows.Forms.Button btnSampleRunAnalysis;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.GroupBox groupBox16;
        private Accord.Controls.HistogramView dataHistogramView1;
        private System.Windows.Forms.Button btnProject2;
        private System.Windows.Forms.SplitContainer splitContainer7;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComponent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEigenValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProportion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCumulativeProportion;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.TabPage tabPage4;
        private Accord.Controls.ScatterplotView inputScatterplot;
        private Accord.Controls.ScatterplotView outputScatterplot;
        private Accord.Controls.ComponentView distributionView;
        private Accord.Controls.ComponentView cumulativeView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox18;
        private System.Windows.Forms.DataGridView dgvScatterWithin;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.DataGridView dgvScatterBetween;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.DataGridView dgvScatterTotal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.DataGridView dgvScatter;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.DataGridView dgvClasses;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrevalence;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCount;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.DataGridView dgvClassData;
        private System.Windows.Forms.GroupBox dgvClassHistogram;
        private Accord.Controls.HistogramView dataHistogramView2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    }
}

