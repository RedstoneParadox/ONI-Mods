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

        public bool CanConvert()
        {
            List<GameObject> items = storage.items;

            for (int index = 0; index < items.Count; ++index) 
            {
                GameObject go = items[index];
                PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();

                if ((primaryElement.Element.IsGas && IsEvaporator()) || (primaryElement.Element.IsLiquid && !IsEvaporator()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsClogged()
        {
            List<GameObject> items = storage.items;

            for (int index = 0; index < items.Count; ++index)
            {
                GameObject go = items[index];
                PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if ((IsEvaporator() && element.IsLiquid && !element.highTempTransition.IsGas) || (!IsEvaporator() && element.IsGas && !element.lowTempTransition.IsLiquid))
                {
                    return false;
                }
                if (element.highTempTransitionOreMassConversion >= 0 || element.lowTempTransitionOreMassConversion >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Convert()
        {
            List<GameObject> items = storage.items;

            if (IsEvaporator())
            {
                for (int index = 0; index < items.Count; ++index)
                {
                    GameObject go = items[index];
                    PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();
                    Element element = primaryElement.Element;
                    Element gas = element.highTempTransition;

                    storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                }
            } 
            else
            {
                for (int index = 0; index < items.Count; ++index)
                {
                    GameObject go = items[index];
                    PrimaryElement primaryElement = go.GetComponent<PrimaryElement>();
                    Element element = primaryElement.Element;
                    Element liquid = element.lowTempTransition;

                    storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                }
            }
        }

        public bool IsEvaporator()
        {
            return false;
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
            public State on;
            public State converting;
            public State clogged;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                root.EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational);
                root.EventTransition(GameHashes.OperationalChanged, on, smi => smi.master.operational.IsOperational);
                root.EventTransition(GameHashes.OnStorageChange, converting, smi => smi.master.CanConvert() && !smi.master.IsClogged());
                root.EventTransition(GameHashes.OnStorageChange, clogged, smi => smi.master.IsClogged());
                converting
                    .Enter("Ready", smi => smi.master.operational.SetActive(true))
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.Convert())
                    .EventTransition(GameHashes.OnStorageChange, on, smi => !smi.master.CanConvert())
                    .Exit("Ready", smi => smi.master.operational.SetActive(false));
            }
        }
    }
}
