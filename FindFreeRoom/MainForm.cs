using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FindFreeRoom.ExchangeConnector;
using FindFreeRoom.ExchangeConnector.Base;

namespace FindFreeRoom
{
	public partial class MainForm : Form
	{
		private ExchangeConnector.ExchangeConnector _connector;
		private LocationResolver _locations;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			try
			{
				var props = Properties.Settings.Default;
				_connector = new ExchangeConnector.ExchangeConnector(props.email);

				string currentSite = props.currentSite;
				_locations = new LocationResolver();
				_locations.Load("locationMap.csv");
				_connector.LocationFilter = _locations.OfSite(currentSite).ToArray(); // filter locations by site
				_connector.Connect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				Application.Exit();
			}
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			choicesListView.Items.Clear();
			choicesListView.Refresh();

			// TODO: need progress bar

			var roomsNearby = _connector.GetFilteredRooms();
			var roomsWithLocations = _locations.ResolveLocations(roomsNearby);

			var results = _connector.GetAvaialility(roomsWithLocations).Select(
				room => 
					room.Availability == TimeInterval.Zero ? 
						$"{room.Room.Name} is not available in the nearest future" : 
						$"{room.Room.Name} is available from {room.Availability.Start} for {room.Availability.Duration.TotalMinutes} minutes").ToArray();

			choicesListView.BeginUpdate();
			foreach (var result in results)
			{
				choicesListView.Items.Add(result);
			}
			choicesListView.EndUpdate();
		}

		private void ticker_Tick(object sender, EventArgs e)
		{
			timeLabel.Text = DateTime.Now.ToShortTimeString();
		}
	}
}
