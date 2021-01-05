using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SecurityProject.Models;

namespace SecurityProject.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public IActionResult SqlInjection([FromBody] User user)
        {
            SqlConnection connection = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=SECURITY_DATABASE;Trusted_Connection=True;");
            try
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Users where Username='" + user.Username + "' AND Password='" + user.Password + "'", connection);
                int login = (int)sqlCommand.ExecuteScalar();
                if (login == 0)
                {
                    return BadRequest();
                }                    
                else
                {
                    return Ok();
                }
            }
            catch
            { }
            finally
            {
                connection.Close();
            }
            return BadRequest();
        }
    }
}
