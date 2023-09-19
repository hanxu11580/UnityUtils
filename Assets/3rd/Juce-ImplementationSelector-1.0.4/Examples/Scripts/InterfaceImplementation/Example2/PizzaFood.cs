using UnityEngine;

namespace Juce.ImplementationSelector.Example2
{
    [System.Serializable]
    public class PizzaFood : IFood
    {
        [SerializeField, Min(0)] private string pizzaType = default;
        [SerializeField, Min(0)] private int ammountOfSlices = default;

        public void Print() {
            Debug.Log($"这是一个{pizzaType}，它被切成了{pizzaType}块");
        }
    }
}
