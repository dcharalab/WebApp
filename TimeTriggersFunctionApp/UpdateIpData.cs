using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TimeTriggersFunctionApp.HelperClasses;
using TimeTriggersFunctionApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TimeTriggersFunctionApp
{
    public class UpdateIpData
    {
        public UpdateIpData()
        {
        }
        [FunctionName("UpdateIpData")]
        public async Task Run([TimerTrigger("0 0 * * * * "
            #if DEBUG
            , RunOnStartup=true
            #endif
            )]TimerInfo myTimer)
        {

            var result = new List<int>();
            var connStr = Environment.GetEnvironmentVariable("IpDbConnStr");
            try
            {
                var sqlStr = "SELECT COUNT(Id) as ct FROM IPAddresses";
                SqlConnection conn = new(connStr);
                SqlCommand command = new(sqlStr, conn);
                command.CommandTimeout = 180;
                
                using (conn)
                {
                    conn.Open();
                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(Int32.Parse(reader["ct"].ToString()));
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Cant get min or max");
            }


            int Count = result[0];
            int OffSet = 0; //Next Time 100000
            int FetchNext = 100; //Next Time 200000
            var listOfChanges = new List<IpCountryCode>();
            while (OffSet < Count)
            {

                string q = "select IP, Countries.Name as CountryName, TwoLetterCode, ThreeLetterCode " +
                           "FROM IPAddresses inner join Countries on Countries.Id = IPAddresses.CountryId " +
                            $"ORDER BY IPAddresses.Id OFFSET {OffSet} ROWS  FETCH NEXT {FetchNext} ROWS ONLY";
                string c = Environment.GetEnvironmentVariable("IpDbConnStr");
                
                var qry = new QueryExe(q, c);
                var temp = await qry.GetIpDiff();
                if (temp?.Count > 0)
                {
                    listOfChanges.AddRange(temp.ToArray());
                }

                OffSet += FetchNext;
            }
            

            if (listOfChanges.Count>0)
            {
                try
                {
                    SqlConnection conn = new(connStr);
                    
                    using (conn)
                    {
                        conn.Open();
                        foreach (var changedItem in listOfChanges)
                        {
                            var resultCountyExists = new List<int>();

                            using (SqlCommand command = new SqlCommand($"SELECT Id FROM Countries WHERE Countries.ThreeLetterCode = '{changedItem.ThreeLetterCode}'", conn))
                            {
                                using SqlDataReader reader = command.ExecuteReader();

                                while (reader.Read())
                                {
                                    resultCountyExists.Add(Int32.Parse(reader["Id"].ToString()));
                                }
                                reader.Close();
                            }

                            if (resultCountyExists.Count > 0)
                            {
                                using (SqlCommand cmdUpd = new SqlCommand($"UPDATE IPAddresses SET CountryId = {resultCountyExists[0]} WHERE IP = '{changedItem.Ip}'", conn))
                                {
                                    cmdUpd.ExecuteScalar();
                                }
                            }
                            else
                            {
                                using (SqlCommand cmdIns = new SqlCommand($"INSERT INTO Countries(Name, TwoLetterCode, ThreeLetterCode ) VALUES ('{changedItem.CountryName}', '{changedItem.TwoLetterCode}', '{changedItem.ThreeLetterCode}');", conn))
                                {
                                    cmdIns.ExecuteScalar();
                                }
                                var resultList = new List<int>();
                                using (SqlCommand cmdRead = new SqlCommand($"SELECT Id FROM Countries WHERE Countries.ThreeLetterCode = '{changedItem.ThreeLetterCode}'", conn))
                                {
                                    using SqlDataReader readerSelect = cmdRead.ExecuteReader();
                                    while (readerSelect.Read())
                                    {
                                        resultList.Add(Int32.Parse(readerSelect["Id"].ToString()));
                                    }
                                    readerSelect.Close();
                                }

                                if (resultList.Count > 0)
                                {
                                    var newCountryId = resultList[0];
                                    using (SqlCommand cmdUpd = new SqlCommand($"UPDATE IPAddresses SET CountryId = {newCountryId} WHERE IP = '{changedItem.Ip}'", conn))
                                    {
                                        cmdUpd.ExecuteScalar();
                                    }
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
