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

#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace Accord::Video;
using namespace Accord::Math;

// Forward declarations
namespace libffmpeg {
    struct AVPacketList;
    struct AVPacket;
}

namespace Accord {
    namespace Video {
        namespace FFMPEG
        {
            private ref struct PacketQueue
            {
                libffmpeg::AVPacketList* first_pkt;
                libffmpeg::AVPacketList* last_pkt;
                int nb_packets;
                int size;
                System::Object^ mutex;
                System::Threading::ManualResetEvent^ cond;

            public:
                PacketQueue();

                int packet_queue_put(libffmpeg::AVPacket* pkt);

                int packet_queue_get(libffmpeg::AVPacket* pkt, int block);
            };

        }
    }
}
