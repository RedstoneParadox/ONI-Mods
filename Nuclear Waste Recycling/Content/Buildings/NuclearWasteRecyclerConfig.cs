using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;

namespace NuclearWasteRecycling.Buildings
{
    class NuclearWasteRecyclerConfig : IBuildingConfig
    {
        public const string ID = "NuclearWasteRecycler";
        private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>()
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal
        };

        public override BuildingDef CreateBuildingDef()
        {
            float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] allMetals = MATERIALS.REFINED_METALS;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues decor = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 3, "nuclear_waste_recycler_kanim", 30, 10f, mass, allMetals, 1600f, BuildLocationRule.OnFloor, decor, noise);

            BuildingTemplates.CreateElectricalBuildingDef(buildingDef);

            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.UseHighEnergyParticleInputPort = true;
            buildingDef.HighEnergyParticleInputOffset = new CellOffset(1, 1);
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityInputOffset = new CellOffset(0, 2);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort((HashedString) "HEP_STORAGE", new CellOffset(0, 2), (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
            };

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            fabricator.duplicantOperated = false;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            energyParticleStorage.capacity = 400f;
            energyParticleStorage.autoStore = true;
            energyParticleStorage.PORT_ID = "HEP_STORAGE";
            energyParticleStorage.showCapacityStatusItem = true;
            ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

            ComplexRecipe.RecipeElement[] ingredients = new ComplexRecipe.RecipeElement[1]
            {
                new ComplexRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 1000f)
            };
            ComplexRecipe.RecipeElement[] normalResults = new ComplexRecipe.RecipeElement[4]
            {
                new ComplexRecipe.RecipeElement(SimHashes.EnrichedUranium.CreateTag(), 3f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.DepletedUranium.CreateTag(), 2f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.Radium.CreateTag(), 4f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
            };
            ComplexRecipe.RecipeElement[] irradiatedResults = new ComplexRecipe.RecipeElement[4]
            {
                new ComplexRecipe.RecipeElement(SimHashes.EnrichedUranium.CreateTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.DepletedUranium.CreateTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.Radium.CreateTag(), 6f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
                new ComplexRecipe.RecipeElement(SimHashes.Lead.CreateTag(), 2f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
            };

            ComplexRecipe regularRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, ingredients, normalResults), ingredients, normalResults, 1000, 0)
            {
                time = 80f,
                consumedHEP = 0,
                description = "",
                //description = string.Format(STRINGS.BUILDINGS.PREFABS.DIAMONDPRESS.REFINED_CARBON_RECIPE_DESCRIPTION, SimHashes.Diamond.CreateTag().ProperName(), SimHashes.RefinedCarbon.CreateTag().ProperName()),
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite,
                fabricators = new List<Tag>()
                {
                    TagManager.Create(ID)
                }
            };
            ComplexRecipe irriadiatedRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, ingredients, irradiatedResults) + "_2", ingredients, irradiatedResults, 1000, 0)
            {
                time = 80f,
                consumedHEP = 200,
                description = "",
                //description = string.Format(STRINGS.BUILDINGS.PREFABS.DIAMONDPRESS.REFINED_CARBON_RECIPE_DESCRIPTION, SimHashes.Diamond.CreateTag().ProperName(), SimHashes.RefinedCarbon.CreateTag().ProperName()),
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite,
                fabricators = new List<Tag>()
                {
                    TagManager.Create(ID)
                }
            };

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 1.0f;

            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            
        }
    }
}
