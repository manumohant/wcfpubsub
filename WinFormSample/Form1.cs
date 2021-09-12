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
using WinFormSample.ServiceReference;

namespace WinFormSample
{
    public partial class Form1 : Form
    {
        public delegate void MyEventCallbackHandler(string Name);
        public static event MyEventCallbackHandler MyEventCallbackEvent;

        delegate void SafeThreadCheck(string Name);

        [CallbackBehaviorAttribute(UseSynchronizationContext = false)]
        public class ServiceCallback : IPubSubServiceCallback
        {
            public void NameChange(string Name)
            {
                Form1.MyEventCallbackEvent(Name);
            }
        }
        public Form1()
        {
            InitializeComponent();

            InstanceContext context = new InstanceContext(new ServiceCallback());
            PubSubServiceClient client = new PubSubServiceClient(context);

            MyEventCallbackHandler callbackHandler = new MyEventCallbackHandler(UpdateForm);
            MyEventCallbackEvent += callbackHandler;

            client.Subscribe();
        }
        public void UpdateForm(string Name)
        {
            if (label1.InvokeRequired)
            {
                SafeThreadCheck sc = new SafeThreadCheck(UpdateForm);
                this.BeginInvoke(sc, new object[] { Name });
            }
            else
            {
                label1.Text += Name;
            }
        }
    }
}
