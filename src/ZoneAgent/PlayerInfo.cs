namespace ZoneAgent
{
    /// <summary>
    /// Class to save player information
    /// </summary>
    class PlayerInfo
    {
        public Client Client { get; set; } //stores and returns Client refrence
        public string Account { get; set; } // stores and returns account name
        public string Time { get; set; } // stores and returns timestamp
        public bool Prepared { get; set; } // true=user prepared ; false=user not prepared
        public int ZoneStatus { get; set; } //true=user in game ; false=user not in game
        public string CharName { get; set; }
        /// <summary>
        /// Constructor that stores refrences
        /// </summary>
        /// <param name="account">account name</param>
        /// <param name="time">timestamp</param>
        /// <param name="prepared">user prepared</param>
        /// <param name="zoneStatus">zonestatus</param>
        public PlayerInfo(string account, string time, bool prepared, int zoneStatus)
        {
            Account = account;
            Time = time;
            Prepared = prepared;
            ZoneStatus = zoneStatus;
            CharName = null;
        }
    }
}
