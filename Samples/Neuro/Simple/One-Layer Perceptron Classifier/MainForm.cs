// One-Layer Perceptron Classifier
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
using System.IO;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace Classifier
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.CheckBox saveFilesCheck;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView weightsList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.GroupBox groupBox3;
		private AForge.Controls.Chart errorChart;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			samples = 0;
		private double[,]	data = null;
		private int[]		classes = null;
		private int			classesCount = 0;
		private int[]		samplesPerClass = null;

		private double		learningRate = 0.1;
		private bool		saveStatisticsToFiles = false;

		private Thread workerThread = null;
        private volatile bool needToStop = false;

		// color for data series
		private static Color[]	dataSereisColors = new Color[10] {
																	 Color.Red,		Color.Blue,
																	 Color.Green,	Color.DarkOrange,
																	 Color.Violet,	Color.Brown,
																	 Color.Black,	Color.Pink,
																	 Color.Olive,	Color.Navy };

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// update some controls
			saveFilesCheck.Checked = saveStatisticsToFiles;
			UpdateSettings( );

			// initialize charts
			errorChart.AddDataSeries( "error", Color.Red, Chart.SeriesType.ConnectedDots, 3 );
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
			this.loadButton = new System.Windows.Forms.Button();
			this.chart = new AForge.Controls.Chart();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.weightsList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.saveFilesCheck = new System.Windows.Forms.CheckBox();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.errorChart = new AForge.Controls.Chart();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.loadButton,
																					this.chart});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 255);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 225);
			this.loadButton.Name = "loadButton";
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(200, 200);
			this.chart.TabIndex = 0;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.weightsList,
																					this.label4,
																					this.label3,
																					this.saveFilesCheck,
																					this.stopButton,
																					this.startButton,
																					this.iterationsBox,
																					this.label2,
																					this.learningRateBox,
																					this.label1});
			this.groupBox2.Location = new System.Drawing.Point(240, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 410);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Training";
			// 
			// weightsList
			// 
			this.weightsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.columnHeader1,
																						  this.columnHeader2,
																						  this.columnHeader3});
			this.weightsList.FullRowSelect = true;
			this.weightsList.GridLines = true;
			this.weightsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.weightsList.Location = new System.Drawing.Point(10, 130);
			this.weightsList.Name = "weightsList";
			this.weightsList.Size = new System.Drawing.Size(220, 270);
			this.weightsList.TabIndex = 14;
			this.weightsList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Neuron";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Weigh";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Value";
			this.columnHeader3.Width = 65;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 110);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(55, 16);
			this.label4.TabIndex = 13;
			this.label4.Text = "Weights:";
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(10, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(220, 2);
			this.label3.TabIndex = 12;
			// 
			// saveFilesCheck
			// 
			this.saveFilesCheck.Location = new System.Drawing.Point(10, 80);
			this.saveFilesCheck.Name = "saveFilesCheck";
			this.saveFilesCheck.Size = new System.Drawing.Size(150, 16);
			this.saveFilesCheck.TabIndex = 11;
			this.saveFilesCheck.Text = "Save weights and errors to files";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(155, 49);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 10;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(155, 19);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 9;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(90, 50);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.ReadOnly = true;
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 3;
			this.iterationsBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Iterations:";
			// 
			// learningRateBox
			// 
			this.learningRateBox.Location = new System.Drawing.Point(90, 20);
			this.learningRateBox.Name = "learningRateBox";
			this.learningRateBox.Size = new System.Drawing.Size(50, 20);
			this.learningRateBox.TabIndex = 1;
			this.learningRateBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Learning rate:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.errorChart});
			this.groupBox3.Location = new System.Drawing.Point(10, 270);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(220, 150);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Error\'s dynamics";
			// 
			// errorChart
			// 
			this.errorChart.Location = new System.Drawing.Point(10, 20);
			this.errorChart.Name = "errorChart";
			this.errorChart.Size = new System.Drawing.Size(200, 120);
			this.errorChart.TabIndex = 0;
			this.errorChart.Text = "chart1";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(489, 430);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "One-Layer Perceptron Classifier";
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
        private delegate void ClearListCallback( System.Windows.Forms.ListView control );
        private delegate ListViewItem AddListItemCallback( System.Windows.Forms.ListView control, string itemText );
        private delegate void AddListSubitemCallback( ListViewItem item, string subItemText );

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

        // Thread safe clearing of list view
        private void ClearList( System.Windows.Forms.ListView control )
        {
            if ( control.InvokeRequired )
            {
                ClearListCallback d = new ClearListCallback( ClearList );
                Invoke( d, new object[] { control } );
            }
            else
            {
                control.Items.Clear( );
            }
        }

        // Thread safe adding of item to list control
        private ListViewItem AddListItem( System.Windows.Forms.ListView control, string itemText )
        {
            ListViewItem item = null;

            if ( control.InvokeRequired )
            {
                AddListItemCallback d = new AddListItemCallback( AddListItem );
                item = (ListViewItem) Invoke( d, new object[] { control, itemText } );
            }
            else
            {
                item = control.Items.Add( itemText );
            }

            return item;
        }

        // Thread safe adding of subitem to list control
        private void AddListSubitem( ListViewItem item, string subItemText )
        {
            if ( this.InvokeRequired )
            {
                AddListSubitemCallback d = new AddListSubitemCallback( AddListSubitem );
                Invoke( d, new object[] { item, subItemText } );
            }
            else
            {
                item.SubItems.Add( subItemText );
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

		// Load input data
		private void loadButton_Click(object sender, System.EventArgs e)
		{
			// data file format:
			// X1, X2, class

			// load maximum 10 classes !

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 200 samples only)
                float[,]	tempData    = new float[200, 2];
				int[]		tempClasses = new int[200];

				// min and max X values
                float minX = float.MaxValue;
                float maxX = float.MinValue;

				// samples count
				samples = 0;
				// classes count
				classesCount = 0;
				samplesPerClass = new int[10];

				try
				{
					string	str = null;

					// open selected file
					reader = File.OpenText( openFileDialog.FileName );

					// read the data
					while ( ( samples < 200 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// split the string
						string[] strs = str.Split( ';' );
						if ( strs.Length == 1 )
							strs = str.Split( ',' );

						// check tokens count
						if ( strs.Length != 3 )
							throw new ApplicationException( "Invalid file format" );

						// parse tokens
                        tempData[samples, 0] = float.Parse( strs[0] );
                        tempData[samples, 1] = float.Parse( strs[1] );
						tempClasses[samples] = int.Parse( strs[2] );

						// skip classes over 10, except only first 10 classes
						if ( tempClasses[samples] >= 10 )
							continue;

						// count the amount of different classes
						if ( tempClasses[samples] >= classesCount )
							classesCount = tempClasses[samples] + 1;
						// count samples per class
						samplesPerClass[tempClasses[samples]]++;

						// search for min value
						if ( tempData[samples, 0] < minX )
							minX = tempData[samples, 0];
						// search for max value
						if ( tempData[samples, 0] > maxX )
							maxX = tempData[samples, 0];

						samples++;
					}

					// allocate and set data
					data = new double[samples, 2];
					Array.Copy( tempData, 0, data, 0, samples * 2 );
					classes = new int[samples];
					Array.Copy( tempClasses, 0, classes, 0, samples );

					// clear current result
					weightsList.Items.Clear( );
					errorChart.UpdateDataSeries( "error", null );
				}
				catch ( Exception )
				{
					MessageBox.Show( "Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				finally
				{
					// close file
					if ( reader != null )
						reader.Close( );
				}

				// update chart
				chart.RangeX = new Range( minX, maxX );
				ShowTrainingData( );

				// enable start button
				startButton.Enabled = true;
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			learningRateBox.Text = learningRate.ToString( );
		}

		// Show training data on chart
		private void ShowTrainingData( )
		{
			double[][,]	dataSeries = new double[classesCount][,];
			int[]		indexes = new int[classesCount];

			// allocate data arrays
			for ( int i = 0; i < classesCount; i++ )
			{
				dataSeries[i] = new double[samplesPerClass[i], 2];
			}

			// fill data arrays
			for ( int i = 0; i < samples; i++ )
			{
				// get sample's class
				int dataClass = classes[i];
				// copy data into appropriate array
				dataSeries[dataClass][indexes[dataClass], 0] = data[i, 0];
				dataSeries[dataClass][indexes[dataClass], 1] = data[i, 1];
				indexes[dataClass]++;
			}

			// remove all previous data series from chart control
			chart.RemoveAllDataSeries( );

			// add new data series
			for ( int i = 0; i < classesCount; i++ )
			{
				string className = string.Format( "class" + i );

				// add data series
				chart.AddDataSeries( className, dataSereisColors[i], Chart.SeriesType.Dots, 5 );
				chart.UpdateDataSeries( className, dataSeries[i] );
				// add classifier
				chart.AddDataSeries( string.Format( "classifier" + i ), Color.Gray, Chart.SeriesType.Line, 1, false );
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
			    learningRateBox.Enabled	= enable;
			    loadButton.Enabled		= enable;
			    startButton.Enabled		= enable;
			    saveFilesCheck.Enabled	= enable;
    			stopButton.Enabled		= !enable;
	    	}
        }

		// On "Start" button click
		private void startButton_Click(object sender, System.EventArgs e)
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
			// prepare learning data
			double[][] input = new double[samples][];
			double[][] output = new double[samples][];

			for ( int i = 0; i < samples; i++ )
			{
				input[i] = new double[2];
				output[i] = new double[classesCount];

				// set input
				input[i][0] = data[i, 0];
				input[i][1] = data[i, 1];
				// set output
				output[i][classes[i]] = 1;
			}

			// create perceptron
			ActivationNetwork	network = new ActivationNetwork( new ThresholdFunction( ), 2, classesCount );
			ActivationLayer		layer = network.Layers[0] as ActivationLayer;
			// create teacher
			PerceptronLearning teacher = new PerceptronLearning( network );
			// set learning rate
			teacher.LearningRate = learningRate;

			// iterations
			int iteration = 1;

			// statistic files
			StreamWriter errorsFile = null;
			StreamWriter weightsFile = null;

			try
			{
				// check if we need to save statistics to files
				if ( saveStatisticsToFiles )
				{
					// open files
					errorsFile	= File.CreateText( "errors.csv" );
					weightsFile	= File.CreateText( "weights.csv" );
				}

				// erros list
				ArrayList errorsList = new ArrayList( );

				// loop
				while ( !needToStop )
				{
					// save current weights
					if ( weightsFile != null )
					{
						for ( int i = 0; i < classesCount; i++ )
						{
							weightsFile.Write( "neuron" + i + "," );
							weightsFile.Write( layer.Neurons[i].Weights[0] + "," );
							weightsFile.Write( layer.Neurons[i].Weights[1] + "," );
							weightsFile.WriteLine( ( (ActivationNeuron) layer.Neurons[i] ).Threshold );
						}
					}

					// run epoch of learning procedure
					double error = teacher.RunEpoch( input, output );
					errorsList.Add( error );

					// save current error
					if ( errorsFile != null )
					{
						errorsFile.WriteLine( error );
					}				

					// show current iteration
                    SetText( iterationsBox, iteration.ToString( ) );

					// stop if no error
					if ( error == 0 )
						break;

					// show classifiers
					for ( int j = 0; j < classesCount; j++ )
					{
						double k = ( layer.Neurons[j].Weights[1] != 0 ) ? ( - layer.Neurons[j].Weights[0] / layer.Neurons[j].Weights[1] ) : 0;
						double b = ( layer.Neurons[j].Weights[1] != 0 ) ? ( - ( ( ActivationNeuron) layer.Neurons[j] ).Threshold / layer.Neurons[j].Weights[1] ) : 0;

						double[,] classifier = new double[2, 2] {
							{ chart.RangeX.Min, chart.RangeX.Min * k + b },
							{ chart.RangeX.Max, chart.RangeX.Max * k + b }
																};

						// update chart
						chart.UpdateDataSeries( string.Format( "classifier" + j ), classifier );
					}

					iteration++;
				}

				// show perceptron's weights
                ClearList( weightsList );
				for ( int i = 0; i < classesCount; i++ )
				{
					string neuronName = string.Format( "Neuron {0}", i + 1 );

					// weight 0
                    ListViewItem item = AddListItem( weightsList, neuronName );
                    AddListSubitem( item, "Weight 1" );
                    AddListSubitem( item, layer.Neurons[i].Weights[0].ToString( "F6" ) );
					// weight 1
                    item = AddListItem( weightsList, neuronName );
                    AddListSubitem( item, "Weight 2" );
                    AddListSubitem( item, layer.Neurons[i].Weights[1].ToString( "F6" ) );
					// threshold
                    item = AddListItem( weightsList, neuronName );
                    AddListSubitem( item, "Threshold" );
                    AddListSubitem( item, ( (ActivationNeuron) layer.Neurons[i] ).Threshold.ToString( "F6" ) );
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
				if ( weightsFile != null )
					weightsFile.Close( );
			}

			// enable settings controls
			EnableControls( true );
		}
	}
}
