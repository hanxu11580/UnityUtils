using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Juce.ImplementationSelector.Example2 {

    [CreateAssetMenu(menuName = "ImplementationSelector", fileName = "FoodSO")]
    public class FoodSO : ScriptableObject {
        [SelectImplementation(typeof(IFood))]
        [SerializeReference] private List<IFood> foods = new List<IFood>();
    }
}