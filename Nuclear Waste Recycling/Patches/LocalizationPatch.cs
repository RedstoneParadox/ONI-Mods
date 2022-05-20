﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuclearWasteRecycling.Patches
{
    public class LocalizationPatch
    {
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                Util.AddString(LocaleStrings.ELEMENTS.MOXFUEL.NAME);
                Util.AddString(LocaleStrings.ELEMENTS.MOXFUEL.DESC);

                Util.AddString(LocaleStrings.BUILDINGS.PREFABS.NUCLEARWASTERECYCLER.NAME);
                Util.AddString(LocaleStrings.BUILDINGS.PREFABS.NUCLEARWASTERECYCLER.DESC);
                Util.AddString(LocaleStrings.BUILDINGS.PREFABS.NUCLEARWASTERECYCLER.EFFECT);

                Util.AddString(LocaleStrings.MISC.TAGS.REACTORFUEL);
            }
        }
    }
}