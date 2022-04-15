using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticUtils
{
    public static int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }

    public static Vector3[] TransformMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale)
        {
           
            Vector3[] vertices = mesh.vertices;
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint(vertices[i]);
            }
            return vertices;
        }
}
