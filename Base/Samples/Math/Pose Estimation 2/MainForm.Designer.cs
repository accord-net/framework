namespace PoseEstimation
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
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.components = new System.ComponentModel.Container( );
            this.mainPanel = new System.Windows.Forms.Panel( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.label7 = new System.Windows.Forms.Label( );
            this.label6 = new System.Windows.Forms.Label( );
            this.label5 = new System.Windows.Forms.Label( );
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            this.mainMenu = new System.Windows.Forms.MenuStrip( );
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator( );
            this.openSample1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openSample2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openSample3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openSample4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openSample5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator( );
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.panel1 = new System.Windows.Forms.Panel( );
            this.poseGroupBox = new System.Windows.Forms.GroupBox( );
            this.alternatePoseButton = new System.Windows.Forms.Button( );
            this.bestPoseButton = new System.Windows.Forms.Button( );
            this.focalLengthBox = new System.Windows.Forms.TextBox( );
            this.focalLengthLabel = new System.Windows.Forms.Label( );
            this.copositRadio = new System.Windows.Forms.RadioButton( );
            this.positRadio = new System.Windows.Forms.RadioButton( );
            this.estimatePostButton = new System.Windows.Forms.Button( );
            this.estimatedTransformationMatrixControl = new PoseEstimation.MatrixControl( );
            this.modelPointsGroupBox = new System.Windows.Forms.GroupBox( );
            this.modelPoint4zBox = new System.Windows.Forms.TextBox( );
            this.modelPoint4yBox = new System.Windows.Forms.TextBox( );
            this.modelPoint4xBox = new System.Windows.Forms.TextBox( );
            this.modelPoint3zBox = new System.Windows.Forms.TextBox( );
            this.modelPoint3yBox = new System.Windows.Forms.TextBox( );
            this.modelPoint3xBox = new System.Windows.Forms.TextBox( );
            this.modelPoint2zBox = new System.Windows.Forms.TextBox( );
            this.modelPoint2yBox = new System.Windows.Forms.TextBox( );
            this.modelPoint2xBox = new System.Windows.Forms.TextBox( );
            this.modelPoint1zBox = new System.Windows.Forms.TextBox( );
            this.modelPoint1yBox = new System.Windows.Forms.TextBox( );
            this.modelPoint1xBox = new System.Windows.Forms.TextBox( );
            this.modelPoint4Label = new System.Windows.Forms.Label( );
            this.modelPoint1Label = new System.Windows.Forms.Label( );
            this.modelPoint3Label = new System.Windows.Forms.Label( );
            this.modelPoint2Label = new System.Windows.Forms.Label( );
            this.imagePointsGroupBox = new System.Windows.Forms.GroupBox( );
            this.imagePoint4ColorLabel = new System.Windows.Forms.Label( );
            this.imagePoint3ColorLabel = new System.Windows.Forms.Label( );
            this.imagePoint2ColorLabel = new System.Windows.Forms.Label( );
            this.imagePoint1ColorLabel = new System.Windows.Forms.Label( );
            this.locate4Button = new System.Windows.Forms.Button( );
            this.imagePoint4Box = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.locate3Button = new System.Windows.Forms.Button( );
            this.imagePoint3Box = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.locate2Button = new System.Windows.Forms.Button( );
            this.imagePoint2Box = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.locate1Button = new System.Windows.Forms.Button( );
            this.imagePoint1Box = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.statusStrip = new System.Windows.Forms.StatusStrip( );
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.estimationLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.imageSizeLabel = new System.Windows.Forms.ToolStripStatusLabel( );
            this.errorProvider = new System.Windows.Forms.ErrorProvider( this.components );
            this.toolTip = new System.Windows.Forms.ToolTip( this.components );
            this.mainPanel.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.mainMenu.SuspendLayout( );
            this.panel1.SuspendLayout( );
            this.poseGroupBox.SuspendLayout( );
            this.modelPointsGroupBox.SuspendLayout( );
            this.imagePointsGroupBox.SuspendLayout( );
            this.statusStrip.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.errorProvider ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mainPanel.Controls.Add( this.groupBox1 );
            this.mainPanel.Controls.Add( this.pictureBox );
            this.mainPanel.Location = new System.Drawing.Point( 0, 27 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 974, 478 );
            this.mainPanel.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.label7 );
            this.groupBox1.Controls.Add( this.label6 );
            this.groupBox1.Controls.Add( this.label5 );
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point( 10, 5 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 65, 85 );
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Legend";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 204 ) ) );
            this.label7.ForeColor = System.Drawing.Color.Lime;
            this.label7.Location = new System.Drawing.Point( 10, 60 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 41, 13 );
            this.label7.TabIndex = 2;
            this.label7.Text = "Z axis";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 204 ) ) );
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point( 10, 40 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 41, 13 );
            this.label6.TabIndex = 1;
            this.label6.Text = "Y axis";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 204 ) ) );
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point( 10, 20 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 41, 13 );
            this.label5.TabIndex = 0;
            this.label5.Text = "X axis";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point( 299, 128 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 320, 240 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler( this.pictureBox_MouseMove );
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler( this.pictureBox_MouseClick );
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler( this.pictureBox_Paint );
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem} );
            this.mainMenu.Location = new System.Drawing.Point( 0, 0 );
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size( 974, 24 );
            this.mainMenu.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.openImageToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openSample1ToolStripMenuItem,
            this.openSample2ToolStripMenuItem,
            this.openSample3ToolStripMenuItem,
            this.openSample4ToolStripMenuItem,
            this.openSample5ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem} );
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openImageToolStripMenuItem
            // 
            this.openImageToolStripMenuItem.Name = "openImageToolStripMenuItem";
            this.openImageToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O ) ) );
            this.openImageToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openImageToolStripMenuItem.Text = "&Open image";
            this.openImageToolStripMenuItem.Click += new System.EventHandler( this.openImageToolStripMenuItem_Click );
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size( 190, 6 );
            // 
            // openSample1ToolStripMenuItem
            // 
            this.openSample1ToolStripMenuItem.Name = "openSample1ToolStripMenuItem";
            this.openSample1ToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1 ) ) );
            this.openSample1ToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openSample1ToolStripMenuItem.Tag = "0";
            this.openSample1ToolStripMenuItem.Text = "Open sample &1";
            this.openSample1ToolStripMenuItem.Click += new System.EventHandler( this.openSampleToolStripMenuItem_Click );
            // 
            // openSample2ToolStripMenuItem
            // 
            this.openSample2ToolStripMenuItem.Name = "openSample2ToolStripMenuItem";
            this.openSample2ToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2 ) ) );
            this.openSample2ToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openSample2ToolStripMenuItem.Tag = "1";
            this.openSample2ToolStripMenuItem.Text = "Open sample &2";
            this.openSample2ToolStripMenuItem.Click += new System.EventHandler( this.openSampleToolStripMenuItem_Click );
            // 
            // openSample3ToolStripMenuItem
            // 
            this.openSample3ToolStripMenuItem.Name = "openSample3ToolStripMenuItem";
            this.openSample3ToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3 ) ) );
            this.openSample3ToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openSample3ToolStripMenuItem.Tag = "2";
            this.openSample3ToolStripMenuItem.Text = "Open sample &3";
            this.openSample3ToolStripMenuItem.Click += new System.EventHandler( this.openSampleToolStripMenuItem_Click );
            // 
            // openSample4ToolStripMenuItem
            // 
            this.openSample4ToolStripMenuItem.Name = "openSample4ToolStripMenuItem";
            this.openSample4ToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4 ) ) );
            this.openSample4ToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openSample4ToolStripMenuItem.Tag = "3";
            this.openSample4ToolStripMenuItem.Text = "Open sample &4";
            this.openSample4ToolStripMenuItem.Click += new System.EventHandler( this.openSampleToolStripMenuItem_Click );
            // 
            // openSample5ToolStripMenuItem
            // 
            this.openSample5ToolStripMenuItem.Name = "openSample5ToolStripMenuItem";
            this.openSample5ToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys) ( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5 ) ) );
            this.openSample5ToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.openSample5ToolStripMenuItem.Tag = "4";
            this.openSample5ToolStripMenuItem.Text = "Open sample &5";
            this.openSample5ToolStripMenuItem.Click += new System.EventHandler( this.openSampleToolStripMenuItem_Click );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 190, 6 );
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size( 193, 22 );
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler( this.exitToolStripMenuItem_Click );
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Image files (*.jpg, *.bmp, *.png)|*.jpg; *.bmp; *.png";
            this.openFileDialog.Title = "Select a image file";
            // 
            // panel1
            // 
            this.panel1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panel1.Controls.Add( this.poseGroupBox );
            this.panel1.Controls.Add( this.modelPointsGroupBox );
            this.panel1.Controls.Add( this.imagePointsGroupBox );
            this.panel1.Location = new System.Drawing.Point( 0, 511 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 974, 130 );
            this.panel1.TabIndex = 2;
            // 
            // poseGroupBox
            // 
            this.poseGroupBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.poseGroupBox.Controls.Add( this.alternatePoseButton );
            this.poseGroupBox.Controls.Add( this.bestPoseButton );
            this.poseGroupBox.Controls.Add( this.focalLengthBox );
            this.poseGroupBox.Controls.Add( this.focalLengthLabel );
            this.poseGroupBox.Controls.Add( this.copositRadio );
            this.poseGroupBox.Controls.Add( this.positRadio );
            this.poseGroupBox.Controls.Add( this.estimatePostButton );
            this.poseGroupBox.Controls.Add( this.estimatedTransformationMatrixControl );
            this.poseGroupBox.Location = new System.Drawing.Point( 530, 0 );
            this.poseGroupBox.Name = "poseGroupBox";
            this.poseGroupBox.Size = new System.Drawing.Size( 432, 127 );
            this.poseGroupBox.TabIndex = 2;
            this.poseGroupBox.TabStop = false;
            this.poseGroupBox.Text = "Pose estimation";
            // 
            // alternatePoseButton
            // 
            this.alternatePoseButton.Location = new System.Drawing.Point( 345, 7 );
            this.alternatePoseButton.Name = "alternatePoseButton";
            this.alternatePoseButton.Size = new System.Drawing.Size( 20, 20 );
            this.alternatePoseButton.TabIndex = 2;
            this.alternatePoseButton.Text = "&A";
            this.toolTip.SetToolTip( this.alternatePoseButton, "Select alternate CoPOSIT estimation" );
            this.alternatePoseButton.UseVisualStyleBackColor = true;
            this.alternatePoseButton.Click += new System.EventHandler( this.alternatePoseButton_Click );
            // 
            // bestPoseButton
            // 
            this.bestPoseButton.Location = new System.Drawing.Point( 320, 7 );
            this.bestPoseButton.Name = "bestPoseButton";
            this.bestPoseButton.Size = new System.Drawing.Size( 20, 20 );
            this.bestPoseButton.TabIndex = 1;
            this.bestPoseButton.Text = "&B";
            this.toolTip.SetToolTip( this.bestPoseButton, "Select best CoPOSIT estimation" );
            this.bestPoseButton.UseVisualStyleBackColor = true;
            this.bestPoseButton.Click += new System.EventHandler( this.bestPoseButton_Click );
            // 
            // focalLengthBox
            // 
            this.focalLengthBox.Location = new System.Drawing.Point( 95, 50 );
            this.focalLengthBox.Name = "focalLengthBox";
            this.focalLengthBox.Size = new System.Drawing.Size( 85, 20 );
            this.focalLengthBox.TabIndex = 4;
            this.focalLengthBox.Leave += new System.EventHandler( this.focalLengthBox_Leave );
            // 
            // focalLengthLabel
            // 
            this.focalLengthLabel.AutoSize = true;
            this.focalLengthLabel.Location = new System.Drawing.Point( 10, 53 );
            this.focalLengthLabel.Name = "focalLengthLabel";
            this.focalLengthLabel.Size = new System.Drawing.Size( 68, 13 );
            this.focalLengthLabel.TabIndex = 3;
            this.focalLengthLabel.Text = "Focal length:";
            this.toolTip.SetToolTip( this.focalLengthLabel, "Effective Focal Length" );
            // 
            // copositRadio
            // 
            this.copositRadio.AutoSize = true;
            this.copositRadio.Location = new System.Drawing.Point( 80, 23 );
            this.copositRadio.Name = "copositRadio";
            this.copositRadio.Size = new System.Drawing.Size( 102, 17 );
            this.copositRadio.TabIndex = 2;
            this.copositRadio.TabStop = true;
            this.copositRadio.Text = "Coplanar POSIT";
            this.copositRadio.UseVisualStyleBackColor = true;
            this.copositRadio.CheckedChanged += new System.EventHandler( this.copositRadio_CheckedChanged );
            // 
            // positRadio
            // 
            this.positRadio.AutoSize = true;
            this.positRadio.Location = new System.Drawing.Point( 10, 23 );
            this.positRadio.Name = "positRadio";
            this.positRadio.Size = new System.Drawing.Size( 57, 17 );
            this.positRadio.TabIndex = 1;
            this.positRadio.TabStop = true;
            this.positRadio.Text = "POSIT";
            this.positRadio.UseVisualStyleBackColor = true;
            // 
            // estimatePostButton
            // 
            this.estimatePostButton.Location = new System.Drawing.Point( 105, 93 );
            this.estimatePostButton.Name = "estimatePostButton";
            this.estimatePostButton.Size = new System.Drawing.Size( 75, 23 );
            this.estimatePostButton.TabIndex = 0;
            this.estimatePostButton.Text = "&Estimate";
            this.estimatePostButton.UseVisualStyleBackColor = true;
            this.estimatePostButton.Click += new System.EventHandler( this.estimatePostButton_Click );
            // 
            // estimatedTransformationMatrixControl
            // 
            this.estimatedTransformationMatrixControl.Location = new System.Drawing.Point( 190, 10 );
            this.estimatedTransformationMatrixControl.Name = "estimatedTransformationMatrixControl";
            this.estimatedTransformationMatrixControl.Size = new System.Drawing.Size( 230, 110 );
            this.estimatedTransformationMatrixControl.TabIndex = 5;
            this.estimatedTransformationMatrixControl.Title = "Estimated transformation";
            // 
            // modelPointsGroupBox
            // 
            this.modelPointsGroupBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint4zBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint4yBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint4xBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint3zBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint3yBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint3xBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint2zBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint2yBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint2xBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint1zBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint1yBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint1xBox );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint4Label );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint1Label );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint3Label );
            this.modelPointsGroupBox.Controls.Add( this.modelPoint2Label );
            this.modelPointsGroupBox.Location = new System.Drawing.Point( 250, 0 );
            this.modelPointsGroupBox.Name = "modelPointsGroupBox";
            this.modelPointsGroupBox.Size = new System.Drawing.Size( 270, 127 );
            this.modelPointsGroupBox.TabIndex = 1;
            this.modelPointsGroupBox.TabStop = false;
            this.modelPointsGroupBox.Text = "Model coordinates (x, y, z)";
            // 
            // modelPoint4zBox
            // 
            this.modelPoint4zBox.Location = new System.Drawing.Point( 200, 95 );
            this.modelPoint4zBox.Name = "modelPoint4zBox";
            this.modelPoint4zBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint4zBox.TabIndex = 15;
            this.modelPoint4zBox.Tag = "32";
            this.modelPoint4zBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint4yBox
            // 
            this.modelPoint4yBox.Location = new System.Drawing.Point( 135, 95 );
            this.modelPoint4yBox.Name = "modelPoint4yBox";
            this.modelPoint4yBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint4yBox.TabIndex = 14;
            this.modelPoint4yBox.Tag = "31";
            this.modelPoint4yBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint4xBox
            // 
            this.modelPoint4xBox.Location = new System.Drawing.Point( 70, 95 );
            this.modelPoint4xBox.Name = "modelPoint4xBox";
            this.modelPoint4xBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint4xBox.TabIndex = 13;
            this.modelPoint4xBox.Tag = "30";
            this.modelPoint4xBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint3zBox
            // 
            this.modelPoint3zBox.Location = new System.Drawing.Point( 200, 70 );
            this.modelPoint3zBox.Name = "modelPoint3zBox";
            this.modelPoint3zBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint3zBox.TabIndex = 11;
            this.modelPoint3zBox.Tag = "22";
            this.modelPoint3zBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint3yBox
            // 
            this.modelPoint3yBox.Location = new System.Drawing.Point( 135, 70 );
            this.modelPoint3yBox.Name = "modelPoint3yBox";
            this.modelPoint3yBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint3yBox.TabIndex = 10;
            this.modelPoint3yBox.Tag = "21";
            this.modelPoint3yBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint3xBox
            // 
            this.modelPoint3xBox.Location = new System.Drawing.Point( 70, 70 );
            this.modelPoint3xBox.Name = "modelPoint3xBox";
            this.modelPoint3xBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint3xBox.TabIndex = 9;
            this.modelPoint3xBox.Tag = "20";
            this.modelPoint3xBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint2zBox
            // 
            this.modelPoint2zBox.Location = new System.Drawing.Point( 200, 45 );
            this.modelPoint2zBox.Name = "modelPoint2zBox";
            this.modelPoint2zBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint2zBox.TabIndex = 7;
            this.modelPoint2zBox.Tag = "12";
            this.modelPoint2zBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint2yBox
            // 
            this.modelPoint2yBox.Location = new System.Drawing.Point( 135, 45 );
            this.modelPoint2yBox.Name = "modelPoint2yBox";
            this.modelPoint2yBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint2yBox.TabIndex = 6;
            this.modelPoint2yBox.Tag = "11";
            this.modelPoint2yBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint2xBox
            // 
            this.modelPoint2xBox.Location = new System.Drawing.Point( 70, 45 );
            this.modelPoint2xBox.Name = "modelPoint2xBox";
            this.modelPoint2xBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint2xBox.TabIndex = 5;
            this.modelPoint2xBox.Tag = "10";
            this.modelPoint2xBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint1zBox
            // 
            this.modelPoint1zBox.Location = new System.Drawing.Point( 200, 20 );
            this.modelPoint1zBox.Name = "modelPoint1zBox";
            this.modelPoint1zBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint1zBox.TabIndex = 3;
            this.modelPoint1zBox.Tag = "2";
            this.modelPoint1zBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint1yBox
            // 
            this.modelPoint1yBox.Location = new System.Drawing.Point( 135, 20 );
            this.modelPoint1yBox.Name = "modelPoint1yBox";
            this.modelPoint1yBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint1yBox.TabIndex = 2;
            this.modelPoint1yBox.Tag = "1";
            this.modelPoint1yBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint1xBox
            // 
            this.modelPoint1xBox.Location = new System.Drawing.Point( 70, 20 );
            this.modelPoint1xBox.Name = "modelPoint1xBox";
            this.modelPoint1xBox.Size = new System.Drawing.Size( 60, 20 );
            this.modelPoint1xBox.TabIndex = 1;
            this.modelPoint1xBox.Tag = "0";
            this.modelPoint1xBox.Leave += new System.EventHandler( this.modelPointBox_Leave );
            // 
            // modelPoint4Label
            // 
            this.modelPoint4Label.AutoSize = true;
            this.modelPoint4Label.Location = new System.Drawing.Point( 10, 98 );
            this.modelPoint4Label.Name = "modelPoint4Label";
            this.modelPoint4Label.Size = new System.Drawing.Size( 43, 13 );
            this.modelPoint4Label.TabIndex = 12;
            this.modelPoint4Label.Text = "Point 4:";
            // 
            // modelPoint1Label
            // 
            this.modelPoint1Label.AutoSize = true;
            this.modelPoint1Label.Location = new System.Drawing.Point( 10, 23 );
            this.modelPoint1Label.Name = "modelPoint1Label";
            this.modelPoint1Label.Size = new System.Drawing.Size( 43, 13 );
            this.modelPoint1Label.TabIndex = 0;
            this.modelPoint1Label.Text = "Point 1:";
            // 
            // modelPoint3Label
            // 
            this.modelPoint3Label.AutoSize = true;
            this.modelPoint3Label.Location = new System.Drawing.Point( 10, 73 );
            this.modelPoint3Label.Name = "modelPoint3Label";
            this.modelPoint3Label.Size = new System.Drawing.Size( 43, 13 );
            this.modelPoint3Label.TabIndex = 8;
            this.modelPoint3Label.Text = "Point 3:";
            // 
            // modelPoint2Label
            // 
            this.modelPoint2Label.AutoSize = true;
            this.modelPoint2Label.Location = new System.Drawing.Point( 10, 48 );
            this.modelPoint2Label.Name = "modelPoint2Label";
            this.modelPoint2Label.Size = new System.Drawing.Size( 43, 13 );
            this.modelPoint2Label.TabIndex = 4;
            this.modelPoint2Label.Text = "Point 2:";
            // 
            // imagePointsGroupBox
            // 
            this.imagePointsGroupBox.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint4ColorLabel );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint3ColorLabel );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint2ColorLabel );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint1ColorLabel );
            this.imagePointsGroupBox.Controls.Add( this.locate4Button );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint4Box );
            this.imagePointsGroupBox.Controls.Add( this.label4 );
            this.imagePointsGroupBox.Controls.Add( this.locate3Button );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint3Box );
            this.imagePointsGroupBox.Controls.Add( this.label3 );
            this.imagePointsGroupBox.Controls.Add( this.locate2Button );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint2Box );
            this.imagePointsGroupBox.Controls.Add( this.label2 );
            this.imagePointsGroupBox.Controls.Add( this.locate1Button );
            this.imagePointsGroupBox.Controls.Add( this.imagePoint1Box );
            this.imagePointsGroupBox.Controls.Add( this.label1 );
            this.imagePointsGroupBox.Location = new System.Drawing.Point( 10, 0 );
            this.imagePointsGroupBox.Name = "imagePointsGroupBox";
            this.imagePointsGroupBox.Size = new System.Drawing.Size( 230, 127 );
            this.imagePointsGroupBox.TabIndex = 0;
            this.imagePointsGroupBox.TabStop = false;
            this.imagePointsGroupBox.Text = "Image coordinates (x, y)";
            // 
            // imagePoint4ColorLabel
            // 
            this.imagePoint4ColorLabel.BackColor = System.Drawing.Color.Black;
            this.imagePoint4ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePoint4ColorLabel.Location = new System.Drawing.Point( 55, 115 );
            this.imagePoint4ColorLabel.Name = "imagePoint4ColorLabel";
            this.imagePoint4ColorLabel.Size = new System.Drawing.Size( 75, 4 );
            this.imagePoint4ColorLabel.TabIndex = 20;
            // 
            // imagePoint3ColorLabel
            // 
            this.imagePoint3ColorLabel.BackColor = System.Drawing.Color.Black;
            this.imagePoint3ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePoint3ColorLabel.Location = new System.Drawing.Point( 55, 90 );
            this.imagePoint3ColorLabel.Name = "imagePoint3ColorLabel";
            this.imagePoint3ColorLabel.Size = new System.Drawing.Size( 75, 4 );
            this.imagePoint3ColorLabel.TabIndex = 19;
            // 
            // imagePoint2ColorLabel
            // 
            this.imagePoint2ColorLabel.BackColor = System.Drawing.Color.Black;
            this.imagePoint2ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePoint2ColorLabel.Location = new System.Drawing.Point( 55, 65 );
            this.imagePoint2ColorLabel.Name = "imagePoint2ColorLabel";
            this.imagePoint2ColorLabel.Size = new System.Drawing.Size( 75, 4 );
            this.imagePoint2ColorLabel.TabIndex = 18;
            // 
            // imagePoint1ColorLabel
            // 
            this.imagePoint1ColorLabel.BackColor = System.Drawing.Color.Black;
            this.imagePoint1ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePoint1ColorLabel.Location = new System.Drawing.Point( 55, 40 );
            this.imagePoint1ColorLabel.Name = "imagePoint1ColorLabel";
            this.imagePoint1ColorLabel.Size = new System.Drawing.Size( 75, 4 );
            this.imagePoint1ColorLabel.TabIndex = 17;
            // 
            // locate4Button
            // 
            this.locate4Button.Location = new System.Drawing.Point( 140, 94 );
            this.locate4Button.Name = "locate4Button";
            this.locate4Button.Size = new System.Drawing.Size( 75, 23 );
            this.locate4Button.TabIndex = 11;
            this.locate4Button.Tag = "3";
            this.locate4Button.Text = "Locate &4th";
            this.locate4Button.UseVisualStyleBackColor = true;
            this.locate4Button.Click += new System.EventHandler( this.locatePointButton_Click );
            // 
            // imagePoint4Box
            // 
            this.imagePoint4Box.Location = new System.Drawing.Point( 55, 95 );
            this.imagePoint4Box.Name = "imagePoint4Box";
            this.imagePoint4Box.ReadOnly = true;
            this.imagePoint4Box.Size = new System.Drawing.Size( 75, 20 );
            this.imagePoint4Box.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 98 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 43, 13 );
            this.label4.TabIndex = 9;
            this.label4.Text = "Point 4:";
            // 
            // locate3Button
            // 
            this.locate3Button.Location = new System.Drawing.Point( 140, 69 );
            this.locate3Button.Name = "locate3Button";
            this.locate3Button.Size = new System.Drawing.Size( 75, 23 );
            this.locate3Button.TabIndex = 8;
            this.locate3Button.Tag = "2";
            this.locate3Button.Text = "Locate &3rd";
            this.locate3Button.UseVisualStyleBackColor = true;
            this.locate3Button.Click += new System.EventHandler( this.locatePointButton_Click );
            // 
            // imagePoint3Box
            // 
            this.imagePoint3Box.Location = new System.Drawing.Point( 55, 70 );
            this.imagePoint3Box.Name = "imagePoint3Box";
            this.imagePoint3Box.ReadOnly = true;
            this.imagePoint3Box.Size = new System.Drawing.Size( 75, 20 );
            this.imagePoint3Box.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 73 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 43, 13 );
            this.label3.TabIndex = 6;
            this.label3.Text = "Point 3:";
            // 
            // locate2Button
            // 
            this.locate2Button.Location = new System.Drawing.Point( 140, 44 );
            this.locate2Button.Name = "locate2Button";
            this.locate2Button.Size = new System.Drawing.Size( 75, 23 );
            this.locate2Button.TabIndex = 5;
            this.locate2Button.Tag = "1";
            this.locate2Button.Text = "Locate &2nd";
            this.locate2Button.UseVisualStyleBackColor = true;
            this.locate2Button.Click += new System.EventHandler( this.locatePointButton_Click );
            // 
            // imagePoint2Box
            // 
            this.imagePoint2Box.Location = new System.Drawing.Point( 55, 45 );
            this.imagePoint2Box.Name = "imagePoint2Box";
            this.imagePoint2Box.ReadOnly = true;
            this.imagePoint2Box.Size = new System.Drawing.Size( 75, 20 );
            this.imagePoint2Box.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 48 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 43, 13 );
            this.label2.TabIndex = 3;
            this.label2.Text = "Point 2:";
            // 
            // locate1Button
            // 
            this.locate1Button.Location = new System.Drawing.Point( 140, 19 );
            this.locate1Button.Name = "locate1Button";
            this.locate1Button.Size = new System.Drawing.Size( 75, 23 );
            this.locate1Button.TabIndex = 2;
            this.locate1Button.Tag = "0";
            this.locate1Button.Text = "Locate &1st";
            this.locate1Button.UseVisualStyleBackColor = true;
            this.locate1Button.Click += new System.EventHandler( this.locatePointButton_Click );
            // 
            // imagePoint1Box
            // 
            this.imagePoint1Box.Location = new System.Drawing.Point( 55, 20 );
            this.imagePoint1Box.Name = "imagePoint1Box";
            this.imagePoint1Box.ReadOnly = true;
            this.imagePoint1Box.Size = new System.Drawing.Size( 75, 20 );
            this.imagePoint1Box.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 23 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 43, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Point 1:";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.estimationLabel,
            this.imageSizeLabel} );
            this.statusStrip.Location = new System.Drawing.Point( 0, 644 );
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size( 974, 22 );
            this.statusStrip.TabIndex = 3;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size( 300, 17 );
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // estimationLabel
            // 
            this.estimationLabel.AutoSize = false;
            this.estimationLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.estimationLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.estimationLabel.Name = "estimationLabel";
            this.estimationLabel.Size = new System.Drawing.Size( 539, 17 );
            this.estimationLabel.Spring = true;
            this.estimationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imageSizeLabel
            // 
            this.imageSizeLabel.AutoSize = false;
            this.imageSizeLabel.BorderSides = ( (System.Windows.Forms.ToolStripStatusLabelBorderSides) ( ( ( ( System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right )
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom ) ) );
            this.imageSizeLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.imageSizeLabel.Name = "imageSizeLabel";
            this.imageSizeLabel.Size = new System.Drawing.Size( 120, 17 );
            this.imageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 974, 666 );
            this.Controls.Add( this.statusStrip );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.mainPanel );
            this.Controls.Add( this.mainMenu );
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pose Estimation";
            this.Resize += new System.EventHandler( this.MainForm_Resize );
            this.mainPanel.ResumeLayout( false );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.mainMenu.ResumeLayout( false );
            this.mainMenu.PerformLayout( );
            this.panel1.ResumeLayout( false );
            this.poseGroupBox.ResumeLayout( false );
            this.poseGroupBox.PerformLayout( );
            this.modelPointsGroupBox.ResumeLayout( false );
            this.modelPointsGroupBox.PerformLayout( );
            this.imagePointsGroupBox.ResumeLayout( false );
            this.imagePointsGroupBox.PerformLayout( );
            this.statusStrip.ResumeLayout( false );
            this.statusStrip.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.errorProvider ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox imagePointsGroupBox;
        private System.Windows.Forms.Button locate1Button;
        private System.Windows.Forms.TextBox imagePoint1Box;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button locate4Button;
        private System.Windows.Forms.TextBox imagePoint4Box;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button locate3Button;
        private System.Windows.Forms.TextBox imagePoint3Box;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button locate2Button;
        private System.Windows.Forms.TextBox imagePoint2Box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripStatusLabel imageSizeLabel;
        private System.Windows.Forms.GroupBox modelPointsGroupBox;
        private System.Windows.Forms.TextBox modelPoint1xBox;
        private System.Windows.Forms.Label modelPoint4Label;
        private System.Windows.Forms.Label modelPoint1Label;
        private System.Windows.Forms.Label modelPoint3Label;
        private System.Windows.Forms.Label modelPoint2Label;
        private System.Windows.Forms.TextBox modelPoint4zBox;
        private System.Windows.Forms.TextBox modelPoint4yBox;
        private System.Windows.Forms.TextBox modelPoint4xBox;
        private System.Windows.Forms.TextBox modelPoint3zBox;
        private System.Windows.Forms.TextBox modelPoint3yBox;
        private System.Windows.Forms.TextBox modelPoint3xBox;
        private System.Windows.Forms.TextBox modelPoint2zBox;
        private System.Windows.Forms.TextBox modelPoint2yBox;
        private System.Windows.Forms.TextBox modelPoint2xBox;
        private System.Windows.Forms.TextBox modelPoint1zBox;
        private System.Windows.Forms.TextBox modelPoint1yBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.GroupBox poseGroupBox;
        private System.Windows.Forms.Button estimatePostButton;
        private System.Windows.Forms.Label imagePoint1ColorLabel;
        private System.Windows.Forms.Label imagePoint4ColorLabel;
        private System.Windows.Forms.Label imagePoint3ColorLabel;
        private System.Windows.Forms.Label imagePoint2ColorLabel;
        private System.Windows.Forms.RadioButton copositRadio;
        private System.Windows.Forms.RadioButton positRadio;
        private System.Windows.Forms.TextBox focalLengthBox;
        private System.Windows.Forms.Label focalLengthLabel;
        private MatrixControl estimatedTransformationMatrixControl;
        private System.Windows.Forms.ToolStripMenuItem openSample1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSample2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSample3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openSample4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSample5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel estimationLabel;
        private System.Windows.Forms.Button alternatePoseButton;
        private System.Windows.Forms.Button bestPoseButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
    }
}

