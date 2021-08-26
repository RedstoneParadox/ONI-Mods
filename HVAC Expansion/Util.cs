using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HVACExpansion
{
    class Util
    {
        public static void ApplyBuildingTint(BuildingComplete instance, int r, int g, int b)
        {
            var kAnimBase = instance.GetComponent<KAnimControllerBase>();
            kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
        }

        public static void AddToTech(HashedString tech, string item)
        {
            Db.Get().Techs.Get(tech).unlockedItemIDs.Add(item);
        }

        public static void AddBuildingStrings(LocString name, LocString desc, LocString effect)
        {
            Strings.Add(name.key.String, name.text);
            Strings.Add(desc.key.String, desc.text);
            Strings.Add(effect.key.String, effect.text);
        }

        public static void AddString(LocString @string)
        {
            Strings.Add(@string.key.String, @string.text);
        }

        public static void ReapplyTints()
        {
            foreach (var building in Components.BuildingCompletes.Items)
            {
                if (building.name == GasRefrigerationUnitConfig.ID + "Complete")
                {
                    ApplyBuildingTint(building, 255, 190, 102);
                }
                else if (building.name == LiquidRefrigerationUnitConfig.ID + "Complete")
                {
                    ApplyBuildingTint(building, 255, 102, 102);
                }
            }
        }
    }
}
