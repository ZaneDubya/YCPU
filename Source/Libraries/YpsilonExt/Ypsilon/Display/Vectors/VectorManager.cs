// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

namespace Ypsilon.Display.Vectors
{
    /// <summary>
    /// Class to handle drawing a list of RoundLines.
    /// </summary>
    class VectorManager
    {
        private GraphicsDevice m_Graphics;

        private Effect m_Effect;
        private EffectParameter m_ViewProjMatrixParameter;
        private EffectParameter m_InstanceDataParameter;
        private EffectParameter m_TimeParameter;
        private EffectParameter m_LineRadiusParameter;
        private EffectParameter m_LineColorParameter;
        private EffectParameter m_BlurThresholdParameter;

        private VertexBuffer m_Vertices;
        private IndexBuffer m_Indices;

        private int numInstances;
        private int numVertices;
        private int numIndices;
        private int numPrimitivesPerInstance;
        private int numPrimitives;
        private float[] translationData;

        public int NumLinesDrawn;
        public float BlurThreshold = 0.97f;

        public void Init(GraphicsDevice device, ContentManager content)
        {
            m_Graphics = device;

            m_Effect = content.Load<Effect>("RoundLine");
            m_ViewProjMatrixParameter = m_Effect.Parameters["viewProj"];
            m_InstanceDataParameter = m_Effect.Parameters["instanceData"];
            m_TimeParameter = m_Effect.Parameters["time"];
            m_LineRadiusParameter = m_Effect.Parameters["lineRadius"];
            m_LineColorParameter = m_Effect.Parameters["lineColor"];
            m_BlurThresholdParameter = m_Effect.Parameters["blurThreshold"];

            CreateRoundLineMesh();
        }

        public string[] TechniqueNames
        {
            get
            {
                string[] names = new string[m_Effect.Techniques.Count];
                int index = 0;
                foreach (EffectTechnique technique in m_Effect.Techniques)
                    names[index++] = technique.Name;
                return names;
            }
        }

