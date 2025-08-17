using System.Collections.Generic;

namespace Game.Platforms.Achievements
{
    public abstract class AAchievementManager
    {
        public PlatformManager PlatformManager;

        private List<Achievement> _achievements = new List<Achievement>(new Achievement[12]);


        public virtual void Initialize(PlatformManager platformManager)
        {
            PlatformManager = platformManager;
        }


        public abstract void SetAchievement(Achievement achievement);

        public abstract void ShowAchievementUI();
    }
}