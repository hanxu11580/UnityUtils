using UnityEngine;

namespace Juce.ImplementationSelector.Example2
{
    [SelectImplementationDefaultType]
    [System.Serializable]
    public class AppleFood : IFood
    {
        [SerializeField] private string appleName = default;

        public void Print() {
            Debug.Log($"这个苹果的名字是:{appleName}");
        }
    }
}
