using Harmony;
using STRINGS;
using System;
using UnityEngine;

namespace RefrigerationUnits
{
    public static class Mod_OnLoad
    {
        public static void OnLoad()
        {
            Debug.Log("Hello world!");
        }
    }

    public class Patches
    {

        //ColorPatch. Thx Pholith!
        [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
        public class ColorPatch
        {
            public static void Postfix(BuildingComplete __instance)
            {
                if (__instance.name == GasRefrigerationUnitConfig.ID + "Complete") {
                    Util.ApplyBuildingTint(__instance, 255, 190, 102);
                }
                else if (__instance.name == LiquidRefrigerationUnitConfig.ID + "Complete") {
                    Util.ApplyBuildingTint(__instance, 255, 102, 102);
                }
            }
        }

        // Again based off of Pholith's code.
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class BuildingInfoPatch
        {
            public static LocString GRU_NAME = new LocString("Gas Refigeration Unit",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".NAME");

            public static LocString GRU_DESC = new LocString("A gas refrigeration unit doesn't remove heat, but relocates it to a new area.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".DESC");

            public static LocString GRU_EFFECT = new LocString(UI.FormatAsLink("Heats", "HEAT") + " the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");

            public static LocString LRU_NAME = new LocString("Liquid Refigeration Unit",
    "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".NAME");

            public static LocString LRU_DESC = new LocString("A liquid refrigeration unit heats liquids to cool its surroundings.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".DESC");

            public static LocString LRU_EFFECT = new LocString(UI.FormatAsLink("Heats", "HEAT") + " the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");

            static void Prefix()
            {
                Util.AddBuildingStrings(GRU_NAME, GRU_DESC, GRU_EFFECT);
                ModUtil.AddBuildingToPlanScreen("Utilities", GasRefrigerationUnitConfig.ID);

                Util.AddBuildingStrings(LRU_NAME, LRU_DESC, LRU_EFFECT);
                ModUtil.AddBuildingToPlanScreen("Utilities", LiquidRefrigerationUnitConfig.ID);
            }

        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DbPatch
        {
            public static void Postfix()
            {
                Util.AddToTech("HVAC", GasRefrigerationUnitConfig.ID);
                Util.AddToTech("LiquidTemperature", LiquidRefrigerationUnitConfig.ID);
            }
        }

        [HarmonyPatch(typeof(OverlayScreen))]
        [HarmonyPatch("ToggleOverlay")]
        public static class OverlayMenu_OnOverlayChanged_Patch
        {
            public static void Postfix(HashedString newMode)
            {
                Util.ReapplyTints();
            }
        }
    }
}
