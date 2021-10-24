using KSerialization;
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
        private bool newCachedResult = false;

        public bool IsEvaporator = false;

        private bool TryConvertNew()
        {
            List<GameObject> items = storage.GetItems();

            if (items.Count <= 0) return false;

            int converted = 0;

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (IsEvaporator)
                {
                    if (IsEvaporator && element.IsLiquid)
                    {
                        Element gas = element.highTempTransition;

                        storage.Remove(item);
                        storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                        converted++;
                    }
                    else if (!IsEvaporator && element.IsGas)
                    {
                        Element liquid = element.lowTempTransition;

                        storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                        storage.Remove(item);
                        converted++;
                    }
                }
            }

            return true;
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, FluidConverter, object>.GameInstance
        {
            public StatesInstance(FluidConverter master) : base(master)
            {
            }
        }

        public class States: GameStateMachine<States, StatesInstance, FluidConverter>
        {
            public State off;
            public State idle;
            public State converting;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                root
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational)
                    .EventTransition(GameHashes.OperationalChanged, idle, smi => smi.master.operational.IsOperational);

                idle
                    .Enter("Ready", smi => smi.master.operational.SetActive(true))
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.newCachedResult = smi.master.TryConvertNew())
                    .EventTransition(GameHashes.OnStorageChange, converting, smi => smi.master.newCachedResult == true)
                    .Exit("Ready", smi => smi.master.operational.SetActive(false));

                converting
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.newCachedResult = smi.master.TryConvertNew())
                    .EventTransition(GameHashes.OnStorageChange, idle, smi => smi.master.newCachedResult == false);

            }
        }
    }
}
