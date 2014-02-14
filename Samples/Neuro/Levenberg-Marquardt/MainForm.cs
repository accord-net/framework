using System;
using System.Windows.Forms;

namespace Samples.LM
{
    /// <summary>
    ///   Levenberg-Marquardt multiple sample applications.
    /// </summary>
    /// 
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        /// <summary>
        ///   Launches the approximation sample application.
        /// </summary>
        /// 
        private void btnApproximation_Click(object sender, EventArgs e)
        {
            launch(new Approximation());
        }

        /// <summary>
        ///   Launches the time-series sample application.
        /// </summary>
        /// 
        private void btnTimeSeries_Click(object sender, EventArgs e)
        {
            launch(new TimeSeries());
        }

        /// <summary>
        ///   Launches the xor-problem sample application.
        /// </summary>
        /// 
        private void btnXOR_Click(object sender, EventArgs e)
        {
            launch(new XorProblem());
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
