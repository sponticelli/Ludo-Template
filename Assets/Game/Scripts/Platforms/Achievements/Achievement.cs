using System;

namespace Game.Platforms.Achievements
{
    [Serializable]
    public class Achievement
    {
        public string SteamID;

        public string AndroidID;

        public string IOSID;

        public Achievement(string SteamAndIosID, string AndroidID)
        {
            SteamID = SteamAndIosID;
            IOSID = SteamAndIosID;
            this.AndroidID = AndroidID;
        }
    }
}