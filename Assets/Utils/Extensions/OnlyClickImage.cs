using UnityEngine.UI;

namespace XhO_OKit
{
    /// <summary>
    /// 仅点击，不产生OverDraw
    /// </summary>
    public class OnlyClickImage : Image
    {
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}