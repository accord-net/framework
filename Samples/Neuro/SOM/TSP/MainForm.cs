// Traveling Salesman Problem using Elastic Net
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

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace TSP
{
	/// <summary>
    /// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button generateMapButton;
		private System.Windows.Forms.TextBox citiesCountBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox neuronsBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox rateBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox radiusBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int citiesCount		= 10;
		private int neurons			= 20;
		private int	iterations		= 500;
		private double learningRate	= 0.5;
		private double learningRadius = 0.5;

		private double[,]	map = null;
		private Random		rand = new Random();

		private Thread workerThread = null;
        private volatile bool needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// initialize chart
			chart.AddDataSeries( "cities", Color.Red, Chart.SeriesType.Dots, 5, false );
			chart.AddDataSeries( "path", Color.Blue, Chart.SeriesType.Line, 1, false );
			chart.RangeX = new Range( 0, 1000 );
			chart.RangeY = new Range( 0, 1000 );

			//
			UpdateSettings( );
			GenerateMap( );
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
			this.generateMapButton = new System.Windows.Forms.Button();
			this.citiesCountBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.rateBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.neuronsBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.radiusBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			this.chart = new AForge.Controls.Chart( );
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.generateMapButton);
			this.groupBox1.Controls.Add(this.citiesCountBox);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.chart);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(300, 340);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Map";
			// 
			// generateMapButton
			// 
			this.generateMapButton.Location = new System.Drawing.Point(110, 309);
			this.generateMapButton.Name = "generateMapButton";
			this.generateMapButton.Size = new System.Drawing.Size(75, 22);
			this.generateMapButton.TabIndex = 3;
			this.generateMapButton.Text = "&Generate";
			this.generateMapButton.Click += new System.EventHandler(this.generateMapButton_Click);
			// 
			// citiesCountBox
			// 
			this.citiesCountBox.Location = new System.Drawing.Point(50, 310);
			this.citiesCountBox.Name = "citiesCountBox";
			this.citiesCountBox.Size = new System.Drawing.Size(50, 20);
			this.citiesCountBox.TabIndex = 2;
			this.citiesCountBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 312);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Cities:";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(280, 280);
			this.chart.TabIndex = 4;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radiusBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.stopButton);
			this.groupBox2.Controls.Add(this.startButton);
			this.groupBox2.Controls.Add(this.currentIterationBox);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.rateBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.iterationsBox);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.neuronsBox);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(320, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(180, 340);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Neural Network";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(95, 305);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 23;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(10, 305);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 22;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(110, 150);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.ReadOnly = true;
			this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
			this.currentIterationBox.TabIndex = 21;
			this.currentIterationBox.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(10, 152);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 16);
			this.label8.TabIndex = 20;
			this.label8.Text = "Curren iteration:";
			// 
			// label7
			// 
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.Location = new System.Drawing.Point(10, 139);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(160, 2);
			this.label7.TabIndex = 19;
			// 
			// rateBox
			// 
			this.rateBox.Location = new System.Drawing.Point(110, 85);
			this.rateBox.Name = "rateBox";
			this.rateBox.Size = new System.Drawing.Size(60, 20);
			this.rateBox.TabIndex = 18;
			this.rateBox.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 87);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 17;
			this.label5.Text = "Initial learning rate:";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(110, 60);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(60, 20);
			this.iterationsBox.TabIndex = 16;
			this.iterationsBox.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 62);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 16);
			this.label6.TabIndex = 15;
			this.label6.Text = "Iteraions:";
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(10, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(160, 2);
			this.label3.TabIndex = 4;
			// 
			// neuronsBox
			// 
			this.neuronsBox.Location = new System.Drawing.Point(110, 20);
			this.neuronsBox.Name = "neuronsBox";
			this.neuronsBox.Size = new System.Drawing.Size(60, 20);
			this.neuronsBox.TabIndex = 1;
			this.neuronsBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Neurons:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 24;
			this.label4.Text = "Learning radius:";
			// 
			// radiusBox
			// 
			this.radiusBox.Location = new System.Drawing.Point(110, 110);
			this.radiusBox.Name = "radiusBox";
			this.radiusBox.Size = new System.Drawing.Size(60, 20);
			this.radiusBox.TabIndex = 25;
			this.radiusBox.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(509, 360);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Traveling Salesman Problem using Elastic Net";
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
			citiesCountBox.Text	= citiesCount.ToString( );
			neuronsBox.Text		= neurons.ToString( );
			iterationsBox.Text	= iterations.ToString( );
			rateBox.Text		= learningRate.ToString( );
			radiusBox.Text		= learningRadius.ToString( );
		}

		// Generate new map for the Traivaling Salesman problem
		private void GenerateMap( )
		{
			Random rand = new Random( (int) DateTime.Now.Ticks );

			// create coordinates array
			map = new double[citiesCount, 2];

			for ( int i = 0; i < citiesCount; i++ )
			{
				map[i, 0] = rand.Next( 1001 );
				map[i, 1] = rand.Next( 1001 );
			}

			// set the map
			chart.UpdateDataSeries( "cities", map );
			// erase path if it is
			chart.UpdateDataSeries( "path", null );
		}

		// On "Generate" button click - generate map
		private void generateMapButton_Click(object sender, System.EventArgs e)
		{
			// get cities count
			try
			{
				citiesCount = Math.Max( 5, Math.Min( 50, int.Parse( citiesCountBox.Text ) ) );
			}
			catch
			{
				citiesCount = 20;
			}
			citiesCountBox.Text = citiesCount.ToString( );

			// regenerate map
			GenerateMap( );
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
			    neuronsBox.Enabled		= enable;
			    iterationsBox.Enabled	= enable;
			    rateBox.Enabled			= enable;
			    radiusBox.Enabled		= enable;

			    startButton.Enabled			= enable;
			    generateMapButton.Enabled	= enable;
			    stopButton.Enabled			= !enable;
		    }
        }

		// On "Start" button click
		private void startButton_Click(object sender, System.EventArgs e)
		{
			// get network size
			try
			{
				neurons = Math.Max( 5, Math.Min( 50, int.Parse( neuronsBox.Text ) ) );
			}
			catch
			{
				neurons = 20;
			}
			// get iterations count
			try
			{
				iterations = Math.Max( 10, Math.Min( 1000000, int.Parse( iterationsBox.Text ) ) );
			}
			catch
			{
				iterations = 500;
			}
			// get learning rate
			try
			{
				learningRate = Math.Max( 0.00001, Math.Min( 1.0, double.Parse( rateBox.Text ) ) );
			}
			catch
			{
				learningRate = 0.5;
			}
			// get learning radius
			try
			{
				learningRadius = Math.Max( 0.00001, Math.Min( 1.0, double.Parse( radiusBox.Text ) ) );
			}
			catch
			{
				learningRadius = 0.5;
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
			// set random generators range
			Neuron.RandRange = new Range( 0, 1000 );

			// create network
			DistanceNetwork network = new DistanceNetwork( 2, neurons );

			// create learning algorithm
			ElasticNetworkLearning	trainer = new ElasticNetworkLearning( network );

			double	fixedLearningRate = learningRate / 20;
			double	driftingLearningRate = fixedLearningRate * 19;

			// path
			double[,] path = new double[neurons + 1, 2];

			// input
			double[] input = new double[2];

			// iterations
			int i = 0;

			// loop
			while ( !needToStop )
			{
				// update learning speed & radius
				trainer.LearningRate = driftingLearningRate * ( iterations - i ) / iterations + fixedLearningRate;
				trainer.LearningRadius = learningRadius * ( iterations - i ) / iterations;

				// set network input
				int currentCity = rand.Next( citiesCount );
				input[0] = map[currentCity, 0];
				input[1] = map[currentCity, 1];

				// run one training iteration
				trainer.Run( input );

				// show current path
				for ( int j = 0; j < neurons; j++ )
				{
					path[j, 0] = network.Layers[0].Neurons[j].Weights[0];
					path[j, 1] = network.Layers[0].Neurons[j].Weights[1];
				}
				path[neurons, 0] = network.Layers[0].Neurons[0].Weights[0];
				path[neurons, 1] = network.Layers[0].Neurons[0].Weights[1];

				chart.UpdateDataSeries( "path", path );

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