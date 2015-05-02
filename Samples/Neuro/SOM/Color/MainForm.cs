// Color Clustering using Kohonen SOM
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace Color
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel mapPanel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox rateBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox radiusBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button randomizeButton;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox currentIterationBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private DistanceNetwork	network;
		private Bitmap			mapBitmap;
		private Random			rand = new Random();

		private int				iterations = 5000;
		private double			learningRate = 0.1;
		private double			radius = 15;

		private Thread workerThread = null;
        private volatile bool needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// Create network
			network = new DistanceNetwork( 3, 100 * 100 );

			// Create map bitmap
			mapBitmap = new Bitmap( 200, 200, PixelFormat.Format24bppRgb );

			//
			RandomizeNetwork( );
			UpdateSettings( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if ( components != null ) 
				{
					components.Dispose( );
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.randomizeButton = new System.Windows.Forms.Button();
			this.mapPanel = new BufferedPanel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.radiusBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.rateBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.randomizeButton,
																					this.mapPanel});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(222, 265);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Map";
			// 
			// randomizeButton
			// 
			this.randomizeButton.Location = new System.Drawing.Point(10, 230);
			this.randomizeButton.Name = "randomizeButton";
			this.randomizeButton.TabIndex = 1;
			this.randomizeButton.Text = "&Randomize";
			this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
			// 
			// mapPanel
			// 
			this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mapPanel.Location = new System.Drawing.Point(10, 20);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(202, 202);
			this.mapPanel.TabIndex = 0;
			this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.currentIterationBox,
																					this.label5,
																					this.stopButton,
																					this.startButton,
																					this.label4,
																					this.radiusBox,
																					this.label3,
																					this.rateBox,
																					this.label2,
																					this.iterationsBox,
																					this.label1});
			this.groupBox2.Location = new System.Drawing.Point(240, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(190, 265);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Neural Network";
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(110, 120);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.ReadOnly = true;
			this.currentIterationBox.Size = new System.Drawing.Size(70, 20);
			this.currentIterationBox.TabIndex = 10;
			this.currentIterationBox.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 122);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 9;
			this.label5.Text = "Curren iteration:";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(105, 230);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 8;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(20, 230);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 7;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(10, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(170, 2);
			this.label4.TabIndex = 6;
			// 
			// radiusBox
			// 
			this.radiusBox.Location = new System.Drawing.Point(110, 70);
			this.radiusBox.Name = "radiusBox";
			this.radiusBox.Size = new System.Drawing.Size(70, 20);
			this.radiusBox.TabIndex = 5;
			this.radiusBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Initial radius:";
			// 
			// rateBox
			// 
			this.rateBox.Location = new System.Drawing.Point(110, 45);
			this.rateBox.Name = "rateBox";
			this.rateBox.Size = new System.Drawing.Size(70, 20);
			this.rateBox.TabIndex = 3;
			this.rateBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Initial learning rate:";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(110, 20);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(70, 20);
			this.iterationsBox.TabIndex = 1;
			this.iterationsBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Iteraions:";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(439, 285);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Color Clustering using Kohonen SOM";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

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
			iterationsBox.Text	= iterations.ToString( );
			rateBox.Text		= learningRate.ToString( );
			radiusBox.Text		= radius.ToString( );
		}

		// On "Rundomize" button clicked
		private void randomizeButton_Click(object sender, System.EventArgs e)
		{
			RandomizeNetwork( );
		}

		// Radnomize weights of network
		private void RandomizeNetwork( )
		{
			Neuron.RandRange = new Range( 0, 255 );

			// randomize net
			network.Randomize( );

			// update map
			UpdateMap( );
		}

		// Update map from network weights
		private void UpdateMap( )
		{
			// lock
			Monitor.Enter( this );

			// lock bitmap
			BitmapData mapData = mapBitmap.LockBits( new Rectangle( 0, 0 , 200, 200 ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			int stride = mapData.Stride;
			int offset = stride - 200 * 3;
			Layer layer = network.Layers[0];

			unsafe
			{
				byte* ptr = (byte*) mapData.Scan0;

				// for all rows
				for ( int y = 0, i = 0; y < 100; y++ )
				{
					// for all pixels
					for ( int x = 0; x < 100; x++, i++, ptr += 6 )
					{
						Neuron neuron = layer.Neurons[i];

						// red
						ptr[2] = ptr[2 + 3] = ptr[2 + stride] = ptr[2 + 3 + stride]	=
							(byte) Math.Max( 0, Math.Min( 255, neuron.Weights[0] ) );
						// green
						ptr[1] = ptr[1 + 3] = ptr[1 + stride] = ptr[1 + 3 + stride]	=
                            (byte) Math.Max( 0, Math.Min( 255, neuron.Weights[1] ) );
						// blue
						ptr[0] = ptr[0 + 3] = ptr[0 + stride] = ptr[0 + 3 + stride]	=
                            (byte) Math.Max( 0, Math.Min( 255, neuron.Weights[2] ) );
					}

					ptr += offset;
					ptr += stride;
				}
			}

			// unlock image
			mapBitmap.UnlockBits( mapData );

			// unlock
			Monitor.Exit( this );

			// invalidate maps panel
			mapPanel.Invalidate( );
		}

		// Paint map
		private void mapPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			// lock
			Monitor.Enter( this );

			// drat image
			g.DrawImage( mapBitmap, 0, 0, 200, 200 );

			// unlock
			Monitor.Exit( this );
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
			    iterationsBox.Enabled	= enable;
			    rateBox.Enabled			= enable;
			    radiusBox.Enabled		= enable;

			    startButton.Enabled		= enable;
			    randomizeButton.Enabled	= enable;
			    stopButton.Enabled		= !enable;
            }
		}

		// On "Start" button click
		private void startButton_Click(object sender, System.EventArgs e)
		{
			// get iterations count
			try
			{
				iterations = Math.Max( 10, Math.Min( 1000000, int.Parse( iterationsBox.Text ) ) );
			}
			catch
			{
				iterations = 5000;
			}
			// get learning rate
			try
			{
				learningRate = Math.Max( 0.00001, Math.Min( 1.0, double.Parse( rateBox.Text ) ) );
			}
			catch
			{
				learningRate = 0.1;
			}
			// get radius
			try
			{
				radius = Math.Max( 5, Math.Min( 75, int.Parse( radiusBox.Text ) ) );
			}
			catch
			{
				radius = 15;
			}
			// update settings controls
			UpdateSettings( );

			// disable all settings controls except "Stop" button
			EnableControls( false );

			// run worker thread
			needToStop = false;
			workerThread = new Thread( new ThreadStart( SearchSolution ) );
			workerThread.Start( );
		}

		// On "Stop" button click
		private void stopButton_Click(object sender, System.EventArgs e)
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
			// create learning algorithm
			SOMLearning	trainer = new SOMLearning( network );

			// input
			double[] input = new double[3];

			double	fixedLearningRate = learningRate / 10;
			double	driftingLearningRate = fixedLearningRate * 9;

			// iterations
			int i = 0;

			// loop
			while ( !needToStop )
			{
				trainer.LearningRate = driftingLearningRate * ( iterations - i ) / iterations + fixedLearningRate;
				trainer.LearningRadius = (double) radius * ( iterations - i ) / iterations;

				input[0] = rand.Next( 256 );
				input[1] = rand.Next( 256 );
				input[2] = rand.Next( 256 );

				trainer.Run( input );

				// update map once per 50 iterations
				if ( ( i % 10 ) == 9 )
				{
					UpdateMap( );
				}

				// increase current iteration
				i++;

				// set current iteration's info
                SetText( currentIterationBox, i.ToString( ) );

				// stop ?
				if ( i >= iterations )
					break;
			}

			// enable settings controls
			EnableControls( true );
		}
	}
}
