// 1D Optimization using Genetic Algorithms
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
using AForge.Genetic;
using AForge.Controls;

namespace Optimization1D
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox minXBox;
		private System.Windows.Forms.TextBox maxXBox;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox populationSizeBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox chromosomeLengthBox;
		private System.Windows.Forms.CheckBox onlyBestCheck;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox selectionBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox modeBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox currentValueBox;

		private UserFunction userFunction = new UserFunction( );
		private int populationSize = 40;
		private int chromosomeLength = 32;
		private int iterations = 100;
		private int selectionMethod = 0;
		private int optimizationMode = 0;
		private bool showOnlyBest = false;

		private Thread workerThread = null;
		private volatile bool needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// add data series to chart
			chart.AddDataSeries( "function", Color.Red, Chart.SeriesType.Line, 1 );
			chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Dots, 5 );
			UpdateChart( );

			// update controls
			minXBox.Text = userFunction.Range.Min.ToString( );
			maxXBox.Text = userFunction.Range.Max.ToString( );
			selectionBox.SelectedIndex = selectionMethod;
			modeBox.SelectedIndex = optimizationMode;
			UpdateSettings( );
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
			this.chart = new AForge.Controls.Chart();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.minXBox = new System.Windows.Forms.TextBox();
			this.maxXBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.modeBox = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.selectionBox = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.onlyBestCheck = new System.Windows.Forms.CheckBox();
			this.chromosomeLengthBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.populationSizeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.currentValueBox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(280, 270);
			this.chart.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chart);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.minXBox);
			this.groupBox1.Controls.Add(this.maxXBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(300, 330);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Function";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 297);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Range:";
			// 
			// minXBox
			// 
			this.minXBox.Location = new System.Drawing.Point(60, 295);
			this.minXBox.Name = "minXBox";
			this.minXBox.Size = new System.Drawing.Size(50, 20);
			this.minXBox.TabIndex = 3;
			this.minXBox.Text = "";
			this.minXBox.TextChanged += new System.EventHandler(this.minXBox_TextChanged);
			// 
			// maxXBox
			// 
			this.maxXBox.Location = new System.Drawing.Point(130, 295);
			this.maxXBox.Name = "maxXBox";
			this.maxXBox.Size = new System.Drawing.Size(50, 20);
			this.maxXBox.TabIndex = 4;
			this.maxXBox.Text = "";
			this.maxXBox.TextChanged += new System.EventHandler(this.maxXBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(115, 297);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "-";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.modeBox);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.selectionBox);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.iterationsBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.onlyBestCheck);
			this.groupBox2.Controls.Add(this.chromosomeLengthBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.populationSizeBox);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(320, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(185, 222);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Settings";
			// 
			// modeBox
			// 
			this.modeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.modeBox.Items.AddRange(new object[] {
														 "Maximize",
														 "Minimize"});
			this.modeBox.Location = new System.Drawing.Point(110, 95);
			this.modeBox.Name = "modeBox";
			this.modeBox.Size = new System.Drawing.Size(65, 21);
			this.modeBox.TabIndex = 7;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(10, 97);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(110, 17);
			this.label8.TabIndex = 6;
			this.label8.Text = "Optimization mode:";
			// 
			// selectionBox
			// 
			this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.selectionBox.Items.AddRange(new object[] {
															  "Elite",
															  "Rank",
															  "Roulette"});
			this.selectionBox.Location = new System.Drawing.Point(110, 70);
			this.selectionBox.Name = "selectionBox";
			this.selectionBox.Size = new System.Drawing.Size(65, 21);
			this.selectionBox.TabIndex = 5;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(10, 72);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 16);
			this.label7.TabIndex = 4;
			this.label7.Text = "Selection method:";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(125, 175);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 16);
			this.label6.TabIndex = 10;
			this.label6.Text = "( 0 - inifinity )";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 155);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 9;
			this.iterationsBox.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 157);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 16);
			this.label5.TabIndex = 8;
			this.label5.Text = "Iterations:";
			// 
			// onlyBestCheck
			// 
			this.onlyBestCheck.Location = new System.Drawing.Point(10, 195);
			this.onlyBestCheck.Name = "onlyBestCheck";
			this.onlyBestCheck.Size = new System.Drawing.Size(144, 16);
			this.onlyBestCheck.TabIndex = 11;
			this.onlyBestCheck.Text = "Show only best solution";
			// 
			// chromosomeLengthBox
			// 
			this.chromosomeLengthBox.Location = new System.Drawing.Point(125, 45);
			this.chromosomeLengthBox.Name = "chromosomeLengthBox";
			this.chromosomeLengthBox.Size = new System.Drawing.Size(50, 20);
			this.chromosomeLengthBox.TabIndex = 3;
			this.chromosomeLengthBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(110, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "Chromosome length:";
			// 
			// populationSizeBox
			// 
			this.populationSizeBox.Location = new System.Drawing.Point(125, 20);
			this.populationSizeBox.Name = "populationSizeBox";
			this.populationSizeBox.Size = new System.Drawing.Size(50, 20);
			this.populationSizeBox.TabIndex = 1;
			this.populationSizeBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Population size:";
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(340, 317);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 3;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(430, 317);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.currentValueBox);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.currentIterationBox);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Location = new System.Drawing.Point(320, 235);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(185, 75);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Current iteration";
			// 
			// currentValueBox
			// 
			this.currentValueBox.Location = new System.Drawing.Point(125, 45);
			this.currentValueBox.Name = "currentValueBox";
			this.currentValueBox.ReadOnly = true;
			this.currentValueBox.Size = new System.Drawing.Size(50, 20);
			this.currentValueBox.TabIndex = 3;
			this.currentValueBox.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(10, 47);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(60, 15);
			this.label10.TabIndex = 2;
			this.label10.Text = "Value:";
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(125, 20);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.ReadOnly = true;
			this.currentIterationBox.Size = new System.Drawing.Size(50, 20);
			this.currentIterationBox.TabIndex = 1;
			this.currentIterationBox.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 22);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(60, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Iteration:";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(514, 350);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "1D Optimization using Genetic Algorithms";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
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
		private void MainForm_Closing( object sender, System.ComponentModel.CancelEventArgs e )
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
			populationSizeBox.Text		= populationSize.ToString( );
			chromosomeLengthBox.Text	= chromosomeLength.ToString( );
			iterationsBox.Text			= iterations.ToString( );
		}
		
		// Update chart
		private void UpdateChart( )
		{
			// update chart range
			chart.RangeX = userFunction.Range;

			double[,] data = null;

			if ( chart.RangeX.Length > 0 )
			{
				// prepare data
				data = new double[501, 2];

				double minX = userFunction.Range.Min;
				double length = userFunction.Range.Length;

				for ( int i = 0; i <= 500; i++ )
				{
					data[i, 0] = minX + length * i / 500;
					data[i, 1] = userFunction.OptimizationFunction( data[i, 0] );
				}
			}

			// update chart series
			chart.UpdateDataSeries( "function", data );
		}

		// Update min value
		private void minXBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				userFunction.Range = new Range( float.Parse( minXBox.Text ), userFunction.Range.Max );
				UpdateChart( );
			}
			catch
			{
			}
		}

		// Update max value
		private void maxXBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				userFunction.Range = new Range( userFunction.Range.Min, float.Parse( maxXBox.Text ) );
				UpdateChart( );
			}
			catch
			{
			}
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
                minXBox.Enabled = enable;
                maxXBox.Enabled = enable;

                populationSizeBox.Enabled = enable;
                chromosomeLengthBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;
                modeBox.Enabled = enable;
                onlyBestCheck.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
		}

		// On "Start" button click
		private void startButton_Click(object sender, System.EventArgs e)
		{
			// get population size
			try
			{
				populationSize = Math.Max( 10, Math.Min( 100, int.Parse( populationSizeBox.Text ) ) );
			}
			catch
			{
				populationSize = 40;
			}
			// get chromosome length
			try
			{
				chromosomeLength = Math.Max( 8, Math.Min( 64, int.Parse( chromosomeLengthBox.Text ) ) );
			}
			catch
			{
				chromosomeLength = 32;
			}
			// iterations
			try
			{
				iterations = Math.Max( 0, int.Parse( iterationsBox.Text ) );
			}
			catch
			{
				iterations = 100;
			}
			// update settings controls
			UpdateSettings( );

			selectionMethod = selectionBox.SelectedIndex;
			optimizationMode = modeBox.SelectedIndex;
			showOnlyBest = onlyBestCheck.Checked;

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
			// create population
			Population population = new Population( populationSize,
				new BinaryChromosome( chromosomeLength ),
				userFunction,
				( selectionMethod == 0 ) ? (ISelectionMethod) new EliteSelection( ) :
				( selectionMethod == 1 ) ? (ISelectionMethod) new RankSelection( ) :
										   (ISelectionMethod) new RouletteWheelSelection( )
				);
			// set optimization mode
			userFunction.Mode = ( optimizationMode == 0 ) ?
				OptimizationFunction1D.Modes.Maximization :
				OptimizationFunction1D.Modes.Minimization;
			// iterations
			int i = 1;
			// solution
			double[,] data = new double[(showOnlyBest) ? 1 : populationSize, 2];


			// loop
			while ( !needToStop )
			{
				// run one epoch of genetic algorithm
				population.RunEpoch( );

				// show current solution
				if ( showOnlyBest )
				{
					data[0, 0] = userFunction.Translate( population.BestChromosome );
					data[0, 1] = userFunction.OptimizationFunction( data[0, 0] );
				}
				else
				{
					for ( int j = 0; j < populationSize; j++ )
					{
						data[j, 0] = userFunction.Translate( population[j] );
						data[j, 1] = userFunction.OptimizationFunction( data[j, 0] );
					}
				}
				chart.UpdateDataSeries( "solution", data );

				// set current iteration's info
                SetText( currentIterationBox, i.ToString( ) );
                SetText( currentValueBox, userFunction.Translate( population.BestChromosome ).ToString( "F3" ) );

				// increase current iteration
				i++;

				//
				if ( ( iterations != 0 ) && ( i > iterations ) )
					break;
			}

			// enable settings controls
			EnableControls( true );
		}
	}

	// Function to optimize
	public class UserFunction : OptimizationFunction1D
	{
		public UserFunction( ) : base( new Range( 0, 255 ) ) { }

		public override double OptimizationFunction( double x )
		{
			return Math.Cos( x / 23 ) * Math.Sin( x / 50 ) + 2;
		}
	}
}
