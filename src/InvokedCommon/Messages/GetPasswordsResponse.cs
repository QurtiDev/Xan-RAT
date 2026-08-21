

using InvokedCommon.Models;
using ProtoBuf;
using System.Collections.Generic;


namespace InvokedCommon.Messages
{
    [ProtoContract]
    public class GetPasswordsResponse : IMessage
    {
        [ProtoMember(1)]
        public List<RecoveredAccount> RecoveredAccounts { get; set; }
    }
}
