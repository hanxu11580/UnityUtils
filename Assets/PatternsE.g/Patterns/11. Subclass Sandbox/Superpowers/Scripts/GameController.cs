using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubclassSandbox.Superpowers
{
    // 子类组合父类的行为
    //Implementation of the Subclass Sandbox pattern from the book Game Programming Patterns
    public class GameController : MonoBehaviour
    {
        private SkyLaunch skyLaunch;


        void Start()
        {
            skyLaunch = new SkyLaunch();
        }


        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                skyLaunch.Activate();
            }
        }
    }
}
