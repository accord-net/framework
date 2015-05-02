// Perceptron Classifier
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
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Label noVisualizationLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListView weightsList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Label label5;
		private AForge.Controls.Chart errorChart;
		private System.Windows.Forms.CheckBox saveFilesCheck;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			samples = 0;
		private int			variables = 0;
		private double[,]	data = null;
		private int[]		classes = null;

		private double		learningRate = 0.1;
		private bool		saveStatisticsToFiles = false;

		private Thread workerThread = null;
        private volatile bool needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// initialize charts
			chart.AddDataSeries( "class1", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "class2", Color.Blue, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "classifier", Color.Gray, Chart.SeriesType.Line, 1, false );

			errorChart.AddDataSeries( "error", Color.Red, Chart.SeriesType.ConnectedDots, 3, false );

			// update some controls
			saveFilesCheck.Checked = saveStatisticsToFiles;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chart = new AForge.Controls.Chart();
			this.loadButton = new System.Windows.Forms.Button();
			this.dataList = new System.Windows.Forms.ListView();
			this.noVisualizationLabel = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.errorChart = new AForge.Controls.Chart();
			this.label5 = new System.Windows.Forms.Label();
			this.stopButton = new System.Windows.Forms.Button();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.weightsList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.saveFilesCheck = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.chart,
																					this.loadButton,
																					this.dataList,
																					this.noVisualizationLabel});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(190, 420);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 215);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(170, 170);
			this.chart.TabIndex = 2;
			this.chart.Text = "chart1";
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 390);
			this.loadButton.Name = "loadButton";
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// dataList
			// 
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(170, 190);
			this.dataList.TabIndex = 0;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// noVisualizationLabel
			// 
			this.noVisualizationLabel.Location = new System.Drawing.Point(10, 215);
			this.noVisualizationLabel.Name = "noVisualizationLabel";
			this.noVisualizationLabel.Size = new System.Drawing.Size(170, 170);
			this.noVisualizationLabel.TabIndex = 2;
			this.noVisualizationLabel.Text = "Visualization is not available.";
			this.noVisualizationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.noVisualizationLabel.Visible = false;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.saveFilesCheck,
																					this.errorChart,
																					this.label5,
																					this.stopButton,
																					this.iterationsBox,
																					this.label4,
																					this.weightsList,
																					this.label3,
																					this.label2,
																					this.startButton,
																					this.learningRateBox,
																					this.label1});
			this.groupBox2.Location = new System.Drawing.Point(210, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 420);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Training";
			// 
			// errorChart
			// 
			this.errorChart.Location = new System.Drawing.Point(10, 270);
			this.errorChart.Name = "errorChart";
			this.errorChart.Size = new System.Drawing.Size(220, 140);
			this.errorChart.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 250);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(101, 15);
			this.label5.TabIndex = 9;
			this.label5.Text = "Error\'s dynamics:";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(155, 49);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 8;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(90, 50);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.ReadOnly = true;
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 7;
			this.iterationsBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 16);
			this.label4.TabIndex = 6;
			this.label4.Text = "Iterations:";
			// 
			// weightsList
			// 
			this.weightsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.columnHeader1,
																						  this.columnHeader2});
			this.weightsList.FullRowSelect = true;
			this.weightsList.GridLines = true;
			this.weightsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.weightsList.Location = new System.Drawing.Point(10, 130);
			this.weightsList.Name = "weightsList";
			this.weightsList.Size = new System.Drawing.Size(220, 110);
			this.weightsList.TabIndex = 5;
			this.weightsList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Weight";
			this.columnHeader1.Width = 70;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Value";
			this.columnHeader2.Width = 100;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Perceptron weights:";
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(10, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(220, 2);
			this.label2.TabIndex = 3;
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(155, 19);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 2;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
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
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Learning rate:";
			// 
			// saveFilesCheck
			// 
			this.saveFilesCheck.Location = new System.Drawing.Point(10, 80);
			this.saveFilesCheck.Name = "saveFilesCheck";
			this.saveFilesCheck.Size = new System.Drawing.Size(182, 16);
			this.saveFilesCheck.TabIndex = 11;
			this.saveFilesCheck.Text = "Save weights and errors to files";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(459, 440);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Perceptron Classifier";
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

		// On "Load" button click - load data
		private void loadButton_Click( object sender, System.EventArgs e )
		{
			// data file format:
			// X1, X2, ... Xn, class (0|1)

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 50 samples only)
                float[,]	tempData = null;
				int[]		tempClasses = new int[50];

				// min and max X values
                float minX = float.MaxValue;
                float maxX = float.MinValue;

				// samples count
				samples = 0;

				try
				{
					string	str = null;

					// open selected file
					reader = File.OpenText( openFileDialog.FileName );

					// read the data
					while ( ( samples < 50 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// split the string
						string[] strs = str.Split( ';' );
						if ( strs.Length == 1 )
							strs = str.Split( ',' );

						// allocate data array
						if ( samples == 0 )
						{
							variables = strs.Length - 1;
                            tempData = new float[50, variables];
						}

						// parse data
						for ( int j = 0; j < variables; j++ )
						{
                            tempData[samples, j] = float.Parse( strs[j] );
						}
						tempClasses[samples] = int.Parse( strs[variables] );

						// search for min value
						if ( tempData[samples, 0] < minX )
							minX = tempData[samples, 0];
                        // search for max value
						if ( tempData[samples, 0] > maxX )
							maxX = tempData[samples, 0];

						samples++;
					}

					// allocate and set data
					data = new double[samples, variables];
					Array.Copy( tempData, 0, data, 0, samples * variables );
					classes = new int[samples];
					Array.Copy( tempClasses, 0, classes, 0, samples );

					// clear current result
					ClearCurrentSolution( );
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

				// update list and chart
				UpdateDataListView( );

				// show chart or not
				bool showChart = ( variables == 2 );

				if ( showChart )
				{
					chart.RangeX = new Range( minX, maxX );
                    ShowTrainingData( );
				}

				chart.Visible = showChart;
				noVisualizationLabel.Visible = !showChart;

				// enable start button
				startButton.Enabled = true;
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			learningRateBox.Text = learningRate.ToString( );
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all curent data and columns
			dataList.Items.Clear( );
			dataList.Columns.Clear( );

			// add columns
			for ( int i = 0, n = variables; i < n; i++ )
			{
				dataList.Columns.Add( string.Format( "X{0}", i + 1 ),
					50, HorizontalAlignment.Left );
			}
			dataList.Columns.Add( "Class", 50, HorizontalAlignment.Left );

			// add items
			for ( int i = 0; i < samples; i++ )
			{
				dataList.Items.Add( data[i, 0].ToString( ) );

				for ( int j = 1; j < variables; j++ )
				{
					dataList.Items[i].SubItems.Add( data[i, j].ToString( ) );
				}
				dataList.Items[i].SubItems.Add( classes[i].ToString( ) );
			}
		}

		// Show training data on chart
		private void ShowTrainingData( )
		{
			int class1Size = 0;
			int class2Size = 0;

			// calculate number of samples in each class
			for ( int i = 0, n = samples; i < n; i++ )
			{
				if ( classes[i] == 0 )
					class1Size++;
				else
					class2Size++;
			}

			// allocate classes arrays
			double[,] class1 = new double[class1Size, 2];
			double[,] class2 = new double[class2Size, 2];

			// fill classes arrays
			for ( int i = 0, c1 = 0, c2 = 0; i < samples; i++ )
			{
				if ( classes[i] == 0 )
				{
					// class 1
					class1[c1, 0] = data[i, 0];
					class1[c1, 1] = data[i, 1];
					c1++;
				}
				else
				{
					// class 2
					class2[c2, 0] = data[i, 0];
					class2[c2, 1] = data[i, 1];
					c2++;
				}
			}

			// updata chart control
			chart.UpdateDataSeries( "class1", class1 );
			chart.UpdateDataSeries( "class2", class2 );
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

		// Clear current solution
		private void ClearCurrentSolution( )
		{
			chart.UpdateDataSeries( "classifier", null );
			errorChart.UpdateDataSeries( "error", null );
			weightsList.Items.Clear( );
		}

		// On button "Start" - start learning procedure
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

		// On button "Stop" - stop learning procedure
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
				input[i] = new double[variables];
				output[i] = new double[1];

				// copy input
				for ( int j = 0; j < variables; j++ )
					input[i][j] = data[i, j];
				// copy output
				output[i][0] = classes[i];
			}

			// create perceptron
			ActivationNetwork	network = new ActivationNetwork( new ThresholdFunction( ), variables, 1 );
			ActivationNeuron	neuron = network.Layers[0].Neurons[0] as ActivationNeuron;
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
						for ( int i = 0; i < variables; i++ )
						{
							weightsFile.Write( neuron.Weights[i] + "," );
						}
						weightsFile.WriteLine( neuron.Threshold );
					}

					// run epoch of learning procedure
					double error = teacher.RunEpoch( input, output );
					errorsList.Add( error );

					// show current iteration
                    SetText( iterationsBox, iteration.ToString( ) );

					// save current error
					if ( errorsFile != null )
					{
						errorsFile.WriteLine( error );
					}				

					// show classifier in the case of 2 dimensional data
					if ( ( neuron.InputsCount == 2 ) && ( neuron.Weights[1] != 0 ) )
					{
						double k = - neuron.Weights[0] / neuron.Weights[1];
						double b = - neuron.Threshold / neuron.Weights[1];

						double[,] classifier = new double[2, 2] {
							{ chart.RangeX.Min, chart.RangeX.Min * k + b },
							{ chart.RangeX.Max, chart.RangeX.Max * k + b }
																};
                        // update chart
						chart.UpdateDataSeries( "classifier", classifier );
					}

					// stop if no error
					if ( error == 0 )
						break;

					iteration++;
				}

				// show perceptron's weights
                ListViewItem item = null;
                
                ClearList( weightsList );
				for ( int i = 0; i < variables; i++ )
				{
                    item = AddListItem( weightsList, string.Format( "Weight {0}", i + 1 ) );
                    AddListSubitem( item, neuron.Weights[i].ToString( "F6" ) );
				}
                item = AddListItem( weightsList, "Threshold" );
                AddListSubitem( item, neuron.Threshold.ToString( "F6" ) );

				// show error's dynamics
				double[,] errors = new double[errorsList.Count, 2];

				for ( int i = 0, n = errorsList.Count; i < n; i++ )
				{
					errors[i, 0] = i;
					errors[i, 1] = (double) errorsList[i];
				}

				errorChart.RangeX = new Range( 0, errorsList.Count - 1 );
				errorChart.RangeY = new Range( 0, samples );
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
