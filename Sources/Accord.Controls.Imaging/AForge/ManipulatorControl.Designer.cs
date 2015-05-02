namespace AForge.Controls
{
    partial class ManipulatorControl
    {
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// 
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.components = new System.ComponentModel.Container( );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.SuspendLayout( );
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler( this.timer_Tick );
            // 
            // ManipulatorControl
            // 
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.ManipulatorControl_Paint );
            this.MouseMove += new System.Windows.Forms.MouseEventHandler( this.ManipulatorControl_MouseMove );
            this.MouseDown += new System.Windows.Forms.MouseEventHandler( this.ManipulatorControl_MouseDown );
            this.MouseUp += new System.Windows.Forms.MouseEventHandler( this.ManipulatorControl_MouseUp );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.ComponentModel.IContainer components;
    }
}
