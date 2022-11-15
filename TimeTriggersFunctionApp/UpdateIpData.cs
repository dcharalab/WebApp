using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TimeTriggersFunctionApp.HelperClasses;
using TimeTriggersFunctionApp.Interfaces;
using TimeTriggersFunctionApp.Models;

namespace TimeTriggersFunctionApp
{
    public class UpdateIpData
    {
        private readonly IIpDataApi _ipDataApi;
        public UpdateIpData(IIpDataApi ipDataApi)
        {
            _ipDataApi = ipDataApi;
        }
        [FunctionName("UpdateIpData")]
        public void Run([TimerTrigger("0 0 * * * * ")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var result = new List<int>();
            var connStr = Environment.GetEnvironmentVariable("IpDbConnStr");
            try
            {
                var sqlStr = "SELECT MIN(Id) as MinId, MAX(Id) as MaxId FROM IPAddresses";
                SqlConnection conn = new(connStr);
                SqlCommand command = new(sqlStr, conn);
                command.CommandTimeout = 180;
                
                using (conn)
                {
                    conn.Open();
                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(Int32.Parse(reader["MinId"].ToString()));
                        result.Add(Int32.Parse(reader["MaxId"].ToString()));
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Cant get min or max");
            }


            int MinId = result.Min();
            int MaxId = result.Max();

            int BatchCount = 100;
            List<QueryExe> Alllist = new List<QueryExe>(BatchCount);
            var stackOfChanges = new ConcurrentStack<IpCounty>();

            int i = MinId;
            int index = 0;
            while (i < MaxId + 1)
            {
                int minid = i;
                int maxid = i + BatchCount;
                string q = "select IP, Countries.Name as CountryName, TwoLetterCode, ThreeLetterCode "+
                            "FROM IpAddresses inner join Countries on Countries.Id = IPAddresses.CountryId with(nolock) "+
                            $" WHERE Id>={minid} and Id<{maxid} ";
                string c = "";
                i = maxid;
                Alllist.Add(new QueryExe(q, c, index, _ipDataApi));
                index++;
            }

            Parallel.ForEach(Alllist, new ParallelOptions { MaxDegreeOfParallelism = 100 }, async command =>
            {
                _ = Task.Yield();
                var temp = await command.GetIpDiff();
                if (temp?.Count > 0)
                {
                    stackOfChanges.PushRange(temp.ToArray());
                }
            });
            

            if (!stackOfChanges.IsEmpty)
            {
                try
                {
                    foreach (var changedItem in stackOfChanges)
                    {
                        var sqlStr = $"SELECT Id FROM Countries WHERE Countries.ThreeLetterCode = '{changedItem.ThreeLetterCode}'";
                        SqlConnection conn = new(connStr);
                        SqlCommand command = new(sqlStr, conn);
                        command.CommandTimeout = 180;
                        var resultCountyExists = new List<int>();
                        using (conn)
                        {
                            conn.Open();
                            using SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                resultCountyExists.Add(Int32.Parse(reader["Id"].ToString()));
                            }

                            if (resultCountyExists.Count > 0)
                            {
                                var sqlUpdate = $"UPDATE IPAddresses SET CountryId = {resultCountyExists[0]} WHERE IP = '{changedItem.Ip}'";
                                command = new(sqlUpdate, conn);
                                command.ExecuteScalar();
                            }
                            else
                            {
                                var sqlInsert = $"INSERT INTO Countries(Name, TwoLetterCode, ThreeLetterCode ) VALUES ('{changedItem.CountryName}', '{changedItem.TwoLetterCode}', '{changedItem.ThreeLetterCode}');";
                                command = new(sqlInsert, conn);
                                command.ExecuteScalar();

                                var sqlSelect = $"SELECT Id FROM Countries WHERE Countries.ThreeLetterCode = '{changedItem.ThreeLetterCode}'";
                                using SqlDataReader readerSelect = command.ExecuteReader();
                                var resultList = new List<int>();
                                while (readerSelect.Read())
                                {
                                    resultList.Add(Int32.Parse(reader["Id"].ToString()));
                                }

                                if (resultList.Count > 0)
                                {
                                    var newCountryId = resultList[0];
                                    var sqlUpdate = $"UPDATE IPAddresses SET CountryId = {newCountryId} WHERE IP = '{changedItem.Ip}'";
                                    command = new(sqlUpdate, conn);
                                    command.ExecuteScalar();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error in updating Data");
                }
            }
        }
    }
}
