using HarmonyLib;
using HVACExpansion.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HVACExpansion.Science
{
    /// <summary>
    /// add research card to research screen
    /// </summary>
    [HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
    public class ResourceTreeLoader_Load_Patch
    {
        public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
        {
            TechUtil.AddNode(
                __instance,
                Techs.RefigerationCycleTechID,
                new string[]
                {
                    "HVAC", "LiquidTemperature"
                },
                xDiff: 1,
                yDiff: -1
                );
        }
    }

    /// <summary>
    /// Add research node to tree (thx Sgt_Imalas)
    /// </summary>
    [HarmonyPatch(typeof(Database.Techs), "Init")]
    public class Techs_TargetMethod_Patch
    {
        public static void Prefix()
        {
            Util.AddString(Locale.Research.Techs.RefrigerationCycle.NAME);
            Util.AddString(Locale.Research.Techs.RefrigerationCycle.DESC);
        }

        public static void Postfix(Database.Techs __instance)
        {
            Techs.RefrigerationCycleTech = new Tech(Techs.RefigerationCycleTechID, new List<string>
                {
                    AutoEvaporatorConfig.ID, AutoCondenserConfig.ID
                },
            __instance
            );
        }
    }
}
