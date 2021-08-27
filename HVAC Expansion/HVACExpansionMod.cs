using HarmonyLib;
using KMod;

namespace HVACExpansion
{
    public class HVACExpansionMod: UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            // let the game patch everything
            base.OnLoad(harmony);
        }
    }
}
