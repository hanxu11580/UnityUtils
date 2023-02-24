﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollRectInput : ScrollRect
{
    public string m_UIEventKey;
    InputEventRegisterInfo<InputUIOnScrollEvent> m_register;

    public virtual void Init(string UIEventKey)
    {
        m_UIEventKey = UIEventKey;
        m_register = InputUIEventProxy.GetOnScrollListener(m_UIEventKey, name, OnSetContentAnchoredPosition);
    }

    public virtual void Dispose()
    {
        m_register.RemoveListener(true);
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        InputUIEventProxy.DispatchScrollEvent(m_UIEventKey, name,"", position);
    }

    protected virtual void OnSetContentAnchoredPosition(InputUIOnScrollEvent e)
    {
        base.SetContentAnchoredPosition(e.m_pos);
    }

}
