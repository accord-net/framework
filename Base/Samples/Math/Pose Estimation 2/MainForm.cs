// 3D Pose Estimation (2) sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;

using AForge;
using AForge.Math;
using AForge.Math.Geometry;

namespace PoseEstimation
{
    public partial class MainForm : Form
    {
        private readonly AForge.Point emptyPoint = new AForge.Point( -30000, -30000 );

        // image point of the object to estimate pose for
        private AForge.Point[] imagePoints = new AForge.Point[4];
        // model points
        private Vector3[] modelPoints = new Vector3[4];
        // camera's focal length
        private float focalLength;
        // estimated transformation
        private Matrix3x3 rotationMatrix, bestRotationMatrix, alternateRotationMatrix;
        private Vector3 translationVector, bestTranslationVector, alternateTranslationVector;
        private bool isPoseEstimated = false;
        private float modelRadius;

        // size of the opened image
        private Size imageSize = new Size( 0, 0 );

        // colors used to highlight points on image
        private Color[] pointsColors = new Color[4]
        {
            Color.Yellow, Color.Blue, Color.Red, Color.Lime
        };

        private bool useCoplanarPosit = false;

        // point index currently locating with mouse
        private int pointIndexToLocate = -1;
        private AForge.Point pointPreviousValue;

        // model used to draw coordinate system's axes
        private Vector3[] axesModel = new Vector3[]
        {
            new Vector3( 0, 0, 0 ),
            new Vector3( 1, 0, 0 ),
            new Vector3( 0, 1, 0 ),
            new Vector3( 0, 0, 1 ),
        };

        // a structure describing built-in sample
        private struct Sample
        {
            public readonly string ImageName;
            public readonly AForge.Point[] ImagePoints;
            public readonly Vector3[] ModelPoints;
            public readonly float FocalLength;
            public readonly bool IsCoplanar;

            public Sample( string imageName, AForge.Point[] imagePoints, Vector3[] modelPoints, float focalLength, bool isCoplanar )
            {
                ImageName   = imageName;
                ImagePoints = imagePoints;
                ModelPoints = modelPoints;
                FocalLength = focalLength;
                IsCoplanar  = isCoplanar;
            }
        }

        #region Embedded samples
        private Sample[] samples = new Sample[]
        {
            // -----
            new Sample(
                "sample1.jpg",
                new AForge.Point[]
                {
                    new AForge.Point( -4, 29 ),
                    new AForge.Point( -180, 86 ),
                    new AForge.Point( -5, -102 ),
                    new AForge.Point( 76, 137 ),
                },
                new Vector3[]
                {
                    new Vector3(  28,  28, -28 ),
                    new Vector3( -28,  28, -28 ),
                    new Vector3(  28, -28, -28 ),
                    new Vector3(  28,  28,  28 ),
                },
                640, false ),

            // -----
            new Sample(
                "sample2.jpg",
                new AForge.Point[]
                {
                    new AForge.Point( -11, 58 ),
                    new AForge.Point( -125, 84 ),
                    new AForge.Point( -7, -35 ),
                    new AForge.Point( 37, 124 ),
                },
                new Vector3[]
                {
                    new Vector3(  28,  28, -28 ),
                    new Vector3( -28,  28, -28 ),
                    new Vector3(  28, -28, -28 ),
                    new Vector3(  28,  28,  28 ),
                },
                640, false ),

            // -----
            new Sample(
                "sample3.jpg",
                new AForge.Point[]
                {
                    new AForge.Point( 4, 55 ),
                    new AForge.Point( -80, 81 ),
                    new AForge.Point( 3, -8 ),
                    new AForge.Point( 40, 109 ),
                },
                new Vector3[]
                {
                    new Vector3(  28,  28, -28 ),
                    new Vector3( -28,  28, -28 ),
                    new Vector3(  28, -28, -28 ),
                    new Vector3(  28,  28,  28 ),
                },
                640, false ),

            // -----
            new Sample(
                "sample4.jpg",
                new AForge.Point[]
                {
                    new AForge.Point( -77, 48 ),
                    new AForge.Point( 44, 66 ),
                    new AForge.Point( 75, -36 ),
                    new AForge.Point( -61, -58 ),
                },
                new Vector3[]
                {
                    new Vector3( -56.5f, 0,  56.5f ),
                    new Vector3(  56.5f, 0,  56.5f ),
                    new Vector3(  56.5f, 0, -56.5f ),
                    new Vector3( -56.5f, 0, -56.5f ),
                },
                640, true ),

            // -----
            new Sample(
                "sample5.jpg",
                new AForge.Point[]
                {
                    new AForge.Point( -117, 33 ),
                    new AForge.Point( -15, 86 ),
                    new AForge.Point( 89, 38 ),
                    new AForge.Point( -13, -30 ),
                },
                new Vector3[]
                {
                    new Vector3( -56.5f, 0,  56.5f ),
                    new Vector3(  56.5f, 0,  56.5f ),
                    new Vector3(  56.5f, 0, -56.5f ),
                    new Vector3( -56.5f, 0, -56.5f ),
                },
                640, true ),
        };
        #endregion

