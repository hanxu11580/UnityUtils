using System.Collections.Generic;
using UnityEngine;


// https://github.com/Juce-Assets/Juce-ImplementationSelector

namespace Juce.ImplementationSelector.Example1
{
    /* SerializeReference不知道啥意思，或者忘了
     * 就把下面的，跑一次就知道了
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
     */

    /*
     * 下面这个就是把选择的类名字去掉Food字段
     * 比如AppleFood类，选择项机会变成Apple
     * [SelectImplementationTrimDisplayName("Food")]
     * public interface IFood
     * {
     * }
     */

    /*
     * forceExpanded: true 类的属性是否可以折叠
     */

    public class SelectImplementationExample1 : MonoBehaviour
    {
        [SelectImplementation(typeof(IInteraface))]
        [SerializeField, SerializeReference] private List<IInteraface> listImplementations = new List<IInteraface>();

        [SelectImplementation(typeof(IInteraface), displayLabel: true, forceExpanded: true)]
        [SerializeField, SerializeReference] private List<IInteraface> listForceExpandImplementations = new List<IInteraface>();

        [SelectImplementation(typeof(IInteraface), displayLabel: false, forceExpanded: true)]
        [SerializeField, SerializeReference] private List<IInteraface> listForceExpandNoLabelImplementations = new List<IInteraface>();

        [Header("SingleImplementation")]
        [SelectImplementation(typeof(IInteraface))]
        [SerializeField, SerializeReference] private IInteraface singleImplementation = new Implementation2Interface();

        [Header("SingleForceExpandImplementation")]
        [SelectImplementation(typeof(IInteraface), displayLabel: true, forceExpanded: true)]
        [SerializeField, SerializeReference] private IInteraface singleForceExpandImplementation = new Implementation2Interface();

        [Header("SingleForceExpandNoLabelImplementation")]
        [SelectImplementation(typeof(IInteraface), displayLabel: false, forceExpanded: true)]
        [SerializeField, SerializeReference]
        private IInteraface singleForceExpandNoLabelImplementation = new Implementation2Interface();
    }
}
