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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RoundLineManager roundLineManager;
        List<RoundLine> blueRoundLines = new List<RoundLine>();
        List<RoundLine> greenRoundLines = new List<RoundLine>();
        Matrix viewMatrix;
        Matrix projMatrix;
        float cameraX = 0;
        float cameraY = 0;
        float cameraZoom = 300;
        RoundDisc dude = new RoundDisc(0, 0);
        bool aButtonDown = false;
        int roundLineTechniqueIndex = 0;
        string[] roundLineTechniqueNames;


        public RoundLineDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            roundLineManager = new RoundLineManager();
            roundLineManager.Init(GraphicsDevice, Content);
            roundLineTechniqueNames = roundLineManager.TechniqueNames;

            float rho = 100;
            bool blue = false;
            for (float y = -2000; y <= 2000; y += 200)
            {
                for (float x = -1000; x <= 1000; x += 200)
                {
                    blue = !blue;
                    for (float deg = 0; deg < 360; deg += 10)
                    {
                        float theta = MathHelper.ToRadians(deg);
                        float x1 = rho * (float)Math.Cos(theta);
                        float y1 = rho * (float)Math.Sin(theta);

                        if (blue)
                            blueRoundLines.Add(new RoundLine(x, y, x + x1, y + y1));
                        else
                            greenRoundLines.Add(new RoundLine(x, y, x + x1, y + y1));
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
            float width = graphics.GraphicsDevice.Viewport.Width;
            float height = graphics.GraphicsDevice.Viewport.Height;
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
            projMatrix = Matrix.CreateScale(projScaleX, projScaleY, 0.0f);
            projMatrix.M43 = 0.5f;
        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (gamePadState.Buttons.A == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A))
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

            float leftX = gamePadState.ThumbSticks.Left.X;
            if (keyboardState.IsKeyDown(Keys.Left))
                leftX -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Right))
                leftX += 1.0f;

            float leftY = gamePadState.ThumbSticks.Left.Y;
            if (keyboardState.IsKeyDown(Keys.Up))
                leftY += 1.0f;
            if (keyboardState.IsKeyDown(Keys.Down))
                leftY -= 1.0f;

            float dx = leftX * 0.01f * cameraZoom;
            float dy = leftY * 0.01f * cameraZoom;

            bool zoomIn = gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Z);
            bool zoomOut = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.X);

            cameraX += dx;
            cameraY += dy;
            if (zoomIn)
                cameraZoom /= 0.995f;
            if (zoomOut)
                cameraZoom *= 0.995f;

            viewMatrix = Matrix.CreateTranslation(-cameraX, -cameraY, 0) * Matrix.CreateScale(1.0f / cameraZoom, 1.0f / cameraZoom, 1.0f);

            if (keyboardState.IsKeyDown(Keys.PageUp))
                roundLineManager.BlurThreshold *= 1.001f;
            if (keyboardState.IsKeyDown(Keys.PageDown))
                roundLineManager.BlurThreshold /= 1.001f;

            if (roundLineManager.BlurThreshold > 1)
                roundLineManager.BlurThreshold = 1;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            Matrix viewProjMatrix = viewMatrix * projMatrix;

            GraphicsDevice.Clear(Color.DarkGray);

            roundLineManager.NumLinesDrawn = 0;

            float lineRadius = 4;
            roundLineManager.BlurThreshold = roundLineManager.ComputeBlurThreshold(lineRadius, viewProjMatrix,
                GraphicsDevice.PresentationParameters.BackBufferWidth);

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            string curTechniqueName = roundLineTechniqueNames[roundLineTechniqueIndex];

            roundLineManager.Draw(blueRoundLines, lineRadius, Color.Blue, viewProjMatrix, time, curTechniqueName);
            roundLineManager.Draw(greenRoundLines, lineRadius, Color.Green, viewProjMatrix, time, curTechniqueName);

            dude.Pos = new Vector2(cameraX, cameraY);
            roundLineManager.Draw(dude, 8, Color.Red, viewProjMatrix, time, "Tubular");

            base.Draw(gameTime);
        }
    }
}
