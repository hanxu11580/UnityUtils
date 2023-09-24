using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PureMVC.MyTest {
    public class DataCommand : SimpleCommand {

        public override void Execute(INotification notification) {
            var proxy = Facade.RetrieveProxy(DataProxy.NAME) as DataProxy;
            proxy.AddScore(10);
        }
    }
}