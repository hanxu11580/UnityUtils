using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */
public class GoapPlanner
{

	/**
	 * Plan what sequence of actions can fulfill the goal.
	 * Returns null if a plan could not be found, or a list of the actions
	 * that must be performed, in order, to fulfill the goal.
	 */
	public Queue<GoapAction> plan(GameObject agent,
								  HashSet<GoapAction> availableActions, 
	                              HashSet<KeyValuePair<string,object>> worldState, 
	                              HashSet<KeyValuePair<string,object>> goal) 
	{
		// reset the actions so we can start fresh with them
		foreach (GoapAction a in availableActions) {
			a.doReset ();
		}

		// ɸѡ�����õ�Action
		HashSet<GoapAction> usableActions = new HashSet<GoapAction> ();
		foreach (GoapAction a in availableActions) {
			if ( a.checkProceduralPrecondition(agent) )
				usableActions.Add(a);
		}
		
		// we now have all actions that can run, stored in usableActions

		// build up the tree and record the leaf nodes that provide a solution to the goal.
		List<Node> leaves = new List<Node>();

		// build graph
		Node start = new Node (null, 0, worldState, null);
		bool success = buildGraph(start, leaves, usableActions, goal);

		if (!success) {
			// oh no, we didn't get a plan
			Debug.Log("NO PLAN");
			return null;
		}

		// �ҵ�����˻�����Node
		Node cheapest = null;
		foreach (Node leaf in leaves) {
			if (cheapest == null)
				cheapest = leaf;
			else {
				if (leaf.runningCost < cheapest.runningCost)
					cheapest = leaf;
			}
		}

		// ��Ŀ��Node��ԭ����ɵ�·��
		List<GoapAction> result = new List<GoapAction> ();
		Node n = cheapest;
		while (n != null) {
			if (n.action != null) {
				result.Insert(0, n.action);
			}
			n = n.parent;
		}
		// we now have this action list in correct order

		Queue<GoapAction> queue = new Queue<GoapAction> ();
		foreach (GoapAction a in result) {
			queue.Enqueue(a);
		}

		// hooray we have a plan!
		return queue;
	}

	/**
	 * Returns true if at least one solution was found.
	 * The possible paths are stored in the leaves list. Each leaf has a
	 * 'runningCost' value where the lowest cost will be the best action
	 * sequence.
	 */
	private bool buildGraph (Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
	{
		bool foundOne = false;

		// go through each action available at this node and see if we can use it here
		foreach (GoapAction action in usableActions) {

			// ������Ϊ���жϵ�ǰʱ��״̬�Ƿ�����
			if ( inState(action.Preconditions, parent.state) ) {

				// �����Ϊ��Ч���ᣬӰ�쵽����״̬������ı�����״̬
				HashSet<KeyValuePair<string,object>> currentState = populateState (parent.state, action.Effects);
                //Debug.Log(GoapAgent.prettyPrint(currentState));
				// ������Node�����ϸ�NodeΪ���ڵ�
                Node node = new Node(parent, parent.runningCost+action.cost, currentState, action);

				// ����ı������磬����Ŀ��
				// ˵���ҵ����յĽ��������
				if (inState(goal, currentState)) {
					// we found a solution!
					leaves.Add(node);
					foundOne = true;
				} else {
					// ��ʹ�õĽڵ��Ƴ�������������ͼ��
					HashSet<GoapAction> subset = actionSubset(usableActions, action);
					bool found = buildGraph(node, leaves, subset, goal);
					if (found)
						foundOne = true;
				}
			}
		}

		return foundOne;
	}

	/**
	 * Create a subset of the actions excluding the removeMe one. Creates a new set.
	 */
	private HashSet<GoapAction> actionSubset(HashSet<GoapAction> actions, GoapAction removeMe) {
		HashSet<GoapAction> subset = new HashSet<GoapAction> ();
		foreach (GoapAction a in actions) {
			if (!a.Equals(removeMe))
				subset.Add(a);
		}
		return subset;
	}

	/**
	 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
	 * then this returns false.
	 */
	private bool inState(HashSet<KeyValuePair<string,object>> test, HashSet<KeyValuePair<string,object>> worldState) {
		bool allMatch = true;
		// ��������
		foreach (KeyValuePair<string,object> t in test) {
			bool match = false;
			// ������״̬�����ң����Ƿ���������
			foreach (KeyValuePair<string,object> s in worldState) {
				if (s.Equals(t)) {
					match = true;
					break;
				}
			}
			if (!match)
				allMatch = false;
		}
		return allMatch;
	}
	
	/**
	 * Apply the stateChange to the currentState
	 */
	private HashSet<KeyValuePair<string,object>> populateState(HashSet<KeyValuePair<string,object>> currentState, HashSet<KeyValuePair<string,object>> stateChange) {
		HashSet<KeyValuePair<string,object>> state = new HashSet<KeyValuePair<string,object>> ();
		// copy the KVPs over as new objects
		foreach (KeyValuePair<string,object> s in currentState) {
			state.Add(new KeyValuePair<string, object>(s.Key,s.Value));
		}

		foreach (KeyValuePair<string,object> change in stateChange) {
			// if the key exists in the current state, update the Value
			bool exists = false;

			foreach (KeyValuePair<string,object> s in state) {
				if (s.Equals(change)) {
					exists = true;
					break;
				}
			}

			if (exists) {
				state.RemoveWhere( (KeyValuePair<string,object> kvp) => { return kvp.Key.Equals (change.Key); } );
				KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key,change.Value);
				state.Add(updated);
			}
			// if it does not exist in the current state, add it
			else {
				state.Add(new KeyValuePair<string, object>(change.Key,change.Value));
			}
		}
		return state;
	}

	/**
	 * Used for building up the graph and holding the running costs of actions.
	 */
	private class Node {
		public Node parent;
		public float runningCost;
		public HashSet<KeyValuePair<string,object>> state;
		public GoapAction action;

		public Node(Node parent, float runningCost, HashSet<KeyValuePair<string,object>> state, GoapAction action) {
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
		}
	}

}


