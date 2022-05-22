﻿using HarmonyLib;
using UnityEngine;

namespace LiquidTint
{
    public class Patches
    {
        [HarmonyPatch(typeof(LiquidConditionerConfig),"DoPostConfigureComplete")]
        public class LiquidConditionerConfig_DoPostConfigureCompletePatch
        {
            public static void Prefix(GameObject go)
            {
                StorageLiquidTint tint = go.AddOrGet<StorageLiquidTint>();
                tint.symbols = new string[] { "liquid" };
            }
        }
    }
}
