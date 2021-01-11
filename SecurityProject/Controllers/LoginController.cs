using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SecurityProject.Helpers;
using SecurityProject.Models;

namespace SecurityProject.Controllers
{
    public class LoginController : Controller
    {
        private const int MAX_NUMBER_OF_FAULT_LOGINS = 5;
        private TimeSpan TIME_TO_WAIT_BETWEEN_FAULT_LOGIN = TimeSpan.FromSeconds(60);

        private const string CONNECTION_STRING_MILOVAN = @"Server=(localdb)\MSSQLLocalDB;Database=SECURITY_DATABASE;Trusted_Connection=True;";
        private const string CONNECTION_STRING_BOJAN = @"Server=DESKTOP-OVA2KPB\SQLEXPRESS;Database=SECURITY_DATABASE;Trusted_Connection=True;";

        private SqlConnection connection = new SqlConnection(CONNECTION_STRING_MILOVAN);
       
        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public IActionResult SqlInjection([FromBody] User user)
        {
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

        [HttpPost]
        public IActionResult DictionaryAttack([FromBody] User user)
        {
            bool isUserFound = SafeSqlCheck(user);
            if (isUserFound)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [HttpPost]
        public IActionResult SecureLogin([FromBody] User user)
        {
            bool isLoginNumberOK = CheckNumberOfFaultLogin(user);
            if (!isLoginNumberOK) 
            {
                return BadRequest();
            }
            bool isUserFound = SafeSqlCheck(user);
            if (isUserFound)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        private bool SafeSqlCheck(User user)
        {
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password",connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.Password);
                int login = (int)command.ExecuteScalar();
                if(login == 0)
                {
                    command = new SqlCommand("UPDATE USERS set NumberOfInvalidLogin = NumberOfInvalidLogin + 1 where Username = @username",connection);
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.ExecuteNonQuery();
                    command = new SqlCommand("UPDATE USERS set LastInvalidLogin = @datetime",connection);
                    command.Parameters.AddWithValue("@datetime", DateTime.Now);
                    command.ExecuteNonQuery();
                    return false;
                }
                else
                {
                    command = new SqlCommand("UPDATE USERS set NumberOfInvalidLogin = 0 where Username = @username",connection);
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            { }
            finally
            {
                connection.Close();
            }
            return false;
        }
        private bool CheckNumberOfFaultLogin(User user)
        {
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select NumberOfInvalidLogin from Users where Username = @username",connection);
                command.Parameters.AddWithValue("@username", user.Username);
                int login = (int)command.ExecuteScalar();
                if(login == MAX_NUMBER_OF_FAULT_LOGINS) 
                {
                    command = new SqlCommand("SELECT LastInvalidLogin FROM Users WHERE Username=@username",connection);
                    command.Parameters.AddWithValue("@username", user.Username);
                    DateTime banTime = (DateTime)command.ExecuteScalar();
                    if(DateTime.Now - banTime > TIME_TO_WAIT_BETWEEN_FAULT_LOGIN) 
                    {
                        command = new SqlCommand("UPDATE USERS set NumberOfInvalidLogin = 0 WHERE Username=@username",connection);
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.ExecuteNonQuery();
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
                }
                else 
                {
                    return true;
                }
            }
            catch
            { }
            finally
            {
                connection.Close();
            }
            return false;
        }
    }
}
