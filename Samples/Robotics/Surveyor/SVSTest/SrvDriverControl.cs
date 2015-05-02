// Surveyor SVS test application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using AForge.Robotics.Surveyor;

namespace SVSTest
{
    public delegate void SrvDrivingCommandHandler( object sender, SRV1.MotorCommand command );

    public partial class SrvDriverControl : UserControl
    {
        public event SrvDrivingCommandHandler SrvDrivingCommand;

        public SrvDriverControl( )
        {
            InitializeComponent( );
        }

        private void DispatchCommand( SRV1.MotorCommand command )
        {
            if ( SrvDrivingCommand != null )
            {
                SrvDrivingCommand( this, command );
            }
        }

        private void leftButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveLeft );
        }

        private void forwardButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveForward );
        }

        private void rightButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveRight );
        }

        private void leftDriftButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriftLeft );
        }

        private void stopButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.Stop );
        }

        private void rightDriftButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriftRight );
        }

        private void leftBackwardButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveBackLeft );
        }

        private void backwardButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveBack );
        }

        private void rightBackwardButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DriveBackRight );
        }

        private void speedUpButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.IncreaseSpeed );
        }

        private void slowDownButton_Click( object sender, EventArgs e )
        {
            DispatchCommand( SRV1.MotorCommand.DecreaseSpeed );
        }
    }
}
