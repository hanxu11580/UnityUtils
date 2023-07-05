using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup tapGroup;
    [HideInInspector]public Image background;

    private void Start()
    {
        background = GetComponent<Image>();
        tapGroup.Register(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tapGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tapGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tapGroup.OnTabSelected(this);
    }
}
