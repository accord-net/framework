namespace TexturesDemo
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
            this.label1 = new System.Windows.Forms.Label( );
            this.texturesCombo = new System.Windows.Forms.ComboBox( );
            this.pictureBox = new System.Windows.Forms.PictureBox( );
            this.regenerateButton = new System.Windows.Forms.Button( );
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 10, 12 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 46, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "&Texture:";
            // 
            // texturesCombo
            // 
            this.texturesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.texturesCombo.FormattingEnabled = true;
            this.texturesCombo.Items.AddRange( new object[] {
            "Clouds",
            "Marble",
            "Wood",
            "Labyrinth",
            "Textile"} );
            this.texturesCombo.Location = new System.Drawing.Point( 60, 10 );
            this.texturesCombo.Name = "texturesCombo";
            this.texturesCombo.Size = new System.Drawing.Size( 150, 21 );
            this.texturesCombo.TabIndex = 1;
            this.texturesCombo.SelectedIndexChanged += new System.EventHandler( this.texturesCombo_SelectedIndexChanged );
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point( 10, 40 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 300, 300 );
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // regenerateButton
            // 
            this.regenerateButton.Location = new System.Drawing.Point( 235, 10 );
            this.regenerateButton.Name = "regenerateButton";
            this.regenerateButton.Size = new System.Drawing.Size( 75, 23 );
            this.regenerateButton.TabIndex = 3;
            this.regenerateButton.Text = "&Regenerate";
            this.regenerateButton.UseVisualStyleBackColor = true;
            this.regenerateButton.Click += new System.EventHandler( this.regenerateButton_Click );
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 320, 350 );
            this.Controls.Add( this.regenerateButton );
            this.Controls.Add( this.pictureBox );
            this.Controls.Add( this.texturesCombo );
            this.Controls.Add( this.label1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Textures Demo";
            ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox texturesCombo;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button regenerateButton;
    }
}

