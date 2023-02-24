﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurgenceSystem :SystemBase
{

    public override void Init()
    {
        AddEntityCompChangeLisenter();
    }

    public override void Dispose()
    {
        RemoveEntityCompChangeLisenter();
    }

    public override Type[] GetFilter()
    {
        return new Type[] {
            typeof(LifeComponent),
        };
    }

    public override void FixedUpdate(int deltaTime)
    {
        List<EntityBase> list = GetEntityList();

        for (int i = 0; i < list.Count; i++)
        {
            LifeComponent lc = list[i].GetComp<LifeComponent>();

            if(lc.Life < 0)
            {
                lc.ResurgenceTimer += deltaTime;

                if(lc.ResurgenceTimer > 10 * 1000)
                {
                    lc.Life = lc.maxLife;
                    //m_world.eventSystem.DispatchEvent(GameUtils.GetEventKey(list[i].ID, CharacterEventType.Recover), list[i]);
                }
            }
        }
    }

    //public override void OnEntityCompChange(EntityBase entity, string compName, ComponentBase previousComponent, ComponentBase newComponent)
    //{
    //    if(compName == "LifeComponent")
    //    {
    //        LifeComponent oldLc = (LifeComponent)previousComponent;
    //        LifeComponent newLc = (LifeComponent)newComponent;

    //        //Debug.Log("OnEntityCompChange " + oldLc.life + " --> " + newLc.life);

    //        if (oldLc.life < 0 && newLc.life > 0)
    //        {
    //            m_world.eventSystem.DispatchEvent(GameUtils.GetEventKey(entity.ID, CharacterEventType.Recover), entity);
    //        }

    //        if (oldLc.life > 0 && newLc.life < 0)
    //        {
    //            m_world.eventSystem.DispatchEvent(GameUtils.GetEventKey(entity.ID, CharacterEventType.Die), entity);
    //        }
    //    }
    //}

    //void DispatchEvent(LifeComponent comp)
    //{
    //    if (comp.life > 0 && comp.isAlive == false)
    //    {
    //        comp.isAlive = true;
    //        comp.Entity.World.eventSystem.DispatchEvent(GameUtils.GetEventKey(comp.Entity.ID, CharacterEventType.Recover), comp.Entity);
    //    }

    //    if (comp.life < 0 && comp.isAlive == true)
    //    {
    //        comp.isAlive = false;
    //        comp.Entity.World.eventSystem.DispatchEvent(GameUtils.GetEventKey(comp.Entity.ID, CharacterEventType.Die), comp.Entity);
    //        comp.ResurgenceTimer = 0;
    //    }
    //}
    }
