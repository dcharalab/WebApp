using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTriggersFunctionApp.HelperClasses
{
    internal class QueryExe
    {
        private string Q = "";
        private string C = "";
        private int i = 0;
        public QueryExe(string Q, string C, int i)
        {
            this.Q = Q;
            this.C = C;
            this.i = i;
        }

        public List<int> GetId()
        {
            var result = new List<int>();
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
                        //compare with api answer
                        //if api answer is diferent add instance in results
                        result.Add(reader.GetInt32(0));
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
