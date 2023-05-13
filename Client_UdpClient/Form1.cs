using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client_UdpClient
{
    public partial class Form1 : Form
    {
        int remotePort = 4569; // or 11000
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        // Сокет для приєднання до сервера
        Socket listeningSocket = null;

        public Form1()
        {
            InitializeComponent();

            // Первинні налаштування інтерфейсу
            btnSendMessage.Enabled = false;
            tbUserMessage.Enabled = false;
            tbUserName.Text = "USER";
        }

        // Натискання на кнопку Приєнання/Від'єднання
        private void btnStartChat_Click(object sender, EventArgs e)
        {
            if(listeningSocket != null)
            {
                // Відключення від сервера
                Form1_FormClosing(sender, null);

                // Налаштування інтерфейсу - Від'єднаний режим
                btnStartChat.Text = "Start Chat";
                tbUserName.Enabled = true;
                tbUserMessage.Text = "";
                tbUserMessage.Enabled = false;
                btnSendMessage.Enabled = false;
            }
            else
            {
                // Підключення до сервера
                try
                {
                    // Якщо поле для введення логіна користувача пусте - метод перериває роботу
                    if(tbUserName.Text.Trim().Length == 0)
                    {
                        MessageBox.Show("Потрібно ввести логін користувача");
                        return;
                    }

                    // Ініціалізація сокету
                    listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    
                    // Алгоритм реєстрації нового користувача
                    // (клієнт відправляє запит на підключення введеного користувачем у відповідне поле логіна)
                    string messageReceived;
                    string message = $"#client-newUser#####{tbUserName.Text}";
                    byte[] data = Encoding.Default.GetBytes(message);
                    EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
                    // Відправка запиту
                    listeningSocket.SendTo(data, remoteEndpoint);
                    // Отримання повідомлення від сервера
                    messageReceived = MessageReceiver();

                    // Алгоритм переривання роботи методу у разі, якщо на сервері вже зареєстрований користувач, який введений на поточному клієнті
                    if(messageReceived.Substring(0, 20).Contains("#server-User-false##"))
                    {
                        MessageBox.Show("Користувач з таким логіном\r\nвже зареєстрований у чаті.\r\nОберіть інший логін");
                        SocketClose();
                        return;
                    }

                    // Запуск алгоритму прослуховування повідомлень від сервера - Запуск чату
                    Task.Run(() =>
                    {
                        Listen();
                    });

                    // Налаштування інтерфейсу - Приєднаний режим
                    //btnStartChat.Enabled = false;
                    btnStartChat.Text = "Disconnect";
                    tbUserName.Enabled = false;
                    tbUserMessage.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Прослуховування відправлених від серверва повідомлень
        private void Listen()
        {
            try
            {
                // Цикл прослуховування
                while (true)
                {
                    // Отримання повідомлення
                    string messageReceived = MessageReceiver();

                    // ВИведення повідомлення у візуальний компонент
                    tbChatMessages.BeginInvoke(new Action<string>(AddTextToTbChatMessages), messageReceived);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); ;
            }
            finally
            {
                SocketClose();
            }
        }

        // Алгоритм Отримання повідомлень
        private string MessageReceiver()
        {
            StringBuilder builder = new StringBuilder();
            int len = 0;
            byte[] data = new byte[1024];
            EndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
            do
            {
                len = listeningSocket.ReceiveFrom(data, ref remoteIP);
                builder.AppendLine(Encoding.Default.GetString(data, 0, len));
            } while (listeningSocket.Available > 0);

            return builder.ToString();
        }

        // Відображення повідомлення у візуальному компоненті
        private void AddTextToTbChatMessages(string str)
        {
            StringBuilder builder = new StringBuilder(tbChatMessages.Text);
            builder.Append(str);
            tbChatMessages.Text = builder.ToString();
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // переміщення каретки в кінець тексту
            tbChatMessages.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)
        }

        // Алгоритм підготовки та відправлення повідомлень від клієнта до сервера
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string messageNew = "#client-newMessage##" + 
                $"{DateTime.Now} " +
                $"\r\n   повідомлення від : {tbUserName.Text} " +
                $"\r\n   {tbUserMessage.Text}";

            byte[] data = Encoding.Default.GetBytes(messageNew);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);

            tbUserMessage.Clear();
        }

        // Алгоритм закриття з'єднання та відправки на сервер повідомлення про вихід клієнта з чату 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Припинення виконання методу у випадку, коли клієнт не заходив до чату
            if (listeningSocket == null)
            {
                return;
            }

            string message = "#client-UserOut#####" + $"{DateTime.Now}\r\n   {tbUserName.Text} покинув чат";

            byte[] data = Encoding.Default.GetBytes(message);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);
            
            SocketClose();
        }

        // Зміна інтерфейсу - кнопки відправки повідомлень в залежності
        // від заповненості компонента для введення повідомлень
        private void tbUserMessage_TextChanged(object sender, EventArgs e)
        {
            if(tbUserMessage.Text.Length == 0)
            {
                btnSendMessage.Enabled = false;
            }
            else
            {
                btnSendMessage.Enabled = true;
            }
        }

        // Закриття з'єднання
        private void SocketClose()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }
    }
}
