using System.Collections.Generic;
using UnityEngine;

namespace Juce.ImplementationSelector.Example2
{
    public class SelectImplementationExample2 : MonoBehaviour
    {
        [SelectImplementation(typeof(IFood), forceExpanded: true)]
        [SerializeField, SerializeReference] private IFood food = default;

        [SelectImplementation(typeof(IFood), forceExpanded: true)]
        [SerializeReference] private List<IFood> foods = new List<IFood>();


        private void Start() {
            foreach (var food in foods) {
                food.Print();
            }
        }
    }
}
