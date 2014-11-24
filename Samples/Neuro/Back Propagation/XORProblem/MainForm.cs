// XOR Problem solution using Multi-Layer Neural Network
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace XORProblem
{
	/// <summary>
    /// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox alphaBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox errorLimitBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox sigmoidTypeCombo;
		private System.Windows.Forms.TextBox currentErrorBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox saveFilesCheck;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox momentumBox;
        private Chart errorChart;
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private double		learningRate = 0.1;
		private double		momentum = 0.0;
		private double		sigmoidAlphaValue = 2.0;
		private double		learningErrorLimit = 0.1;
		private int			sigmoidType = 0;
		private bool		saveStatisticsToFiles = false;

		private Thread workerThread = null;
        private volatile bool needToStop = false;


		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// update controls
			UpdateSettings();

			// initialize charts
			errorChart.AddDataSeries( "error", Color.Red, Chart.SeriesType.Line, 1 );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox( );
            this.momentumBox = new System.Windows.Forms.TextBox( );
            this.label6 = new System.Windows.Forms.Label( );
            this.stopButton = new System.Windows.Forms.Button( );
            this.startButton = new System.Windows.Forms.Button( );
            this.label5 = new System.Windows.Forms.Label( );
            this.currentErrorBox = new System.Windows.Forms.TextBox( );
            this.label11 = new System.Windows.Forms.Label( );
            this.currentIterationBox = new System.Windows.Forms.TextBox( );
            this.label8 = new System.Windows.Forms.Label( );
            this.label7 = new System.Windows.Forms.Label( );
            this.sigmoidTypeCombo = new System.Windows.Forms.ComboBox( );
            this.errorLimitBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.alphaBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.learningRateBox = new System.Windows.Forms.TextBox( );
            this.label1 = new System.Windows.Forms.Label( );
            this.label4 = new System.Windows.Forms.Label( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.errorChart = new AForge.Controls.Chart( );
            this.saveFilesCheck = new System.Windows.Forms.CheckBox( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.momentumBox );
            this.groupBox1.Controls.Add( this.label6 );
            this.groupBox1.Controls.Add( this.stopButton );
            this.groupBox1.Controls.Add( this.startButton );
            this.groupBox1.Controls.Add( this.label5 );
            this.groupBox1.Controls.Add( this.currentErrorBox );
            this.groupBox1.Controls.Add( this.label11 );
            this.groupBox1.Controls.Add( this.currentIterationBox );
            this.groupBox1.Controls.Add( this.label8 );
            this.groupBox1.Controls.Add( this.label7 );
            this.groupBox1.Controls.Add( this.sigmoidTypeCombo );
            this.groupBox1.Controls.Add( this.errorLimitBox );
            this.groupBox1.Controls.Add( this.label3 );
            this.groupBox1.Controls.Add( this.alphaBox );
            this.groupBox1.Controls.Add( this.label2 );
            this.groupBox1.Controls.Add( this.learningRateBox );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Controls.Add( this.label4 );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 195, 260 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Neural Network";
            // 
            // momentumBox
            // 
            this.momentumBox.Location = new System.Drawing.Point( 125, 45 );
            this.momentumBox.Name = "momentumBox";
            this.momentumBox.Size = new System.Drawing.Size( 60, 20 );
            this.momentumBox.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point( 10, 47 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 82, 17 );
            this.label6.TabIndex = 2;
            this.label6.Text = "Momentum:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point( 110, 225 );
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size( 75, 23 );
            this.stopButton.TabIndex = 28;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point( 25, 225 );
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size( 75, 23 );
            this.startButton.TabIndex = 27;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler( this.startButton_Click );
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point( 10, 211 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 175, 2 );
            this.label5.TabIndex = 26;
            // 
            // currentErrorBox
            // 
            this.currentErrorBox.Location = new System.Drawing.Point( 125, 185 );
            this.currentErrorBox.Name = "currentErrorBox";
            this.currentErrorBox.ReadOnly = true;
            this.currentErrorBox.Size = new System.Drawing.Size( 60, 20 );
            this.currentErrorBox.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point( 10, 187 );
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size( 121, 14 );
            this.label11.TabIndex = 24;
            this.label11.Text = "Current summary error:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point( 125, 160 );
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size( 60, 20 );
            this.currentIterationBox.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point( 10, 162 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 98, 16 );
            this.label8.TabIndex = 22;
            this.label8.Text = "Current iteration:";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point( 10, 150 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 175, 2 );
            this.label7.TabIndex = 21;
            // 
            // sigmoidTypeCombo
            // 
            this.sigmoidTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sigmoidTypeCombo.Items.AddRange( new object[] {
            "Unipolar",
            "Bipolar"} );
            this.sigmoidTypeCombo.Location = new System.Drawing.Point( 125, 120 );
            this.sigmoidTypeCombo.Name = "sigmoidTypeCombo";
            this.sigmoidTypeCombo.Size = new System.Drawing.Size( 60, 21 );
            this.sigmoidTypeCombo.TabIndex = 9;
            // 
            // errorLimitBox
            // 
            this.errorLimitBox.Location = new System.Drawing.Point( 125, 95 );
            this.errorLimitBox.Name = "errorLimitBox";
            this.errorLimitBox.Size = new System.Drawing.Size( 60, 20 );
            this.errorLimitBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 10, 97 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 110, 15 );
            this.label3.TabIndex = 6;
            this.label3.Text = "Learning error limit:";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point( 125, 70 );
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size( 60, 20 );
            this.alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 10, 72 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 120, 15 );
            this.label2.TabIndex = 4;
            this.label2.Text = "Sigmoid\'s alpha value:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point( 125, 20 );
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size( 60, 20 );
            this.learningRateBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 10, 22 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 78, 14 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Learning rate:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 10, 122 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 100, 15 );
            this.label4.TabIndex = 8;
            this.label4.Text = "Sigmoid\'s type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.errorChart );
            this.groupBox2.Controls.Add( this.saveFilesCheck );
            this.groupBox2.Location = new System.Drawing.Point( 215, 10 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 220, 260 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Error\'s dynamics";
            // 
            // errorChart
            // 
            this.errorChart.Location = new System.Drawing.Point( 10, 20 );
            this.errorChart.Name = "errorChart";
            this.errorChart.Size = new System.Drawing.Size( 200, 205 );
            this.errorChart.TabIndex = 2;
            this.errorChart.Text = "chart1";
            // 
            // saveFilesCheck
            // 
            this.saveFilesCheck.Location = new System.Drawing.Point( 10, 233 );
            this.saveFilesCheck.Name = "saveFilesCheck";
            this.saveFilesCheck.Size = new System.Drawing.Size( 200, 18 );
            this.saveFilesCheck.TabIndex = 1;
            this.saveFilesCheck.Text = "Save errors to files";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 452, 278 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "XOR Problem";
            this.Closing += new System.ComponentModel.CancelEventHandler( this.MainForm_Closing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.ResumeLayout( false );

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( ) 
		{
			Application.Run( new MainForm( ) );
		}

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback( System.Windows.Forms.Control control, string text );

        // Thread safe updating of control's text property
        private void SetText( System.Windows.Forms.Control control, string text )
        {
            if ( control.InvokeRequired )
            {
                SetTextCallback d = new SetTextCallback( SetText );
                Invoke( d, new object[] { control, text } );
            }
            else
            {
                control.Text = text;
            }
        }

        // On main form closing
		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// check if worker thread is running
			if ( ( workerThread != null ) && ( workerThread.IsAlive ) )
			{
				needToStop = true;
                while ( !workerThread.Join( 100 ) )
                    Application.DoEvents( );
            }
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			learningRateBox.Text	= learningRate.ToString( );
			momentumBox.Text		= momentum.ToString( );
			alphaBox.Text			= sigmoidAlphaValue.ToString( );
			errorLimitBox.Text		= learningErrorLimit.ToString( );
			sigmoidTypeCombo.SelectedIndex = sigmoidType;

			saveFilesCheck.Checked = saveStatisticsToFiles;
		}

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback( bool enable );

        // Enable/disale controls (safe for threading)
        private void EnableControls( bool enable )
		{
            if ( InvokeRequired )
            {
                EnableCallback d = new EnableCallback( EnableControls );
                Invoke( d, new object[] { enable } );
            }
            else
            {
			    learningRateBox.Enabled		= enable;
			    momentumBox.Enabled			= enable;
			    alphaBox.Enabled			= enable;
			    errorLimitBox.Enabled		= enable;
			    sigmoidTypeCombo.Enabled	= enable;
			    saveFilesCheck.Enabled		= enable;

			    startButton.Enabled			= enable;
			    stopButton.Enabled			= !enable;
		    }
        }

		// On "Start" button click
		private void startButton_Click( object sender, System.EventArgs e )
		{
			// get learning rate
			try
			{
				learningRate = Math.Max( 0.00001, Math.Min( 1, double.Parse( learningRateBox.Text ) ) );
			}
			catch
			{
				learningRate = 0.1;
			}
			// get momentum
			try
			{
				momentum = Math.Max( 0, Math.Min( 0.5, double.Parse( momentumBox.Text ) ) );
			}
			catch
			{
				momentum = 0;
			}
			// get sigmoid's alpha value
			try
			{
				sigmoidAlphaValue = Math.Max( 0.01, Math.Min( 100, double.Parse( alphaBox.Text ) ) );
			}
			catch
			{
				sigmoidAlphaValue = 2;
			}
			// get learning error limit
			try
			{
				learningErrorLimit = Math.Max( 0, double.Parse( errorLimitBox.Text ) );
			}
			catch
			{
				learningErrorLimit = 0.1;
			}
			// get sigmoid's type
			sigmoidType = sigmoidTypeCombo.SelectedIndex;

			saveStatisticsToFiles = saveFilesCheck.Checked;

			// update settings controls
			UpdateSettings( );

			// disable all settings controls
			EnableControls( false );

			// run worker thread
			needToStop = false;
			workerThread = new Thread( new ThreadStart( SearchSolution ) );
			workerThread.Start( );
		}

		// On "Stop" button click
		private void stopButton_Click( object sender, System.EventArgs e )
		{
			// stop worker thread
			needToStop = true;
            while ( !workerThread.Join( 100 ) )
                Application.DoEvents( );
            workerThread = null;
		}

		// Worker thread
		void SearchSolution( )
		{
			// initialize input and output values
			double[][] input = null;
			double[][] output = null;

			if ( sigmoidType == 0 )
			{
				// unipolar data
				input = new double[4][] {
											new double[] {0, 0},
											new double[] {0, 1},
											new double[] {1, 0},
											new double[] {1, 1}
										};
				output = new double[4][] {
											 new double[] {0},
											 new double[] {1},
											 new double[] {1},
											 new double[] {0}
										 };
			}
			else
			{
				// biipolar data
				input = new double[4][] {
											new double[] {-1, -1},
											new double[] {-1,  1},
											new double[] { 1, -1},
											new double[] { 1,  1}
										};
				output = new double[4][] {
											 new double[] {-1},
											 new double[] { 1},
											 new double[] { 1},
											 new double[] {-1}
										 };
			}

			// create neural network
			ActivationNetwork	network = new ActivationNetwork(
				( sigmoidType == 0 ) ? 
					(IActivationFunction) new SigmoidFunction( sigmoidAlphaValue ) :
					(IActivationFunction) new BipolarSigmoidFunction( sigmoidAlphaValue ),
				2, 2, 1 );
			// create teacher
			BackPropagationLearning teacher = new BackPropagationLearning( network );
			// set learning rate and momentum
			teacher.LearningRate	= learningRate;
			teacher.Momentum		= momentum;

			// iterations
			int iteration = 1;

			// statistic files
			StreamWriter errorsFile = null;

			try
			{
				// check if we need to save statistics to files
				if ( saveStatisticsToFiles )
				{
					// open files
					errorsFile	= File.CreateText( "errors.csv" );
				}
				
				// erros list
				ArrayList errorsList = new ArrayList( );

				// loop
				while ( !needToStop )
				{
					// run epoch of learning procedure
					double error = teacher.RunEpoch( input, output );
					errorsList.Add( error );

					// save current error
					if ( errorsFile != null )
					{
						errorsFile.WriteLine( error );
					}				

					// show current iteration & error
                    SetText( currentIterationBox, iteration.ToString( ) );
                    SetText( currentErrorBox, error.ToString( ) );
					iteration++;

					// check if we need to stop
					if ( error <= learningErrorLimit )
						break;
				}

				// show error's dynamics
				double[,] errors = new double[errorsList.Count, 2];

				for ( int i = 0, n = errorsList.Count; i < n; i++ )
				{
					errors[i, 0] = i;
					errors[i, 1] = (double) errorsList[i];
				}

				errorChart.RangeX = new Range( 0, errorsList.Count - 1 );
				errorChart.UpdateDataSeries( "error", errors );
			}
			catch ( IOException )
			{
				MessageBox.Show( "Failed writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				// close files
				if ( errorsFile != null )
					errorsFile.Close( );
			}

			// enable settings controls
			EnableControls( true );
		}
	}
}
