using NuclearWasteRecycling.Content;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuclearWasteRecycling
{
    class LocaleStrings
    {
        public class ELEMENTS
        {
            public class MOXFUEL
            {
                public static LocString NAME = new LocString(UI.FormatAsLink("Radium MOX Fuel", Elements.MOXFuel.ToString()), "STRINGS.ELEMENTS.MOXFUEL.NAME");
                public static LocString DESC = new LocString("A nuclear fuel made from reactor waste products.", "STRINGS.ELEMENTS.MOXFUEL.DESC");
            }
        }

        public class BUILDINGS
        {
            public class PREFABS
            {
                public class NUCLEAR_WASTE_RECYCLER
                {
                    public static LocString NAME = new LocString(UI.FormatAsLink("Nuclear Waste Recycler", Elements.MOXFuel.ToString()), "STRINGS.BUILDINGS.PREFABS.NUCLEAR_WASTE_RECYCLER.NAME");
                    public static LocString DESC = new LocString("Nuclear Waste Recyclers process the byproducts of a nuclear reaction.", "STRINGS.BUILDINGS.PREFABS.NUCLEAR_WASTE_RECYCLER.DESC");
                    public static LocString EFFECT = new LocString($"Extracts various elements from {UI.FormatAsLink("Nuclear Waste", SimHashes.NuclearWaste.ToString())}.\n\nRadbolts can be used to change the quantities of those elements.", 
                        "STRINGS.BUILDINGS.PREFABS.NUCLEAR_WASTE_RECYCLER.EFFECT");
                }

                public class URANIUMCENTRIFUGE
                {
                    public static LocString MOX_FUEL_RECIPE_DESCRIPTION = new LocString($"Create {UI.FormatAsLink("Radium MOX Fuel", Elements.MOXFuel.ToString())} from {UI.FormatAsLink("Depleted Uranium", SimHashes.DepletedUranium.ToString())} and {UI.FormatAsLink("Radium", SimHashes.Radium.ToString())}",
                        "STRINGS.BUILDINGS.PREFABS.URANIUMCENTRIFUGE.MOX_FUEL_RECIPE_DESCRIPTION");
                }
            }
        }
    }
}
