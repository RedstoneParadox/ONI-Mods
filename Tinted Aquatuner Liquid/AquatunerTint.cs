using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tinted_Aquatuner_Liquid
{
    class AquatunerTint: KMonoBehaviour
    {
        private static readonly EventSystem.IntraObjectHandler<AquatunerTint> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<AquatunerTint>((component, data) => component.UpdateTint());
        private static readonly EventSystem.IntraObjectHandler<AquatunerTint> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<AquatunerTint>((component, data) => component.ClearTint(data));

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

        private Storage GetStorage()
        {
            AirConditioner airConditioner = GetComponent<AirConditioner>();
            Storage storage = Traverse.Create(airConditioner).Property("storage").GetValue<Storage>();

            return storage;
        }

        private void UpdateTint()
        {
            var items = GetStorage().GetItems().ToArray();

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

            controller.SetSymbolTint("liquid", color);
        }
    }
}
