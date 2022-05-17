using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechLib
{
    class TechTreeGraph
    {
        List<GraphTitle> graphTitles = new List<GraphTitle>();

        public void AddTitle(string titleID)
        {
            graphTitles.Add(new GraphTitle()
            {
                name = titleID
            });
        }

        public struct GraphTitle
        {
            public string name;
        }
    }
}
