﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Adit_Service
{
    class AditService
    {
        private static SocketAsyncEventArgs socketArgs;
        private static byte[] receiveBuffer;
        public static TcpClient TcpClient { get; set; }
        public static ServiceSocketMessages SocketMessageHandler { get; set; }
        public static Timer HeartbeatTimer { get; set; } = new Timer();
        private static int bufferSize = 9999999;
        public static bool IsConnected
        {
            get
            {
                return TcpClient?.Client?.Connected == true;
            }
        }
        public static string SessionID { get; set; }

        public static void Connect()
        {
            if (IsConnected)
            {
                return;
            }
            TcpClient = new TcpClient();
            try
            {
                TcpClient.Connect(Config.Current.ClientHost, Config.Current.ClientPort);
                TcpClient.Client.ReceiveBufferSize = bufferSize;
                TcpClient.Client.SendBufferSize = bufferSize;
                if (receiveBuffer == null)
                {
                    receiveBuffer = new byte[bufferSize];
                }
                socketArgs = new SocketAsyncEventArgs();
                socketArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                socketArgs.Completed += ReceiveFromServerCompleted;
            }
            catch
            {
                WaitToRetryConnection();
                return;
            }
            SocketMessageHandler = new ServiceSocketMessages(TcpClient.Client);
            WaitForServerMessage();
            SocketMessageHandler.SendConnectionType(ConnectionTypes.Service);
            SocketMessageHandler.SendHeartbeat();
        }

        public static void WaitToRetryConnection()
        {
            var timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = 30000;
            timer.Elapsed += (sender, args) =>
            {
                Utilities.WriteToLog("Failed to connect.");
                Connect();
            };
            timer.Start();
        }

        private static void WaitForServerMessage()
        {
            if (IsConnected)
            {
                var willFireCallback = TcpClient.Client.ReceiveAsync(socketArgs);
                if (!willFireCallback)
                {
                    ReceiveFromServerCompleted(TcpClient.Client, socketArgs);
                }
            }
        }


        private static void ReceiveFromServerCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Utilities.WriteToLog($"Socket closed in AditService: {e.SocketError.ToString()}");
                SessionID = String.Empty;
                return;
            }
            SocketMessageHandler.ProcessSocketMessage(e);
            WaitForServerMessage();
        }
    }
}
