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
            static void Prefix()
            {
                Util.AddBuildingStrings(
                    Locale.Buildings.GasRefrigerator.NAME,
                    Locale.Buildings.GasRefrigerator.DESC,
                    Locale.Buildings.GasRefrigerator.EFFECT
                    );
                Util.AddString(Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_AIRREFRIGERATOR);
                Util.AddString(Locale.UI.BuildingEffects.HEATCONSUMED_AIRREFRIGERATOR);
                ModUtil.AddBuildingToPlanScreen("Utilities", ThermoConditionerConfig.ID);

                Util.AddBuildingStrings(
                    Locale.Buildings.LiquidRefrigerator.NAME,
                    Locale.Buildings.LiquidRefrigerator.DESC,
                    Locale.Buildings.LiquidRefrigerator.EFFECT
                    );
                Util.AddString(Locale.UI.BuildingEffects.Tooltips.HEATCONSUMED_LIQUIDREFRIGERATOR);
                Util.AddString(Locale.UI.BuildingEffects.HEATCONSUMED_LIQUIDREFRIGERATOR);
                ModUtil.AddBuildingToPlanScreen("Utilities", ThermoAquacoolerConfig.ID);

                Util.AddString(Locale.UI.BuildingEffects.Tooltips.LIQUIDHEATING);
                Util.AddString(Locale.UI.BuildingEffects.Tooltips.GASHEATING);
                Util.AddString(Locale.UI.BuildingEffects.LIQUIDHEATING);
                Util.AddString(Locale.UI.BuildingEffects.GASHEATING);

                ModUtil.AddBuildingToPlanScreen("Utilities", AutoCondenserConfig.ID);
                Util.AddBuildingStrings(
                    Locale.Buildings.AutoCondenser.NAME, 
                    Locale.Buildings.AutoCondenser.DESC, 
                    Locale.Buildings.AutoCondenser.EFFECT
                    );

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
                Util.AddToTech("HVAC", ThermoConditionerConfig.ID);
                Util.AddToTech("LiquidTemperature", ThermoAquacoolerConfig.ID);
            }
        }

        [HarmonyPatch(typeof(AirConditioner))]
        [HarmonyPatch("GetDescriptors")]
        public class AirConditioner_GetDescriptorsPatch
        {
            public static void Postfix(ref List<Descriptor> __result, ref AirConditioner __instance)
            {
                if (__instance is FluidHeater)
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

        [HarmonyPatch(typeof(AirConditioner))]
        [HarmonyPatch("")]
        class AirConditioner_Sim200ms_Patch
        {
            public static void prefix(ref AirConditioner __instance)
            {
                if (__instance is FluidHeater)
                {
                    ((FluidHeater)__instance).UpdateTemperatureDelta();
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
