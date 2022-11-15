using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TimeTriggersFunctionApp.HelperClasses;

namespace TimeTriggersFunctionApp
{
    public class UpdateIpData
    {
        [FunctionName("UpdateIpData")]
        public void Run([TimerTrigger("0 0 * * * * ")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            int MinId = 0;
            int MaxId = 500;

            int BatchCount = 100;
            List<QueryExe> Alllist = new List<QueryExe>(BatchCount);
            var stackOfChanges = new ConcurrentStack<int>();

            int i = MinId;
            int index = 0;
            while (i < MaxId + 1)
            {
                int minid = i;
                int maxid = i + BatchCount;
                string q = $"SELECT IP FROM IPAddresses with(nolock) WHERE Id>={minid} and Id<{maxid} ";
                string c = "";
                i = maxid;
                Alllist.Add(new QueryExe(q, c, index));
                index++;
            }

            Parallel.ForEach(Alllist, new ParallelOptions { MaxDegreeOfParallelism = 100 }, command =>
            {
                Task.Yield();
                var temp = command.GetId();
                if (temp?.Count > 0)
                {
                    stackOfChanges.PushRange(temp.ToArray());
                }
            });
            

            if (!stackOfChanges.IsEmpty)
            {
                foreach (var changedItem in stackOfChanges)
                {
                    /// <summary>
                    /// update db with data changes
                    /// </summary>
                }
            }
        }
    }
}
