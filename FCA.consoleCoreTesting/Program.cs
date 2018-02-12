using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FCA.Core;
using System.IO.Ports;
namespace FCA.consoleCoreTesting
{
    class Program
    {
		static void Main(string[] args)
		{
			CommunicationsControl serial = new CommunicationsControl();

			SerialPort test = new SerialPort("COM1", 1000000);


			int[] test_array = new int[10];

			serial.SetPort("COM2", 1000000);

			serial.InputStringRecieved += InputStringRecievedEventHandler;

			test.Open();
			while (true)
			{
				// Testing the string parsing, makeing sure it only reads the json formated text
				test.WriteLine("Some intro text { \"name\":\"Jillian\", \"age\":31, \"city\":\"New York\" } Some Intermeddiate test text { \"name\":\"Hubert\", \"age\":24, \"city\":\"Red Deer\" } Some Ending text");
				Thread.Sleep(3);
				test.WriteLine("{CPU: 'Intel',Drives: ['DVD read/writer','500 gigabyte hard drive']}");
				Thread.Sleep(3);
			}
		}

        static void InputStringRecievedEventHandler(object sender, EventArgs e)
        {
            InputStringRecievedEventArgs args = (InputStringRecievedEventArgs)e;
            Console.WriteLine(args.JObject.ToString());
        }
    }
}
