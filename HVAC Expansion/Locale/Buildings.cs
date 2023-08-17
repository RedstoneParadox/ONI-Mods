using HVACExpansion.Buildings;

namespace HVACExpansion.Locale
{
    public class Buildings
    {
        public class AutoCondenser
        {
            public static LocString NAME = Util.WithKey(
                UI.FormatAsLink("Auto Condenser", nameof(AutoCondenser)),
                "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".NAME"
                );
            public static LocString DESC = new LocString("Cools and condenses gases.",
                "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(
                STRINGS.UI.FormatAsLink("Cools", "HEAT") + " piped " + STRINGS.UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " and condenses them into their " + STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " form.\n\n" + 
                "Gases with transition byproducts damage the machine. Gases that are too hot will be dumped.",
                "STRINGS.BUILDINGS.PREFABS." + AutoCondenserConfig.ID.ToUpper() + ".EFFECT"
                );
        }
        public class AutoEvaporator
        {
            public static LocString NAME = Util.WithKey(
                UI.FormatAsLink("Auto Evaporator", nameof(AutoEvaporator)),
                "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".NAME"
                );
            public static LocString DESC = new LocString("Heats and evaporates liquids.",
                "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(
                STRINGS.UI.FormatAsLink("Heats", "HEAT") + " piped " + STRINGS.UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " and evaporates them into their " + STRINGS.UI.FormatAsLink("Gaseous", "ELEMENTS_GAS") + " form.\n\n" 
                + "Liquids with transition byproducts damage the machine. Liquids that are too cold will be dumped.",
                "STRINGS.BUILDINGS.PREFABS." + AutoEvaporatorConfig.ID.ToUpper() + ".EFFECT"
                );
        }
        public class GasRefrigerator
        {
            public static LocString NAME = Util.WithKey(
                UI.FormatAsLink("Thermo Conditioner", nameof(GasRefrigerator)),
                "STRINGS.BUILDINGS.PREFABS." + ThermoConditionerConfig.ID.ToUpper() + ".NAME"
                );
            public static LocString DESC = new LocString("A thermo conditioner doesn't remove chill, but relocates it to a new area.",
                "STRINGS.BUILDINGS.PREFABS." + ThermoConditionerConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(STRINGS.UI.FormatAsLink("Heats", "HEAT") + " the " + STRINGS.UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + ThermoConditionerConfig.ID.ToUpper() + ".EFFECT");
        }

        public class LiquidRefrigerator
        {
            public static LocString NAME = Util.WithKey(
                UI.FormatAsLink("Thermo Aquacooler", nameof(LiquidRefrigerator)),
                "STRINGS.BUILDINGS.PREFABS." + ThermoAquacoolerConfig.ID.ToUpper() + ".NAME"
                );
            public static LocString DESC = new LocString("A thermo aquacooler heats liquids and outputs the chill elsewhere.",
                "STRINGS.BUILDINGS.PREFABS." + ThermoAquacoolerConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString(STRINGS.UI.FormatAsLink("Heats", "HEAT") + " the " + STRINGS.UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, cooling its immediate vicinity.",
                "STRINGS.BUILDINGS.PREFABS." + ThermoAquacoolerConfig.ID.ToUpper() + ".EFFECT");
        }
    }
}
