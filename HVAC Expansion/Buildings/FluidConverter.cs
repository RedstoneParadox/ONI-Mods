﻿using KSerialization;
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
        private ConversionResult cachedResult = ConversionResult.IDLE;

        private ConversionResult TryConvert()
        {
            List<GameObject> items = storage.items;

            if (items.Count <= 0) return ConversionResult.IDLE;

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (IsEvaporator())
                {
                    if ((element.IsLiquid && !element.highTempTransition.IsGas) || element.highTempTransitionOreMassConversion >= 0)
                    {
                        return ConversionResult.CLOGGED;
                    }
                    if (element.IsGas)
                    {
                        return ConversionResult.IDLE;
                    }
                }
                else
                {
                    if ((element.IsGas && !element.lowTempTransition.IsLiquid) || element.lowTempTransitionOreMassConversion >= 0)
                    {
                        return ConversionResult.CLOGGED;
                    }
                    if (element.IsLiquid)
                    {
                        return ConversionResult.IDLE;
                    }
                }
            }

            foreach (GameObject item in items)
            {
                PrimaryElement primaryElement = item.GetComponent<PrimaryElement>();
                Element element = primaryElement.Element;

                if (IsEvaporator())
                {
                    Element gas = element.highTempTransition;

                    storage.AddGasChunk(gas.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                }
                if (!IsEvaporator())
                {
                    Element liquid = element.lowTempTransition;

                    storage.AddLiquid(liquid.id, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, false);
                }
            }

            return ConversionResult.SUCCESS;
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
            public State idle;
            public State converting;
            public State clogged;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                root
                    .EventTransition(GameHashes.OperationalChanged, off, smi => !smi.master.operational.IsOperational)
                    .EventTransition(GameHashes.OperationalChanged, idle, smi => smi.master.operational.IsOperational);

                idle
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.cachedResult = smi.master.TryConvert())
                    .EventTransition(GameHashes.OnStorageChange, converting, smi => smi.master.cachedResult == ConversionResult.SUCCESS)
                    .EventTransition(GameHashes.OnStorageChange, clogged, smi => smi.master.cachedResult == ConversionResult.CLOGGED);

                converting
                    .Enter("Ready", smi => smi.master.operational.SetActive(true))
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.cachedResult = smi.master.TryConvert())
                    .EventTransition(GameHashes.OnStorageChange, idle, smi => smi.master.cachedResult == ConversionResult.IDLE)
                    .EventTransition(GameHashes.OnStorageChange, clogged, smi => smi.master.cachedResult == ConversionResult.CLOGGED)
                    .Exit("Ready", smi => smi.master.operational.SetActive(false));

                clogged
                    .EventHandler(GameHashes.OnStorageChange, smi => smi.master.cachedResult = smi.master.TryConvert())
                    .EventTransition(GameHashes.OnStorageChange, converting, smi => smi.master.cachedResult == ConversionResult.SUCCESS)
                    .EventTransition(GameHashes.OnStorageChange, idle, smi => smi.master.cachedResult == ConversionResult.IDLE);

            }
        }

        private enum ConversionResult
        {
            SUCCESS,
            CLOGGED,
            IDLE
        }
    }
}
