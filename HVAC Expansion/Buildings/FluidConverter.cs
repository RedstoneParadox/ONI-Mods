﻿using FMOD.Studio;
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

        private void Run(float dt)
        {
            UpdateTint();
            PlaySound();
            hasConverted = TryConvert(dt);
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
                    float kj = temperatureDelta * element.specificHeatCapacity * primaryElement.Mass;

                    if (IsEvaporator && element.IsLiquid)
                    {
                        Element gas = element.highTempTransition;

                        ApplyTint(element.substance.uiColour, false);
                        ApplyTint(gas.substance.uiColour, true);


                        storage.items.Remove(item);
                        storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature + temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);

                        kj = -kj;

                        converted++;
                    }
                    else if (!IsEvaporator && element.IsGas)
                    {
                        Element liquid = element.lowTempTransition;

                        ApplyTint(liquid.substance.uiColour, false);
                        ApplyTint(element.substance.uiColour, true);

                        storage.items.Remove(item);
                        storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature - temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);

                        converted++;
                    }

                    GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, dt);
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
            public State off;
            public OnStates on;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                root
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational);
                off
                    .EventTransition(GameHashes.OperationalChanged, on.waiting, smi => smi.master.operational.IsOperational)
                    .PlayAnim("ffo");
                on
                    .PlayAnim("no")
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational)
                    .DefaultState(on.waiting);
                on.waiting
                    .EventTransition(GameHashes.OnStorageChange, on.working_pre, smi => !smi.master.storage.IsEmpty());
                on.working_pre
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(on.working);
                on.working
                    .Enter("Working", (smi) => smi.master.operational.SetActive(true))
                    .Update((smi, dt) => smi.master.Run(dt))
                    .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .EventTransition(GameHashes.OnStorageChange, on.working_pst, smi => smi.master.hasConverted == false)
                    .Exit(smi => smi.master.operational.SetActive(false));
                on.working_pst
                    .Enter(smi => smi.master.StopSound())
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
