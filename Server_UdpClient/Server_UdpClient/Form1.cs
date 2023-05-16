using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_UdpClient
{
    public partial class Form1 : Form
    {
        int localPort = 4569; // or 11000
        string multicastGroupIp = "0.0.0.0";
        // �������� ������� ����� - ���������� �볺���
        List<IPEndPoint> clientsList = new List<IPEndPoint>();
        // �������� ����� ���������� ������������
        List<string> usersNamesList = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        // �������� ������� ������ - �������
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Task.Run(Listener);

            btnStartServer.Enabled = false;
        }

        // ��������������� ����������� �볺����� ����������
        private void Listener()
        {
            UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(multicastGroupIp), localPort));
            IPEndPoint remoteEP = null;

            Text = "Server was started !";
            tbServerStatistics.Text = $"Server was started at {DateTime.Now.ToString()}\r\n\r\n";

            // ���� ���������������
            while (true)
            {
                // ��������� �����������
                byte[] buffer = listener.Receive(ref remoteEP);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Encoding.Default.GetString(buffer));

                // ������������ ���������� �� �볺��� ����������� � ������ �����
                string clientMessage = builder.ToString();

                // ��������� ���������� ����������� (�� �볺��� �� �������)
                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), remoteEP.Address + ":" + remoteEP.Port + $" - clients in chat: {clientsList.Count}" + "\r\n - " + clientMessage);

                // �������� ������� ������ �� ���������� ������ �볺���
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
                        // ��������� ����� ����������� (������) �� �������� �����
                        usersNamesList.Add(clientMessage.Substring(20));
                        // ��������� ������ ����� ����������� (������) �� �������� ���������� ������������
                        clientsList.Add(remoteEP);

                        SendMessagesForUser("#server-User-true###" + "###", remoteEP);
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), "Servers message to " + remoteEP.Address + ":" + remoteEP.Port + "#server-User-true###" + "###" + $" - clients in chat: {clientsList.Count}\r\n");
                        SendMessagesAllUsers($"{DateTime.Now}\r\n�� ���� ��������� :{clientMessage.Substring(20)}" + $"�볺��� � ���: {clientsList.Count}\r\n") ;
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"{DateTime.Now}\r\n�� ���� ��������� :{clientMessage.Substring(20)}" + $"�볺��� � ���: {clientsList.Count}\r\n");
                    }
                }
                // �������� ������� ���������� ����������� �볺���
                else if(builder.ToString().Substring(0, 20).Contains("#client-newMessage##"))
                {
                    SendMessagesAllUsers(builder.ToString().Substring(20));
                }
                // �������� ������� ������ �� ���������� �볺���
                else if(builder.ToString().Substring(0, 20).Contains("#client-UserOut#####"))
                {
                    int posUserNsme = clientsList.IndexOf(remoteEP);
                    string userToRemove = usersNamesList[posUserNsme];
                    usersNamesList.Remove(userToRemove);
                    clientsList.Remove(remoteEP);
                    SendMessagesAllUsers(builder.ToString().Substring(20) + $"�볺��� � ���: {clientsList.Count}\r\n");
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"Servers info at {DateTime.Now} - clients in chat: {clientsList.Count}\r\n");
                }
            }
        }

        // ³������� ���������� �� ��� ���������� �볺���
        private void SendMessagesAllUsers(string text)
        {
            for (int i = 0; i < clientsList.Count; i++)
            {
                SendMessagesForUser(text, clientsList[i]);
            }
        }

        // ³������� ���������� �� 1 �볺���
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

        // ϳ�������� �� ����������� �������� ���������� � ���������� ��������� �������
        private void AddTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(tbServerStatistics.Text);
            sb.Append(str + "\r\n");
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // ���������� ������� � ����� ������
            tbServerStatistics.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)
        }
    }
}