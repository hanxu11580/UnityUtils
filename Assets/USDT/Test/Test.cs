using System;
using UnityEditor;
using UnityEngine;


namespace USDT.Test {

    public class Test : MonoBehaviour {
        [SerializeReference] private Skill _skill;

        [ContextMenu("Set Attack Skill")]
        public void SetAttackSkill(MenuCommand command) {
            var example = (Test)command.context;
            example._skill = new AttackSkill();
            EditorUtility.SetDirty(example);
        }

        [ContextMenu("Set Buff Skill")]
        public void SetBuffSkill(MenuCommand command) {
            var example = (Test)command.context;
            example._skill = new BuffSkill();
            EditorUtility.SetDirty(example);
        }

    }

    [Serializable]
    public abstract class Skill {
        public float value;
    }

    [Serializable]
    public class AttackSkill : Skill {
    }

    [Serializable]
    public class BuffSkill : Skill {
        public BuffType buffType;
        public float duration;
    }

    public enum BuffType { Attack, Defense }
}