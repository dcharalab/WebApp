using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Models;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Repositories
{
    public class RawSqlIpRepository : IRawSqlIpRepository
    {
        private readonly string connStr = Constants.connStr;
        public async Task<List<Report>> GetReport(List<string>? codes)
        {
            var reportList = new List<Report>();

            using SqlConnection connection = new(connStr);
            string sql = "";
            if (codes != null && codes.Count == 0)
            {
                sql = "SELECT Countries.Name as countryName, COUNT(*) as addressesCount, MAX(IpAddresses.UpdatedAt) lastAdderssUpdated " +
                    "FROM IpAddresses " +
                    "INNER JOIN Countries " +
                    "ON IpAddresses.CountryId = Countries.Id " +
                    "GROUP BY Countries.Id, Countries.Name";
            }
            else
            {
                string combinedCodes = "";
                foreach (var c in codes)
                {
                    combinedCodes = combinedCodes + "'" + c + "',";
                }
                combinedCodes = combinedCodes.Remove(combinedCodes.Length - 1, 1);

                sql = "SELECT Countries.Name as countryName, COUNT(*) as addressesCount, MAX(IpAddresses.UpdatedAt) lastAdderssUpdated " +
                    "FROM IpAddresses " +
                    "INNER JOIN Countries " +
                    "ON IpAddresses.CountryId = Countries.Id " +
                    "WHERE Countries.TwoLetterCode IN (" + combinedCodes +
                    ") GROUP BY Countries.Id, Countries.Name";
            }

            // Create a SqlCommand object.
            using (SqlCommand sqlCommand = new(sql, connection))
            {
                try
                {
                    connection.Open();
                    using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        reportList.Add(new Report()
                        {
                            CountryName = reader["countryName"].ToString(),
                            AddressesCount = Int16.Parse(reader["addressesCount"].ToString()),
                            LastAdderssUpdated = Convert.ToDateTime(reader["lastAdderssUpdated"].ToString()),
                        });
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    // Close the connection.
                    connection.Close();
                }
            }

            return reportList;
        }
    }
}