using HVACExpansion.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.STRINGS
{
    public class RefrigerationUnitsUI
    {
        public class BuildingEffects
        {
            public static LocString HEATCONSUMED_AIRREFRIGERATOR = new LocString(UI.FormatAsLink("Heat", "HEAT") + ": -{0} (Approximate Value)", "STRINGS.UI.BUILDINGEFFECTS.HEATCONSUMED_AIRREFRIGERATOR");
            public static LocString HEATCONSUMED_LIQUIDREFRIGERATOR = new LocString(UI.FormatAsLink("Heat", "HEAT") + ": -{0} (Approximate Value)", "STRINGS.UI.BUILDINGEFFECTS.HEATCONSUMED_LIQUIDREFRIGERATOR");
            public static LocString GASHEATING = new LocString(UI.FormatAsLink("Heating factor", "HEAT") + ": {0}", "STRINGS.UI.BUILDINGEFFECTS.GASHEATING");
            public static LocString LIQUIDHEATING = new LocString(UI.FormatAsLink("Heating factor", "HEAT") + ": {0}", "STRINGS.UI.BUILDINGEFFECTS.LIQUIDHEATING");

            public class TOOLTIPS
            {
                public static LocString GASHEATING = new LocString("Increases the " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " of piped " + UI.PRE_KEYWORD + "Gases" + UI.PST_KEYWORD + " by <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.GASHEATING");
                public static LocString LIQUIDHEATING = new LocString("Increases the " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " of piped " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " by <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.LIQUIDHEATING");
                public static LocString HEATCONSUMED_AIRREFRIGERATOR = new LocString("Dissapates " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " based on the " +
                    UI.PRE_KEYWORD + "Volume" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Specific Heat Capacity" + UI.PST_KEYWORD + " of the pumped " +
                    UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + "\n\nHeating 1 " + UI.UNITSUFFIXES.MASS.KILOGRAM + " of " + ELEMENTS.OXYGEN.NAME +
                    " the entire <b>{1}</b> will consume <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_" + GasRefrigerationUnitConfig.ID.ToUpper());
                public static LocString HEATCONSUMED_LIQUIDREFRIGERATOR = new LocString("Dissapates " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " based on the " +
                    UI.PRE_KEYWORD + "Volume" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Specific Heat Capacity" + UI.PST_KEYWORD + " of the pumped " +
                    UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + "\n\nHeating 10 " + UI.UNITSUFFIXES.MASS.KILOGRAM + " of " + ELEMENTS.WATER.NAME +
                    " the entire <b>{1}</b> will consume <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_" + LiquidRefrigerationUnitConfig.ID.ToUpper());
            }
        }
    }
}
