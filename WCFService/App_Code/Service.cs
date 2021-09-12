using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Runtime.Serialization;

// A WCF service consists of a contract (defined below as IMyService, DataContract1), 
// a class which implements that interface (see MyService), 
// and configuration entries that specify behaviors associated with 
// that implementation (see <system.serviceModel> in web.config)

namespace ListPublishSubscribe.Service
{
    [ServiceContract(Namespace = "http://ListPublishSubscribe.Service", SessionMode = SessionMode.Required, CallbackContract = typeof(IPubSubContract))]
    public interface IPubSubService
    {
        [OperationContract(IsOneWay = false, IsInitiating=true)]
        void Subscribe();
        [OperationContract(IsOneWay = false, IsInitiating=true)]
        void Unsubscribe();
        [OperationContract(IsOneWay = false)]
        void PublishNameChange(string Name);
    }

    public interface IPubSubContract
    {
        [OperationContract(IsOneWay = true)]
        void NameChange(string Name);
    }

    public class ServiceEventArgs : EventArgs
    {
        public string Name;
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class PubSubService : IPubSubService
    {
        public delegate void NameChangeEventHandler(object sender, ServiceEventArgs e);
        public static event NameChangeEventHandler NameChangeEvent;

        IPubSubContract ServiceCallback = null;
        NameChangeEventHandler NameHandler = null;

        public void Subscribe()
        {
            ServiceCallback = OperationContext.Current.GetCallbackChannel<IPubSubContract>();
            NameHandler = new NameChangeEventHandler(PublishNameChangeHandler);
            NameChangeEvent += NameHandler;
        }

        public void Unsubscribe()
        {
            NameChangeEvent -= NameHandler;
        }

        public void PublishNameChange(string Name)
        {
            ServiceEventArgs se = new ServiceEventArgs();
            se.Name = Name;
            NameChangeEvent(this, se);
        }

        public void PublishNameChangeHandler(object sender,ServiceEventArgs se)
        {
            ServiceCallback.NameChange(se.Name);
       
        }
    }   
}
