using HarmonyLib;
using NuclearWasteRecycling.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NuclearWasteRecycling.Patches
{
    class NuclearReactorPatch
    {
        [HarmonyPatch(typeof(NuclearReactorConfig), "ConfigureBuildingTemplate")]
        public class NuclearReactorConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(ref GameObject go)
            {
                ManualDeliveryKG manualDeliveryKg = go.GetComponent<ManualDeliveryKG>();
                manualDeliveryKg.RequestedItemTag = Tags.ReactorFuel;
            }
        }

        [HarmonyPatch(typeof(Reactor), "GetStoredFuel")]
        public class Reactor_GetStoredFuel_Patch
        {
            public static void Prefix(ref Reactor __instance)
            {
                Storage supplyStorage = Traverse.Create(__instance).Field("supplyStorage").GetValue<Storage>();
                Storage reactionStorage = Traverse.Create(__instance).Field("reactionStorage").GetValue<Storage>();
                var fuelTagField = Traverse.Create(__instance).Field<Tag>("fuelTag");
                
                if (reactionStorage.GetMassAvailable(fuelTagField.Value) < 0.25f)
                {
                    foreach (GameObject go in supplyStorage.items)
                    {
                        PrimaryElement primary = go.GetComponent<PrimaryElement>();

                        if (primary != null && primary.Element.oreTags.Contains(Tags.ReactorFuel))
                        {
                            fuelTagField.Value = primary.Element.tag;
                        }
                    }
                }
            }
        }
    }
}
