using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.Locale
{
    public class Research
    {
        public class Techs
        {
            public class RefrigerationCycle
            {
                public static LocString NAME = new LocString(UI.FormatAsLink("Refrigeration Cycle", nameof(RefrigerationCycle)),
                    "STRINGS.RESEARCH.TECHS.REFRIGERATIONCYCLE.NAME");
                public static LocString DESC = new LocString("Evaporate and Condense " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gases", "ELEMENTS_GAS"),
                    "STRINGS.RESEARCH.TECHS.REFRIGERATIONCYCLE.DESC");
            }
        }
    }
}
