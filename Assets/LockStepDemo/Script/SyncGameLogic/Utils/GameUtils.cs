﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GameUtils
{
    public const string c_gameFinish =  "GameFinsih";
    public const string c_scoreChange = "ScoreChange";

    public const string c_addBuff    = "AddBuff";
    public const string c_removeBuff = "RemoveBuff";
    public const string c_HitBuff    = "HitBuff";

    public const string c_SkillHit = "SkillHit";
    public const string c_SkillStatusEnter = "SkillStatusEnter";
    public const string c_SkillFXTrigger = "SkillStatusEnter";

    public const int c_element_default = 0;

    public static string GetEventKey(int entityID, CharacterEventType EventType)
    {
        return entityID + EventType.ToString();
    }
}


