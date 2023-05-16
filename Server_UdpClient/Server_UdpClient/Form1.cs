using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_UdpClient
{
    public partial class Form1 : Form
    {
        int localPort = 4569; // or 11000
        string multicastGroupIp = "0.0.0.0";
        // Колекція кінцевих точок - підключених клієнтів
        List<IPEndPoint> clientsList = new List<IPEndPoint>();
        // Колекція логінів підключених користувачів
        List<string> usersNamesList = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        // Алгоритм запуску методу - слухача
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Task.Run(Listener);

            btnStartServer.Enabled = false;
        }

        // Прослуховування відправлених клієнтами повідомлень
        private void Listener()
        {
            UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(multicastGroupIp), localPort));
            IPEndPoint remoteEP = null;

            Text = "Server was started !";
            tbServerStatistics.Text = $"Server was started at {DateTime.Now.ToString()}\r\n\r\n";

            // Цикл прослуховування
            while (true)
            {
                // Отримання повідомлення
                byte[] buffer = listener.Receive(ref remoteEP);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Encoding.Default.GetString(buffer));

                // Перетворення отриманого від клієнта повідомлення у формат рядка
                string clientMessage = builder.ToString();

                // Виведення отриманого повідомлення (від клієнта до сервера)
                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), remoteEP.Address + ":" + remoteEP.Port + $" - clients in chat: {clientsList.Count}" + "\r\n - " + clientMessage);

                // Алгоритм обробки запитів на підключення нового клієнта
                if(builder.ToString().Substring(0, 20).Contains("#client-newUser#####"))
                {
                    //MessageBox.Show(clientMessage.Substring(20));
                    if (usersNamesList.Contains(clientMessage.Substring(20)))
                    {
                        SendMessagesForUser("#server-User-false##" + "###", remoteEP);
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), "Servers message to " + remoteEP.Address + ":" + remoteEP.Port + "#server-User-false##" + "###" + $" - clients in chat: {clientsList.Count}\r\n");
                    }
                    else
                    {
                        // Додавання логіна користувача (нового) до колекції логінів
                        usersNamesList.Add(clientMessage.Substring(20));
                        // Додавання кінцевої точки користувача (нового) до колекції підключених користувачів
                        clientsList.Add(remoteEP);

                        SendMessagesForUser("#server-User-true###" + "###", remoteEP);
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), "Servers message to " + remoteEP.Address + ":" + remoteEP.Port + "#server-User-true###" + "###" + $" - clients in chat: {clientsList.Count}\r\n");
                        SendMessagesAllUsers($"{DateTime.Now}\r\nДо чату приєднався :{clientMessage.Substring(20)}" + $"Клієнтів у чаті: {clientsList.Count}\r\n") ;
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"{DateTime.Now}\r\nДо чату приєднався :{clientMessage.Substring(20)}" + $"Клієнтів у чаті: {clientsList.Count}\r\n");
                    }
                }
                // Алгоритм обробки повідомлень підключеного клієнта
                else if(builder.ToString().Substring(0, 20).Contains("#client-newMessage##"))
                {
                    SendMessagesAllUsers(builder.ToString().Substring(20));
                }
                // Алгоритм обробки запитів на відключення клієнта
                else if(builder.ToString().Substring(0, 20).Contains("#client-UserOut#####"))
                {
                    int posUserNsme = clientsList.IndexOf(remoteEP);
                    string userToRemove = usersNamesList[posUserNsme];
                    usersNamesList.Remove(userToRemove);
                    clientsList.Remove(remoteEP);
                    SendMessagesAllUsers(builder.ToString().Substring(20) + $"Клієнтів у чаті: {clientsList.Count}\r\n");
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"Servers info at {DateTime.Now} - clients in chat: {clientsList.Count}\r\n");
                }
            }
        }

        // Відправка повідомлень до усіх підключених клієнтів
        private void SendMessagesAllUsers(string text)
        {
            for (int i = 0; i < clientsList.Count; i++)
            {
                SendMessagesForUser(text, clientsList[i]);
            }
        }

        // Відправка повідомлень до 1 клієнта
        private void SendMessagesForUser(string text, IPEndPoint remoteEP)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            UdpClient client = new UdpClient(); 
            try
            {
                client.Send(data, data.Length, remoteEP);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        // Підготовка та відображення текстової інформації у візуальному компоненті сервера
        private void AddTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(tbServerStatistics.Text);
            sb.Append(str + "\r\n");
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // переміщення каретки в кінець тексту
            tbServerStatistics.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)
        }
    }
}