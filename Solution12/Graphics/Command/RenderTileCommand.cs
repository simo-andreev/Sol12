﻿// using System.Numerics;
// using Emotion.Graphics;
// using Emotion.Graphics.Command;
// using Emotion.Graphics.Objects;
// using Emotion.Primitives;
//
// namespace Solution12.Graphics.Command
// {
//     /// <summary>
//     /// Render Command specifically for rendering a sprite between 4 vertices, as opposed to the default behaviour of <see cref="RenderSpriteCommand"/>,
//     /// which takes a single Vertex and generates the rest as [X,Y] offsets based on <see cref="Emotion.Graphics.Command.RenderSpriteCommand.Size"/>
//     /// </summary>
//     public class RenderTileCommand : RenderCommand
//     {
//         public readonly Vector3[] Normals = new Vector3[4];
//         private readonly Vector3[] _boundingVertices;
//         private static readonly int[] TriangleIndices = {0, 1, 2, 2, 3, 0};
//
//         public RenderTileCommand(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Texture texture = null) : base()
//         {
//             this._boundingVertices = new Vector3[4] {vertex0, vertex1, vertex2, vertex3};
//
//             this.Position = vertex0;
//             this.Color = Color.White.ToUint();
//             this.Texture = texture;
//         }
//
//         public override void Process()
//         {
//             base.Process();
//
//             // Reassign all but the first vertex, from the ones generated by RenderSpriteCommand#Process to ones passed @ the Constructor
//             for (var i = 1; i < Vertices.Length; i++)
//                 Vertices[i].Vertex = _boundingVertices[i];
//
//             // Generate normal for lighting.
//             for (int v = 0; v < TriangleIndices.Length; v += 3)
//             {
//                 // Normal for the whole triangle face.
//                 Vector3 v1 = Vertices[TriangleIndices[v + 1]].Vertex - Vertices[TriangleIndices[v]].Vertex;
//                 Vector3 v2 = Vertices[TriangleIndices[v + 2]].Vertex - Vertices[TriangleIndices[v]].Vertex;
//                 Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(v1, v2));
//
//                 // Add the Face/Surface normal for the triangle to all of its vertices.
//                 // Effectively adding-up the face normals for each triangle that includes a said vertex in Normals[vertex.index]
//                 // note*: as this RenderCommand only handles quadrilaterals, this only sums the 1-or-2 face normals per Vertex (the normals of the 2 triangles which make up the quadrilateral)
//                 Normals[TriangleIndices[v]] += faceNormal;
//                 Normals[TriangleIndices[v + 1]] += faceNormal;
//                 Normals[TriangleIndices[v + 2]] += faceNormal;
//             }
//
//             // Normalize normals => ||a|| = 1
//             for (int j = 0; j < Normals.Length; j++)
//             {
//                 Normals[j] = Vector3.Normalize(Normals[j]);
//             }
//         }
//     }
// }