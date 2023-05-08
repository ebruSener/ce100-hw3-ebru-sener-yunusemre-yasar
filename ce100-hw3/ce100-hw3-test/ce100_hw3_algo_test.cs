
using System.Collections;

namespace ce100_hw3_algo_test
{
     

    
    public class AssemblyGuideTest
    {
        [Fact]
       
        public void TestGetAssemblySteps()
        {
            // Arrange
            var itemA = new Item("A");
            var itemB = new Item("B");
            var itemC = new Item("C");
            var itemD = new Item("D");

            itemB.AddDependency(itemA);
            itemC.AddDependency(itemB);
            itemD.AddDependency(itemC);

            var guide = new AssemblyGuide();
            guide.AddItem(itemA);
            guide.AddItem(itemB);
            guide.AddItem(itemC);
            guide.AddItem(itemD);

            // Act
            var assemblySteps = guide.GetAssemblySteps();

            // Assert
            var expectedSteps = new ArrayList
            {
                "1. Assemble A",
                "2. Assemble B",
                "3. Assemble C",
                "4. Assemble D"
            };
            CollectionAssert.AreEqual(expectedSteps, assemblySteps);
        }
    }

    internal class CollectionAssert
    {
        internal static void AreEqual(ArrayList expectedSteps, object assemblySteps)
        {
            throw new NotImplementedException();
        }
    }

    internal class AssemblyGuide
    {
        public AssemblyGuide()
        {
        }

        internal void AddItem(Item itemA)
        {
            throw new NotImplementedException();
        }

        internal object GetAssemblySteps()
        {
            throw new NotImplementedException();
        }
    }
}
