using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;




namespace ce100_hw3_algo_lib
{


    class Item
    {
        public string Name { get; set; }
        public List<Item> Dependencies { get; set; }
        public bool Visited { get; set; }
        public bool Finished { get; set; }

        public Item(string name)
        {
            Name = name;
            Dependencies = new List<Item>();
            Visited = false;
            Finished = false;
        }

        public void AddDependency(Item dependency)
        {
            Dependencies.Add(dependency);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class AssemblyGuide
    {
        private List<Item> Items { get; set; }

        public AssemblyGuide()
        {
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void BuildConnections()
        {
            // TODO: Parse the assembly instructions from the IKEA website and build the connections between the items based on their dependencies
        }

        public List<Item> TopologicalSort()
        {
            var sorted = new List<Item>();
            foreach (var item in Items)
            {
                if (!item.Visited)
                {
                    TopologicalSortDFS(item, sorted);
                }
            }
            sorted.Reverse();
            return sorted;
        }

        private void TopologicalSortDFS(Item item, List<Item> sorted)
        {
            item.Visited = true;
            foreach (var dependency in item.Dependencies)
            {
                if (!dependency.Visited)
                {
                    TopologicalSortDFS(dependency, sorted);
                }
                else if (!dependency.Finished)
                {
                    throw new Exception("Cycle detected in the assembly guide");
                }
            }
            item.Finished = true;
            sorted.Add(item);
        }

        public ArrayList GetAssemblySteps()
        {
            var sorted = TopologicalSort();
            var steps = new ArrayList();
            for (int i = 0; i < sorted.Count; i++)
            {
                steps.Add($"{i + 1}. Assemble {sorted[i]}");
            }
            return steps;
        }
    }


}
