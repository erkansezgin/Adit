﻿using Adit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adit.Server_Code
{
    public class AditServer
    {
        private static TcpListener tcpListener;
        private static int bufferSize;
        private static List<AditClient> clientList = new List<AditClient>();
        public static void Start()
        {
            if (tcpListener?.Server?.IsBound == true)
            {
                throw new Exception("Server is already running.");
            }
            tcpListener = TcpListener.Create(ServerSettings.Current.Port);
            bufferSize = tcpListener.Server.ReceiveBufferSize;
            var acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += acceptClientCompleted;
            tcpListener.Start();
            tcpListener.Server.AcceptAsync(acceptArgs);
        }

        public static bool IsEnabled
        {
            get
            {
                if (tcpListener == null)
                {
                    return false;
                }
                return tcpListener.Server.IsBound;
            }
        }
        public static int ClientCount
        {
            get
            {
                return clientList.Count;
            }
        }
        private static void acceptClientCompleted(object sender, SocketAsyncEventArgs e)
        {
            var aditClient = new AditClient();
            aditClient.Socket = e.AcceptSocket;
            clientList.Add(aditClient);
            handleClient(aditClient);
        }

        private static void handleClient(AditClient aditClient)
        {
            var receiveBuffer = new byte[bufferSize];
            var socketArgs = new SocketAsyncEventArgs();
            socketArgs.SetBuffer(0, bufferSize);
            socketArgs.UserToken = aditClient;
            socketArgs.BufferList = new List<ArraySegment<byte>>() { new ArraySegment<byte>(receiveBuffer) };
            socketArgs.Completed += receiveFromClientCompleted;
            aditClient.Socket.ReceiveAsync(socketArgs);
           
        }

        internal static void Stop()
        {
            throw new NotImplementedException();
        }

        private static void receiveFromClientCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                clientList.Remove(e.UserToken as AditClient);
                return;
            }
            handleClient(e.UserToken as AditClient);
        }
    }
}
