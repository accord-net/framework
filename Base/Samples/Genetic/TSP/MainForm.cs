// Traveling Salesman Problem using Genetic Algorithms
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

namespace TSP
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private AForge.Controls.Chart mapControl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox citiesCountBox;
		private System.Windows.Forms.Button generateMapButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox populationSizeBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox selectionBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox pathLengthBox;
		private System.Windows.Forms.CheckBox greedyCrossoverBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int citiesCount = 20;
		private int populationSize = 40;
		private int iterations = 100;
		private int selectionMethod = 0;
		private bool greedyCrossover = true;

		private double[,]	map = null;

		private Thread workerThread = null;
		private volatile bool needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// set up map control
			mapControl.RangeX = new Range( 0, 1000 );
			mapControl.RangeY = new Range( 0, 1000 );
			mapControl.AddDataSeries( "map", Color.Red, Chart.SeriesType.Dots, 5, false );
			mapControl.AddDataSeries( "path", Color.Blue, Chart.SeriesType.Line, 1, false );

			//
			selectionBox.SelectedIndex = selectionMethod;
			greedyCrossoverBox.Checked = greedyCrossover;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.generateMapButton = new System.Windows.Forms.Button();
			this.citiesCountBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.mapControl = new AForge.Controls.Chart();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.greedyCrossoverBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.selectionBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.populationSizeBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.pathLengthBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.generateMapButton);
			this.groupBox1.Controls.Add(this.citiesCountBox);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.mapControl);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(300, 340);
			this.groupBox1.TabIndex = 0;
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
			// mapControl
			// 
			this.mapControl.Location = new System.Drawing.Point(10, 20);
			this.mapControl.Name = "mapControl";
			this.mapControl.Size = new System.Drawing.Size(280, 280);
			this.mapControl.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.greedyCrossoverBox);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.iterationsBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.selectionBox);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.populationSizeBox);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(320, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(185, 225);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Settings";
			// 
			// greedyCrossoverBox
			// 
			this.greedyCrossoverBox.Location = new System.Drawing.Point(10, 70);
			this.greedyCrossoverBox.Name = "greedyCrossoverBox";
			this.greedyCrossoverBox.Size = new System.Drawing.Size(120, 24);
			this.greedyCrossoverBox.TabIndex = 7;
			this.greedyCrossoverBox.Text = "Greedy crossover";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(125, 200);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 15);
			this.label5.TabIndex = 6;
			this.label5.Text = "( 0 - inifinity )";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 180);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 5;
			this.iterationsBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 182);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 16);
			this.label4.TabIndex = 4;
			this.label4.Text = "Iterations:";
			// 
			// selectionBox
			// 
			this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.selectionBox.Items.AddRange(new object[] {
															  "Elite",
															  "Rank",
															  "Roulette"});
			this.selectionBox.Location = new System.Drawing.Point(110, 45);
			this.selectionBox.Name = "selectionBox";
			this.selectionBox.Size = new System.Drawing.Size(65, 21);
			this.selectionBox.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Selection method:";
			// 
			// populationSizeBox
			// 
			this.populationSizeBox.Location = new System.Drawing.Point(125, 20);
			this.populationSizeBox.Name = "populationSizeBox";
			this.populationSizeBox.Size = new System.Drawing.Size(50, 20);
			this.populationSizeBox.TabIndex = 1;
			this.populationSizeBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Population size:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.pathLengthBox);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.currentIterationBox);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Location = new System.Drawing.Point(320, 240);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(185, 75);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Current iteration";
			// 
			// pathLengthBox
			// 
			this.pathLengthBox.Location = new System.Drawing.Point(125, 45);
			this.pathLengthBox.Name = "pathLengthBox";
			this.pathLengthBox.ReadOnly = true;
			this.pathLengthBox.Size = new System.Drawing.Size(50, 20);
			this.pathLengthBox.TabIndex = 3;
			this.pathLengthBox.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(10, 47);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 16);
			this.label7.TabIndex = 2;
			this.label7.Text = "Path length:";
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
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 22);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(50, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "Iteration:";
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(340, 325);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 3;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(430, 325);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(514, 360);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Traveling Salesman Problem using Genetic Algorithms";
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
			citiesCountBox.Text		= citiesCount.ToString( );
			populationSizeBox.Text	= populationSize.ToString( );
			iterationsBox.Text		= iterations.ToString( );
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
                citiesCountBox.Enabled      = enable;
                populationSizeBox.Enabled   = enable;
                iterationsBox.Enabled       = enable;
                selectionBox.Enabled        = enable;

                generateMapButton.Enabled   = enable;

                startButton.Enabled = enable;
                stopButton.Enabled  = !enable;
            }
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
			mapControl.UpdateDataSeries( "map", map );
			// erase path if it is
			mapControl.UpdateDataSeries( "path", null );
		}

		// On "Generate" button click - generate map
		private void generateMapButton_Click( object sender, System.EventArgs e )
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
			greedyCrossover = greedyCrossoverBox.Checked;

			// disable all settings controls except "Stop" button
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
            if ( workerThread != null )
            {
                needToStop = true;
                while ( !workerThread.Join( 100 ) )
                    Application.DoEvents( );
                workerThread = null;
            }
		}

		// Worker thread
		void SearchSolution( )
		{
			// create fitness function
			TSPFitnessFunction fitnessFunction = new TSPFitnessFunction( map );
			// create population
			Population population = new Population( populationSize,
				( greedyCrossover ) ? new TSPChromosome( map ) : new PermutationChromosome( citiesCount ),
				fitnessFunction,
				( selectionMethod == 0 ) ? (ISelectionMethod) new EliteSelection( ) :
				( selectionMethod == 1 ) ? (ISelectionMethod) new RankSelection( ) :
				(ISelectionMethod) new RouletteWheelSelection( )
				);
			// iterations
			int i = 1;

			// path
			double[,] path = new double[citiesCount + 1, 2];

			// loop
			while ( !needToStop )
			{
				// run one epoch of genetic algorithm
				population.RunEpoch( );

				// display current path
				ushort[] bestValue = ((PermutationChromosome) population.BestChromosome).Value;

				for ( int j = 0; j < citiesCount; j++ )
				{
					path[j, 0] = map[bestValue[j], 0];
					path[j, 1] = map[bestValue[j], 1];
				}
				path[citiesCount, 0] = map[bestValue[0], 0];
				path[citiesCount, 1] = map[bestValue[0], 1];

				mapControl.UpdateDataSeries( "path", path );

				// set current iteration's info
                SetText( currentIterationBox, i.ToString( ) );
                SetText( pathLengthBox, fitnessFunction.PathLength( population.BestChromosome ).ToString( ) );

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
}
