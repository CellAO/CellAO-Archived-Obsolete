// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageSerializer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MessageSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AO.Core.Components
{
    using System.IO;

    using SmokeLounge.AOtomation.Messaging.Messages;

    public class MessageSerializer : IMessageSerializer
    {
        #region Fields

        private readonly SmokeLounge.AOtomation.Messaging.Serialization.MessageSerializer serializer;

        #endregion

        #region Constructors and Destructors

        public MessageSerializer()
        {
            this.serializer = new SmokeLounge.AOtomation.Messaging.Serialization.MessageSerializer();
        }

        #endregion

        #region Public Methods and Operators

        public Message Deserialize(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return this.serializer.Deserialize(stream);
            }
        }

        public byte[] Serialize(Message message)
        {
            using (var stream = new MemoryStream())
            {
                this.serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        #endregion
    }
}