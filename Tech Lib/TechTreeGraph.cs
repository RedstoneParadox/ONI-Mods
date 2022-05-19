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
        private Dictionary<string, GraphTech> techsReference = new Dictionary<string, GraphTech>();

        public void AddTitle(string titleID)
        {
            graphTitles.Add(new GraphTitle(titleID));
        }

        public void AddTech(string techID)
        {
            if (techsReference.ContainsKey(techID))
            {
                Debug.LogError($"Tried to add tech '{techID}' to graph when it already exists.");
            }
        }

        private int CalculateColumn(List<GraphTech> dependencies)
        {
            int column = -1;

            foreach (GraphTitle title in graphTitles)
            {
                foreach (var entry in title.Columns)
                {
                    int index = entry.Key;
                    List<GraphTech> columnTechs = entry.Value;

                    foreach (GraphTech dependency in dependencies)
                    {
                        if (columnTechs.Contains(dependency))
                        {
                            column = index;
                            break;
                        }
                    }
                }
            }

            return column + 1;
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
            public readonly int Column;
            public List<GraphTech> dependencies = new List<GraphTech>();

            public GraphTech(string name)
            {
                Name = name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
    }
}
