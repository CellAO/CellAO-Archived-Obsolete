// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the MessagePublisher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AO.Core.Components
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using SmokeLounge.AOtomation.Messaging.Messages;

    [Export(typeof(IMessagePublisher))]
    public class MessagePublisher : IMessagePublisher
    {
        #region Fields

        private readonly Dictionary<Type, IList<IHandleMessage>> messageHandlers;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MessagePublisher([ImportMany] IEnumerable<IHandleMessage> messageHandlers)
        {
            this.messageHandlers = new Dictionary<Type, IList<IHandleMessage>>();

            foreach (var messageHandler in messageHandlers)
            {
                var handlerInterface =
                    messageHandler.GetType()
                                  .GetInterfaces()
                                  .FirstOrDefault(i => typeof(IHandleMessage).IsAssignableFrom(i) && i.IsGenericType);
                if (handlerInterface == null)
                {
                    continue;
                }

                var arg = handlerInterface.GetGenericArguments().FirstOrDefault();
                if (arg == null)
                {
                    continue;
                }

                IList<IHandleMessage> handlers;
                if (this.messageHandlers.TryGetValue(arg, out handlers) == false)
                {
                    handlers = new List<IHandleMessage>();
                    this.messageHandlers.Add(arg, handlers);
                }

                handlers.Add(messageHandler);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Publish(object sender, Message message)
        {
            IList<IHandleMessage> handlers;
            if (this.messageHandlers.TryGetValue(message.Body.GetType(), out handlers) == false)
            {
                return;
            }

            foreach (var handler in handlers)
            {
                handler.Handle(sender, message);
            }
        }

        #endregion
    }
}