﻿// Accord Debugging Visualizers
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Darren Schroeder, 2017
// https://github.com/fdncred
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
// 
// Copyright © Jaex
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

using System.Windows.Forms;

namespace Accord.DebuggerVisualizers
{
    public static class Extensions
    {
        public static int Between(this int num, int min, int max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static float Between(this float num, float min, float max)
        {
            if (num <= min) return min;
            if (num >= max) return max;
            return num;
        }

        public static void HideImageMargin(this ToolStripDropDownItem tsddi)
        {
            ((ToolStripDropDownMenu)tsddi.DropDown).ShowImageMargin = false;
        }

        public static bool IsValidImage(this PictureBox pb)
        {
            return pb.Image != null && pb.Image != pb.InitialImage && pb.Image != pb.ErrorImage;
        }

        public static bool IsValidImage(this Accord.Controls.Cyotek.ImageBox pb)
        {
            return pb.Image != null;
        }
    }
}