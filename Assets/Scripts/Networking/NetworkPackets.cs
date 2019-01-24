using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//get send from Server to Client
// client has to listen to serverpackets
public enum ServerPackets
{
    SConnectionOK = 1, StoC = 2,
}

//get send from Client to Server
// server has to listen to clientpackets
public enum ClientPackets
{
    CThankYou = 1, CtoS = 2, CAssignInfo = 3,
}

