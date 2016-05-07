using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public delegate float NoiseMethod (Vector3 point, float frequency, bool interpolate);

public static class PseudorandomNoise  {


	public static NoiseMethod[] valueMethods = {
		Value1D,
		Value2D,
		Value3D
	};

	private static float Smooth (float t) {
		return t * t * t * (t * (t * 6f - 15f) + 10f);
	}

//	private static int[] hash = {
//
//		////permutation array or a permutation table
//		////0, 1, 2, 3, 4, 5, 6, 7
//		4, 12, 11, 8, 13, 10, 1, 5, 3, 7, 2, 0, 14, 6, 9, 15
//	};

	private static int[] hash = {
		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,


		151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
		140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
		247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
		57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
		74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
		60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
		65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
		200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
		52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
		207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
		119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
		129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
		218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
		81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
		184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
		222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
	};

	private const int hashMask = 255;


	public static float Value1D (Vector3 point, float frequency, bool interpolate) {

		if (!interpolate) {
			//////non interpolation
			point *= frequency;
			//int i = (int)point.x;
			int i = Mathf.FloorToInt (point.x);

			//return i % 2; ////returning the remainder of divided integer by two
			//return i & 1; ////look at the least significant bit to determine whether the number is odd

			//i &= 15;
			//return hash[i] / 15f; // ////lattice noise using permutation array as a hashing method

			i &= hashMask;
			return hash [i] * (1f / hashMask);

		} else {
			//////with interpolation also called Value noise
			//// perfoming interpolation between two hases, from One for the lattice coordinate to the left of our sample point, and one for the lattice coordinate to the right of it. 
			point *= frequency;
			int i0 = Mathf.FloorToInt (point.x);
			float t = point.x - i0;
			i0 &= hashMask;

			int i1 = i0 + 1;

			int h0 = hash [i0];
			int h1 = hash [i1];

			t = Smooth (t);
			return Mathf.Lerp (h0, h1, t) * (1f / hashMask);
		}
	}


	public static float Value2D (Vector3 point, float frequency, bool interpolate) {

		if (!interpolate) {

			//////non interpolation
			point *= frequency;

			int ix = Mathf.FloorToInt (point.x);
			int iy = Mathf.FloorToInt (point.y);
			ix &= hashMask;
			iy &= hashMask;

			//return hash[(hash[ix] + iy) & hashMask] * (1f / hashMask);
			return hash [hash [ix] + iy] * (1f / hashMask);

		} else {

			//////with interpolation - performing a bilinear interpolation between four hashes
			point *= frequency;
			int ix0 = Mathf.FloorToInt (point.x);
			int iy0 = Mathf.FloorToInt (point.y);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			ix0 &= hashMask;
			iy0 &= hashMask;

			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			int h0 = hash [ix0];
			int h1 = hash [ix1];
			int h00 = hash [h0 + iy0];
			int h10 = hash [h1 + iy0];
			int h01 = hash [h0 + iy1];
			int h11 = hash [h1 + iy1];


			tx = Smooth (tx);
			ty = Smooth (ty);
			return Mathf.Lerp (Mathf.Lerp (h00, h10, tx), Mathf.Lerp (h01, h11, tx), ty) * (1f / hashMask);
		}
	}

	public static float Value3D (Vector3 point, float frequency, bool interpolate) {

		if (!interpolate) {
			//////non interpolation
			point *= frequency;
			int ix = Mathf.FloorToInt(point.x);
			int iy = Mathf.FloorToInt(point.y);
			int iz = Mathf.FloorToInt(point.z);
			ix &= hashMask;
			iy &= hashMask;
			iz &= hashMask;

			//return hash[(hash[(hash[ix] + iy) & hashMask] + iz) & hashMask] * (1f / hashMask);
			return hash[hash[hash[ix] + iy] + iz] * (1f / hashMask);


		} else {
			////you end up with eight hashes and need to perform a trilinear interpolation
			point *= frequency;
			int ix0 = Mathf.FloorToInt (point.x);
			int iy0 = Mathf.FloorToInt (point.y);
			int iz0 = Mathf.FloorToInt (point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			ix0 &= hashMask;
			iy0 &= hashMask;
			iz0 &= hashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;

			int h0 = hash [ix0];
			int h1 = hash [ix1];
			int h00 = hash [h0 + iy0];
			int h10 = hash [h1 + iy0];
			int h01 = hash [h0 + iy1];
			int h11 = hash [h1 + iy1];
			int h000 = hash [h00 + iz0];
			int h100 = hash [h10 + iz0];
			int h010 = hash [h01 + iz0];
			int h110 = hash [h11 + iz0];
			int h001 = hash [h00 + iz1];
			int h101 = hash [h10 + iz1];
			int h011 = hash [h01 + iz1];
			int h111 = hash [h11 + iz1];

			tx = Smooth (tx);
			ty = Smooth (ty);
			tz = Smooth (tz);
			return Mathf.Lerp (
				Mathf.Lerp (Mathf.Lerp (h000, h100, tx), Mathf.Lerp (h010, h110, tx), ty),
				Mathf.Lerp (Mathf.Lerp (h001, h101, tx), Mathf.Lerp (h011, h111, tx), ty),
				tz) * (1f / hashMask);
		}
	}

}
