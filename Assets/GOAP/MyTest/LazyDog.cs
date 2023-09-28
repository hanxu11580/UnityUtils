using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.MyTest {

    public class LazyDog : MonoBehaviour, IGoap {
        [SerializeField] private int _coin;
        public int Coin { get => _coin; set => _coin = value; }

        [SerializeField] private int _targetCoin;
        /// <summary>
        /// 懒狗的目标是什么?
        /// 躺平
        /// </summary>
        /// <returns></returns>
        public HashSet<KeyValuePair<string, object>> createGoalState() {
            HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>> {
                // 躺平
                new KeyValuePair<string, object>("lieFlat", true)
            };
            return goal;
        }

        public HashSet<KeyValuePair<string, object>> getWorldState() {
            HashSet<KeyValuePair<string, object>> worldState = new HashSet<KeyValuePair<string, object>> {
                new KeyValuePair<string, object>("lieFlat", CheckLieFlat()),
            };
            return worldState;
        }

        public bool moveAgent(GoapAction nextAction) {
            return true;
        }

        public void planAborted(GoapAction aborter) {
            Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
        }

        public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal) {

        }

        public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions) {
            Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
        }
        public void actionsFinished() {
            Debug.Log("<color=blue>Actions completed</color>");
        }

        public bool CheckLieFlat() {
            return _coin >= _targetCoin;
        }
    }
}