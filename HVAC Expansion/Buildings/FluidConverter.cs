using FMOD.Studio;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class FluidConverter: StateMachineComponent<FluidConverter.StatesInstance>
    {
        [MyCmpAdd]
        private Storage storage;
        [MyCmpReq]
        private Operational operational;
        [MyCmpReq]
        private BuildingComplete building;
        private bool hasConverted = false;
        private HandleVector<int>.Handle structureTemperature;
        EventInstance soundEventInstance;
        private int cooledAirOutputCell = -1;

        public bool IsEvaporator = false;
        public float temperatureDelta = 0.0f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            cooledAirOutputCell = building.GetUtilityOutputCell();
            smi.StartSM();
        }

        private bool TryConvert(float dt)
        {
            GameObject[] items = storage.GetItems().ToArray();

            if (items.Length <= 0) return false;

            int converted = 0;

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (primaryElement.Mass > 0)
                {
                    if (IsEvaporator && element.IsLiquid)
                    {
                        Element gas = element.highTempTransition;

                        ApplyTint(element.substance.uiColour, false);
                        ApplyTint(gas.substance.uiColour, true);

                        storage.items.Remove(item);

                        float massOutputted = (IsEvaporator ? Game.Instance.gasConduitFlow : Game.Instance.liquidConduitFlow).AddElement(cooledAirOutputCell, gas.id, primaryElement.Mass, primaryElement.Temperature + temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                        float kj = temperatureDelta * element.specificHeatCapacity * massOutputted;


                        GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, -kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, 0.0f);

                        converted++;
                    }
                    else if (!IsEvaporator && element.IsGas)
                    {
                        Element liquid = element.lowTempTransition;

                        ApplyTint(liquid.substance.uiColour, false);
                        ApplyTint(element.substance.uiColour, true);

                        storage.items.Remove(item);
                        float massOutputted = (IsEvaporator ? Game.Instance.gasConduitFlow : Game.Instance.liquidConduitFlow).AddElement(cooledAirOutputCell, liquid.id, primaryElement.Mass, primaryElement.Temperature - temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                        float kj = temperatureDelta * element.specificHeatCapacity * massOutputted;

                        GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, 0.0f);

                        converted++;
                    }
                }
            }

            return true;
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
                    ApplyTint(color, primaryElement.Element.IsGas);
                    break;
                }
            }
        }



        private void ApplyTint(Color color, bool gas)
        {
            var controller = GetComponent<KBatchedAnimController>();

            if (IsEvaporator)
            {
                if (gas)
                {
                    controller.SetSymbolTint("gas", color);
                }
                else
                {
                    controller.SetSymbolTint("liquid", color);
                    controller.SetSymbolTint("liquid_top", color);
                }
            }
            else
            {

            }
        }


        private void PlaySound()
        {
            soundEventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("SpaceHeater_wave_long"), smi.master.gameObject.transform.position, 2f);
            if (soundEventInstance.isValid())
            {
                KFMOD.EndOneShot(soundEventInstance);
            }
        }

        private void StopSound()
        {
            KFMOD.EndOneShot(soundEventInstance);
        }


        public class StatesInstance : GameStateMachine<States, StatesInstance, FluidConverter, object>.GameInstance
        {
            public StatesInstance(FluidConverter master) : base(master)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, FluidConverter>
        {
            public State disabled;
            public State waiting;
            public State working_pre;
            public State working;
            public State working_post;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = disabled;
                root
                    .EventTransition(GameHashes.OperationalChanged, disabled, smi => !smi.master.operational.IsOperational);
                disabled
                    .EventTransition(GameHashes.OperationalChanged, waiting, smi => smi.master.operational.IsOperational)
                    .PlayAnim("ffo");
                waiting
                    .Enter("Waiting", smi => smi.master.operational.SetActive(false))
                    .EventTransition(GameHashes.OnStorageChange, working_pre, smi => !smi.master.storage.IsEmpty())
                    .PlayAnim("no");
                working_pre
                    .Enter("Ready", smi =>
                    {
                        smi.master.operational.SetActive(true);
                        smi.master.UpdateTint();
                        smi.master.PlaySound();
                    })
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(working);
                working
                    .Enter("Working", (smi) => smi.master.hasConverted = smi.master.TryConvert(0.0f))
                    .EventHandler(GameHashes.OnStorageChange, (smi) =>
                    {
                        smi.master.hasConverted = smi.master.TryConvert(0.0f);
                        smi.master.PlaySound();
                    })
                    .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .Transition(working_post, smi => smi.master.hasConverted == false);
                working_post
                    .Enter(smi =>
                    {
                        smi.master.StopSound();
                    })
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(waiting);
            }
        }
    }
}
