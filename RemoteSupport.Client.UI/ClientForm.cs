using System;
using System.Drawing;
using System.Windows.Forms;
using RemoteSupport.Client.EventArguments;
using RemoteSupport.Client.UI.MessagesExtensions;
using RemoteSupport.Shared.Enums;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Client.UI
{
    public partial class ClientForm : Form
    {
        private readonly Client _client;

        public ClientForm()
        {
            _client = new Client();

            InitializeComponent();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            //this
            btnConnect.Click += btnConnect_Click;
            btnLogin.Click += btnLogin_Click;
            btnStartSession.Click += btnStartSession_Click;
            btnRemoteDesktop.Click += btnRemoteDesktop_Click;
            btnSendMessage.Click += btnSendMessage_Click;
            btnUploadFile.Click += btnUploadFile_Click;
            btnDisconnect.Click += btnDisconnect_Click;
            btnEndSession.Click += btnEndSession_Click;
            FormClosing += ClientForm_FormClosing;

            //client
            _client.SessionRequest += client_SessionRequest;
            _client.TextMessageReceived += client_TextMessageReceived;
            _client.FileUploadRequest += client_FileUploadRequest;
            _client.FileUploadProgress += client_FileUploadProgress;
            _client.ClientDisconnected += client_ClientDisconnected;
            _client.SessionClientDisconnected += client_SessionClientDisconnected;
            _client.GenericRequestReceived += client_GenericRequestReceived;
            _client.SessionEndedByTheRemoteClient += client_SessionEndedByTheRemoteClient;
        }

        private void btnEndSession_Click(object sender, EventArgs e)
        {
            _client.EndCurrentSession((senderClient, response) =>
            {
                InvokeUi(() =>
                {
                    if (!response.HasError)
                    {
                        btnEndSession.Enabled = false;
                        btnStartSession.Enabled = true;
                        btnRemoteDesktop.Enabled = false;
                        btnSendMessage.Enabled = false;
                        btnUploadFile.Enabled = false;
                    }
                    else
                    {
                        Status(response.Exception.ToString());
                    }
                });
            });
        }

        private void client_SessionEndedByTheRemoteClient(Client client)
        {
            InvokeUi(() =>
            {
                MessageBox.Show(this, @"Sesion ended by the remote client.", Text);
                btnEndSession.Enabled = false;
                btnStartSession.Enabled = true;
                btnRemoteDesktop.Enabled = false;
                btnSendMessage.Enabled = false;
                btnUploadFile.Enabled = false;
            });
        }

        private static void client_GenericRequestReceived(Client client, GenericRequest msg)
        {
            if (msg.GetType() == typeof(CalcMessageRequest))
            {
                var request = msg as CalcMessageRequest;

                var response = new CalcMessageResponse(request);
                // ReSharper disable once PossibleNullReferenceException
                response.Result = request.A + request.B;
                client.SendGenericResponse(response);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_client.Status == StatusEnum.Connected)
            {
                _client.Disconnect();
            }
        }

        private void client_SessionClientDisconnected(Client obj)
        {
            InvokeUi(() =>
            {
                btnSendMessage.Enabled = false;
                btnEndSession.Enabled = false;
                btnRemoteDesktop.Enabled = false;
                btnUploadFile.Enabled = false;
            });

            Status("The remote session client was disconnected!");
        }

        private void client_ClientDisconnected(Client obj)
        {
            InvokeUi(() =>
            {
                btnDisconnect.Enabled = false;
                btnLogin.Enabled = false;
                btnSendMessage.Enabled = false;
                btnRemoteDesktop.Enabled = false;
                btnStartSession.Enabled = false;
                btnUploadFile.Enabled = false;
                btnConnect.Enabled = true;
                btnEndSession.Enabled = false;
            });

            Status("The client was disconnected!");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
        }

        private void client_FileUploadProgress(Client client, FileUploadProgressEventArguments args)
        {
            Status("Downloading " + args.FileName + " To " + args.DestinationPath + ", " +
                   args.CurrentPosition*100/args.TotalBytes + "%...");

            if (args.CurrentPosition >= args.TotalBytes)
            {
                Status("Downloading " + args.FileName + " Completed!");
            }
        }

        private void client_FileUploadRequest(Client client, FileUploadRequestEventArguments args)
        {
            InvokeUi(() =>
            {
                if (
                    MessageBox.Show(this,
                        @"File upload request, " + args.Request.FileName + @", " + args.Request.TotalBytes + @". Confirm?",
                        Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var dlg = new SaveFileDialog();
                    dlg.Title = Text + @" Save files as";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        args.Confirm(dlg.FileName);
                    }
                    else
                    {
                        args.Refuse();
                    }
                }
                else
                {
                    args.Refuse();
                }
            });
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = Text + @" select file to upload";
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _client.UploadFile(dlg.FileName, (clientSender, response) =>
            {
                Status("Uploading " + response.FileName + ", " + response.CurrentPosition*100/response.TotalBytes +
                       "%...");

                if (response.CurrentPosition >= response.TotalBytes)
                {
                    Status("Uploading " + response.FileName + " Completed!");
                }
            });
        }

        private void client_TextMessageReceived(Client sender, string message)
        {
            Status("Message received: " + message);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            var dlg = new DialogInput("Enter text message:");
            if (dlg.ShowDialog() != DialogResult.OK) return;
            _client.SendTextMessage(dlg.Result);
        }

        private void btnRemoteDesktop_Click(object sender, EventArgs e)
        {
            _client.RequestDesktop((clientSender, response) =>
            {
                panelPreview.BackgroundImage = new Bitmap(response.FrameBytes);
                response.FrameBytes.Dispose();
            });
        }

        private void client_SessionRequest(Client client, SessionRequestEventArguments args)
        {
            InvokeUi(() =>
            {
                if (
                    MessageBox.Show(this, @"Session request from " + args.Request.Email + @". Confirm request?", Text,
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    args.Confirm();
                    Status("Session started with " + args.Request.Email);

                    InvokeUi(() =>
                    {
                        btnSendMessage.Enabled = true;
                        btnRemoteDesktop.Enabled = true;
                        btnUploadFile.Enabled = true;
                        btnEndSession.Enabled = true;
                    });
                }
                else
                {
                    args.Refuse();
                }
            });
        }

        private void btnStartSession_Click(object sender, EventArgs e)
        {
            var dlg = new DialogInput("Please enter target user name:");
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _client.RequestSession(dlg.Result, (senderClient, args) =>
            {
                if (args.IsConfirmed)
                {
                    Status("Session started with " + dlg.Result);

                    InvokeUi(() =>
                    {
                        btnSendMessage.Enabled = true;
                        btnRemoteDesktop.Enabled = true;
                        btnUploadFile.Enabled = true;
                        btnEndSession.Enabled = true;
                    });
                }
                else
                {
                    Status(args.Exception.ToString());
                }
            });
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var dlg = new DialogInput("Please enter your user name:");
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _client.Login(dlg.Result, (senderClient, args) =>
            {
                if (args.IsValid)
                {
                    Status("User Validated!");
                    InvokeUi(() =>
                    {
                        Text = @"Client - " + dlg.Result;
                        btnStartSession.Enabled = true;
                        btnLogin.Enabled = false;
                    });
                }

                if (args.HasError)
                {
                    Status(args.Exception.ToString());
                }
            });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _client.Connect("localhost", 8888);
            btnLogin.Enabled = true;
            btnDisconnect.Enabled = true;
            btnConnect.Enabled = false;
        }

        private void Status(string str)
        {
            InvokeUi(() => { lbStatus.Text = str; });
        }

        private void InvokeUi(Action action)
        {
            Invoke(action);
        }
    }
}