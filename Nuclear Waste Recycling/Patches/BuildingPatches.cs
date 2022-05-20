using HarmonyLib;
using NuclearWasteRecycling.Buildings;

namespace NuclearWasteRecycling
{
    public class BuildingPatches
    {
        // Based off of Pholith's code.
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class BuildingInfoPatch
        {
            static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen("HEP", NuclearWasteRecyclerConfig.ID);
            }

        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Util.AddToTech("NuclearRefinement", NuclearWasteRecyclerConfig.ID);
            }
        }
    }
}
