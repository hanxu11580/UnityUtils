using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 关联文章：https://indienova.com/u/ivan/blogread/43

public sealed class GoapAgent : MonoBehaviour {

	private FSM stateMachine;

	// 找些事情做
	private FSM.FSMState idleState;
	// 去为目标做事
	private FSM.FSMState moveToState;
	// 执行具体行为
	private FSM.FSMState performActionState;
	
	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

	private GoapPlanner planner;


	void Start () {
		stateMachine = new FSM ();
		availableActions = new HashSet<GoapAction> ();
		currentActions = new Queue<GoapAction> ();
		planner = new GoapPlanner ();
		// 设置含有目标的对象dataProvider
		findDataProvider();
		// idle状态主要就是找找计划, 转换新的状态
		createIdleState ();
		// 移动到目标位置
		createMoveToState ();
		// 
		createPerformActionState ();
		stateMachine.pushState (idleState);
		loadActions ();
	}
	

	void Update () {
		stateMachine.Update (this.gameObject);
	}


	public void addAction(GoapAction a) {
		availableActions.Add (a);
	}

	public GoapAction getAction(Type action) {
		foreach (GoapAction g in availableActions) {
			if (g.GetType().Equals(action) )
			    return g;
		}
		return null;
	}

	public void removeAction(GoapAction action) {
		availableActions.Remove (action);
	}

	private bool hasActionPlan() {
		return currentActions.Count > 0;
	}

	private void createIdleState() {
		idleState = (fsm, gameObj) => {
			// GOAP planning

			// get the world state and the goal we want to plan for
			HashSet<KeyValuePair<string,object>> worldState = dataProvider.getWorldState();
			HashSet<KeyValuePair<string,object>> goal = dataProvider.createGoalState();

			// Plan
			Queue<GoapAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
			if (plan != null) {
				// we have a plan, hooray!
				currentActions = plan;
				// 打印一下
				dataProvider.planFound(goal, plan);

				fsm.popState(); // move to PerformAction state
				// 设置执行 行为的状态
				fsm.pushState(performActionState);

			} else {
				// ugh, we couldn't get a plan
				Debug.Log("<color=orange>Failed Plan:</color>"+prettyPrint(goal));
				// 打印一下
				dataProvider.planFailed(goal);
				fsm.popState (); // move back to IdleAction state
				// 继续执行idle状态
				fsm.pushState (idleState);
			}

		};
	}
	
	private void createMoveToState() {
		moveToState = (fsm, gameObj) => {
			// move the game object

			GoapAction action = currentActions.Peek();
			// 需要靠近目标，但是target为空了
			if (action.requiresInRange() && action.target == null) {
				Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			// 有个问题，如果action.requiresInRange()返回false。不需要靠近目标，下面的逻辑是不是就可以跳过了？ 进入到这个状态，action.requiresInRange()必然是true
			// 移动到目标位置
			if ( dataProvider.moveAgent(action) ) {
				// 弹出当前状态，返回行为执行状态
				fsm.popState();
			}

			/*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			if (movable == null) {
				Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			float step = movable.moveSpeed * Time.deltaTime;
			gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

			if (gameObj.transform.position.Equals(action.target.transform.position) ) {
				// we are at the target location, we are done
				action.setInRange(true);
				fsm.popState();
			}*/
		};
	}
	
	private void createPerformActionState() {

		performActionState = (fsm, gameObj) => {

			// 如果计划中没有了要执行的行为，进入待机状态
			if (!hasActionPlan()) {
				Debug.Log("<color=red>Done actions</color>");
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
				return;
			}

			// 如果计划还有行为，判断行为是否完成
			// 完成后将从行为序列中弹出
			GoapAction action = currentActions.Peek();
			if ( action.isDone() ) {
				// the action is done. Remove it so we can perform the next one
				currentActions.Dequeue();
			}
			// 二次判断是否计划完成
			// 如果计划中还有行为
			if (hasActionPlan()) {
				action = currentActions.Peek();
				// 如果需要靠近目标，action.isInRange()表示在目标附近
				// 如果不需要靠近目标，之间返回true，表示准备好了
				bool inRange = action.requiresInRange() ? action.isInRange() : true;

				if ( inRange ) {
					// we are in range, so perform the action
					bool success = action.perform(gameObj);
					
					// 如果行为执行失败，将会重新进入待机状态（重新计划）
					if (!success) {
						// action failed, we need to plan again
						fsm.popState();
						fsm.pushState(idleState);
						dataProvider.planAborted(action);
					}
				} else {
					// 如果在返回那么进入，移动到目标区域状态
					// 当目标移动到目标区域，此时会返回到当前行为执行状态
					// we need to move there first
					// push moveTo state
					fsm.pushState(moveToState);
				}

			} else {
				// 如果计划中没有行为可以执行了
				// 继续进入待机状态
				// no actions left, move to Plan state
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
			}

		};
	}

	private void findDataProvider() {
		foreach (Component comp in gameObject.GetComponents(typeof(Component)) ) {
			if ( typeof(IGoap).IsAssignableFrom(comp.GetType()) ) {
				dataProvider = (IGoap)comp;
				return;
			}
		}
	}

	private void loadActions ()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();
		foreach (GoapAction a in actions) {
			availableActions.Add (a);
		}
		Debug.Log("Found actions: "+prettyPrint(actions));
	}

	public static string prettyPrint(HashSet<KeyValuePair<string,object>> state) {
		String s = "";
		foreach (KeyValuePair<string,object> kvp in state) {
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string prettyPrint(Queue<GoapAction> actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
			s += "-> ";
		}
		s += "GOAL";
		return s;
	}

	public static string prettyPrint(GoapAction[] actions) {
		String s = "";
		foreach (GoapAction a in actions) {
			s += a.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string prettyPrint(GoapAction action) {
		String s = ""+action.GetType().Name;
		return s;
	}
}
