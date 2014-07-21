using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Avoid_Death
{
    class IO
    {
        public static void writeSave(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Main.player[Main.myPlayer].active);
            writer.Write(Main.player[Main.myPlayer].position.X);
            writer.Write(Main.player[Main.myPlayer].position.Y);
            for (int i = 0; i < Main.player[Main.myPlayer].weapon.Length; i++)
            {
                writer.Write(Main.player[Main.myPlayer].weapon[i].active);
                writer.Write(Main.player[Main.myPlayer].weapon[i].type);
            }/*
            foreach (Structure s in Main.structure)
            {
                writer.Write(s.active);
                writer.Write(s.type);
                writer.Write(s.position.X);
                writer.Write(s.position.Y);
                writer.Write(s.life);
                writer.Write(s.maxLife);
            }
            foreach (NPC n in Main.npc)
            {
                writer.Write(n.active);
                writer.Write(n.type);
                writer.Write(n.position.X);
                writer.Write(n.position.Y);
                writer.Write(n.life);
                writer.Write(n.maxLife);
            }*/
            writer.Close();
            stream.Close();
        }

        public void readLevel(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            Main.player[Main.myPlayer].active = reader.ReadBoolean();
            Main.player[Main.myPlayer].position.X = reader.ReadSingle();
            Main.player[Main.myPlayer].position.Y = reader.ReadSingle();

            for (int i = 0; i < Main.player[Main.myPlayer].weapon.Length; i++)
            {
                Main.player[Main.myPlayer].weapon[i].active = reader.ReadBoolean();
                Main.player[Main.myPlayer].weapon[i].type = reader.ReadByte();
            }
            reader.Close();
            stream.Close();
        }

        public static void readSave(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            Main.player[Main.myPlayer].active = reader.ReadBoolean();
            Main.player[Main.myPlayer].position.X = reader.ReadSingle();
            Main.player[Main.myPlayer].position.Y = reader.ReadSingle();

            for (int i = 0; i < Main.player[Main.myPlayer].weapon.Length; i++)
            {
                Main.player[Main.myPlayer].weapon[i].active = reader.ReadBoolean();
                Main.player[Main.myPlayer].weapon[i].type = reader.ReadByte();
            }
            reader.Close();
            stream.Close();
        }
    }
}
