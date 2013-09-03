﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics.OpenTK20
{
	/// <summary>
	/// Functionality for all OpenGL based circular buffers to render small batches quickly.
	/// </summary>
	public class OpenTK20CircularBuffer : CircularBuffer
	{
		public OpenTK20CircularBuffer(Device device, ShaderWithFormat shader, BlendMode blendMode,
			VerticesMode drawMode = VerticesMode.Triangles)
			: base(device, shader, blendMode, drawMode) {}

		protected override void CreateNative()
		{
			glDevice = (OpenTK20Device)device;
			nativeVertexBuffer = glDevice.CreateVertexBuffer(maxNumberOfVertices * vertexSize,
				OpenTK20BufferMode.Stream);
			if (nativeVertexBuffer == OpenTK20Device.InvalidHandle)
				throw new UnableToCreateOpenGLGeometry();
			if (UsesIndexBuffer)
				nativeIndexBuffer = glDevice.CreateIndexBuffer(maxNumberOfIndices * indexSize,
					OpenTK20BufferMode.Stream);
		}

		private class UnableToCreateOpenGLGeometry : Exception {}

		private OpenTK20Device glDevice;
		private int nativeVertexBuffer;
		private int nativeIndexBuffer;

		protected override void AddDataNative<VertexType>(Chunk textureChunk, VertexType[] vertexData,
			short[] indices, int numberOfVertices, int numberOfIndices)
		{
			glDevice.BindVertexBuffer(nativeVertexBuffer);
			glDevice.LoadVertexData(totalVertexOffsetInBytes, vertexData, vertexSize * numberOfVertices);
			if (!UsesIndexBuffer)
				return;
			if (indices == null)
				indices = ComputeIndices(textureChunk.NumberOfVertices, numberOfVertices);
			else if (textureChunk.FirstVertexOffsetInBytes > 0)
				indices = RemapIndices(indices, numberOfIndices);
			glDevice.BindIndexBuffer(nativeIndexBuffer);
			glDevice.LoadIndices(totalIndexOffsetInBytes, indices, indexSize * numberOfIndices);
		}

		public override void DrawAllTextureChunks()
		{
			glDevice.BindVertexBuffer(nativeVertexBuffer);
			if (UsesIndexBuffer)
				glDevice.BindIndexBuffer(nativeIndexBuffer);
			if (UsesTexturing)
				glDevice.EnableTexturing();
			else
				glDevice.DisableTexturing();
			base.DrawAllTextureChunks();
		}

		protected override void DrawChunk(Chunk chunk)
		{
			shader.BindVertexDeclaration();
			if (UsesIndexBuffer)
			{
				if (chunk.Texture != null)
					shader.SetDiffuseTexture(chunk.Texture);
				glDevice.DrawTriangles(chunk.FirstIndexOffsetInBytes, chunk.NumberOfIndices);
			}
			else
				glDevice.DrawLines(chunk.FirstVertexOffsetInBytes / vertexSize, chunk.NumberOfVertices);
		}

		protected override void DisposeNative()
		{
			glDevice.DeleteBuffer(nativeVertexBuffer);
			if (UsesIndexBuffer)
				glDevice.DeleteBuffer(nativeIndexBuffer);
		}
	}
}