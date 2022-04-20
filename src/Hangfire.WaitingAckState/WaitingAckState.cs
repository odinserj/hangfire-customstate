using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Newtonsoft.Json;

namespace Hangfire.WaitingAckState
{
    public class WaitingAckState : IState
    {
        [JsonIgnore]
        public string Name => StateName;

        [JsonIgnore]
        public string Reason => "Waiting for acknowledgement from an external service.";

        [JsonIgnore]
        public bool IsFinal => false;

        [JsonIgnore]
        public bool IgnoreJobLoadException => true;

        public static readonly string StateName = "WaitingAck";

        public Dictionary<string, string> SerializeData() => new Dictionary<string, string>();

        public class Handler : IStateHandler
        {
            public const string StateIndexKey = "my-waiting-ack";
            
            public void Apply(ApplyStateContext context, IWriteOnlyTransaction transaction)
            {
                transaction.AddToSet(StateIndexKey, context.BackgroundJob.Id, JobHelper.ToTimestamp(DateTime.UtcNow));
            }

            public void Unapply(ApplyStateContext context, IWriteOnlyTransaction transaction)
            {
                transaction.RemoveFromSet(StateIndexKey, context.BackgroundJob.Id);
            }

            public string StateName => WaitingAckState.StateName;
        }
    }
}