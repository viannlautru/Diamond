﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class StartClient
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect();
            Console.ReadLine();
        }
    }
}
