using HarmonyLib;

namespace TechLib
{
    class TechPatches
    {
        [HarmonyPatch(typeof(Db), "Initialize")]
        private static class PatchFirst_Db_Initialize
        {
            [HarmonyPriority(Priority.First)]
            private static void Postfix(Db __instance)
            {
                TechTree.Create(__instance.Techs);
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        private static class Patch_GeneratedBuildings_LoadGeneratedBuildings
        {
            private static void Postfix()
            {
                TechTree.Instance.RebuildArragement();
            }
        }

    }
}
