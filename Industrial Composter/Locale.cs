using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Industrial_Composter
{
    internal class Locale
    {
        internal class IndustrialComposter
        {
            public static LocString NAME = Util.WithKey(
                STRINGS.UI.FormatAsLink("Industrial Composter", nameof(IndustrialComposter)),
                "STRINGS.BUILDINGS.PREFABS." + IndustrialComposterConfig.ID.ToUpper() + ".NAME"
                );
            public static LocString DESC = new LocString("Mechanized composter to eliminate manual labor.",
                "STRINGS.BUILDINGS.PREFABS." + IndustrialComposterConfig.ID.ToUpper() + ".DESC");
            public static LocString EFFECT = new LocString("Converts " + STRINGS.UI.FormatAsLink("Compostables", "COMPOSTABLE") + " into " + STRINGS.UI.FormatAsLink("dirt", "DIRT") +
                " in a much higher volume than its manual counterpart.",
                "STRINGS.BUILDINGS.PREFABS." + IndustrialComposterConfig.ID.ToUpper() + ".EFFECT"
                );
        }

        internal class Tech
        {
            internal class AdvancedAgiculture
            {
                public static LocString NAME = new LocString(STRINGS.UI.FormatAsLink("Advanced Agriculture", nameof(AdvancedAgiculture)),
                    "STRINGS.RESEARCH.TECHS.ADVANCEDAGRICULTURE.NAME");
                public static LocString DESC = new LocString("New machines for advanced agricultural automation",
                    "STRINGS.RESEARCH.TECHS.ADVANCEDAGRICULTURE.DESC");
            }
        }
    }
}
