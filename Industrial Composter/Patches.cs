using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Industrial_Composter
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
                Util.AddBuildingStrings
                    (
                        Locale.IndustrialComposter.NAME,
                        Locale.IndustrialComposter.DESC,
                        Locale.IndustrialComposter.EFFECT
                    );
                ModUtil.AddBuildingToPlanScreen("Refining", IndustrialComposterConfig.ID);
            }

        }

    }
}
