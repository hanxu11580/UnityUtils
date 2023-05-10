using Lockstep.Math;
using System.Runtime.InteropServices;

namespace Lockstep.Collision2D {
    public class CRay : CBaseShape {
        public override int TypeId => (int) EShape2D.Ray;
        public LVector2 pos;
        public LVector2 dir;
    }
        
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Ray2D  {
        public int TypeId => (int) EShape2D.Ray;
        public LVector2 origin;
        public LVector2 direction;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LRaycastHit2D {
        public LVector2 point;
        public LFloat distance;
        public int colliderId;
        
    }
}