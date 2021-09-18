using HarmonyLib;
using KMod;
using RefrigerationUnits.Buildings;
using RefrigerationUnits;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RefrigerationUnits
{
    public class RefrigerationUnitsMod: UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            // let the game patch everything
            base.OnLoad(harmony);
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
            public static LocString GRU_NAME = STRINGS.BUILDINGS.GASREFRIGERATOR.NAME;
            public static LocString GRU_DESC = STRINGS.BUILDINGS.GASREFRIGERATOR.DESC;
            public static LocString GRU_EFFECT = STRINGS.BUILDINGS.GASREFRIGERATOR.EFFECT;
            public static LocString HEATCONSUMED_GRU = STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.HEATCONSUMED_AIRREFRIGERATOR;
            public static LocString HC_GRU_EFFECT = STRINGS.RefrigerationUnitsUI.BuildingEffects.HEATCONSUMED_AIRREFRIGERATOR;


            public static LocString LRU_NAME = STRINGS.BUILDINGS.LIQUIDREFRIGERATOR.NAME;
            public static LocString LRU_DESC = STRINGS.BUILDINGS.LIQUIDREFRIGERATOR.DESC;
            public static LocString LRU_EFFECT = STRINGS.BUILDINGS.LIQUIDREFRIGERATOR.EFFECT;
            public static LocString HEATCONSUMED_LRU = STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.HEATCONSUMED_LIQUIDREFRIGERATOR;
            public static LocString HC_LRU_EFFECT = STRINGS.RefrigerationUnitsUI.BuildingEffects.HEATCONSUMED_LIQUIDREFRIGERATOR;

            public static LocString LIQUIDHEATING = STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.LIQUIDHEATING;
            public static LocString GASHEATING = STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.GASHEATING;
            public static LocString LIQUIDHEATING_EFF = STRINGS.RefrigerationUnitsUI.BuildingEffects.LIQUIDHEATING;
            public static LocString GASHEATING_EFF = STRINGS.RefrigerationUnitsUI.BuildingEffects.GASHEATING;


            static void Prefix()
            {
                Util.AddBuildingStrings(GRU_NAME, GRU_DESC, GRU_EFFECT);
                Util.AddString(HEATCONSUMED_GRU);
                Util.AddString(HC_GRU_EFFECT);
                ModUtil.AddBuildingToPlanScreen("Utilities", GasRefrigerationUnitConfig.ID);

                Util.AddBuildingStrings(LRU_NAME, LRU_DESC, LRU_EFFECT);
                Util.AddString(HEATCONSUMED_LRU);
                Util.AddString(HC_LRU_EFFECT);
                ModUtil.AddBuildingToPlanScreen("Utilities", LiquidRefrigerationUnitConfig.ID);

                Util.AddString(LIQUIDHEATING);
                Util.AddString(GASHEATING);
                Util.AddString(LIQUIDHEATING_EFF);
                Util.AddString(GASHEATING_EFF);
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

        [HarmonyPatch(typeof(AirConditioner))]
        [HarmonyPatch("GetDescriptors")]
        public class AirConditionerPatch
        {
            public static void Postfix(ref List<Descriptor> __result, ref AirConditioner __instance)
            {
                if (__instance is RefrigerationUnit)
                {
                    AirConditioner @this = __instance;

                    List<Descriptor> descriptorList = new List<Descriptor>();
                    string formattedTemperature = GameUtil.GetFormattedTemperature(@this.temperatureDelta, interpretation: GameUtil.TemperatureInterpretation.Relative);
                    Element elementByName = ElementLoader.FindElementByName(@this.isLiquidConditioner ? "Water" : "Oxygen");
                    float dtu = (!@this.isLiquidConditioner ? Mathf.Abs((float)((double)@this.temperatureDelta * elementByName.specificHeatCapacity * 1000.0)) : Mathf.Abs((float)((double)@this.temperatureDelta * elementByName.specificHeatCapacity * 10000.0))) * 1f;
                    Descriptor descriptor1 = new Descriptor();
                    string txt = string.Format(@this.isLiquidConditioner ? STRINGS.RefrigerationUnitsUI.BuildingEffects.HEATCONSUMED_LIQUIDREFRIGERATOR : STRINGS.RefrigerationUnitsUI.BuildingEffects.HEATCONSUMED_AIRREFRIGERATOR, GameUtil.GetFormattedHeatEnergy(dtu), GameUtil.GetFormattedTemperature(Mathf.Abs(@this.temperatureDelta), interpretation: GameUtil.TemperatureInterpretation.Relative));
                    string tooltip = string.Format(
                        @this.isLiquidConditioner ? STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.HEATCONSUMED_LIQUIDREFRIGERATOR : STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.HEATCONSUMED_AIRREFRIGERATOR, 
                        GameUtil.GetFormattedHeatEnergy(dtu), 
                        GameUtil.GetFormattedTemperature(
                            Mathf.Abs(@this.temperatureDelta), 
                            interpretation: GameUtil.TemperatureInterpretation.Relative
                            )
                        );
                    descriptor1.SetupDescriptor(txt, tooltip);
                    descriptorList.Add(descriptor1);
                    Descriptor descriptor2 = new Descriptor();
                    descriptor2.SetupDescriptor(
                        string.Format(
                            @this.isLiquidConditioner ? STRINGS.RefrigerationUnitsUI.BuildingEffects.LIQUIDHEATING : STRINGS.RefrigerationUnitsUI.BuildingEffects.LIQUIDHEATING, 
                            formattedTemperature
                            ), 
                        string.Format(
                            @this.isLiquidConditioner ? STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.LIQUIDHEATING : STRINGS.RefrigerationUnitsUI.BuildingEffects.TOOLTIPS.GASHEATING, 
                            formattedTemperature
                            )
                        );
                    descriptorList.Add(descriptor2);
                    __result = descriptorList;
                }
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

        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        internal class Game_OnPrefabInit
        {
            internal static void Postfix(Game __instance)
            {
                Util.SetGameInstance(__instance);
            }
        }
    }
}
