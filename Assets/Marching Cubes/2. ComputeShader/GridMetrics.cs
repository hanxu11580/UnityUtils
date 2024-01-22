
namespace MarchingCubes_ComputeShader {
	public static class GridMetrics {
		public const int NumThreads = 8;

		public static int[] LODs = {
			8,
			16,
			24,
			32,
			40,
            48,
            56,
            64,
        };

		public static int LastLod = LODs.Length - 1;

		public static int PointsPerChunk(int lod) {
			return LODs[lod];
		}

		public static int ThreadGroups(int lod) {
			return LODs[lod] / NumThreads;
		}

		public const int Scale = 32;

		public const int GroundLevel = Scale / 2;
	}
}