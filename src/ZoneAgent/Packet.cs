using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZoneAgent
{
    /// <summary>
    /// Class for creating and  manipulating packets
    /// </summary>
    class Packet
    {
        /// <summary>
        /// Creating packet to connect to login server
        /// </summary>
        /// <returns>returns packet to connect to loginserver</returns>
        public static byte[] LoginServerConnectPacket()
        {
            var packet = CombineByteArray(new byte[] { 0x20 }, GetBytesFrom(GetNullString(7)));
            packet = CombineByteArray(packet, new byte[] { 0x02, 0xe0 });
            string aid = string.Format("{0:x}", Config.AGENT_ID);
            string sid = string.Format("{0:x}", Config.SERVER_ID);
            var tempByte = new[] { Convert.ToByte(sid, 16) };
            packet = CombineByteArray(packet, new[] { tempByte[0] });
            tempByte = new[] { Convert.ToByte(aid, 16) };
            packet = CombineByteArray(packet, new[] { tempByte[0] });
            packet = CombineByteArray(packet, GetBytesFrom(Config.ZA_IP.ToString()));
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(16 - Config.ZA_IP.ToString().Length)));
            packet = CombineByteArray(packet, CreateReverseHexPacket(Config.ZA_PORT));
            return packet;
        }
        /// <summary>
        /// Creating packet to connect to AccountServer,ZoneServer,BattleServer
        /// </summary>
        /// <returns>Returns packet to connect to Zone</returns>
        public static byte[] ZoneConnectPacket()
        {
            byte[] packet = new byte[] { 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xE0};
            string aid = string.Format("{0:x}", Config.AGENT_ID);
            var tempByte = new[] { Convert.ToByte(aid, 16) };
            packet = CombineByteArray(packet, new[] { tempByte[0] });
            return packet;
        }
        /// <summary>
        /// Adding reverse of Client ID to the packet received from client
        /// </summary>
        /// <param name="pack">packet data</param>
        /// <param name="id">client id</param>
        /// <param name="size">size of packet</param>
        /// <returns>returns packet with ClientId contains in it</returns>
        public static byte[] AddClientID(byte[] pack, int id, int size)
        {
            byte[] packet = new byte[size];
            Array.Copy(pack, 0, packet, 0, 4);
            var temp = CreateReverseHexPacket(id);
            Array.Copy(temp, 0, packet, 4, temp.Length);
            Array.Copy(pack, 8, packet, 8, size - 8);
            return packet;
        }

        /// <summary>
        /// Gets character name from packet starting from specified index value
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <param name="index">index value</param>
        /// <returns>returns string fetched from packet data</returns>
        public static string GetCharName(byte[] packet, int index)
        {
            string character = "";
            character = Encoding.Default.GetString(packet, index, 20).TrimEnd('\0');
            return character;
        }
        /// <summary>
        /// Gets Client ID from packet
        /// </summary>
        /// <param name="data">packet data</param>
        /// <returns>returns integer value that represents Client ID</returns>
        public static int GetClientId(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }
        /// <summary>
        /// Alter charcter packet received from AccountServer(.acl file) according to 562 client
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <returns>returns 952 bytes packet that contains character info compatible</returns>
        public static byte[] AlterAccountServerPacket(byte[] packet)
        {
            var tempbytes = Crypt.Decrypt(packet);
            for (int i = 32; i <= 784; i += 188)
            {
                tempbytes[i + 3] = tempbytes[i + 2];
                tempbytes[i + 2] = tempbytes[i + 1];
                tempbytes[i + 1] = Convert.ToByte(1);
                tempbytes[i] = 0x00;
            }
            return Crypt.Encrypt(tempbytes);
        }

        /// <summary>
        /// Create client status packet to send to LoginServer
        /// </summary>
        /// <param name="clientId">client id</param>
        /// <param name="accountId">account name</param>
        /// <returns>returns client status info packet to be send to loginserver</returns>
        public static byte[] CreateClientStatusPacket(int clientId, string accountId)
        {
            var packet = new byte[4] { 0x1F, 0x00, 0x00, 0x00 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(clientId));
            var temp = new byte[] { 0x02, 0xe3 };
            packet = CombineByteArray(packet, CombineByteArray(temp, GetBytesFrom(accountId)));
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(31 - packet.Length)));
            return packet;
        }
        /// <summary>
        /// Create packet to get characters from Accountserver (this packet will be send to AccountServer)
        /// </summary>
        /// <param name="clientId">client Id</param>
        /// <param name="accountId">account name</param>
        /// <param name="ip">ip address of client</param>
        /// <returns>returns packet to get characters</returns>
        public static byte[] CreateGetCharacterPacket(int clientId, string accountId, string ip)
        {
            var packet = new byte[] { 0x92, 0x00, 0x00, 0x00 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(clientId));
            packet = CombineByteArray(packet, new byte[] { 0x01, 0xE1 });
            packet = CombineByteArray(packet, GetBytesFrom(accountId));
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(52 - packet.Length)));
            packet[34] = 0x01;
            packet = CombineByteArray(packet, GetBytesFrom(ip));
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(68 - packet.Length)));
            packet = CombineByteArray(packet, new byte[] { 0x1A, 0x00, 0x70, 0xDD, 0x18, 0x00, 0x00, 0x00, 0x69, 0x77, 0xF0, 0xDA, 0x93 });
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(146 - packet.Length)));
            return packet;
        }
        /// <summary>
        /// Report to LS total no. of players online every 5 seconds
        /// </summary>
        /// <returns>Packet to send report packet to loginserver</returns>
        public static byte[] LSReporter()
        {
            byte[] packet = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xE1 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(Config.PLAYER_COUNT));
            packet = CombineByteArray(packet, new byte[] { 0x03, 0x03 });
            return packet;
        }
        /// <summary>
        /// Gets current timestamp and returns it
        /// </summary>
        /// <returns>current timestamp</returns>
        public static string GetTime()
        {
            string time = DateTime.Now.ToString("yyyyMMdd") + '\0' + DateTime.Now.ToString("HHmmss") + '\0';
            return time;
        }

        /// <summary>
        /// Disconnect packet to send to LoginServer to disconnect player from LoginServer
        /// </summary>
        /// <param name="clientid">client id</param>
        /// <param name="username">account name</param>
        /// <param name="time">timestamp</param>
        /// <returns>packet to send disconnect request to loginserver</returns>
 
        public static byte[] SendDCToLS(int clientid,string username,string time)
        {
            byte[] packet = new byte[] { 0x30, 0x00, 0x00, 0x00 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(clientid));
            packet = CombineByteArray(packet, new byte[] { 0x02,0xE2,0x00});
            packet = CombineByteArray(packet, GetBytesFrom(username));
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(32 - packet.Length)));
            packet = CombineByteArray(packet,GetBytesFrom(GetTime()));
            return packet;
        }
        /// <summary>
        /// Disconnect packet to send to ZoneServer to disconnect player from ZoneServer
        /// </summary>
        /// <param name="clientid">client id</param>
        /// <returns>packet to disconnect character from zoneserver</returns>
        public static byte[] SendDCToASZS(int clientid)
        {
            byte[] packet = new byte[] { 0x0B, 0x00, 0x00, 0x00 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(clientid));
            packet = CombineByteArray(packet, new byte[] { 0x01, 0xE2, 0x00 });
            return packet;
        }
        /// <summary>
        /// Get packet type from packet
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <param name="length">length of packet</param>
        /// <returns>packet type (to which server packet is to be send)</returns>
        public static int GetPacketType(byte[] packet, int length, PlayerInfo playerInformation)
        {
            int packetType=Config.ZS_PACKET;
            if (playerInformation!=null &&  playerInformation.ZoneStatus == Config.BS_ID)
                packetType = Config.BS_ID;
            if (length == 0 || length <= 10)//For packet length 0 or <=10
                packetType = Config.INVALID;
            else if (packet[8] == 0x01 && packet[9] == 0xE2)//Login Packet
                packetType = Config.LOGIN_PACKET;
            else if (packet[10] == 0x06 && packet[11] == 0x11)//Validating character to enter game
                packetType = Config.AS_PACKET;
            else if (packet[10] == 0x22 && packet[11] == 0x23)//For KH Crest
                packetType = Config.AS_PACKET;
            else if (packet[10] == 0x23 && packet[11] == 0x23)//For KH Crest
                packetType = Config.AS_PACKET;
            else if (packet[10] == 0x08 && packet[11] == 0x11)//Disconnet Packet from ZS
                packetType = Config.DISCONNECT_PACKET;
            else if (packet[10] == 0x1B && packet[11] == 0x50 && playerInformation.ZoneStatus == Config.BS_ID)//Disconnet Packet from BS
                packetType = Config.DISCONNECT_PACKET;
            else if (packet[10] == 0x01 && packet[11] == 0xA0)//Create char packet
                packetType = Config.AS_PACKET;
            else if (packet[10] == 0x02 && packet[11] == 0xA0)//Delete char packet
                packetType = Config.AS_PACKET;
            else if (packet[10] == 0x00 && packet[11] == 0xC0)//Payment info packet
                packetType = Config.PAYMENT_PACKET;
            else if (packet[10] == 0x00 && packet[11] == 0x37)//BattleServer entry Packet
                packetType = Config.BS_PACKET;
            else if (packet[10] == 0x01 && packet[11] == 0x35)//BattleServer exit Packet
                packetType = Config.ZS_PACKET;
            return packetType;
        }
        /// <summary>
        /// if client returns multiple packets in one packet then this nethod will split and add client id to each one of the packet and combine again
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <param name="clientId">client id</param>
        /// <param name="length">length of packet</param>
        /// <returns>packet with client id</returns>
        public static byte[] CheckForMultiplePackets(byte[] packet,int clientId,int length)
        {
            byte[] packetLength=new byte[4];
            Array.Copy(packet, packetLength, 4);
            int packetLen = GetClientId(packetLength);
            if (packetLen == length)
                return AddClientID(packet,clientId,length);
            else
            {
                int i=0;
                byte[] returnPacket = new byte[length];
                byte[] temp;
                while (i < length)
                {
                    Array.Copy(packet, i, packetLength, 0, 4);
                    packetLen = GetClientId(packetLength);
                    temp = new byte[packetLen];
                    Array.Copy(packet, i, temp, 0, packetLen);
                    Array.Copy(AddClientID(temp,clientId,temp.Length), 0, returnPacket, i, packetLen);
                    i+=packetLen;
                }
                //File.WriteAllBytes("Changed_" + Environment.TickCount + "_" + length, returnPacket);
                return returnPacket;
            }
        }
        /// <summary>
        /// Will split packet and add packet to packet list
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <param name="length">length of packet</param>
        /// <param name="spltPackets">list of packet by reference</param>
        public static void SplitPackets(byte[] packet, int length,ref List<byte[]> spltPackets)
        {
            byte[] packetLength = new byte[4];
            Array.Copy(packet,0, packetLength,0, 4);
            int packetLen = GetClientId(packetLength);
            if (packetLen == length)
                spltPackets.Add(packet);
            else
            {
                byte[] temp;
                for (int i = 0; i < length;i=i+packetLen )
                {
                    Array.Copy(packet, i, packetLength, 0, 4);
                    packetLen = GetClientId(packetLength);
                    temp = new byte[packetLen];
                    Array.Copy(packet, i, temp, 0, packetLen);
                    spltPackets.Add(temp);
                    //File.WriteAllBytes("Changed_ZS_" + Environment.TickCount + "_" +i+"_"+ temp.Length, temp);
                    //i += packetLen;
                }
            }
        }
        /// <summary>
        /// Will make packet to display ping to player
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] DisplayPing(int clientID,long ping)
        {
            byte[] packet = new byte[4];
            packet = CreateReverseHexPacket(clientID);
            packet = CombineByteArray(packet, new byte[] { 0x03, 0xFF, 0x00, 0x18 });
            packet = CombineByteArray(packet, new byte[] { 0x0C, 0xFF, 0xFF, 0xFF, 0xFF, 0x4E, 0x4F, 0x54, 0x49, 0x43, 0x45, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            packet = CombineByteArray(packet, GetBytesFrom("Wz:"+Config.WZ+"  "));
            packet = CombineByteArray(packet, GetBytesFrom("Exp:" + Config.EXP + "  "));
            packet = CombineByteArray(packet, GetBytesFrom("Quest Exp:" + Config.QUEST_EXP + "  "));
            packet = CombineByteArray(packet, GetBytesFrom("Drop Rate:" + Config.DROP_RATE + "  "));
            if(ping<1000)
                packet = CombineByteArray(packet, GetBytesFrom("Ping:"+ping.ToString()+" ms"));
            else
                packet = CombineByteArray(packet, GetBytesFrom("Ping:---"));
            int MsgLength = packet.Length - 34;
            MsgLength %= 4;
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(4 - MsgLength)));
            packet = CombineByteArray(CreateReverseHexPacket(packet.Length + 4), packet);
            var tempBytes = Crypt.Encrypt(packet);
            return tempBytes;
            
        }
        /// <summary>
        /// To set status bar values like Wz, Exp,Quest Exp , Drop rate
        /// It will be used by DisplayPing timer to display ping
        /// </summary>
        /// <param name="packet">Data packet</param>
        public static void SetStatusValues(byte[] packet)
        {
            var statusPacket = Crypt.Decrypt(packet);
            string status = Encoding.Default.GetString(statusPacket);
            int i=0, j=0;
            for (int k = 0; k < 4; k++)
            {
                if (k == 0)
                {
                    i = status.IndexOf('[');
                    j = status.IndexOf(']');
                    Config.WZ = status.Substring(i + 1, (j - i) - 1).Trim();
                }
                else
                {
                    i = status.IndexOf('[', i);
                    j = status.IndexOf(']', j);
                }
                if (k == 1)
                    Config.EXP = status.Substring(i + 1, (j - i) - 1).Trim();
                if(k==2)
                    Config.QUEST_EXP=status.Substring(i + 1, (j - i) - 1).Trim();
                if(k==3)
                    Config.DROP_RATE = status.Substring(i + 1, (j - i) - 1).Trim();
                i++;
                j++;

            }
        }

        /// <summary>
        /// Payement information when client clicks Check Payment Info.
        /// </summary>
        /// <param name="clientID">uniq id of client</param>
        /// <returns>payement information</returns>
        public static byte[] PrivateMessage(int clientID,string message)
        {
            byte[] packet = new byte[4];
            packet = CreateReverseHexPacket(clientID);
            packet = CombineByteArray(packet, new byte[] { 0x03, 0xFF, 0x00, 0x18 });
            packet = CombineByteArray(packet, new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x53, 0x59, 0x53, 0x54, 0x45, 0x4D, 0x00, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0x00 });
            packet = CombineByteArray(packet, GetBytesFrom(message));
            int MsgLength = packet.Length - 34;
            MsgLength %= 4;
            packet = CombineByteArray(packet, GetBytesFrom(GetNullString(4 - MsgLength)));
            packet = CombineByteArray(CreateReverseHexPacket(packet.Length + 4), packet);
            var tempBytes = Crypt.Encrypt(packet);
            return tempBytes;
        }

        /// <summary>
        /// Combining 2 byte array
        /// </summary>
        /// <param name="a">byte array 1</param>
        /// <param name="b">byte array 2</param>
        /// <returns>returns combined byte array (byte array 1 + byte array 2)</returns>
        public static byte[] CombineByteArray(byte[] a, byte[] b)
        {
            var c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }
        /// <summary>
        /// getting byte[] from string
        /// </summary>
        /// <param name="str">string data</param>
        /// <returns>byte[] obtained from string data</returns>
        public static byte[] GetBytesFrom(string str)
        {
            Byte[] bytes = Encoding.Default.GetBytes(str);
            return bytes;
        }
        /// <summary>
        /// Creating reverse byte[4] of int32 value
        /// </summary>
        /// <param name="num">int value</param>
        /// <returns>reverse byte[] of int value</returns>
        private static byte[] CreateReverseHexPacket(int num)
        {
            byte[] byteArray = BitConverter.GetBytes(num);
            return byteArray;
        }
        /// <summary>
        /// Creating null string of specified no. of length
        /// </summary>
        /// <param name="length">length of packet</param>
        /// <returns>null string of specified length</returns>
        public static string GetNullString(int length)
        {
            string str = "";
            for (var i = 0; i < length; i++)
                str += char.ConvertFromUtf32(0);
            return str;
        }
        /// <summary>
        /// Triming packet according to length of packet
        /// </summary>
        /// <param name="packet">packet data</param>
        /// <param name="length">length of packet</param>
        /// <returns>trimed packet according to length specified</returns>
        public static byte[] TrimPacket(byte[] packet, int length)
        {
            var newPacket = new byte[] { 0x00 };
            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                    newPacket[i] = packet[i];
                else
                {
                    var temp = new[] { packet[i] };
                    newPacket = CombineByteArray(newPacket, temp);
                }
            }
            return newPacket;
        }
        public static byte[] DuplicateUserDCPacket(byte[] packet)
        {
            packet[8] = 0x02;
            packet[9] = 0xE2;
            var tempByte = new byte[32];
            Array.Copy(packet, 0, tempByte, 0, 32);
            tempByte = CombineByteArray(tempByte, GetBytesFrom(GetTime()));
            return tempByte;
        }
    }
}
