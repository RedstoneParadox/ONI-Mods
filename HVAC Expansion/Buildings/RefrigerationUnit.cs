using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion.Buildings
{
    class RefrigerationUnit: AirConditioner
    {
        public Storage GetStorage()
        {
            return storage;
        }
    }
}
