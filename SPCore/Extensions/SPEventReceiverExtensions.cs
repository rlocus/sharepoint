using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPEventReceiverExtensions
    {
        public static void Register(this SPEventReceiverDefinitionCollection collection, string name, Type receiverType, SPEventReceiverType actionsToHandle, int sequenceNumber = 11000)
        {
            SPEventReceiverDefinition receiverDefinition = collection
                                                            .Cast<SPEventReceiverDefinition>()
                                                            .FirstOrDefault(r => r.Name == name);

            if (receiverDefinition == null)
            {
                SPEventReceiverDefinition eventReceiver = collection.Add();
                eventReceiver.Name = name;
                eventReceiver.Synchronization = SPEventReceiverSynchronization.Synchronous;
                eventReceiver.Type = actionsToHandle;
                eventReceiver.SequenceNumber = sequenceNumber;
                eventReceiver.Assembly = receiverType.Assembly.ToString();
                eventReceiver.Class = receiverType.FullName;

                eventReceiver.Update();
            }
        }

        public static void Delete(this SPEventReceiverDefinitionCollection collection, string name)
        {
            SPEventReceiverDefinition receiverDefinition =
                collection.Cast<SPEventReceiverDefinition>().SingleOrDefault(receiver => receiver.Name == name);

            if (receiverDefinition != null)
            {
                receiverDefinition.Delete();
                receiverDefinition.Update();
            }
        }
    }
}
