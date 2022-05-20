using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NuclearWasteRecycling
{
    class Util
    {
        public static string ModPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
    }
}
