using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RemoteSupport.Server.EventArguments;

namespace RemoteSupport.Server.UI
{
    public partial class ServerForm : Form
    {
        private readonly Server _server;

        public ServerForm()
        {
            _server = new Server(8888);
            InitializeComponent();
            RegisterEvents();

            var updateListTimer = new Timer {Interval = 5000};
            updateListTimer.Tick += updateListTimer_Tick;
            updateListTimer.Start();
        }

        private void updateListTimer_Tick(object sender, EventArgs e)
        {
            UpdateClientsList();
        }

        private void RegisterEvents()
        {
            //this
            Load += ServerForm_Load;

            //Server
            _server.ClientValidating += server_ClientValidating;
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            _server.Start();
        }

        private void server_ClientValidating(ClientValidatingEventArgs args)
        {
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = emailRegex.Match(args.Request.Email);
            if (!_server.Receivers.Exists(x => x.Email == args.Request.Email) && match.Success)
            {
                args.Confirm();
            }
            else
            {
                args.Refuse();
            }
        }


        private void InvokeUi(Action action)
        {
            Invoke(action);
        }

        private void UpdateClientsList()
        {
            InvokeUi(() =>
            {
                listClients.Items.Clear();

                foreach (var receiver in _server.Receivers)
                {
                    var str = new string[5];
                    str[0] = receiver.ID.ToString();
                    str[1] = receiver.Email;
                    str[2] = receiver.Status.ToString();
                    str[3] = receiver.TotalBytesUsage.ToString();

                    if (receiver.OtherSideReceiver != null)
                    {
                        str[4] = receiver.OtherSideReceiver.Email;
                    }

                    var item = new ListViewItem(str);
                    listClients.Items.Add(item);
                }
            });
        }
    }
}