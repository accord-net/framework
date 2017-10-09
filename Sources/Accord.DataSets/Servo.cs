// Accord Datasets Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.DataSets
{
    using Accord.DataSets.Base;
    using Accord.Math;
    using System;

    /// <summary>
    ///   Servo Data
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This is the dataset described in <a href="http://archive.ics.uci.edu/ml/datasets/Servo">
    ///   http://archive.ics.uci.edu/ml/datasets/Servo</a>. There is no much information about this
    ///   dataset except what has been described in a recollection by their authors:</para>
    ///   
    /// <para>
    ///   - Ross Quinlan: "This data was given to me by Karl Ulrich at MIT in 1986. I didn't record his description
    ///     at the time, but here's his subsequent(1992) recollection:"
    ///     'I seem to remember that the data was from a simulation of a servo system involving a servo amplifier, 
    ///     a motor, a lead screw/nut, and a sliding carriage of some sort. It may have been on of the translational 
    ///     axes of a robot on the 9th floor of the AI lab. In any case, the output value is almost certainly a rise 
    ///     time, or the time required for the system to respond to a step change in a position set point.'"</para>
    ///   - (Quinlan, ML'93) 
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://archive.ics.uci.edu/ml/datasets/Servo">
    ///       Lichman, M. (2013). UCI Machine Learning Repository [http://archive.ics.uci.edu/ml]. Irvine, CA:
    ///       University of California, School of Information and Computer Science. </a>
    ///       </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    public class Servo
    {

        /// <summary>
        ///   Gets the name of each variable in the Servo dataset: motor, screw, pgain, vgain
        /// </summary>
        /// 
        public string[] VariableNames { get; private set; }

        /// <summary>
        ///   Gets the training instances in the Servo dataset. The first
        ///   two columns contain strings and the last two columns contain
        ///   discrete numbers. 
        /// </summary>
        /// 
        public object[][] Instances { get; private set; }

        /// <summary>
        ///   Gets the regression output for the <see cref="Instances">data in the Servo dataset</see>.
        /// </summary>
        /// 
        public double[] Output { get; private set; }

        /// <summary>
        ///   Prepares the Servo dataset from UCI's Irvine Repository.
        /// </summary>
        /// 
        public Servo()
        {
            VariableNames = new[]
            {
                "motor", "screw", "pgain", "vgain"
            };

            object[,] data =
            {
                { "E","E",5,4, 0.28125095   },
                { "B","D",6,5, 0.5062525    },
                { "D","D",4,3, 0.35625148   },
                { "B","A",3,2, 5.500033     },
                { "D","B",6,5, 0.35625148   },
                { "E","C",4,3, 0.8062546    },
                { "C","A",3,2, 5.100014     },
                { "A","A",3,2, 5.7000422    },
                { "C","A",6,5, 0.76875436   },
                { "D","A",4,1, 1.0312537    },
                { "B","E",6,5, 0.46875226   },
                { "E","C",5,4, 0.39375174   },
                { "B","C",4,1, 0.28125095   },
                { "E","C",3,1, 1.1          },
                { "C","C",5,4, 0.5062525    },
                { "E","B",3,2, 1.8999897    },
                { "D","C",3,1, 0.9000011    },
                { "B","C",5,4, 0.46875226   },
                { "B","B",5,4, 0.5437528    },
                { "C","E",4,2, 0.20625044   },
                { "E","D",4,3, 0.9187554    },
                { "A","D",4,3, 1.1062483    },
                { "B","C",6,5, 0.46875226   },
                { "A","C",4,2, 0.58125305   },
                { "A","B",6,5, 0.58125305   },
                { "E","C",6,5, 0.39375174   },
                { "A","A",3,1, 5.3000236    },
                { "A","E",4,2, 0.46875226   },
                { "C","D",3,2, 1.8999897    },
                { "B","B",3,2, 4.299977     },
                { "B","E",4,2, 0.35625148   },
                { "B","C",3,1, 3.899964     },
                { "C","E",4,1, 0.5437528    },
                { "C","A",6,2, 0.5437528    },
                { "C","C",6,5, 0.5062525    },
                { "E","E",3,2, 1.1          },
                { "D","E",3,1, 0.5000003    },
                { "E","C",4,2, 0.13124992   },
                { "C","B",6,5, 0.5437528    },
                { "C","D",4,1, 0.20625044   },
                { "D","B",4,1, 0.69375384   },
                { "C","B",4,3, 0.88125515   },
                { "C","C",4,3, 0.9187554    },
                { "B","D",4,1, 0.2437507    },
                { "B","A",5,3, 0.6562536    },
                { "A","B",4,3, 1.0312537    },
                { "B","A",4,1, 0.8062546    },
                { "E","D",4,2, 0.431252     },
                { "C","E",3,2, 4.0999675    },
                { "D","D",3,1, 0.7000007    },
                { "D","A",6,5, 0.431252     },
                { "C","B",3,2, 4.499986     },
                { "B","E",3,2, 4.6999955    },
                { "C","D",5,4, 0.5062525    },
                { "B","B",4,2, 0.7312541    },
                { "A","E",4,3, 1.1437455    },
                { "A","A",4,2, 0.88125515   },
                { "B","D",4,3, 1.0312537    },
                { "E","A",3,2, 6.9000983    },
                { "B","C",4,3, 0.9562557    },
                { "E","B",4,2, 0.58125305   },
                { "E","A",5,4, 0.58125305   },
                { "E","B",5,4, 0.431252     },
                { "C","A",6,1, 0.5437528    },
                { "D","A",4,3, 0.7312541    },
                { "C","B",4,2, 0.5062525    },
                { "D","B",3,2, 1.6999923    },
                { "D","C",3,2, 1.2999974    },
                { "C","A",5,2, 0.5437528    },
                { "B","D",4,2, 0.39375174   },
                { "B","A",6,5, 0.8062546    },
                { "D","A",4,2, 0.28125095   },
                { "C","B",5,4, 0.5437528    },
                { "A","E",6,5, 0.5062525    },
                { "A","C",4,1, 0.35625148   },
                { "A","E",5,4, 0.5062525    },
                { "E","C",4,1, 0.28125095   },
                { "B","B",3,1, 4.499986     },
                { "A","D",3,2, 4.6999955    },
                { "E","D",3,2, 1.2999974    },
                { "E","A",3,1, 7.1001077    },
                { "A","C",6,5, 0.5062525    },
                { "C","E",5,4, 0.46875226   },
                { "C","A",5,4, 0.76875436   },
                { "E","A",6,5, 0.58125305   },
                { "B","E",5,4, 0.46875226   },
                { "E","E",4,3, 0.8437549    },
                { "B","A",4,2, 0.8437549    },
                { "B","D",5,4, 0.5062525    },
                { "C","C",4,2, 0.35625148   },
                { "A","A",5,3, 0.69375384   },
                { "C","E",4,3, 1.068751     },
                { "A","A",4,3, 1.1062483    },
                { "C","A",6,3, 0.5437528    },
                { "A","E",4,1, 0.2437507    },
                { "A","D",6,5, 0.5062525    },
                { "E","D",3,1, 0.9000011    },
                { "C","B",4,1, 0.431252     },
                { "B","D",3,2, 4.0999675    },
                { "B","B",4,3, 0.99375594   },
                { "B","C",4,2, 0.5062525    },
                { "A","E",3,2, 4.499986     },
                { "B","D",3,1, 3.899964     },
                { "D","B",5,4, 0.39375174   },
                { "C","C",4,1, 0.2437507    },
                { "C","D",4,2, 0.2437507    },
                { "E","B",4,1, 1.1812428    },
                { "D","B",3,1, 1.2999974    },
                { "E","B",6,5, 0.431252     },
                { "D","A",3,1, 2.499982     },
                { "A","D",5,4, 0.5062525    },
                { "C","A",4,1, 0.7312541    },
                { "C","D",6,5, 0.46875226   },
                { "B","A",4,3, 1.068751     },
                { "E","A",4,3, 1.2187401    },
                { "A","A",4,1, 0.8437549    },
                { "A","C",4,3, 0.99375594   },
                { "E","D",6,5, 0.31875122   },
                { "E","A",4,2, 0.99375594   },
                { "C","D",3,1, 1.4999949    },
                { "B","B",4,1, 0.58125305   },
                { "C","A",4,2, 0.76875436   },
                { "C","A",5,1, 0.5437528    },
                { "C","E",3,1, 1.2999974    },
                { "C","A",3,1, 4.299977     },
                { "C","A",4,3, 1.0312537    },
                { "C","C",3,1, 1.8999897    },
                { "D","A",5,4, 0.431252     },
                { "A","B",5,4, 0.58125305   },
                { "C","C",3,2, 4.299977     },
                { "E","D",5,4, 0.31875122   },
                { "D","C",4,3, 0.5437528    },
                { "E","E",6,5, 0.28125095   },
                { "D","B",4,2, 0.35625148   },
                { "A","D",4,2, 0.46875226   },
                { "B","B",6,5, 0.5437528    },
                { "A","B",4,1, 0.6187533    },
                { "A","C",5,4, 0.5062525    },
                { "B","E",4,1, 0.20625044   },
                { "C","B",3,1, 3.899964     },
                { "E","E",4,2, 0.5062525    },
                { "B","E",4,3, 1.1062483    },
                { "A","E",3,1, 3.899964     },
                { "A","B",4,2, 0.8062546    },
                { "A","C",3,1, 3.899964     },
                { "E","C",3,2, 1.4999949    },
                { "B","A",3,1, 5.100014     },
                { "D","D",3,2, 1.4999949    },
                { "A","C",3,2, 4.6999955    },
                { "E","A",4,1, 0.88125515   },
                { "B","A",5,4, 0.8062546    },
                { "E","E",3,1, 0.7000007    },
                { "D","E",3,2, 0.9000011    },
                { "E","B",3,1, 1.4999949    },
                { "A","D",4,1, 0.2437507    },
                { "A","D",3,1, 4.0999675    },
                { "E","B",4,3, 0.99375594   },
                { "A","B",3,1, 4.6999955    },
                { "D","B",4,3, 0.58125305   },
                { "A","A",5,4, 0.8062546    },
                { "D","A",3,2, 2.6999795    },
                { "C","E",6,5, 0.46875226   },
                { "B","C",3,2, 4.499986     },
                { "B","E",3,1, 3.6999667    },
                { "C","D",4,3, 0.9562557    },
                { "A","B",3,2, 4.499986     },
                { "A","A",6,5, 0.8062546    },
            };

            int n = data.GetLength(0);
            Output = new double[n];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = (double)data[i, 4];

            Instances = new object[n][];
            for (int i = 0; i < Instances.Length; i++)
            {
                Instances[i] = new object[4];
                for (int j = 0; j < Instances[i].Length; j++)
                    Instances[i][j] = data[i, j];
            }
        }

    }
}
