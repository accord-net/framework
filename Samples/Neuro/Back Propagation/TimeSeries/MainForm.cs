// Time Series Prediction using Multi-Layer Neural Network
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

namespace TimeSeries
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.ColumnHeader yColumnHeader;
		private System.Windows.Forms.ColumnHeader estimatedYColumnHeader;
		private System.Windows.Forms.Button loadDataButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox momentumBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox alphaBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox predictionSizeBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox windowSizeBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox currentPredictionErrorBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox currentLearningErrorBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label11;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private double[]	data = null;
		private double[,]	dataToShow = null;

		private double		learningRate = 0.1;
		private double		momentum = 0.0;
		private double		sigmoidAlphaValue = 2.0;
		private int			windowSize = 5;
		private int			predictionSize = 1;
		private int			iterations = 1000;

		private Thread workerThread = null;
        private volatile bool needToStop = false;

		private double[,]	windowDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };
		private double[,]	predictionDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// initializa chart control
			chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Line, 1 );
			chart.AddDataSeries( "window", Color.LightGray, Chart.SeriesType.Line, 1, false );
			chart.AddDataSeries( "prediction", Color.Gray, Chart.SeriesType.Line, 1, false );

			// update controls
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
			this.dataList = new System.Windows.Forms.ListView();
			this.yColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.estimatedYColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.loadDataButton = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chart = new AForge.Controls.Chart();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.momentumBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.alphaBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.predictionSizeBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.windowSizeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.currentPredictionErrorBox = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.currentLearningErrorBox = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dataList);
			this.groupBox1.Controls.Add(this.loadDataButton);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(180, 380);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// dataList
			// 
			this.dataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.yColumnHeader,
																					   this.estimatedYColumnHeader});
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(160, 315);
			this.dataList.TabIndex = 3;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// yColumnHeader
			// 
			this.yColumnHeader.Text = "Y:Real";
			this.yColumnHeader.Width = 70;
			// 
			// estimatedYColumnHeader
			// 
			this.estimatedYColumnHeader.Text = "Y:Estimated";
			this.estimatedYColumnHeader.Width = 70;
			// 
			// loadDataButton
			// 
			this.loadDataButton.Location = new System.Drawing.Point(10, 345);
			this.loadDataButton.Name = "loadDataButton";
			this.loadDataButton.TabIndex = 2;
			this.loadDataButton.Text = "&Load";
			this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chart);
			this.groupBox2.Location = new System.Drawing.Point(200, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(300, 380);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Function";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(280, 350);
			this.chart.TabIndex = 0;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.momentumBox);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.alphaBox);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.learningRateBox);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.iterationsBox);
			this.groupBox3.Controls.Add(this.predictionSizeBox);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.windowSizeBox);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Location = new System.Drawing.Point(510, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(195, 205);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Settings";
			// 
			// momentumBox
			// 
			this.momentumBox.Location = new System.Drawing.Point(125, 45);
			this.momentumBox.Name = "momentumBox";
			this.momentumBox.Size = new System.Drawing.Size(60, 20);
			this.momentumBox.TabIndex = 9;
			this.momentumBox.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 47);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 17);
			this.label6.TabIndex = 8;
			this.label6.Text = "Momentum:";
			// 
			// alphaBox
			// 
			this.alphaBox.Location = new System.Drawing.Point(125, 70);
			this.alphaBox.Name = "alphaBox";
			this.alphaBox.Size = new System.Drawing.Size(60, 20);
			this.alphaBox.TabIndex = 11;
			this.alphaBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 15);
			this.label2.TabIndex = 10;
			this.label2.Text = "Sigmoid\'s alpha value:";
			// 
			// learningRateBox
			// 
			this.learningRateBox.Location = new System.Drawing.Point(125, 20);
			this.learningRateBox.Name = "learningRateBox";
			this.learningRateBox.Size = new System.Drawing.Size(60, 20);
			this.learningRateBox.TabIndex = 7;
			this.learningRateBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 14);
			this.label1.TabIndex = 6;
			this.label1.Text = "Learning rate:";
			// 
			// label8
			// 
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.Location = new System.Drawing.Point(10, 157);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(175, 2);
			this.label8.TabIndex = 22;
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 165);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(60, 20);
			this.iterationsBox.TabIndex = 24;
			this.iterationsBox.Text = "";
			// 
			// predictionSizeBox
			// 
			this.predictionSizeBox.Location = new System.Drawing.Point(125, 130);
			this.predictionSizeBox.Name = "predictionSizeBox";
			this.predictionSizeBox.Size = new System.Drawing.Size(60, 20);
			this.predictionSizeBox.TabIndex = 21;
			this.predictionSizeBox.Text = "";
			this.predictionSizeBox.TextChanged += new System.EventHandler(this.predictionSizeBox_TextChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(10, 132);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 16);
			this.label7.TabIndex = 20;
			this.label7.Text = "Prediction size:";
			// 
			// windowSizeBox
			// 
			this.windowSizeBox.Location = new System.Drawing.Point(125, 105);
			this.windowSizeBox.Name = "windowSizeBox";
			this.windowSizeBox.Size = new System.Drawing.Size(60, 20);
			this.windowSizeBox.TabIndex = 19;
			this.windowSizeBox.Text = "";
			this.windowSizeBox.TextChanged += new System.EventHandler(this.windowSizeBox_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 107);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 18;
			this.label3.Text = "Window size:";
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label10.Location = new System.Drawing.Point(126, 185);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(58, 14);
			this.label10.TabIndex = 25;
			this.label10.Text = "( 0 - inifinity )";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 167);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 16);
			this.label9.TabIndex = 23;
			this.label9.Text = "Iterations:";
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label5.Location = new System.Drawing.Point(10, 97);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(175, 2);
			this.label5.TabIndex = 17;
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(630, 360);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 6;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(540, 360);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 5;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.currentPredictionErrorBox);
			this.groupBox4.Controls.Add(this.label13);
			this.groupBox4.Controls.Add(this.currentLearningErrorBox);
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.currentIterationBox);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Location = new System.Drawing.Point(510, 225);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(195, 100);
			this.groupBox4.TabIndex = 7;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Current iteration:";
			// 
			// currentPredictionErrorBox
			// 
			this.currentPredictionErrorBox.Location = new System.Drawing.Point(125, 70);
			this.currentPredictionErrorBox.Name = "currentPredictionErrorBox";
			this.currentPredictionErrorBox.ReadOnly = true;
			this.currentPredictionErrorBox.Size = new System.Drawing.Size(60, 20);
			this.currentPredictionErrorBox.TabIndex = 5;
			this.currentPredictionErrorBox.Text = "";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(10, 72);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(100, 16);
			this.label13.TabIndex = 4;
			this.label13.Text = "Prediction error:";
			// 
			// currentLearningErrorBox
			// 
			this.currentLearningErrorBox.Location = new System.Drawing.Point(125, 45);
			this.currentLearningErrorBox.Name = "currentLearningErrorBox";
			this.currentLearningErrorBox.ReadOnly = true;
			this.currentLearningErrorBox.Size = new System.Drawing.Size(60, 20);
			this.currentLearningErrorBox.TabIndex = 3;
			this.currentLearningErrorBox.Text = "";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(10, 47);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(80, 16);
			this.label12.TabIndex = 2;
			this.label12.Text = "Learning error:";
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(125, 20);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.ReadOnly = true;
			this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
			this.currentIterationBox.TabIndex = 1;
			this.currentIterationBox.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(10, 22);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(70, 16);
			this.label11.TabIndex = 0;
			this.label11.Text = "Iteration:";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(715, 398);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Time Series Prediction using Multi-Layer Neural Network";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
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
        private delegate void AddSubItemCallback( System.Windows.Forms.ListView control, int item, string subitemText );

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

        // Thread safe adding of subitem to list control
        private void AddSubItem( System.Windows.Forms.ListView control, int item, string subitemText )
        {
            if ( control.InvokeRequired )
            {
                AddSubItemCallback d = new AddSubItemCallback( AddSubItem );
                Invoke( d, new object[] { control, item, subitemText } );
            }
            else
            {
                control.Items[item].SubItems.Add( subitemText );
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
			windowSizeBox.Text		= windowSize.ToString( );
			predictionSizeBox.Text	= predictionSize.ToString( );
			iterationsBox.Text		= iterations.ToString( );
		}

		// Load data
		private void loadDataButton_Click(object sender, System.EventArgs e)
		{
			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;
				// read maximum 50 points
				double[] tempData = new double[50];

				try
				{
					// open selected file
					reader = File.OpenText( openFileDialog.FileName );
					string	str = null;
					int		i = 0;

					// read the data
					while ( ( i < 50 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// parse the value
						tempData[i] = double.Parse( str );

						i++;
					}

					// allocate and set data
					data = new double[i];
					dataToShow = new double[i, 2];
					Array.Copy( tempData, 0, data, 0, i );
					for ( int j = 0; j < i; j++ )
					{
						dataToShow[j, 0] = j;
						dataToShow[j, 1] = data[j];
					}
				}
				catch (Exception)
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
				chart.RangeX = new Range( 0, data.Length - 1 );
				chart.UpdateDataSeries( "data", dataToShow );
				chart.UpdateDataSeries( "solution", null );
				// set delimiters
				UpdateDelimiters( );
				// enable "Start" button
				startButton.Enabled = true;
			}
		}

		// Update delimiters on the chart
		private void UpdateDelimiters( )
		{
			// window delimiter
			windowDelimiter[0, 0] = windowDelimiter[1, 0] = windowSize;
			windowDelimiter[0, 1] = chart.RangeY.Min;
			windowDelimiter[1, 1] = chart.RangeY.Max;
			chart.UpdateDataSeries( "window", windowDelimiter );
			// prediction delimiter
			predictionDelimiter[0, 0] = predictionDelimiter[1, 0] = data.Length - 1 - predictionSize;
			predictionDelimiter[0, 1] = chart.RangeY.Min;
			predictionDelimiter[1, 1] = chart.RangeY.Max;
			chart.UpdateDataSeries( "prediction", predictionDelimiter );
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all current records
			dataList.Items.Clear( );
			// add new records
			for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				dataList.Items.Add( data[i].ToString( ) );
			}
		}

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback( bool enable );

        // Enable/disable controls
		private void EnableControls( bool enable )
		{
            if ( InvokeRequired )
            {
                EnableCallback d = new EnableCallback( EnableControls );
                Invoke( d, new object[] { enable } );
            }
            else
            {
			    loadDataButton.Enabled		= enable;
			    learningRateBox.Enabled		= enable;
			    momentumBox.Enabled			= enable;
			    alphaBox.Enabled			= enable;
			    windowSizeBox.Enabled		= enable;
			    predictionSizeBox.Enabled	= enable;
			    iterationsBox.Enabled		= enable;

			    startButton.Enabled			= enable;
			    stopButton.Enabled			= !enable;
    		}
        }

		// On window size changed
		private void windowSizeBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdateWindowSize( );
		}

		// On prediction changed
		private void predictionSizeBox_TextChanged(object sender, System.EventArgs e)
		{
			UpdatePredictionSize( );		
		}

		// Update window size
		private void UpdateWindowSize( )
		{
			if ( data != null )
			{
				// get new window size value
				try
				{
					windowSize = Math.Max( 1, Math.Min( 15, int.Parse( windowSizeBox.Text ) ) );
				}
				catch
				{
					windowSize = 5;
				}
				// check if we have too few data
				if ( windowSize >= data.Length )
					windowSize = 1;
				// update delimiters
				UpdateDelimiters( );
			}
		}

		// Update prediction size
		private void UpdatePredictionSize( )
		{
			if ( data != null )
			{
				// get new prediction size value
				try
				{
					predictionSize = Math.Max( 1, Math.Min( 10, int.Parse( predictionSizeBox.Text ) ) );
				}
				catch
				{
					predictionSize = 1;
				}
				// check if we have too few data
				if ( data.Length - predictionSize - 1 < windowSize )
					predictionSize = 1;
				// update delimiters
				UpdateDelimiters( );
			}
		}

		// On button "Start"
		private void startButton_Click( object sender, System.EventArgs e )
		{
			// clear previous solution
			for ( int j = 0, n = data.Length; j < n; j++ )
			{
				if ( dataList.Items[j].SubItems.Count > 1 )
					dataList.Items[j].SubItems.RemoveAt( 1 );
			}

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
				sigmoidAlphaValue = Math.Max( 0.001, Math.Min( 50, double.Parse( alphaBox.Text ) ) );
			}
			catch
			{
				sigmoidAlphaValue = 2;
			}
			// iterations
			try
			{
				iterations = Math.Max( 0, int.Parse( iterationsBox.Text ) );
			}
			catch
			{
				iterations = 1000;
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

		// On button "Stop"
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
			// number of learning samples
			int samples = data.Length - predictionSize - windowSize;
			// data transformation factor
			double factor = 1.7 / chart.RangeY.Length;
			double yMin = chart.RangeY.Min;
			// prepare learning data
			double[][] input = new double[samples][];
			double[][] output = new double[samples][];

			for ( int i = 0; i < samples; i++ )
			{
				input[i] = new double[windowSize];
				output[i] = new double[1];
			
				// set input
				for ( int j = 0; j < windowSize; j++ )
				{
					input[i][j] = ( data[i + j] - yMin ) * factor - 0.85;
				}
				// set output
				output[i][0] = ( data[i + windowSize] - yMin ) * factor - 0.85;
			}

			// create multi-layer neural network
			ActivationNetwork	network = new ActivationNetwork(
				new BipolarSigmoidFunction( sigmoidAlphaValue ),
				windowSize, windowSize * 2, 1 );
			// create teacher
			BackPropagationLearning teacher = new BackPropagationLearning( network );
			// set learning rate and momentum
			teacher.LearningRate	= learningRate;
			teacher.Momentum		= momentum;

			// iterations
			int iteration = 1;

			// solution array
			int			solutionSize = data.Length - windowSize;
			double[,]	solution = new double[solutionSize, 2];
			double[]	networkInput = new double[windowSize];

			// calculate X values to be used with solution function
			for ( int j = 0; j < solutionSize; j++ )
			{
				solution[j, 0] = j + windowSize;
			}
			
			// loop
			while ( !needToStop )
			{
				// run epoch of learning procedure
				double error = teacher.RunEpoch( input, output ) / samples;
			
				// calculate solution and learning and prediction errors
				double learningError = 0.0;
				double predictionError = 0.0;
				// go through all the data
				for ( int i = 0, n = data.Length - windowSize; i < n; i++ )
				{
					// put values from current window as network's input
					for ( int j = 0; j < windowSize; j++ )
					{
						networkInput[j] = ( data[i + j] - yMin ) * factor - 0.85;
					}

					// evalue the function
					solution[i, 1] = ( network.Compute( networkInput)[0] + 0.85 ) / factor + yMin;

					// calculate prediction error
					if ( i >= n - predictionSize )
					{
						predictionError += Math.Abs( solution[i, 1] - data[windowSize + i] );
					}
					else
					{
						learningError += Math.Abs( solution[i, 1] - data[windowSize + i] );
					}
				}
				// update solution on the chart
				chart.UpdateDataSeries( "solution", solution );

				// set current iteration's info
				SetText( currentIterationBox, iteration.ToString( ) );
				SetText( currentLearningErrorBox, learningError.ToString( "F3" ) );
				SetText( currentPredictionErrorBox, predictionError.ToString( "F3" ) );

				// increase current iteration
				iteration++;

				// check if we need to stop
				if ( ( iterations != 0 ) && ( iteration > iterations ) )
					break;
			}
			
			// show new solution
			for ( int j = windowSize, k = 0, n = data.Length; j < n; j++, k++ )
			{
                AddSubItem( dataList, j, solution[k, 1].ToString( ) );
            }

			// enable settings controls
			EnableControls( true );
		}
	}
}
