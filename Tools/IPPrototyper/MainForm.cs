// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2010-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using AForge;
using AForge.Imaging;
using AForge.Imaging.IPPrototyper;

namespace IPPrototyper
{
    internal partial class MainForm : Form
    {
        // selected folder containing images to process
        private string selectedFolder;
        // list of found image processing routines
        private Dictionary<string, IImageProcessingRoutine> processingRoutines = new Dictionary<string,IImageProcessingRoutine>( );
        // currently active image processing routine to use
        private IImageProcessingRoutine ipRoutineToUse = null;
        // image processing log
        private ImageProcessingLog processingLog = new ImageProcessingLog( );

        private HistogramForm histogramForm = null;

        // list of recently used folders
        private List<string> recentFolders = new List<string>( );

        // stopwatch to measure time taken by image processing routine
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch( );

        #region Configuration Option Names
        private const string mainFormXOption = "MainFormX";
        private const string mainFormYOption = "MainFormY";
        private const string mainFormWidthOption = "MainFormWidth";
        private const string mainFormHeightOption = "MainFormHeight";
        private const string mainFormStateOption = "MainFormState";
        private const string splitter1Option = "Splitter1";
        private const string splitter2Option = "Splitter2";
        private const string splitter3Option = "Splitter3";
        private const string recentFolderOption = "RecentFolder";
        private const string pictureSizeModeOption = "PictureSizeMode";
        private const string openLastOption = "OpenLastFolder";
        #endregion


        public MainForm( )
        {
            InitializeComponent( );

        }

