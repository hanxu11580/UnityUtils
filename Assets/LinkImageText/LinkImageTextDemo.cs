using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkImageTextDemo : MonoBehaviour
{
    [SerializeField]
    LinkImageText _text;

    private void Start() {
        _text.onHrefClick.AddListener(OnHrefClick);
    }

    private void OnHrefClick(string url) {
        UnityEngine.Application.OpenURL(url);
    }
}
