using UnityEngine;

namespace Juce.ImplementationSelector.Example2
{
    [System.Serializable]
    public class GrapesFood : IFood
    {
        [SerializeField, Min(0)] private int ammountOfGrapes = default;

        public void Print() {
            Debug.Log($"这个葡萄有{ammountOfGrapes}颗");
        }
    }
}
