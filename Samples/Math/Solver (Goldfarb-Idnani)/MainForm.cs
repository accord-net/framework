// Accord.NET Sample Applications
// http://accord-framework.net
//
// Copyright © 2009-2014, César Souza
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
// 
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

using Accord;
using Accord.Math.Optimization;
using System;
using System.Text;
using System.Windows.Forms;

namespace Solver.QP
{
    /// <summary>
    ///   Goldfarb-Idnani solver for Quadratic Programming (QP) problems
    /// </summary>
    /// 
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            comboBox1.DataSource = new string[] { "min", "max" };
        }

        /// <summary>
        ///   Computes the optimization algorithm when the user
        ///   presses the "Compute" button in the main interface.
        /// </summary>
        /// 
        private void btnCompute_Click(object sender, EventArgs e)
        {
            // First, get what the user entered on screen:
            String strObjective = tbObjective.Text;
            String[] strConstraints = tbConstraints.Lines;

            // Check if this is a minimization or maximization task
            bool minimize = (string)comboBox1.SelectedItem == "min";

            // Now we can start creating our function:
            QuadraticObjectiveFunction function;
            LinearConstraint[] constraints = new LinearConstraint[strConstraints.Length];


            // Attempt to parse the string and create the objective function
            if (!QuadraticObjectiveFunction.TryParse(strObjective, out function))
            {
                tbSolution.Text = "Invalid objective function.";
                return;
            }

            // Create list of constraints
            for (int i = 0; i < constraints.Length; i++)
            {
                if (!LinearConstraint.TryParse(strConstraints[i], function, out constraints[i]))
                {
                    tbSolution.Text = "Invalid constraint at line " + i + ".";
                    return;
                }
            }


            // Now, after the text has been parsed into actual objects, finally create the solver
            var solver = new GoldfarbIdnaniQuadraticSolver(function.NumberOfVariables, constraints);

            double optimumValue;

            try
            {
                // Solve the optimization problem:
                optimumValue = (minimize) ? 
                    solver.Minimize(function) : // the user wants to minimize it
                    solver.Maximize(function);  // the user wants to maximize it
            }
            catch (NonPositiveDefiniteMatrixException)
            {
                tbSolution.Text = "Function is not positive definite.";
                return;
            }
            catch (ConvergenceException)
            {
                tbSolution.Text = "No possible solution could be attained.";
                return;
            }


            // Retrieve the computed solution 
            double[] solution = solver.Solution;

            // And let's format and display it:
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Solution:");
            sb.AppendLine();
            sb.AppendLine(" " + strObjective + " = " + optimumValue);
            sb.AppendLine();

            for (int i = 0; i < solution.Length; i++)
            {
                string variableName = function.Indices[i];
                sb.AppendLine(" " + variableName + " = " + solution[i]);
            }

            tbSolution.Text = sb.ToString();
        }
    }
}
