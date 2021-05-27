using Harmony;
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
                    if (__instance.name == "GasRefigerationUnitComplete")
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

            public static LocString GRU_DESC = new LocString("Cools its immediate vicinity, but heats the gas piped through it.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".DESC");

            public static LocString GRU_EFFECT = new LocString("Transforms " + STRINGS.UI.FormatAsLink("Dirty water", "DIRTYWATER") + " into " + STRINGS.UI.FormatAsLink("Polluted oxygen", "CONTAMINATEDOXYGEN") + " and " + STRINGS.UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");



            static void Prefix()
            {
                Strings.Add(GRU_NAME.key.String, GRU_NAME.text);
                Strings.Add(GRU_DESC.key.String, GRU_DESC.text);
                Strings.Add(GRU_EFFECT.key.String, GRU_EFFECT.text);
                ModUtil.AddBuildingToPlanScreen("Utilities", GasRefrigerationUnitConfig.ID);
            }

            static void Postfix()
            {
                object obj = Activator.CreateInstance(typeof(GasRefrigerationUnitConfig));
                BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
            }

        }
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DbPatch
        {
            public static void Prefix()
            {
                Debug.Log("Adding new buildings to technologies!");
                Db.Get().Techs.Get("HVAC").unlockedItemIDs.Add(GasRefrigerationUnitConfig.ID);
            }
        }
    }
}
