using System.Numerics;
using Emotion.Primitives;
using Emotion.Utility;

namespace Solution12.Graphics.Data
{
    public class Quadrilateral : Transform
    {
        public Vector3 Vertex0;
        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;

        public Quadrilateral(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;
            
            Bounds = Rectangle.BoundsFromPolygonPoints(new []{ Vertex0.ToVec2(), Vertex1.ToVec2(), Vertex2.ToVec2(), Vertex3.ToVec2() });
        }
    }
}