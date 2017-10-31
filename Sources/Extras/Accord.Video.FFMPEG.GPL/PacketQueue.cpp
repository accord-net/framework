// Accord FFMPEG Library
// The Accord.NET Framework
// http://accord-framework.net
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
#include "PacketQueue.h"

#include <string>

namespace libffmpeg
{
    extern "C"
    {
#include "libavformat\avformat.h"
#include "libavformat\avio.h"
#include "libavcodec\avcodec.h"
#include "libswscale\swscale.h"
    }
}


namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            PacketQueue::PacketQueue()
            {
                this->mutex = gcnew System::Object();
                this->cond = gcnew System::Threading::ManualResetEvent(true);
            }

            int PacketQueue::packet_queue_put(libffmpeg::AVPacket *pkt)
            {
                libffmpeg::AVPacketList *pkt1;
                if (av_dup_packet(pkt) < 0) {
                    return -1;
                }

                pkt1 = (libffmpeg::AVPacketList*)libffmpeg::av_malloc(sizeof(libffmpeg::AVPacketList));
                if (!pkt1)
                    return -1;
                pkt1->pkt = *pkt;
                pkt1->next = NULL;

                System::Threading::Monitor::Enter(this->mutex);

                if (!this->last_pkt)
                    this->first_pkt = pkt1;
                else
                    this->last_pkt->next = pkt1;
                this->last_pkt = pkt1;
                this->nb_packets++;
                this->size += pkt1->pkt.size;

                this->cond->Set();

                System::Threading::Monitor::Exit(this->mutex);

                return 0;
            }

            int PacketQueue::packet_queue_get(libffmpeg::AVPacket *pkt, int block)
            {
                libffmpeg::AVPacketList *pkt1;
                int ret;

                System::Threading::Monitor::Enter(this->mutex);

                for (;;) {

                    /*if (global_video_state->quit) {
                    ret = -1;
                    break;
                    }*/

                    pkt1 = this->first_pkt;
                    if (pkt1)
                    {
                        this->first_pkt = pkt1->next;
                        if (!this->first_pkt)
                            this->last_pkt = NULL;
                        this->nb_packets--;
                        this->size -= pkt1->pkt.size;
                        *pkt = pkt1->pkt;
                        av_free(pkt1);
                        ret = 1;
                        break;
                    }
                    else if (!block)
                    {
                        ret = 0;
                        break;
                    }
                    else
                    {
                        this->cond->WaitOne();
                    }
                }

                System::Threading::Monitor::Exit(this->mutex);

                return ret;
            }
        };

    }
}
