using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.MyTest {

    public class WorkAction : GoapAction {

        private float _startWorkTime;
        private bool _isDone;
        private LazyDog _lazyDog;

        private void Start() {
            _lazyDog = GetComponent<LazyDog>();
            // ����ǰ����
            addPrecondition("lieFlat", false);
            // ������ı�Ľ��
            addEffect("addCoin", true);
        }

        public override bool isDone() {
            return _isDone;
        }

        public override bool checkProceduralPrecondition(GameObject agent) {
            // ��ʱûɶ�Ⱦ�����
            return true;
        }


        public override bool perform(GameObject agent) {
            if (_startWorkTime == 0) {
                _startWorkTime = Time.time;
            }
            // ����1�룬�൱����һ��
            if (Time.time - _startWorkTime > 1) {
                _lazyDog.Coin += 1000;
                _isDone = true;
            }
            return true;
        }

        public override bool requiresInRange() {
            return false;
        }

        public override void reset() {
            _startWorkTime = 0f;
            _isDone = false;
        }
    }
}
