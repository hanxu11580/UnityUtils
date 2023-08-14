using UnityEngine;

namespace USDT.Expand {
    public static class GameObjectExpand
    {
        /// <summary>
        /// 显示
        /// </summary>
        public static void Visible(this GameObject go, bool isShow) {
            if (go.activeSelf != isShow) { //多了条判断 避免重复设置SetActive
                go.SetActive(isShow);
            }
        }

        public static GameObject GetChildGameObject(this GameObject gameObject, params string[] childs)
        {
            if (gameObject == null) return null;
            string child = string.Empty;
            for (int i = 0; i < childs.Length - 1; i++)
            {
                child += $"{childs[i]}/";
            }
            child += childs[childs.Length - 1];
            Transform childTransform = gameObject.transform.Find(child);
            if (childTransform == null) return null;
            return childTransform.gameObject;
        }
        public static T GetChildComponent<T>(this GameObject gameObject, params string[] childs) where T : UnityEngine.Component
        {
            GameObject childGameObject = gameObject.GetChildGameObject(childs);
            if (childGameObject == null) return null;
            return childGameObject.GetComponent<T>();
        }
        public static T AddChildComponent<T>(this GameObject gameObject, params string[] childs) where T : UnityEngine.Component
        {
            GameObject childGameObject = gameObject.GetChildGameObject(childs);
            if (childGameObject == null) return null;
            return childGameObject.AddComponent<T>();
        }

        public static void SafeDestroy(this Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) {
                Object.Destroy(obj);
            }
            else {
                Object.DestroyImmediate(obj);
            }
        }
    }
}