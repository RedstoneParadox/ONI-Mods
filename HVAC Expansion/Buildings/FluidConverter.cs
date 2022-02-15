﻿using KSerialization;
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
        private bool hasConverted = false;
        private HandleVector<int>.Handle structureTemperature;

        public bool IsEvaporator = false;
        public float temperatureDelta = 0.0f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            smi.StartSM();
        }

        private bool TryConvert()
        {
            GameObject[] items = storage.GetItems().ToArray();

            if (items.Length <= 0) return false;

            int converted = 0;

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                float kj = temperatureDelta * element.specificHeatCapacity * primaryElement.Mass;

                if (primaryElement.Mass > 0)
                {
                    if (IsEvaporator && element.IsLiquid)
                    {
                        Element gas = element.highTempTransition;

                        ApplyTint(element.substance.uiColour, false);
                        ApplyTint(gas.substance.uiColour, true);

                        storage.items.Remove(item);
                        storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature + temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);

                        GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, -kj, BUILDING.STATUSITEMS.OPERATINGENERGY.PIPECONTENTS_TRANSFER, 0.0f);

                        converted++;
                    }
                    else if (!IsEvaporator && element.IsGas)
                    {
                        Element liquid = element.lowTempTransition;

                        ApplyTint(liquid.substance.uiColour, false);
                        ApplyTint(element.substance.uiColour, true);

                        storage.items.Remove(item);
                        storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature - temperatureDelta, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);

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

        private void ClearTint()
        {
            ApplyTint(Color.clear, true);
            ApplyTint(Color.clear, false);
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
                    .PlayAnim("off");
                waiting
                    .Enter("Waiting", smi => smi.master.operational.SetActive(false))
                    .EventTransition(GameHashes.OnStorageChange, working_pre, smi => !smi.master.storage.IsEmpty());
                working_pre
                    .Enter("Ready", smi =>
                    {
                        smi.master.operational.SetActive(true);
                        smi.master.UpdateTint();
                    })
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(working);
                working
                    .Enter("Working", (smi) => smi.master.hasConverted = smi.master.TryConvert())
                    .EventHandler(GameHashes.OnStorageChange, (smi) => smi.master.hasConverted = smi.master.TryConvert())
                    .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                    .Transition(working_post, smi => smi.master.hasConverted == false);
                working_post
                    .Enter(smi => smi.master.ClearTint())
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(waiting);
            }
        }
    }
}