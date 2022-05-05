using HVACExpansion.Buildings;
using TUNING;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    class ThermoConditionerConfig: IBuildingConfig
    {
        public const string ID = "GasRefrigerationUnit";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] allMetals = MATERIALS.ALL_METALS;
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "air_heater_kanim", 100, 120f, tieR3, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
            BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.ThermalConductivity = 5f;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            AirConditioner airConditioner = go.AddOrGet<FluidHeater>();
            airConditioner.temperatureDelta = 14f;
            airConditioner.maxEnvironmentDelta = 50f;
            BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = Util.GetMaxGasMass() * Util.GetThroughputPercent();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
            go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 16f;
        }
    }
}
