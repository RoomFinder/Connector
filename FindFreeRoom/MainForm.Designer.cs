namespace FindFreeRoom
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.startButton = new System.Windows.Forms.Button();
			this.choicesListView = new System.Windows.Forms.ListView();
			this.reserveButton = new System.Windows.Forms.Button();
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.timeLabel = new System.Windows.Forms.Label();
			this.ticker = new System.Windows.Forms.Timer(this.components);
			this.buttonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// startButton
			// 
			this.startButton.BackColor = System.Drawing.Color.Salmon;
			this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.startButton.ForeColor = System.Drawing.Color.White;
			this.startButton.Location = new System.Drawing.Point(3, 3);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(85, 61);
			this.startButton.TabIndex = 0;
			this.startButton.Text = "Search";
			this.startButton.UseVisualStyleBackColor = false;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// choicesListView
			// 
			this.choicesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.choicesListView.Location = new System.Drawing.Point(0, 70);
			this.choicesListView.MultiSelect = false;
			this.choicesListView.Name = "choicesListView";
			this.choicesListView.Size = new System.Drawing.Size(353, 84);
			this.choicesListView.TabIndex = 1;
			this.choicesListView.UseCompatibleStateImageBehavior = false;
			this.choicesListView.View = System.Windows.Forms.View.List;
			this.choicesListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.choicesListView_ItemSelectionChanged);
			// 
			// reserveButton
			// 
			this.reserveButton.BackColor = System.Drawing.Color.LimeGreen;
			this.reserveButton.Enabled = false;
			this.reserveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.reserveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reserveButton.ForeColor = System.Drawing.Color.White;
			this.reserveButton.Location = new System.Drawing.Point(94, 3);
			this.reserveButton.Name = "reserveButton";
			this.reserveButton.Size = new System.Drawing.Size(84, 61);
			this.reserveButton.TabIndex = 1;
			this.reserveButton.Text = "Reserve";
			this.reserveButton.UseVisualStyleBackColor = false;
			this.reserveButton.Click += new System.EventHandler(this.reserveButton_Click);
			// 
			// buttonPanel
			// 
			this.buttonPanel.Controls.Add(this.timeLabel);
			this.buttonPanel.Controls.Add(this.startButton);
			this.buttonPanel.Controls.Add(this.reserveButton);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonPanel.Location = new System.Drawing.Point(0, 0);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new System.Drawing.Size(353, 68);
			this.buttonPanel.TabIndex = 3;
			// 
			// timeLabel
			// 
			this.timeLabel.AutoSize = true;
			this.timeLabel.Location = new System.Drawing.Point(184, 9);
			this.timeLabel.Name = "timeLabel";
			this.timeLabel.Size = new System.Drawing.Size(0, 13);
			this.timeLabel.TabIndex = 2;
			// 
			// ticker
			// 
			this.ticker.Enabled = true;
			this.ticker.Interval = 1000;
			this.ticker.Tick += new System.EventHandler(this.ticker_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(353, 154);
			this.Controls.Add(this.buttonPanel);
			this.Controls.Add(this.choicesListView);
			this.Name = "MainForm";
			this.Text = "Find Free Room";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.buttonPanel.ResumeLayout(false);
			this.buttonPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.ListView choicesListView;
		private System.Windows.Forms.Button reserveButton;
		private System.Windows.Forms.Panel buttonPanel;
		private System.Windows.Forms.Label timeLabel;
		private System.Windows.Forms.Timer ticker;
	}
}

