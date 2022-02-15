using HarmonyLib;
using HVACExpansion.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PipLib.Tech;
using Database;

namespace HVACExpansion
{
    public class Patches
    {
        // Based off of Pholith's code.
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public class BuildingInfoPatch
        {
            public static LocString GRU_NAME = Locale.Buildings.GasRefrigerator.NAME;
            public static LocString GRU_DESC = Locale.Buildings.GasRefrigerator.DESC;
            public static LocString GRU_EFFECT = Locale.Buildings.GasRefrigerator.EFFECT;
            public static LocString HEATCONSUMED_GRU = Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_AIRREFRIGERATOR;
            public static LocString HC_GRU_EFFECT = Locale.UI.BuildingEffects.HEATCONSUMED_AIRREFRIGERATOR;


            public static LocString LRU_NAME = Locale.Buildings.LiquidRefrigerator.NAME;
            public static LocString LRU_DESC = Locale.Buildings.LiquidRefrigerator.DESC;
            public static LocString LRU_EFFECT = Locale.Buildings.LiquidRefrigerator.EFFECT;
            public static LocString HEATCONSUMED_LRU = Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_LIQUIDREFRIGERATOR;
            public static LocString HC_LRU_EFFECT = Locale.UI.BuildingEffects.HEATCONSUMED_LIQUIDREFRIGERATOR;

            public static LocString LIQUIDHEATING = Locale.UI.BuildingEffects.Tooltips.LIQUIDHEATING;
            public static LocString GASHEATING = Locale.UI.BuildingEffects.Tooltips.GASHEATING;
            public static LocString LIQUIDHEATING_EFF = Locale.UI.BuildingEffects.LIQUIDHEATING;
            public static LocString GASHEATING_EFF = Locale.UI.BuildingEffects.GASHEATING;


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

                ModUtil.AddBuildingToPlanScreen("Utilities", AutoCondenserConfig.ID);
                Util.AddBuildingStrings(Locale.Buildings.AutoCondenser.NAME, Locale.Buildings.AutoCondenser.DESC, Locale.Buildings.AutoCondenser.EFFECT);

                ModUtil.AddBuildingToPlanScreen("Utilities", AutoEvaporatorConfig.ID);
                Util.AddBuildingStrings(Locale.Buildings.AutoEvaporator.NAME, Locale.Buildings.AutoEvaporator.DESC, Locale.Buildings.AutoEvaporator.EFFECT);

                Util.AddString(Locale.Misc.Tags.FULLYCONDENSABLE);
                Util.AddString(Locale.Misc.Tags.FULLYEVAPORATABLE);
            }

        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DbPatch
        {
            public static void Postfix()
            {
                Util.AddToTech("HVAC", GasRefrigerationUnitConfig.ID);
                Util.AddToTech("LiquidTemperature", LiquidRefrigerationUnitConfig.ID);

                /*
                Techs techs = Db.Get().Techs;

                TechTree.CreateTech("RefrigerationCycle");
                TechTree.AddRequirement(techs.Get("RefrigerationCycle"), techs.Get("HVAC"));
                TechTree.AddRequirement(techs.Get("RefrigerationCycle"), techs.Get("LiquidTemperature"));
                
                if (DlcManager.IsExpansion1Active())
                {
                    TechTree.SetTier(techs.Get("RefrigerationCycle"), 5);
                }
                else
                {
                    TechTree.SetTier(techs.Get("RefrigerationCycle"), 5);
                }
                */

                Util.AddToTech("HVAC", AutoCondenserConfig.ID);
                Util.AddToTech("HVAC", AutoEvaporatorConfig.ID);
            }
        }

        [HarmonyPatch(typeof(AirConditioner))]
        [HarmonyPatch("GetDescriptors")]
        public class AirConditioner_GetDescriptorsPatch
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
                    string txt = string.Format(@this.isLiquidConditioner ? Locale.UI.BuildingEffects.HEATCONSUMED_LIQUIDREFRIGERATOR : Locale.UI.BuildingEffects.HEATCONSUMED_AIRREFRIGERATOR, GameUtil.GetFormattedHeatEnergy(dtu), GameUtil.GetFormattedTemperature(Mathf.Abs(@this.temperatureDelta), interpretation: GameUtil.TemperatureInterpretation.Relative));
                    string tooltip = string.Format(
                        @this.isLiquidConditioner ? Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_LIQUIDREFRIGERATOR : Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_AIRREFRIGERATOR, 
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
                            @this.isLiquidConditioner ? Locale.UI.BuildingEffects.LIQUIDHEATING : Locale.UI.BuildingEffects.LIQUIDHEATING, 
                            formattedTemperature
                            ), 
                        string.Format(
                            @this.isLiquidConditioner ? Locale.UI.BuildingEffects.Tooltips.LIQUIDHEATING : Locale.UI.BuildingEffects.Tooltips.GASHEATING, 
                            formattedTemperature
                            )
                        );
                    descriptorList.Add(descriptor2);
                    __result = descriptorList;
                }
            }
        }

        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch("FinaliseElementsTable")]
        public static class ElementLoader_FinaliseElementsTablePatch
        {
            public static void Postfix(ref Hashtable substanceList)
            {
                foreach (Element elem in ElementLoader.elements)
                {
                    if (elem.IsLiquid && (elem.highTempTransitionOreMassConversion == 0 || elem.highTempTransitionOreID == (SimHashes)0) && elem.highTempTransition != null && elem.highTempTransition.IsGas)
                    {
                        List<Tag> tags = new List<Tag>(new Tag[] { HVACTags.FullyEvaporatable });
                        tags.AddRange(elem.oreTags);
                        elem.oreTags = tags.ToArray();
                    }
                    else if (elem.IsGas && (elem.lowTempTransitionOreMassConversion == 0 || elem.lowTempTransitionOreID == (SimHashes)0) && elem.lowTempTransition != null && elem.lowTempTransition.IsLiquid)
                    {
                        List<Tag> tags = new List<Tag>(new Tag[] { HVACTags.FullyCondensable });
                        tags.AddRange(elem.oreTags);
                        elem.oreTags = tags.ToArray();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AudioSheets))]
        [HarmonyPatch("CreateSound")]
        public static class AudioSheets_CreateSoundPatch
        {
            public static void Prefix(string file_name, string anim_name, string type, float min_interval, string sound_name)
            {
                // Debug.Log("File name: " + file_name + " Sound name: " + sound_name);
            }
        }
    }
}
