using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Avoid_Death
{
    public class Structure
    {
        public bool active = false;
        public bool indestructable = false;
        public bool collideWithPlayer = false;
        public int width = 256;
        public int height = 512;
        public int curFrame = 0;
        public int frames = 3;
        public byte type;
        public int life;
        public int maxLife;
        public int whoAmI;
        public byte debriType;
        public int maxDebri;
        public Vector2 position = new Vector2(0, 0);
        public Rectangle frame = new Rectangle(0, 0, 512, 256);

        public Structure()
        {

        }

        public void BlowUp()
        {
            Dust.Explosion(5, this.position);
            for (int i = 0; i < this.maxDebri; i++)
            {
                int x = Main.random.Next(-this.width / 2, this.width / 2);
                int y = Main.random.Next(-this.height / 2, this.height / 2);
                Dust d = Dust.newDust(this.debriType, new Vector2(x, y) + this.position);
                if (d != null)
                {
                    d.velocity = new Vector2(Main.random.Next(-5, 5), -Main.random.Next(5));
                }
            }
        }

        public void UpdateFrame()
        {
            this.curFrame = (int) (this.frames - (((double) this.life / (double) this.maxLife) * this.frames));
            if (this.life <= 0)
            {
                this.active = false;
                this.BlowUp();
            }
            else
            {
                this.frame = new Rectangle(this.curFrame * this.width, 0, this.width, this.height);
            }
        }

        public static Structure newStructure(byte type, int pos)
        {
            Structure s = new Structure();
            for (int i = 0; i < Main.structure.Length; i++)
            {
                if (!Main.structure[i].active)
                {
                    Main.structure[i] = s;
                    s.whoAmI = i;
                    s.active = true;
                    s.type = type;
                    Defaults.setDefaults(s);
                    s.position.X = pos;
                    s.position.Y = Main.worldBoundsDown - Main.groundHeight - (s.height / 2);
                    break;
                }
            }
            if (s.active)
            {
                return s;
            }
            return null;
        }
    }
}
