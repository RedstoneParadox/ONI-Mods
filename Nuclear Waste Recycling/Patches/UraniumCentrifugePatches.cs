using HarmonyLib;
using NuclearWasteRecycling.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuclearWasteRecycling.Patches
{
    class UraniumCentrifugePatches
    {
        [HarmonyPatch(typeof(UraniumCentrifuge), "DropEnrichedProducts")]
        public class UraniumCentrifuge_DropEnrichedProducts_Patch
        {
            public static void Postfix(ref UraniumCentrifuge _instance)
            {
                foreach (Storage storage in _instance.GetComponents<Storage>())
                {
                    storage.Drop(ElementLoader.FindElementByHash(Elements.MOXFuel).tag);
                }
            }
        }

        [HarmonyPatch(typeof(UraniumCentrifugeConfig), "ConfigureBuildingTemplate")]
        public class UraniumCentrifugeConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix()
            {
                ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[3]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.DepletedUranium).tag, 2f),
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag, 1f),
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Radium).tag, 7f)
                };
                ComplexRecipe.RecipeElement[] results = new ComplexRecipe.RecipeElement[1]
                {
                    new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(Elements.MOXFuel).tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                };
                ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("UraniumCentrifuge", (IList<ComplexRecipe.RecipeElement>)ingredients, (IList<ComplexRecipe.RecipeElement>)results), ingredients, results)
                {
                    time = 40f,
                    nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                    description = LocaleStrings.BUILDINGS.PREFABS.URANIUMCENTRIFUGE.MOX_FUEL_RECIPE_DESCRIPTION,
                    fabricators = new List<Tag>()
                    {
                        TagManager.Create("UraniumCentrifuge")
                    }
                };
            }
        }
    }
}
