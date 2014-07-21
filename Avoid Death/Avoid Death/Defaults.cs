using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Avoid_Death
{
    class Defaults
    {
        public static void setDefaults(Weapon w)
        {

            switch (w.type)
            {
                case 0:
                    {
                        w.origin = new Vector2(25, 6);
                        w.width = 50;
                        w.height = 12;
                        w.posOnOwner = new Vector2(20f, 0);
                        w.shootSpeed = 25;
                        w.lastShot = 25;
                        w.bulletType = 0;
                        w.maxAmmo = 10;
                        w.ammo = w.maxAmmo;
                        w.reloadTime = 150;
                        w.name = "T1 Missile Launcher";
                        break;
                    }
                case 1:
                    {
                        w.origin = new Vector2(25, 6);
                        w.width = 50;
                        w.height = 12;
                        w.posOnOwner = new Vector2(20f, 0);
                        w.shootSpeed = 50;
                        w.lastShot = 50;
                        w.bulletType = 1;
                        w.maxAmmo = 1;
                        w.ammo = w.maxAmmo;
                        w.reloadTime = 30;
                        w.name = "T16 Heat Seakers";
                        break;
                    }
                case 2:
                    {
                        w.origin = new Vector2(43, 7);
                        w.width = 50;
                        w.height = 12;
                        w.posOnOwner = new Vector2(0, 16);
                        w.shootSpeed = 6;
                        w.lastShot = 6;
                        w.bulletType = 2;
                        w.maxAmmo = 100;
                        w.ammo = w.maxAmmo;
                        w.reloadTime = 150;
                        w.name = "M97 Gatling Gun";
                        break;
                    }
                case 3:
                    {
                        w.origin = new Vector2(38, 5);
                        w.width = 50;
                        w.height = 12;
                        w.posOnOwner = new Vector2(10f, 0);
                        w.shootSpeed = 100;
                        w.lastShot = 100;
                        w.bulletType = 3;
                        w.maxAmmo = 5;
                        w.ammo = w.maxAmmo;
                        w.reloadTime = 400;
                        w.name = "L49 Plasma Bolt";
                        break;
                    }
            }
        }

        public static void setDefaults(Projectile p)
        {

            switch (p.type)
            {
                case 0:
                    {
                        p.width = 45;
                        p.height = 12;
                        p.damage = 50;
                        p.lifeTime = 200;
                        p.hasDust = true;
                        p.dustRate = 2;
                        p.dustType = 0;
                        p.selfPropelled = true;
                        p.speed = 10f;
                        p.explosive = true;
                        break;
                    }
                case 1:
                    {
                        p.width = 45;
                        p.height = 12;
                        p.damage = 50;
                        p.lifeTime = 200;
                        p.hasDust = true;
                        p.dustRate = 2;
                        p.dustType = 0;
                        p.selfPropelled = true;
                        p.speed = 10f;
                        p.explosive = true;
                        break;
                    }
                case 2:
                    {
                        p.width = 24;
                        p.height = 4;
                        p.damage = 2;
                        p.lifeTime = 200;
                        p.hasDust = false;
                        p.dustRate = 0;
                        p.selfPropelled = true;
                        p.speed = 20f;
                        break;
                    }
                case 3:
                    {
                        p.width = 32;
                        p.height = 32;
                        p.damage = 100;
                        p.lifeTime = 200;
                        p.hasDust = false;
                        p.dustRate = 0;
                        p.selfPropelled = true;
                        p.speed = 20f;
                        p.explosive = true;
                        break;
                    }
            }
        }

        public static void setDefaults(Dust d)
        {

            switch (d.type)
            {
                case 0:
                    {
                        d.width = 16;
                        d.height = 16;
                        d.lifeTime = 50;
                        d.maxLifeTime = 50;
                        break;
                    }
                case 1:
                    {
                        d.width = 32;
                        d.height = 32;
                        d.lifeTime = 64;
                        d.maxLifeTime = 64;
                        break;
                    }
                case 2:
                    {
                        d.width = 32;
                        d.height = 32;
                        d.lifeTime = 256;
                        d.maxLifeTime = 256;
                        d.fades = false;
                        d.hasGravity = true;
                        d.collideWithGround = true;
                        break;
                    }
                case 3:
                    {
                        d.width = 32;
                        d.height = 32;
                        d.lifeTime = 64;
                        d.maxLifeTime = 64;
                        break;
                    }
                case 4:
                    {
                        d.width = 16;
                        d.height = 16;
                        d.lifeTime = 256;
                        d.maxLifeTime = 256;
                        d.fades = false;
                        d.hasGravity = true;
                        d.collideWithGround = true;
                        break;
                    }
            }
        }

        public static void setDefaults(NPC n)
        {

            switch (n.type)
            {
                case 0:
                    {
                        n.width = 165;
                        n.height = 50;
                        n.maxShootCoolDown = 75;
                        n.maxLife = 100;
                        n.life = 100;
                        break;
                    }
                case 1:
                    {
                        n.width = 103;
                        n.height = 34;
                        n.maxShootCoolDown = 75;
                        n.maxLife = 100;
                        n.life = 100;
                        break;
                    }
            }
        }

        public static void setDefaults(Structure s)
        {
            switch (s.type)
            {
                case 0:
                    {
                        s.width = 256;
                        s.height = 128;
                        s.curFrame = 0;
                        s.frames = 3;
                        s.frame = new Rectangle(0, 0, s.width, s.height);
                        s.life = 100;
                        s.maxLife = 100;
                        s.debriType = 2;
                        s.maxDebri = 4;
                        break;
                    }
                case 1:
                    {
                        s.width = 170;
                        s.height = 58;
                        s.curFrame = 0;
                        s.frames = 2;
                        s.frame = new Rectangle(0, 0, s.width, s.height);
                        s.life = 50;
                        s.maxLife = 50;
                        s.debriType = 4;
                        s.maxDebri = 10;
                        s.collideWithPlayer = true;
                        break;
                    }
            }

        }
    }
}
