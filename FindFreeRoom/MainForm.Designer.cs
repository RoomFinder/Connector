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
			this.startButton = new System.Windows.Forms.Button();
			this.choicesListView = new System.Windows.Forms.ListView();
			this.reserveButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// startButton
			// 
			this.startButton.BackColor = System.Drawing.Color.Salmon;
			this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.startButton.ForeColor = System.Drawing.Color.White;
			this.startButton.Location = new System.Drawing.Point(2, 3);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(85, 61);
			this.startButton.TabIndex = 0;
			this.startButton.Text = "Search";
			this.startButton.UseVisualStyleBackColor = false;
			// 
			// choicesListView
			// 
			this.choicesListView.Location = new System.Drawing.Point(2, 70);
			this.choicesListView.Name = "choicesListView";
			this.choicesListView.Size = new System.Drawing.Size(278, 133);
			this.choicesListView.TabIndex = 1;
			this.choicesListView.UseCompatibleStateImageBehavior = false;
			// 
			// reserveButton
			// 
			this.reserveButton.BackColor = System.Drawing.Color.LimeGreen;
			this.reserveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.reserveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reserveButton.ForeColor = System.Drawing.Color.White;
			this.reserveButton.Location = new System.Drawing.Point(196, 3);
			this.reserveButton.Name = "reserveButton";
			this.reserveButton.Size = new System.Drawing.Size(84, 61);
			this.reserveButton.TabIndex = 2;
			this.reserveButton.Text = "Reserve";
			this.reserveButton.UseVisualStyleBackColor = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 208);
			this.Controls.Add(this.reserveButton);
			this.Controls.Add(this.choicesListView);
			this.Controls.Add(this.startButton);
			this.Name = "MainForm";
			this.Text = "Find Free Room";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.ListView choicesListView;
		private System.Windows.Forms.Button reserveButton;
	}
}

