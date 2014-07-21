using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avoid_Death
{
    public class NPC
    {
        public static double MainRotorTheta = Math.Atan2(23, 18);
        public static double tailRotorTheta = Math.Atan2(16, -74);

        public int width = 0;
        public int height = 0;
        public int whoAmI = -1;
        public int rotorSize = 128;
        public int rotorSizeIncrement = 32;
        public byte type = 0;
        public int shootCoolDown = 0;
        public int maxShootCoolDown = 0;
        public int maxLife = 0;
        public int life = 0;
        public int[] activeWeapons;
        public int shotsToTake = 5;
        public bool active = false;
        public bool onGround = false;
        public bool dead = false;
        public bool flyingIn = true;
        public bool propOn = true;
        public float tireRot = 0f;
        public float controlX = 0f;
        public float controlY = 0f;
        public float rotation = 0f;
        public float rotationVel = 0f;
        public float tailRotorRotation = 0f;
        public Weapon[] weapon = new Weapon[10];
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Vector2 pointer = Vector2.Zero;
        public Vector2 targetPos = Vector2.Zero;
        public Rectangle frame = new Rectangle(0, 0, 0, 0);

        public NPC()
        {

        }

        public void Update()
        {
            if (this.type == 0)
            {
                this.rotorSize += this.rotorSizeIncrement;
                if (this.rotorSize < 0 || this.rotorSize > 128) this.rotorSizeIncrement *= -1;
            }
            if (!this.dead)
            {
                if (this.shootCoolDown > 0)
                {
                    this.shootCoolDown--;
                }

                if (this.shootCoolDown <= 0)
                {
                    for (int j = 0; j < this.shotsToTake; j++)
                    {
                        int r = Main.random.Next(this.activeWeapons.Length);
                        int i = this.activeWeapons[r];
                        if (this.weapon[i].active)
                        {
                            this.weapon[i].Shoot();
                            if (Main.random.Next(10) == 5)
                            {
                                this.shootCoolDown = Main.random.Next(this.maxShootCoolDown);
                            }
                        }
                    }
                }

                this.pointer = Main.player[Main.myPlayer].position;

                this.tailRotorRotation += MathHelper.ToRadians(9f); ;
                for (int i = 0; i < this.weapon.Length; i++)
                {
                    if (this.weapon[i].active)
                    {
                        this.weapon[i].Update(this.pointer);
                    }
                }

                if (Math.Abs(this.velocity.X) < 5 && Math.Abs(this.controlX) > 0)
                {
                    this.velocity.X += this.controlX * 0.1f;
                }
                if (Math.Abs(this.velocity.Y) < 5 && Math.Abs(this.controlY) > 0)
                {
                    this.velocity.Y += this.controlY * 0.1f;
                }
                if (this.velocity.X >= 5) this.velocity.X = 4;
                if (this.velocity.X <= -5) this.velocity.X = -4;

                if (this.controlX == 0)
                {
                    if (this.velocity.X > 0) this.velocity.X -= 0.1f;
                    if (this.velocity.X < 0) this.velocity.X += 0.1f;
                    if (Math.Abs(this.velocity.X) < 0.1f) this.velocity.X = 0f;
                }
                if (this.controlY == 0)
                {
                    if (this.type == 0)
                    {
                        this.velocity = Vector2.Lerp(this.velocity, Vector2.Zero, 0.01f);
                        if (Math.Abs(this.velocity.Y) < 0.1f) this.velocity.Y = 0f;
                    }
                    else if (this.type == 1)
                    {
                        if (this.position.Y + (this.height / 2) >= Main.worldBoundsDown - Main.groundHeight)
                        {
                            this.position.Y = Main.worldBoundsDown - Main.groundHeight - (this.height / 2);
                        }
                        else
                        {
                            this.velocity.Y++;
                        }
                    }
                }
                if (this.type == 0)
                {
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
                                this.rotation += MathHelper.ToRadians(-0.5f);
                            }
                        }
                    }
                    else
                    {
                        if (this.rotation < 0) this.rotation += MathHelper.ToRadians(0.5f);
                        if (this.rotation > 0) this.rotation -= MathHelper.ToRadians(0.5f);
                        if (Math.Abs(this.rotation) < MathHelper.ToRadians(1)) this.rotation = 0;
                    }
                }
                else if (this.type == 1)
                {
                    this.tireRot += MathHelper.ToRadians(this.controlX*10);
                    if (this.controlX > 0 && this.velocity.X < 0 || this.controlX < 0 && this.velocity.X > 0)
                    {
                        Dust d = Dust.newDust(0, this.position + new Vector2(36, this.height/2));
                        if (d != null)
                        {
                            d.velocity.X = this.controlX * -4;
                            d.velocity.Y = -Main.random.Next(5);
                        }
                    }
                }
            }

            if (!this.propOn)
            {
                this.velocity.Y += 0.1f;
            }

            this.position += this.velocity;
            this.rotation += this.rotationVel;

            if (this.life <= 0) this.dead = true;

            if (this.dead)
            {
                this.propOn = false;
                for (int i = 0; i < Main.random.Next(10); i++)
                {
                    int d1 = Main.random.Next(-this.width / 2, this.width / 2);

                    int x = (int) (this.position.X + (Math.Cos(this.rotation) * d1));
                    int y = (int) (this.position.Y + (Math.Sin(this.rotation) * d1));
                    byte t = 0;
                    if (Main.random.Next(100) < 50) t = 1;
                    Dust d = Dust.newDust(t, new Vector2(x, y));
                }

                this.velocity.Y += 0.1f;
                if (this.position.Y > Main.worldBoundsDown - Main.groundHeight || this.type == 1)
                {
                    float fac = 60f;
                    float dist = Vector2.Distance(this.position, Main.player[Main.myPlayer].position);
                    if (dist > 1500f)
                    {
                        dist = 1500f;
                    }
                    fac = 1500f - dist;
                    fac /= 17;
                    Main.camShakeFactor = (int) (fac);
                    Dust.Explosion(1, this.position);
                    this.active = false;
                }
            }

            if (!this.dead)
            {
                this.targetPos = Main.player[Main.myPlayer].position;
                double angle = Math.Atan2(this.position.Y - this.targetPos.Y, this.position.X - this.targetPos.X);
                this.controlX = (float) -Math.Cos(angle) / 2;
                this.controlY = (float) -Math.Sin(angle) / 2;
                if (this.type == 1) this.controlY = 0f;

                if (Vector2.Distance(this.position, this.targetPos) < this.width / 4)
                {
                    this.controlX = 0f;
                    this.controlY = 0f;
                }

                if (this.position.X - (this.width / 2) < Main.worldBoundsLeft) this.position.X = Main.worldBoundsLeft + (this.width / 2);
                if (this.position.X + (this.width / 2) > Main.worldBoundsRight) this.position.X = Main.worldBoundsRight - (this.width / 2);
                if (this.position.Y - (this.height / 2) < Main.worldBoundsUp) this.position.Y = Main.worldBoundsUp + (this.height / 2);
                if (this.position.Y + (this.height / 2) > Main.worldBoundsDown + Main.groundHeight) this.position.Y = Main.worldBoundsDown - Main.groundHeight - (this.height / 2);

                int n = 0;
                if (this.type == 1) n = 8;
                if (this.position.Y + (this.height / 2) + n > Main.worldBoundsDown - Main.groundHeight) { this.position.Y = Main.worldBoundsDown - Main.groundHeight - (this.height / 2) - n; this.velocity.Y = 0f;}
            }
        }

        public void draw()
        {
            int posX = (int) (Main.screenPosition.X + Main.camShake.X);
            int posY = (int) (Main.screenPosition.Y + Main.camShake.Y);
            SpriteEffects effects = SpriteEffects.None;
            if (this.type == 0)
            {
                Main.spriteBatch.Draw(
                    Main.npcTexture[0],
                    new Rectangle((int) this.position.X - posX, (int) this.position.Y - posY, this.width, this.height),
                    new Rectangle(31, 0, 165, 50),
                    Color.White,
                    this.rotation,
                    new Vector2(this.width / 2, this.height / 2),
                    effects,
                    0f);

                int MainRotorX = (int) (this.position.X - (Math.Cos(this.rotation + MainRotorTheta) * 29));
                int MainRotorY = (int) (this.position.Y - (Math.Sin(this.rotation + MainRotorTheta) * 29));

                Main.spriteBatch.Draw(
                    Main.npcTexture[0],
                    new Rectangle(MainRotorX - posX, MainRotorY - posY, this.rotorSize, 1),
                    new Rectangle(31, 0, 165, 50),
                    Color.Black,
                    this.rotation,
                    new Vector2(this.rotorSize / 2, 0),
                    effects,
                    0f);

                int tailRotorX = (int) (this.position.X - (Math.Cos(this.rotation + tailRotorTheta) * 75));
                int tailRotorY = (int) (this.position.Y - (Math.Sin(this.rotation + tailRotorTheta) * 75));

                Main.spriteBatch.Draw(
                    Main.npcTexture[0],
                    new Rectangle(tailRotorX - posX, tailRotorY - posY, 30, 30),
                    new Rectangle(0, 0, 30, 30),
                    Color.Black,
                    this.tailRotorRotation,
                    new Vector2(15, 15),
                    effects,
                    0f);
            }
            else if (this.type == 1)
            {
                Main.spriteBatch.Draw(
                    Main.npcTexture[1],
                    new Rectangle((int) this.position.X - posX, (int) this.position.Y - posY, this.width, this.height),
                    new Rectangle(16, 0, 103, 34),
                    Color.White,
                    0f,
                    new Vector2(this.width / 2, this.height / 2),
                    effects,
                    0f);

                Main.spriteBatch.Draw(
                    Main.npcTexture[1],
                    new Rectangle((int) this.position.X - posX - 33, (int) this.position.Y - posY + (this.height / 2), 16, 16),
                    new Rectangle(0, 0, 16, 16),
                    Color.White,
                    this.tireRot,
                    new Vector2(8, 8),
                    effects,
                    0f);

                Main.spriteBatch.Draw(
                    Main.npcTexture[1],
                    new Rectangle((int) this.position.X - posX + 36, (int) this.position.Y - posY + (this.height / 2), 16, 16),
                    new Rectangle(0, 0, 16, 16),
                    Color.White,
                    this.tireRot,
                    new Vector2(8, 8),
                    effects,
                    0f);
            }

            for (int j = 0; j < this.weapon.Length; j++)
            {
                Weapon w = this.weapon[j];
                if (w.active)
                {
                    int x = (int) (this.position.X + w.posOnOwner.X);
                    int y = (int) (this.position.Y + w.posOnOwner.Y);
                    Main.spriteBatch.Draw(
                        Main.weaponTexture[w.type],
                        new Rectangle(x - posX, y - posY, w.width, w.height),
                        null,
                        Color.White,
                        w.rotation,
                        w.origin,
                        SpriteEffects.None,
                        0f);
                }
            }

            double curLife = ((double) this.life / (double) this.maxLife) * 100f;
            if (curLife < 0) curLife = 0;
            string text = curLife + "%";
            int c = (int) (curLife / 100);
            Color color = new Color(255-(c*255), c * 255, 0);
            Main.spriteBatch.DrawString(Main.font[0], text, this.position - new Vector2(Main.font[0].MeasureString(text).X  / 2, this.height) - Main.screenPosition, color);
        }

        public static void UpdateNPCs()
        {
            foreach (NPC n in Main.npc)
            {
                if (n.active)
                {
                    n.Update();
                }
            }
        }

        public static NPC newNPC(byte type, Vector2 pos)
        {
            NPC n = new NPC();
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Main.npc[i].active)
                {
                    n.type = type;
                    n.whoAmI = i;
                    n.active = true;
                    n.width = 165;
                    n.height = 50;
                    n.position = pos;
                    for (int j = 0; j < n.weapon.Length; j++)
                    {
                        n.weapon[j] = new Weapon();
                        n.weapon[j].ownedByNPC = true;
                    }
                    Defaults.setDefaults(n);
                    Main.npc[i] = n;
                    break;
                }
            }
            if (n.active)
            {
                return n;
            }
            return null;
        }
    }
}
