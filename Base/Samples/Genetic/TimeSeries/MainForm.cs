// Time Series Prediction using Genetic Programming and Gene Expression Programming
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
using AForge.Genetic;
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
		private System.Windows.Forms.Button loadDataButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.GroupBox groupBox2;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox populationSizeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox selectionBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox functionsSetBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox geneticMethodBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox windowSizeBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox predictionSizeBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox currentLearningErrorBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox currentPredictionErrorBox;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox solutionBox;
		private System.Windows.Forms.ColumnHeader estimatedYColumnHeader;
		private System.Windows.Forms.Button moreSettingsButton;
		private System.Windows.Forms.ToolTip toolTip;

		private double[]	data = null;
		private double[,]	dataToShow = null;

		private int populationSize = 100;
		private int iterations = 1000;
		private int windowSize = 5;
		private int predictionSize = 1;
		private int selectionMethod = 0;
		private int functionsSet = 0;
		private int geneticMethod = 0;

		private int headLength = 20;

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

			//
			chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Line, 1 );
			chart.AddDataSeries( "window", Color.LightGray, Chart.SeriesType.Line, 1, false );
			chart.AddDataSeries( "prediction", Color.Gray, Chart.SeriesType.Line, 1, false );

			selectionBox.SelectedIndex		= selectionMethod;
			functionsSetBox.SelectedIndex	= functionsSet;
			geneticMethodBox.SelectedIndex	= geneticMethod;
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
			this.components = new System.ComponentModel.Container();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dataList = new System.Windows.Forms.ListView();
			this.yColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.estimatedYColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.loadDataButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chart = new AForge.Controls.Chart();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.moreSettingsButton = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.predictionSizeBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.windowSizeBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.geneticMethodBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.functionsSetBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.selectionBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.populationSizeBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.currentPredictionErrorBox = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.currentLearningErrorBox = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.solutionBox = new System.Windows.Forms.TextBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
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
			this.dataList.TabIndex = 1;
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
			this.loadDataButton.TabIndex = 1;
			this.loadDataButton.Text = "&Load";
			this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chart);
			this.groupBox2.Location = new System.Drawing.Point(200, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(300, 380);
			this.groupBox2.TabIndex = 1;
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
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.moreSettingsButton);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.iterationsBox);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.predictionSizeBox);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.windowSizeBox);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.geneticMethodBox);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.functionsSetBox);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.selectionBox);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.populationSizeBox);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Location = new System.Drawing.Point(510, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(185, 240);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Settings";
			// 
			// moreSettingsButton
			// 
			this.moreSettingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.moreSettingsButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this.moreSettingsButton.Location = new System.Drawing.Point(10, 220);
			this.moreSettingsButton.Name = "moreSettingsButton";
			this.moreSettingsButton.Size = new System.Drawing.Size(25, 15);
			this.moreSettingsButton.TabIndex = 17;
			this.moreSettingsButton.Text = ">>";
			this.toolTip.SetToolTip(this.moreSettingsButton, "More settings");
			this.moreSettingsButton.Click += new System.EventHandler(this.moreSettingsButton_Click);
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label10.Location = new System.Drawing.Point(125, 220);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(58, 14);
			this.label10.TabIndex = 16;
			this.label10.Text = "( 0 - inifinity )";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 200);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 15;
			this.iterationsBox.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 202);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 16);
			this.label9.TabIndex = 14;
			this.label9.Text = "Iterations:";
			// 
			// label8
			// 
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label8.Location = new System.Drawing.Point(10, 190);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(165, 2);
			this.label8.TabIndex = 13;
			// 
			// predictionSizeBox
			// 
			this.predictionSizeBox.Location = new System.Drawing.Point(125, 160);
			this.predictionSizeBox.Name = "predictionSizeBox";
			this.predictionSizeBox.Size = new System.Drawing.Size(50, 20);
			this.predictionSizeBox.TabIndex = 12;
			this.predictionSizeBox.Text = "";
			this.predictionSizeBox.TextChanged += new System.EventHandler(this.predictionSizeBox_TextChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(10, 162);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 16);
			this.label7.TabIndex = 11;
			this.label7.Text = "Prediction size:";
			// 
			// windowSizeBox
			// 
			this.windowSizeBox.Location = new System.Drawing.Point(125, 135);
			this.windowSizeBox.Name = "windowSizeBox";
			this.windowSizeBox.Size = new System.Drawing.Size(50, 20);
			this.windowSizeBox.TabIndex = 10;
			this.windowSizeBox.Text = "";
			this.windowSizeBox.TextChanged += new System.EventHandler(this.windowSizeBox_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 137);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 16);
			this.label6.TabIndex = 9;
			this.label6.Text = "Window size:";
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label5.Location = new System.Drawing.Point(10, 125);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(165, 2);
			this.label5.TabIndex = 8;
			// 
			// geneticMethodBox
			// 
			this.geneticMethodBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.geneticMethodBox.Items.AddRange(new object[] {
																  "GP",
																  "GEP"});
			this.geneticMethodBox.Location = new System.Drawing.Point(110, 95);
			this.geneticMethodBox.Name = "geneticMethodBox";
			this.geneticMethodBox.Size = new System.Drawing.Size(65, 21);
			this.geneticMethodBox.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 6;
			this.label4.Text = "Genetic method:";
			// 
			// functionsSetBox
			// 
			this.functionsSetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.functionsSetBox.Items.AddRange(new object[] {
																 "Simple",
																 "Extended"});
			this.functionsSetBox.Location = new System.Drawing.Point(110, 70);
			this.functionsSetBox.Name = "functionsSetBox";
			this.functionsSetBox.Size = new System.Drawing.Size(65, 21);
			this.functionsSetBox.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Function set:";
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
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Selection method:";
			// 
			// populationSizeBox
			// 
			this.populationSizeBox.Location = new System.Drawing.Point(125, 20);
			this.populationSizeBox.Name = "populationSizeBox";
			this.populationSizeBox.Size = new System.Drawing.Size(50, 20);
			this.populationSizeBox.TabIndex = 1;
			this.populationSizeBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Population size:";
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(535, 364);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 3;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(620, 364);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.currentPredictionErrorBox);
			this.groupBox4.Controls.Add(this.label13);
			this.groupBox4.Controls.Add(this.currentLearningErrorBox);
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.currentIterationBox);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Location = new System.Drawing.Point(510, 255);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(185, 100);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Current iteration:";
			// 
			// currentPredictionErrorBox
			// 
			this.currentPredictionErrorBox.Location = new System.Drawing.Point(125, 70);
			this.currentPredictionErrorBox.Name = "currentPredictionErrorBox";
			this.currentPredictionErrorBox.ReadOnly = true;
			this.currentPredictionErrorBox.Size = new System.Drawing.Size(50, 20);
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
			this.currentLearningErrorBox.Size = new System.Drawing.Size(50, 20);
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
			this.currentIterationBox.Size = new System.Drawing.Size(50, 20);
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
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.solutionBox);
			this.groupBox5.Location = new System.Drawing.Point(10, 395);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(685, 50);
			this.groupBox5.TabIndex = 6;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Solution";
			// 
			// solutionBox
			// 
			this.solutionBox.Location = new System.Drawing.Point(10, 20);
			this.solutionBox.Name = "solutionBox";
			this.solutionBox.ReadOnly = true;
			this.solutionBox.Size = new System.Drawing.Size(665, 20);
			this.solutionBox.TabIndex = 0;
			this.solutionBox.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(704, 455);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Time Series Prediction using Genetic Programming and Gene Expression Programming";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
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
			populationSizeBox.Text	= populationSize.ToString( );
			iterationsBox.Text		= iterations.ToString( );
			windowSizeBox.Text		= windowSize.ToString( );
			predictionSizeBox.Text	= predictionSize.ToString( );
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

                loadDataButton.Enabled = enable;
                populationSizeBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;
                functionsSetBox.Enabled = enable;
                geneticMethodBox.Enabled = enable;
                windowSizeBox.Enabled = enable;
                predictionSizeBox.Enabled = enable;

                moreSettingsButton.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
		}
		
		// On window size changed
		private void windowSizeBox_TextChanged( object sender, System.EventArgs e )
		{
			UpdateWindowSize( );
		}

		// On prediction changed
		private void predictionSizeBox_TextChanged( object sender, System.EventArgs e )
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

		// Clear current solution
		private void ClearSolution( )
		{
			// remove solution form chart
			chart.UpdateDataSeries( "solution", null );
			// remove it from solution box
			solutionBox.Text = string.Empty;
			// remove it from data list view
			for ( int i = 0, n = dataList.Items.Count; i < n; i++ )
			{
				if ( dataList.Items[i].SubItems.Count > 1 )
					dataList.Items[i].SubItems.RemoveAt( 1 );
			}
		}

		// On button "Start"
		private void startButton_Click( object sender, System.EventArgs e )
		{
			ClearSolution( );

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

			selectionMethod	= selectionBox.SelectedIndex;
			functionsSet	= functionsSetBox.SelectedIndex;
			geneticMethod	= geneticMethodBox.SelectedIndex;
		
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
			// constants
			double[] constants = new double[10] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };
			// create fitness function
			TimeSeriesPredictionFitness fitness = new TimeSeriesPredictionFitness(
				data, windowSize, predictionSize, constants );
			// create gene function
			IGPGene gene = ( functionsSet == 0 ) ?
				(IGPGene) new SimpleGeneFunction( windowSize + constants.Length ) :
				(IGPGene) new ExtendedGeneFunction( windowSize + constants.Length );
			// create population
			Population population = new Population( populationSize,
				( geneticMethod == 0 ) ?
				(IChromosome) new GPTreeChromosome( gene ) :
				(IChromosome) new GEPChromosome( gene, headLength ),
				fitness,
				( selectionMethod == 0 ) ? (ISelectionMethod) new EliteSelection( ) :
				( selectionMethod == 1 ) ? (ISelectionMethod) new RankSelection( ) :
				(ISelectionMethod) new RouletteWheelSelection( )
				);
			// iterations
			int i = 1;
			// solution array
			int			solutionSize = data.Length - windowSize;
			double[,]	solution = new double[solutionSize, 2];
			double[]	input = new double[windowSize + constants.Length];

			// calculate X values to be used with solution function
			for ( int j = 0; j < solutionSize; j++ )
			{
				solution[j, 0] = j + windowSize;
			}
			// prepare input
			Array.Copy( constants, 0, input, windowSize, constants.Length );

			// loop
			while ( !needToStop )
			{
				// run one epoch of genetic algorithm
				population.RunEpoch( );

				try
				{
					// get best solution
					string bestFunction = population.BestChromosome.ToString( );

					// calculate best function and prediction error
					double learningError = 0.0;
					double predictionError = 0.0;
					// go through all the data
					for ( int j = 0, n = data.Length - windowSize; j < n; j++ )
					{
						// put values from current window as variables
						for ( int k = 0, b = j + windowSize - 1; k < windowSize; k++ )
						{
							input[k] = data[b - k];
						}

						// evalue the function
						solution[j, 1] = PolishExpression.Evaluate( bestFunction, input );

						// calculate prediction error
						if ( j >= n - predictionSize )
						{
							predictionError += Math.Abs( solution[j, 1] - data[windowSize + j] );
						}
						else
						{
							learningError += Math.Abs( solution[j, 1] - data[windowSize + j] );
						}
					}
					// update solution on the chart
					chart.UpdateDataSeries( "solution", solution );
				
					// set current iteration's info
                    SetText( currentIterationBox, i.ToString( ) );
                    SetText( currentLearningErrorBox, learningError.ToString( "F3" ) );
                    SetText( currentPredictionErrorBox, predictionError.ToString( "F3" ) );
				}
				catch
				{
					// remove any solutions from chart in case of any errors
					chart.UpdateDataSeries( "solution", null );
				}


				// increase current iteration
				i++;

				//
				if ( ( iterations != 0 ) && ( i > iterations ) )
					break;
			}

			// show solution
            SetText( solutionBox, population.BestChromosome.ToString( ) );
			for ( int j = windowSize, k = 0, n = data.Length; j < n; j++, k++ )
			{
                AddSubItem( dataList, j, solution[k, 1].ToString( ) );
			}

			// enable settings controls
			EnableControls( true );
		}

		// On "More settings" button click
		private void moreSettingsButton_Click( object sender, System.EventArgs e )
		{
			ExSettingsDialog settingsDlg = new ExSettingsDialog( );

			// init the dialog
			settingsDlg.MaxInitialTreeLevel	= GPTreeChromosome.MaxInitialLevel;
			settingsDlg.MaxTreeLevel		= GPTreeChromosome.MaxLevel;
			settingsDlg.HeadLength			= headLength;

			// show the dialog
			if ( settingsDlg.ShowDialog( ) == DialogResult.OK )
			{
				GPTreeChromosome.MaxInitialLevel = settingsDlg.MaxInitialTreeLevel;
				GPTreeChromosome.MaxLevel = settingsDlg.MaxTreeLevel;
				headLength = settingsDlg.HeadLength;
			}
		}
	}
}
