namespace Animat
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
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.worldSizeBox = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.cellWorld = new Animat.CellWorld( );
            this.loadButton = new System.Windows.Forms.Button( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.algorithmCombo = new System.Windows.Forms.ComboBox( );
            this.label10 = new System.Windows.Forms.Label( );
            this.goalRewardBox = new System.Windows.Forms.TextBox( );
            this.label9 = new System.Windows.Forms.Label( );
            this.wallRewardBox = new System.Windows.Forms.TextBox( );
            this.label8 = new System.Windows.Forms.Label( );
            this.moveRewardBox = new System.Windows.Forms.TextBox( );
            this.label7 = new System.Windows.Forms.Label( );
            this.label6 = new System.Windows.Forms.Label( );
            this.iterationsBox = new System.Windows.Forms.TextBox( );
            this.label5 = new System.Windows.Forms.Label( );
            this.learningRateBox = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.explorationRateBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.showSolutionButton = new System.Windows.Forms.Button( );
            this.iterationBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.stopButton = new System.Windows.Forms.Button( );
            this.startLearningButton = new System.Windows.Forms.Button( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.groupBox3.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.worldSizeBox );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Controls.Add( this.cellWorld );
            this.groupBox1.Controls.Add( this.loadButton );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 300, 335 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map";
            // 
            // worldSizeBox
            // 
            this.worldSizeBox.Location = new System.Drawing.Point( 220, 307 );
            this.worldSizeBox.Name = "worldSizeBox";
            this.worldSizeBox.ReadOnly = true;
            this.worldSizeBox.Size = new System.Drawing.Size( 70, 20 );
            this.worldSizeBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 160, 309 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 59, 13 );
            this.label1.TabIndex = 2;
            this.label1.Text = "World size:";
            // 
            // cellWorld
            // 
            this.cellWorld.Coloring = null;
            this.cellWorld.Location = new System.Drawing.Point( 10, 20 );
            this.cellWorld.Map = null;
            this.cellWorld.Name = "cellWorld";
            this.cellWorld.Size = new System.Drawing.Size( 280, 280 );
            this.cellWorld.TabIndex = 1;
            this.cellWorld.Text = "cellWorld1";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point( 10, 305 );
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size( 75, 23 );
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "&Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler( this.loadButton_Click );
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "MAP (World\'s map file) (*.map)|*.map";
            this.openFileDialog.Title = "Select map file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.algorithmCombo );
            this.groupBox2.Controls.Add( this.label10 );
            this.groupBox2.Controls.Add( this.goalRewardBox );
            this.groupBox2.Controls.Add( this.label9 );
            this.groupBox2.Controls.Add( this.wallRewardBox );
            this.groupBox2.Controls.Add( this.label8 );
            this.groupBox2.Controls.Add( this.moveRewardBox );
            this.groupBox2.Controls.Add( this.label7 );
            this.groupBox2.Controls.Add( this.label6 );
            this.groupBox2.Controls.Add( this.iterationsBox );
            this.groupBox2.Controls.Add( this.label5 );
            this.groupBox2.Controls.Add( this.learningRateBox );
            this.groupBox2.Controls.Add( this.label4 );
            this.groupBox2.Controls.Add( this.explorationRateBox );
            this.groupBox2.Controls.Add( this.label3 );
            this.groupBox2.Location = new System.Drawing.Point( 320, 10 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 187, 220 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // algorithmCombo
            // 
            this.algorithmCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmCombo.FormattingEnabled = true;
            this.algorithmCombo.Items.AddRange( new object[] {
            "Q-Learning",
            "Sarsa"} );
            this.algorithmCombo.Location = new System.Drawing.Point( 105, 20 );
            this.algorithmCombo.Name = "algorithmCombo";
            this.algorithmCombo.Size = new System.Drawing.Size( 70, 21 );
            this.algorithmCombo.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point( 10, 22 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 96, 13 );
            this.label10.TabIndex = 0;
            this.label10.Text = "Learning &algorithm:";
            // 
            // goalRewardBox
            // 
            this.goalRewardBox.Location = new System.Drawing.Point( 125, 180 );
            this.goalRewardBox.Name = "goalRewardBox";
            this.goalRewardBox.Size = new System.Drawing.Size( 50, 20 );
            this.goalRewardBox.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point( 10, 182 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 70, 15 );
            this.label9.TabIndex = 12;
            this.label9.Text = "&Goal reward:";
            // 
            // wallRewardBox
            // 
            this.wallRewardBox.Location = new System.Drawing.Point( 125, 155 );
            this.wallRewardBox.Name = "wallRewardBox";
            this.wallRewardBox.Size = new System.Drawing.Size( 50, 20 );
            this.wallRewardBox.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point( 10, 157 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 80, 15 );
            this.label8.TabIndex = 10;
            this.label8.Text = "&Wall reward:";
            // 
            // moveRewardBox
            // 
            this.moveRewardBox.Location = new System.Drawing.Point( 125, 130 );
            this.moveRewardBox.Name = "moveRewardBox";
            this.moveRewardBox.Size = new System.Drawing.Size( 50, 20 );
            this.moveRewardBox.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point( 10, 132 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 100, 15 );
            this.label7.TabIndex = 8;
            this.label7.Text = "&Move reward:";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point( 10, 121 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 165, 2 );
            this.label6.TabIndex = 14;
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point( 125, 95 );
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size( 50, 20 );
            this.iterationsBox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 10, 97 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 96, 13 );
            this.label5.TabIndex = 6;
            this.label5.Text = "Learning &iterations:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point( 125, 70 );
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size( 50, 20 );
            this.learningRateBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 10, 72 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 95, 13 );
            this.label4.TabIndex = 4;
            this.label4.Text = "Initial learning &rate:";
            // 
            // explorationRateBox
            // 
            this.explorationRateBox.Location = new System.Drawing.Point( 125, 45 );
            this.explorationRateBox.Name = "explorationRateBox";
            this.explorationRateBox.Size = new System.Drawing.Size( 50, 20 );
            this.explorationRateBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 10, 48 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 109, 13 );
            this.label3.TabIndex = 2;
            this.label3.Text = "Initial &exploration rate:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.showSolutionButton );
            this.groupBox3.Controls.Add( this.iterationBox );
            this.groupBox3.Controls.Add( this.label2 );
            this.groupBox3.Controls.Add( this.stopButton );
            this.groupBox3.Controls.Add( this.startLearningButton );
            this.groupBox3.Location = new System.Drawing.Point( 320, 235 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 187, 110 );
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Learning";
            // 
            // showSolutionButton
            // 
            this.showSolutionButton.Enabled = false;
            this.showSolutionButton.Location = new System.Drawing.Point( 10, 80 );
            this.showSolutionButton.Name = "showSolutionButton";
            this.showSolutionButton.Size = new System.Drawing.Size( 155, 23 );
            this.showSolutionButton.TabIndex = 4;
            this.showSolutionButton.Text = "S&how solution";
            this.showSolutionButton.UseVisualStyleBackColor = true;
            this.showSolutionButton.Click += new System.EventHandler( this.showSolutionButton_Click );
            // 
            // iterationBox
            // 
            this.iterationBox.Location = new System.Drawing.Point( 65, 20 );
            this.iterationBox.Name = "iterationBox";
            this.iterationBox.ReadOnly = true;
            this.iterationBox.Size = new System.Drawing.Size( 100, 20 );
            this.iterationBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 10, 23 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 48, 13 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Iteration:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point( 90, 50 );
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size( 75, 23 );
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "S&top";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
            // 
            // startLearningButton
            // 
            this.startLearningButton.Enabled = false;
            this.startLearningButton.Location = new System.Drawing.Point( 10, 50 );
            this.startLearningButton.Name = "startLearningButton";
            this.startLearningButton.Size = new System.Drawing.Size( 75, 23 );
            this.startLearningButton.TabIndex = 2;
            this.startLearningButton.Text = "&Start";
            this.startLearningButton.UseVisualStyleBackColor = true;
            this.startLearningButton.Click += new System.EventHandler( this.startLearningButton_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 519, 355 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Animat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout( );
            this.groupBox3.ResumeLayout( false );
            this.groupBox3.PerformLayout( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private CellWorld cellWorld;
        private System.Windows.Forms.TextBox worldSizeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox iterationBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startLearningButton;
        private System.Windows.Forms.Button showSolutionButton;
        private System.Windows.Forms.TextBox learningRateBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox explorationRateBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox goalRewardBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox wallRewardBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox moveRewardBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox algorithmCombo;
        private System.Windows.Forms.Label label10;
    }
}

