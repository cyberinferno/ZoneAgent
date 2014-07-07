﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ZoneAgent
{
    public partial class Main : Form
    {
        #region ini file access
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        /// <summary>
        /// read the key value in the ini file
        /// </summary>
        /// <param name="Section">Section String</param>
        /// <param name="Key">Key String</param>
        /// <param name="iniPath">ini File Path String</param>
        /// <param name="defaultValue">default Return Value String</param>
        public String GetIniValue(String Section, String Key, String iniPath, String defaultValue = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return (temp.ToString() != "") ? temp.ToString() : defaultValue;
        }
        #endregion
        public Main()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Prompts user if user wants to close ZoneAgent or not
        /// if yes program exits else not
        /// </summary>
        private void btnclose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close ??", "ZoneAgent", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                ExitZoneAgent();
            }
        }
        /// <summary>
        /// executes when program starts
        /// </summary>
        private void Main_Load(object sender, EventArgs e)
        {
            //Checks if SvrInfo.ini is available or not.If not availabe exits ZoneAgent
            if(!File.Exists("SvrInfo.ini"))
            {
                MessageBox.Show("SvrInfo.ini file missing !!!", "ZoneAgent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("ZoneAgent.log", "Stop => File not found SvrInfo.ini");
                ExitZoneAgent();
            }
            //Checks if SvrInfo.ini is available or not.If not availabe exits ZoneAgent
            if (!File.Exists("msvcp100d.dll"))
            {
                MessageBox.Show("msvcp100d.dll file missing !!!", "ZoneAgent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("ZoneAgent.log", "Stop => File not found msvcp100d.dll");
                ExitZoneAgent();
            }
            //Checks if SvrInfo.ini is available or not.If not availabe exits ZoneAgent
            if (!File.Exists("msvcr100d.dll"))
            {
                MessageBox.Show("msvcr100d.dll file missing !!!", "ZoneAgent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("ZoneAgent.log", "Stop => File not found msvcr100d.dll");
                ExitZoneAgent();
            }
            //Checks if SvrInfo.ini is available or not.If not availabe exits ZoneAgent
            if (!File.Exists("asdecr.dll"))
            {
                MessageBox.Show("asdecr.dll file missing !!!", "ZoneAgent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Write("ZoneAgent.log", "Stop => File not found asdecr.dll");
                ExitZoneAgent();
            }
            LoadConfig();
            lblserverid.Text = Config.SERVER_ID.ToString();
            lblagentid.Text = Config.AGENT_ID.ToString();
            lblzoneport.Text = Config.ZA_PORT.ToString();
            new ZoneAgent();
        }

        /// <summary>
        /// loads values from Svrinfo.ini file to variables of Config class
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                string SvrInfo = Directory.GetCurrentDirectory() + @"\\SvrInfo.ini";
                //ZA
                Config.SERVER_ID = Int16.Parse(GetIniValue("STARTUP", "SERVERID", SvrInfo, "0"));
                Config.AGENT_ID = Int16.Parse(GetIniValue("STARTUP", "AGENTID", SvrInfo, "0"));
                Config.ZA_IP = IPAddress.Parse(GetIniValue("STARTUP", "IP", SvrInfo, "127.0.0.1"));
                Config.ZA_PORT = Int16.Parse(GetIniValue("STARTUP", "PORT", SvrInfo, "9984"));
                //LS
                Config.LS_IP = Int16.Parse(GetIniValue("LOGINSERVER", "IP", SvrInfo, "127.0.0.1"));
                Config.LS_PORT = Int16.Parse(GetIniValue("LOGINSERVER", "PORT", SvrInfo, "3200"));
                //AS
                Config.AS_ID = Int16.Parse(GetIniValue("ACCOUNTSERVER", "ID", SvrInfo, "255"));
                Config.AS_IP = Int16.Parse(GetIniValue("ACCOUNTSERVER", "IP", SvrInfo, "127.0.0.1"));
                Config.AS_PORT = Int16.Parse(GetIniValue("ACCOUNTSERVER", "PORT", SvrInfo, "5589"));
                //ZS
                Config.ZS_ID = Int16.Parse(GetIniValue("ZONESERVER", "ID", SvrInfo, "0"));
                Config.ZS_IP = Int16.Parse(GetIniValue("ZONESERVER", "IP", SvrInfo, "127.0.0.1"));
                Config.ZS_PORT = Int16.Parse(GetIniValue("ZONESERVER", "PORT", SvrInfo, "6689"));
                //BS
                Config.BS_ID = Int16.Parse(GetIniValue("BATTLESERVER", "ID", SvrInfo, "3"));
                Config.BS_IP = Int16.Parse(GetIniValue("BATTLESERVER", "IP", SvrInfo, "127.0.0.1"));
                Config.BS_PORT = Int16.Parse(GetIniValue("BATTLESERVER", "PORT", SvrInfo, "6999"));
            }
            catch (Exception reader)
            {
                Logger.Write(Logger.GetLoggerFileName("ZoneAgent"), "StreamReader : "+reader.ToString());
                ExitZoneAgent();
            }
        }
        
        /// <summary>
        /// Exits ZoneAgent
        /// </summary>
        private void ExitZoneAgent()
        {
            Application.ExitThread();
            Application.Exit();
        }
        
        /// <summary>
        /// refreshes status of Servers connected/disconnected on form every 2 seconds
        /// </summary>
        private void refreshzonestatus_Tick(object sender, EventArgs e)
        {
            Config.CONNECTED_SERVER_COUNT = 0;
            lstzone.Items.Clear();
            if (Config.isASConnected)
                lstzone.Items.Add(Config.AS_IP + ":" + Config.AS_PORT + ": Connected");
            else
                lstzone.Items.Add(Config.AS_IP + ":" + Config.AS_PORT + ": Disconnected");
            if (Config.isZSConnected)
                lstzone.Items.Add(Config.ZS_IP + ":" + Config.ZS_PORT + ": Connected");
            else
                lstzone.Items.Add(Config.ZS_IP + ":" + Config.ZS_PORT + ": Disconnected");
            if (Config.isBSConnected)
                lstzone.Items.Add(Config.BS_IP + ":" + Config.BS_PORT + ": Connected");
            else
                lstzone.Items.Add(Config.BS_IP + ":" + Config.BS_PORT + ": Disconnected");
            if (Config.isLSConnected)
                lbllssockstatus.Text = "Login Server : Connected";
            else
                lbllssockstatus.Text = "Login Server : Disconnected";
            if (Config.isASConnected)
                Config.CONNECTED_SERVER_COUNT++;
            if (Config.isZSConnected)
                Config.CONNECTED_SERVER_COUNT++;
            if (Config.isBSConnected)
                Config.CONNECTED_SERVER_COUNT++;
            lblconnectedzonecount.Text = Config.CONNECTED_SERVER_COUNT.ToString();
        }
        /// <summary>
        /// Executes when form is closed
        /// </summary>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Write("ZoneAgent.log", "Stop => Closed");
        }
    }
}
