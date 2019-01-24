using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

public class ServerTCP : MonoBehaviour
{
    private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _buffer = new byte[1024];

    public static Client[] _clients = new Client[Constants.MAX_PLAYERS];


    void Awake()
    {
        Application.runInBackground = true;
        ServerTCP.SetupServer();
        SHandleNetworkData.InitializeNetworkPackages();
    }
    
    public static void SetupServer()
    {
        for (int i = 0; i < Constants.MAX_PLAYERS; i++)
        {
            _clients[i] = new Client();
        }
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
        _serverSocket.Listen(10);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    }

    private static void AcceptCallback (IAsyncResult ar)
    {
        Socket socket = _serverSocket.EndAccept(ar);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        for (int i = 0; i < Constants.MAX_PLAYERS; i++)
        {
            if (_clients[i].socket == null)
                {
                _clients[i].socket = socket;
                _clients[i].index = i;
                _clients[i].ip = socket.RemoteEndPoint.ToString();
                _clients[i].StartClient();
                Console.WriteLine("Connection from {0} received.", _clients[i].ip);
                SendConnectionOK(i);
                return;
                }
        }
    }

    public static void SendDataTo(int index, byte[]data)
    {
        byte[] sizeinfo = new byte[4];
        sizeinfo[0] = (byte)data.Length;
        sizeinfo[1] = (byte)(data.Length << 8);
        sizeinfo[1] = (byte)(data.Length << 16);
        sizeinfo[1] = (byte)(data.Length << 24);

        _clients[index].socket.Send(sizeinfo);
        _clients[index].socket.Send(data);
    }
    
    public static void SendConnectionOK(int index)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ServerPackets.SConnectionOK);
        buffer.WriteString("You connected to the server.");
        SendDataTo(index, buffer.ToArray());
        buffer.Dispose();
    }

    public static void ServerToClient(int index, string msg)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteInteger((int)ServerPackets.StoC);
        buffer.WriteString(msg);
        SendDataTo(index, buffer.ToArray());
        buffer.Dispose();
    }


}

public class Client
{
    public int index;
    public string ip;
    public string username;
    public Socket socket;
    public bool closing = false;
    private byte[] _buffer = new byte[1024];

    public void StartClient()
    {
        socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        closing = false;
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;

        try
        {
            int received = socket.EndReceive(ar);
            if (received <= 0)
            {
                CloseClient(index);
            } else
            {
                byte[] databuffer = new byte[received];
                Array.Copy(_buffer, databuffer, received);
                SHandleNetworkData.HandleNetworkInformation(index, databuffer);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
        }
        catch
        {
            CloseClient(index);
        }                
    }

    private void CloseClient(int index)
        {
        closing = true;
        Debug.Log("Connection from " + ip + " has been terminated.");
        //PlayerLeftGame
        socket.Close();

        ServerTCP._clients[index].ip = null;
        ServerTCP._clients[index].username = null;
        ServerTCP._clients[index].socket = null;
        ServerTCP._clients[index].closing = false;
        //ServerTCP._clients[index]._buffer = null;

    }
}
