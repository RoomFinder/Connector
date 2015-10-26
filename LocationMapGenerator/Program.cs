using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FindFreeRoom.ExchangeConnector;

namespace LocationMapGenerator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("LocationMapGenerator started.");

			try
			{
				new Program().DoWork();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				Console.WriteLine(ex);
			}
		}

		private void DoWork()
		{
			var props = Properties.Settings.Default;
			var connector = new ExchangeConnector(props.username, props.password, props.serverUrl, props.serviceEmail);

			connector.Connect();
			var regex = new Regex(props.regex);
			using (var file = new StreamWriter(File.OpenWrite("locationMap.csv")))
			{
				foreach (var room in connector.GetAllRoomLists())
				{
					Console.Write($"Roomlist found: {room}...   ");
					var match = regex.Match(room);
					if (!match.Success)
					{
						Console.WriteLine("Did not match");
						continue;
					}
					var email = match.Groups[0];
					var site = match.Groups["site"];
					var building = match.Groups["building"];
					var floor = match.Groups["floor"];
					file.WriteLine($"{email},{site},{building},{floor}");
					Console.WriteLine("OK");
				}
			}
			//connector.PrintActive();
		}
	}
}
