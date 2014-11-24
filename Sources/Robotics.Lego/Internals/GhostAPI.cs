// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.Internals
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    /// <summary>
    /// GhostAPI wrapper class.
    /// </summary>
    /// 
    /// <remarks><para>GhostAPI is a library provided by Lego, to communicate with its
    /// RCX robotics kit.</para></remarks>
    /// 
    internal static class GhostAPI
    {
        /// <summary>
        /// Creates Ghost communication stack.
        /// </summary>
        /// 
        /// <param name="port">Port implementation required.</param>
        /// <param name="protocol">Protocol implementation required.</param>
        /// <param name="session">Session implementation required.</param>
        /// <param name="stack">Stack handle created by this function.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhCreateStack(
            string port,
            string protocol,
            string session,
            out IntPtr stack );

        /// <summary>
        /// Finds and selects the first available device.
        /// </summary>
        /// 
        /// <param name="stack">Stack handle.</param>
        /// <param name="deviceName">Buffer, which will be filled with the name of the selected port device.</param>
        /// <param name="bufferSize">Size of the specified buffer.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhSelectFirstDevice(
            IntPtr stack,
            [In, MarshalAs( UnmanagedType.LPStr )] StringBuilder deviceName,
            int bufferSize
        );

        /// <summary>
        /// Opens the currently selected device.
        /// </summary>
        /// 
        /// <param name="stack">Handle of the stack to be opened.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhOpen( IntPtr stack );

        /// <summary>
        /// Closes the currently selected device.
        /// </summary>
        /// 
        /// <param name="stack">Handle of the stack to be closed.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhClose( IntPtr stack );

        /// <summary>
        /// Sets the current command interleave between the execute and download queue.
        /// </summary>
        /// 
        /// <param name="stack">Stack handle.</param>
        /// <param name="interleaveExecute">Number of immediate command blocks.</param>
        /// <param name="interleaveDownload">Number of download slices.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhSetInterleave(
            IntPtr stack,
            int interleaveExecute,
            int interleaveDownload
        );

        /// <summary>
        /// Set the current notification mode to WAIT.
        /// </summary>
        /// 
        /// <param name="stack">Stack handle.</param>
        /// <param name="notify">Must be <b>IntPtr.Zero</b> - no callback function.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhSetWaitMode(
            IntPtr stack,
            IntPtr notify
        );

        /// <summary>
        /// Creates a command queue (containing one command to start with) and return handle.
        /// </summary>
        /// 
        /// <param name="queue">Queue handle created by this function.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhCreateCommandQueue( out IntPtr queue );

        /// <summary>
        /// Releases a command queue.
        /// </summary>
        /// 
        /// <param name="queue">Queue handle.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhDestroyCommandQueue( IntPtr queue );

        /// <summary>
        /// Appends a command to the given command queue.
        /// </summary>
        /// 
        /// <param name="queue"> Queue handle.</param>
        /// <param name="commandData">Command buffer (command + parameters).</param>
        /// <param name="commandLen">Length of the command buffer.</param>
        /// <param name="expectedReplyLen">Length of the expected reply to this command.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhAppendCommand(
            IntPtr queue,
            byte[] commandData,
            int commandLen,
            int expectedReplyLen
        );

        /// <summary>
        /// Submits a command queue on the EXECUTE queue.
        /// </summary>
        /// 
        /// <param name="stack">Stack handle.</param>
        /// <param name="queue">Queue handle.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhExecute( IntPtr stack, IntPtr queue );

        /// <summary>
        /// Gets the first command in the queue.
        /// </summary>
        /// <param name="queue">Queue handle.</param>
        /// <param name="command">Retrieved command handle.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhGetFirstCommand( IntPtr queue, out IntPtr command );

        /// <summary>
        /// Gets command reply length.
        /// </summary>
        /// 
        /// <param name="command">Command handle.</param>
        /// <param name="replyLen">Reply length.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhGetCommandReplyLen( IntPtr command, out uint replyLen );

        /// <summary>
        /// Gets command reply.
        /// </summary>
        /// 
        /// <param name="command">Command handle.</param>
        /// <param name="replyData">Buffer for reply data.</param>
        /// <param name="bufSize">Buffer size.</param>
        /// 
        /// <returns>GhostAPI error code.</returns>
        /// 
        [DllImport( "GhostAPI.dll" )]
        public extern static uint GhGetCommandReply(
            IntPtr command,
            byte[] replyData,
            uint bufSize );


        /// <summary>
        /// Mask of severity bits.
        /// </summary>
        const uint PBKERR_SEVERITYBITS              = 0xC0000000;

        /// <summary>
        /// Success severity.
        /// </summary>
        const uint PBKERR_SEVERITY_SUCCESS          = 0x00000000;

        /// <summary>
        /// Informational severity.
        /// </summary>
        const uint PBKERR_SEVERITY_INFORMATIONAL    = 0x40000000;

        /// <summary>
        /// Warning severity.
        /// </summary>
        const uint PBKERR_SEVERITY_WARNING          = 0x80000000;

        /// <summary>
        /// Error severity.
        /// </summary>
        const uint PBKERR_SEVERITY_ERROR            = 0xC0000000;

        /// <summary>
        /// Checks if return value has success severity.
        /// </summary>
        /// 
        /// <param name="e">Error return value.</param>
        /// 
        /// <returns>True if the return value has success severity.</returns>
        /// 
        public static bool PBK_IS_SUCCESS( uint e ) { return ( ( e & PBKERR_SEVERITYBITS ) == PBKERR_SEVERITY_SUCCESS ); }

        /// <summary>
        /// Checks if return value has info severity.
        /// </summary>
        /// 
        /// <param name="e">Error return value.</param>
        /// 
        /// <returns>True if the return value has info severity.</returns>
        /// 
        public static bool PBK_IS_INFO( uint e ) { return ( ( e & PBKERR_SEVERITYBITS ) == PBKERR_SEVERITY_INFORMATIONAL ); }

        /// <summary>
        /// Checks if return value has warning severity.
        /// </summary>
        /// 
        /// <param name="e">Error return value.</param>
        /// 
        /// <returns>True if the return value has warning severity.</returns>
        /// 
        public static bool PBK_IS_WARNING( uint e ) { return ( ( e & PBKERR_SEVERITYBITS ) == PBKERR_SEVERITY_WARNING ); }

        /// <summary>
        /// Checks if return value has error severity.
        /// </summary>
        /// 
        /// <param name="e">Error return value.</param>
        /// 
        /// <returns>True if the return value has error severity.</returns>
        /// 
        public static bool PBK_IS_ERROR( uint e ) { return ( ( e & PBKERR_SEVERITYBITS ) == PBKERR_SEVERITY_ERROR ); }

        /// <summary>
        /// Checks for successful return code, which has success or info severity.
        /// </summary>
        /// 
        /// <param name="e">Error return value.</param>
        /// 
        /// <returns>True if return value is successful.</returns>
        /// 
        public static bool PBK_SUCCEEDED( uint e ) { return ( PBK_IS_SUCCESS( e ) || PBK_IS_INFO( e ) ); }

    }
}
