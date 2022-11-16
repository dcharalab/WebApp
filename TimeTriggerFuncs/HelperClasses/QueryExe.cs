using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTriggersFunctionApp.Models;

namespace TimeTriggersFunctionApp.HelperClasses
{
    public class QueryExe
    {
        private string Q = "";
        private string C = "";
        private int i = 0;
        public QueryExe(string Q, string C)
        {
            this.Q = Q;
            this.C = C;
        }

        public async Task<List<IpCountryCode>> GetIpDiff()
        {
            var result = new List<IpCountryCode>();
            try
            {
                SqlConnection conn = new(C);
                SqlCommand command = new(this.Q, conn);
                command.CommandTimeout = 180;
                using (conn)
                {
                    conn.Open();
                    using SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        //create an object here
                        var ip = reader["IP"].ToString();
                        var countryName = reader["CountryName"].ToString();
                        var twoLetterCode = reader["TwoLetterCode"].ToString();
                        var threeLetterCode = reader["ThreeLetterCode"].ToString();

                        var myIpObject = new IpCountryCode(ip, twoLetterCode, threeLetterCode, countryName);
                        //get api answer
                        var dataApi = new IpDataApi();
                        var apiResult = await dataApi.GetIpDetails(ip);
                        if (apiResult != null)
                        {
                            var apiIpObject = new IpCountryCode(ip, apiResult.TwoLetterCode, apiResult.ThreeLetterCode, apiResult.Name);
                            //compare with api answer
                            if (!myIpObject.Equals(apiIpObject))
                            {
                                //if api answer is diferent add instance in results
                                result.Add(apiIpObject);
                            }
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
