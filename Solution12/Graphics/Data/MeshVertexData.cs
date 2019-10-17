#region Using

using System.Numerics;
using System.Runtime.InteropServices;
using Emotion.Graphics.Data;

#endregion

namespace Solution12.Graphics.Data
{
    /// <summary>
    ///  A <see cref="VertexData"/> -like struct, that also stores a pre-calculated Normal Vector.
    /// </summary>
    public struct MeshVertexData
    {
        public static readonly int SizeInBytes = Marshal.SizeOf(new MeshVertexData());

        [VertexAttribute(3, false)] public Vector3 Vertex;
        [VertexAttribute(2, false)] public Vector2 UV;
        [VertexAttribute(1, true)] public float Tid;
        [VertexAttribute(4, true, typeof(byte))] public uint Color;
        [VertexAttribute(3, false)] public Vector3 Normal;
    }
}