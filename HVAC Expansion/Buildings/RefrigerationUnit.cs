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
        private int onStorageChangedID = 0;
        private static readonly EventSystem.IntraObjectHandler<RefrigerationUnit> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<RefrigerationUnit>((component, data) => component.UpdateTint());

        public Storage GetStorage()
        {
            return storage;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Unsubscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
        }

        public void UpdateTint()
        {
            var controller = GetComponent<KBatchedAnimController>();
            var items = storage.GetItems().ToArray();

            foreach (GameObject item in items)
            {
                var primaryElement = item.GetComponent<PrimaryElement>();
                var color = primaryElement.Element.substance.uiColour;

                if (primaryElement.Mass > 0)
                {
                    if (isLiquidConditioner)
                    {
                        // TODO: Tint liquid sprites once the liquid kanim is added.
                        break;
                    }
                    else
                    {
                        controller.SetSymbolTint("gas", color);
                        break;
                    }
                }
            }
        }
    }
}
