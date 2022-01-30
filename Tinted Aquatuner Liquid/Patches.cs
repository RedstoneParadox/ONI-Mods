using HarmonyLib;
using UnityEngine;

namespace Tinted_Aquatuner_Liquid
{
    public class Patches
    {
        [HarmonyPatch(typeof(LiquidConditionerConfig),"DoPostConfigureComplete")]
        public class LiquidConditionerConfig_DoPostConfigureCompletePatch
        {
            public static void Prefix(GameObject go)
            {
                go.AddOrGet<AquatunerTint>();
            }
        }
    }
}
