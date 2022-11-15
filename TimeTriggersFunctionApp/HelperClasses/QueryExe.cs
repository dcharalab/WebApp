using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTriggersFunctionApp.Interfaces;
using TimeTriggersFunctionApp.Models;

namespace TimeTriggersFunctionApp.HelperClasses
{
    public class QueryExe
    {
        private string Q = "";
        private string C = Environment.GetEnvironmentVariable("IpDbConnStr");
        private int i = 0;
        private readonly IIpDataApi _ipDataApi;
        public QueryExe(string Q, string C, int i, IIpDataApi ipDataApi)
        {
            this.Q = Q;
            this.C = C;
            this.i = i;
            _ipDataApi = ipDataApi;
        }

        public async Task<List<IpCounty>> GetIpDiff()
        {
            var result = new List<IpCounty>();
            try
            {
                SqlConnection conn = new(C);
                SqlCommand command = new(this.Q, conn);
                command.CommandTimeout = 180;
                using (conn)
                {
                    conn.Open();
                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        //create an object here
                        var ip = reader["IP"].ToString();
                        var countryName = reader["CountryName"].ToString();
                        var twoLetterCode = reader["TwoLetterCode"].ToString();
                        var threeLetterCode = reader["ThreeLetterCode"].ToString();

                        var myIpObject = new IpCounty(ip, twoLetterCode, threeLetterCode, countryName);
                        //get api answer
                        var apiResult = await _ipDataApi.GetIpDetails(ip);
                        var apiIpObject = new IpCounty(ip, apiResult.TwoLetterCode, apiResult.ThreeLetterCode, apiResult.Name);
                        //compare with api answer
                        if (!myIpObject.Equals(apiIpObject))
                        {
                            //if api answer is diferent add instance in results
                            result.Add(apiIpObject);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Exception at QueryExe>{i}");
            }

            return result;
        }
    }
}
