using HVACExpansion.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.Locale
{
    public class Buildings
    {
        public class GasRefrigerator
        {
            public static LocString NAME = new LocString("Thermo Conditioner",
    "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("A thermo conditioner doesn't remove heat, but relocates it to a new area.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(STRINGS.UI.FormatAsLink("Heats", "HEAT") + " the " + STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + GasRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");
        }

        public class LiquidRefrigerator
        {
            public static LocString NAME = new LocString("Thermo Aquapump",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("A thermo aquapump heats liquids to cool its surroundings.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(STRINGS.UI.FormatAsLink("Heats", "HEAT") + " the " + STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");
        }
    }
}
