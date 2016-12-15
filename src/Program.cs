﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace PwPass
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .Build();

            host.Run();
        }
    }
}