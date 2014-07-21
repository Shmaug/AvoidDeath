using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Avoid_Death
{
    public class Player
    {
        public int width = 0;
        public int height = 0;
        public int whoAmI = -1;
        public int controlX = 0;
        public int controlY = 0;
        public int rotorSize = 128;
        public int rotorSizeIncrement = 60;
        public int maxLife = 0;
        public int life = 0;
        public bool active = false;
        public bool onGround = false;
        public bool dead = false;
        public bool shooting = false;
        public bool engineOn = false;
        public float moveSpeed = 0.1f;
        public float rotation = 0f;
        public float rotationVel = 0f;
        public float tailRotorRotation = 0f;
        public float pointerRot = 0f;
        public float pointerDist = 0f;
        public float pointerDrawRot = 0f;
        public float fuel = 100f;
        public float maxFuel = 100f;
        public Weapon[] weapon = new Weapon[10];
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Vector2 pointer = Vector2.Zero;

        public Player()
        {

        }

        public void Update()
        {
            if (this.life <= 0)
            {
                this.life = 0;
                this.dead = true;
            }
            this.pointerDrawRot = 0f;
            if (this.shooting)
            {
                this.pointerDrawRot = MathHelper.PiOver2;
            }
            this.rotorSize += this.rotorSizeIncrement;
            if (this.rotorSize < 0 || this.rotorSize > 128) this.rotorSizeIncrement *= -1;
            this.tailRotorRotation += MathHelper.ToRadians(12f); ;
            for (int i = 0; i < this.weapon.Length; i++)
            {
                if (this.weapon[i].active)
                {
                    if (Main.usingGamepad)
                    {
                        this.pointer.X = (this.position.X - Main.screenPosition.X) + ((float) Math.Cos(this.pointerRot) * -this.pointerDist);
                        this.pointer.Y = (this.position.Y - Main.screenPosition.Y) + ((float) Math.Sin(this.pointerRot) * -this.pointerDist);
                    }
                    else
                    {
                        this.pointer.X = Main.mouseState.X;
                        this.pointer.Y = Main.mouseState.Y;
                    }

                    this.weapon[i].Update(Main.screenPosition + this.pointer);
                    if (this.shooting)
                    {
                        this.weapon[i].Shoot();
                    }
                }
            }
            if (!this.dead)
            {
                if (this.engineOn)
                {
                    this.fuel -= 0.05f;
                    if (this.fuel <= 0f) this.dead = true;
                    if (this.controlX > 0 && this.velocity.X < 5)
                    {
                        this.velocity.X += this.controlX * this.moveSpeed;
                    }
                    else if (this.controlX < 0 && this.velocity.X > -5)
                    {
                        this.velocity.X += this.controlX * this.moveSpeed;
                    }

                    if (this.controlY > 0 && this.velocity.Y < 5)
                    {
                        this.velocity.Y += this.controlY * this.moveSpeed;
                    }
                    else if (this.controlY < 0 && this.velocity.Y > -5)
                    {
                        this.velocity.Y += this.controlY * this.moveSpeed;
                    }

                    if (Math.Abs(this.controlX) > 0)
                    {
                        if (this.controlX > 0)
                        {
                            if (this.rotation < MathHelper.ToRadians(10f))
                            {
                                this.rotation += MathHelper.ToRadians(0.5f);
                            }
                        }
                        else
                        {
                            if (this.rotation > MathHelper.ToRadians(-10f))
                            {
                                this.rotation -= MathHelper.ToRadians(0.5f);
                            }
                        }
                    }
                    if (this.controlX == 0)
                    {
                        if (this.velocity.X > 0) this.velocity.X -= 0.1f;
                        if (this.velocity.X < 0) this.velocity.X += 0.1f;
                        if (Math.Abs(this.velocity.X) < 0.1f) this.velocity.X = 0f;
                        if (this.rotation < 0) this.rotation += MathHelper.ToRadians(0.5f);
                        if (this.rotation > 0) this.rotation -= MathHelper.ToRadians(0.5f);
                        if (Math.Abs(this.rotation) < MathHelper.ToRadians(1)) this.rotation = 0;
                    }
                    if (this.controlY == 0)
                    {
                        if (this.velocity.Y > 0) this.velocity.Y -= 0.1f;
                        if (this.velocity.Y < 0) this.velocity.Y += 0.1f;
                        if (Math.Abs(this.velocity.Y) < 0.1f) this.velocity.Y = 0f;
                    }
                }
                else
                {
                    this.velocity.Y += 0.1f;
                }
            }
            else
            {
                this.engineOn = false;
                for (int i = 0; i < Main.random.Next(10); i++)
                {
                    int d1 = Main.random.Next(-this.width / 2, this.width / 2);

                    int x = (int) (this.position.X + (Math.Cos(this.rotation) * d1));
                    int y = (int) (this.position.Y + (Math.Sin(this.rotation) * d1));
                    byte t = 0;
                    if (Main.random.Next(100) < 75) t = 1;
                    Dust d = Dust.newDust(t, new Vector2(x, y));
                }
                this.velocity.X = (float) Math.Cos(this.rotation + MathHelper.PiOver2) * 5;
                if (this.rotation > MathHelper.ToRadians(1.05f))
                {
                    this.rotationVel -= MathHelper.ToRadians(0.1f);
                }
                else if (this.rotation < -MathHelper.ToRadians(1.05f))
                {
                    this.rotationVel += MathHelper.ToRadians(0.1f);
                }
                else
                {
                    this.rotationVel += MathHelper.ToRadians(Main.random.Next(-1, 1));
                }
                if (this.position.Y > Main.worldBoundsDown - Main.groundHeight || this.position.Y < 0 || this.position.X < 0 || this.position.X > Main.screenWidth)
                {
                    Dust.Explosion(1, this.position);
                    this.active = false;
                }
            }

            this.position += this.velocity;
            this.rotation += this.rotationVel;

            foreach (Structure s in Main.structure)
            {
                if (s.active)
                {
                    if (s.collideWithPlayer)
                    {
                        Vector2 p2 = this.position - new Vector2(this.width / 2, this.height / 2);
                        Vector2 sp = s.position - new Vector2(s.width / 2, s.height / 2);
                        Rectangle pr = new Rectangle((int)p2.X, (int)p2.Y, this.width, this.height);
                        Rectangle sr = new Rectangle((int)sp.X, (int)sp.Y, s.width, s.height);
                        if (pr.Intersects(sr))
                        {
                            Rectangle r = Rectangle.Intersect(pr, sr);
                            Vector2 depth = new Vector2(r.Width, r.Height);
                            if (this.velocity.X > 0 && depth.X > 0)
                            {
                                depth.X = -depth.X;
                                this.velocity.X = 0;
                            }
                            if (this.velocity.X < 0 && depth.X > 0)
                            {
                                this.velocity.X = 0;
                            }
                            if (this.velocity.Y > 0 && depth.Y > 0)
                            {
                                depth.Y = -depth.Y;
                                this.velocity.Y = 0;
                            }
                            if (this.velocity.Y < 0 && depth.Y > 0)
                            {
                                this.velocity.Y = 0;
                            }

                            this.position += depth;
                        }
                    }
                }
            }

            if (this.position.X - (this.width / 2) < Main.worldBoundsLeft) { this.position.X = Main.worldBoundsLeft + (this.width / 2); this.velocity.X = 0; }
            if (this.position.X + (this.width / 2) > Main.worldBoundsRight) { this.position.X = Main.worldBoundsRight - (this.width / 2); this.velocity.X = 0; }
            if (this.position.Y - (this.height / 2) < Main.worldBoundsUp) { this.position.Y = Main.worldBoundsUp + (this.height / 2); this.velocity.Y = 0;}
            if (!this.dead) if (this.position.Y + (this.height / 2) > Main.worldBoundsDown - Main.groundHeight) { this.position.Y = Main.worldBoundsDown - Main.groundHeight - (this.height / 2); this.velocity.Y = 0; }
        }

        public static void UpdatePlayers()
        {
            foreach (Player p in Main.player)
            {
                if (p.active)
                {
                    p.Update();
                }
            }
        }

        public static Player newPlayer(Vector2 pos)
        {
            Player p = new Player();
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (!Main.player[i].active)
                {
                    Main.player[i] = p;
                    p.whoAmI = i;
                    p.active = true;
                    p.width = 165;
                    p.height = 50;
                    p.position = pos;
                    p.life = 100;
                    p.maxLife = 100;
                    for (int j = 0; j < p.weapon.Length; j++) p.weapon[j] = new Weapon();
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
