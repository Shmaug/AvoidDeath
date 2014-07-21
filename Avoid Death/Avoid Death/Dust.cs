using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avoid_Death
{
    public class Dust
    {
        public int width = 0;
        public int height = 0;
        public int whoAmI = -1;
        public byte type = 0;
        public int lifeTime = 0;
        public int maxLifeTime = 0;
        public bool active = false;
        public bool hasGravity = false;
        public bool collideWithGround = false;
        public bool fades = true;
        public bool isStatic = false;
        public float rotation = 0f;
        public float rotationVel = 0f;
        public float alpha = 1;
        public float scale = 1f;
        public float growRate = 0;
        public Vector2 startPos = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;

        public Dust()
        {

        }

        public static int numDust()
        {
            int num = 0;
            foreach (Dust d in Main.dust)
            {
                if (d.active)
                {
                    num++;
                }
            }
            return num;
        }

        public static void ClearDust()
        {
            foreach (Dust d in Main.dust)
            {
                d.active = false;
            }
        }

        public void Update()
        {
            if (this.active)
            {
                this.scale += this.growRate;
                if (!this.isStatic)
                {
                    if (this.hasGravity && this.velocity.Y < 10) this.velocity.Y += 0.5f;
                    this.rotation += this.rotationVel;
                    this.position += this.velocity;
                    if (this.collideWithGround && this.position.Y + (this.height / 2) > Main.worldBoundsDown - Main.groundHeight)
                    {
                        if (Math.Abs(this.velocity.X) < 0.2f)
                        {
                            this.velocity.X = 0f;
                        }
                        else
                        {
                            if (this.velocity.X > 0)
                            {
                                this.velocity.X -= 0.1f;
                            }
                            else if (this.velocity.X < 0)
                            {
                                this.velocity.X += 0.1f;
                            }
                            this.rotation += MathHelper.ToRadians(this.velocity.X);
                        }
                        this.velocity.Y = 0f;
                        this.position.Y = Main.worldBoundsDown - Main.groundHeight - (this.height / 2);
                    }
                }

                if (this.fades) this.alpha = (float) this.lifeTime / (float) this.maxLifeTime;

                this.lifeTime--;
                if (this.lifeTime <= 0) this.Die();
            }
        }

        public void Die()
        {
            this.active = false;
        }

        public static void UpdateDust()
        {
            foreach (Dust d in Main.dust)
            {
                if (d.active)
                {
                    d.Update();
                }
            }
        }

        public static Dust newDust(byte type, Vector2 pos)
        {
            Dust d = new Dust();
            for (int i = 0; i < Main.dust.Length; i++)
            {
                if (!Main.dust[i].active)
                {
                    Main.dust[i] = d;
                    d.type = type;
                    d.whoAmI = i;
                    d.active = true;
                    d.position = pos;
                    Defaults.setDefaults(d);
                    d.startPos = pos;
                    break;
                }
            }
            if (d.active)
            {
                return d;
            }
            return null;
        }

        public static void Explosion(int increment, Vector2 pos, int maxVelocity = 5, byte type = 1, bool glow = true)
        {
            if (glow)
            {
                Dust e = Dust.newDust(type, pos);
                if (e != null)
                {
                    e.isStatic = true;
                    e.growRate = 0.3f;
                }
            }
            for (int i = 0; i < 360; i += increment)
            {
                Dust d = Dust.newDust(type, pos);
                if (d != null)
                {
                    int rand = Main.random.Next(maxVelocity);
                    int rand2 = Main.random.Next(maxVelocity);
                    d.velocity = new Vector2((float) Math.Cos(MathHelper.ToRadians(i)) * rand, (float) Math.Sin(MathHelper.ToRadians(i)) * rand2);
                }
            }
        }
    }
}
