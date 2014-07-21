using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avoid_Death
{
    public class Projectile
    {
        public int width = 0;
        public int height = 0;
        public int whoAmI = -1;
        public byte type = 0;
        public int damage = 0;
        public int lifeTime = 0;
        public int lastDust = 0;
        public int dustRate = 0;
        public byte dustType = 0;
        public bool hasDust = false;
        public bool hostile = false;
        public bool active = false;
        public bool selfPropelled = false;
        public bool menuMissile = false;
        public bool explosive = false;
        public bool charging = false;
        public float rotation = 0f;
        public float rotationVel = 0f;
        public float speed = 0f;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;

        public Projectile()
        {

        }

        public static void ClearProjectiles()
        {
            foreach (Projectile p in Main.projectile)
            {
                p.active = false;
            }
        }

        public static int numProjectiles()
        {
            int num = 0;
            foreach (Projectile p in Main.projectile)
            {
                if (p.active)
                {
                    num++;
                }
            }
            return num;
        }

        public void Update()
        {
            if (this.lastDust > 0) this.lastDust--;
            if (this.lastDust <= 0 && this.hasDust)
            {
                byte t = this.dustType;
                Vector2 pos = new Vector2((float) Math.Cos(this.rotation) * (this.width / 2), (float) Math.Sin(this.rotation) * (this.height / 2));
                Dust d = Dust.newDust(t, this.position);
                if (d != null)
                {
                    d.rotationVel = MathHelper.ToRadians(Main.random.Next(0, 10));
                    d.velocity.X = (float) Math.Cos(this.rotation) * Main.random.Next(3);
                    d.velocity.Y = (float) Math.Sin(this.rotation) * Main.random.Next(3);
                    if (Main.random.NextDouble() > 0.5) d.rotationVel *= -1;
                }
            }
            if (this.hostile)
            {
                if (this.type == 1)
                {
                    Vector2 closest = new Vector2(this.position.X + (float) Math.Cos(this.rotation) * -2, this.position.Y + (float) Math.Sin(this.rotation) * -2);
                    for (int i = 0; i < Main.player.Length; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead)
                        {
                            if (Vector2.Distance(this.position, Main.player[i].position) < Vector2.Distance(this.position, closest))
                            {
                                closest = Main.player[i].position;
                            }
                        }
                    }
                    float angle = (float) Math.Atan2(this.position.Y - closest.Y, this.position.X - closest.X);

                    this.rotation = angle;
                }
            }
            else
            {
                if (this.type == 1)
                {
                    int dist = 10000;
                    Vector2 closest = new Vector2(this.position.X + (float) Math.Cos(this.rotation) * -2, this.position.Y + (float) Math.Sin(this.rotation) * -2);
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].active && !Main.npc[i].dead)
                        {
                            if (Vector2.Distance(this.position, Main.npc[i].position) < dist)
                            {
                                closest = Main.npc[i].position;
                                dist = (int) Vector2.Distance(this.position, closest);
                            }
                        }
                    }
                    float angle = (float) Math.Atan2(this.position.Y - closest.Y, this.position.X - closest.X);

                    this.rotation = angle;
                }
            }
            if (this.selfPropelled)
            {
                this.velocity.X = (float) Math.Cos(this.rotation) * -this.speed;
                this.velocity.Y = (float) Math.Sin(this.rotation) * -this.speed;
            }
            this.rotation += this.rotationVel;
            this.position += this.velocity;
            bool dead = false;
            if (this.position.X + this.width < Main.worldBoundsLeft) dead = true;
            if (this.position.X > Main.worldBoundsRight) dead = true;
            if (!this.menuMissile)
            {
                if (this.position.Y + this.height < 0) dead = true;
            }
            else
            {
                if (this.position.Y > Main.screenHeight - 64) dead = true;
                if (this.position.X > Main.screenWidth) dead = true;
            }
            if (this.position.Y > Main.worldBoundsDown - Main.groundHeight) dead = true;

            Rectangle tRect = new Rectangle((int) (this.position.X - (Math.Cos(this.rotation) * this.width)), (int) (this.position.Y - (Math.Sin(this.rotation) * this.width)), 2, 2);
            if (this.hostile)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        Rectangle pRect = new Rectangle((int) (Main.player[i].position.X - Main.player[i].width / 2), (int) (Main.player[i].position.Y - Main.player[i].height / 2), Main.player[i].width, Main.player[i].height);
                        if (tRect.Intersects(pRect))
                        {
                            Main.player[i].life -= this.damage;
                            dead = true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dead)
                    {
                        Rectangle pRect = new Rectangle((int) (Main.npc[i].position.X - Main.npc[i].width / 2), (int) (Main.npc[i].position.Y - Main.npc[i].height / 2), Main.npc[i].width, Main.npc[i].height);
                        if (tRect.Intersects(pRect))
                        {
                            if (!this.explosive)
                            {
                                Main.npc[i].life -= this.damage;
                            }
                            dead = true;
                        }
                    }
                }
            }
            for (int i = 0; i < Main.structure.Length; i++)
            {
                if (Main.structure[i].active && !Main.structure[i].indestructable)
                {
                    Rectangle sRect = new Rectangle((int) Main.structure[i].position.X - (Main.structure[i].width / 2), (int) Main.structure[i].position.Y - (Main.structure[i].height / 2), Main.structure[i].width, Main.structure[i].height);
                    if (tRect.Intersects(sRect))
                    {
                        Main.structure[i].life -= this.damage;
                        Main.structure[i].UpdateFrame();
                        dead = true;
                    }
                }
            }

            if (dead == true)
            {
                if (this.explosive)
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].active && !Main.npc[i].dead)
                        {
                            if (Vector2.Distance(this.position, Main.npc[i].position) <= 128)
                            {
                                Main.npc[i].life -= this.damage;
                                Main.npc[i].velocity += this.velocity / 5;
                            }
                        }
                    }

                    for (int i = 0; i < Main.player.Length; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead)
                        {
                            if (Vector2.Distance(this.position, Main.player[i].position) <= 128)
                            {
                                Main.player[i].life -= this.damage;
                                Main.player[i].velocity += this.velocity / 3;
                            }
                        }
                    }
                }
                this.Die();
            }

            this.lifeTime--;
            if (this.lifeTime <= 0) this.Die();
        }

        public void Die()
        {
            if (this.explosive)
            {
                if (this.type == 3)
                {
                    Dust.Explosion(10, this.position, 5, 3, true);
                    Main.explosionSound[0].Play();
                }
                else
                {
                    Dust.Explosion(10, this.position);
                    Main.explosionSound[0].Play();
                }
                if (Main.inGame)
                {
                    float fac = 60f;
                    float dist = Vector2.Distance(this.position, Main.player[Main.myPlayer].position);
                    if (dist > 1000f)
                    {
                        dist = 1000f;
                    }
                    fac = 1000f - dist;
                    fac /= 40;
                    Main.camShakeFactor = (int) (fac);
                }
            }
            this.active = false;
        }

        public static void UpdateProjectiles()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.active)
                {
                    p.Update();
                }
            }
        }

        public static Projectile newProjectile(byte type, Vector2 pos)
        {
            Projectile p = new Projectile();
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (!Main.projectile[i].active)
                {
                    Main.projectile[i] = p;
                    p.type = type;
                    p.whoAmI = i;
                    p.active = true;
                    p.position = pos;
                    Defaults.setDefaults(p);
                    break;
                }
            }
            if (p.active)
            {
                return p;
            }
            return null;
        }
    }
}
