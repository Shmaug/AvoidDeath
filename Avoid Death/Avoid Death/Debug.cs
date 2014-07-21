using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Avoid_Death
{
    public class Debug
    {
        public static string[] constantText = new string[10];
        public static string[] debugText = new string[10];

        public static void print(string output)
        {
            Debug.debugText[0] = Debug.debugText[1];
            Debug.debugText[1] = Debug.debugText[2];
            Debug.debugText[2] = Debug.debugText[3];
            Debug.debugText[3] = Debug.debugText[4];
            Debug.debugText[4] = Debug.debugText[5];
            Debug.debugText[5] = Debug.debugText[6];
            Debug.debugText[6] = Debug.debugText[7];
            Debug.debugText[7] = Debug.debugText[8];
            Debug.debugText[8] = Debug.debugText[9];
            Debug.debugText[9] = output;
        }

        public static void draw()
        {
            string final = "";
            string final2 = "";
            for (int i = 0; i < Debug.debugText.Length; i++)
            {
                if (Debug.debugText[i] != null && Debug.debugText[i] != "")
                {
                    final = final + Debug.debugText[i] + "\n";
                }
                if (Debug.constantText[i] != null && Debug.constantText[i] != "")
                {
                    final2 = final2 + Debug.constantText[i] + "\n";
                }
            }

            Main.spriteBatch.DrawString(Main.font[0], final, new Vector2(0, 0), Color.Red);
            Main.spriteBatch.DrawString(Main.font[0], final2, new Vector2(Main.screenWidth - 800, 0), Color.Blue);
        }
    }
}
