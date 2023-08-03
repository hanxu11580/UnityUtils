using UnityEngine;

namespace USDT.Expand {
    public static class MaterialExpand {
        static int colorPropertyId = Shader.PropertyToID("_Color");

        /// <summary>
        /// 设置材质
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="mat"></param>
        public static void SetMaterial(this Transform trans, Material mat)
        {
            MeshRenderer renderer = trans.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                //Debug.Log(trans.name + "组件不存在Renderer");
                return;
            }
            renderer.material = mat;
        }

        /// <summary>
        /// 设置材质颜色，普通设置颜色会创建新材质,造成不必要的开销
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="newColor"></param>
        public static void SetMaterialColor(this Transform trans, Color newColor)
        {
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(colorPropertyId, newColor);
            MeshRenderer meshRenderer = trans.GetComponent<MeshRenderer>();
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
        public static void SetMaterialColor(this MeshRenderer mr, Color newColor)
        {
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(colorPropertyId, newColor);
            mr.SetPropertyBlock(propertyBlock);
        }
    }
}
