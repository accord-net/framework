namespace Performance.ROC
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSamples = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvSource = new System.Windows.Forms.DataGridView();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.scatterplotView1 = new Accord.Controls.ScatterplotView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbError = new System.Windows.Forms.TextBox();
            this.tbArea = new System.Windows.Forms.TextBox();
            this.numIncrement = new System.Windows.Forms.NumericUpDown();
            this.numPoints = new System.Windows.Forms.NumericUpDown();
            this.rbThreshold = new System.Windows.Forms.RadioButton();
            this.rbNumPoints = new System.Windows.Forms.RadioButton();
            this.btnPlot = new System.Windows.Forms.Button();
            this.tabDistribution = new System.Windows.Forms.TabPage();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.dgvPointDetails = new System.Windows.Forms.DataGridView();
            this.colCutoff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTruePositive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTrueNegative = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFalsePositive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFalseNegative = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSpecificity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensitivity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEfficiency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccuracy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNegativePrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFalseAlarmRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMatthews = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSamples.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSource)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIncrement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).BeginInit();
            this.tabDistribution.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDetails)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1461, 35);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFileOpen,
            this.toolStripSeparator,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // MenuFileOpen
            // 
            this.MenuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileOpen.Image")));
            this.MenuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuFileOpen.Name = "MenuFileOpen";
            this.MenuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuFileOpen.Size = new System.Drawing.Size(201, 30);
            this.MenuFileOpen.Text = "&Open";
            this.MenuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(198, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 30);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(146, 30);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSamples);
            this.tabControl.Controls.Add(this.tabDistribution);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 35);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1461, 812);
            this.tabControl.TabIndex = 8;
            // 
            // tabSamples
            // 
            this.tabSamples.Controls.Add(this.tableLayoutPanel1);
            this.tabSamples.Location = new System.Drawing.Point(4, 29);
            this.tabSamples.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabSamples.Name = "tabSamples";
            this.tabSamples.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabSamples.Size = new System.Drawing.Size(1453, 779);
            this.tabSamples.TabIndex = 0;
            this.tabSamples.Text = "Curve Creation";
            this.tabSamples.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvSource);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(4, 4);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.SetRowSpan(this.groupBox7, 2);
            this.groupBox7.Size = new System.Drawing.Size(310, 763);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Inputs";
            // 
            // dgvSource
            // 
            this.dgvSource.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSource.Location = new System.Drawing.Point(4, 23);
            this.dgvSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvSource.Name = "dgvSource";
            this.dgvSource.Size = new System.Drawing.Size(302, 736);
            this.dgvSource.TabIndex = 5;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.scatterplotView1);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(322, 4);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(1119, 586);
            this.groupBox6.TabIndex = 6;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Receiver Operating Characteristic Curve";
            // 
            // scatterplotView1
            // 
            this.scatterplotView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scatterplotView1.LinesVisible = true;
            this.scatterplotView1.Location = new System.Drawing.Point(4, 23);
            this.scatterplotView1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.scatterplotView1.Name = "scatterplotView1";
            this.scatterplotView1.ScaleTight = true;
            this.scatterplotView1.Size = new System.Drawing.Size(1111, 559);
            this.scatterplotView1.SymbolSize = 0F;
            this.scatterplotView1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(827, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Error:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Area:";
            // 
            // tbError
            // 
            this.tbError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbError.Location = new System.Drawing.Point(885, 36);
            this.tbError.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbError.Name = "tbError";
            this.tbError.Size = new System.Drawing.Size(218, 26);
            this.tbError.TabIndex = 5;
            // 
            // tbArea
            // 
            this.tbArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbArea.Location = new System.Drawing.Point(64, 36);
            this.tbArea.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbArea.Name = "tbArea";
            this.tbArea.Size = new System.Drawing.Size(742, 26);
            this.tbArea.TabIndex = 5;
            // 
            // numIncrement
            // 
            this.numIncrement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numIncrement.DecimalPlaces = 2;
            this.numIncrement.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numIncrement.Location = new System.Drawing.Point(1011, 114);
            this.numIncrement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numIncrement.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numIncrement.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numIncrement.Name = "numIncrement";
            this.numIncrement.Size = new System.Drawing.Size(99, 26);
            this.numIncrement.TabIndex = 4;
            this.numIncrement.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // numPoints
            // 
            this.numPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numPoints.Location = new System.Drawing.Point(1011, 76);
            this.numPoints.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numPoints.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPoints.Name = "numPoints";
            this.numPoints.Size = new System.Drawing.Size(99, 26);
            this.numPoints.TabIndex = 4;
            this.numPoints.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // rbThreshold
            // 
            this.rbThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbThreshold.AutoSize = true;
            this.rbThreshold.Location = new System.Drawing.Point(820, 116);
            this.rbThreshold.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbThreshold.Name = "rbThreshold";
            this.rbThreshold.Size = new System.Drawing.Size(182, 24);
            this.rbThreshold.TabIndex = 3;
            this.rbThreshold.Text = "Threshold increment:";
            this.rbThreshold.UseVisualStyleBackColor = true;
            // 
            // rbNumPoints
            // 
            this.rbNumPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbNumPoints.AutoSize = true;
            this.rbNumPoints.Checked = true;
            this.rbNumPoints.Location = new System.Drawing.Point(818, 77);
            this.rbNumPoints.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbNumPoints.Name = "rbNumPoints";
            this.rbNumPoints.Size = new System.Drawing.Size(159, 24);
            this.rbNumPoints.TabIndex = 3;
            this.rbNumPoints.TabStop = true;
            this.rbNumPoints.Text = "Number of points:";
            this.rbNumPoints.UseVisualStyleBackColor = true;
            // 
            // btnPlot
            // 
            this.btnPlot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlot.Location = new System.Drawing.Point(7, 76);
            this.btnPlot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPlot.Name = "btnPlot";
            this.btnPlot.Size = new System.Drawing.Size(799, 69);
            this.btnPlot.TabIndex = 1;
            this.btnPlot.Text = "Plot Curve";
            this.btnPlot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlot.UseVisualStyleBackColor = true;
            this.btnPlot.Click += new System.EventHandler(this.btnRunAnalysis_Click);
            // 
            // tabDistribution
            // 
            this.tabDistribution.Controls.Add(this.groupBox11);
            this.tabDistribution.Location = new System.Drawing.Point(4, 29);
            this.tabDistribution.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabDistribution.Name = "tabDistribution";
            this.tabDistribution.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabDistribution.Size = new System.Drawing.Size(1453, 779);
            this.tabDistribution.TabIndex = 7;
            this.tabDistribution.Text = "Curve Point Details";
            this.tabDistribution.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dgvPointDetails);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(4, 4);
            this.groupBox11.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox11.Size = new System.Drawing.Size(1445, 771);
            this.groupBox11.TabIndex = 2;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Point details";
            // 
            // dgvPointDetails
            // 
            this.dgvPointDetails.AllowUserToAddRows = false;
            this.dgvPointDetails.AllowUserToDeleteRows = false;
            this.dgvPointDetails.AllowUserToOrderColumns = true;
            this.dgvPointDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPointDetails.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvPointDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPointDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPointDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPointDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCutoff,
            this.colTruePositive,
            this.colTrueNegative,
            this.colFalsePositive,
            this.colFalseNegative,
            this.colSpecificity,
            this.colSensitivity,
            this.colEfficiency,
            this.colAccuracy,
            this.colPrecision,
            this.colNegativePrecision,
            this.colFalseAlarmRate,
            this.colMatthews});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.Format = "N5";
            dataGridViewCellStyle12.NullValue = null;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPointDetails.DefaultCellStyle = dataGridViewCellStyle12;
            this.dgvPointDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPointDetails.Location = new System.Drawing.Point(4, 23);
            this.dgvPointDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvPointDetails.Name = "dgvPointDetails";
            this.dgvPointDetails.ReadOnly = true;
            this.dgvPointDetails.RowHeadersVisible = false;
            this.dgvPointDetails.Size = new System.Drawing.Size(1437, 744);
            this.dgvPointDetails.TabIndex = 1;
            // 
            // colCutoff
            // 
            this.colCutoff.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCutoff.DataPropertyName = "Cutoff";
            this.colCutoff.HeaderText = "Cutoff Value";
            this.colCutoff.Name = "colCutoff";
            this.colCutoff.ReadOnly = true;
            this.colCutoff.Width = 101;
            // 
            // colTruePositive
            // 
            this.colTruePositive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTruePositive.DataPropertyName = "TruePositives";
            dataGridViewCellStyle8.Format = "N0";
            this.colTruePositive.DefaultCellStyle = dataGridViewCellStyle8;
            this.colTruePositive.HeaderText = "True Positives";
            this.colTruePositive.Name = "colTruePositive";
            this.colTruePositive.ReadOnly = true;
            this.colTruePositive.Width = 113;
            // 
            // colTrueNegative
            // 
            this.colTrueNegative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTrueNegative.DataPropertyName = "TrueNegatives";
            dataGridViewCellStyle9.Format = "N0";
            this.colTrueNegative.DefaultCellStyle = dataGridViewCellStyle9;
            this.colTrueNegative.HeaderText = "True Negatives";
            this.colTrueNegative.Name = "colTrueNegative";
            this.colTrueNegative.ReadOnly = true;
            this.colTrueNegative.Width = 119;
            // 
            // colFalsePositive
            // 
            this.colFalsePositive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalsePositive.DataPropertyName = "FalsePositives";
            dataGridViewCellStyle10.Format = "N0";
            this.colFalsePositive.DefaultCellStyle = dataGridViewCellStyle10;
            this.colFalsePositive.HeaderText = "False Positives";
            this.colFalsePositive.Name = "colFalsePositive";
            this.colFalsePositive.ReadOnly = true;
            this.colFalsePositive.Width = 116;
            // 
            // colFalseNegative
            // 
            this.colFalseNegative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalseNegative.DataPropertyName = "FalseNegatives";
            dataGridViewCellStyle11.Format = "N0";
            dataGridViewCellStyle11.NullValue = null;
            this.colFalseNegative.DefaultCellStyle = dataGridViewCellStyle11;
            this.colFalseNegative.HeaderText = "False Negatives";
            this.colFalseNegative.Name = "colFalseNegative";
            this.colFalseNegative.ReadOnly = true;
            this.colFalseNegative.Width = 123;
            // 
            // colSpecificity
            // 
            this.colSpecificity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSpecificity.DataPropertyName = "Specificity";
            this.colSpecificity.HeaderText = "Specificity";
            this.colSpecificity.Name = "colSpecificity";
            this.colSpecificity.ReadOnly = true;
            this.colSpecificity.Width = 96;
            // 
            // colSensitivity
            // 
            this.colSensitivity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSensitivity.DataPropertyName = "Sensitivity";
            this.colSensitivity.HeaderText = "Sensitivity";
            this.colSensitivity.Name = "colSensitivity";
            this.colSensitivity.ReadOnly = true;
            this.colSensitivity.Width = 96;
            // 
            // colEfficiency
            // 
            this.colEfficiency.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEfficiency.DataPropertyName = "Efficiency";
            this.colEfficiency.HeaderText = "Efficiency";
            this.colEfficiency.Name = "colEfficiency";
            this.colEfficiency.ReadOnly = true;
            this.colEfficiency.Width = 93;
            // 
            // colAccuracy
            // 
            this.colAccuracy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAccuracy.DataPropertyName = "Accuracy";
            this.colAccuracy.HeaderText = "Accuracy";
            this.colAccuracy.Name = "colAccuracy";
            this.colAccuracy.ReadOnly = true;
            this.colAccuracy.Width = 91;
            // 
            // colPrecision
            // 
            this.colPrecision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colPrecision.DataPropertyName = "PositivePredictiveValue";
            this.colPrecision.HeaderText = "Positive Predictive Value";
            this.colPrecision.Name = "colPrecision";
            this.colPrecision.ReadOnly = true;
            this.colPrecision.Width = 139;
            // 
            // colNegativePrecision
            // 
            this.colNegativePrecision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colNegativePrecision.DataPropertyName = "NegativePredictiveValue";
            this.colNegativePrecision.HeaderText = "Negative Predictive Value";
            this.colNegativePrecision.Name = "colNegativePrecision";
            this.colNegativePrecision.ReadOnly = true;
            this.colNegativePrecision.Width = 145;
            // 
            // colFalseAlarmRate
            // 
            this.colFalseAlarmRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalseAlarmRate.DataPropertyName = "FalsePositiveRate";
            this.colFalseAlarmRate.HeaderText = "False Positive Rate";
            this.colFalseAlarmRate.Name = "colFalseAlarmRate";
            this.colFalseAlarmRate.ReadOnly = true;
            this.colFalseAlarmRate.ToolTipText = "Also known as False Alarm Rate";
            this.colFalseAlarmRate.Width = 141;
            // 
            // colMatthews
            // 
            this.colMatthews.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colMatthews.DataPropertyName = "MatthewsCorrelationCoefficient";
            this.colMatthews.HeaderText = "Phi";
            this.colMatthews.Name = "colMatthews";
            this.colMatthews.ReadOnly = true;
            this.colMatthews.ToolTipText = "As known as Matthews Correlation Coefficient";
            this.colMatthews.Width = 53;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xls";
            this.openFileDialog.Filter = "Xml Files (*.xml) |*.xml|Excel files (*.xls)|*.xls|Text files (*.txt)|*.txt|All f" +
    "iles (*.*)|*.*";
            this.openFileDialog.FilterIndex = 2;
            this.openFileDialog.Title = "Open file";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.07612F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.92387F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox6, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1445, 771);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.tbArea);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnPlot);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbNumPoints);
            this.groupBox1.Controls.Add(this.tbError);
            this.groupBox1.Controls.Add(this.rbThreshold);
            this.groupBox1.Controls.Add(this.numPoints);
            this.groupBox1.Controls.Add(this.numIncrement);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(321, 597);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1121, 171);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Curve creation";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1461, 847);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "Receiver Operating Characteristic Curve Demonstration";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabSamples.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSource)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numIncrement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).EndInit();
            this.tabDistribution.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDetails)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSamples;
        private System.Windows.Forms.DataGridView dgvSource;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnPlot;
        private System.Windows.Forms.TabPage tabDistribution;
        //   private Sinapse.Forms.Controls.Controls.DataHistogramView dataHistogramView2;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.DataGridView dgvPointDetails;
        private System.Windows.Forms.RadioButton rbNumPoints;
        private System.Windows.Forms.NumericUpDown numIncrement;
        private System.Windows.Forms.NumericUpDown numPoints;
        private System.Windows.Forms.RadioButton rbThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbError;
        private System.Windows.Forms.TextBox tbArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCutoff;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTruePositive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTrueNegative;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFalsePositive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFalseNegative;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpecificity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensitivity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEfficiency;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccuracy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNegativePrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFalseAlarmRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMatthews;
        private Accord.Controls.ScatterplotView scatterplotView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}