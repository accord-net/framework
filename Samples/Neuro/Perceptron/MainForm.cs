using System;
using System.Windows.Forms;

namespace SampleApp
{
    /// <summary>
    ///   Perceptron multiple sample applications.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        /// <summary>
        ///   Launches the delta rule sample application.
        /// </summary>
        /// 
        private void btnDeltaRule_Click(object sender, EventArgs e)
        {
            launch(new DeltaRuleForm());
        }

        /// <summary>
        ///   Launches the one-layer perceptron sample application.
        /// </summary>
        /// 
        private void btnOneLayerPerceptron_Click(object sender, EventArgs e)
        {
            launch(new OneLayerPerceptronForm());
        }

        /// <summary>
        ///   Launches the Perceptron sample application.
        /// </summary>
        /// 
        private void btnPerceptron_Click(object sender, EventArgs e)
        {
            launch(new PerceptronForm());
        }




        private void launch(Form form)
        {
            form.Show();
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            WindowState = FormWindowState.Minimized;
        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}
