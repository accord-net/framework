// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//
// Copyright © MelvinGr, 2016-2017
// https://github.com/MelvinGr
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

#include "StdAfx.h"
#include "VideoFileSource.h"
#include "VideoFileReader.h"

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            VideoFileSource::VideoFileSource(String^ fileName)
            {
                m_fileName = fileName;
                m_workerThread = nullptr;
                m_needToStop = nullptr;

                m_frameIntervalFromSource = true;
                m_frameInterval = 0;
            }

            void VideoFileSource::Start()
            {
                if (IsRunning)
                    return;

                // check source
                if (String::IsNullOrEmpty(m_fileName))
                    throw gcnew ArgumentException("Video file name is not specified.");

                m_framesReceived = 0;
                m_bytessReceived = 0;

                // create events
                m_needToStop = gcnew ManualResetEvent(false);

                // create and start new thread
                m_workerThread = gcnew Thread(gcnew ThreadStart(this, &VideoFileSource::WorkerThreadHandler));
                m_workerThread->Name = m_fileName; // just for debugging
                m_workerThread->Start();
            }

            void VideoFileSource::SignalToStop()
            {
                if (m_workerThread != nullptr)
                {
                    // signal to stop
                    m_needToStop->Set();
                }
            }

            void VideoFileSource::WaitForStop()
            {
                if (m_workerThread != nullptr)
                {
                    // wait for thread stop
                    m_workerThread->Join();
                    Free();
                }
            }

            void VideoFileSource::Stop()
            {
                if (IsRunning)
                {
                    m_workerThread->Abort();
                    WaitForStop();
                }
            }

            void VideoFileSource::Free()
            {
                m_workerThread = nullptr;

                // release events
                m_needToStop->Close();
                m_needToStop = nullptr;
            }

            void VideoFileSource::WorkerThreadHandler()
            {
                ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying::StoppedByUser;
                VideoFileReader^ videoReader = gcnew VideoFileReader();

                try
                {
                    videoReader->Open(m_fileName);

                    // frame interval
                    int interval = m_frameIntervalFromSource
                        ? (int)(1000 / ((videoReader->FrameRate == 0) ? 25 : videoReader->FrameRate))
                        : m_frameInterval;

                    while (!m_needToStop->WaitOne(0, false))
                    {
                        // start time
                        DateTime start = DateTime::Now;

                        // get next video frame
                        Bitmap^ bitmap = videoReader->ReadVideoFrame();

                        if (bitmap == nullptr)
                        {
                            reasonToStop = ReasonToFinishPlaying::EndOfStreamReached;
                            break;
                        }

                        m_framesReceived++;
                        m_bytessReceived += bitmap->Width * bitmap->Height *
                            (Bitmap::GetPixelFormatSize(bitmap->PixelFormat) >> 3);

                        // notify clients about the new video frame
                        NewFrame(this, gcnew NewFrameEventArgs(bitmap));

                        // dispose the frame since we no longer need it
                        delete bitmap;

                        // wait for a while ?
                        if (interval > 0)
                        {
                            // get frame extract duration
                            TimeSpan^ span = DateTime::Now.Subtract(start);

                            // miliseconds to sleep
                            int msec = interval - (int)span->TotalMilliseconds;
                            if ((msec > 0) && (m_needToStop->WaitOne(msec, false) == true))
                                break;
                        }
                    }
                }
                catch (Exception^ exception)
                {
                    VideoSourceError(this, gcnew VideoSourceErrorEventArgs(exception->Message));
                }

                videoReader->Close();
                PlayingFinished(this, reasonToStop);
            }
        }
    }
}
