using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RobotCat {
    public enum Lifetime {
        Self,
        Root,
        Transient
    }

    public class RobotCatServiceRegistry {

        public Type ServiceType { get; set; }

        public Lifetime Lifetime { get; set; }

        public Func<RobotCatContainer, Type[], object> Factory { get; set; }

        public RobotCatServiceRegistry Next { get; set; }

        public RobotCatServiceRegistry(Type serviceType, Lifetime lifetime, Func<RobotCatContainer, Type[], object> factory) {
            ServiceType = serviceType;
            Lifetime = lifetime;
            Factory = factory;
        }

        internal IEnumerable<RobotCatServiceRegistry> AsEnumerable() {
            var list = new List<RobotCatServiceRegistry>();
            for (var self = this; self != null; self = Next) {
                list.Add(self);
            }
            return list;
        }
    }
}