using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WebApp.Controllers
{
    public class TestBatches
    {
        public async Task Operation(){
            int MinId = 0;
            int MaxId = 500;

            int BatchCount = 100;
            List<QueryExe> Alllist = new List<QueryExe>(BatchCount);
            var stackOfChanges =new  ConcurrentStack<int>();

            int i = MinId;
            int index = 0;
            while(i<MaxId+1)
            {
                int minid = i;
                int maxid = i + BatchCount;
                string q = $"SELECT  [Id]   FROM YourTable with(nolock)  WHERE Id>={minid} and Id<{maxid} ";
                string c = "";
                i = maxid;
                Alllist.Add(new QueryExe(q,c, index));
                index++;
            }

            Parallel.ForEach(Alllist, new ParallelOptions {MaxDegreeOfParallelism = 100}, command =>
            {
                Task.Yield();
                var temp = command.GetId();
                if(temp?.Count>0){
                    stackOfChanges.PushRange(temp.ToArray());
                }
            });
            GC.Collect();

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

    public class QueryExe
    {
        private  string Q = "";
        private  string C = "";
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
                Console.WriteLine($"Exception>{i}");
            }

            return result;
        }

    }
}