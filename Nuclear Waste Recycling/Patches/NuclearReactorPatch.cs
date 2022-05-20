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
        public class UraniumCentrifugeConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(ref GameObject go)
            {
                ManualDeliveryKG manualDeliveryKg = go.GetComponent<ManualDeliveryKG>();
                manualDeliveryKg.RequestedItemTag = Tags.ReactorFuel;

                Reactor reactor = go.AddOrGet<Reactor>();
                Traverse.Create(reactor).Field("fuelTag").SetValue(Tags.ReactorFuel);
            }
        }
    }
}
