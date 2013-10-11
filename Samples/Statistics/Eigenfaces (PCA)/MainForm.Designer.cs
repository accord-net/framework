namespace Eigenfaces.PCA
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.colEigen = new System.Windows.Forms.DataGridViewImageColumn();
            this.colProportion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHand = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnCreateProjection = new System.Windows.Forms.Button();
            this.btnClassify = new System.Windows.Forms.Button();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.colClass2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLabel2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHand2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.colProjection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClassification = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvPrincipalComponents = new System.Windows.Forms.DataGridView();
            this.colComponent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEigenValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSingularValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCumulativeProportion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.graphShare = new ZedGraph.ZedGraphControl();
            this.graphCurve = new ZedGraph.ZedGraphControl();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrincipalComponents)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(666, 544);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.dataGridView2);
            this.tabPage3.Controls.Add(this.dataGridView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(658, 518);
            this.tabPage3.TabIndex = 12;
            this.tabPage3.Text = "Compute analysis";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(332, 468);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(320, 44);
            this.button1.TabIndex = 11;
            this.button1.Text = "Compute Principal Component Analysis";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEigen,
            this.colProportion});
            this.dataGridView2.GridColor = System.Drawing.SystemColors.Window;
            this.dataGridView2.Location = new System.Drawing.Point(332, 6);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowHeadersWidth = 30;
            this.dataGridView2.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView2.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.dataGridView2.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView2.RowTemplate.Height = 100;
            this.dataGridView2.Size = new System.Drawing.Size(318, 456);
            this.dataGridView2.TabIndex = 10;
            // 
            // colEigen
            // 
            this.colEigen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colEigen.HeaderText = "Eigenhand";
            this.colEigen.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colEigen.Name = "colEigen";
            this.colEigen.ReadOnly = true;
            this.colEigen.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEigen.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colProportion
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "N3";
            dataGridViewCellStyle2.NullValue = null;
            this.colProportion.DefaultCellStyle = dataGridViewCellStyle2;
            this.colProportion.HeaderText = "Proportion";
            this.colProportion.Name = "colProportion";
            this.colProportion.ReadOnly = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Black;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClass,
            this.colLabel,
            this.colHand});
            this.dataGridView1.GridColor = System.Drawing.Color.Black;
            this.dataGridView1.Location = new System.Drawing.Point(11, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.dataGridView1.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Black;
            this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView1.RowTemplate.Height = 110;
            this.dataGridView1.Size = new System.Drawing.Size(315, 506);
            this.dataGridView1.TabIndex = 10;
            // 
            // colClass
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colClass.DefaultCellStyle = dataGridViewCellStyle3;
            this.colClass.HeaderText = "Class";
            this.colClass.Name = "colClass";
            this.colClass.Width = 90;
            // 
            // colLabel
            // 
            this.colLabel.HeaderText = "Label";
            this.colLabel.Name = "colLabel";
            this.colLabel.Visible = false;
            // 
            // colHand
            // 
            this.colHand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHand.HeaderText = "Hand";
            this.colHand.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colHand.Name = "colHand";
            this.colHand.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colHand.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCreateProjection);
            this.tabPage1.Controls.Add(this.btnClassify);
            this.tabPage1.Controls.Add(this.dataGridView3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(658, 472);
            this.tabPage1.TabIndex = 13;
            this.tabPage1.Text = "Projection classification";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCreateProjection
            // 
            this.btnCreateProjection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateProjection.Enabled = false;
            this.btnCreateProjection.Location = new System.Drawing.Point(8, 439);
            this.btnCreateProjection.Name = "btnCreateProjection";
            this.btnCreateProjection.Size = new System.Drawing.Size(234, 27);
            this.btnCreateProjection.TabIndex = 12;
            this.btnCreateProjection.Text = "Extract projections and create classifier";
            this.btnCreateProjection.UseVisualStyleBackColor = true;
            this.btnCreateProjection.Click += new System.EventHandler(this.btnFeature_Click);
            // 
            // btnClassify
            // 
            this.btnClassify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClassify.Enabled = false;
            this.btnClassify.Location = new System.Drawing.Point(248, 439);
            this.btnClassify.Name = "btnClassify";
            this.btnClassify.Size = new System.Drawing.Size(150, 27);
            this.btnClassify.TabIndex = 12;
            this.btnClassify.Text = "Run classifier";
            this.btnClassify.UseVisualStyleBackColor = true;
            this.btnClassify.Click += new System.EventHandler(this.btnClassify_Click);
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView3.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClass2,
            this.colLabel2,
            this.colHand2,
            this.colProjection,
            this.colClassification});
            this.dataGridView3.GridColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView3.Location = new System.Drawing.Point(8, 6);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersWidth = 30;
            this.dataGridView3.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView3.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridView3.RowTemplate.Height = 70;
            this.dataGridView3.Size = new System.Drawing.Size(642, 427);
            this.dataGridView3.TabIndex = 11;
            // 
            // colClass2
            // 
            this.colClass2.FillWeight = 101.5228F;
            this.colClass2.HeaderText = "Class";
            this.colClass2.Name = "colClass2";
            this.colClass2.Width = 57;
            // 
            // colLabel2
            // 
            this.colLabel2.HeaderText = "Label";
            this.colLabel2.Name = "colLabel2";
            this.colLabel2.Visible = false;
            this.colLabel2.Width = 50;
            // 
            // colHand2
            // 
            this.colHand2.FillWeight = 99.49239F;
            this.colHand2.HeaderText = "Hand";
            this.colHand2.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colHand2.MinimumWidth = 80;
            this.colHand2.Name = "colHand2";
            this.colHand2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colHand2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colHand2.Width = 80;
            // 
            // colProjection
            // 
            this.colProjection.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colProjection.FillWeight = 99.49239F;
            this.colProjection.HeaderText = "Projection";
            this.colProjection.Name = "colProjection";
            // 
            // colClassification
            // 
            this.colClassification.FillWeight = 99.49239F;
            this.colClassification.HeaderText = "Classification";
            this.colClassification.Name = "colClassification";
            this.colClassification.Width = 93;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(658, 472);
            this.tabPage2.TabIndex = 14;
            this.tabPage2.Text = "Analysis details";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer2.Size = new System.Drawing.Size(652, 466);
            this.splitContainer2.SplitterDistance = 275;
            this.splitContainer2.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvPrincipalComponents);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 275);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Principal Components";
            // 
            // dgvPrincipalComponents
            // 
            this.dgvPrincipalComponents.AllowUserToAddRows = false;
            this.dgvPrincipalComponents.AllowUserToDeleteRows = false;
            this.dgvPrincipalComponents.AllowUserToResizeRows = false;
            this.dgvPrincipalComponents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPrincipalComponents.BackgroundColor = System.Drawing.Color.White;
            this.dgvPrincipalComponents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPrincipalComponents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPrincipalComponents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrincipalComponents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colComponent,
            this.colEigenValue,
            this.colSingularValue,
            this.dataGridViewTextBoxColumn1,
            this.colCumulativeProportion});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPrincipalComponents.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgvPrincipalComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrincipalComponents.Location = new System.Drawing.Point(3, 16);
            this.dgvPrincipalComponents.Name = "dgvPrincipalComponents";
            this.dgvPrincipalComponents.ReadOnly = true;
            this.dgvPrincipalComponents.RowHeadersVisible = false;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvPrincipalComponents.RowsDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvPrincipalComponents.Size = new System.Drawing.Size(646, 256);
            this.dgvPrincipalComponents.TabIndex = 1;
            // 
            // colComponent
            // 
            this.colComponent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colComponent.DataPropertyName = "Index";
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colComponent.DefaultCellStyle = dataGridViewCellStyle5;
            this.colComponent.FillWeight = 10F;
            this.colComponent.HeaderText = "Component";
            this.colComponent.Name = "colComponent";
            this.colComponent.ReadOnly = true;
            this.colComponent.Width = 78;
            // 
            // colEigenValue
            // 
            this.colEigenValue.DataPropertyName = "Eigenvalue";
            dataGridViewCellStyle6.Format = "N5";
            dataGridViewCellStyle6.NullValue = null;
            this.colEigenValue.DefaultCellStyle = dataGridViewCellStyle6;
            this.colEigenValue.HeaderText = "Eigen Value";
            this.colEigenValue.Name = "colEigenValue";
            this.colEigenValue.ReadOnly = true;
            // 
            // colSingularValue
            // 
            this.colSingularValue.DataPropertyName = "SingularValue";
            dataGridViewCellStyle7.Format = "N5";
            this.colSingularValue.DefaultCellStyle = dataGridViewCellStyle7;
            this.colSingularValue.HeaderText = "Singular Value";
            this.colSingularValue.Name = "colSingularValue";
            this.colSingularValue.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Proportion";
            dataGridViewCellStyle8.Format = "N5";
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn1.HeaderText = "Proportion";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // colCumulativeProportion
            // 
            this.colCumulativeProportion.DataPropertyName = "CumulativeProportion";
            dataGridViewCellStyle9.Format = "N5";
            this.colCumulativeProportion.DefaultCellStyle = dataGridViewCellStyle9;
            this.colCumulativeProportion.HeaderText = "Cumulative Proportion";
            this.colCumulativeProportion.Name = "colCumulativeProportion";
            this.colCumulativeProportion.ReadOnly = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.splitContainer1);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(652, 187);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Visualization";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.graphShare);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.graphCurve);
            this.splitContainer1.Size = new System.Drawing.Size(646, 168);
            this.splitContainer1.SplitterDistance = 190;
            this.splitContainer1.TabIndex = 0;
            // 
            // graphShare
            // 
            this.graphShare.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphShare.Location = new System.Drawing.Point(0, 0);
            this.graphShare.Name = "graphShare";
            this.graphShare.ScrollGrace = 0D;
            this.graphShare.ScrollMaxX = 0D;
            this.graphShare.ScrollMaxY = 0D;
            this.graphShare.ScrollMaxY2 = 0D;
            this.graphShare.ScrollMinX = 0D;
            this.graphShare.ScrollMinY = 0D;
            this.graphShare.ScrollMinY2 = 0D;
            this.graphShare.Size = new System.Drawing.Size(190, 168);
            this.graphShare.TabIndex = 2;
            // 
            // graphCurve
            // 
            this.graphCurve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphCurve.Location = new System.Drawing.Point(0, 0);
            this.graphCurve.Name = "graphCurve";
            this.graphCurve.ScrollGrace = 0D;
            this.graphCurve.ScrollMaxX = 0D;
            this.graphCurve.ScrollMaxY = 0D;
            this.graphCurve.ScrollMaxY2 = 0D;
            this.graphCurve.ScrollMinX = 0D;
            this.graphCurve.ScrollMinY = 0D;
            this.graphCurve.ScrollMinY2 = 0D;
            this.graphCurve.Size = new System.Drawing.Size(452, 168);
            this.graphCurve.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 544);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Image classification with Principal Component Analysis";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrincipalComponents)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnClassify;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Button btnCreateProjection;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClass2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLabel2;
        private System.Windows.Forms.DataGridViewImageColumn colHand2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjection;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClassification;
        private System.Windows.Forms.DataGridViewImageColumn colEigen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProportion;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvPrincipalComponents;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComponent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEigenValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSingularValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCumulativeProportion;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ZedGraph.ZedGraphControl graphShare;
        private ZedGraph.ZedGraphControl graphCurve;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLabel;
        private System.Windows.Forms.DataGridViewImageColumn colHand;


    }
}

