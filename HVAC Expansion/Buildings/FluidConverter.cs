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

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
        }

        private bool TryConvert()
        {
            GameObject[] items = storage.GetItems().ToArray();

            if (items.Length <= 0) return false;

            int converted = 0;

            try
            {
                foreach (GameObject item in items)
                {
                    PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                    Element element = primaryElement.Element;

                    if (primaryElement.Mass > 0)
                    {
                        if (IsEvaporator && element.IsLiquid)
                        {
                            Element gas = element.highTempTransition;

                            storage.items.Remove(item);
                            storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                            converted++;
                        }
                        else if (!IsEvaporator && element.IsGas)
                        {
                            Element liquid = element.lowTempTransition;

                            storage.items.Remove(item);
                            storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                            converted++;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());

                return false;
            }
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, FluidConverter, object>.GameInstance
        {
            public StatesInstance(FluidConverter master) : base(master)
            {
            }
        }

        public class States: GameStateMachine<States, StatesInstance, FluidConverter>
        {
            public State disabled;
            public State waiting;
            public State converting;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = disabled;
                root
                    .EventTransition(GameHashes.OperationalChanged, disabled, smi => !smi.master.operational.IsOperational);
                disabled

                    .EventTransition(GameHashes.OperationalChanged, waiting, smi => smi.master.operational.IsOperational);
                waiting
                    .Enter("Waiting", smi => smi.master.operational.SetActive(false))
                    .EventTransition(GameHashes.OnStorageChange, converting, smi => !smi.master.storage.IsEmpty());
                converting
                    .Enter("Ready", smi =>
                    {
                        smi.master.operational.SetActive(true);
                        smi.master.newCachedResult = smi.master.TryConvert();
                    })
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.newCachedResult = smi.master.TryConvert())
                    .Transition(waiting, smi => smi.master.newCachedResult == false);

            }
        }
    }
}
