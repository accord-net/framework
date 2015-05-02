namespace RCXTest
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
            this.connectButton = new System.Windows.Forms.Button( );
            this.disconnectButton = new System.Windows.Forms.Button( );
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.powerBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.firmwareVersionBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.romVersionBox = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.playButton = new System.Windows.Forms.Button( );
            this.soundTypeCombo = new System.Windows.Forms.ComboBox( );
            this.groupBox4 = new System.Windows.Forms.GroupBox( );
            this.setSensorModeButton = new System.Windows.Forms.Button( );
            this.sensorModeCombo = new System.Windows.Forms.ComboBox( );
            this.label6 = new System.Windows.Forms.Label( );
            this.setSensorTypeButton = new System.Windows.Forms.Button( );
            this.sensorTypeCombo = new System.Windows.Forms.ComboBox( );
            this.label5 = new System.Windows.Forms.Label( );
            this.clearSensorButton = new System.Windows.Forms.Button( );
            this.valueBox = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.getValueButton = new System.Windows.Forms.Button( );
            this.sensorCombo = new System.Windows.Forms.ComboBox( );
            this.groupBox5 = new System.Windows.Forms.GroupBox( );
            this.motorOffButton = new System.Windows.Forms.Button( );
            this.motorOnButton = new System.Windows.Forms.Button( );
            this.powerCombo = new System.Windows.Forms.ComboBox( );
            this.label8 = new System.Windows.Forms.Label( );
            this.directionCombo = new System.Windows.Forms.ComboBox( );
            this.label7 = new System.Windows.Forms.Label( );
            this.motorCombo = new System.Windows.Forms.ComboBox( );
            this.turnDeviceOffButton = new System.Windows.Forms.Button( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.groupBox3.SuspendLayout( );
            this.groupBox4.SuspendLayout( );
            this.groupBox5.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point( 10, 25 );
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size( 75, 23 );
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "&Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler( this.connectButton_Click );
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point( 125, 25 );
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size( 75, 23 );
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler( this.disconnectButton_Click );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.connectButton );
            this.groupBox1.Controls.Add( this.disconnectButton );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 210, 55 );
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.powerBox );
            this.groupBox2.Controls.Add( this.label3 );
            this.groupBox2.Controls.Add( this.firmwareVersionBox );
            this.groupBox2.Controls.Add( this.label2 );
            this.groupBox2.Controls.Add( this.romVersionBox );
            this.groupBox2.Controls.Add( this.label1 );
            this.groupBox2.Location = new System.Drawing.Point( 10, 70 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 210, 100 );
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device info";
            // 
            // powerBox
            // 
            this.powerBox.Location = new System.Drawing.Point( 100, 70 );
            this.powerBox.Name = "powerBox";
            this.powerBox.ReadOnly = true;
            this.powerBox.Size = new System.Drawing.Size( 100, 20 );
            this.powerBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 73 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 91, 13 );
            this.label3.TabIndex = 4;
            this.label3.Text = "Battery power (V):";
            // 
            // firmwareVersionBox
            // 
            this.firmwareVersionBox.Location = new System.Drawing.Point( 100, 45 );
            this.firmwareVersionBox.Name = "firmwareVersionBox";
            this.firmwareVersionBox.ReadOnly = true;
            this.firmwareVersionBox.Size = new System.Drawing.Size( 100, 20 );
            this.firmwareVersionBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 48 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 89, 13 );
            this.label2.TabIndex = 2;
            this.label2.Text = "Firmware version:";
            // 
            // romVersionBox
            // 
            this.romVersionBox.Location = new System.Drawing.Point( 100, 20 );
            this.romVersionBox.Name = "romVersionBox";
            this.romVersionBox.ReadOnly = true;
            this.romVersionBox.Size = new System.Drawing.Size( 100, 20 );
            this.romVersionBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 23 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 69, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Rom version:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.playButton );
            this.groupBox3.Controls.Add( this.soundTypeCombo );
            this.groupBox3.Location = new System.Drawing.Point( 10, 175 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 210, 50 );
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sound";
            // 
            // playButton
            // 
            this.playButton.Enabled = false;
            this.playButton.Location = new System.Drawing.Point( 125, 19 );
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size( 75, 23 );
            this.playButton.TabIndex = 5;
            this.playButton.Text = "&Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler( this.playButton_Click );
            // 
            // soundTypeCombo
            // 
            this.soundTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.soundTypeCombo.FormattingEnabled = true;
            this.soundTypeCombo.Items.AddRange( new object[] {
            "Bleep",
            "Beep-beep",
            "Downward Tones",
            "Upward Tones",
            "Low Buzz",
            "Fast Upward Tones"} );
            this.soundTypeCombo.Location = new System.Drawing.Point( 10, 20 );
            this.soundTypeCombo.Name = "soundTypeCombo";
            this.soundTypeCombo.Size = new System.Drawing.Size( 110, 21 );
            this.soundTypeCombo.TabIndex = 5;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add( this.setSensorModeButton );
            this.groupBox4.Controls.Add( this.sensorModeCombo );
            this.groupBox4.Controls.Add( this.label6 );
            this.groupBox4.Controls.Add( this.setSensorTypeButton );
            this.groupBox4.Controls.Add( this.sensorTypeCombo );
            this.groupBox4.Controls.Add( this.label5 );
            this.groupBox4.Controls.Add( this.clearSensorButton );
            this.groupBox4.Controls.Add( this.valueBox );
            this.groupBox4.Controls.Add( this.label4 );
            this.groupBox4.Controls.Add( this.getValueButton );
            this.groupBox4.Controls.Add( this.sensorCombo );
            this.groupBox4.Location = new System.Drawing.Point( 230, 10 );
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size( 210, 160 );
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Sensor";
            // 
            // setSensorModeButton
            // 
            this.setSensorModeButton.Enabled = false;
            this.setSensorModeButton.Location = new System.Drawing.Point( 150, 129 );
            this.setSensorModeButton.Name = "setSensorModeButton";
            this.setSensorModeButton.Size = new System.Drawing.Size( 50, 23 );
            this.setSensorModeButton.TabIndex = 10;
            this.setSensorModeButton.Text = "Set";
            this.setSensorModeButton.UseVisualStyleBackColor = true;
            this.setSensorModeButton.Click += new System.EventHandler( this.setSensorModeButton_Click );
            // 
            // sensorModeCombo
            // 
            this.sensorModeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensorModeCombo.FormattingEnabled = true;
            this.sensorModeCombo.Items.AddRange( new object[] {
            "Raw",
            "Boolean",
            "Edge Count",
            "Pulse Count",
            "Percentage",
            "Temperature °C ",
            "Temperature °F",
            "Angle"} );
            this.sensorModeCombo.Location = new System.Drawing.Point( 50, 130 );
            this.sensorModeCombo.Name = "sensorModeCombo";
            this.sensorModeCombo.Size = new System.Drawing.Size( 90, 21 );
            this.sensorModeCombo.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point( 10, 133 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 37, 13 );
            this.label6.TabIndex = 8;
            this.label6.Text = "Mode:";
            // 
            // setSensorTypeButton
            // 
            this.setSensorTypeButton.Enabled = false;
            this.setSensorTypeButton.Location = new System.Drawing.Point( 150, 99 );
            this.setSensorTypeButton.Name = "setSensorTypeButton";
            this.setSensorTypeButton.Size = new System.Drawing.Size( 50, 23 );
            this.setSensorTypeButton.TabIndex = 7;
            this.setSensorTypeButton.Text = "Set";
            this.setSensorTypeButton.UseVisualStyleBackColor = true;
            this.setSensorTypeButton.Click += new System.EventHandler( this.setSensorTypeButton_Click );
            // 
            // sensorTypeCombo
            // 
            this.sensorTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensorTypeCombo.FormattingEnabled = true;
            this.sensorTypeCombo.Items.AddRange( new object[] {
            "Raw",
            "Touch",
            "Temperature",
            "Light",
            "Rotation"} );
            this.sensorTypeCombo.Location = new System.Drawing.Point( 50, 100 );
            this.sensorTypeCombo.Name = "sensorTypeCombo";
            this.sensorTypeCombo.Size = new System.Drawing.Size( 90, 21 );
            this.sensorTypeCombo.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 10, 103 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 34, 13 );
            this.label5.TabIndex = 5;
            this.label5.Text = "Type:";
            // 
            // clearSensorButton
            // 
            this.clearSensorButton.Enabled = false;
            this.clearSensorButton.Location = new System.Drawing.Point( 125, 49 );
            this.clearSensorButton.Name = "clearSensorButton";
            this.clearSensorButton.Size = new System.Drawing.Size( 75, 23 );
            this.clearSensorButton.TabIndex = 4;
            this.clearSensorButton.Text = "C&lear";
            this.clearSensorButton.UseVisualStyleBackColor = true;
            this.clearSensorButton.Click += new System.EventHandler( this.clearSensorButton_Click );
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point( 50, 50 );
            this.valueBox.Name = "valueBox";
            this.valueBox.ReadOnly = true;
            this.valueBox.Size = new System.Drawing.Size( 65, 20 );
            this.valueBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 53 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 37, 13 );
            this.label4.TabIndex = 2;
            this.label4.Text = "Value:";
            // 
            // getValueButton
            // 
            this.getValueButton.Enabled = false;
            this.getValueButton.Location = new System.Drawing.Point( 125, 19 );
            this.getValueButton.Name = "getValueButton";
            this.getValueButton.Size = new System.Drawing.Size( 75, 23 );
            this.getValueButton.TabIndex = 1;
            this.getValueButton.Text = "&Get value";
            this.getValueButton.UseVisualStyleBackColor = true;
            this.getValueButton.Click += new System.EventHandler( this.getValueButton_Click );
            // 
            // sensorCombo
            // 
            this.sensorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sensorCombo.FormattingEnabled = true;
            this.sensorCombo.Items.AddRange( new object[] {
            "First",
            "Second",
            "Third"} );
            this.sensorCombo.Location = new System.Drawing.Point( 6, 19 );
            this.sensorCombo.Name = "sensorCombo";
            this.sensorCombo.Size = new System.Drawing.Size( 110, 21 );
            this.sensorCombo.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add( this.motorOffButton );
            this.groupBox5.Controls.Add( this.motorOnButton );
            this.groupBox5.Controls.Add( this.powerCombo );
            this.groupBox5.Controls.Add( this.label8 );
            this.groupBox5.Controls.Add( this.directionCombo );
            this.groupBox5.Controls.Add( this.label7 );
            this.groupBox5.Controls.Add( this.motorCombo );
            this.groupBox5.Location = new System.Drawing.Point( 450, 10 );
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size( 150, 160 );
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Motor";
            // 
            // motorOffButton
            // 
            this.motorOffButton.Enabled = false;
            this.motorOffButton.Location = new System.Drawing.Point( 80, 125 );
            this.motorOffButton.Name = "motorOffButton";
            this.motorOffButton.Size = new System.Drawing.Size( 60, 23 );
            this.motorOffButton.TabIndex = 6;
            this.motorOffButton.Text = "O&ff";
            this.motorOffButton.UseVisualStyleBackColor = true;
            this.motorOffButton.Click += new System.EventHandler( this.motorOffButton_Click );
            // 
            // motorOnButton
            // 
            this.motorOnButton.Enabled = false;
            this.motorOnButton.Location = new System.Drawing.Point( 10, 125 );
            this.motorOnButton.Name = "motorOnButton";
            this.motorOnButton.Size = new System.Drawing.Size( 60, 23 );
            this.motorOnButton.TabIndex = 5;
            this.motorOnButton.Text = "O&n";
            this.motorOnButton.UseVisualStyleBackColor = true;
            this.motorOnButton.Click += new System.EventHandler( this.motorOnButton_Click );
            // 
            // powerCombo
            // 
            this.powerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.powerCombo.FormattingEnabled = true;
            this.powerCombo.Items.AddRange( new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"} );
            this.powerCombo.Location = new System.Drawing.Point( 65, 80 );
            this.powerCombo.Name = "powerCombo";
            this.powerCombo.Size = new System.Drawing.Size( 75, 21 );
            this.powerCombo.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point( 10, 83 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 40, 13 );
            this.label8.TabIndex = 3;
            this.label8.Text = "Power:";
            // 
            // directionCombo
            // 
            this.directionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.directionCombo.FormattingEnabled = true;
            this.directionCombo.Items.AddRange( new object[] {
            "Forward",
            "Backward"} );
            this.directionCombo.Location = new System.Drawing.Point( 65, 50 );
            this.directionCombo.Name = "directionCombo";
            this.directionCombo.Size = new System.Drawing.Size( 75, 21 );
            this.directionCombo.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 10, 53 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 52, 13 );
            this.label7.TabIndex = 1;
            this.label7.Text = "Direction:";
            // 
            // motorCombo
            // 
            this.motorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.motorCombo.FormattingEnabled = true;
            this.motorCombo.Items.AddRange( new object[] {
            "Motor A",
            "Motor B",
            "Motor C",
            "Motor A+C"} );
            this.motorCombo.Location = new System.Drawing.Point( 10, 20 );
            this.motorCombo.Name = "motorCombo";
            this.motorCombo.Size = new System.Drawing.Size( 130, 21 );
            this.motorCombo.TabIndex = 0;
            // 
            // turnDeviceOffButton
            // 
            this.turnDeviceOffButton.Enabled = false;
            this.turnDeviceOffButton.Location = new System.Drawing.Point( 480, 195 );
            this.turnDeviceOffButton.Name = "turnDeviceOffButton";
            this.turnDeviceOffButton.Size = new System.Drawing.Size( 110, 23 );
            this.turnDeviceOffButton.TabIndex = 7;
            this.turnDeviceOffButton.Text = "&Turn device OFF";
            this.turnDeviceOffButton.UseVisualStyleBackColor = true;
            this.turnDeviceOffButton.Click += new System.EventHandler( this.turnDeviceOffButton_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 612, 234 );
            this.Controls.Add( this.turnDeviceOffButton );
            this.Controls.Add( this.groupBox5 );
            this.Controls.Add( this.groupBox4 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Lego RCX Test";
            this.groupBox1.ResumeLayout( false );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout( );
            this.groupBox3.ResumeLayout( false );
            this.groupBox4.ResumeLayout( false );
            this.groupBox4.PerformLayout( );
            this.groupBox5.ResumeLayout( false );
            this.groupBox5.PerformLayout( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox romVersionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox firmwareVersionBox;
        private System.Windows.Forms.TextBox powerBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox soundTypeCombo;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button getValueButton;
        private System.Windows.Forms.ComboBox sensorCombo;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button clearSensorButton;
        private System.Windows.Forms.ComboBox sensorTypeCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button setSensorTypeButton;
        private System.Windows.Forms.Button setSensorModeButton;
        private System.Windows.Forms.ComboBox sensorModeCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox motorCombo;
        private System.Windows.Forms.ComboBox directionCombo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button motorOffButton;
        private System.Windows.Forms.Button motorOnButton;
        private System.Windows.Forms.ComboBox powerCombo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button turnDeviceOffButton;
    }
}

