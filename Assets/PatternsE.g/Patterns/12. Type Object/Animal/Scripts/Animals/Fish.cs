using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TypeObject.Animal
{
    public class Fish : Animal
    {
        //This is the Type Object
        private IFlyingType flyingType;

        public Fish(string name, bool canFly)
        {
            this.name = name;

            if (canFly) {
                this.flyingType = new ICanFly();
            }
            else {
                this.flyingType = new ICantFly();
            }
        }

        public override void Talk()
        {
            string canFlyString = flyingType.CanIFly() ? "can" : "can't";

            Debug.Log($"Hello this is {name}, I'm a fish, and I {canFlyString} fly!");
        }
    }
}
