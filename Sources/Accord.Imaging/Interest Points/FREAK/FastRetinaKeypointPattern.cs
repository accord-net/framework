// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright (c) 2011-2012 LTS2, EPFL
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of the CherryPy Team nor the names of its contributors 
//      may be used to endorse or promote products derived from this software 
//      without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Imaging
{
    using Accord.Math;
    using System;
    using System.Collections.Generic;

    internal class FastRetinaKeypointPattern
    {

        /// <summary>
        ///   Pattern scale resolution.
        /// </summary>
        public const int Scales = 64;

        /// <summary>
        ///   Pattern orientation resolution.
        /// </summary>
        /// 
        public const int Orientations = 256;

        /// <summary>
        ///   Number of pattern points.
        /// </summary>
        /// 
        public const int Points = 43;

        /// <summary>
        ///   Smallest keypoint size.
        /// </summary>
        /// 
        public const int Size = 7;

        /// <summary>
        ///   Look-up table for the pattern points (position + 
        ///   sigma of all points at all scales and orientation)
        /// </summary>
        /// 
        public PatternPoint[] lookupTable;

        public int Octaves { get; private set; }
        public float Scale { get; private set; }

        public double step;

        public int[] patternSizes = new int[Scales];
        public int[] pointsValues = new int[Points];

        public DescriptionPair[] descriptionPairs = new DescriptionPair[512];
        public OrientationPair[] orientationPairs = new OrientationPair[45];


        public FastRetinaKeypointPattern(int octaves = 4, float scale = 22.0f)
        {
            this.Octaves = octaves;
            this.Scale = scale;

            lookupTable = new PatternPoint[Scales * Orientations * Points];

            double scaleStep = Math.Pow(2.0, (double)(octaves) / Scales);
            step = (float)(Scales / (Constants.Log2 * octaves));

            // pattern definition, radius normalized to 1.0 (outer point position + sigma = 1.0)

            // number of points on each concentric circle (from outer to inner)
            int[] n = { 6, 6, 6, 6, 6, 6, 6, 1 };

            double bigR = 2.0 / 3.0;    // bigger radius
            double smallR = 2.0 / 24.0; // smaller radius

            double unitSpace = ((bigR - smallR) / 21.0); // define spaces between concentric circles (from center to outer: 1,2,3,4,5,6)

            double[] radius = 
            {
                bigR, bigR - 6 * unitSpace, // radii of the concentric circles (from outer to inner)
                bigR - 11 * unitSpace, bigR - 15 * unitSpace,
                bigR - 18 * unitSpace, bigR - 20 * unitSpace,
                smallR, 0.0
            };

            double[] sigma = 
            {         
                radius[0]/2.0, radius[1]/2.0, 
                radius[2]/2.0, radius[3]/2.0,  // sigma of the pattern points (each group of 6 
                radius[4]/2.0, radius[5]/2.0,  //  points on a concentric circle has same sigma)
                radius[6]/2.0, radius[6]/2.0
            };


            // fill the lookup table
            for (int scaleIdx = 0; scaleIdx < Scales; scaleIdx++)
            {
                patternSizes[scaleIdx] = 0;
                double scalingFactor = Math.Pow(scaleStep, scaleIdx);

                for (int orientationIdx = 0; orientationIdx < Orientations; orientationIdx++)
                {
                    // orientation of the pattern
                    double theta = orientationIdx * 2 * Math.PI / Orientations;
                    int pointIdx = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < n[i]; k++)
                        {
                            // Compute orientation offset so that groups
                            // of points on each circles become staggered
                            //
                            double beta = Math.PI / n[i] * (i % 2);
                            double alpha = k * 2 * Math.PI / n[i] + beta + theta;

                            // Add the point to the pattern look-up table
                            var point = new PatternPoint(
                                (float)(radius[i] * Math.Cos(alpha) * scalingFactor * scale),
                                (float)(radius[i] * Math.Sin(alpha) * scalingFactor * scale),
                                (float)(sigma[i] * scalingFactor * scale));

                            lookupTable[scaleIdx * Orientations * Points
                                + orientationIdx * Points + pointIdx] = point;

                            // adapt the sizeList if necessary
                            int sizeMax = (int)Math.Ceiling((radius[i] + sigma[i]) * scalingFactor * scale) + 1;
                            if (patternSizes[scaleIdx] < sizeMax)
                                patternSizes[scaleIdx] = sizeMax;

                            pointIdx++;
                        }
                    }
                }
            }

            // build the list of orientation pairs
            orientationPairs[0].i = 0; orientationPairs[0].j = 3; orientationPairs[1].i = 1; orientationPairs[1].j = 4; orientationPairs[2].i = 2; orientationPairs[2].j = 5;
            orientationPairs[3].i = 0; orientationPairs[3].j = 2; orientationPairs[4].i = 1; orientationPairs[4].j = 3; orientationPairs[5].i = 2; orientationPairs[5].j = 4;
            orientationPairs[6].i = 3; orientationPairs[6].j = 5; orientationPairs[7].i = 4; orientationPairs[7].j = 0; orientationPairs[8].i = 5; orientationPairs[8].j = 1;

            orientationPairs[9].i = 6; orientationPairs[9].j = 9; orientationPairs[10].i = 7; orientationPairs[10].j = 10; orientationPairs[11].i = 8; orientationPairs[11].j = 11;
            orientationPairs[12].i = 6; orientationPairs[12].j = 8; orientationPairs[13].i = 7; orientationPairs[13].j = 9; orientationPairs[14].i = 8; orientationPairs[14].j = 10;
            orientationPairs[15].i = 9; orientationPairs[15].j = 11; orientationPairs[16].i = 10; orientationPairs[16].j = 6; orientationPairs[17].i = 11; orientationPairs[17].j = 7;

            orientationPairs[18].i = 12; orientationPairs[18].j = 15; orientationPairs[19].i = 13; orientationPairs[19].j = 16; orientationPairs[20].i = 14; orientationPairs[20].j = 17;
            orientationPairs[21].i = 12; orientationPairs[21].j = 14; orientationPairs[22].i = 13; orientationPairs[22].j = 15; orientationPairs[23].i = 14; orientationPairs[23].j = 16;
            orientationPairs[24].i = 15; orientationPairs[24].j = 17; orientationPairs[25].i = 16; orientationPairs[25].j = 12; orientationPairs[26].i = 17; orientationPairs[26].j = 13;

            orientationPairs[27].i = 18; orientationPairs[27].j = 21; orientationPairs[28].i = 19; orientationPairs[28].j = 22; orientationPairs[29].i = 20; orientationPairs[29].j = 23;
            orientationPairs[30].i = 18; orientationPairs[30].j = 20; orientationPairs[31].i = 19; orientationPairs[31].j = 21; orientationPairs[32].i = 20; orientationPairs[32].j = 22;
            orientationPairs[33].i = 21; orientationPairs[33].j = 23; orientationPairs[34].i = 22; orientationPairs[34].j = 18; orientationPairs[35].i = 23; orientationPairs[35].j = 19;

            orientationPairs[36].i = 24; orientationPairs[36].j = 27; orientationPairs[37].i = 25; orientationPairs[37].j = 28; orientationPairs[38].i = 26; orientationPairs[38].j = 29;
            orientationPairs[39].i = 30; orientationPairs[39].j = 33; orientationPairs[40].i = 31; orientationPairs[40].j = 34; orientationPairs[41].i = 32; orientationPairs[41].j = 35;
            orientationPairs[42].i = 36; orientationPairs[42].j = 39; orientationPairs[43].i = 37; orientationPairs[43].j = 40; orientationPairs[44].i = 38; orientationPairs[44].j = 41;

            for (int m = 0; m < orientationPairs.Length; m++)
            {
                float dx = lookupTable[orientationPairs[m].i].x - lookupTable[orientationPairs[m].j].x;
                float dy = lookupTable[orientationPairs[m].i].y - lookupTable[orientationPairs[m].j].y;
                float norm_sq = (dx * dx + dy * dy);
                orientationPairs[m].weight_dx = (int)((dx / (norm_sq)) * 4096.0 + 0.5);
                orientationPairs[m].weight_dy = (int)((dy / (norm_sq)) * 4096.0 + 0.5);
            }

            // build the list of description pairs
            var allPairs = new List<DescriptionPair>();
            for (int i = 1; i < Points; i++)
                for (int j = 0; j < i; j++)
                    allPairs.Add(new DescriptionPair(i, j));

            // default selected pairs
            for (int i = 0; i < descriptionPairs.Length; i++)
                descriptionPairs[i] = allPairs[CV_FREAK_DEF_PAIRS[i]];
        }



        public struct PatternPoint
        {
            public PatternPoint(float x, float y, float sigma)
            {
                this.x = x;
                this.y = y;
                this.sigma = sigma;
            }

            public float x;
            public float y;
            public float sigma;
        };

        public struct DescriptionPair
        {
            public int i; // index of the first point
            public int j; // index of the second point

            public DescriptionPair(int i, int j)
            {
                this.i = i;
                this.j = j;
            }
        };

        public struct OrientationPair
        {
            public int i; // index of the first point
            public int j; // index of the second point
            public int weight_dx; // dx/(norm_sq))*4096
            public int weight_dy; // dy/(norm_sq))*4096
        };

        static int[] CV_FREAK_DEF_PAIRS = // default pairs
        { 
            404,431,818,511,181,52,311,874,774,543,719,230,417,205,11,
            560,149,265,39,306,165,857,250,8,61,15,55,717,44,412,
            592,134,761,695,660,782,625,487,549,516,271,665,762,392,178,
            796,773,31,672,845,548,794,677,654,241,831,225,238,849,83,
            691,484,826,707,122,517,583,731,328,339,571,475,394,472,580,
            381,137,93,380,327,619,729,808,218,213,459,141,806,341,95,
            382,568,124,750,193,749,706,843,79,199,317,329,768,198,100,
            466,613,78,562,783,689,136,838,94,142,164,679,219,419,366,
            418,423,77,89,523,259,683,312,555,20,470,684,123,458,453,833,
            72,113,253,108,313,25,153,648,411,607,618,128,305,232,301,84,
            56,264,371,46,407,360,38,99,176,710,114,578,66,372,653,
            129,359,424,159,821,10,323,393,5,340,891,9,790,47,0,175,346,
            236,26,172,147,574,561,32,294,429,724,755,398,787,288,299,
            769,565,767,722,757,224,465,723,498,467,235,127,802,446,233,
            544,482,800,318,16,532,801,441,554,173,60,530,713,469,30,
            212,630,899,170,266,799,88,49,512,399,23,500,107,524,90,
            194,143,135,192,206,345,148,71,119,101,563,870,158,254,214,
            276,464,332,725,188,385,24,476,40,231,620,171,258,67,109,
            844,244,187,388,701,690,50,7,850,479,48,522,22,154,12,659,
            736,655,577,737,830,811,174,21,237,335,353,234,53,270,62,
            182,45,177,245,812,673,355,556,612,166,204,54,248,365,226,
            242,452,700,685,573,14,842,481,468,781,564,416,179,405,35,
            819,608,624,367,98,643,448,2,460,676,440,240,130,146,184,
            185,430,65,807,377,82,121,708,239,310,138,596,730,575,477,
            851,797,247,27,85,586,307,779,326,494,856,324,827,96,748,
            13,397,125,688,702,92,293,716,277,140,112,4,80,855,839,1,
            413,347,584,493,289,696,19,751,379,76,73,115,6,590,183,734,
            197,483,217,344,330,400,186,243,587,220,780,200,793,246,824,
            41,735,579,81,703,322,760,720,139,480,490,91,814,813,163,
            152,488,763,263,425,410,576,120,319,668,150,160,302,491,515,
            260,145,428,97,251,395,272,252,18,106,358,854,485,144,550,
            131,133,378,68,102,104,58,361,275,209,697,582,338,742,589,
            325,408,229,28,304,191,189,110,126,486,211,547,533,70,215,
            670,249,36,581,389,605,331,518,442,822
        };
    }

}
