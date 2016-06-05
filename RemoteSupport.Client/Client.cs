using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using RemoteSupport.Client.EventArguments;
using RemoteSupport.Client.Helpers;
using RemoteSupport.Shared;
using RemoteSupport.Shared.Enums;
using RemoteSupport.Shared.Messages;
using RemoteSupport.Shared.Models;

namespace RemoteSupport.Client
{
    public sealed class Client
    {
        private readonly List<ResponseCallbackObject> _callBacks;
        private Thread _receivingThread;
        private Thread _sendingThread;

        #region Constructors

        /// <summary>
        ///     Inisializes a new Client instance.
        /// </summary>
        public Client()
        {
            _callBacks = new List<ResponseCallbackObject>();
            MessageQueue = new List<MessageBase>();
            Status = StatusEnum.Disconnected;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The Client that is encapsulated by this client instance.
        /// </summary>
        public Socket SocketClient { get; set; }

        /// <summary>
        ///     The ip/domain address of the remote server.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        ///     The Port that is used to connect to the remote server.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        ///     The status of the client.
        /// </summary>
        public StatusEnum Status { get; private set; }

        /// <summary>
        ///     List containing all messages that is waiting to be delivered to the remote client/server
        /// </summary>
        public List<MessageBase> MessageQueue { get; }

        #endregion

        #region Events

        /// <summary>
        ///     Raises when a new session is requested by a remote client.
        /// </summary>
        public event Delegates.SessionRequestDelegate SessionRequest;

        /// <summary>
        ///     Raises when a new text message was received by the remote session client.
        /// </summary>
        public event Action<Client, string> TextMessageReceived;

        /// <summary>
        ///     Raises when a new file upload request was received by the remote session client.
        /// </summary>
        public event Delegates.FileUploadRequestDelegate FileUploadRequest;

        /// <summary>
        ///     Raises when a progress was made when a remote session client is uploading a file to this client instance.
        /// </summary>
        public event Action<Client, FileUploadProgressEventArguments> FileUploadProgress;

        /// <summary>
        ///     Raises when the client was disconnected;
        /// </summary>
        public event Action<Client> ClientDisconnected;

        /// <summary>
        ///     Raises when the remote session client was disconnected;
        /// </summary>
        public event Action<Client> SessionClientDisconnected;

        /// <summary>
        ///     Raises when a new unhandled message is received.
        /// </summary>
        public event Action<Client, GenericRequest> GenericRequestReceived;

        /// <summary>
        ///     Raises when the current session was ended by the remote client.
        /// </summary>
        public event Action<Client> SessionEndedByTheRemoteClient;

        #endregion

        #region Methods

        /// <summary>
        ///     Connect to a remote server.
        ///     (The client will not be able to perform any operations until it is loged in and validated).
        /// </summary>
        /// <param name="address">The server ip/domain address.</param>
        /// <param name="port">The server port.</param>
        public void Connect(string address, int port)
        {
            Address = address;
            Port = port;
            SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketClient.Connect(Address, Port);
            Status = StatusEnum.Connected;
            SocketClient.ReceiveBufferSize = 1024;
            SocketClient.SendBufferSize = 1024;

            _receivingThread = new Thread(ReceivingMethod);
            _receivingThread.IsBackground = true;
            _receivingThread.Start();

            _sendingThread = new Thread(SendingMethod);
            _sendingThread.IsBackground = true;
            _sendingThread.Start();
        }

        /// <summary>
        ///     Disconnect from the remote server.
        /// </summary>
        public void Disconnect()
        {
            MessageQueue.Clear();
            _callBacks.Clear();
            try
            {
                SendMessage(new DisconnectRequest());
            }
            catch
            {
                // ignored
            }
            Thread.Sleep(1000);
            Status = StatusEnum.Disconnected;
            SocketClient.Disconnect(false);
            SocketClient.Close();
            OnClientDisconnected();
        }

        /// <summary>
        ///     Log in to the remote server.
        /// </summary>
        /// <param name="email">The email address that will be used to identify this client instance.</param>
        /// <param name="callback">Will be invoked when a Validation Response was received from remote server.</param>
        public void Login(string email, Action<Client, ValidationResponse> callback)
        {
            //Create a new validation request message
            var request = new ValidationRequest();
            request.Email = email;

            //Add a callback before we send the message
            AddCallback(callback, request);

            //Send the message (Add it to the message queue)
            SendMessage(request);
        }

        /// <summary>
        ///     Request session from a remote client.
        /// </summary>
        /// <param name="email">The remote client email address (Case sensitive).</param>
        /// <param name="callback">Will be invoked when a Session Response was received from the remote client.</param>
        public void RequestSession(string email, Action<Client, SessionResponse> callback)
        {
            var request = new SessionRequest();
            request.Email = email;
            AddCallback(callback, request);
            SendMessage(request);
        }

        /// <summary>
        ///     Ends the current session with the remote user.
        /// </summary>
        /// <param name="callback">Will be invoked when an EndSession response was received from the server.</param>
        public void EndCurrentSession(Action<Client, EndSessionResponse> callback)
        {
            var request = new EndSessionRequest();
            AddCallback(callback, request);
            SendMessage(request);
        }

        /// <summary>
        ///     Watch the remote client's desktop.
        /// </summary>
        /// <param name="callback">Will be invoked when a RemoteDesktop Response was received.</param>
        public void RequestDesktop(Action<Client, RemoteDesktopResponse> callback)
        {
            var request = new RemoteDesktopRequest();
            AddCallback(callback, request);
            SendMessage(request);
        }

        /// <summary>
        ///     Send a text message to the remote client.
        /// </summary>
        /// <param name="message"></param>
        public void SendTextMessage(string message)
        {
            var request = new TextMessageRequest();
            request.Message = message;
            SendMessage(request);
        }

        /// <summary>
        ///     Upload a file to the remote client.
        /// </summary>
        /// <param name="fileName">The full file path to the file.</param>
        /// <param name="callback">Will be invoked when a progress is made in uploading the file</param>
        public void UploadFile(string fileName, Action<Client, FileUploadResponse> callback)
        {
            var request = new FileUploadRequest();
            request.SourceFilePath = fileName;
            request.FileName = Path.GetFileName(fileName);
            request.TotalBytes = FileHelper.GetFileLength(fileName);
            AddCallback(callback, request);
            SendMessage(request);
        }

        /// <summary>
        ///     Send a message of type MessageBase to the remote client.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(MessageBase message)
        {
            MessageQueue.Add(message);
        }

        /// <summary>
        ///     Send a message of type GenericResponse to the remote session client.
        /// </summary>
        /// <param name="response">A message of type GenericResponse.</param>
        public void SendGenericResponse(GenericResponse response)
        {
            var genericResponse = new GenericResponse(response);
            SendMessage(genericResponse);
        }

        #endregion

        #region Threads Methods

        private void SendingMethod(object obj)
        {
            while (Status != StatusEnum.Disconnected)
            {
                if (MessageQueue.Count > 0)
                {
                    var m = MessageQueue[0];

                    var f = new BinaryFormatter();
                    try
                    {
                        f.Serialize(new NetworkStream(SocketClient), m);
                    }
                    catch
                    {
                        Disconnect();
                    }

                    MessageQueue.Remove(m);
                }

                Thread.Sleep(30);
            }
        }

        private void ReceivingMethod(object obj)
        {
            while (Status != StatusEnum.Disconnected)
            {
                if (SocketClient.Available > 0)
                {
                    //try
                    //{
                    var f = new BinaryFormatter();
                    f.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                    var msg = f.Deserialize(new NetworkStream(SocketClient)) as MessageBase;
                    OnMessageReceived(msg);
                    //}
                    //catch (Exception e)
                    //{
                    //    Exception ex = new Exception("Unknown message recieved. Could not deserialize the stream.", e);
                    //    OnClientError(this, ex);
                    //    Debug.WriteLine(ex.Message);
                    //}
                }

                Thread.Sleep(30);
            }
        }

        #endregion

        #region Message Handlers

        private void OnMessageReceived(MessageBase msg)
        {
            var type = msg.GetType();

            if (msg is ResponseMessageBase)
            {
                if (type == typeof(GenericResponse))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    msg = (msg as GenericResponse).ExtractInnerMessage();
                }

                // ReSharper disable once PossibleNullReferenceException
                InvokeMessageCallback(msg, (msg as ResponseMessageBase).DeleteCallbackAfterInvoke);

                if (type == typeof(RemoteDesktopResponse))
                {
                    RemoteDesktopResponseHandler(msg as RemoteDesktopResponse);
                }
                else if (type == typeof(FileUploadResponse))
                {
                    FileUploadResponseHandler(msg as FileUploadResponse);
                }
            }
            else
            {
                if (type == typeof(SessionRequest))
                {
                    SessionRequestHandler(msg as SessionRequest);
                }
                else if (type == typeof(EndSessionRequest))
                {
                    OnSessionEndedByTheRemoteClient();
                }
                else if (type == typeof(RemoteDesktopRequest))
                {
                    RemoteDesktopRequestHandler(msg as RemoteDesktopRequest);
                }
                else if (type == typeof(TextMessageRequest))
                {
                    TextMessageRequestHandler(msg as TextMessageRequest);
                }
                else if (type == typeof(FileUploadRequest))
                {
                    FileUploadRequestHandler(msg as FileUploadRequest);
                }
                else if (type == typeof(DisconnectRequest))
                {
                    OnSessionClientDisconnected();
                }
                else if (type == typeof(GenericRequest))
                {
                    OnGenericRequestReceived(msg as GenericRequest);
                }
            }
        }

