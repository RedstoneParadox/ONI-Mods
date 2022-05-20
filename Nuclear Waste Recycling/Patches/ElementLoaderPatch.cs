using HarmonyLib;
using Klei;
using NuclearWasteRecycling.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NuclearWasteRecycling.Patches
{
    // Credit: Aki (Slag)
    public class ElementLoaderPatch
    {
        internal static Hashtable substanceList;

        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public class ElementLoader_CollectElementsFromYAML_Patch
        {
            public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                var path = Path.Combine(Util.ModPath, "elements", "elements.yaml");
                var elementListText = ReadText(path);

                if (elementListText.IsNullOrWhiteSpace())
                {
                    return;
                }

                var elementList = YamlIO.Parse<ElementLoader.ElementEntryCollection>(elementListText, new FileHandle());
                __result.AddRange(elementList.elements);

                Elements.RegisterSubstances(substanceList);
            }

            private static string ReadText(string path)
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                    Debug.LogWarning($"Element configuration could not be read: {e.Message}\n" +
                        $"Some elements will not be added to the game.");

                    return null;
                }
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(ref Hashtable substanceList)
            {
                ElementLoaderPatch.substanceList = substanceList;
            }

            public static void Postfix()
            {
                Elements.SetSolidMaterials();
            }
        }

        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch("FinaliseElementsTable")]
        public static class ElementLoader_FinaliseElementsTable_Patch
        {
            public static void Postfix()
            {
                var uranium = ElementLoader.GetElement(SimHashes.EnrichedUranium.CreateTag());
                List<Tag> tags = new List<Tag>(new Tag[] { Tags.ReactorFuel });
                tags.AddRange(uranium.oreTags);
                uranium.oreTags = tags.ToArray();
            }
        }
    }
}
