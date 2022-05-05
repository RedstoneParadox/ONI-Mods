using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HVACExpansion.Buildings
{
    class AirConditionerTint : KMonoBehaviour
    {
        [MyCmpGet]
        private Storage storage;
        public string[] symbols = { };

        private static readonly EventSystem.IntraObjectHandler<AirConditionerTint> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<AirConditionerTint>((component, data) => component.UpdateTint());
        private static readonly EventSystem.IntraObjectHandler<AirConditionerTint> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<AirConditionerTint>((component, data) => component.ClearTint(data));

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
            //Subscribe(824508782, OnActiveChangedDelegate);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Unsubscribe(Convert.ToInt32(GameHashes.OnStorageChange), OnStorageChangeDelegate);
            //Unsubscribe(824508782, OnActiveChangedDelegate);
        }

        private void UpdateTint()
        {
            if (storage != null)
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
        }

        private void ClearTint(object data)
        {
            if (!((Operational)data).IsActive)
            {
                ApplyTint(Color.clear);
            }
        }

        private void ApplyTint(Color color)
        {
            var controller = GetComponent<KBatchedAnimController>();

            foreach (string symbol in symbols)
            {
                controller.SetSymbolTint(symbol, color);
            }
        }
    }
}
