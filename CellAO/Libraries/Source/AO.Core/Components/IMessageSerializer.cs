// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IMessageSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AO.Core.Components
{
    using SmokeLounge.AOtomation.Messaging.Messages;

    public interface IMessageSerializer
    {
        #region Public Methods and Operators

        Message Deserialize(byte[] buffer);

        byte[] Serialize(Message message);

        #endregion
    }
}