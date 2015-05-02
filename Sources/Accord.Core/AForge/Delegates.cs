// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// A delegate which is used by events notifying abount sent/received message.
    /// </summary>
    /// 
    /// <param name="sender">Event sender.</param>
    /// <param name="eventArgs">Event arguments containing details about the transferred message.</param>
    ///
    public delegate void MessageTransferHandler( object sender, CommunicationBufferEventArgs eventArgs );
}
