using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPEventReceiverExtensions
    {
        public static void Register(this SPEventReceiverDefinitionCollection collection, string name, Type receiverType, SPEventReceiverType actionsToHandle, SPEventReceiverSynchronization synchronization = SPEventReceiverSynchronization.Synchronous, int sequenceNumber = 11000)
        {
            SPEventReceiverDefinition receiverDefinition = collection.Cast<SPEventReceiverDefinition>()
                .SingleOrDefault(receiver => string.Equals(receiver.Name, name));

            if (receiverDefinition == null)
            {
                receiverDefinition = collection.Add();
                receiverDefinition.Name = name;
                receiverDefinition.Synchronization = synchronization;
                receiverDefinition.Type = actionsToHandle;
                receiverDefinition.SequenceNumber = sequenceNumber;
                receiverDefinition.Assembly = receiverType.Assembly.ToString();
                receiverDefinition.Class = receiverType.FullName;
                receiverDefinition.Update();
            }
        }

        public static void Delete(this SPEventReceiverDefinitionCollection collection, string name)
        {
            SPEventReceiverDefinition receiverDefinition = collection.Cast<SPEventReceiverDefinition>()
                .SingleOrDefault(receiver => string.Equals(receiver.Name, name));

            if (receiverDefinition != null)
            {
                receiverDefinition.Delete();
                receiverDefinition.Update();
            }
        }
    }
}
