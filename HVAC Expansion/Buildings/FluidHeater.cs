using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    class FluidHeater: AirConditioner
    {
        private static readonly EventSystem.IntraObjectHandler<FluidHeater> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<FluidHeater>((component, data) => component.UpdateTint());
        private static readonly EventSystem.IntraObjectHandler<FluidHeater> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<FluidHeater>((component, data) => {
            if (((Operational)data).IsActive)
            {
                component.UpdateTint();
            }
        });

        [MyCmpReq]
        private BuildingComplete building;
        private int inputCell = -1;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            inputCell = building.GetUtilityInputCell();
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
            Subscribe(824508782, OnActiveChangedDelegate);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Unsubscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
            Unsubscribe(824508782, OnActiveChangedDelegate);
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
                    ApplyTint(color);
                    break;
                }
            }

            if (items.Length <= 0)
            {
                ConduitFlow flow = (isLiquidConditioner ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow);

                if (flow.HasConduit(inputCell))
                {
                    SimHashes hashes = flow.GetContents(inputCell).element;
                    Element element = ElementLoader.FindElementByHash(hashes);
                    var color = element.substance.uiColour;

                    ApplyTint(color);
                }
            }
        }

        public void ClearTint(object data)
        {
            if (!((Operational)data).IsActive) {
                ApplyTint(Color.clear);
            }
        }

        private void ApplyTint(Color color)
        {
            var controller = GetComponent<KBatchedAnimController>();

            if (isLiquidConditioner)
            {
                controller.SetSymbolTint("liquid", color);
                controller.SetSymbolTint("liquid_trans", color);
            }
            else
            {
                controller.SetSymbolTint("gas", color);
            }
        }
    }
}
