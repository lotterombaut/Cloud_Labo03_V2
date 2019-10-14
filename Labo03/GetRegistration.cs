using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Labo03.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Labo03
{
    public static class GetRegistration
    {
        [FunctionName("GetRegistrarion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registrations")] HttpRequest req,
            ILogger log)
        {
            try
            {
                //connectionstring ophalen uit local.settings
                string connectionstring = Environment.GetEnvironmentVariable("ConnectionString");

                //lege lijst met registrations aanmaken
                List<Registration> reg = new List<Registration>();

                //we maken een connectie, wanneer deze niet meer gebruikt wordt, wordt ze gedropt
                using (SqlConnection con = new SqlConnection())
                {
                    //connectionstring doorgeven aan connectie
                    con.ConnectionString = connectionstring;

                    //wachten tot de connection geopend is
                    await con.OpenAsync();

                    //we zullen een sqlcommando gebruiken, als dit niet meer gebruikt wordt zal dit "gedropt" worden
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        //connection waarop het commando zal worden uitgevoerd doorgeven
                        cmd.Connection = con;
                        cmd.CommandText = "select * from tblRegistraties";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            Registration r = new Registration();
                            r.RegistrationId = Convert.ToString(reader["RegistrationID"]);
                            r.LastName = reader["LastName"].ToString();
                            r.FirstName = reader["FirstName"].ToString();
                            r.Email = reader["Email"].ToString();
                            r.Age = Convert.ToInt32(reader["Age"]);
                            r.ZipCode = reader["Zipcode"].ToString();
                            r.IsFirstTimer = Convert.ToBoolean(reader["isFirstTimer"]);
                            reg.Add(r);
                        }

                    }//end using sqlcommand

                }//end using sqlconnection

                return new OkObjectResult(reg);
            }
            catch (Exception ex)
            {
                log.LogError(ex + "     ---->getRegistrations");
                return new StatusCodeResult(500);
            }//end catch
        }//end static async
    }//end class GetRegistration
}//end namespace
