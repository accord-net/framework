// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

using Accord;
using Accord.Math.Optimization;
using System;
using System.Text;
using System.Windows.Forms;

namespace Solver
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            comboBox1.DataSource = new string[] { "min", "max" };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String objectiveString = tbObjective.Text;
            String[] constraintStrings = tbConstraints.Lines;
            bool minimize = (string)comboBox1.SelectedItem == "min";

            QuadraticObjectiveFunction function;
            LinearConstraint[] constraints = new LinearConstraint[constraintStrings.Length];

            try
            {
                // Create objective function
                function = new QuadraticObjectiveFunction(objectiveString);
            }
            catch (FormatException)
            {
                tbSolution.Text = "Invalid objective function.";
                return;
            }

            // Create list of constraints
            for (int i = 0; i < constraints.Length; i++)
            {
                try
                {
                    constraints[i] = new LinearConstraint(function, constraintStrings[i]);
                }
                catch (FormatException)
                {
                    tbSolution.Text = "Invalid constraint at line " + i + ".";
                    return;
                }
            }

            // Create solver
            var solver = new GoldfarbIdnaniQuadraticSolver(function.NumberOfVariables, constraints);

            try
            {
                // Solve the minimization or maximization problem
                double value = (minimize) ? solver.Minimize(function) : solver.Maximize(function);

                // Grab the solution found
                double[] solution = solver.Solution;

                // Format and display solution
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Solution:");
                sb.AppendLine();
                sb.AppendLine(" " + objectiveString + " = " + value);
                sb.AppendLine();
                for (int i = 0; i < solution.Length; i++)
                {
                    string variableName = function.Indices[i];
                    sb.AppendLine(" " + variableName + " = " + solution[i]);
                }

                tbSolution.Text = sb.ToString();
            }
            catch (NonPositiveDefiniteMatrixException)
            {
                tbSolution.Text = "Function is not positive definite.";
            }
            catch (ConvergenceException)
            {
                tbSolution.Text = "No possible solution could be attained.";
            }

        }
    }
}