        // Class constructor
        public MainForm( )
        {
            InitializeComponent( );
            EnableControls( false );
            UpdatePictureBoxPositon( );

            imagePoint1ColorLabel.BackColor = pointsColors[0];
            imagePoint2ColorLabel.BackColor = pointsColors[1];
            imagePoint3ColorLabel.BackColor = pointsColors[2];
            imagePoint4ColorLabel.BackColor = pointsColors[3];

            if ( useCoplanarPosit )
            {
                copositRadio.Checked = true;
            }
            else
            {
                positRadio.Checked = true;
            }

            imagePoints[0] = emptyPoint;
            imagePoints[1] = emptyPoint;
            imagePoints[2] = emptyPoint;
            imagePoints[3] = emptyPoint;

            ClearEstimation( );
        }

        // On File->Exit - close the application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }
        
        // On File->Open - open an image file
        private void openImageToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                try
                {
                    OpenImage( (Bitmap) Bitmap.FromFile( openFileDialog.FileName ) );

                    // reset image points
                    imagePoints[0] = emptyPoint;
                    imagePoints[1] = emptyPoint;
                    imagePoints[2] = emptyPoint;
                    imagePoints[3] = emptyPoint;

                    imagePoint1Box.Text =
                    imagePoint2Box.Text =
                    imagePoint3Box.Text =
                    imagePoint4Box.Text = string.Empty;

                    // clear current pose estimation
                    ClearEstimation( );

                    // set default focal length to image width
                    focalLength = imageSize.Width;
                    focalLengthBox.Text = focalLength.ToString( CultureInfo.InvariantCulture );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "Failed opening selected file.\n\nException: " + ex.Message );
                }
            }
        }

        // Open one of the embedded samples
        private void openSampleToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem) sender;
            int sampleIndex = int.Parse( (string) menuItem.Tag );

            Sample sample = samples[sampleIndex];

            OpenEmbeddedImage( sample.ImageName );

            // set image points
            imagePoints = (AForge.Point[]) sample.ImagePoints.Clone( );

            imagePoint1Box.Text = imagePoints[0].ToString( );
            imagePoint2Box.Text = imagePoints[1].ToString( );
            imagePoint3Box.Text = imagePoints[2].ToString( );
            imagePoint4Box.Text = imagePoints[3].ToString( );

            // set model points
            modelPoints = (Vector3[]) sample.ModelPoints.Clone( );

            modelPoint1xBox.Text = modelPoints[0].X.ToString( );
            modelPoint1yBox.Text = modelPoints[0].Y.ToString( );
            modelPoint1zBox.Text = modelPoints[0].Z.ToString( );

            modelPoint2xBox.Text = modelPoints[1].X.ToString( );
            modelPoint2yBox.Text = modelPoints[1].Y.ToString( );
            modelPoint2zBox.Text = modelPoints[1].Z.ToString( );

            modelPoint3xBox.Text = modelPoints[2].X.ToString( );
            modelPoint3yBox.Text = modelPoints[2].Y.ToString( );
            modelPoint3zBox.Text = modelPoints[2].Z.ToString( );

            modelPoint4xBox.Text = modelPoints[3].X.ToString( );
            modelPoint4yBox.Text = modelPoints[3].Y.ToString( );
            modelPoint4zBox.Text = modelPoints[3].Z.ToString( );

            // set focal length
            focalLength = sample.FocalLength;
            focalLengthBox.Text = focalLength.ToString( );

            // POSIT or Coplanar POSIT
            useCoplanarPosit = sample.IsCoplanar;
            if ( useCoplanarPosit )
            {
                copositRadio.Checked = true;
            }
            else
            {
                positRadio.Checked = true;
            }

            errorProvider.Clear( );

            EstimatePose( );
        }

        // Enable/disable controls which are available when image is opened
        private void EnableControls( bool enable )
        {
            imagePointsGroupBox.Enabled = enable;
            modelPointsGroupBox.Enabled = enable;
            poseGroupBox.Enabled = enable;
        }

        // Close current image
        private void CloseImage( )
        {
            pictureBox.Image = null;
            EnableControls( false );
        }

        // Open image which is embedded into the assembly as resource
        private void OpenEmbeddedImage( string imageName )
        {
            // load arrow bitmap
            Assembly assembly = this.GetType( ).Assembly;
            Bitmap image = new Bitmap( assembly.GetManifestResourceStream( "PoseEstimation.Samples." + imageName ) );
            OpenImage( image );
        }

        // Opens the specified image
        private void OpenImage( Bitmap image )
        {
            // close previous image if any
            CloseImage( );

            // open new image
            imageSize = image.Size;

            pictureBox.Image = image;
            pictureBox.Size = new Size( imageSize.Width + 2, imageSize.Height + 2 );
            imageSizeLabel.Text = string.Format( "{0} x {1}", image.Width, image.Height );

            ClearEstimation( );
            UpdatePictureBoxPositon( );
            EnableControls( true );
        }

        private void ClearEstimation( )
        {
            isPoseEstimated = false;
            estimatedTransformationMatrixControl.Clear( );
            bestPoseButton.Visible = false;
            alternatePoseButton.Visible = false;
        }

        // Paint image points on the image
        private void pictureBox_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            if ( pictureBox.Image != null )
            {
                int cx = imageSize.Width  / 2;
                int cy = imageSize.Height / 2;

                for ( int i = 0; i < 4; i++ )
                {
                    if ( imagePoints[i] != emptyPoint )
                    {
                        using ( Brush brush = new SolidBrush( pointsColors[i] ) )
                        {
                            g.FillEllipse( brush, new Rectangle(
                                (int) ( cx + imagePoints[i].X - 3 ),
                                (int) ( cy - imagePoints[i].Y - 3 ),
                                7, 7 ) );
                        }
                    }
                }

                if ( ( isPoseEstimated ) && ( pointIndexToLocate == -1 ) )
                {
                    AForge.Point[] projectedAxes = PerformProjection( axesModel,
                        // create tranformation matrix
                        Matrix4x4.CreateTranslation( translationVector ) *       // 3: translate
                        Matrix4x4.CreateFromRotation( rotationMatrix ) *         // 2: rotate
                        Matrix4x4.CreateDiagonal(
                            new Vector4( modelRadius, modelRadius, modelRadius, 1 ) ), // 1: scale
                        imageSize.Width
                    );

                    using ( Pen pen = new Pen( Color.Blue, 5 ) )
                    {
                        g.DrawLine( pen,
                            cx + projectedAxes[0].X, cy - projectedAxes[0].Y,
                            cx + projectedAxes[1].X, cy - projectedAxes[1].Y );
                    }

                    using ( Pen pen = new Pen( Color.Red, 5 ) )
                    {
                        g.DrawLine( pen,
                            cx + projectedAxes[0].X, cy - projectedAxes[0].Y,
                            cx + projectedAxes[2].X, cy - projectedAxes[2].Y );
                    }

                    using ( Pen pen = new Pen( Color.Lime, 5 ) )
                    {
                        g.DrawLine( pen,
                            cx + projectedAxes[0].X, cy - projectedAxes[0].Y,
                            cx + projectedAxes[3].X, cy - projectedAxes[3].Y );
                    }
                }
            }
        }

        private AForge.Point[] PerformProjection( Vector3[] model, Matrix4x4 transformationMatrix, int viewSize )
        {
            AForge.Point[] projectedPoints = new AForge.Point[model.Length];

            for ( int i = 0; i < model.Length; i++ )
            {
                Vector3 scenePoint = ( transformationMatrix * model[i].ToVector4( ) ).ToVector3( );

                projectedPoints[i] = new AForge.Point(
                    (int) ( scenePoint.X / scenePoint.Z * viewSize ),
                    (int) ( scenePoint.Y / scenePoint.Z * viewSize ) );
            }

            return projectedPoints;
        }

        // Update position of picture box so it is centered in the main panel
        private void UpdatePictureBoxPositon( )
        {
            pictureBox.Location = new System.Drawing.Point(
                ( mainPanel.Width - pictureBox.Width ) / 2,
                ( mainPanel.Height - pictureBox.Height ) / 2 );
        }

        // On resize of main form
        private void MainForm_Resize( object sender, EventArgs e )
        {
            UpdatePictureBoxPositon( );
        }

        // One of the locate point button were clicked
        private void locatePointButton_Click( object sender, EventArgs e )
        {
            pictureBox.Capture = true;

            Button sourceButton = (Button) sender;
            pointIndexToLocate = int.Parse( (string) sourceButton.Tag );

            pointPreviousValue = imagePoints[pointIndexToLocate];
            imagePoints[pointIndexToLocate] = emptyPoint;

            statusLabel.Text = string.Format( "Locate point #{0} in the image ...", pointIndexToLocate + 1 );
            pictureBox.Invalidate( );
        }

        // Mouse click on the image - accept new point or reject it (depending on mouse button)
        private void pictureBox_MouseClick( object sender, MouseEventArgs e )
        {
            if ( pointIndexToLocate != -1 )
            {
                pictureBox.Cursor = Cursors.Default;
                pictureBox.Capture = false;
                statusLabel.Text = string.Empty;

                if ( e.Button == MouseButtons.Left )
                {
                    int x = Math.Max( 0, Math.Min( e.X, imageSize.Width - 1 ) );
                    int y = Math.Max( 0, Math.Min( e.Y, imageSize.Height - 1 ) );

                    imagePoints[pointIndexToLocate] = new AForge.Point( x - imageSize.Width / 2, imageSize.Height / 2 - y );

                    TextBox imagePointTextBox = (TextBox) imagePointsGroupBox.Controls[string.Format( "imagePoint{0}Box", pointIndexToLocate + 1 )];
                    imagePointTextBox.Text = imagePoints[pointIndexToLocate].ToString( );
                }
                else
                {
                    imagePoints[pointIndexToLocate] = pointPreviousValue;
                }

                ClearEstimation( );

                pointIndexToLocate = -1;
                pictureBox.Invalidate( );
            }
        }

        private void pictureBox_MouseMove( object sender, MouseEventArgs e )
        {
            if ( pointIndexToLocate != -1 )
            {
                pictureBox.Cursor = Cursors.Help;               
            }
        }

        // Leaving one of the model point's boxes - validate it
        private void modelPointBox_Leave( object sender, EventArgs e )
        {
            GetCoordinateValue( (TextBox) sender );
        }

        private void GetCoordinateValue( TextBox textBox )
        {
            int tag = int.Parse( (string) textBox.Tag );
            int pointIndex = tag / 10;
            int coordinateIndex = tag % 10;
            float coordinateValue, oldValue = 0;

            textBox.Text = textBox.Text.Trim( );

            // try parsing the coordinate value
            if ( float.TryParse( textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out coordinateValue) )
            {
                switch ( coordinateIndex )
                {
                    case 0:
                        oldValue = modelPoints[pointIndex].X;
                        modelPoints[pointIndex].X = coordinateValue;
                        break;
                    case 1:
                        oldValue = modelPoints[pointIndex].Y;
                        modelPoints[pointIndex].Y = coordinateValue;
                        break;
                    case 2:
                        oldValue = modelPoints[pointIndex].Z;
                        modelPoints[pointIndex].Z = coordinateValue;
                        break;
                }
                errorProvider.Clear( );

                if ( oldValue != coordinateValue )
                {
                    ClearEstimation( );
                }
            }
            else
            {
                Label pointLabel = (Label) modelPointsGroupBox.Controls[string.Format( "modelPoint{0}Label", pointIndex + 1 )];

                errorProvider.SetError( pointLabel, string.Format( "Failed parsing {0} coordinate",
                    ( coordinateIndex == 0 ) ? "X" : ( ( coordinateIndex == 1 ) ? "Y" : "Z" ) ) );

                textBox.Text = string.Empty;
            }
        }

        // Validate focal length on leaving the text box
        private void focalLengthBox_Leave( object sender, EventArgs e )
        {
            float value;

            if ( float.TryParse( focalLengthBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value ) )
            {
                focalLength = value;
            }
            else
            {
                focalLengthBox.Text = focalLength.ToString( );
                errorProvider.SetError( focalLengthLabel, "Wrong focal length was specified. Restored to previous value." );
            }
        }

        // Switch between POSIT and CoPOSIT algorithms
        private void copositRadio_CheckedChanged( object sender, EventArgs e )
        {
            useCoplanarPosit = copositRadio.Checked;
        }
        
        private void estimatePostButton_Click( object sender, EventArgs e )
        {
            EstimatePose( );
        }

        // Estimate 3D position
        private void EstimatePose( )
        {
            try
            {
                // check if all image coordinates are specified
                if ( ( string.IsNullOrEmpty( imagePoint1Box.Text ) ) ||
                     ( string.IsNullOrEmpty( imagePoint2Box.Text ) ) ||
                     ( string.IsNullOrEmpty( imagePoint3Box.Text ) ) ||
                     ( string.IsNullOrEmpty( imagePoint4Box.Text ) ) )
                {
                    throw new ApplicationException( "Some image coordinates are not specified." );
                }

                // check if all model coordnates are specified
                if ( ( string.IsNullOrEmpty( modelPoint1xBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint2xBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint3xBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint4xBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint1yBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint2yBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint3yBox.Text ) ) ||
                     ( string.IsNullOrEmpty( modelPoint4yBox.Text ) ) ||
                     ( ( !useCoplanarPosit ) && (
                       ( string.IsNullOrEmpty( modelPoint1zBox.Text ) ) ||
                       ( string.IsNullOrEmpty( modelPoint2zBox.Text ) ) ||
                       ( string.IsNullOrEmpty( modelPoint3zBox.Text ) ) ||
                       ( string.IsNullOrEmpty( modelPoint4zBox.Text ) ) ) ) )
                {
                    throw new ApplicationException( "Some model coordinates are not specified." );
                }

                // calculate model's center
                Vector3 modelCenter = new Vector3(
                    ( modelPoints[0].X + modelPoints[1].X + modelPoints[2].X + modelPoints[3].X ) / 4,
                    ( modelPoints[0].Y + modelPoints[1].Y + modelPoints[2].Y + modelPoints[3].Y ) / 4,
                    ( modelPoints[0].Z + modelPoints[1].Z + modelPoints[2].Z + modelPoints[3].Z ) / 4
                );

                // calculate ~ model's radius
                modelRadius = 0;
                foreach ( Vector3 modelPoint in modelPoints )
                {
                    float distanceToCenter = ( modelPoint - modelCenter ).Norm;
                    if ( distanceToCenter > modelRadius )
                    {
                        modelRadius = distanceToCenter;
                    }
                }

                if ( !useCoplanarPosit )
                {
                    Posit posit = new Posit( modelPoints, focalLength );
                    posit.EstimatePose( imagePoints, out rotationMatrix, out translationVector );

                    bestPoseButton.Visible = alternatePoseButton.Visible = false;
                }
                else
                {
                    CoplanarPosit coposit = new CoplanarPosit( modelPoints, focalLength );
                    coposit.EstimatePose( imagePoints, out rotationMatrix, out translationVector );

                    bestRotationMatrix = coposit.BestEstimatedRotation;
                    bestTranslationVector = coposit.BestEstimatedTranslation;

                    alternateRotationMatrix = coposit.AlternateEstimatedRotation;
                    alternateTranslationVector = coposit.AlternateEstimatedTranslation;

                    bestPoseButton.Visible = alternatePoseButton.Visible = true;
                }

                isPoseEstimated = true;
                UpdateEstimationInformation( );
                pictureBox.Invalidate( );
            }
            catch ( ApplicationException ex )
            {
                MessageBox.Show( ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void UpdateEstimationInformation( )
        {
            estimatedTransformationMatrixControl.SetMatrix(
                Matrix4x4.CreateTranslation( translationVector ) *
                Matrix4x4.CreateFromRotation( rotationMatrix ) );

            float estimatedYaw;
            float estimatedPitch;
            float estimatedRoll;

            rotationMatrix.ExtractYawPitchRoll( out estimatedYaw, out estimatedPitch, out estimatedRoll );

            estimatedYaw   *= (float) ( 180.0 / Math.PI );
            estimatedPitch *= (float) ( 180.0 / Math.PI );
            estimatedRoll  *= (float) ( 180.0 / Math.PI );

            estimationLabel.Text = string.Format( "Rotation: (yaw(y)={0}, pitch(x)={1}, roll(z)={2})",
                estimatedYaw, estimatedPitch, estimatedRoll );
        }

        // Select best pose estimation
        private void bestPoseButton_Click( object sender, EventArgs e )
        {
            rotationMatrix = bestRotationMatrix;
            translationVector = bestTranslationVector;

            UpdateEstimationInformation( );
            pictureBox.Invalidate( );
        }

        // Select alternate pose estimation
        private void alternatePoseButton_Click( object sender, EventArgs e )
        {
            rotationMatrix = alternateRotationMatrix;
            translationVector = alternateTranslationVector;

            UpdateEstimationInformation( );
            pictureBox.Invalidate( );
        }
    }
}
