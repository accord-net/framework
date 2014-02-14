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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSamples = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
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
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSamples.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSource)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIncrement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).BeginInit();
            this.tabDistribution.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(737, 24);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // MenuFileOpen
            // 
            this.MenuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("MenuFileOpen.Image")));
            this.MenuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuFileOpen.Name = "MenuFileOpen";
            this.MenuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuFileOpen.Size = new System.Drawing.Size(152, 22);
            this.MenuFileOpen.Text = "&Open";
            this.MenuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSamples);
            this.tabControl.Controls.Add(this.tabDistribution);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(737, 441);
            this.tabControl.TabIndex = 8;
            // 
            // tabSamples
            // 
            this.tabSamples.Controls.Add(this.splitContainer3);
            this.tabSamples.Location = new System.Drawing.Point(4, 22);
            this.tabSamples.Name = "tabSamples";
            this.tabSamples.Padding = new System.Windows.Forms.Padding(3);
            this.tabSamples.Size = new System.Drawing.Size(729, 415);
            this.tabSamples.TabIndex = 0;
            this.tabSamples.Text = "Curve Creation";
            this.tabSamples.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox7);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox6);
            this.splitContainer3.Size = new System.Drawing.Size(723, 409);
            this.splitContainer3.SplitterDistance = 255;
            this.splitContainer3.TabIndex = 7;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvSource);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(255, 409);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Inputs";
            // 
            // dgvSource
            // 
            this.dgvSource.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSource.Location = new System.Drawing.Point(3, 16);
            this.dgvSource.Name = "dgvSource";
            this.dgvSource.Size = new System.Drawing.Size(249, 390);
            this.dgvSource.TabIndex = 5;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.scatterplotView1);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.tbError);
            this.groupBox6.Controls.Add(this.tbArea);
            this.groupBox6.Controls.Add(this.numIncrement);
            this.groupBox6.Controls.Add(this.numPoints);
            this.groupBox6.Controls.Add(this.rbThreshold);
            this.groupBox6.Controls.Add(this.rbNumPoints);
            this.groupBox6.Controls.Add(this.btnPlot);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(0, 0);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(464, 409);
            this.groupBox6.TabIndex = 6;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Receiver Operating Characteristic Curve";
            // 
            // scatterplotView1
            // 
            this.scatterplotView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scatterplotView1.LinesVisible = true;
            this.scatterplotView1.Location = new System.Drawing.Point(3, 16);
            this.scatterplotView1.Name = "scatterplotView1";
            this.scatterplotView1.ScaleTight = true;
            this.scatterplotView1.Size = new System.Drawing.Size(458, 309);
            this.scatterplotView1.SymbolSize = 0F;
            this.scatterplotView1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 334);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Error:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 334);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Area:";
            // 
            // tbError
            // 
            this.tbError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbError.Location = new System.Drawing.Point(309, 331);
            this.tbError.Name = "tbError";
            this.tbError.Size = new System.Drawing.Size(147, 20);
            this.tbError.TabIndex = 5;
            // 
            // tbArea
            // 
            this.tbArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbArea.Location = new System.Drawing.Point(44, 331);
            this.tbArea.Name = "tbArea";
            this.tbArea.Size = new System.Drawing.Size(201, 20);
            this.tbArea.TabIndex = 5;
            // 
            // numIncrement
            // 
            this.numIncrement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numIncrement.DecimalPlaces = 2;
            this.numIncrement.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numIncrement.Location = new System.Drawing.Point(393, 383);
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
            this.numIncrement.Size = new System.Drawing.Size(66, 20);
            this.numIncrement.TabIndex = 4;
            this.numIncrement.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // numPoints
            // 
            this.numPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numPoints.Location = new System.Drawing.Point(393, 357);
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
            this.numPoints.Size = new System.Drawing.Size(66, 20);
            this.numPoints.TabIndex = 4;
            this.numPoints.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // rbThreshold
            // 
            this.rbThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbThreshold.AutoSize = true;
            this.rbThreshold.Location = new System.Drawing.Point(263, 383);
            this.rbThreshold.Name = "rbThreshold";
            this.rbThreshold.Size = new System.Drawing.Size(124, 17);
            this.rbThreshold.TabIndex = 3;
            this.rbThreshold.Text = "Threshold increment:";
            this.rbThreshold.UseVisualStyleBackColor = true;
            // 
            // rbNumPoints
            // 
            this.rbNumPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbNumPoints.AutoSize = true;
            this.rbNumPoints.Checked = true;
            this.rbNumPoints.Location = new System.Drawing.Point(263, 357);
            this.rbNumPoints.Name = "rbNumPoints";
            this.rbNumPoints.Size = new System.Drawing.Size(108, 17);
            this.rbNumPoints.TabIndex = 3;
            this.rbNumPoints.TabStop = true;
            this.rbNumPoints.Text = "Number of points:";
            this.rbNumPoints.UseVisualStyleBackColor = true;
            // 
            // btnPlot
            // 
            this.btnPlot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlot.Location = new System.Drawing.Point(6, 357);
            this.btnPlot.Name = "btnPlot";
            this.btnPlot.Size = new System.Drawing.Size(251, 46);
            this.btnPlot.TabIndex = 1;
            this.btnPlot.Text = "Plot Curve";
            this.btnPlot.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlot.UseVisualStyleBackColor = true;
            this.btnPlot.Click += new System.EventHandler(this.btnRunAnalysis_Click);
            // 
            // tabDistribution
            // 
            this.tabDistribution.Controls.Add(this.groupBox11);
            this.tabDistribution.Location = new System.Drawing.Point(4, 22);
            this.tabDistribution.Name = "tabDistribution";
            this.tabDistribution.Padding = new System.Windows.Forms.Padding(3);
            this.tabDistribution.Size = new System.Drawing.Size(729, 415);
            this.tabDistribution.TabIndex = 7;
            this.tabDistribution.Text = "Curve Point Details";
            this.tabDistribution.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dgvPointDetails);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(3, 3);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(723, 409);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPointDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
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
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.Format = "N5";
            dataGridViewCellStyle6.NullValue = null;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPointDetails.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvPointDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPointDetails.Location = new System.Drawing.Point(3, 16);
            this.dgvPointDetails.Name = "dgvPointDetails";
            this.dgvPointDetails.ReadOnly = true;
            this.dgvPointDetails.RowHeadersVisible = false;
            this.dgvPointDetails.Size = new System.Drawing.Size(717, 390);
            this.dgvPointDetails.TabIndex = 1;
            // 
            // colCutoff
            // 
            this.colCutoff.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCutoff.DataPropertyName = "Cutoff";
            this.colCutoff.HeaderText = "Cutoff Value";
            this.colCutoff.Name = "colCutoff";
            this.colCutoff.ReadOnly = true;
            this.colCutoff.Width = 76;
            // 
            // colTruePositive
            // 
            this.colTruePositive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTruePositive.DataPropertyName = "TruePositives";
            dataGridViewCellStyle2.Format = "N0";
            this.colTruePositive.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTruePositive.HeaderText = "True Positives";
            this.colTruePositive.Name = "colTruePositive";
            this.colTruePositive.ReadOnly = true;
            this.colTruePositive.Width = 82;
            // 
            // colTrueNegative
            // 
            this.colTrueNegative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTrueNegative.DataPropertyName = "TrueNegatives";
            dataGridViewCellStyle3.Format = "N0";
            this.colTrueNegative.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTrueNegative.HeaderText = "True Negatives";
            this.colTrueNegative.Name = "colTrueNegative";
            this.colTrueNegative.ReadOnly = true;
            this.colTrueNegative.Width = 86;
            // 
            // colFalsePositive
            // 
            this.colFalsePositive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalsePositive.DataPropertyName = "FalsePositives";
            dataGridViewCellStyle4.Format = "N0";
            this.colFalsePositive.DefaultCellStyle = dataGridViewCellStyle4;
            this.colFalsePositive.HeaderText = "False Positives";
            this.colFalsePositive.Name = "colFalsePositive";
            this.colFalsePositive.ReadOnly = true;
            this.colFalsePositive.Width = 87;
            // 
            // colFalseNegative
            // 
            this.colFalseNegative.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalseNegative.DataPropertyName = "FalseNegatives";
            dataGridViewCellStyle5.Format = "N0";
            dataGridViewCellStyle5.NullValue = null;
            this.colFalseNegative.DefaultCellStyle = dataGridViewCellStyle5;
            this.colFalseNegative.HeaderText = "False Negatives";
            this.colFalseNegative.Name = "colFalseNegative";
            this.colFalseNegative.ReadOnly = true;
            this.colFalseNegative.Width = 90;
            // 
            // colSpecificity
            // 
            this.colSpecificity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSpecificity.DataPropertyName = "Specificity";
            this.colSpecificity.HeaderText = "Specificity";
            this.colSpecificity.Name = "colSpecificity";
            this.colSpecificity.ReadOnly = true;
            this.colSpecificity.Width = 73;
            // 
            // colSensitivity
            // 
            this.colSensitivity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSensitivity.DataPropertyName = "Sensitivity";
            this.colSensitivity.HeaderText = "Sensitivity";
            this.colSensitivity.Name = "colSensitivity";
            this.colSensitivity.ReadOnly = true;
            this.colSensitivity.Width = 74;
            // 
            // colEfficiency
            // 
            this.colEfficiency.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEfficiency.DataPropertyName = "Efficiency";
            this.colEfficiency.HeaderText = "Efficiency";
            this.colEfficiency.Name = "colEfficiency";
            this.colEfficiency.ReadOnly = true;
            this.colEfficiency.Width = 71;
            // 
            // colAccuracy
            // 
            this.colAccuracy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAccuracy.DataPropertyName = "Accuracy";
            this.colAccuracy.HeaderText = "Accuracy";
            this.colAccuracy.Name = "colAccuracy";
            this.colAccuracy.ReadOnly = true;
            this.colAccuracy.Width = 70;
            // 
            // colPrecision
            // 
            this.colPrecision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colPrecision.DataPropertyName = "PositivePredictiveValue";
            this.colPrecision.HeaderText = "Positive Predictive Value";
            this.colPrecision.Name = "colPrecision";
            this.colPrecision.ReadOnly = true;
            this.colPrecision.Width = 101;
            // 
            // colNegativePrecision
            // 
            this.colNegativePrecision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colNegativePrecision.DataPropertyName = "NegativePredictiveValue";
            this.colNegativePrecision.HeaderText = "Negative Predictive Value";
            this.colNegativePrecision.Name = "colNegativePrecision";
            this.colNegativePrecision.ReadOnly = true;
            this.colNegativePrecision.Width = 105;
            // 
            // colFalseAlarmRate
            // 
            this.colFalseAlarmRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colFalseAlarmRate.DataPropertyName = "FalsePositiveRate";
            this.colFalseAlarmRate.HeaderText = "False Positive Rate";
            this.colFalseAlarmRate.Name = "colFalseAlarmRate";
            this.colFalseAlarmRate.ReadOnly = true;
            this.colFalseAlarmRate.ToolTipText = "Also known as False Alarm Rate";
            this.colFalseAlarmRate.Width = 102;
            // 
            // colMatthews
            // 
            this.colMatthews.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colMatthews.DataPropertyName = "MatthewsCorrelationCoefficient";
            this.colMatthews.HeaderText = "Phi";
            this.colMatthews.Name = "colMatthews";
            this.colMatthews.ReadOnly = true;
            this.colMatthews.ToolTipText = "As known as Matthews Correlation Coefficient";
            this.colMatthews.Width = 43;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xls";
            this.openFileDialog.Filter = "Xml Files (*.xml) |*.xml|Excel files (*.xls)|*.xls|Text files (*.txt)|*.txt|All f" +
    "iles (*.*)|*.*";
            this.openFileDialog.FilterIndex = 2;
            this.openFileDialog.Title = "Open file";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 465);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Receiver Operating Characteristic Curve Demonstration";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabSamples.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSource)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIncrement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).EndInit();
            this.tabDistribution.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPointDetails)).EndInit();
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
        private System.Windows.Forms.SplitContainer splitContainer3;
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
    }
}