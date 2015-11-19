using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfHostedQuestionBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string lServerURL = "http://+:19221";
            WebApp.Start<Startup>(lServerURL);
            Console.WriteLine("Server launched on => " + lServerURL + "\r\n");
            Console.Read();
        }
    }
}
