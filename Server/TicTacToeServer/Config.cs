using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Server
{
    public class Config
    {
        public int port;
        public string ip;

        override public string ToString() {
            return string.Format("Server.Config: port = {0}; ip = {1}", port, ip);
        }

        public static Config Load(string path) {
            string json = File.ReadAllText(path);
			Config config = JsonConvert.DeserializeObject<Config>(json);
			
            return config; 
        }
    }
}
