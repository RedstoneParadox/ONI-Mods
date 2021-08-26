using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefrigerationUnits.STRINGS
{
    public class BUILDINGS
    {
        public class GASREFRIGERATOR
        {
            public static LocString NAME = new LocString("Gas Refrigeration Unit",
    "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("A gas refrigeration unit doesn't remove heat, but relocates it to a new area.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(UI.FormatAsLink("Heats", "HEAT") + " the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");
        }

        public class LIQUIDREFRIGERATOR
        {
            public static LocString NAME = new LocString("Liquid Refrigeration Unit",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("A liquid refrigeration unit heats liquids to cool its surroundings.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(UI.FormatAsLink("Heats", "HEAT") + " the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");
        }
    }
}
