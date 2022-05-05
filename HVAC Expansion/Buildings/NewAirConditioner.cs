using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class NewAirConditioner : KMonoBehaviour, ISaveLoadable, IGameObjectEffectDescriptor, ISim200ms
    {
        [MyCmpReq]
        private KSelectable selectable;
        [MyCmpReq]
        protected Storage storage;
        [MyCmpReq]
        protected Operational operational;
        [MyCmpReq]
        private ConduitConsumer consumer;
        [MyCmpReq]
        private BuildingComplete building;
        [MyCmpGet]
        private OccupyArea occupyArea;
        private HandleVector<int>.Handle structureTemperature;
        public float temperatureDelta = -14f;
        public float maxEnvironmentDelta = -50f;
        private float lowTempLag;
        private bool showingLowTemp = false;
        public bool isLiquidConditioner;
        private bool showingHotEnv = false;
        private Guid statusHandle;
        [Serialize]
        private float targetTemperature;
        private int cooledAirOutputCell = -1;
        private int inputCell = -1;
        private static readonly EventSystem.IntraObjectHandler<NewAirConditioner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<NewAirConditioner>((component, data) => component.OnOperationalChanged(data));
        private static readonly EventSystem.IntraObjectHandler<NewAirConditioner> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<NewAirConditioner>((component, data) => component.OnActiveChanged(data));
        private float lastSampleTime = -1f;
        private float envTemp;
        private int cellCount;
        private static readonly Func<int, object, bool> UpdateStateCbDelegate = (cell, data) => UpdateStateCb(cell, data);

        public float lastEnvTemp { get; private set; }

        public float lastGasTemp { get; private set; }

        public float TargetTemperature => this.targetTemperature;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(-592767678, OnOperationalChangedDelegate);
            Subscribe(824508782, OnActiveChangedDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            GameScheduler.Instance.Schedule("InsulationTutorial", 2f, (System.Action<object>)(obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Insulation)), (object)null, (SchedulerGroup)null);
            this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
            this.cooledAirOutputCell = this.building.GetUtilityOutputCell();
            inputCell = building.GetUtilityInputCell();
        }

        public void Sim200ms(float dt)
        {
            if ((UnityEngine.Object)this.operational != (UnityEngine.Object)null && !this.operational.IsOperational)
            {
                this.operational.SetActive(false);
            }
            else
            {
                this.UpdateState(dt);
            }
        }

        private static bool UpdateStateCb(int cell, object data)
        {
            NewAirConditioner airConditioner = data as NewAirConditioner;
            ++airConditioner.cellCount;
            airConditioner.envTemp += Grid.Temperature[cell];
            return true;
        }

        private void UpdateState(float dt)
        {
            UpdateTint();

            bool flag = this.consumer.IsSatisfied;
            this.envTemp = 0.0f;
            this.cellCount = 0;
            if ((UnityEngine.Object)this.occupyArea != (UnityEngine.Object)null && (UnityEngine.Object)this.gameObject != (UnityEngine.Object)null)
            {
                this.occupyArea.TestArea(Grid.PosToCell(this.gameObject), (object)this, NewAirConditioner.UpdateStateCbDelegate);
                this.envTemp /= (float)this.cellCount;
            }
            this.lastEnvTemp = this.envTemp;
            List<GameObject> items = this.storage.items;
            for (int index = 0; index < items.Count; ++index)
            {
                PrimaryElement component = items[index].GetComponent<PrimaryElement>();
                if ((double)component.Mass > 0.0 && (!this.isLiquidConditioner || !component.Element.IsGas) && (this.isLiquidConditioner || !component.Element.IsLiquid))
                {
                    flag = true;
                    this.lastGasTemp = component.Temperature;
                    float temperature = component.Temperature + this.temperatureDelta;
                    if ((double)temperature < 1.0)
                    {
                        temperature = 1f;
                        this.lowTempLag = Mathf.Min(this.lowTempLag + dt / 5f, 1f);
                    }
                    else
                        this.lowTempLag = Mathf.Min(this.lowTempLag - dt / 5f, 0.0f);
                    float num1 = (this.isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(this.cooledAirOutputCell, component.ElementID, component.Mass, temperature, component.DiseaseIdx, component.DiseaseCount);
                    component.KeepZeroMassObject = true;
                    float num2 = num1 / component.Mass;
                    int num3 = (int)((double)component.DiseaseCount * (double)num2);
                    component.Mass -= num1;
                    component.ModifyDiseaseCount(-num3, "AirConditioner.UpdateState");
                    float num4 = (temperature - component.Temperature) * component.Element.specificHeatCapacity * num1;
                    float display_dt = (double)this.lastSampleTime > 0.0 ? Time.time - this.lastSampleTime : 1f;
                    this.lastSampleTime = Time.time;
                    GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, -num4, (string)BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, display_dt);
                    break;
                }
            }
            if ((double)Time.time - (double)this.lastSampleTime > 2.0)
            {
                GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, 0.0f, (string)BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, Time.time - this.lastSampleTime);
                this.lastSampleTime = Time.time;
            }

            this.UpdateStatus();
        }

        private void OnOperationalChanged(object data)
        {
            if (!this.operational.IsOperational)
                return;
            this.UpdateState(0.0f);
        }

        private void OnActiveChanged(object data) => this.UpdateStatus();

        private void UpdateStatus()
        {
            if (this.operational.IsActive)
            {
                if ((double)this.lowTempLag >= 1.0 && !this.showingLowTemp)
                {
                    this.statusHandle = this.isLiquidConditioner ? this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdLiquid, (object)this) : this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CoolingStalledColdGas, (object)this);
                    this.showingLowTemp = true;
                    this.showingHotEnv = false;
                }
                else if ((double)this.lowTempLag <= 0.0 && (this.showingHotEnv || this.showingLowTemp))
                {
                    this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling);
                    this.showingLowTemp = false;
                    this.showingHotEnv = false;
                }
                else
                {
                    if (!(this.statusHandle == Guid.Empty))
                        return;
                    this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling);
                    this.showingLowTemp = false;
                    this.showingHotEnv = false;
                }
            }
            else
                this.statusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem)null);
        }

        public void UpdateTint()
        {
            var items = storage.GetItems().ToArray();

            foreach (GameObject item in items)
            {
                var primaryElement = item.GetComponent<PrimaryElement>();
                var color = primaryElement.Element.substance.uiColour;

                if (primaryElement.Mass > 0)
                {
                    ApplyTint(color);
                    break;
                }
            }
        }

        private void ApplyTint(Color color)
        {
            var controller = GetComponent<KBatchedAnimController>();

            if (isLiquidConditioner)
            {
                controller.SetSymbolTint("liquid", color);
                controller.SetSymbolTint("liquid_trans", color);
            }
            else
            {
                controller.SetSymbolTint("gas", color);
            }
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            List<Descriptor> descriptorList = new List<Descriptor>();
            string formattedTemperature = GameUtil.GetFormattedTemperature(this.temperatureDelta, interpretation: GameUtil.TemperatureInterpretation.Relative);
            Element elementByName = ElementLoader.FindElementByName(this.isLiquidConditioner ? "Water" : "Oxygen");
            float dtu = (!this.isLiquidConditioner ? Mathf.Abs((float)((double)this.temperatureDelta * (double)elementByName.specificHeatCapacity * 1000.0)) : Mathf.Abs((float)((double)this.temperatureDelta * (double)elementByName.specificHeatCapacity * 10000.0))) * 1f;
            Descriptor descriptor1 = new Descriptor();
            string txt = string.Format((string)(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.HEATGENERATED_AIRCONDITIONER), (object)GameUtil.GetFormattedHeatEnergy(dtu), (object)GameUtil.GetFormattedTemperature(Mathf.Abs(this.temperatureDelta), interpretation: GameUtil.TemperatureInterpretation.Relative));
            string tooltip = string.Format((string)(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_LIQUIDCONDITIONER : UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED_AIRCONDITIONER), (object)GameUtil.GetFormattedHeatEnergy(dtu), (object)GameUtil.GetFormattedTemperature(Mathf.Abs(this.temperatureDelta), interpretation: GameUtil.TemperatureInterpretation.Relative));
            descriptor1.SetupDescriptor(txt, tooltip);
            descriptorList.Add(descriptor1);
            Descriptor descriptor2 = new Descriptor();
            descriptor2.SetupDescriptor(string.Format((string)(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.GASCOOLING), (object)formattedTemperature), string.Format((string)(this.isLiquidConditioner ? UI.BUILDINGEFFECTS.TOOLTIPS.LIQUIDCOOLING : UI.BUILDINGEFFECTS.TOOLTIPS.GASCOOLING), (object)formattedTemperature));
            descriptorList.Add(descriptor2);
            return descriptorList;
        }
    }
}