        private void RemoteDesktopResponseHandler(RemoteDesktopResponse response)
        {
            if (!response.Cancel)
            {
                var request = new RemoteDesktopRequest();
                request.CallbackId = response.CallbackId;
                SendMessage(request);
            }
            else
            {
                _callBacks.RemoveAll(x => x.Id == response.CallbackId);
            }
        }

        private void FileUploadResponseHandler(FileUploadResponse response)
        {
            var request = new FileUploadRequest(response);

            if (!response.HasError)
            {
                if (request.CurrentPosition < request.TotalBytes)
                {
                    request.BytesToWrite = FileHelper.SampleBytesFromFile(request.SourceFilePath,
                        request.CurrentPosition, request.BufferSize);
                    request.CurrentPosition += request.BufferSize;
                    SendMessage(request);
                }
            }
        }

        private void FileUploadRequestHandler(FileUploadRequest request)
        {
            var response = new FileUploadResponse(request);

            if (request.CurrentPosition == 0)
            {
                var args = new FileUploadRequestEventArguments(() =>
                {
                    //Confirm File Upload
                    response.DestinationFilePath = request.DestinationFilePath;
                    SendMessage(response);
                },
                    () =>
                    {
                        //Refuse File Upload
                        response.HasError = true;
                        response.Exception = new Exception("The file upload request was refused by the user!");
                        SendMessage(response);
                    });

                args.Request = request;
                OnFileUploadRequest(args);
            }
            else
            {
                FileHelper.AppendAllBytes(request.DestinationFilePath, request.BytesToWrite);
                SendMessage(response);
                OnUploadFileProgress(new FileUploadProgressEventArguments
                {
                    CurrentPosition = request.CurrentPosition,
                    FileName = request.FileName,
                    TotalBytes = request.TotalBytes,
                    DestinationPath = request.DestinationFilePath
                });
            }
        }

