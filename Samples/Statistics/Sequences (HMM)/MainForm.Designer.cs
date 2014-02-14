namespace Sequences.HMMs
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgvSequenceSource = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numConvergence = new System.Windows.Forms.NumericUpDown();
            this.rbStopIterations = new System.Windows.Forms.RadioButton();
            this.numIterations = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.rbStopConvergence = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvProbabilities = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvModels = new System.Windows.Forms.DataGridView();
            this.colModelsTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colModelsStates = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvTransitions = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvEmissions = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dgvTesting = new System.Windows.Forms.DataGridView();
            this.colTestSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestTrueClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestAssignedClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestLikelihood = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestMatch = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSequenceSource)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConvergence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProbabilities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvModels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransitions)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmissions)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTesting)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(749, 418);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(741, 392);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input Sequences";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 303);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(561, 81);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.dgvSequenceSource);
            this.groupBox5.Location = new System.Drawing.Point(6, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(727, 296);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sequences";
            // 
            // dgvSequenceSource
            // 
            this.dgvSequenceSource.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSequenceSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSequenceSource.Location = new System.Drawing.Point(3, 16);
            this.dgvSequenceSource.Name = "dgvSequenceSource";
            this.dgvSequenceSource.Size = new System.Drawing.Size(721, 277);
            this.dgvSequenceSource.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(578, 303);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 81);
            this.button1.TabIndex = 1;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(741, 392);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Hidden Markov Model Ensemble (Sequence Classifier)";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.numConvergence);
            this.groupBox7.Controls.Add(this.rbStopIterations);
            this.groupBox7.Controls.Add(this.numIterations);
            this.groupBox7.Controls.Add(this.button2);
            this.groupBox7.Controls.Add(this.rbStopConvergence);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox7.Location = new System.Drawing.Point(602, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(136, 386);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Training";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Stopping criteria:";
            // 
            // numConvergence
            // 
            this.numConvergence.DecimalPlaces = 5;
            this.numConvergence.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numConvergence.Location = new System.Drawing.Point(26, 136);
            this.numConvergence.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numConvergence.Name = "numConvergence";
            this.numConvergence.Size = new System.Drawing.Size(96, 20);
            this.numConvergence.TabIndex = 4;
            // 
            // rbStopIterations
            // 
            this.rbStopIterations.AutoSize = true;
            this.rbStopIterations.Checked = true;
            this.rbStopIterations.Location = new System.Drawing.Point(6, 64);
            this.rbStopIterations.Name = "rbStopIterations";
            this.rbStopIterations.Size = new System.Drawing.Size(120, 17);
            this.rbStopIterations.TabIndex = 3;
            this.rbStopIterations.TabStop = true;
            this.rbStopIterations.Text = "Number of Iterations";
            this.rbStopIterations.UseVisualStyleBackColor = true;
            // 
            // numIterations
            // 
            this.numIterations.Location = new System.Drawing.Point(26, 87);
            this.numIterations.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIterations.Name = "numIterations";
            this.numIterations.Size = new System.Drawing.Size(96, 20);
            this.numIterations.TabIndex = 4;
            this.numIterations.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(6, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 180);
            this.button2.TabIndex = 1;
            this.button2.Text = "Start!";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // rbStopConvergence
            // 
            this.rbStopConvergence.AutoSize = true;
            this.rbStopConvergence.Location = new System.Drawing.Point(6, 113);
            this.rbStopConvergence.Name = "rbStopConvergence";
            this.rbStopConvergence.Size = new System.Drawing.Size(113, 17);
            this.rbStopConvergence.TabIndex = 3;
            this.rbStopConvergence.TabStop = true;
            this.rbStopConvergence.Text = "Until Convergence";
            this.rbStopConvergence.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.dgvProbabilities);
            this.groupBox4.Location = new System.Drawing.Point(3, 319);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(596, 67);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "State Probabilities";
            // 
            // dgvProbabilities
            // 
            this.dgvProbabilities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProbabilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProbabilities.Location = new System.Drawing.Point(3, 16);
            this.dgvProbabilities.Name = "dgvProbabilities";
            this.dgvProbabilities.Size = new System.Drawing.Size(590, 48);
            this.dgvProbabilities.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(596, 310);
            this.splitContainer1.SplitterDistance = 251;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvModels);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 310);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Models";
            // 
            // dgvModels
            // 
            this.dgvModels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvModels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvModels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colModelsTag,
            this.colModelsStates});
            this.dgvModels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvModels.Location = new System.Drawing.Point(3, 16);
            this.dgvModels.Name = "dgvModels";
            this.dgvModels.Size = new System.Drawing.Size(245, 291);
            this.dgvModels.TabIndex = 1;
            this.dgvModels.CurrentCellChanged += new System.EventHandler(this.dgvModels_CurrentCellChanged);
            // 
            // colModelsTag
            // 
            this.colModelsTag.DataPropertyName = "Tag";
            this.colModelsTag.HeaderText = "Class";
            this.colModelsTag.Name = "colModelsTag";
            // 
            // colModelsStates
            // 
            this.colModelsStates.DataPropertyName = "States";
            this.colModelsStates.HeaderText = "States";
            this.colModelsStates.Name = "colModelsStates";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Size = new System.Drawing.Size(341, 310);
            this.splitContainer2.SplitterDistance = 140;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvTransitions);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(341, 140);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Transitions";
            // 
            // dgvTransitions
            // 
            this.dgvTransitions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransitions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTransitions.Location = new System.Drawing.Point(3, 16);
            this.dgvTransitions.Name = "dgvTransitions";
            this.dgvTransitions.Size = new System.Drawing.Size(335, 121);
            this.dgvTransitions.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgvEmissions);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(341, 166);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Emissions";
            // 
            // dgvEmissions
            // 
            this.dgvEmissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmissions.Location = new System.Drawing.Point(3, 16);
            this.dgvEmissions.Name = "dgvEmissions";
            this.dgvEmissions.Size = new System.Drawing.Size(335, 147);
            this.dgvEmissions.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.groupBox6);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(741, 392);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Classification Testing";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(621, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(114, 380);
            this.button3.TabIndex = 3;
            this.button3.Text = "Evaluate";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dgvTesting);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(612, 386);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Sequences";
            // 
            // dgvTesting
            // 
            this.dgvTesting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTesting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTestSequence,
            this.colTestTrueClass,
            this.colTestAssignedClass,
            this.colTestLikelihood,
            this.colTestMatch});
            this.dgvTesting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTesting.Location = new System.Drawing.Point(3, 16);
            this.dgvTesting.Name = "dgvTesting";
            this.dgvTesting.Size = new System.Drawing.Size(606, 367);
            this.dgvTesting.TabIndex = 1;
            // 
            // colTestSequence
            // 
            this.colTestSequence.HeaderText = "Sequence";
            this.colTestSequence.Name = "colTestSequence";
            // 
            // colTestTrueClass
            // 
            this.colTestTrueClass.HeaderText = "True Class";
            this.colTestTrueClass.Name = "colTestTrueClass";
            // 
            // colTestAssignedClass
            // 
            this.colTestAssignedClass.HeaderText = "Assigned Class";
            this.colTestAssignedClass.Name = "colTestAssignedClass";
            // 
            // colTestLikelihood
            // 
            this.colTestLikelihood.HeaderText = "Likelihood";
            this.colTestLikelihood.Name = "colTestLikelihood";
            // 
            // colTestMatch
            // 
            this.colTestMatch.FalseValue = "false";
            this.colTestMatch.HeaderText = "Match";
            this.colTestMatch.Name = "colTestMatch";
            this.colTestMatch.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTestMatch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colTestMatch.TrueValue = "true";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(749, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
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
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 442);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Hidden Markov Models for sequence classification";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSequenceSource)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConvergence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIterations)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProbabilities)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvModels)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransitions)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmissions)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTesting)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvSequenceSource;
        private System.Windows.Forms.DataGridView dgvTransitions;
        private System.Windows.Forms.DataGridView dgvModels;
        private System.Windows.Forms.DataGridView dgvProbabilities;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvEmissions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numConvergence;
        private System.Windows.Forms.NumericUpDown numIterations;
        private System.Windows.Forms.RadioButton rbStopConvergence;
        private System.Windows.Forms.RadioButton rbStopIterations;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.DataGridView dgvTesting;
        private System.Windows.Forms.DataGridViewTextBoxColumn colModelsTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colModelsStates;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestSequence;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestTrueClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestAssignedClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestLikelihood;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTestMatch;
    }
}

