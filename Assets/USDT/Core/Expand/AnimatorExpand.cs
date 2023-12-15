using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace USDT.Expand {
    public static class AnimatorExpand {
        public static bool IsPlaying(this Animator animator, string name = null) {
            if(animator == null) {
                return false;
            }

            var state = animator.GetCurrentAnimatorStateInfo(0);
            var isPlaying = state.normalizedTime <= 1 || animator.IsInTransition(0);
            if(isPlaying && !string.IsNullOrWhiteSpace(name)) {
                return state.IsName(name);
            }
            else {
                return isPlaying;
            }
        }
    }
}