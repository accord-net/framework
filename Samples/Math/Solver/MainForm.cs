using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Math.Optimization;
using Accord;

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
