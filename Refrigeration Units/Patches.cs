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
                var kAnimBase = __instance.GetComponent<KAnimControllerBase>();
                if (kAnimBase != null)
                {
                    if (__instance.name == "GasRefrigerationUnitComplete")
                    {
                        float r = 255;
                        float g = 190;
                        float b = 102;
                        kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                    }
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

            static void Prefix()
            {
                Strings.Add(GRU_NAME.key.String, GRU_NAME.text);
                Strings.Add(GRU_DESC.key.String, GRU_DESC.text);
                Strings.Add(GRU_EFFECT.key.String, GRU_EFFECT.text);
                ModUtil.AddBuildingToPlanScreen("Utilities", GasRefrigerationUnitConfig.ID);
            }

        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DbPatch
        {
            public static void Postfix()
            {
                Db.Get().Techs.Get("HVAC").unlockedItemIDs.Add(GasRefrigerationUnitConfig.ID);
            }
        }
    }
}