        private void TextMessageRequestHandler(TextMessageRequest request)
        {
            OnTextMessageReceived(request.Message);
        }

        private void RemoteDesktopRequestHandler(RemoteDesktopRequest request)
        {
            var response = new RemoteDesktopResponse(request);
            try
            {
                response.FrameBytes = RemoteDesktop.CaptureScreenToMemoryStream(request.Quality);
            }
            catch (Exception e)
            {
                response.HasError = true;
                response.Exception = e;
            }

            SendMessage(response);
        }

        private void SessionRequestHandler(SessionRequest request)
        {
            var response = new SessionResponse(request);

            var args = new SessionRequestEventArguments(() =>
            {
                //Confirm Session
                response.IsConfirmed = true;
                response.Email = request.Email;
                SendMessage(response);
            },
                () =>
                {
                    //Refuse Session
                    response.IsConfirmed = false;
                    response.Email = request.Email;
                    SendMessage(response);
                });

            args.Request = request;
            OnSessionRequest(args);
        }

        #endregion

        #region Callback Methods

        private void AddCallback(Delegate callBack, MessageBase msg)
        {
            if (callBack != null)
            {
                var callbackId = Guid.NewGuid();
                var responseCallback = new ResponseCallbackObject
                {
                    Id = callbackId,
                    CallBack = callBack
                };

                msg.CallbackId = callbackId;
                _callBacks.Add(responseCallback);
            }
        }

        private void InvokeMessageCallback(MessageBase msg, bool deleteCallback)
        {
            var callBackObject = _callBacks.SingleOrDefault(x => x.Id == msg.CallbackId);

            if (callBackObject != null)
            {
                if (deleteCallback)
                {
                    _callBacks.Remove(callBackObject);
                }
                callBackObject.CallBack.DynamicInvoke(this, msg);
            }
        }

        #endregion

        #region Virtuals

        private void OnSessionRequest(SessionRequestEventArguments args)
        {
            if (SessionRequest != null) SessionRequest(this, args);
        }

        private void OnFileUploadRequest(FileUploadRequestEventArguments args)
        {
            if (FileUploadRequest != null) FileUploadRequest(this, args);
        }

        private void OnTextMessageReceived(string txt)
        {
            if (TextMessageReceived != null) TextMessageReceived(this, txt);
        }

        private void OnUploadFileProgress(FileUploadProgressEventArguments args)
        {
            if (FileUploadProgress != null) FileUploadProgress(this, args);
        }

        private void OnClientDisconnected()
        {
            if (ClientDisconnected != null) ClientDisconnected(this);
        }

        private void OnSessionClientDisconnected()
        {
            if (SessionClientDisconnected != null) SessionClientDisconnected(this);
        }

        private void OnGenericRequestReceived(GenericRequest request)
        {
            if (GenericRequestReceived != null) GenericRequestReceived(this, request.ExtractInnerMessage());
        }

        private void OnSessionEndedByTheRemoteClient()
        {
            if (SessionEndedByTheRemoteClient != null) SessionEndedByTheRemoteClient(this);
        }

        #endregion
    }
}