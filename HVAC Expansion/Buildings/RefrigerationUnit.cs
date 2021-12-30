using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    class RefrigerationUnit: AirConditioner
    {
        private static readonly EventSystem.IntraObjectHandler<RefrigerationUnit> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<RefrigerationUnit>((component, data) => component.UpdateTint());
        private static readonly EventSystem.IntraObjectHandler<RefrigerationUnit> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<RefrigerationUnit>((component, data) => component.ClearTint(data));

        public Storage GetStorage()
        {
            return storage;
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
                // TODO: Tint liquid sprites once the liquid kanim is added.
            }
            else
            {
                controller.SetSymbolTint("gas", color);
            }
        }
    }
}
