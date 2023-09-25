
using System;
using UnityEngine;

public class MineOreAction : GoapAction
{
	private bool mined = false;
	// 铁矿
	private IronRockComponent targetRock; // where we get the ore from

	private float startTime = 0;
	public float miningDuration = 2; // seconds

	public MineOreAction () {
		// 条件 需要有工具 且 手里没有矿石
		// 结果	手里有矿石了
		addPrecondition ("hasTool", true); // we need a tool to do this
		addPrecondition ("hasOre", false); // if we have ore we don't want more
		addEffect ("hasOre", true);
	}
	
	
	public override void reset ()
	{
		mined = false;
		targetRock = null;
		startTime = 0;
	}
	
	public override bool isDone ()
	{
		return mined;
	}
	
	public override bool requiresInRange ()
	{
		// 需要靠近矿石
		return true;
	}
	
	public override bool checkProceduralPrecondition (GameObject agent)
	{
		// 找到所有的铁矿
		IronRockComponent[] rocks = FindObjectsOfType ( typeof(IronRockComponent) ) as IronRockComponent[];
		IronRockComponent closest = null;
		float closestDist = 0;
		
		// 找到离自己最近的铁矿
		foreach (IronRockComponent rock in rocks) {
			if (closest == null) {
				// first one, so choose it for now
				closest = rock;
				closestDist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
			} else {
				// is this one closer than the last?
				float dist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
				if (dist < closestDist) {
					// we found a closer one, use it
					closest = rock;
					closestDist = dist;
				}
			}
		}

		// 设置目标铁矿
		targetRock = closest;
		target = targetRock.gameObject;
		
		return closest != null;
	}
	
	public override bool perform (GameObject agent)
	{
		if (startTime == 0)
			startTime = Time.time;

		// 开采2秒
		if (Time.time - startTime > miningDuration) {
			// 把2个铁矿放入背包
			BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
			backpack.numOre += 2;
			mined = true;

			// 开采工具 被损耗50%
			// 判断是否该销毁了, 如果需要被销毁，将销毁
			// 并把背包里的工具设置为空
			ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;
			tool.use(0.5f);
			if (tool.destroyed()) {
				Destroy(backpack.tool);
				backpack.tool = null;
			}
		}
		return true;
	}
	
}


