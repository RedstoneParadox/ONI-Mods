using HVACExpansion.Buildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.Locale
{
    public class UI
    {
        public class BuildingEffects
        {
            public static LocString HEATCONSUMED_AIRREFRIGERATOR = new LocString(STRINGS.UI.FormatAsLink("Heat", "HEAT") + ": -{0} (Approximate Value)", "STRINGS.UI.BUILDINGEFFECTS.HEATCONSUMED_AIRREFRIGERATOR");
            public static LocString HEATCONSUMED_LIQUIDREFRIGERATOR = new LocString(STRINGS.UI.FormatAsLink("Heat", "HEAT") + ": -{0} (Approximate Value)", "STRINGS.UI.BUILDINGEFFECTS.HEATCONSUMED_LIQUIDREFRIGERATOR");
            public static LocString GASHEATING = new LocString(STRINGS.UI.FormatAsLink("Heating factor", "HEAT") + ": {0}", "STRINGS.UI.BUILDINGEFFECTS.GASHEATING");
            public static LocString LIQUIDHEATING = new LocString(STRINGS.UI.FormatAsLink("Heating factor", "HEAT") + ": {0}", "STRINGS.UI.BUILDINGEFFECTS.LIQUIDHEATING");

            public class Tooltips
            {
                public static LocString GASHEATING = new LocString("Increases the " + STRINGS.UI.PRE_KEYWORD + "Temperature" + STRINGS.UI.PST_KEYWORD + " of piped " + STRINGS.UI.PRE_KEYWORD + "Gases" + STRINGS.UI.PST_KEYWORD + " by <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.GASHEATING");
                public static LocString LIQUIDHEATING = new LocString("Increases the " + STRINGS.UI.PRE_KEYWORD + "Temperature" + STRINGS.UI.PST_KEYWORD + " of piped " + STRINGS.UI.PRE_KEYWORD + "Liquid" + STRINGS.UI.PST_KEYWORD + " by <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.LIQUIDHEATING");
                public static LocString HEATCONSUMED_AIRREFRIGERATOR = new LocString("Dissapates " + STRINGS.UI.PRE_KEYWORD + "Heat" + STRINGS.UI.PST_KEYWORD + " based on the " +
                    STRINGS.UI.PRE_KEYWORD + "Volume" + STRINGS.UI.PST_KEYWORD + " and " + STRINGS.UI.PRE_KEYWORD + "Specific Heat Capacity" + STRINGS.UI.PST_KEYWORD + " of the pumped " +
                    STRINGS.UI.PRE_KEYWORD + "Gas" + STRINGS.UI.PST_KEYWORD + "\n\nHeating 1 " + STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM + " of " + ELEMENTS.OXYGEN.NAME +
                    " the entire <b>{1}</b> will consume <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_" + ThermoConditionerConfig.ID.ToUpper());
                public static LocString HEATCONSUMED_LIQUIDREFRIGERATOR = new LocString("Dissapates " + STRINGS.UI.PRE_KEYWORD + "Heat" + STRINGS.UI.PST_KEYWORD + " based on the " +
                    STRINGS.UI.PRE_KEYWORD + "Volume" + STRINGS.UI.PST_KEYWORD + " and " + STRINGS.UI.PRE_KEYWORD + "Specific Heat Capacity" + STRINGS.UI.PST_KEYWORD + " of the pumped " +
                    STRINGS.UI.PRE_KEYWORD + "Liquid" + STRINGS.UI.PST_KEYWORD + "\n\nHeating 10 " + STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM + " of " + ELEMENTS.WATER.NAME +
                    " the entire <b>{1}</b> will consume <b>{0}</b>",
                    "STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_" + ThermoAquacoolerConfig.ID.ToUpper());
            }
        }

        internal static LocString FormatAsLink(string text, string linkID)
        {
            return STRINGS.UI.FormatAsLink(text, linkID);
        }
    }
}
