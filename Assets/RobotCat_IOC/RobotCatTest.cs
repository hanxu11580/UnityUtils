using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace RobotCat {

    public class RobotCatTest : MonoBehaviour {


        private void Start() {
            RobotCatContainer robotCatContainer = new RobotCatContainer();
            robotCatContainer.Register<IBar, Bar>(Lifetime.Root);
            
            
            var obj = robotCatContainer.GetService(typeof(IBar));

            lg.i(obj);
        }
    }


    public interface IBar {

    }

    public class Bar : IBar {
        public Bar() {

        }
    }

    public class Tar : IBar {

    }
}