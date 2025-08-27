using HarmonyLib;
using EnergisticsNotIncluded.Buildings;
using KMod;

namespace EnergisticsNotIncluded
{
    public class EnergisticsNotIncludedMod : UserMod2
    {
        internal static UtilityNetworkManager<MEUtilityNetwork, MECable> meUtilityNetwork;
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
        }
    }
}
