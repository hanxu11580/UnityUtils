using UnityEngine;
using System.Collections;
namespace GOAP {
public interface FSMState 
{
	
	void Update (FSM fsm, GameObject gameObject);
}
}
