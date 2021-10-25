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
        public class AutoCondenser
        {
            public static LocString NAME = new LocString("Auto Condenser",
    "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("Condenses gases.",
                "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString("Condenses piped " + STRINGS.UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " into their " + STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " form while maintaining temperature. Gases with transition byproducts damage the machine.",
                "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".EFFECT");
        }
        public class AutoEvaporator
        {
            public static LocString NAME = new LocString("Auto Evaporator",
    "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("Evaporates liquids.",
                "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString("Evaporates piped " + STRINGS.UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " into their " + STRINGS.UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " form while maintaining temperature. Liquids with transition byproducts damage the machine.",
                "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".EFFECT");
        }
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
            public static LocString NAME = new LocString("Thermo Aquacooler",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".NAME");
            public static LocString DESC = new LocString("A thermo aquacooler heats liquids to cool its surroundings.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(STRINGS.UI.FormatAsLink("Heats", "HEAT") + " the " + STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + LiquidRefrigerationUnitConfig.ID.ToUpper() + ".EFFECT");
        }
    }
}
