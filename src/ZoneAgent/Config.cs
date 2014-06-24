﻿using System.Net;
namespace ZoneAgent
{
    /// <summary>
    /// Class that holds information about configuration of ZoneAgent and other servers
    /// AccountServer,ZoneServer,BattleServer and LoginServer
    /// Also defines constants
    /// </summary>
    class Config
    {
        //ZoneAgent information
        public static int SERVER_ID = 0;
        public static int AGENT_ID = 0;
        public static int CONNECTED_SERVER_COUNT = 0;
        public static IPAddress ZA_IP = IPAddress.Parse("127.0.0.1");
        public static int ZA_PORT = 9984;
        //Account server information
        public static IPAddress AS_IP = IPAddress.Parse("127.0.0.1");
        public static int AS_PORT = 5589;
        //Zone server information
        public static IPAddress ZS_IP = IPAddress.Parse("127.0.0.1");
        public static int ZS_PORT = 6689;
        //Battle server information
        public static IPAddress BS_IP = IPAddress.Parse("127.0.0.1");
        public static int BS_PORT = 6999;
        //Login server information
        public static IPAddress LS_IP = IPAddress.Parse("127.0.0.1");
        public static int LS_PORT = 3200;
        //Total player count
        public static int PLAYER_COUNT = 0;
        //Server Status
        public static bool isASConnected = false;
        public static bool isZSConnected = false;
        public static bool isBSConnected = false;
        public static bool isLSConnected = false;

        //Constants defining packet type
        public const int INVALID = -1;//if packet size 0 or <10
        public const int LOGIN_PACKET = 0;//login packet . packet size 56
        public const int AS_PACKET = 1; // Packet to be send to AccountServer
        public const int ZS_PACKET = 2; // Packet to be send to ZoneServer
        public const int BS_PACKET = 3; // PAcket to be send to BattleServer
        public const int DISCONNECT_PACKET = 4; // Disconnect packet

        //variables that is displayed in bar where ping is displayed
        public static string WZ = "[1.0]";
        public static string EXP = "[1.0]";
        public static string QUEST_EXP = "[1.0]";
        public static string DROP_RATE = "[1.0]";


    }
}