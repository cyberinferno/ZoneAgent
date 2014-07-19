using System;
using System.Runtime.InteropServices;
namespace ZoneAgent
{
    /// <summary>
    /// Class for encrypting and decrypting packets
    /// </summary>
    class Crypt
    {
        /// <summary>
        /// To decrypt packet data
        /// </summary>
        /// <param name="packet">data</param>
        /// <returns>returns decrypted data</returns>
        public static byte[] Decrypt(byte[] packet)
        {
            //[0]-[11]: Packet Header
            for(int i = 12; i + 4 <= packet.Length; i += 4)
            {
                int DynamicKey = Config.m_DynamicKey;
                for (int j = i; j < i + 4; j++)
                {
                    byte pSrc = packet[j];
                    packet[j] = (byte)(packet[j] ^ (DynamicKey >> 8));
                    DynamicKey = (pSrc + DynamicKey) * Config.m_ConstKey1 + Config.m_ConstKey2;
                }
            }
            return packet;
        }
        /// <summary>
        /// To encrypt packet data
        /// </summary>
        /// <param name="packet">data</param>
        /// <returns>returns encrypted packet</returns>
        public static byte[] Encrypt(byte[] packet)
        {
            //[0]-[11]: Packet Header
            for (int i = 12; i + 4 <= packet.Length; i += 4)
            {
                int DynamicKey = Config.m_DynamicKey;
                for (int j = i; j < i + 4; j++)
                {
                    packet[j] = (byte)(packet[j] ^ (DynamicKey >> 8));
                    DynamicKey = (packet[j] + DynamicKey) * Config.m_ConstKey1 + Config.m_ConstKey2;
                }
            }
            return packet;
        }
    }
}
