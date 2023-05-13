using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client_UdpClient
{
    public partial class Form1 : Form
    {
        int remotePort = 4569; // or 11000
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        // ����� ��� ��������� �� �������
        Socket listeningSocket = null;

        public Form1()
        {
            InitializeComponent();

            // ������� ������������ ����������
            btnSendMessage.Enabled = false;
            tbUserMessage.Enabled = false;
            tbUserName.Text = "USER";
        }

        // ���������� �� ������ ��������/³�'�������
        private void btnStartChat_Click(object sender, EventArgs e)
        {
            if(listeningSocket != null)
            {
                // ³��������� �� �������
                Form1_FormClosing(sender, null);

                // ������������ ���������� - ³�'������� �����
                btnStartChat.Text = "Start Chat";
                tbUserName.Enabled = true;
                tbUserMessage.Text = "";
                tbUserMessage.Enabled = false;
                btnSendMessage.Enabled = false;
            }
            else
            {
                // ϳ��������� �� �������
                try
                {
                    // ���� ���� ��� �������� ����� ����������� ����� - ����� �������� ������
                    if(tbUserName.Text.Trim().Length == 0)
                    {
                        MessageBox.Show("������� ������ ���� �����������");
                        return;
                    }

                    // ����������� ������
                    listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    
                    // �������� ��������� ������ �����������
                    // (�볺�� ��������� ����� �� ���������� ��������� ������������ � �������� ���� �����)
                    string messageReceived;
                    string message = $"#client-newUser#####{tbUserName.Text}";
                    byte[] data = Encoding.Default.GetBytes(message);
                    EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
                    // ³������� ������
                    listeningSocket.SendTo(data, remoteEndpoint);
                    // ��������� ����������� �� �������
                    messageReceived = MessageReceiver();

                    // �������� ����������� ������ ������ � ���, ���� �� ������ ��� ������������� ����������, ���� �������� �� ��������� �볺��
                    if(messageReceived.Substring(0, 20).Contains("#server-User-false##"))
                    {
                        MessageBox.Show("���������� � ����� ������\r\n��� ������������� � ���.\r\n������ ����� ����");
                        SocketClose();
                        return;
                    }

                    // ������ ��������� ��������������� ���������� �� ������� - ������ ����
                    Task.Run(() =>
                    {
                        Listen();
                    });

                    // ������������ ���������� - ��������� �����
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

        // ��������������� ����������� �� �������� ����������
        private void Listen()
        {
            try
            {
                // ���� ���������������
                while (true)
                {
                    // ��������� �����������
                    string messageReceived = MessageReceiver();

                    // ��������� ����������� � ��������� ���������
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

        // �������� ��������� ����������
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

        // ³���������� ����������� � ���������� ���������
        private void AddTextToTbChatMessages(string str)
        {
            StringBuilder builder = new StringBuilder(tbChatMessages.Text);
            builder.Append(str);
            tbChatMessages.Text = builder.ToString();
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // ���������� ������� � ����� ������
            tbChatMessages.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)
        }

        // �������� ��������� �� ����������� ���������� �� �볺��� �� �������
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            string messageNew = "#client-newMessage##" + 
                $"{DateTime.Now} " +
                $"\r\n   ����������� �� : {tbUserName.Text} " +
                $"\r\n   {tbUserMessage.Text}";

            byte[] data = Encoding.Default.GetBytes(messageNew);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);

            tbUserMessage.Clear();
        }

        // �������� �������� �'������� �� �������� �� ������ ����������� ��� ����� �볺��� � ���� 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���������� ��������� ������ � �������, ���� �볺�� �� ������� �� ����
            if (listeningSocket == null)
            {
                return;
            }

            string message = "#client-UserOut#####" + $"{DateTime.Now}\r\n   {tbUserName.Text} ������� ���";

            byte[] data = Encoding.Default.GetBytes(message);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);
            
            SocketClose();
        }

        // ���� ���������� - ������ �������� ���������� � ���������
        // �� ����������� ���������� ��� �������� ����������
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

        // �������� �'�������
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
