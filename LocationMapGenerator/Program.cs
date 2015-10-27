using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
				Dowork();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				Console.WriteLine(ex);
			}
		}

		class RoomList
		{
			public string Id;
			public string Site;
			public string Building;
			public string Floor;

			public static bool TryParse(string room, out RoomList output)
			{
				var props = Properties.Settings.Default;
				var regex = new Regex(props.regex);

				Console.Write($"Roomlist found: {room}...   ");
				var match = regex.Match(room);
				if (!match.Success)
				{
					Console.WriteLine("Did not match");
					output = null;
					return false;
				}

				output = new RoomList
				{
					Id = match.Groups[0].Value,
					Site = match.Groups["site"].Value,
					Building = match.Groups["building"].Value,
					Floor = match.Groups["floor"].Value
				};
				return true;
			}
		} 

		private static void Dowork()
		{
			var props = Properties.Settings.Default;
			var connector = new ExchangeConnector(props.username, props.password, props.serverUrl, props.serviceEmail);

			connector.Connect();
			
			var roomLists = ParseRooms(connector.GetAllRoomLists()).ToArray();
			DumpToCsv(roomLists);
			DumpToJson(roomLists);
		}

		private static IEnumerable<RoomList> ParseRooms(IEnumerable<string> rooms)
		{
			foreach (var room in rooms)
			{
				RoomList item;
				if (RoomList.TryParse(room, out item))
				{
					yield return item;
				}
			}
		}  

		private static void DumpToCsv(IEnumerable<RoomList> roomsLists)
		{
			using (var file = new StreamWriter(File.OpenWrite("locationMap.csv")))
			{
				foreach (var room in roomsLists)
				{
					file.WriteLine($"{room.Id},{room.Site},{room.Building},{room.Floor}");
				}
			}
		}

		private static void DumpToJson(IEnumerable<RoomList> roomsLists)
		{
			var lines = roomsLists.Select(room => $"{room.Id}:{{site:{room.Site},buildind:{room.Building},floor:{room.Floor}}}");
			var content = "{" + string.Join(",\n", lines) + "}";
			using (var file = new StreamWriter(File.OpenWrite("locationMap.json")))
			{
				file.Write(content);
			}
		}
	}
}
