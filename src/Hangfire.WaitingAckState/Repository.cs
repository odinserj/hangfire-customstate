using System.Collections.Generic;
using Hangfire.Storage;

namespace Hangfire.WaitingAckState
{
    public class Repository
    {
        private readonly JobStorage _storage;

        public Repository(JobStorage storage)
        {
            _storage = storage;
        }
        
        public List<WaitingAckJobDto> GetWaitingAckJobs()
        {
            var result = new List<WaitingAckJobDto>();

            using (var connection = _storage.GetConnection() as JobStorageConnection)
            {
                var jobIds = connection.GetAllItemsFromSet(WaitingAckState.Handler.StateIndexKey);

                foreach (var jobId in jobIds)
                {
                    var job = connection.GetJobData(jobId);
                    result.Add(new WaitingAckJobDto
                    {
                        JobId = jobId,
                        CreatedAt = job.CreatedAt,
                        JobName = $"{job.Job.Type.Name}.{job.Job.Method.Name}"
                    });
                }
            }

            return result;
        }
        
        public long GetWaitingAckJobCount()
        {
            using var connection = _storage.GetConnection() as JobStorageConnection;
            return connection.GetSetCount(WaitingAckState.Handler.StateIndexKey);
        }
    }
}