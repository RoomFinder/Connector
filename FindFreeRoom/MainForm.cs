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
		private Location _myLocation;

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

				_myLocation = new Location
				{
					Site =  props.currentSite,
					Building = props.currentBuilding,
					Floor = props.currentFloor
				};
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

			try
			{
				// TODO: need progress bar

				var roomsNearby = _connector.GetFilteredRooms();
				var roomsWithLocations = _locations.ResolveLocations(roomsNearby);

				var results = _locations.SmartSort(_connector.GetAvaialility(roomsWithLocations)
					.Where(room => room.Availability != TimeInterval.Zero), _myLocation)
					.ToArray();

				choicesListView.BeginUpdate();
				foreach (var result in results)
				{
					var item = choicesListView.Items.Add(new ListViewItem(new[]
					{
						result.Room.Name,
						FormatAvailableIn((int)(result.Availability.Start - DateTime.Now).TotalMinutes),
						FormatAvailableFor((int)result.Availability.Duration.TotalMinutes)
					}));
					item.Tag = result.Room;
				}
				choicesListView.EndUpdate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				Application.Exit();
			}
		}

		private static string FormatAvailableIn(int minutes)
		{
			if (minutes >= 120)
			{
				return $"in {minutes / 60} hours";
			}
			if (minutes <= 0)
			{
				return "now";
			}
			return $"in {minutes} min";
		}

		private static string FormatAvailableFor(int minutes)
		{
			if (minutes >= 8*60)
			{
				return "the whole day";
			}
			if (minutes >= 120)
			{
				return $"{minutes / 60} hours";
			}
			return $"{minutes} min";
		}

		private void ticker_Tick(object sender, EventArgs e)
		{
			timeLabel.Text = DateTime.Now.ToShortTimeString();
		}

		private void choicesListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				if (choicesListView.SelectedItems.Count == 1)
				{
					reserveButton.Enabled = true;
				}
				else
				{
					reserveButton.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private async void reserveButton_Click(object sender, EventArgs e)
		{
			try
			{
				var item = choicesListView.SelectedItems[0];
				;

				if (!await _connector.ReserveRoom((RoomInfo)item.Tag, TimeSpan.FromMinutes(30)))
				{
					throw new Exception("Unable to reserve the room");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
