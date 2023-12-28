using UnityEngine.UI;

namespace USDT.Components {
    /// <summary>
    /// 仅点击，不产生OverDraw
    /// </summary>
    public class OnlyClickImage : Image
    {
        protected OnlyClickImage() {
            useLegacyMeshGeneration = false;
        }


        protected override void OnPopulateMesh(VertexHelper toFill) {
            toFill.Clear();
        }
    }
}