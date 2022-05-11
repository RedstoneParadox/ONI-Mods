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
        private bool attemptedConversion = false;
        private HandleVector<int>.Handle structureTemperature;
        EventInstance soundEventInstance;
        private int outputCell = -1;

        public bool IsEvaporator = false;
        public float temperatureDelta = 0.0f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            outputCell = building.GetUtilityOutputCell();
            smi.StartSM();
        }

        private void Run(float dt)
        {
            PlaySound();
            TryConvert(dt);
            attemptedConversion = true;
        }

        private void TryConvert(float dt)
        {
            GameObject[] items = storage.GetItems().ToArray();

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (primaryElement.Mass > 0)
                {
                    Element transitionElement = IsEvaporator ? element.highTempTransition : element.lowTempTransition;
                    ConduitFlow conduitFlow = IsEvaporator ? Game.Instance.gasConduitFlow : Game.Instance.liquidConduitFlow;

                    ApplyTint(element.substance.uiColour, element.IsGas);
                    ApplyTint(transitionElement.substance.uiColour, transitionElement.IsGas);

                    if (transitionElement == null) continue;

                    float maxMass = Util.GetMaxGasMass() * Util.GetThroughputPercent();
                    float finalTemperature = IsEvaporator ? primaryElement.Temperature + temperatureDelta : primaryElement.Temperature - temperatureDelta;
                    float emittedMass = conduitFlow.AddElement(outputCell, transitionElement.id, Mathf.Min(primaryElement.Mass, maxMass), finalTemperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                    float percent = emittedMass / primaryElement.Mass;
                    float kj = temperatureDelta * element.specificHeatCapacity * emittedMass;

                    if (IsEvaporator) kj = -kj;

                    if (emittedMass > 0)
                    {
                        hasConverted = true;

                        primaryElement.KeepZeroMassObject = false;
                        primaryElement.Mass -= emittedMass;
                        primaryElement.ModifyDiseaseCount(-(int)(primaryElement.DiseaseCount * percent), IsEvaporator ? "Evaporator.Convert" : "Condenser.Convert");

                        GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, dt);
                        return;
                    }
                }
            }

            hasConverted = false;
        }


        public void UpdateTint()
        {
            var items = storage.GetItems().ToArray();

            foreach (GameObject item in items)
            {
                var primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (primaryElement.Mass > 0)
                {
                    Element transitionElement = IsEvaporator ? element.highTempTransition : element.lowTempTransition;

                    if (transitionElement == null) continue;

                    ApplyTint(element.substance.uiColour, element.IsGas);
                    ApplyTint(transitionElement.substance.uiColour, transitionElement.IsGas);
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
                if (!gas)
                {
                    controller.SetSymbolTint("liquid", color);
                }
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
            public State off;
            public OnStates on;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                off
                    .PlayAnim("ffo")
                    .EventTransition(GameHashes.OperationalChanged, on, smi => smi.master.operational.IsOperational);
                on
                    .PlayAnim("no")
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational)
                    .DefaultState(on.waiting);
                on.waiting
                    .EventTransition(GameHashes.OnStorageChange, on.working_pre, smi => !smi.master.storage.IsEmpty());
                on.working_pre
                    .Enter(smi =>
                    {
                        smi.master.attemptedConversion = false;
                        smi.master.UpdateTint();
                    })
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(on.working);
                on.working
                    .Enter("Working", (smi) =>
                    {
                        smi.master.operational.SetActive(true);
                        smi.master.Run(0.0f);
                    })
                    .QueueAnim("working_loop", true)
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.Run(0.0f))
                    .EventTransition(GameHashes.OnStorageChange, on.working_pst, smi => smi.master.attemptedConversion == true && smi.master.hasConverted == false)
                    .Exit(smi => smi.master.operational.SetActive(false));
                on.working_pst
                    .Enter(smi =>
                    {
                        smi.master.StopSound();
                    })
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(on.waiting);
            }
        }

        public class OnStates: GameStateMachine<States, StatesInstance, FluidConverter, object>.State
        {
            public GameStateMachine<States, StatesInstance, FluidConverter, object>.State waiting;
            public GameStateMachine<States, StatesInstance, FluidConverter, object>.State working_pre;
            public GameStateMachine<States, StatesInstance, FluidConverter, object>.State working;
            public GameStateMachine<States, StatesInstance, FluidConverter, object>.State working_pst;
        }
    }
}