        // On form loading
        private void MainForm_Load( object sender, EventArgs e )
        {
            // collect available modules in application's directory
            CollectModules( Path.GetDirectoryName( Application.ExecutablePath ) );
            // add modules' name to application's menu
            foreach ( string routineName in processingRoutines.Keys )
            {
                ToolStripItem item = modulesToolStripMenuItem.DropDownItems.Add( routineName );

                item.Click += new System.EventHandler( this.module_Click );

                if ( ipRoutineToUse == null )
                {
                    ipRoutineToUse = processingRoutines[routineName];
                }
            }

            // load configuratio
            Configuration config = Configuration.Instance;

            if ( config.Load( ) )
            {
                try
                {
                    bool windowPositionIsValid = false;
                    // get window location/size
                    Size windowSize = new Size(
                        int.Parse( config.GetConfigurationOption( mainFormWidthOption ) ),
                        int.Parse( config.GetConfigurationOption( mainFormHeightOption ) ) );
                    System.Drawing.Point windowTopLeft = new System.Drawing.Point(
                        int.Parse( config.GetConfigurationOption( mainFormXOption ) ),
                        int.Parse( config.GetConfigurationOption( mainFormYOption ) ) );
                    System.Drawing.Point windowTopRight = new System.Drawing.Point(
                        windowTopLeft.X + windowSize.Width, windowTopLeft.Y );

                    // check if window location is within of the displays
                    foreach ( Screen screen in Screen.AllScreens )
                    {
                        if ( ( screen.WorkingArea.Contains( windowTopLeft ) ) ||
                             ( screen.WorkingArea.Contains( windowTopRight ) ) )
                        {
                            windowPositionIsValid = true;
                            break;
                        }
                    }

                    if ( windowPositionIsValid )
                    {
                        Location = windowTopLeft;
                        Size = windowSize;

                        WindowState = (FormWindowState) Enum.Parse( typeof( FormWindowState ),
                            config.GetConfigurationOption( mainFormStateOption ) );

                        mainSplitContainer.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter1Option ) );
                        splitContainer1.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter2Option ) );
                        splitContainer2.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter3Option ) );
                    }

                    // get size mode of picture box
                    SetPictureBoxSizeMode( (PictureBoxSizeMode) Enum.Parse( typeof( PictureBoxSizeMode ),
                        config.GetConfigurationOption( pictureSizeModeOption ) ) );

                    // get recent folders
                    for ( int i = 0; i < 7; i++ )
                    {
                        string rf = config.GetConfigurationOption( recentFolderOption + i );

                        if ( rf != null )
                            recentFolders.Add( rf );
                    }

                    RebuildRecentFoldersList( );

                    bool openLast = bool.Parse( config.GetConfigurationOption( openLastOption ) );
                    openLastFolderOnStartToolStripMenuItem.Checked = openLast;

                    if ( ( openLast ) && ( recentFolders.Count > 0 ) )
                    {
                        OpenFolder( recentFolders[0] );
                    }
                }
                catch
                {
                }
            }
        }

        // Rebuild menu with the list of recently used folders
        private void RebuildRecentFoldersList( )
        {
            // unsubscribe from events
            foreach ( ToolStripItem item in recentFoldersToolStripMenuItem.DropDownItems )
            {
                item.Click -= new EventHandler( recentFolder_Click );
            }

            // remove all current items
            recentFoldersToolStripMenuItem.DropDownItems.Clear( );

            // add new items
            foreach ( string folderName in recentFolders )
            {
                ToolStripItem item = recentFoldersToolStripMenuItem.DropDownItems.Add( folderName );

                item.Click += new EventHandler( recentFolder_Click );
            }
        }

        // On form closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Configuration config = Configuration.Instance;

            // save window location/size
            if ( WindowState != FormWindowState.Minimized )
            {
                if ( WindowState != FormWindowState.Maximized )
                {
                    config.SetConfigurationOption( mainFormXOption, Location.X.ToString( ) );
                    config.SetConfigurationOption( mainFormYOption, Location.Y.ToString( ) );
                    config.SetConfigurationOption( mainFormWidthOption, Width.ToString( ) );
                    config.SetConfigurationOption( mainFormHeightOption, Height.ToString( ) );
                }
                config.SetConfigurationOption( mainFormStateOption, WindowState.ToString( ) );

                config.SetConfigurationOption( splitter1Option, mainSplitContainer.SplitterDistance.ToString( ) );
                config.SetConfigurationOption( splitter2Option, splitContainer1.SplitterDistance.ToString( ) );
                config.SetConfigurationOption( splitter3Option, splitContainer2.SplitterDistance.ToString( ) );
            }

            // save size mode of picture box
            config.SetConfigurationOption( pictureSizeModeOption, pictureBox.SizeMode.ToString( ) );

            // save recent folders
            for ( int i = 0, n = recentFolders.Count; i < n; i++ )
            {
                config.SetConfigurationOption( recentFolderOption + i, recentFolders[i] );
            }
            config.SetConfigurationOption( openLastOption, openLastFolderOnStartToolStripMenuItem.Checked.ToString( ) );

            try
            {
                config.Save( );
            }
            catch ( IOException ex )
            {
                MessageBox.Show( "Failed saving confguration file.\r\n\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // Add folder to the list of recently used folders
        public void AddRecentFolder( string folderName )
        {
            int index = recentFolders.IndexOf( folderName );

            if ( index != 0 )
            {
                if ( index != -1 )
                {
                    // remove previous entry
                    recentFolders.RemoveAt( index );
                }

                // put this folder as the most recent
                recentFolders.Insert( 0, folderName );

                if ( recentFolders.Count > 7 )
                {
                    recentFolders.RemoveAt( 7 );
                }
            }
        }

        // Remove specified folder from the list of recently used folders
        public void RemoveRecentFolder( string folderName )
        {
            recentFolders.Remove( folderName );
        }

        // Update check style of modules' items
        private void modulesToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            foreach ( ToolStripMenuItem item in modulesToolStripMenuItem.DropDownItems )
            {
                item.Checked = ( ( ipRoutineToUse != null ) && ( ipRoutineToUse.Name == item.Text ) );
            }
        }

        // Item is clicked in modules' menu
        private void module_Click( object sender, EventArgs e )
        {
            ipRoutineToUse = processingRoutines[( (ToolStripMenuItem) sender ).Text];
            ProcessSelectedImage( );
        }

        // Exit from application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        // Collect information about available modules
        private void CollectModules( string path )
        {
            // create directory info
            DirectoryInfo dir = new DirectoryInfo( path );

            // get all dll files from the directory
            FileInfo[] files = dir.GetFiles( "*.dll" );

            // walk through all files
            foreach ( FileInfo f in files )
            {
                CollectImageProcessingRoutines( Path.Combine( path, f.Name ) );
            }
        }

        // Collect available image processing routines in the specified assembly
        private void CollectImageProcessingRoutines( string fname )
        {
            Type typeIImageProcessingRoutine = typeof( IImageProcessingRoutine );
            Assembly assembly = null;

            try
            {
                // try to load assembly
                assembly = Assembly.LoadFrom( fname );

                // get types of the assembly
                Type[] types = assembly.GetTypes( );

                // check all types
                foreach ( Type type in types )
                {
                    // get interfaces ot the type
                    Type[] interfaces = type.GetInterfaces( );

                    // check, if the type is inherited from IImageProcessingRoutine
                    if ( Array.IndexOf( interfaces, typeIImageProcessingRoutine ) != -1 )
                    {
                        IImageProcessingRoutine	ipRoutine = null;

                        try
                        {
                            // create an instance of the type
                            ipRoutine = (IImageProcessingRoutine) Activator.CreateInstance( type );
                            // add routine to collection
                            if ( !processingRoutines.ContainsKey( ipRoutine.Name ) )
                                processingRoutines.Add( ipRoutine.Name, ipRoutine );
                        }
                        catch ( Exception )
                        {
                            // something failed during instance creatinion
                        }
                    }
                }
            }
            catch ( Exception )
            {
            }
        }

        // Open folder
        private void openFolderToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( folderBrowserDialog.ShowDialog( ) == DialogResult.OK )
            {
                if ( OpenFolder( folderBrowserDialog.SelectedPath ) )
                {
                    // remember this folder
                    AddRecentFolder( selectedFolder );
                    RebuildRecentFoldersList( );
                }
            }
        }

        // Item is clicked in recent folders list
        private void recentFolder_Click( object sender, EventArgs e )
        {
            string folderName = ( (ToolStripMenuItem) sender ).Text;

            if ( OpenFolder( folderName ) )
            {
                // move the folder up in the list
                AddRecentFolder( folderName );
            }
            else
            {
                // remove failing folder
                RemoveRecentFolder( folderName );
            }
            RebuildRecentFoldersList( );
        }

        // Open specified folder
        private bool OpenFolder( string folderName )
        {
            bool success = false;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo( folderName );
                FileInfo[] fileInfos = dirInfo.GetFiles( );

                filesListView.Items.Clear( );

                // collect all image files
                foreach ( FileInfo fi in fileInfos )
                {
                    string ext = fi.Extension.ToLower( );

                    // check for supported extension
                    if (
                        ( ext == ".jpg" ) || ( ext == ".jpeg" ) ||
                        ( ext == ".bmp" ) || ( ext == ".png" ) )
                    {
                        filesListView.Items.Add( fi.Name );
                    }
                }

                logListView.Items.Clear( );
                filesListView.Focus( );
                ProcessSelectedImage( );

                selectedFolder = folderName;
                success = true;
            }
            catch
            {
                MessageBox.Show( "Failed opening the folder:\n" + folderName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            UpdateImageCountStatus( );

            return success;
        }

        // Selection has changed in files list view control
        private void filesListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            ProcessSelectedImage( );
        }

        // Process currently selected image
        private void ProcessSelectedImage( )
        {
            if ( filesListView.SelectedItems.Count == 1 )
            {
                Bitmap image = null;

                try
                {
                    image = (Bitmap) Bitmap.FromFile( Path.Combine( selectedFolder, filesListView.SelectedItems[0].Text ) );
                }
                catch
                {
                }

                if ( image != null )
                {
                    ProcessImage( image );
                    ShowLogMessages( );
                    UpdateLogView( );
                    return;
                }
                else
                {
                    MessageBox.Show( "Failed loading file: " + filesListView.SelectedItems[0].Text,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    filesListView.Items.Remove( filesListView.SelectedItems[0] );
                    UpdateImageCountStatus( );
                }
            }

            pictureBox.Image = null;
            logBox.Text = string.Empty;
        }

        // Process specified image
        private void ProcessImage( Bitmap image )
        {
            processingLog.Clear( );
            processingLog.AddImage( "Source", image );

            if ( ipRoutineToUse != null )
            {
                stopWatch.Reset( );
                stopWatch.Start( );

                // process image with selected image processing routine
                ipRoutineToUse.Process( image, processingLog );

                stopWatch.Stop( );

                UpdateProcessingTimeStatus( stopWatch.ElapsedMilliseconds );
            }
        }

        // Update log view
        private void UpdateLogView( )
        {
            string currentSelection = string.Empty;
            int newSelectionIndex = 0;
            int i = 0;

            if ( logListView.SelectedIndices.Count > 0 )
            {
                currentSelection = logListView.Items[logListView.SelectedIndices[0]].Text;
            }

            logListView.Items.Clear( );

            foreach ( KeyValuePair<string, Bitmap> kvp in processingLog.Images )
            {
                logListView.Items.Add( kvp.Key );

                if ( kvp.Key == currentSelection )
                    newSelectionIndex = i;

                i++;
            }

            logListView.SelectedIndices.Add( newSelectionIndex );
            logListView.EnsureVisible( newSelectionIndex );
        }

        // Display log messages
        private void ShowLogMessages( )
        {
            StringBuilder sb = new StringBuilder( );

            foreach ( string message in processingLog.Messages )
            {
                sb.Append( message );
                sb.Append( "\r\n" );
            }

            logBox.Text = sb.ToString( );
        }

        // Selection has changed in log list view - update image
        private void logListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( logListView.SelectedIndices.Count == 1 )
            {
                string stepName = logListView.SelectedItems[0].Text;

                Bitmap oldImage = (Bitmap) pictureBox.Image;
                pictureBox.Image = (Bitmap) processingLog.Images[stepName].Clone( );

                if ( oldImage != null )
                {
                    oldImage.Dispose( );
                }

                ShowCurrentImageHistogram( );
                UpdateImageSizeStatus( );
            }
        }

        // Update status of menu items in Settings->Image view
        private void imageviewToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            normalToolStripMenuItem.Checked   = ( pictureBox.SizeMode == PictureBoxSizeMode.Normal );
            centerToolStripMenuItem.Checked   = ( pictureBox.SizeMode == PictureBoxSizeMode.CenterImage );
            stretchToolStripMenuItem.Checked  = ( pictureBox.SizeMode == PictureBoxSizeMode.StretchImage );
            autoSizeToolStripMenuItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.AutoSize );
        }

        // Set Normal view for images
        private void normalToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.Normal );
        }

        // Set Centred view for images
        private void centerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.CenterImage );
        }

        // Set Stretched view for images
        private void stretchToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.StretchImage );
        }

        // Set Auto Size view for image
        private void autoSizeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.AutoSize );
        }

        // Set size mode for picture box
        private void SetPictureBoxSizeMode( PictureBoxSizeMode sizeMode )
        {
            if ( sizeMode == PictureBoxSizeMode.AutoSize )
            {
                pictureBox.Dock = DockStyle.None;
                pictureBox.Location = new System.Drawing.Point( 0, 0 );
                splitContainer2.Panel1.AutoScroll = true;
            }
            else
            {
                pictureBox.Dock = DockStyle.Fill;
                splitContainer2.Panel1.AutoScroll = false;
            }

            pictureBox.SizeMode = sizeMode;
        }

        // Switch option for openning last folder on application load
        private void openLastFolderOnStartToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openLastFolderOnStartToolStripMenuItem.Checked = !openLastFolderOnStartToolStripMenuItem.Checked;
        }

        // Show about form
        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }

        // Copy current image to clipboard
        private void copyImageClipboardToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                Clipboard.SetImage( pictureBox.Image );
            }
        }

        // Update status of "copy to clipboard" menu item
        private void toolsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            copyImageClipboardToolStripMenuItem.Enabled = ( pictureBox.Image != null );
        }

        // Show histogram window
        private void showhistogramToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( histogramForm == null )
            {
                histogramForm = new HistogramForm( );
                histogramForm.FormClosing += delegate( object eventSender, FormClosingEventArgs eventArgs )
                {
                    histogramForm = null;
                };
            }

            histogramForm.Show( );
            ShowCurrentImageHistogram( );
        }

        // Show histomgram for 
        private void ShowCurrentImageHistogram( )
        {
            if ( ( pictureBox.Image != null ) && ( histogramForm != null ) )
            {
                histogramForm.SetImageStatistics( new ImageStatistics( (Bitmap) pictureBox.Image ) );
            }
        }

        // Update status label displaying number of available images
        private void UpdateImageCountStatus( )
        {
            imagesCountLabel.Text = "Images: " + filesListView.Items.Count.ToString( );
        }

        // Update staus label displaying processing time of last routine
        private void UpdateProcessingTimeStatus( long msTime )
        {
            double sTime = (double) msTime / 1000;

            processingTimeLabel.Text = string.Format( "Processing time: {0}s", sTime.ToString( "F3" ) );
        }

        // Update status label displaying size of selected image
        private void UpdateImageSizeStatus( )
        {
            if ( pictureBox.Image != null )
            {
                imageSizeLabel.Text = string.Format( "Image size: {0}x{1}",
                    pictureBox.Image.Width, pictureBox.Image.Height );
            }
            else
            {
                imageSizeLabel.Text = string.Empty;
            }
        }

        // Resize columns of file list view and log list view
        private void mainSplitContainer_Panel1_Resize( object sender, EventArgs e )
        {
            if ( fileNameColumn != null )
            {
                fileNameColumn.Width = filesListView.Width - 24;
            }
            if ( processingStepsColumn != null )
            {
                processingStepsColumn.Width = logListView.Width - 24;
            }
        }
    }
}
