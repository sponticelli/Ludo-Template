using Game.Platforms.Achievements;
using UnityEngine;

namespace Game.Platforms.Achievements
{
    public class NoDrmAchievementManager : AAchievementManager
    {
        public override void SetAchievement(Achievement achievement)
        {
            Debug.Log("Set achievement " + achievement.SteamID);
        }

        public override void ShowAchievementUI()
        {
        }
    }
}