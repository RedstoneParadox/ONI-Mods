using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergisticsNotIncluded
{
    internal class Patches
    {
        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class Game_OnPrefabInit
        {
            public static void Postfix()
            {
                EnergisticsNotIncludedMod.meUtilityNetwork = new UtilityNetworkManager<Buildings.MEUtilityNetwork, Buildings.MECable>(Grid.WidthInCells, Grid.HeightInCells, 27);
                Debug.Log("Here!");
            }
        }
    }
}
