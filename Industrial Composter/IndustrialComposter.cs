using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Industrial_Composter
{
    internal class IndustrialComposter : StateMachineComponent<IndustrialComposter.StatesInstance>
    {
        [MyCmpGet]
        private Operational operational;
        private ManualDeliveryKG[] deliveryComponents;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            deliveryComponents = GetComponents<ManualDeliveryKG>();
            smi.StartSM();
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, IndustrialComposter, object>.GameInstance
        {
            public StatesInstance(IndustrialComposter smi) : base(smi)
            {
            }
        }

        public class States : GameStateMachine<States, StatesInstance, IndustrialComposter>
        {
            public State off;
            public OnStates on;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = off;
                off
                    .PlayAnim("off")
                    .EventTransition(GameHashes.OperationalChanged, on, smi => smi.master.operational.IsOperational);
                on
                    .PlayAnim("on")
                    .EventTransition(GameHashes.OperationalChanged, off, (smi => !smi.master.operational.IsOperational))
                    .DefaultState(on.waiting);
                on.waiting
                    .EventTransition(GameHashes.OnStorageChange, on.working_pre, smi => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
                on.working_pre
                    .PlayAnim("working_pre")
                    .OnAnimQueueComplete(on.working);
                on.working
                    .Enter(smi => smi.master.operational.SetActive(true))
                    .QueueAnim("working_loop", true)
                    .EventTransition(GameHashes.OnStorageChange, on.working_pst, smi => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll())
                    .Exit(smi => smi.master.operational.SetActive(false));
                on.working_pst
                    .PlayAnim("working_pst")
                    .OnAnimQueueComplete(on.waiting);
            }

            public class OnStates : State
            {
                public State waiting;
                public State working_pre;
                public State working;
                public State working_pst;
            }
        }
    }
}
