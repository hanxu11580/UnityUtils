using UnityEngine;

namespace Unity.Labs.ListView
{
    sealed class CardGameInputHandler : MonoBehaviour
    {
        [SerializeField]
        CardGameList m_CardGameList;

        [SerializeField]
        CardGameHand m_Hand;

        Camera m_MainCamera;

        void Start()
        {
            m_MainCamera = Camera.main;
            if (m_MainCamera == null)
                enabled = false;
        }

        void Update()
        {
            var ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var hitCollider = hit.collider;
                    if (hitCollider.name.Equals("Deck"))
                    {
                        m_CardGameList.Deal();
                    }
                    else
                    {
                        var item = hitCollider.GetComponent<Card>();
                        if (item)
                        {
                            var parent = item.transform.parent;
                            if (parent == m_CardGameList.transform)
                                m_Hand.DrawCard(item);
                            else if (parent == m_Hand.transform)
                                m_Hand.ReturnCard(item);
                        }
                    }
                }
            }
        }
    }
}
