﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectorWebService;

namespace ConnectorWebServer
{
	public partial class LogWindow : Form, ILog
	{
		public LogWindow()
		{
			InitializeComponent();
			Debug.WriteLine($"UI on thread {Thread.CurrentThread.ManagedThreadId}");
		}

		public void AddMessage(string format, params object[] args)
		{
			AddMessage(string.Format(format, args));
		}

		public void AddMessage(string message)
		{
			Dispatch(() =>
			{
				_textBox.Text += $"[{DateTime.Now}] {message}{Environment.NewLine}{Environment.NewLine}";
				_textBox.SelectionLength = 0;
				_textBox.SelectionStart = _textBox.TextLength;
			});
		}

		public void Display()
		{
			if (!Visible)
			{
				Show();
			}
			else
			{
				Focus();
			}
		}

		private void Dispatch(Action action)
		{
			if (InvokeRequired)
			{
				BeginInvoke(action);
			}
			else
			{
				action.Invoke();
			}
		}

		private void WindowClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}
	}
}
