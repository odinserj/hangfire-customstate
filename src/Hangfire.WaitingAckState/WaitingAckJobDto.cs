using System;

namespace Hangfire.WaitingAckState
{
    public class WaitingAckJobDto
    {
        public string JobId { get; set; }

        public string JobName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}