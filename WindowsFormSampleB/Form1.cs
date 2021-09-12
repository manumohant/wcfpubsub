using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormSampleB.ServiceReference;

namespace WindowsFormSampleB
{
    public partial class Form1 : Form
    {
        InstanceContext context = null;
        PubSubServiceClient client = null;
        public class ServiceCallback : IPubSubServiceCallback
        {
            public void NameChange(string Name)
            {
                MessageBox.Show(Name);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            context = new InstanceContext(new ServiceCallback());
            client = new PubSubServiceClient(context);
            client.PublishNameChange(textBox1.Text);
            client.Close();
        }
    }
}
