using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.Buildings
{
    class RefrigerationUnit: AirConditioner, ISimEveryTick
    {
        public Storage GetStorage()
        {
            return storage;
        }

        public void SimEveryTick(float dt)
        {
            var controller = GetComponent<KBatchedAnimController>();
            var items = storage.GetItems().ToArray();

            if (items.Length <= 0) return;

            var primaryElement = items[0].GetComponent<PrimaryElement>();
            var color = primaryElement.Element.substance.uiColour;

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
