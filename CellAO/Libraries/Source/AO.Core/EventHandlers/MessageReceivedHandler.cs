using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Core.EventHandlers
{
    using System.ComponentModel.Composition;

    using AO.Core.Components;
    using AO.Core.Events;

    [Export(typeof(IHandle<MessageReceivedEvent>))]
    public class MessageReceivedHandler : IHandle<MessageReceivedEvent>
    {
        private readonly IMessagePublisher messagePublisher;

        [ImportingConstructor]
        public MessageReceivedHandler(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        public void Handle(MessageReceivedEvent obj)
        {
            this.messagePublisher.Publish(obj.Sender, obj.Message);
        }
    }
}
