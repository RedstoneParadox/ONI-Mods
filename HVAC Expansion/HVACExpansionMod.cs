using HarmonyLib;
using KMod;

namespace HVACExpansion
{
    public class HVACExpansionMod: UserMod2
    {
        public static Tech RefrigerationCycleTech;

        public override void OnLoad(Harmony harmony)
        {
            // let the game patch everything
            base.OnLoad(harmony);
        }
    }
}
