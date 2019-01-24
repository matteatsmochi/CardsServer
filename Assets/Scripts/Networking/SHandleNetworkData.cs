using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHandleNetworkData : MonoBehaviour
{
    private delegate void Packet_(int index, byte[] data);
    private static Dictionary<int, Packet_> Packets;

    void Start()
    {

    }
    
    public static void InitializeNetworkPackages()
    {
        Debug.Log("Initialize Network Packages");
        Packets = new Dictionary<int, Packet_> { };
        Packets.Add((int)ClientPackets.CThankYou, HandleConnectionOK);
        Packets.Add((int)ClientPackets.CAssignInfo, HandleAssignInfo);
        Packets.Add((int)ClientPackets.CtoS, HandleCtoS);
    }

    public static void HandleNetworkInformation(int index, byte[] data)
    {
        int packetnum; PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        packetnum = buffer.ReadInteger();
        buffer.Dispose();

        if (Packets.TryGetValue(packetnum, out Packet_ Packet))
        {
            Packet.Invoke(index, data);
        }
    }

    private static void HandleConnectionOK(int index, byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();
        Debug.Log(msg);
        
    }

    private static void HandleAssignInfo(int index, byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();
        Debug.Log(msg);
        
    }

    private static void HandleCtoS(int index, byte[] data)
    {
        PacketBuffer buffer = new PacketBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        string msg = buffer.ReadString();
        buffer.Dispose();

        // Add Code to Execute

        Debug.Log(msg);
        
    }
}