        /// <summary>
        /// Create a mesh for a VectorRoundLine.
        /// </summary>
        /// <remarks>
        /// The VectorRoundLine mesh has 3 sections:
        /// 1.  Two quads, from 0 to 1 (left to right)
        /// 2.  A half-disc, off the left side of the quad
        /// 3.  A half-disc, off the right side of the quad
        ///
        /// The X and Y coordinates of the "normal" encode the rho (length) and theta (angle) of each vertex
        /// The "texture" encodes whether to scale and translate the vertex horizontally by length and radius
        /// </remarks>
        private void CreateRoundLineMesh()
        {
            const int primsPerCap = 2; // A higher primsPerCap produces rounder endcaps at the cost of more vertices
            const int verticesPerCap = primsPerCap * 2 + 2;
            const int primsPerCore = 4;
            const int verticesPerCore = 8;

            numInstances = 200;
            numVertices = (verticesPerCore + verticesPerCap + verticesPerCap) * numInstances;
            numPrimitivesPerInstance = primsPerCore + primsPerCap + primsPerCap;
            numPrimitives = numPrimitivesPerInstance * numInstances;
            numIndices = 3 * numPrimitives;
            short[] indices = new short[numIndices];
            VectorVertex[] tri = new VectorVertex[numVertices];
            translationData = new float[numInstances * 4]; // Used in Draw()

            int iv = 0;
            int ii = 0;
            int iVertex;
            int iIndex;
            for (int instance = 0; instance < numInstances; instance++)
            {
                // core vertices
                const float pi2 = MathHelper.PiOver2;
                const float threePi2 = 3 * pi2;
                iVertex = iv;
                tri[iv++] = new VectorVertex(new Vector3(0.0f, -1.0f, 0), new Vector2(1, threePi2), new Vector2(0, 0), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, -1.0f, 0), new Vector2(1, threePi2), new Vector2(0, 1), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 0.0f, 0), new Vector2(0, threePi2), new Vector2(0, 1), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 0.0f, 0), new Vector2(0, threePi2), new Vector2(0, 0), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 0.0f, 0), new Vector2(0, pi2), new Vector2(0, 1), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 0.0f, 0), new Vector2(0, pi2), new Vector2(0, 0), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 1.0f, 0), new Vector2(1, pi2), new Vector2(0, 1), instance);
                tri[iv++] = new VectorVertex(new Vector3(0.0f, 1.0f, 0), new Vector2(1, pi2), new Vector2(0, 0), instance);

                // core indices
                indices[ii++] = (short)(iVertex + 0);
                indices[ii++] = (short)(iVertex + 1);
                indices[ii++] = (short)(iVertex + 2);
                indices[ii++] = (short)(iVertex + 2);
                indices[ii++] = (short)(iVertex + 3);
                indices[ii++] = (short)(iVertex + 0);

                indices[ii++] = (short)(iVertex + 4);
                indices[ii++] = (short)(iVertex + 6);
                indices[ii++] = (short)(iVertex + 5);
                indices[ii++] = (short)(iVertex + 6);
                indices[ii++] = (short)(iVertex + 7);
                indices[ii++] = (short)(iVertex + 5);

                // left halfdisc
                iVertex = iv;
                iIndex = ii;
                for (int i = 0; i < primsPerCap + 1; i++)
                {
                    float deltaTheta = MathHelper.Pi / primsPerCap;
                    float theta0 = MathHelper.PiOver2 + i * deltaTheta;
                    float theta1 = theta0 + deltaTheta / 2;
                    // even-numbered indices are at the center of the halfdisc
                    tri[iVertex + 0] = new VectorVertex(new Vector3(0, 0, 0), new Vector2(0, theta1), new Vector2(0, 0), instance);

                    // odd-numbered indices are at the perimeter of the halfdisc
                    float x = (float)Math.Cos(theta0);
                    float y = (float)Math.Sin(theta0);
                    tri[iVertex + 1] = new VectorVertex(new Vector3(x, y, 0), new Vector2(1, theta0), new Vector2(1, 0), instance);

                    if (i < primsPerCap)
                    {
                        // indices follow this pattern: (0, 1, 3), (2, 3, 5), (4, 5, 7), ...
                        indices[iIndex + 0] = (short)(iVertex + 0);
                        indices[iIndex + 1] = (short)(iVertex + 1);
                        indices[iIndex + 2] = (short)(iVertex + 3);
                        iIndex += 3;
                        ii += 3;
                    }
                    iVertex += 2;
                    iv += 2;
                }

                // right halfdisc
                for (int i = 0; i < primsPerCap + 1; i++)
                {
                    float deltaTheta = MathHelper.Pi / primsPerCap;
                    float theta0 = 3 * MathHelper.PiOver2 + i * deltaTheta;
                    float theta1 = theta0 + deltaTheta / 2;
                    float theta2 = theta0 + deltaTheta;
                    // even-numbered indices are at the center of the halfdisc
                    tri[iVertex + 0] = new VectorVertex(new Vector3(0, 0, 0), new Vector2(0, theta1), new Vector2(0, 1), instance);

                    // odd-numbered indices are at the perimeter of the halfdisc
                    float x = (float)Math.Cos(theta0);
                    float y = (float)Math.Sin(theta0);
                    tri[iVertex + 1] = new VectorVertex(new Vector3(x, y, 0), new Vector2(1, theta0), new Vector2(1, 1), instance);

                    if (i < primsPerCap)
                    {
                        // indices follow this pattern: (0, 1, 3), (2, 3, 5), (4, 5, 7), ...
                        indices[iIndex + 0] = (short)(iVertex + 0);
                        indices[iIndex + 1] = (short)(iVertex + 1);
                        indices[iIndex + 2] = (short)(iVertex + 3);
                        iIndex += 3;
                        ii += 3;
                    }
                    iVertex += 2;
                    iv += 2;
                }
            }

            m_Vertices = new VertexBuffer(m_Graphics, typeof(VectorVertex), numVertices, BufferUsage.None);
            m_Vertices.SetData<VectorVertex>(tri);

            m_Indices = new IndexBuffer(m_Graphics, IndexElementSize.SixteenBits, numIndices, BufferUsage.None);
            m_Indices.SetData<short>(indices);
        }

        /// <summary>
        /// Compute a reasonable "BlurThreshold" value to use when drawing RoundLines.
        /// See how wide lines of the specified radius will be (in pixels) when drawn
        /// to the back buffer.  Then apply an empirically-determined mapping to get
        /// a good BlurThreshold for such lines.
        /// </summary>
        public float ComputeBlurThreshold(float lineRadius, Matrix viewProjMatrix, float viewportWidth)
        {
            Vector4 lineRadiusTestBase = new Vector4(0, 0, 0, 1);
            Vector4 lineRadiusTest = new Vector4(lineRadius, 0, 0, 1);
            Vector4 delta = lineRadiusTest - lineRadiusTestBase;
            Vector4 output = Vector4.Transform(delta, viewProjMatrix);
            output.X *= viewportWidth;

            double newBlur = 0.125 * Math.Log(output.X) + 0.4;

            return MathHelper.Clamp((float)newBlur, 0.5f, 0.99f);
        }

        /// <summary>
        /// Draw a single RoundLine.  Usually you want to draw a list of RoundLines
        /// at a time instead for better performance.
        /// </summary>
        public void Draw(VectorRoundLine roundLine, float lineRadius, Color lineColor, Matrix viewProjMatrix,
            float time, string techniqueName)
        {
            m_Graphics.SetVertexBuffer(m_Vertices);
            m_Graphics.Indices = m_Indices;

            m_ViewProjMatrixParameter.SetValue(viewProjMatrix);
            m_TimeParameter.SetValue(time);
            m_LineColorParameter.SetValue(lineColor.ToVector4());
            m_LineRadiusParameter.SetValue(lineRadius);
            m_BlurThresholdParameter.SetValue(BlurThreshold);

            int iData = 0;
            translationData[iData++] = roundLine.P0.X;
            translationData[iData++] = roundLine.P0.Y;
            translationData[iData++] = roundLine.Length;
            translationData[iData++] = roundLine.Angle;
            m_InstanceDataParameter.SetValue(translationData);

            if (techniqueName == null)
                m_Effect.CurrentTechnique = m_Effect.Techniques[0];
            else
                m_Effect.CurrentTechnique = m_Effect.Techniques[techniqueName];
            EffectPass pass = m_Effect.CurrentTechnique.Passes[0];
            pass.Apply();

            int numInstancesThisDraw = 1;
            m_Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitivesPerInstance * numInstancesThisDraw);
            NumLinesDrawn += numInstancesThisDraw;
        }

        /// <summary>
        /// Draw a list of Lines.
        /// </summary>
        public void Draw(List<VectorRoundLine> roundLines, float lineRadius, Color lineColor, Matrix viewProjMatrix,
            float time, string techniqueName)
        {
            m_Graphics.SetVertexBuffer(m_Vertices);
            m_Graphics.Indices = m_Indices;

            m_ViewProjMatrixParameter.SetValue(viewProjMatrix);
            m_TimeParameter.SetValue(time);
            m_LineColorParameter.SetValue(lineColor.ToVector4());
            m_LineRadiusParameter.SetValue(lineRadius);
            m_BlurThresholdParameter.SetValue(BlurThreshold);

            if (techniqueName == null)
                m_Effect.CurrentTechnique = m_Effect.Techniques[0];
            else
                m_Effect.CurrentTechnique = m_Effect.Techniques[techniqueName];
            EffectPass pass = m_Effect.CurrentTechnique.Passes[0];
            pass.Apply();

            int iData = 0;
            int numInstancesThisDraw = 0;
            foreach (VectorRoundLine roundLine in roundLines)
            {
                translationData[iData++] = roundLine.P0.X;
                translationData[iData++] = roundLine.P0.Y;
                translationData[iData++] = roundLine.Length;
                translationData[iData++] = roundLine.Angle;
                numInstancesThisDraw++;

                if (numInstancesThisDraw == numInstances)
                {
                    m_InstanceDataParameter.SetValue(translationData);
                    pass.Apply();
                    m_Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitivesPerInstance * numInstancesThisDraw);
                    NumLinesDrawn += numInstancesThisDraw;
                    numInstancesThisDraw = 0;
                    iData = 0;
                }
            }
            if (numInstancesThisDraw > 0)
            {
                m_InstanceDataParameter.SetValue(translationData);
                pass.Apply();
                m_Graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitivesPerInstance * numInstancesThisDraw);
                NumLinesDrawn += numInstancesThisDraw;
            }
        }
    }
}
