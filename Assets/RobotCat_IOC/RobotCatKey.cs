using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotCat {

    public class RobotCatKey : IEquatable<RobotCatKey> {

        public RobotCatServiceRegistry Registry { get; }
        public Type[] GenericArguments { get; }

        public RobotCatKey(RobotCatServiceRegistry registry, Type[] genericArguments) {
            Registry = registry;
            GenericArguments = genericArguments;
        }

        public bool Equals(RobotCatKey other) {
            if (Registry != other.Registry) return false;
            if (GenericArguments.Length != other.GenericArguments.Length) return false;
            for (int i = 0; i < GenericArguments.Length; i++) {
                if (GenericArguments[i].GetHashCode() != other.GenericArguments[i].GetHashCode()) return false;
            }
            return true;
        }

        public override int GetHashCode() {
            var hashCode = Registry.GetHashCode();
            for (int i = 0; i < GenericArguments.Length; i++) {
                hashCode ^= GenericArguments[i].GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj) {
            return obj is RobotCatKey key ? Equals(key) : false;
        }
    }
}