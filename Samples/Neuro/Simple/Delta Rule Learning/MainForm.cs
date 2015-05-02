// Classifier using Delta Rule Learning 
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
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox alphaBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox errorLimitBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox neuronsBox;
		private System.Windows.Forms.CheckBox oneNeuronForTwoCheck;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox classesBox;
		private System.Windows.Forms.CheckBox errorLimitCheck;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox currentErrorBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ListView weightsList;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.CheckBox saveFilesCheck;
		private AForge.Controls.Chart errorChart;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			samples = 0;
		private int			variables = 0;
		private double[,]	data = null;
		private int[]		classes = null;
		private int			classesCount = 0;
		private int[]		samplesPerClass = null;
		private int			neuronsCount = 0;

		private double		learningRate = 0.1;
		private double		sigmoidAlphaValue = 2.0;
		private double		learningErrorLimit = 0.1;
		private double		iterationLimit = 1000;
		private bool		useOneNeuronForTwoClasses = false;
		private bool		useErrorLimit = true;
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

			// update settings controls
			UpdateSettings( );

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
            this.classesBox = new System.Windows.Forms.TextBox( );
            this.label10 = new System.Windows.Forms.Label( );
            this.dataList = new System.Windows.Forms.ListView( );
            this.loadButton = new System.Windows.Forms.Button( );
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog( );
            this.groupBox2 = new System.Windows.Forms.GroupBox( );
            this.currentErrorBox = new System.Windows.Forms.TextBox( );
            this.label11 = new System.Windows.Forms.Label( );
            this.label9 = new System.Windows.Forms.Label( );
            this.currentIterationBox = new System.Windows.Forms.TextBox( );
            this.label8 = new System.Windows.Forms.Label( );
            this.label7 = new System.Windows.Forms.Label( );
            this.errorLimitCheck = new System.Windows.Forms.CheckBox( );
            this.oneNeuronForTwoCheck = new System.Windows.Forms.CheckBox( );
            this.neuronsBox = new System.Windows.Forms.TextBox( );
            this.label6 = new System.Windows.Forms.Label( );
            this.label5 = new System.Windows.Forms.Label( );
            this.iterationsBox = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.errorLimitBox = new System.Windows.Forms.TextBox( );
            this.label3 = new System.Windows.Forms.Label( );
            this.alphaBox = new System.Windows.Forms.TextBox( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.learningRateBox = new System.Windows.Forms.TextBox( );
            this.stopButton = new System.Windows.Forms.Button( );
            this.startButton = new System.Windows.Forms.Button( );
            this.groupBox3 = new System.Windows.Forms.GroupBox( );
            this.saveFilesCheck = new System.Windows.Forms.CheckBox( );
            this.label13 = new System.Windows.Forms.Label( );
            this.weightsList = new System.Windows.Forms.ListView( );
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader( );
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader( );
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader( );
            this.errorChart = new AForge.Controls.Chart( );
            this.label12 = new System.Windows.Forms.Label( );
            this.groupBox1.SuspendLayout( );
            this.groupBox2.SuspendLayout( );
            this.groupBox3.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.classesBox );
            this.groupBox1.Controls.Add( this.label10 );
            this.groupBox1.Controls.Add( this.dataList );
            this.groupBox1.Controls.Add( this.loadButton );
            this.groupBox1.Location = new System.Drawing.Point( 10, 10 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 230, 330 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // classesBox
            // 
            this.classesBox.Location = new System.Drawing.Point( 190, 297 );
            this.classesBox.Name = "classesBox";
            this.classesBox.ReadOnly = true;
            this.classesBox.Size = new System.Drawing.Size( 30, 20 );
            this.classesBox.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point( 140, 299 );
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size( 50, 12 );
            this.label10.TabIndex = 2;
            this.label10.Text = "Classes:";
            // 
            // dataList
            // 
            this.dataList.FullRowSelect = true;
            this.dataList.GridLines = true;
            this.dataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataList.Location = new System.Drawing.Point( 10, 20 );
            this.dataList.Name = "dataList";
            this.dataList.Size = new System.Drawing.Size( 210, 270 );
            this.dataList.TabIndex = 0;
            this.dataList.UseCompatibleStateImageBehavior = false;
            this.dataList.View = System.Windows.Forms.View.Details;
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point( 10, 297 );
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size( 75, 23 );
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "&Load";
            this.loadButton.Click += new System.EventHandler( this.loadButton_Click );
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.currentErrorBox );
            this.groupBox2.Controls.Add( this.label11 );
            this.groupBox2.Controls.Add( this.label9 );
            this.groupBox2.Controls.Add( this.currentIterationBox );
            this.groupBox2.Controls.Add( this.label8 );
            this.groupBox2.Controls.Add( this.label7 );
            this.groupBox2.Controls.Add( this.errorLimitCheck );
            this.groupBox2.Controls.Add( this.oneNeuronForTwoCheck );
            this.groupBox2.Controls.Add( this.neuronsBox );
            this.groupBox2.Controls.Add( this.label6 );
            this.groupBox2.Controls.Add( this.label5 );
            this.groupBox2.Controls.Add( this.iterationsBox );
            this.groupBox2.Controls.Add( this.label4 );
            this.groupBox2.Controls.Add( this.errorLimitBox );
            this.groupBox2.Controls.Add( this.label3 );
            this.groupBox2.Controls.Add( this.alphaBox );
            this.groupBox2.Controls.Add( this.label2 );
            this.groupBox2.Controls.Add( this.label1 );
            this.groupBox2.Controls.Add( this.learningRateBox );
            this.groupBox2.Controls.Add( this.stopButton );
            this.groupBox2.Controls.Add( this.startButton );
            this.groupBox2.Location = new System.Drawing.Point( 250, 10 );
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size( 185, 330 );
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Training";
            // 
            // currentErrorBox
            // 
            this.currentErrorBox.Location = new System.Drawing.Point( 125, 255 );
            this.currentErrorBox.Name = "currentErrorBox";
            this.currentErrorBox.ReadOnly = true;
            this.currentErrorBox.Size = new System.Drawing.Size( 50, 20 );
            this.currentErrorBox.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point( 10, 257 );
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size( 121, 14 );
            this.label11.TabIndex = 19;
            this.label11.Text = "Current average error:";
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point( 10, 283 );
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size( 165, 2 );
            this.label9.TabIndex = 18;
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point( 125, 230 );
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size( 50, 20 );
            this.currentIterationBox.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point( 10, 232 );
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size( 98, 16 );
            this.label8.TabIndex = 16;
            this.label8.Text = "Current iteration:";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point( 10, 220 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 165, 2 );
            this.label7.TabIndex = 15;
            // 
            // errorLimitCheck
            // 
            this.errorLimitCheck.Location = new System.Drawing.Point( 10, 185 );
            this.errorLimitCheck.Name = "errorLimitCheck";
            this.errorLimitCheck.Size = new System.Drawing.Size( 157, 30 );
            this.errorLimitCheck.TabIndex = 14;
            this.errorLimitCheck.Text = "Use error limit (checked) or iterations limit";
            // 
            // oneNeuronForTwoCheck
            // 
            this.oneNeuronForTwoCheck.Enabled = false;
            this.oneNeuronForTwoCheck.Location = new System.Drawing.Point( 10, 165 );
            this.oneNeuronForTwoCheck.Name = "oneNeuronForTwoCheck";
            this.oneNeuronForTwoCheck.Size = new System.Drawing.Size( 168, 15 );
            this.oneNeuronForTwoCheck.TabIndex = 13;
            this.oneNeuronForTwoCheck.Text = "Use 1 neuron for 2 classes";
            this.oneNeuronForTwoCheck.CheckedChanged += new System.EventHandler( this.oneNeuronForTwoCheck_CheckedChanged );
            // 
            // neuronsBox
            // 
            this.neuronsBox.Location = new System.Drawing.Point( 125, 135 );
            this.neuronsBox.Name = "neuronsBox";
            this.neuronsBox.ReadOnly = true;
            this.neuronsBox.Size = new System.Drawing.Size( 50, 20 );
            this.neuronsBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point( 10, 137 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 59, 12 );
            this.label6.TabIndex = 11;
            this.label6.Text = "Neurons:";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font( "Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 204 ) ) );
            this.label5.Location = new System.Drawing.Point( 125, 115 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 58, 17 );
            this.label5.TabIndex = 10;
            this.label5.Text = "( 0 - inifinity )";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point( 125, 95 );
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size( 50, 20 );
            this.iterationsBox.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 10, 97 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 90, 13 );
            this.label4.TabIndex = 8;
            this.label4.Text = "Iterations limit:";
            // 
            // errorLimitBox
            // 
            this.errorLimitBox.Location = new System.Drawing.Point( 125, 70 );
            this.errorLimitBox.Name = "errorLimitBox";
            this.errorLimitBox.Size = new System.Drawing.Size( 50, 20 );
            this.errorLimitBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 10, 72 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 110, 15 );
            this.label3.TabIndex = 6;
            this.label3.Text = "Learning error limit:";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point( 125, 45 );
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size( 50, 20 );
            this.alphaBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 10, 47 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 120, 15 );
            this.label2.TabIndex = 4;
            this.label2.Text = "Sigmoid\'s alpha value:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 10, 22 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 75, 16 );
            this.label1.TabIndex = 2;
            this.label1.Text = "Learning rate:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point( 125, 20 );
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size( 50, 20 );
            this.learningRateBox.TabIndex = 3;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point( 100, 297 );
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size( 75, 23 );
            this.stopButton.TabIndex = 6;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler( this.stopButton_Click );
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point( 10, 297 );
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size( 75, 23 );
            this.startButton.TabIndex = 5;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler( this.startButton_Click );
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add( this.saveFilesCheck );
            this.groupBox3.Controls.Add( this.label13 );
            this.groupBox3.Controls.Add( this.weightsList );
            this.groupBox3.Controls.Add( this.errorChart );
            this.groupBox3.Controls.Add( this.label12 );
            this.groupBox3.Location = new System.Drawing.Point( 445, 10 );
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size( 220, 330 );
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Solution";
            // 
            // saveFilesCheck
            // 
            this.saveFilesCheck.Location = new System.Drawing.Point( 10, 305 );
            this.saveFilesCheck.Name = "saveFilesCheck";
            this.saveFilesCheck.Size = new System.Drawing.Size( 195, 15 );
            this.saveFilesCheck.TabIndex = 4;
            this.saveFilesCheck.Text = "Save weights and errors to files";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point( 10, 170 );
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size( 100, 12 );
            this.label13.TabIndex = 3;
            this.label13.Text = "Error\'s dynamics:";
            // 
            // weightsList
            // 
            this.weightsList.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3} );
            this.weightsList.FullRowSelect = true;
            this.weightsList.GridLines = true;
            this.weightsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.weightsList.Location = new System.Drawing.Point( 10, 35 );
            this.weightsList.Name = "weightsList";
            this.weightsList.Size = new System.Drawing.Size( 200, 130 );
            this.weightsList.TabIndex = 2;
            this.weightsList.UseCompatibleStateImageBehavior = false;
            this.weightsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Neuron";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Weight";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Value";
            // 
            // errorChart
            // 
            this.errorChart.Location = new System.Drawing.Point( 10, 185 );
            this.errorChart.Name = "errorChart";
            this.errorChart.Size = new System.Drawing.Size( 200, 110 );
            this.errorChart.TabIndex = 1;
            this.errorChart.Text = "chart1";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point( 10, 20 );
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size( 100, 15 );
            this.label12.TabIndex = 0;
            this.label12.Text = "Network weights:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 674, 350 );
            this.Controls.Add( this.groupBox3 );
            this.Controls.Add( this.groupBox2 );
            this.Controls.Add( this.groupBox1 );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Classifier using Delta Rule Learning";
            this.Closing += new System.ComponentModel.CancelEventHandler( this.MainForm_Closing );
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout( );
            this.groupBox2.ResumeLayout( false );
            this.groupBox2.PerformLayout( );
            this.groupBox3.ResumeLayout( false );
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

		// Load input data
		private void loadButton_Click( object sender, System.EventArgs e )
		{
			// data file format:
			// X1, X2, ..., Xn, class

			// load maximum 10 classes !

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 200 samples only)
				double[,]	tempData = null;
				int[]		tempClasses = new int[200];

				// min and max X values
				double minX = double.MaxValue;
				double maxX = double.MinValue;

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

						// allocate data array
						if ( samples == 0 )
						{
							variables = strs.Length - 1;
							tempData = new double[200, variables];
						}

						// parse data
						for ( int j = 0; j < variables; j++ )
						{
							tempData[samples, j] = double.Parse( strs[j] );
						}
						tempClasses[samples] = int.Parse( strs[variables] );

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
					data = new double[samples, variables];
					Array.Copy( tempData, 0, data, 0, samples * variables );
					classes = new int[samples];
					Array.Copy( tempClasses, 0, classes, 0, samples );
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

				classesBox.Text = classesCount.ToString( );
				oneNeuronForTwoCheck.Enabled = ( classesCount == 2 );

				// set neurons count
				neuronsCount = ( ( classesCount == 2 ) && ( useOneNeuronForTwoClasses ) ) ? 1 : classesCount;
				neuronsBox.Text = neuronsCount.ToString( );

				ClearSolution( );
				startButton.Enabled = true;
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			learningRateBox.Text	= learningRate.ToString( );
			alphaBox.Text			= sigmoidAlphaValue.ToString( );
			errorLimitBox.Text		= learningErrorLimit.ToString( );
			iterationsBox.Text		= iterationLimit.ToString( );

			oneNeuronForTwoCheck.Checked	= useOneNeuronForTwoClasses;
			errorLimitCheck.Checked			= useErrorLimit;
			saveFilesCheck.Checked			= saveStatisticsToFiles;
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

		// Use or not one neuron to classify two classes
		private void oneNeuronForTwoCheck_CheckedChanged( object sender, System.EventArgs e )
		{
			useOneNeuronForTwoClasses = oneNeuronForTwoCheck.Checked;		
			// update neurons count box
			neuronsCount = ( ( classesCount == 2 ) && ( useOneNeuronForTwoClasses ) ) ? 1 : classesCount;
			neuronsBox.Text = neuronsCount.ToString( );
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
			    alphaBox.Enabled			= enable;
			    errorLimitBox.Enabled		= enable;
			    iterationsBox.Enabled		= enable;
			    oneNeuronForTwoCheck.Enabled = ( ( enable ) && ( classesCount == 2 ) );
			    errorLimitCheck.Enabled		= enable;
			    saveFilesCheck.Enabled		= enable;

			    loadButton.Enabled			= enable;
			    startButton.Enabled			= enable;
			    stopButton.Enabled			= !enable;
		    }
        }

		// Clear current solution
		private void ClearSolution( )
		{
			errorChart.UpdateDataSeries( "error", null );
			weightsList.Items.Clear( );
			currentIterationBox.Text	= string.Empty;
			currentErrorBox.Text		= string.Empty;
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
			// get iterations limit
			try
			{
				iterationLimit = Math.Max( 0, int.Parse( iterationsBox.Text ) );
			}
			catch
			{
				iterationLimit = 1000;
			}

			useOneNeuronForTwoClasses = oneNeuronForTwoCheck.Checked;
			useErrorLimit = errorLimitCheck.Checked;
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
			bool reducedNetwork = ( ( classesCount == 2 ) && ( useOneNeuronForTwoClasses ) );

			// prepare learning data
			double[][] input = new double[samples][];
			double[][] output = new double[samples][];

			for ( int i = 0; i < samples; i++ )
			{
				input[i] = new double[variables];
				output[i] = new double[neuronsCount];

				// set input
				for ( int j = 0; j < variables; j++ )
					input[i][j] = data[i, j];
				// set output
				if ( reducedNetwork )
				{
					output[i][0] = classes[i];
				}
				else
				{
					output[i][classes[i]] = 1;
				}
			}

			// create perceptron
			ActivationNetwork	network = new ActivationNetwork(
				new SigmoidFunction( sigmoidAlphaValue ), variables, neuronsCount );
			ActivationLayer		layer = network.Layers[0] as ActivationLayer;
			// create teacher
			DeltaRuleLearning	teacher = new DeltaRuleLearning( network );
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
						for ( int i = 0; i < neuronsCount; i++ )
						{
							weightsFile.Write( "neuron" + i + "," );
							for ( int j = 0; j < variables; j++ )
								weightsFile.Write( layer.Neurons[i].Weights[j] + "," );
							weightsFile.WriteLine( ( (ActivationNeuron) layer.Neurons[i] ).Threshold );
						}
					}

					// run epoch of learning procedure
					double error = teacher.RunEpoch( input, output ) / samples;
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
					if ( ( useErrorLimit ) && ( error <= learningErrorLimit ) )
						break;
					if ( ( !useErrorLimit ) && ( iterationLimit != 0 ) && ( iteration > iterationLimit ) )
						break;
				}

				// show perceptron's weights
                ClearList( weightsList );
				for ( int i = 0; i < neuronsCount; i++ )
				{
					string neuronName = string.Format( "Neuron {0}", i + 1 );
					ListViewItem item = null;

					// add all weights
					for ( int j = 0; j < variables; j++ )
					{
                        item = AddListItem( weightsList, neuronName );
                        AddListSubitem( item, string.Format( "Weight {0}", j + 1 ) );
                        AddListSubitem( item, layer.Neurons[i].Weights[0].ToString( "F6" ) );
					}
					// threshold
                    item = AddListItem( weightsList, neuronName );
                    AddListSubitem( item, "Threshold");
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
