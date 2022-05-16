using HarmonyLib;
using NuclearWasteRecycling.Buildings;

namespace NuclearWasteRecycling
{
    public class Patches
    {
        // Based off of Pholith's code.
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class BuildingInfoPatch
        {
            static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen("Radiation", NuclearWasteRecyclerConfig.ID);
            }

        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                Util.AddToTech("NuclearRefinement", NuclearWasteRecyclerConfig.ID);
            }

            public static void Postfix()
            {
                Debug.Log("I execute after Db.Initialize!");
            }
        }
    }
}
