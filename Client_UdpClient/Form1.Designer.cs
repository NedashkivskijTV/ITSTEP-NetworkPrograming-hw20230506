namespace Client_UdpClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbChatMessages = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUserMessage = new System.Windows.Forms.TextBox();
            this.btnStartChat = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Messages";
            // 
            // tbChatMessages
            // 
            this.tbChatMessages.Location = new System.Drawing.Point(12, 27);
            this.tbChatMessages.Multiline = true;
            this.tbChatMessages.Name = "tbChatMessages";
            this.tbChatMessages.PlaceholderText = "Chat messages";
            this.tbChatMessages.ReadOnly = true;
            this.tbChatMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbChatMessages.Size = new System.Drawing.Size(327, 254);
            this.tbChatMessages.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 308);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "User name";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(12, 326);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.PlaceholderText = "Enter user name";
            this.tbUserName.Size = new System.Drawing.Size(327, 23);
            this.tbUserName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 372);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "User message";
            // 
            // tbUserMessage
            // 
            this.tbUserMessage.Location = new System.Drawing.Point(12, 390);
            this.tbUserMessage.Multiline = true;
            this.tbUserMessage.Name = "tbUserMessage";
            this.tbUserMessage.PlaceholderText = "Enter message";
            this.tbUserMessage.Size = new System.Drawing.Size(327, 80);
            this.tbUserMessage.TabIndex = 5;
            this.tbUserMessage.TextChanged += new System.EventHandler(this.tbUserMessage_TextChanged);
            // 
            // btnStartChat
            // 
            this.btnStartChat.Location = new System.Drawing.Point(12, 497);
            this.btnStartChat.Name = "btnStartChat";
            this.btnStartChat.Size = new System.Drawing.Size(102, 23);
            this.btnStartChat.TabIndex = 6;
            this.btnStartChat.Text = "Start Chat";
            this.btnStartChat.UseVisualStyleBackColor = true;
            this.btnStartChat.Click += new System.EventHandler(this.btnStartChat_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(237, 497);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(102, 23);
            this.btnSendMessage.TabIndex = 7;
            this.btnSendMessage.Text = "Send message";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 530);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnStartChat);
            this.Controls.Add(this.tbUserMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbUserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbChatMessages);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox tbChatMessages;
        private Label label2;
        private TextBox tbUserName;
        private Label label3;
        private TextBox tbUserMessage;
        private Button btnStartChat;
        private Button btnSendMessage;
    }
}