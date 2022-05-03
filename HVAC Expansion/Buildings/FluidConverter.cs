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
            hasConverted = TryConvert(dt);
            attemptedConversion = true;
        }

        private bool TryConvert(float dt)
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

                    if (transitionElement == null) continue;

                    float finalTemperature = IsEvaporator ? primaryElement.Temperature + temperatureDelta : primaryElement.Temperature - temperatureDelta;
                    float emittedMass = conduitFlow.AddElement(outputCell, transitionElement.id, primaryElement.Mass, finalTemperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
                    float kj = temperatureDelta * element.specificHeatCapacity * primaryElement.Mass;

                    if (IsEvaporator) kj = -kj;

                    primaryElement.Mass -= emittedMass;

                    ApplyTint(element.substance.uiColour, element.IsGas);
                    ApplyTint(transitionElement.substance.uiColour, transitionElement.IsGas);

                    GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, dt);

                    if (emittedMass > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
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
                    .Enter(smi => Debug.Log("Transition to off state!"))
                    .EventTransition(GameHashes.OperationalChanged, on.waiting, smi => smi.master.operational.IsOperational)
                    .PlayAnim("ffo");
                on
                    .Enter(smi => Debug.Log("Transition to on state!"))
                    .PlayAnim("no")
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational)
                    .DefaultState(on.waiting);
                on.waiting
                    .Enter(smi => Debug.Log("Transition to on.waiting state!"))
                    .EventTransition(GameHashes.OnStorageChange, on.working_pre, smi => !smi.master.storage.IsEmpty());
                on.working_pre
                    .Enter(smi =>
                    {
                        smi.master.UpdateTint(); Debug.Log("Transition to on.working_pre state!");
                    })
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(on.working);
                on.working
                    .Enter("Working", (smi) =>
                    {
                        smi.master.operational.SetActive(true); Debug.Log("Transition to on.working state!");
                    })
                    .QueueAnim("working_loop", true)
                    .EventHandler(GameHashes.OnStorageChange, (smi) => smi.master.Run(0.0f))
                    .EventTransition(GameHashes.OnStorageChange, on.working_pst, smi => smi.master.attemptedConversion == true && smi.master.hasConverted == false)
                    .Exit(smi =>
                    {
                        smi.master.operational.SetActive(false); Debug.Log($"Operational? {smi.master.operational.IsOperational}");
                    });
                on.working_pst
                    .Enter(smi =>
                    {
                        smi.master.StopSound(); Debug.Log("Transition to on.working_pst state!");
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
