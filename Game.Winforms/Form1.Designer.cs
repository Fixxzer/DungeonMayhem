namespace Game.Winforms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			listBoxCharacterName = new ListBox();
			menuStrip1 = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			newToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			label1 = new Label();
			label2 = new Label();
			listBoxHealth = new ListBox();
			label3 = new Label();
			listBoxShields = new ListBox();
			labelRoundCounter = new Label();
			labelPlayersTurnName = new Label();
			listBoxHand = new ListBox();
			label4 = new Label();
			label5 = new Label();
			textBoxMessages = new TextBox();
			buttonPlayCard = new Button();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// listBoxCharacterName
			// 
			listBoxCharacterName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			listBoxCharacterName.FormattingEnabled = true;
			listBoxCharacterName.ItemHeight = 15;
			listBoxCharacterName.Location = new Point(10, 54);
			listBoxCharacterName.Margin = new Padding(3, 2, 3, 2);
			listBoxCharacterName.Name = "listBoxCharacterName";
			listBoxCharacterName.Size = new Size(139, 394);
			listBoxCharacterName.TabIndex = 0;
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new Size(20, 20);
			menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new Padding(5, 2, 0, 2);
			menuStrip1.Size = new Size(877, 24);
			menuStrip1.TabIndex = 1;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new Size(98, 22);
			newToolStripMenuItem.Text = "&New";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(95, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(98, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(10, 34);
			label1.Name = "label1";
			label1.Size = new Size(58, 15);
			label1.TabIndex = 2;
			label1.Text = "Character";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(154, 34);
			label2.Name = "label2";
			label2.Size = new Size(42, 15);
			label2.TabIndex = 3;
			label2.Text = "Health";
			// 
			// listBoxHealth
			// 
			listBoxHealth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			listBoxHealth.FormattingEnabled = true;
			listBoxHealth.ItemHeight = 15;
			listBoxHealth.Location = new Point(154, 54);
			listBoxHealth.Margin = new Padding(3, 2, 3, 2);
			listBoxHealth.Name = "listBoxHealth";
			listBoxHealth.Size = new Size(47, 394);
			listBoxHealth.TabIndex = 4;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(206, 34);
			label3.Name = "label3";
			label3.Size = new Size(44, 15);
			label3.TabIndex = 5;
			label3.Text = "Shields";
			// 
			// listBoxShields
			// 
			listBoxShields.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			listBoxShields.FormattingEnabled = true;
			listBoxShields.ItemHeight = 15;
			listBoxShields.Location = new Point(206, 54);
			listBoxShields.Margin = new Padding(3, 2, 3, 2);
			listBoxShields.Name = "listBoxShields";
			listBoxShields.Size = new Size(50, 394);
			listBoxShields.TabIndex = 6;
			// 
			// labelRoundCounter
			// 
			labelRoundCounter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			labelRoundCounter.AutoSize = true;
			labelRoundCounter.Location = new Point(810, 34);
			labelRoundCounter.Name = "labelRoundCounter";
			labelRoundCounter.Size = new Size(51, 15);
			labelRoundCounter.TabIndex = 7;
			labelRoundCounter.Text = "Round 1";
			// 
			// labelPlayersTurnName
			// 
			labelPlayersTurnName.AutoSize = true;
			labelPlayersTurnName.Font = new Font("Segoe UI", 14F);
			labelPlayersTurnName.Location = new Point(318, 34);
			labelPlayersTurnName.Name = "labelPlayersTurnName";
			labelPlayersTurnName.Size = new Size(119, 25);
			labelPlayersTurnName.TabIndex = 8;
			labelPlayersTurnName.Text = "Player Name";
			// 
			// listBoxHand
			// 
			listBoxHand.FormattingEnabled = true;
			listBoxHand.ItemHeight = 15;
			listBoxHand.Location = new Point(262, 71);
			listBoxHand.Margin = new Padding(3, 2, 3, 2);
			listBoxHand.Name = "listBoxHand";
			listBoxHand.Size = new Size(603, 79);
			listBoxHand.TabIndex = 9;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(276, 54);
			label4.Name = "label4";
			label4.Size = new Size(36, 15);
			label4.TabIndex = 10;
			label4.Text = "Hand";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(276, 245);
			label5.Name = "label5";
			label5.Size = new Size(58, 15);
			label5.TabIndex = 11;
			label5.Text = "Messages";
			// 
			// textBoxMessages
			// 
			textBoxMessages.Location = new Point(262, 262);
			textBoxMessages.Margin = new Padding(3, 2, 3, 2);
			textBoxMessages.Multiline = true;
			textBoxMessages.Name = "textBoxMessages";
			textBoxMessages.Size = new Size(603, 177);
			textBoxMessages.TabIndex = 12;
			// 
			// buttonPlayCard
			// 
			buttonPlayCard.Location = new Point(276, 154);
			buttonPlayCard.Margin = new Padding(3, 2, 3, 2);
			buttonPlayCard.Name = "buttonPlayCard";
			buttonPlayCard.Size = new Size(82, 22);
			buttonPlayCard.TabIndex = 13;
			buttonPlayCard.Text = "Play Card";
			buttonPlayCard.UseVisualStyleBackColor = true;
			buttonPlayCard.Click += buttonPlayCard_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(877, 450);
			Controls.Add(buttonPlayCard);
			Controls.Add(textBoxMessages);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(listBoxHand);
			Controls.Add(labelPlayersTurnName);
			Controls.Add(labelRoundCounter);
			Controls.Add(listBoxShields);
			Controls.Add(label3);
			Controls.Add(listBoxHealth);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(listBoxCharacterName);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Margin = new Padding(3, 2, 3, 2);
			Name = "Form1";
			Text = "Dungeon Mayhem";
			Load += Form1_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListBox listBoxCharacterName;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Label label1;
        private Label label2;
        private ListBox listBoxHealth;
        private Label label3;
        private ListBox listBoxShields;
        private Label labelRoundCounter;
        private Label labelPlayersTurnName;
        private ListBox listBoxHand;
        private Label label4;
        private Label label5;
        private TextBox textBoxMessages;
        private Button buttonPlayCard;
    }
}
