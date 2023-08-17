using HVACExpansion.Buildings;
using KMod;
using Newtonsoft.Json;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace HVACExpansion
{
    class Util
    {
        private static double throughputPercent = 1.0;
        private static double maxLiquidMass = 10.0;
        private static double maxGasMass = 1.0;

        public static void LoadCustomizeBuildingsOptions()
        {
            string dir = Manager.GetDirectory() + "/CustomizeBuildings.json";
            StreamReader r = new StreamReader(dir);
            string json = r.ReadToEnd();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            throughputPercent = double.Parse(dictionary["PipeThroughputPercent"].ToString());
            maxLiquidMass = double.Parse(dictionary["PipeLiquidMaxPressure"].ToString());
            maxGasMass = double.Parse(dictionary["PipeGasMaxPressure"].ToString());
        }

        public static float GetThroughputPercent() => (float)throughputPercent;

        public static float GetMaxLiquidMass() => (float)maxLiquidMass;

        public static float GetMaxGasMass() => (float)maxGasMass;

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

        public static LocString WithKey(LocString @string, string key)
        {
            @string.SetKey(key);
            return @string;
        }
    }
}
