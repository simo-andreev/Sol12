#region Using

using System;
using System.Diagnostics;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics.Command;
using Emotion.Graphics.Objects;
using Solution12.Graphics.Data;

#endregion

namespace Solution12.Graphics.Command
{
    public class MeshBatchCommand : QuadBatch
    {
        public Vector3 Sun;

        public override void Process()
        {
            // Check if anything to map.
            if (BatchedTexturables.Count == 0) return;
            
            // Check if all are mapped.
            //if(_mappedTo == BatchedTexturables.Count * 4) return;

            // Create graphics objects, if missing.
            uint bufferLengthNeeded = (uint) (BatchedTexturables.Count * 4 * MeshVertexData.SizeInBytes);
            
            // When resizing (or creating) create a buffer twice as big as what's needed, but don't go over the max which can be used with the default IBOs.
            // The "fullness" check in PushSprite will ensure that the needed data isn't larger than what the default IBOs can fit.
            uint resizeAmount = Math.Min(bufferLengthNeeded * 2, (uint) (Engine.Renderer.MaxIndices * MeshVertexData.SizeInBytes));
            if (_vbo == null)
            {
                _vbo = new VertexBuffer(resizeAmount);
                _vao = new VertexArrayObject<MeshVertexData>(_vbo, IndexBuffer.QuadIbo);
            }

            // Check if a resize is needed.
            if (_vbo.Size < bufferLengthNeeded) _vbo.Upload(IntPtr.Zero, resizeAmount);

            // Open a mapper to the vbo.
            Span<MeshVertexData> mapper = _vbo.CreateMapper<MeshVertexData>(0, (int) bufferLengthNeeded);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int j = 0; j < BatchedTexturables.Count; j++)
            {
                RenderTileCommand x = (RenderTileCommand) BatchedTexturables[j];

                // Check if already mapped.
                //if (j * 4 < _mappedTo) continue;

                // If the sprite is processed it is also considered mapped.
                // This happens when updating a single sprite once the initial mapping has occured.
                //if (x.Processed)
                //{
                //    _mappedTo += 4;
                //    continue;
                //}

                if (_rebindTextures)
                {
                    TextureBindingUpdate(x.Texture.Pointer);
                }

                x.Process();

                // Set the texture id in vertices with the one the batch will bind.
                int tid = -1;
                if (x.Texture != null)
                {
                    uint tidLookingFor = x.Texture.Pointer;
                    for (int i = 0; i < _textureSlotUtilization; i++)
                    {
                        if (_textureBinding[i] == tidLookingFor) tid = i;
                    }

                    // Texture id should have been found.
                    Debug.Assert(tid != -1);
                }

                // Attach texture ids and map into the vbo.
                for (int i = 0; i < x.Vertices.Length; i++)
                {
                    x.Vertices[i].Tid = tid;
                    mapper[_mappedTo].Color = x.Vertices[i].Color;
                    mapper[_mappedTo].Vertex = x.Vertices[i].Vertex;
                    mapper[_mappedTo].UV = x.Vertices[i].UV;
                    mapper[_mappedTo].Tid = x.Vertices[i].Tid;
                    mapper[_mappedTo].Normal = x.Normals[i];
                    _mappedTo++;
                }
            }

            _vbo.FinishMapping();
            _mappedTo = 0;
        }
    }
}