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
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.yawBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.pitchBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.rollBox = new System.Windows.Forms.TextBox( );
            this.groupBox4 = new System.Windows.Forms.GroupBox( );
            this.label10 = new System.Windows.Forms.Label( );
            this.zObjectBox = new System.Windows.Forms.TextBox( );
            this.label11 = new System.Windows.Forms.Label( );
            this.yObjectBox = new System.Windows.Forms.TextBox( );
            this.label12 = new System.Windows.Forms.Label( );
            this.xObjectBox = new System.Windows.Forms.TextBox( );
            this.toolTip = new System.Windows.Forms.ToolTip( this.components );
            this.label17 = new System.Windows.Forms.Label( );
            this.label18 = new System.Windows.Forms.Label( );
            this.label19 = new System.Windows.Forms.Label( );
            this.targetTransformationMatrixControl = new PoseEstimation.MatrixControl( );
            this.errorProvider = new System.Windows.Forms.ErrorProvider( this.components );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.label7 = new System.Windows.Forms.Label( );
            this.zLookAtBox = new System.Windows.Forms.TextBox( );
            this.label8 = new System.Windows.Forms.Label( );
            this.yLookAtBox = new System.Windows.Forms.TextBox( );
            this.label9 = new System.Windows.Forms.Label( );
            this.xLookAtBox = new System.Windows.Forms.TextBox( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.label6 = new System.Windows.Forms.Label( );
            this.zCameraBox = new System.Windows.Forms.TextBox( );
            this.label5 = new System.Windows.Forms.Label( );
            this.yCameraBox = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.xCameraBox = new System.Windows.Forms.TextBox( );
            this.groupBox5 = new System.Windows.Forms.GroupBox( );
            this.worldRendererControl = new PoseEstimation.WorldRendererControl( );
            this.groupBox6 = new System.Windows.Forms.GroupBox( );
            this.objectPoint4ColorLabel = new System.Windows.Forms.Label( );
            this.objectPoint3ColorLabel = new System.Windows.Forms.Label( );
            this.objectPoint2ColorLabel = new System.Windows.Forms.Label( );
            this.objectPoint1ColorLabel = new System.Windows.Forms.Label( );
            this.objectPoint4Box = new System.Windows.Forms.TextBox( );
            this.label16 = new System.Windows.Forms.Label( );
            this.objectPoint3Box = new System.Windows.Forms.TextBox( );
            this.label15 = new System.Windows.Forms.Label( );
            this.objectPoint2Box = new System.Windows.Forms.TextBox( );
            this.label14 = new System.Windows.Forms.Label( );
            this.objectPoint1Box = new System.Windows.Forms.TextBox( );
            this.label13 = new System.Windows.Forms.Label( );
            this.objectTypeCombo = new System.Windows.Forms.ComboBox( );
            this.groupBox7 = new System.Windows.Forms.GroupBox( );
            this.screenPoint4ColorLabel = new System.Windows.Forms.Label( );
            this.screenPoint3ColorLabel = new System.Windows.Forms.Label( );
            this.screenPoint2ColorLabel = new System.Windows.Forms.Label( );
            this.screenPoint1ColorLabel = new System.Windows.Forms.Label( );
            this.screenPoint4Box = new System.Windows.Forms.TextBox( );
            this.label21 = new System.Windows.Forms.Label( );
            this.screenPoint3Box = new System.Windows.Forms.TextBox( );
            this.label22 = new System.Windows.Forms.Label( );
            this.screenPoint2Box = new System.Windows.Forms.TextBox( );
            this.label23 = new System.Windows.Forms.Label( );
            this.screenPoint1Box = new System.Windows.Forms.TextBox( );
            this.label24 = new System.Windows.Forms.Label( );
            this.groupBox8 = new System.Windows.Forms.GroupBox( );
            this.label20 = new System.Windows.Forms.Label( );
            this.estimatedZObjectBox = new System.Windows.Forms.TextBox( );
            this.label25 = new System.Windows.Forms.Label( );
            this.estimatedYObjectBox = new System.Windows.Forms.TextBox( );
            this.label26 = new System.Windows.Forms.Label( );
            this.estimatedXObjectBox = new System.Windows.Forms.TextBox( );
            this.estimatedYawBox = new System.Windows.Forms.TextBox( );
            this.estimatedPitchBox = new System.Windows.Forms.TextBox( );
            this.estimatedRollBox = new System.Windows.Forms.TextBox( );
            this.estimatedTransformationMatrixControl = new PoseEstimation.MatrixControl( );
            this.viewMatrixControl = new PoseEstimation.MatrixControl( );
            this.transformationMatrixControl = new PoseEstimation.MatrixControl( );
            this.groupBox2.SuspendLayout( );
            this.groupBox4.SuspendLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.errorProvider ) ).BeginInit( );
            this.groupBox3.SuspendLayout( );
            this.groupBox1.SuspendLayout( );
            this.groupBox5.SuspendLayout( );
            this.groupBox6.SuspendLayout( );
            this.groupBox7.SuspendLayout( );
            this.groupBox8.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.label1 );
            this.groupBox2.Controls.Add( this.yawBox );
            this.groupBox2.Controls.Add( this.label2 );
            this.groupBox2.Controls.Add( this.pitchBox );
            this.groupBox2.Controls.Add( this.label3 );
            this.groupBox2.Controls.Add( this.rollBox );
            this.groupBox2.Location = new System.Drawing.Point( 10, 75 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 375, 50 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Object Rotation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 23 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 45, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Yaw (y):";
            this.toolTip.SetToolTip( this.label1, "Rotation around Y axis" );
            // 
            // yawBox
            // 
            this.yawBox.Location = new System.Drawing.Point( 60, 20 );
            this.yawBox.Name = "yawBox";
            this.yawBox.Size = new System.Drawing.Size( 60, 20 );
            this.yawBox.TabIndex = 1;
            this.yawBox.TextChanged += new System.EventHandler( this.yawBox_TextChanged );
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 130, 23 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 48, 13 );
            this.label2.TabIndex = 2;
            this.label2.Text = "Pitch (x):";
            this.toolTip.SetToolTip( this.label2, "Rotation around X axis" );
            // 
            // pitchBox
            // 
            this.pitchBox.Location = new System.Drawing.Point( 180, 20 );
            this.pitchBox.Name = "pitchBox";
            this.pitchBox.Size = new System.Drawing.Size( 60, 20 );
            this.pitchBox.TabIndex = 3;
            this.pitchBox.TextChanged += new System.EventHandler( this.pitchBox_TextChanged );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 250, 23 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 42, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Roll (z):";
            this.toolTip.SetToolTip( this.label3, "Rotation around Z axis" );
            // 
            // rollBox
            // 
            this.rollBox.Location = new System.Drawing.Point( 295, 20 );
            this.rollBox.Name = "rollBox";
            this.rollBox.Size = new System.Drawing.Size( 60, 20 );
            this.rollBox.TabIndex = 5;
            this.rollBox.TextChanged += new System.EventHandler( this.rollBox_TextChanged );
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add( this.label10 );
            this.groupBox4.Controls.Add( this.zObjectBox );
            this.groupBox4.Controls.Add( this.label11 );
            this.groupBox4.Controls.Add( this.yObjectBox );
            this.groupBox4.Controls.Add( this.label12 );
            this.groupBox4.Controls.Add( this.xObjectBox );
            this.groupBox4.Location = new System.Drawing.Point( 10, 135 );
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size( 375, 50 );
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Object Position";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point( 272, 23 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 17, 13 );
            this.label10.TabIndex = 4;
            this.label10.Text = "Z:";
            // 
            // zObjectBox
            // 
            this.zObjectBox.Location = new System.Drawing.Point( 295, 20 );
            this.zObjectBox.Name = "zObjectBox";
            this.zObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.zObjectBox.TabIndex = 5;
            this.zObjectBox.TextChanged += new System.EventHandler( this.zObjectBox_TextChanged );
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point( 157, 23 );
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size( 17, 13 );
            this.label11.TabIndex = 2;
            this.label11.Text = "Y:";
            // 
            // yObjectBox
            // 
            this.yObjectBox.Location = new System.Drawing.Point( 180, 20 );
            this.yObjectBox.Name = "yObjectBox";
            this.yObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.yObjectBox.TabIndex = 3;
            this.yObjectBox.TextChanged += new System.EventHandler( this.yObjectBox_TextChanged );
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point( 40, 23 );
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size( 17, 13 );
            this.label12.TabIndex = 0;
            this.label12.Text = "X:";
            // 
            // xObjectBox
            // 
            this.xObjectBox.Location = new System.Drawing.Point( 60, 20 );
            this.xObjectBox.Name = "xObjectBox";
            this.xObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.xObjectBox.TabIndex = 1;
            this.xObjectBox.TextChanged += new System.EventHandler( this.xObjectBox_TextChanged );
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point( 10, 23 );
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size( 45, 13 );
            this.label17.TabIndex = 31;
            this.label17.Text = "Yaw (y):";
            this.toolTip.SetToolTip( this.label17, "Rotation around Y axis" );
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point( 130, 23 );
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size( 48, 13 );
            this.label18.TabIndex = 33;
            this.label18.Text = "Pitch (x):";
            this.toolTip.SetToolTip( this.label18, "Rotation around X axis" );
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point( 250, 23 );
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size( 42, 13 );
            this.label19.TabIndex = 35;
            this.label19.Text = "Roll (z):";
            this.toolTip.SetToolTip( this.label19, "Rotation around Z axis" );
            // 
            // targetTransformationMatrixControl
            // 
            this.targetTransformationMatrixControl.Location = new System.Drawing.Point( 610, 15 );
            this.targetTransformationMatrixControl.Name = "targetTransformationMatrixControl";
            this.targetTransformationMatrixControl.Size = new System.Drawing.Size( 220, 110 );
            this.targetTransformationMatrixControl.TabIndex = 37;
            this.targetTransformationMatrixControl.Title = "Target Transformation";
            this.toolTip.SetToolTip( this.targetTransformationMatrixControl, "The matrix shows target transformation which View Matrix multiplies with World Tr" +
                    "ansformation Matrix" );
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.label7 );
            this.groupBox3.Controls.Add( this.zLookAtBox );
            this.groupBox3.Controls.Add( this.label8 );
            this.groupBox3.Controls.Add( this.yLookAtBox );
            this.groupBox3.Controls.Add( this.label9 );
            this.groupBox3.Controls.Add( this.xLookAtBox );
            this.groupBox3.Location = new System.Drawing.Point( 10, 255 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 375, 50 );
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Look At";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 272, 23 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 17, 13 );
            this.label7.TabIndex = 4;
            this.label7.Text = "Z:";
            // 
            // zLookAtBox
            // 
            this.zLookAtBox.Location = new System.Drawing.Point( 295, 20 );
            this.zLookAtBox.Name = "zLookAtBox";
            this.zLookAtBox.Size = new System.Drawing.Size( 60, 20 );
            this.zLookAtBox.TabIndex = 5;
            this.zLookAtBox.TextChanged += new System.EventHandler( this.zLookAtBox_TextChanged );
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point( 157, 23 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 17, 13 );
            this.label8.TabIndex = 2;
            this.label8.Text = "Y:";
            // 
            // yLookAtBox
            // 
            this.yLookAtBox.Location = new System.Drawing.Point( 180, 20 );
            this.yLookAtBox.Name = "yLookAtBox";
            this.yLookAtBox.Size = new System.Drawing.Size( 60, 20 );
            this.yLookAtBox.TabIndex = 3;
            this.yLookAtBox.TextChanged += new System.EventHandler( this.yLookAtBox_TextChanged );
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point( 40, 23 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 17, 13 );
            this.label9.TabIndex = 0;
            this.label9.Text = "X:";
            // 
            // xLookAtBox
            // 
            this.xLookAtBox.Location = new System.Drawing.Point( 60, 20 );
            this.xLookAtBox.Name = "xLookAtBox";
            this.xLookAtBox.Size = new System.Drawing.Size( 60, 20 );
            this.xLookAtBox.TabIndex = 1;
            this.xLookAtBox.TextChanged += new System.EventHandler( this.xLookAtBox_TextChanged );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.label6 );
            this.groupBox1.Controls.Add( this.zCameraBox );
            this.groupBox1.Controls.Add( this.label5 );
            this.groupBox1.Controls.Add( this.yCameraBox );
            this.groupBox1.Controls.Add( this.label4 );
            this.groupBox1.Controls.Add( this.xCameraBox );
            this.groupBox1.Location = new System.Drawing.Point( 10, 195 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 375, 50 );
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Position";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point( 272, 23 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 17, 13 );
            this.label6.TabIndex = 4;
            this.label6.Text = "Z:";
            // 
            // zCameraBox
            // 
            this.zCameraBox.Location = new System.Drawing.Point( 295, 20 );
            this.zCameraBox.Name = "zCameraBox";
            this.zCameraBox.Size = new System.Drawing.Size( 60, 20 );
            this.zCameraBox.TabIndex = 5;
            this.zCameraBox.TextChanged += new System.EventHandler( this.zCameraBox_TextChanged );
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 157, 23 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 17, 13 );
            this.label5.TabIndex = 2;
            this.label5.Text = "Y:";
            // 
            // yCameraBox
            // 
            this.yCameraBox.Location = new System.Drawing.Point( 180, 20 );
            this.yCameraBox.Name = "yCameraBox";
            this.yCameraBox.Size = new System.Drawing.Size( 60, 20 );
            this.yCameraBox.TabIndex = 3;
            this.yCameraBox.TextChanged += new System.EventHandler( this.yCameraBox_TextChanged );
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 40, 23 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 17, 13 );
            this.label4.TabIndex = 0;
            this.label4.Text = "X:";
            // 
            // xCameraBox
            // 
            this.xCameraBox.Location = new System.Drawing.Point( 60, 20 );
            this.xCameraBox.Name = "xCameraBox";
            this.xCameraBox.Size = new System.Drawing.Size( 60, 20 );
            this.xCameraBox.TabIndex = 1;
            this.xCameraBox.TextChanged += new System.EventHandler( this.xCameraBox_TextChanged );
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add( this.worldRendererControl );
            this.groupBox5.Location = new System.Drawing.Point( 625, 75 );
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size( 220, 230 );
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Object";
            // 
            // worldRendererControl
            // 
            this.worldRendererControl.Location = new System.Drawing.Point( 10, 18 );
            this.worldRendererControl.Name = "worldRendererControl";
            this.worldRendererControl.Size = new System.Drawing.Size( 200, 200 );
            this.worldRendererControl.TabIndex = 13;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add( this.objectPoint4ColorLabel );
            this.groupBox6.Controls.Add( this.objectPoint3ColorLabel );
            this.groupBox6.Controls.Add( this.objectPoint2ColorLabel );
            this.groupBox6.Controls.Add( this.objectPoint1ColorLabel );
            this.groupBox6.Controls.Add( this.objectPoint4Box );
            this.groupBox6.Controls.Add( this.label16 );
            this.groupBox6.Controls.Add( this.objectPoint3Box );
            this.groupBox6.Controls.Add( this.label15 );
            this.groupBox6.Controls.Add( this.objectPoint2Box );
            this.groupBox6.Controls.Add( this.label14 );
            this.groupBox6.Controls.Add( this.objectPoint1Box );
            this.groupBox6.Controls.Add( this.label13 );
            this.groupBox6.Controls.Add( this.objectTypeCombo );
            this.groupBox6.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size( 835, 55 );
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Object";
            // 
            // objectPoint4ColorLabel
            // 
            this.objectPoint4ColorLabel.BackColor = System.Drawing.Color.Black;
            this.objectPoint4ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPoint4ColorLabel.Location = new System.Drawing.Point( 710, 40 );
            this.objectPoint4ColorLabel.Name = "objectPoint4ColorLabel";
            this.objectPoint4ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.objectPoint4ColorLabel.TabIndex = 27;
            // 
            // objectPoint3ColorLabel
            // 
            this.objectPoint3ColorLabel.BackColor = System.Drawing.Color.Black;
            this.objectPoint3ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPoint3ColorLabel.Location = new System.Drawing.Point( 535, 40 );
            this.objectPoint3ColorLabel.Name = "objectPoint3ColorLabel";
            this.objectPoint3ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.objectPoint3ColorLabel.TabIndex = 26;
            // 
            // objectPoint2ColorLabel
            // 
            this.objectPoint2ColorLabel.BackColor = System.Drawing.Color.Black;
            this.objectPoint2ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPoint2ColorLabel.Location = new System.Drawing.Point( 360, 40 );
            this.objectPoint2ColorLabel.Name = "objectPoint2ColorLabel";
            this.objectPoint2ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.objectPoint2ColorLabel.TabIndex = 25;
            // 
            // objectPoint1ColorLabel
            // 
            this.objectPoint1ColorLabel.BackColor = System.Drawing.Color.Black;
            this.objectPoint1ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPoint1ColorLabel.Location = new System.Drawing.Point( 185, 40 );
            this.objectPoint1ColorLabel.Name = "objectPoint1ColorLabel";
            this.objectPoint1ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.objectPoint1ColorLabel.TabIndex = 16;
            // 
            // objectPoint4Box
            // 
            this.objectPoint4Box.Location = new System.Drawing.Point( 710, 20 );
            this.objectPoint4Box.Name = "objectPoint4Box";
            this.objectPoint4Box.ReadOnly = true;
            this.objectPoint4Box.Size = new System.Drawing.Size( 115, 20 );
            this.objectPoint4Box.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point( 660, 23 );
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size( 50, 13 );
            this.label16.TabIndex = 7;
            this.label16.Text = "Point #4:";
            // 
            // objectPoint3Box
            // 
            this.objectPoint3Box.Location = new System.Drawing.Point( 535, 20 );
            this.objectPoint3Box.Name = "objectPoint3Box";
            this.objectPoint3Box.ReadOnly = true;
            this.objectPoint3Box.Size = new System.Drawing.Size( 115, 20 );
            this.objectPoint3Box.TabIndex = 6;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point( 485, 23 );
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size( 50, 13 );
            this.label15.TabIndex = 5;
            this.label15.Text = "Point #3:";
            // 
            // objectPoint2Box
            // 
            this.objectPoint2Box.Location = new System.Drawing.Point( 360, 20 );
            this.objectPoint2Box.Name = "objectPoint2Box";
            this.objectPoint2Box.ReadOnly = true;
            this.objectPoint2Box.Size = new System.Drawing.Size( 115, 20 );
            this.objectPoint2Box.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point( 310, 23 );
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size( 50, 13 );
            this.label14.TabIndex = 3;
            this.label14.Text = "Point #2:";
            // 
            // objectPoint1Box
            // 
            this.objectPoint1Box.Location = new System.Drawing.Point( 185, 20 );
            this.objectPoint1Box.Name = "objectPoint1Box";
            this.objectPoint1Box.ReadOnly = true;
            this.objectPoint1Box.Size = new System.Drawing.Size( 115, 20 );
            this.objectPoint1Box.TabIndex = 2;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point( 135, 23 );
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size( 50, 13 );
            this.label13.TabIndex = 1;
            this.label13.Text = "Point #1:";
            // 
            // objectTypeCombo
            // 
            this.objectTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectTypeCombo.FormattingEnabled = true;
            this.objectTypeCombo.Items.AddRange( new object[] {
            "POSIT",
            "CoPOSIT"} );
            this.objectTypeCombo.Location = new System.Drawing.Point( 10, 20 );
            this.objectTypeCombo.Name = "objectTypeCombo";
            this.objectTypeCombo.Size = new System.Drawing.Size( 110, 21 );
            this.objectTypeCombo.TabIndex = 0;
            this.objectTypeCombo.SelectedIndexChanged += new System.EventHandler( this.objectTypeCombo_SelectedIndexChanged );
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add( this.screenPoint4ColorLabel );
            this.groupBox7.Controls.Add( this.screenPoint3ColorLabel );
            this.groupBox7.Controls.Add( this.screenPoint2ColorLabel );
            this.groupBox7.Controls.Add( this.screenPoint1ColorLabel );
            this.groupBox7.Controls.Add( this.screenPoint4Box );
            this.groupBox7.Controls.Add( this.label21 );
            this.groupBox7.Controls.Add( this.screenPoint3Box );
            this.groupBox7.Controls.Add( this.label22 );
            this.groupBox7.Controls.Add( this.screenPoint2Box );
            this.groupBox7.Controls.Add( this.label23 );
            this.groupBox7.Controls.Add( this.screenPoint1Box );
            this.groupBox7.Controls.Add( this.label24 );
            this.groupBox7.Location = new System.Drawing.Point( 10, 310 );
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size( 835, 55 );
            this.groupBox7.TabIndex = 28;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Projected Points";
            // 
            // screenPoint4ColorLabel
            // 
            this.screenPoint4ColorLabel.BackColor = System.Drawing.Color.Black;
            this.screenPoint4ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.screenPoint4ColorLabel.Location = new System.Drawing.Point( 710, 40 );
            this.screenPoint4ColorLabel.Name = "screenPoint4ColorLabel";
            this.screenPoint4ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.screenPoint4ColorLabel.TabIndex = 27;
            // 
            // screenPoint3ColorLabel
            // 
            this.screenPoint3ColorLabel.BackColor = System.Drawing.Color.Black;
            this.screenPoint3ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.screenPoint3ColorLabel.Location = new System.Drawing.Point( 535, 40 );
            this.screenPoint3ColorLabel.Name = "screenPoint3ColorLabel";
            this.screenPoint3ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.screenPoint3ColorLabel.TabIndex = 26;
            // 
            // screenPoint2ColorLabel
            // 
            this.screenPoint2ColorLabel.BackColor = System.Drawing.Color.Black;
            this.screenPoint2ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.screenPoint2ColorLabel.Location = new System.Drawing.Point( 360, 40 );
            this.screenPoint2ColorLabel.Name = "screenPoint2ColorLabel";
            this.screenPoint2ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.screenPoint2ColorLabel.TabIndex = 25;
            // 
            // screenPoint1ColorLabel
            // 
            this.screenPoint1ColorLabel.BackColor = System.Drawing.Color.Black;
            this.screenPoint1ColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.screenPoint1ColorLabel.Location = new System.Drawing.Point( 185, 40 );
            this.screenPoint1ColorLabel.Name = "screenPoint1ColorLabel";
            this.screenPoint1ColorLabel.Size = new System.Drawing.Size( 115, 4 );
            this.screenPoint1ColorLabel.TabIndex = 16;
            // 
            // screenPoint4Box
            // 
            this.screenPoint4Box.Location = new System.Drawing.Point( 710, 20 );
            this.screenPoint4Box.Name = "screenPoint4Box";
            this.screenPoint4Box.ReadOnly = true;
            this.screenPoint4Box.Size = new System.Drawing.Size( 115, 20 );
            this.screenPoint4Box.TabIndex = 8;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point( 660, 23 );
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size( 50, 13 );
            this.label21.TabIndex = 7;
            this.label21.Text = "Point #4:";
            // 
            // screenPoint3Box
            // 
            this.screenPoint3Box.Location = new System.Drawing.Point( 535, 20 );
            this.screenPoint3Box.Name = "screenPoint3Box";
            this.screenPoint3Box.ReadOnly = true;
            this.screenPoint3Box.Size = new System.Drawing.Size( 115, 20 );
            this.screenPoint3Box.TabIndex = 6;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point( 485, 23 );
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size( 50, 13 );
            this.label22.TabIndex = 5;
            this.label22.Text = "Point #3:";
            // 
            // screenPoint2Box
            // 
            this.screenPoint2Box.Location = new System.Drawing.Point( 360, 20 );
            this.screenPoint2Box.Name = "screenPoint2Box";
            this.screenPoint2Box.ReadOnly = true;
            this.screenPoint2Box.Size = new System.Drawing.Size( 115, 20 );
            this.screenPoint2Box.TabIndex = 4;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point( 310, 23 );
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size( 50, 13 );
            this.label23.TabIndex = 3;
            this.label23.Text = "Point #2:";
            // 
            // screenPoint1Box
            // 
            this.screenPoint1Box.Location = new System.Drawing.Point( 185, 20 );
            this.screenPoint1Box.Name = "screenPoint1Box";
            this.screenPoint1Box.ReadOnly = true;
            this.screenPoint1Box.Size = new System.Drawing.Size( 115, 20 );
            this.screenPoint1Box.TabIndex = 2;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point( 135, 23 );
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size( 50, 13 );
            this.label24.TabIndex = 1;
            this.label24.Text = "Point #1:";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add( this.label20 );
            this.groupBox8.Controls.Add( this.estimatedZObjectBox );
            this.groupBox8.Controls.Add( this.label25 );
            this.groupBox8.Controls.Add( this.estimatedYObjectBox );
            this.groupBox8.Controls.Add( this.label26 );
            this.groupBox8.Controls.Add( this.estimatedXObjectBox );
            this.groupBox8.Controls.Add( this.targetTransformationMatrixControl );
            this.groupBox8.Controls.Add( this.label17 );
            this.groupBox8.Controls.Add( this.estimatedYawBox );
            this.groupBox8.Controls.Add( this.label18 );
            this.groupBox8.Controls.Add( this.estimatedPitchBox );
            this.groupBox8.Controls.Add( this.label19 );
            this.groupBox8.Controls.Add( this.estimatedRollBox );
            this.groupBox8.Controls.Add( this.estimatedTransformationMatrixControl );
            this.groupBox8.Location = new System.Drawing.Point( 10, 370 );
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size( 835, 135 );
            this.groupBox8.TabIndex = 29;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Pose Estimation";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point( 272, 48 );
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size( 17, 13 );
            this.label20.TabIndex = 42;
            this.label20.Text = "Z:";
            // 
            // estimatedZObjectBox
            // 
            this.estimatedZObjectBox.Location = new System.Drawing.Point( 295, 45 );
            this.estimatedZObjectBox.Name = "estimatedZObjectBox";
            this.estimatedZObjectBox.ReadOnly = true;
            this.estimatedZObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedZObjectBox.TabIndex = 43;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point( 157, 48 );
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size( 17, 13 );
            this.label25.TabIndex = 40;
            this.label25.Text = "Y:";
            // 
            // estimatedYObjectBox
            // 
            this.estimatedYObjectBox.Location = new System.Drawing.Point( 180, 45 );
            this.estimatedYObjectBox.Name = "estimatedYObjectBox";
            this.estimatedYObjectBox.ReadOnly = true;
            this.estimatedYObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedYObjectBox.TabIndex = 41;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point( 40, 48 );
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size( 17, 13 );
            this.label26.TabIndex = 38;
            this.label26.Text = "X:";
            // 
            // estimatedXObjectBox
            // 
            this.estimatedXObjectBox.Location = new System.Drawing.Point( 60, 45 );
            this.estimatedXObjectBox.Name = "estimatedXObjectBox";
            this.estimatedXObjectBox.ReadOnly = true;
            this.estimatedXObjectBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedXObjectBox.TabIndex = 39;
            // 
            // estimatedYawBox
            // 
            this.estimatedYawBox.Location = new System.Drawing.Point( 60, 20 );
            this.estimatedYawBox.Name = "estimatedYawBox";
            this.estimatedYawBox.ReadOnly = true;
            this.estimatedYawBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedYawBox.TabIndex = 32;
            // 
            // estimatedPitchBox
            // 
            this.estimatedPitchBox.Location = new System.Drawing.Point( 180, 20 );
            this.estimatedPitchBox.Name = "estimatedPitchBox";
            this.estimatedPitchBox.ReadOnly = true;
            this.estimatedPitchBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedPitchBox.TabIndex = 34;
            // 
            // estimatedRollBox
            // 
            this.estimatedRollBox.Location = new System.Drawing.Point( 295, 20 );
            this.estimatedRollBox.Name = "estimatedRollBox";
            this.estimatedRollBox.ReadOnly = true;
            this.estimatedRollBox.Size = new System.Drawing.Size( 60, 20 );
            this.estimatedRollBox.TabIndex = 36;
            // 
            // estimatedTransformationMatrixControl
            // 
            this.estimatedTransformationMatrixControl.Location = new System.Drawing.Point( 385, 15 );
            this.estimatedTransformationMatrixControl.Name = "estimatedTransformationMatrixControl";
            this.estimatedTransformationMatrixControl.Size = new System.Drawing.Size( 220, 110 );
            this.estimatedTransformationMatrixControl.TabIndex = 30;
            this.estimatedTransformationMatrixControl.Title = "Estimated Transformation";
            // 
            // viewMatrixControl
            // 
            this.viewMatrixControl.Location = new System.Drawing.Point( 395, 195 );
            this.viewMatrixControl.Name = "viewMatrixControl";
            this.viewMatrixControl.Size = new System.Drawing.Size( 220, 110 );
            this.viewMatrixControl.TabIndex = 6;
            this.viewMatrixControl.Title = "View Matrix";
            // 
            // transformationMatrixControl
            // 
            this.transformationMatrixControl.Location = new System.Drawing.Point( 395, 75 );
            this.transformationMatrixControl.Name = "transformationMatrixControl";
            this.transformationMatrixControl.Size = new System.Drawing.Size( 220, 110 );
            this.transformationMatrixControl.TabIndex = 5;
            this.transformationMatrixControl.Title = "World Transformation Matrix";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 856, 517 );
            this.Controls.Add( this.groupBox8 );
            this.Controls.Add( this.groupBox7 );
            this.Controls.Add( this.groupBox6 );
            this.Controls.Add( this.groupBox5 );
            this.Controls.Add( this.viewMatrixControl );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox1 );
            this.Controls.Add( this.transformationMatrixControl );
            this.Controls.Add( this.groupBox4 );
            this.Controls.Add( this.groupBox2 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Pose Estimation";
            this.Load += new System.EventHandler( this.MainForm_Load );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout( );
            this.groupBox4.ResumeLayout( false );
            this.groupBox4.PerformLayout( );
            ( (System.ComponentModel.ISupportInitialize) ( this.errorProvider ) ).EndInit( );
            this.groupBox3.ResumeLayout( false );
            this.groupBox3.PerformLayout( );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox5.ResumeLayout( false );
            this.groupBox6.ResumeLayout( false );
            this.groupBox6.PerformLayout( );
            this.groupBox7.ResumeLayout( false );
            this.groupBox7.PerformLayout( );
            this.groupBox8.ResumeLayout( false );
            this.groupBox8.PerformLayout( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox yawBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pitchBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox rollBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox zObjectBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox yObjectBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox xObjectBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private MatrixControl transformationMatrixControl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox zLookAtBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox yLookAtBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox xLookAtBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox zCameraBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox yCameraBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox xCameraBox;
        private MatrixControl viewMatrixControl;
        private System.Windows.Forms.GroupBox groupBox5;
        private WorldRendererControl worldRendererControl;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox objectPoint1Box;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox objectTypeCombo;
        private System.Windows.Forms.TextBox objectPoint4Box;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox objectPoint3Box;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox objectPoint2Box;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label objectPoint4ColorLabel;
        private System.Windows.Forms.Label objectPoint3ColorLabel;
        private System.Windows.Forms.Label objectPoint2ColorLabel;
        private System.Windows.Forms.Label objectPoint1ColorLabel;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label screenPoint4ColorLabel;
        private System.Windows.Forms.Label screenPoint3ColorLabel;
        private System.Windows.Forms.Label screenPoint2ColorLabel;
        private System.Windows.Forms.Label screenPoint1ColorLabel;
        private System.Windows.Forms.TextBox screenPoint4Box;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox screenPoint3Box;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox screenPoint2Box;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox screenPoint1Box;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.GroupBox groupBox8;
        private MatrixControl estimatedTransformationMatrixControl;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox estimatedYawBox;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox estimatedPitchBox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox estimatedRollBox;
        private MatrixControl targetTransformationMatrixControl;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox estimatedZObjectBox;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox estimatedYObjectBox;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox estimatedXObjectBox;
    }
}

