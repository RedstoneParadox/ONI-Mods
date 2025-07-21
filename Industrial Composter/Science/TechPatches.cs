using HarmonyLib;
using Industrial_Composter;
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
                Techs.AdvancedAgricultureTechID,
                new string[]
                {
                    "FoodRepurposing"
                },
                xDiff: 1,
                yDiff: 0
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
            Industrial_Composter.Util.AddString(Locale.Tech.AdvancedAgiculture.NAME);
            Industrial_Composter.Util.AddString(Locale.Tech.AdvancedAgiculture.DESC);
        }

        public static void Postfix(Database.Techs __instance)
        {
            Techs.AdvancedAgricultureTech = new Tech(Techs.AdvancedAgricultureTechID, new List<string>
                {
                    IndustrialComposterConfig.ID
                },
            __instance
            );
        }
    }
}
