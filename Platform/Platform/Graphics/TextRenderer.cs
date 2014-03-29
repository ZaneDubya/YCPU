using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Platform.Graphics
{
    class TextRenderer
    {
        private Dictionary<string, Texture2D> _renderered;
        private SpriteFont _font;
        GraphicsDevice _graphics;

        public TextRenderer(GraphicsDevice graphics, SpriteFont font)
        {
            _graphics = graphics;
            _font = font;
            _renderered = new Dictionary<string, Texture2D>();
        }

        public Texture2D RenderText(string text)
        {
            if (text == string.Empty)
                return null;
            Vector2 area = _font.MeasureString(text);
            string key = text + area.ToString();
            if (_renderered.ContainsKey(key))
                return _renderered[key];
            else
            {
                TextureBaker baker = new TextureBaker(_graphics, (int)area.X, (int)area.Y, TextureBaker.RenderState.Fill);
                var start = 0;
                var height = 0.0f;

                while (true)
                {
                    start = RenderLine(baker, start, (int)area.X, height, text);

                    if (start >= text.Length)
                    {
                        Texture2D texture = baker.GetTexture();
                        _renderered.Add(key, texture);
                        return texture;
                    }

                    height += _font.LineSpacing;
                }
            }
        }

        private byte MeasureCharacter(string text, int location)
        {
            var front = _font.MeasureString(text.Substring(0, location)).X;
            var end = _font.MeasureString(text.Substring(0, location + 1)).X;

            return (byte)(end - front);
        }

        private int RenderLine(SpriteBatch textureBaker, int start, int width, float height, string text)
        {
            var breakLocation = start;
            var lineLength = 0.0f;
            var row = (byte)(height / _font.LineSpacing);
            string tempText;

            //Starting from end of last line loop though the characters
            for (var iCount = start; iCount < text.Length; iCount++)
            {
                //Calculate the width of the current line
                lineLength += MeasureCharacter(text, iCount);

                //Current line is too long need to split it
                if (lineLength > width)
                {
                    if (breakLocation == start) {
                        //Have to split a word
                        //Render line and return start of new line
                        tempText = text.Substring(start, iCount - start);
                        textureBaker.DrawString(_font, tempText, new Vector2(0.0f, height), Color.White);
                        return iCount + 1;
                    } else {
                        //Have a character we can split on
                        //Render line and return start of new line
                        tempText = text.Substring(start, breakLocation - start);
                        textureBaker.DrawString(_font, tempText, new Vector2(0.0f, height), Color.White);
                        return breakLocation + 1;
                    }
                }

                //Handle characters that force/allow for breaks
                switch (text[iCount])
                {
                    //These characters force a line break
                    case '\r':
                    case '\n':
                        //Render line and return start of new line
                        tempText = text.Substring(start, iCount - start);
                        textureBaker.DrawString(_font, tempText, new Vector2(0.0f, height), Color.White);
                        return iCount + 1;
                    //These characters are good break locations
                    case '-':
                    case ' ':
                        breakLocation = iCount + 1;
                        break;
                }
            }

            //We hit the end of the text box render line and return
            //_textData.Length so RenderText knows to return
            tempText = text.Substring(start, text.Length - start);
            textureBaker.DrawString(_font, tempText, new Vector2(0.0f, height), Color.White);
            return tempText.Length;
        }
    }
}
