using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Unity.Labs.ListView
{
    sealed class Card : ListViewItem<CardData, int>
    {
        public enum Suit
        {
            Diamonds,
            Hearts,
            Spades,
            Clubs
        }

        static readonly Vector3 k_QuadOffset = Vector3.up * 0.001f; //Local y offset for placing quads

        [SerializeField]
        TextMesh m_TopNum;

        [SerializeField]
        TextMesh m_BotNum;

        [SerializeField]
        float m_CenterScale = 3f;

        [SerializeField]
        GameObject m_Diamond;

        [SerializeField]
        GameObject m_Heart;

        [SerializeField]
        GameObject m_Spade;

        [SerializeField]
        GameObject m_Club;

        [SerializeField]
        BoxCollider m_Collider;

        public BoxCollider boxCollider
        {
            get { return m_Collider; }
        }

        public override void Setup(CardData datum, bool firstTime = false)
        {
            base.Setup(datum, firstTime);

            var value = datum.value;
            m_TopNum.text = value;
            m_BotNum.text = value;

            DestroyChildren(transform);
            var prefab = m_Heart;
            var color = Color.red;
            switch (datum.suit)
            {
                case Suit.Clubs:
                    color = Color.black;
                    prefab = m_Club;
                    break;
                case Suit.Diamonds:
                    prefab = m_Diamond;
                    break;
                case Suit.Spades:
                    color = Color.black;
                    prefab = m_Spade;
                    break;
            }

            switch (value)
            {
                case "J":
                case "K":
                case "Q":
                case "A":
                {
                    var quad = AddQuad(prefab);
                    quad.transform.localScale *= m_CenterScale;
                    quad.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                    break;
                }

                default:
                {
                    var valNum = System.Convert.ToInt32(value);
                    var cols = valNum < 4 ? 1 : 2;
                    var rows = valNum / cols;
                    if (valNum == 8)
                        rows = 3;

                    var divisionY = 1f / (rows + 1);
                    var divisionX = 1f / (cols + 1);
                    var size = m_Collider.size;
                    var depth = Vector3.forward * size.z;
                    var width = Vector3.right * size.x;
                    var rotation = Quaternion.AngleAxis(90, Vector3.right);
                    for (var j = 0; j < cols; j++)
                    {
                        for (var i = 0; i < rows; i++)
                        {
                            var quad = AddQuad(prefab);
                            var quadTransform = quad.transform;

                            var delta = depth * (divisionY * (i + 1) - 0.5f) + width * (divisionX * (j + 1) - 0.5f);
                            quadTransform.localPosition += delta;
                            quadTransform.localRotation = rotation;
                        }
                    }

                    var leftover = 0;
                    switch (valNum)
                    {
                        case 5:
                            divisionY = 0f;
                            leftover = 1;
                            break;
                        case 7:
                            divisionY = 0.125f;
                            leftover = 1;
                            break;
                        case 8:
                            divisionY = 0.125f;
                            leftover = 3;
                            break;
                        case 9:
                            divisionY = 0.2f;
                            leftover = 1;
                            break;
                        case 10:
                            divisionY = 0.25f;
                            leftover = 3;
                            break;
                    }

                    for (var i = 0; i < leftover; i += 2)
                    {
                        var quad = AddQuad(prefab);
                        var quadTransform = quad.transform;
                        quadTransform.localPosition -= depth * (divisionY * (i - 1));
                        quadTransform.localRotation = rotation;
                    }

                    break;
                }
            }

            m_TopNum.text = value;
            m_TopNum.color = color;
            m_BotNum.text = value;
            m_BotNum.color = color;
        }

        void DestroyChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                var childGameObject = child.gameObject;
                if (childGameObject != m_BotNum.gameObject && childGameObject != m_TopNum.gameObject)
                    Destroy(childGameObject);
            }
        }

        GameObject AddQuad(GameObject prefab)
        {
            //NOTE: If we were really concerned about performance, we could pool the quads
            var quad = Instantiate(prefab);
            var quadTransform = quad.transform;
            quadTransform.parent = transform;
            quadTransform.localPosition = k_QuadOffset;
            return quad;
        }

        public static void FillDeck(List<CardData> deck, string template)
        {
            for (var i = 0; i < 4; i++)
            {
                for (var j = 1; j < 14; j++)
                {
                    string value;
                    switch (j)
                    {
                        case 1:
                            value = "A";
                            break;
                        case 11:
                            value = "J";
                            break;
                        case 12:
                            value = "Q";
                            break;
                        case 13:
                            value = "K";
                            break;
                        default:
                            value = j + "";
                            break;
                    }

                    deck.Add(new CardData(value, (Suit)i, i * 14 + j, template));
                }
            }
        }

        public static List<CardData> Shuffle(List<CardData> deck)
        {
            var rnd = new Random();
            return deck.OrderBy(x => rnd.Next()).ToList();
        }
    }
}
