using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechLib
{
    class TechTreeGraph
    {
        private List<GraphTitle> graphTitles = new List<GraphTitle>();

        public void AddTitle(string titleID)
        {
            graphTitles.Add(new GraphTitle(titleID));
        }

        public class GraphTitle
        {
            public readonly string Name;
            public Dictionary<int, List<GraphTech>> Columns;

            public GraphTitle(string name)
            {
                Name = name;
            }
        }

        public class GraphTech
        {
            public readonly string Name;
            public List<GraphTech> dependencies = new List<GraphTech>();

            public GraphTech(string Name)
            {

            }
        }
    }
}
