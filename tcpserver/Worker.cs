using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ObligatoriskOpgave;

namespace tcpserver
{
    class Worker
    {
        private static List<Bog> bogliste = new List<Bog>()
        {
            new Bog("olivers bog", "brink", 233, "2354627489037"),
            new Bog("Oguz bog", "jens", 234, "2354627489038"),
            new Bog("brinks bog", "peter", 235, "2354627489039"),
            new Bog("mathias bog", "oguz", 236, "2354627489030")

        };

        public Worker()
        {
            TcpListener socket = new TcpListener(IPAddress.Loopback, 4646);
            socket.Start();
            while (true)
            {
                TcpClient myClient = socket.AcceptTcpClient();
                Task.Run(() =>
                {
                    DoClient(myClient);
                });
            }
        }

        public void DoClient(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            using (StreamWriter sw = new StreamWriter(ns))
            using (StreamReader sr = new StreamReader(ns))
            {

                while (true)
                {
                    printNoget(sw);
                    string input = sr.ReadLine();

                    if (input == "HentAlle")
                    {
                        sr.ReadLine();
                        sw.WriteLine(getAll());
                        sw.Flush();
                    }

                    if (input == "Hent")
                    {
                        string isbn = sr.ReadLine();
                        sw.WriteLine(getBook(isbn));
                        sw.Flush();
                    }

                    if (input == "Gem")
                    {
                        string jsonbog = sr.ReadLine();
                        save(jsonbog);
                    }
                }
            }
        }

        private void printNoget(StreamWriter sw)
        {
            sw.WriteLine("Serveren er nu oppe");
            sw.Flush();
        }

        private string getBook(string isbn)
        {
            Bog bog = bogliste.Find((bog1 => bog1.Isbn13 == isbn));
            return (bog == null) ? "Jeg kan ikke finde den valgte bog. Prøv igen." : JsonConvert.SerializeObject(bog);
        }

        private string getAll()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Bog bog in bogliste)
            {
                sb.Append(JsonConvert.SerializeObject(bog));
            }

            return sb.ToString();
        }

        private void save(string bog)
        {
            Bog b = JsonConvert.DeserializeObject<Bog>(bog);
             bogliste.Add(b);
        }
    }
}
