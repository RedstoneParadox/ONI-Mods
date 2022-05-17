using HarmonyLib;
using KMod;

namespace TechLib
{
    public class TechLibMod: UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            // let the game patch everything
            base.OnLoad(harmony);
        }
    }
}
