using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Industrial_Composter
{
    public class IndustrialComposterConfig : IBuildingConfig
    {
        public const string ID = "IndustrialComposter";
        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] allMetals = MATERIALS.ALL_METALS;
            EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
            EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 3, "fertilizer_maker_kanim", 30, 30f, tieR3, allMetals, 800f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ExhaustKilowattsWhenActive = 1f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            go.AddOrGet<WaterPurifier>();
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(defaultStorage);
            manualDeliveryKg.requestedItemTag = GameTags.Compostable;
            manualDeliveryKg.capacity = 300f;
            manualDeliveryKg.refillMass = 60f;
            manualDeliveryKg.MinimumMass = 1f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements =
            [
                new ElementConverter.ConsumedElement(GameTags.Compostable, 1.5f)
            ];
            elementConverter.outputElements =
            [
                new ElementConverter.OutputElement(1.5f, SimHashes.Dirt, 348.15f, storeOutput: true)
            ];
            ElementDropper elementDropper = go.AddComponent<ElementDropper>();
            elementDropper.emitMass = 10f;
            elementDropper.emitTag = SimHashes.Dirt.CreateTag();
            elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
            Prioritizable.AddRef(go);
        }
    }
}
