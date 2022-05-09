using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    class AutoCondenserConfig : IBuildingConfig
    {
        public const string ID = "AutoCondesner";
        private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>()
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal
        };
        public override BuildingDef CreateBuildingDef()
        {
            Debug.Log("Creating building def!");

            float[] mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] allMetals = MATERIALS.REFINED_METALS;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues decor = BUILDINGS.DECOR.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 3, "condenser_kanim", 30, 10f, mass, allMetals, 1600f, BuildLocationRule.OnFloor, decor, noise);

            BuildingTemplates.CreateElectricalBuildingDef(buildingDef);

            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityInputOffset = new CellOffset(0, 2);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0,0));

            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            FluidConverter converter = go.AddOrGet<FluidConverter>();
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            // ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go);

            converter.IsEvaporator = false;
            converter.temperatureDelta = 7.0f;
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = Util.GetMaxGasMass() * Util.GetThroughputPercent();
            conduitConsumer.capacityTag = HVACTags.FullyCondensable;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            storage.showInUI = true;
            storage.capacityKg = conduitConsumer.consumptionRate * 2f;
            storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            // go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
            go.AddOrGet<LoopingSounds>();
        }
    }
}
