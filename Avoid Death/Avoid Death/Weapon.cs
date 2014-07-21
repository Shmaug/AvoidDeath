using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Avoid_Death
{
    public class Weapon
    {
        public int width = 0;
        public int height = 0;
        public int owner = -1;
        public int shootSpeed = 10;
        public int lastShot = 0;
        public int ammo = 0;
        public int maxAmmo = 0;
        public int reloadTime = 0;
        public int reloadTimer = 0;
        public byte type = 0;
        public byte bulletType = 0;
        public bool active = false;
        public bool ownedByNPC = false;
        public float rotation = 0f;
        public string name = "";
        public Vector2 posOnOwner = Vector2.Zero;
        public Vector2 origin = Vector2.Zero;

        public Weapon()
        {

        }

        public bool Shoot()
        {
            if (this.lastShot <= 0 && this.ammo > 0)
            {
                float ownrot = 0f;
                Vector2 ownpos = Vector2.Zero;

                if (this.ownedByNPC)
                {
                    ownrot = Main.npc[this.owner].rotation;
                    ownpos = Main.npc[this.owner].position;
                }
                else
                {
                    ownrot = Main.player[this.owner].rotation;
                    ownpos = Main.player[this.owner].position;
                }

                double angle = Math.Atan2(this.posOnOwner.Y, this.posOnOwner.X);
                int x = (int) (ownpos.X - (Math.Cos(ownrot + angle) * -this.posOnOwner.X));
                int y = (int) (ownpos.Y - (Math.Sin(ownrot + angle) * -this.posOnOwner.Y));
                Vector2 pos = new Vector2(x, y);
                Projectile p = Projectile.newProjectile(this.bulletType, pos);
                p.rotation = this.rotation;
                p.hostile = this.ownedByNPC;
                this.lastShot = shootSpeed;
                this.ammo--;
                if (this.ammo <= 0 && this.reloadTimer <= 0)
                {
                    this.reloadTimer = this.reloadTime;
                }
                Main.weaponSound[this.type].Play();
            }
            return true;
        }

        public void Update(Vector2 target)
        {
            Vector2 myPos = Vector2.Zero;
            if (this.ownedByNPC) myPos = Main.npc[this.owner].position;
            if (!this.ownedByNPC) myPos = Main.player[this.owner].position;
            if (this.lastShot > 0) this.lastShot--;
            int X = (int) target.X;
            int Y = (int) target.Y;
            float myX = this.posOnOwner.X + myPos.X;
            float myY = this.posOnOwner.Y + myPos.Y;

            float angle = (float) Math.Atan2(myY - Y , myX - X);

            this.rotation = angle;

            if (this.reloadTimer > 0)
            {
                this.reloadTimer--;
                if (this.reloadTimer <= 0)
                {
                    this.ammo = this.maxAmmo;
                }
            }
        }
    }
}
