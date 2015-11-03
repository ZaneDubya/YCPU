// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// A class to efficiently draw thick lines with rounded ends.
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
#endregion

namespace Ypsilon.Display.Vectors
{
    /// <summary>
    /// Main "game" type
    /// </summary>
    public class RoundLineDemo : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        VectorManager m_LineManager;
        List<VectorRoundLine> lines = new List<VectorRoundLine>();
        Matrix viewMatrix;
        Matrix projMatrix;
        float cameraX = 0;
        float cameraY = 0;
        float cameraZoom = 300;
        VectorDisc dude = new VectorDisc(0, 0);
        bool aButtonDown = false;
        int roundLineTechniqueIndex = 0;
        string[] roundLineTechniqueNames;


        public RoundLineDemo()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            m_Graphics.PreferredBackBufferWidth = 1280;
            m_Graphics.PreferredBackBufferHeight = 720;
        }

        protected override void LoadContent()
        {
            m_LineManager = new VectorManager();
            m_LineManager.Init(GraphicsDevice, Content);
            roundLineTechniqueNames = m_LineManager.TechniqueNames;

            for (float y = -2000; y <= 2000; y += 200)
            {
                for (float x = -2000; x <= 2000; x += 200)
                {
                    for (float deg = 0; deg < 360; deg += 10)
                    {
                        float theta = MathHelper.ToRadians(deg);
                        float rho = 100;
                        float x1 = rho * (float)Math.Cos(theta);
                        float y1 = rho * (float)Math.Sin(theta);
                        
                        lines.Add(new VectorRoundLine(x, y, x + x1, y + y1));
                    }
                }
            }

            Create2DProjectionMatrix();
        }


        /// <summary>
        /// Create a simple 2D projection matrix
        /// </summary>
        public void Create2DProjectionMatrix()
        {
            // Projection matrix ignores Z and just squishes X or Y to balance the upcoming viewport stretch
            float projScaleX;
            float projScaleY;
            float width = m_Graphics.GraphicsDevice.Viewport.Width;
            float height = m_Graphics.GraphicsDevice.Viewport.Height;
            if (width > height)
            {
                // Wide window
                projScaleX = height / width;
                projScaleY = 1.0f;
            }
            else
            {
                // Tall window
                projScaleX = 1.0f;
                projScaleY = width / height;
            }

            // projMatrix = Matrix.CreatePerspective(projScaleX, projScaleY, 1.0f, 100f);

            projMatrix = Matrix.CreateScale(projScaleX, projScaleY, 0.0f);
            projMatrix.M43 = 0.5f;
        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (!aButtonDown)
                {
                    aButtonDown = true;
                    roundLineTechniqueIndex++;
                    if (roundLineTechniqueIndex >= roundLineTechniqueNames.Length)
                        roundLineTechniqueIndex = 0;
                }
            }
            else
            {
                aButtonDown = false;
            }

            float leftX = 0;
            if (keyboardState.IsKeyDown(Keys.Left))
                leftX -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Right))
                leftX += 1.0f;

            float leftY = 0;
            if (keyboardState.IsKeyDown(Keys.Up))
                leftY += 1.0f;
            if (keyboardState.IsKeyDown(Keys.Down))
                leftY -= 1.0f;

            float dx = leftX * 0.01f * cameraZoom;
            float dy = leftY * 0.01f * cameraZoom;

            bool zoomIn = keyboardState.IsKeyDown(Keys.Z);
            bool zoomOut = keyboardState.IsKeyDown(Keys.X);

            cameraX += dx;
            cameraY += dy;
            if (zoomIn)
                cameraZoom /= 0.995f;
            if (zoomOut)
                cameraZoom *= 0.995f;

            viewMatrix = Matrix.CreateTranslation(-cameraX, -cameraY, 0) * Matrix.CreateScale(1.0f / cameraZoom, 1.0f / cameraZoom, 1.0f);

            if (keyboardState.IsKeyDown(Keys.PageUp))
                m_LineManager.BlurThreshold += 0.01f;
            if (keyboardState.IsKeyDown(Keys.PageDown))
                m_LineManager.BlurThreshold -= 0.01f;

            if (m_LineManager.BlurThreshold > 1)
                m_LineManager.BlurThreshold = 1;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Matrix viewProjMatrix = viewMatrix * projMatrix;

            GraphicsDevice.Clear(Color.DarkGray);

            m_LineManager.NumLinesDrawn = 0;

            float lineRadius = 1;
            m_LineManager.BlurThreshold = m_LineManager.ComputeBlurThreshold(lineRadius, viewProjMatrix, 
                GraphicsDevice.PresentationParameters.BackBufferWidth);

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            string curTechniqueName = roundLineTechniqueNames[roundLineTechniqueIndex];

            m_LineManager.Draw(lines, lineRadius, Color.Blue, viewProjMatrix, time, curTechniqueName);

            dude.Pos = new Vector2(cameraX, cameraY);
            m_LineManager.Draw(dude, 8, Color.Red, viewProjMatrix, time, "Standard"); // changed from Tubular

            base.Draw(gameTime);
        }
    }
}
