using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avoid_Death
{
    public class Button
    {
        public int buttonID;
        public int goToMenu;
        public Color overlayColor = Color.White;
        public string text;
        public bool active;
        public bool selected;
        public bool visible;
        public Rectangle rect;

        public Button(int x = 0, int y = 0, string text = "", int gotomenu = 0)
        {
            this.active = true;
            this.visible = true;
            this.rect = new Rectangle(x, y, 256, 64);
            this.text = text;
            this.goToMenu = gotomenu;
        }

        public void tryClick()
        {
            if (this.selected)
            {
                if (this.goToMenu == -1)
                {
                    Main.pendingGameExit = true;
                }
                else if (this.goToMenu == -2)
                {
                    Main.pendingGameStart = true;
                }
                else if (this.goToMenu == -3)
                {
                    Main.usingGamepad = true;
                }
                else if (this.goToMenu == -4)
                {
                    Main.usingGamepad = false;
                }
                else if (this.goToMenu == -5)
                {
                    Main.endGame();
                    
                }
                else
                {
                    Main.curMenu = this.goToMenu;
                }
            }
        }

        public bool checkSelected(Rectangle test)
        {
            bool result = test.Intersects(this.rect);
            if (result)
            {
                Main.buttonSelected = this.buttonID;
            }
            this.selected = result;
            return result;
        }

        public void draw()
        {
            if (this.visible)
            {
                int textx = this.rect.X - (int) Main.font[2].MeasureString(this.text).X / 2;
                int texty = this.rect.Y - (int) Main.font[2].MeasureString(this.text).Y / 2;
                Main.spriteBatch.Draw(Main.buttonTexture, this.rect, null, this.overlayColor, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.font[2], this.text, new Vector2(textx, texty), Color.Black, 0f, new Vector2(-128, -32), 1f, SpriteEffects.None, 0f);
            }
            if (this.selected)
            {
                if (this.overlayColor.R - 6 > 0)
                    this.overlayColor.R -= 6;
                if (this.overlayColor.G - 6 > 191)
                    this.overlayColor.G -= 6;
                if (this.overlayColor.B - 6 > 255)
                    this.overlayColor.B -= 6;
            }
            else
            {
                if (this.overlayColor.R + 6 < 255)
                    this.overlayColor.R += 6;
                if (this.overlayColor.G + 6 < 255)
                    this.overlayColor.G += 6;
                if (this.overlayColor.B + 6 < 255)
                    this.overlayColor.B += 6;
            }
        }
    }
}
