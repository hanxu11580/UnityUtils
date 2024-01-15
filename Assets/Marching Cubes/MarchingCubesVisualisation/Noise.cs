using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MarchingCubes {
	public static class Noise {
		public static float PerlinNoise3D(float x, float y, float z) {
			float xy = Mathf.PerlinNoise(x, y);
			float xz = Mathf.PerlinNoise(x, z);
			float yz = Mathf.PerlinNoise(y, z);
			float yx = Mathf.PerlinNoise(y, x);
			float zx = Mathf.PerlinNoise(z, x);
			float zy = Mathf.PerlinNoise(z, y);

			return (xy + xz + yz + yx + zx + zy) / 6;
		}
	}
}