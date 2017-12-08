namespace SampleApp
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.worldSizeBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cellWorld = new SampleApp.CellWorld();
            this.loadButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.algorithmCombo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.goalRewardBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.wallRewardBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.moveRewardBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.explorationRateBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.showSolutionButton = new System.Windows.Forms.Button();
            this.iterationBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.startLearningButton = new System.Windows.Forms.Button();
            this.cbGlobal = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.worldSizeBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cellWorld);
            this.groupBox1.Controls.Add(this.loadButton);
            this.groupBox1.Location = new System.Drawing.Point(20, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(600, 644);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map";
            // 
            // worldSizeBox
            // 
            this.worldSizeBox.Location = new System.Drawing.Point(440, 590);
            this.worldSizeBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.worldSizeBox.Name = "worldSizeBox";
            this.worldSizeBox.ReadOnly = true;
            this.worldSizeBox.Size = new System.Drawing.Size(136, 31);
            this.worldSizeBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(320, 594);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "World size:";
            // 
            // cellWorld
            // 
            this.cellWorld.Coloring = null;
            this.cellWorld.Location = new System.Drawing.Point(20, 38);
            this.cellWorld.Map = null;
            this.cellWorld.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cellWorld.Name = "cellWorld";
            this.cellWorld.Size = new System.Drawing.Size(560, 538);
            this.cellWorld.TabIndex = 1;
            this.cellWorld.Text = "cellWorld1";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(20, 587);
            this.loadButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(150, 44);
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "&Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "MAP (World\'s map file) (*.map)|*.map";
            this.openFileDialog.Title = "Select map file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbGlobal);
            this.groupBox2.Controls.Add(this.algorithmCombo);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.goalRewardBox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.wallRewardBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.moveRewardBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.iterationsBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.learningRateBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.explorationRateBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(640, 19);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(374, 423);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // algorithmCombo
            // 
            this.algorithmCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmCombo.FormattingEnabled = true;
            this.algorithmCombo.Items.AddRange(new object[] {
            "Q-Learning",
            "Sarsa"});
            this.algorithmCombo.Location = new System.Drawing.Point(210, 38);
            this.algorithmCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.algorithmCombo.Name = "algorithmCombo";
            this.algorithmCombo.Size = new System.Drawing.Size(136, 33);
            this.algorithmCombo.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 42);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(196, 25);
            this.label10.TabIndex = 0;
            this.label10.Text = "Learning &algorithm:";
            // 
            // goalRewardBox
            // 
            this.goalRewardBox.Location = new System.Drawing.Point(250, 341);
            this.goalRewardBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.goalRewardBox.Name = "goalRewardBox";
            this.goalRewardBox.Size = new System.Drawing.Size(96, 31);
            this.goalRewardBox.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(20, 342);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 29);
            this.label9.TabIndex = 12;
            this.label9.Text = "&Goal reward:";
            // 
            // wallRewardBox
            // 
            this.wallRewardBox.Location = new System.Drawing.Point(250, 296);
            this.wallRewardBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.wallRewardBox.Name = "wallRewardBox";
            this.wallRewardBox.Size = new System.Drawing.Size(96, 31);
            this.wallRewardBox.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(20, 297);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 29);
            this.label8.TabIndex = 10;
            this.label8.Text = "&Wall reward:";
            // 
            // moveRewardBox
            // 
            this.moveRewardBox.Location = new System.Drawing.Point(250, 253);
            this.moveRewardBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.moveRewardBox.Name = "moveRewardBox";
            this.moveRewardBox.Size = new System.Drawing.Size(96, 31);
            this.moveRewardBox.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(20, 254);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(200, 29);
            this.label7.TabIndex = 8;
            this.label7.Text = "&Move reward:";
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(20, 233);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(328, 2);
            this.label6.TabIndex = 14;
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(250, 183);
            this.iterationsBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(96, 31);
            this.iterationsBox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 187);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(196, 25);
            this.label5.TabIndex = 6;
            this.label5.Text = "Learning &iterations:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point(250, 135);
            this.learningRateBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size(96, 31);
            this.learningRateBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 138);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(194, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "Initial learning &rate:";
            // 
            // explorationRateBox
            // 
            this.explorationRateBox.Location = new System.Drawing.Point(250, 87);
            this.explorationRateBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.explorationRateBox.Name = "explorationRateBox";
            this.explorationRateBox.Size = new System.Drawing.Size(96, 31);
            this.explorationRateBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 92);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Initial &exploration rate:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.showSolutionButton);
            this.groupBox3.Controls.Add(this.iterationBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.stopButton);
            this.groupBox3.Controls.Add(this.startLearningButton);
            this.groupBox3.Location = new System.Drawing.Point(640, 452);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox3.Size = new System.Drawing.Size(374, 212);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Learning";
            // 
            // showSolutionButton
            // 
            this.showSolutionButton.Enabled = false;
            this.showSolutionButton.Location = new System.Drawing.Point(20, 154);
            this.showSolutionButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.showSolutionButton.Name = "showSolutionButton";
            this.showSolutionButton.Size = new System.Drawing.Size(310, 44);
            this.showSolutionButton.TabIndex = 4;
            this.showSolutionButton.Text = "S&how solution";
            this.showSolutionButton.UseVisualStyleBackColor = true;
            this.showSolutionButton.Click += new System.EventHandler(this.showSolutionButton_Click);
            // 
            // iterationBox
            // 
            this.iterationBox.Location = new System.Drawing.Point(130, 38);
            this.iterationBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.iterationBox.Name = "iterationBox";
            this.iterationBox.ReadOnly = true;
            this.iterationBox.Size = new System.Drawing.Size(196, 31);
            this.iterationBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "Iteration:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(180, 96);
            this.stopButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(150, 44);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "S&top";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startLearningButton
            // 
            this.startLearningButton.Enabled = false;
            this.startLearningButton.Location = new System.Drawing.Point(20, 96);
            this.startLearningButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.startLearningButton.Name = "startLearningButton";
            this.startLearningButton.Size = new System.Drawing.Size(150, 44);
            this.startLearningButton.TabIndex = 2;
            this.startLearningButton.Text = "&Start";
            this.startLearningButton.UseVisualStyleBackColor = true;
            this.startLearningButton.Click += new System.EventHandler(this.startLearningButton_Click);
            // 
            // cbGlobal
            // 
            this.cbGlobal.AutoSize = true;
            this.cbGlobal.Location = new System.Drawing.Point(23, 385);
            this.cbGlobal.Name = "cbGlobal";
            this.cbGlobal.Size = new System.Drawing.Size(211, 29);
            this.cbGlobal.TabIndex = 15;
            this.cbGlobal.Text = "Global navigation";
            this.cbGlobal.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 683);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Animat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox cbGlobal;
    }
}

