using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Avoid_Death
{
    public class Main : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        public static int buttonSelected = -1;
        public static int menuLastMissile = 0;
        public static int curMenu = 0;

        public static Texture2D nullTexture;
        public static Texture2D groundTexture;
        public static Texture2D mouseTexture;
        public static Texture2D playerTexture;
        public static Texture2D windowTexture;
        public static Texture2D buttonTexture;
        public static Texture2D buttonHighlightTexture;
        public static Texture2D logoTexture;
        public static Texture2D checkTexture;
        public static Texture2D gasCanTexture;
        public static Texture2D lifeCrossTexture;
        public static Texture2D[] weaponTexture = new Texture2D[4];
        public static Texture2D[] projectileTexture = new Texture2D[4];
        public static Texture2D[] npcTexture = new Texture2D[2];
        public static Texture2D[] dustTexture = new Texture2D[5];
        public static Texture2D[] structureTexture = new Texture2D[2];
        public static Texture2D[] backgroundTexture = new Texture2D[1];

        public static SoundEffect[] weaponSound = new SoundEffect[4];
        public static SoundEffect[] explosionSound = new SoundEffect[1];

        public static SpriteFont[] font = new SpriteFont[3];

        public static bool inGame = false;
        public static bool isPaused = false;
        public static bool usingGamepad = false;

        public static KeyboardState keyboardState = Keyboard.GetState();
        public static MouseState mouseState = Mouse.GetState();
        public static KeyboardState oldKeyboardState = Keyboard.GetState();
        public static MouseState oldMouseState = Mouse.GetState();
        public static GamePadState GamePadState;
        public static GamePadState oldGamePadState;

        public static Random random = new Random(-1);

        public static Player[] player = new Player[200];
        public static NPC[] npc = new NPC[200];
        public static Dust[] dust = new Dust[5000];
        public static Projectile[] projectile = new Projectile[2000];
        public static Structure[] structure = new Structure[200];
        public static int myPlayer = -1;

        public static int[] last = { 100, 0 };

        public static double masterVolume = 100;
        public static int worldBoundsLeft = 0;
        public static int worldBoundsRight = 5000;
        public static int worldBoundsUp = 0;
        public static int worldBoundsDown = 1500;
        public static Vector2 screenPosition = Vector2.Zero;
        public static bool camOnPlayer = true;
        public static Vector2 camShake = Vector2.Zero;
        public static Vector2 playerSpawnPos = Vector2.Zero;
        public static int camShakeFactor = 0;
        public static int groundHeight = 128;

        public static int menuCurWeaponSel = 0;
        public static bool menuPlaceWeapon = false;
        public static Vector2 menuPlaceWeaponPos = Vector2.Zero;

        public static float frameInterval = 1f / 60f;
        public static float frameTime;

        public static Weapon[] Loadout = new Weapon[10];

        public static string saveDirectory = "";

        public static Button[] MainMenuButtons = new Button[4];
        public static Button[] pauseMenuButtons = new Button[1];
        public static Button[] customizeMenuButtons = new Button[1];
        public static Button[] optionsButtons = new Button[3];

        public static bool pendingGameStart = false;
        public static bool pendingGameExit = false;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            //graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = true;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            try
            {
                GamePadState = GamePad.GetState(PlayerIndex.One);
                oldGamePadState = GamePad.GetState(PlayerIndex.One);
            }
            catch { }

            for (int i = 0; i < dust.Length; i++) { dust[i] = new Dust(); dust[i].whoAmI = i; }
            for (int i = 0; i < projectile.Length; i++) { projectile[i] = new Projectile(); projectile[i].whoAmI = i; }
            for (int i = 0; i < player.Length; i++) { player[i] = new Player(); player[i].whoAmI = i; }
            for (int i = 0; i < npc.Length; i++)
            {
                npc[i] = new NPC();
                npc[i].whoAmI = i;
                for (int j = 0; j < npc[i].weapon.Length; j++)
                {
                    npc[i].weapon[j] = new Weapon();
                    npc[i].weapon[j].ownedByNPC = true;
                }
            }
            for (int i = 0; i < structure.Length; i++) { structure[i] = new Structure(); structure[i].whoAmI = i; }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            nullTexture = Content.Load<Texture2D>("image/misc/null");
            groundTexture = Content.Load<Texture2D>("image/misc/ground");
            windowTexture = Content.Load<Texture2D>("image/misc/window");
            mouseTexture = Content.Load<Texture2D>("image/ui/mouse");
            buttonTexture = Content.Load<Texture2D>("image/ui/button");
            buttonHighlightTexture = Content.Load<Texture2D>("image/ui/highlight");
            logoTexture = Content.Load<Texture2D>("image/ui/logo");
            checkTexture = Content.Load<Texture2D>("image/ui/check");
            gasCanTexture = Content.Load<Texture2D>("image/ui/fuel");
            lifeCrossTexture = Content.Load<Texture2D>("image/ui/life");
            playerTexture = Content.Load<Texture2D>("image/entity/player");
            for (int i = 0; i < font.Length; i++) font[i] = Content.Load<SpriteFont>("font/" + i);
            for (int i = 0; i < weaponTexture.Length; i++) weaponTexture[i] = Content.Load<Texture2D>("image/weapon/" + i);
            for (int i = 0; i < projectileTexture.Length; i++) projectileTexture[i] = Content.Load<Texture2D>("image/projectile/" + i);
            for (int i = 0; i < dustTexture.Length; i++) dustTexture[i] = Content.Load<Texture2D>("image/dust/" + i);
            for (int i = 0; i < npcTexture.Length; i++) npcTexture[i] = Content.Load<Texture2D>("image/entity/npc_" + i);
            for (int i = 0; i < structureTexture.Length; i++) structureTexture[i] = Content.Load<Texture2D>("image/structure/" + i);
            for (int i = 0; i < backgroundTexture.Length; i++) backgroundTexture[i] = Content.Load<Texture2D>("image/background/" + i);

            for (int i = 0; i < explosionSound.Length; i++) explosionSound[i] = Content.Load<SoundEffect>("sound/fx/explosion_" + i);
            for (int i = 0; i < weaponSound.Length; i++) weaponSound[i] = Content.Load<SoundEffect>("sound/gun/" + i);

            // Main Menu //
            Button b = new Button(screenWidth - 300, 50, "START GAME", -2);
            MainMenuButtons[0] = b;

            b = new Button(screenWidth - 300, 130, "CUSTOMIZE", 1);
            MainMenuButtons[1] = b;

            b = new Button(screenWidth - 300, 210, "OPTIONS", 2);
            MainMenuButtons[2] = b;

            b = new Button(screenWidth - 300, 290, "EXIT", -1);
            MainMenuButtons[3] = b;

            // Settings //
            b = new Button(screenWidth - 300, 50, "GAMEPAD", -3);
            optionsButtons[0] = b;

            b = new Button(screenWidth - 300, 130, "KEYBOARD", -4);
            optionsButtons[1] = b;

            b = new Button(screenWidth - 300, 210, "BACK", 0);
            optionsButtons[2] = b;

            // Customize //
            b = new Button(16, screenHeight - 80, "BACK", 0);
            customizeMenuButtons[0] = b;
            
            // Pause //
            b = new Button(screenWidth - 300, 50, "Main MENU", -5);
            pauseMenuButtons[0] = b;

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            // remove jitter
            frameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTime > frameInterval)
                frameTime = frameTime % frameInterval;
            else
            {
                SuppressDraw();
                base.Update(gameTime);
                return;
            }
            
            #region debug
            // show update rate //
            //if (last[0] == 0) last[0] = 100;
            //int t = gameTime.ElapsedGameTime.Milliseconds;
            //string fin = t.ToString();
            //if (t < last[0])
            //{
            //    last[0] = t; // save lowest //
            //}
            //if (t > last[1])
            //{
            //    last[1] = t; // save highest
            //}
            //fin += "  " + last[0].ToString();
            //fin += "  " + last[1].ToString();
            //Debug.constantText[0] = fin;
            #endregion
            
            
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            try
            {
                GamePadState = GamePad.GetState(PlayerIndex.One);
            }
            catch { }

            Control();
            
            if (inGame)
            {
                if (!isPaused)
                {
                    // Updates // 
                    if (!player[myPlayer].active)
                    {
                        respawnPlayer(myPlayer);
                    }
                    Player.UpdatePlayers();
                    NPC.UpdateNPCs();
                    Projectile.UpdateProjectiles();
                    Dust.UpdateDust();
                    if (camShakeFactor > 0)
                    {
                        camShake = new Vector2(random.Next(-camShakeFactor, camShakeFactor), random.Next(-camShakeFactor, camShakeFactor));
                        camShakeFactor--;
                    }
                }
            }
            else
            {
                // missiles on Main menu //
                foreach (Projectile p in projectile) if (p.active && p.menuMissile) p.Update();
                Dust.UpdateDust();

                if (menuLastMissile > 0)
                {
                    menuLastMissile--;
                }
                else
                {
                    Vector2 pos = new Vector2(random.Next(screenWidth), 0);
                    Projectile p = Projectile.newProjectile(0, pos);
                    Defaults.setDefaults(p);
                    p.menuMissile = true;
                    float endx = random.Next(screenWidth);
                    p.rotation = (float) Math.Atan2(pos.Y - screenHeight - 100, pos.X - endx);
                    menuLastMissile = random.Next(100);
                }

                if (usingGamepad)
                {
                    // button selection //
                    if (GamePadState.IsButtonDown(Buttons.DPadDown) && oldGamePadState.IsButtonUp(Buttons.DPadDown))
                    {
                        buttonSelected++;
                        if (curMenu == 0 && buttonSelected >= MainMenuButtons.Length) buttonSelected = MainMenuButtons.Length-1;
                        if (curMenu == 1 && buttonSelected >= customizeMenuButtons.Length) buttonSelected = customizeMenuButtons.Length-1;
                        if (curMenu == 2 && buttonSelected >= optionsButtons.Length) buttonSelected = optionsButtons.Length-1;
                        if (inGame && isPaused && buttonSelected >= pauseMenuButtons.Length) buttonSelected = pauseMenuButtons.Length - 1;
                    }
                    if (GamePadState.IsButtonDown(Buttons.DPadUp) && oldGamePadState.IsButtonUp(Buttons.DPadUp))
                    {
                        buttonSelected--;
                        if (buttonSelected < 0) buttonSelected = 0;
                    }
                    if (inGame && isPaused)
                    {
                        foreach (Button b in pauseMenuButtons)
                        {
                            b.selected = false;
                        }
                        pauseMenuButtons[buttonSelected].selected = true;
                    }
                    if (curMenu == 0)
                    {
                        foreach (Button b in MainMenuButtons)
                        {
                            b.selected = false;
                        }
                        MainMenuButtons[buttonSelected].selected = true;
                    }
                    else if (curMenu == 1)
                    {
                        foreach (Button b in customizeMenuButtons)
                        {
                            b.selected = false;
                        }
                        customizeMenuButtons[buttonSelected].selected = true;
                    }
                    else if (curMenu == 2)
                    {
                        foreach (Button b in optionsButtons)
                        {
                            b.selected = false;
                        }
                        optionsButtons[buttonSelected].selected = true;
                    }
                }
                else
                {
                    // button click detecting //
                    Rectangle mouseRect = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                    if (inGame && isPaused)
                    {
                        foreach (Button b in pauseMenuButtons)
                        {
                            b.checkSelected(mouseRect);
                        }
                    }
                    if (curMenu == 0)
                    {
                        foreach (Button b in MainMenuButtons)
                        {
                            b.checkSelected(mouseRect);
                        }
                    }
                    else if (curMenu == 2)
                    {
                        foreach (Button b in optionsButtons)
                        {
                            b.checkSelected(mouseRect);
                        }
                    }
                    else if (curMenu == 1)
                    {
                        foreach (Button b in customizeMenuButtons)
                        {
                            b.checkSelected(mouseRect);
                        }
                    }
                }
                bool click = false;
                if (usingGamepad)
                {
                    if (GamePadState.IsButtonDown(Buttons.A) && oldGamePadState.IsButtonUp(Buttons.A))
                    {
                        click = true;
                    }
                }
                else
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                    {
                        click = true;
                    }
                }
                if (click)
                {
                    if (curMenu == 0) // Main menu //
                    {
                        foreach (Button b in MainMenuButtons) b.tryClick();
                    }
                    else if (curMenu == 1) // customize //
                    {
                        foreach (Button b in customizeMenuButtons) b.tryClick();
                    }
                    else if (curMenu == 2) // options //
                    {
                        foreach (Button b in optionsButtons) b.tryClick();
                    }
                }
            }
            if (pendingGameExit) { this.Exit(); pendingGameExit = false; }
            if (pendingGameStart) { startGame(); pendingGameStart = false; }
            oldKeyboardState = keyboardState;
            oldMouseState = mouseState;
            oldGamePadState = GamePadState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            spriteBatch.Begin();

            if (inGame)
            {
                drawGame();
                if (isPaused)
                {
                    drawPauseMenu();
                }
            }
            else
            {
                drawMenu();
            }
            if (!usingGamepad && (!inGame || isPaused))
            {
                spriteBatch.Draw(mouseTexture, new Vector2(oldMouseState.X, oldMouseState.Y), Color.White);
            }
            if (inGame)
            {
                spriteBatch.Draw(
                    playerTexture,
                    new Rectangle((int) player[myPlayer].pointer.X, (int) player[myPlayer].pointer.Y, 32, 32),
                    new Rectangle(0, 30, 32, 32),
                    Color.White,
                    player[myPlayer].pointerDrawRot,
                    new Vector2(16, 16),
                    SpriteEffects.None,
                    0f);
            }

            Debug.draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
        // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
        // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //

        public static void Control()
        {
            if (keyboardState.IsKeyDown(Keys.LeftAlt) && (keyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter)))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }


            if (!usingGamepad)
            {
                if (inGame)
                {
                    if (keyboardState.IsKeyDown(Keys.LeftControl) && (keyboardState.IsKeyDown(Keys.S) && oldKeyboardState.IsKeyUp(Keys.S)))
                    {

                    }
                    if (keyboardState.IsKeyDown(Keys.LeftControl) && (keyboardState.IsKeyDown(Keys.O) && oldKeyboardState.IsKeyUp(Keys.O)))
                    {

                    }
                    if (keyboardState.IsKeyDown(Keys.Space) && oldKeyboardState.IsKeyUp(Keys.Space))
                    {
                        isPaused = !isPaused;
                    }
                    #region debugging
                    if (keyboardState.IsKeyDown(Keys.Z) && oldKeyboardState.IsKeyUp(Keys.Z))
                    {
                        // spawn helicopter
                        NPC n = NPC.newNPC(0, new Vector2(screenPosition.X + screenWidth + 200, screenHeight / 2));
                        if (n != null)
                        {
                            Weapon w = new Weapon();
                            w.active = true;
                            w.type = 0;
                            w.owner = n.whoAmI;
                            Defaults.setDefaults(w);
                            w.ownedByNPC = true;
                            n.weapon[0] = w;

                            Weapon w2 = new Weapon();
                            w2.active = true;
                            w2.type = 2;
                            w2.owner = n.whoAmI;
                            Defaults.setDefaults(w2);
                            w2.ownedByNPC = true;
                            n.weapon[1] = w2;

                            n.targetPos = new Vector2(mouseState.X, mouseState.Y);
                            n.activeWeapons = new int[2] {0, 1};
                        }
                    }
                    if (keyboardState.IsKeyDown(Keys.X) && oldKeyboardState.IsKeyUp(Keys.X))
                    {
                        // spawn structure
                        Structure s = Structure.newStructure(0, mouseState.X + (int) screenPosition.X);
                    }
                    if (keyboardState.IsKeyDown(Keys.C) && oldKeyboardState.IsKeyUp(Keys.C))
                    {
                        // spawn jeep
                        NPC n = NPC.newNPC(1, new Vector2(screenPosition.X + screenWidth + 20, screenHeight / 2) + screenPosition);
                        if (n != null)
                        {
                            Weapon w = new Weapon();
                            w.active = true;
                            w.type = 0;
                            w.owner = n.whoAmI;
                            Defaults.setDefaults(w);
                            w.ownedByNPC = true;
                            n.weapon[0] = w;

                            Weapon w2 = new Weapon();
                            w2.active = true;
                            w2.type = 2;
                            w2.owner = n.whoAmI;
                            Defaults.setDefaults(w2);
                            w2.ownedByNPC = true;
                            n.weapon[1] = w2;

                            n.targetPos = new Vector2(mouseState.X, mouseState.Y);
                            n.activeWeapons = new int[2] { 0, 1 };
                        }
                    }
                    if (keyboardState.IsKeyDown(Keys.F) && oldKeyboardState.IsKeyUp(Keys.F))
                    {
                        // kill all npc's
                        for (int i = 0; i < npc.Length; i++)
                        {
                            if (npc[i].active)
                            {
                                npc[i].life = 0;
                            }
                        }
                    }
                    #endregion
                    if (!isPaused)
                    {
                        // Shooting
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            player[myPlayer].shooting = true;
                        }
                        else
                        {
                            player[myPlayer].shooting = false;
                        }
                        // Controls
                        if (keyboardState.IsKeyDown(Keys.E) && oldKeyboardState.IsKeyUp(Keys.E))
                        {
                            player[myPlayer].engineOn = !player[myPlayer].engineOn;
                        }
                        if (keyboardState.IsKeyDown(Keys.W))
                        {
                            player[myPlayer].controlY = -1;
                        }
                        else if (keyboardState.IsKeyDown(Keys.S))
                        {
                            player[myPlayer].controlY = 1;
                        }
                        else
                        {
                            player[myPlayer].controlY = 0;
                        }

                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            player[myPlayer].controlX = -1;
                        }
                        else if (keyboardState.IsKeyDown(Keys.D))
                        {
                            player[myPlayer].controlX = 1;
                        }
                        else
                        {
                            player[myPlayer].controlX = 0;
                        }
                    }
                }
            }
            else                          // Gamepad //
            {
                if (GamePadState.IsButtonDown(Buttons.Start) && oldGamePadState.IsButtonUp(Buttons.Start))
                {
                    isPaused = !isPaused;
                }
                if (inGame)
                {
                    #region debugging
                    if (GamePadState.IsButtonDown(Buttons.Y) && oldGamePadState.IsButtonUp(Buttons.Y))
                    {
                        // spawn helicopter
                        NPC n = NPC.newNPC(0, new Vector2(screenPosition.X + screenWidth + 200, screenHeight / 2));
                        if (n != null)
                        {
                            Weapon w = new Weapon();
                            w.active = true;
                            w.type = 0;
                            w.owner = n.whoAmI;
                            Defaults.setDefaults(w);
                            w.ownedByNPC = true;
                            n.weapon[0] = w;

                            Weapon w2 = new Weapon();
                            w2.active = true;
                            w2.type = 2;
                            w2.owner = n.whoAmI;
                            Defaults.setDefaults(w2);
                            w2.ownedByNPC = true;
                            n.weapon[1] = w2;

                            n.targetPos = new Vector2(mouseState.X, mouseState.Y);
                            n.activeWeapons = new int[2] { 0, 1 };
                        }
                    }
                    if (GamePadState.IsButtonDown(Buttons.B) && oldGamePadState.IsButtonUp(Buttons.B))
                    {
                        // spawn jeep
                        NPC n = NPC.newNPC(1, new Vector2(screenPosition.X + screenWidth + 20, screenHeight / 2) + screenPosition);
                        if (n != null)
                        {
                            Weapon w = new Weapon();
                            w.active = true;
                            w.type = 0;
                            w.owner = n.whoAmI;
                            Defaults.setDefaults(w);
                            w.ownedByNPC = true;
                            n.weapon[0] = w;

                            Weapon w2 = new Weapon();
                            w2.active = true;
                            w2.type = 2;
                            w2.owner = n.whoAmI;
                            Defaults.setDefaults(w2);
                            w2.ownedByNPC = true;
                            n.weapon[1] = w2;

                            n.targetPos = new Vector2(mouseState.X, mouseState.Y);
                            n.activeWeapons = new int[2] { 0, 1 };
                        }
                    }
                    if (GamePadState.IsButtonDown(Buttons.X) && oldGamePadState.IsButtonUp(Buttons.X))
                    {
                        player[myPlayer].engineOn = !player[myPlayer].engineOn;
                    }
                    if (GamePadState.IsButtonDown(Buttons.A) && oldGamePadState.IsButtonUp(Buttons.A))
                    {
                        for (int i = 0; i < npc.Length; i++)
                        {
                            if (npc[i].active)
                            {
                                npc[i].life = 0;
                            }
                        }
                    }
                    #endregion
                    if (!isPaused)
                    {
                        // Shooting
                        if (GamePadState.IsButtonDown(Buttons.RightTrigger))
                        {
                            player[myPlayer].shooting = true;
                        }
                        else
                        {
                            player[myPlayer].shooting = false;
                        }
                        // Movement
                        if (GamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                        {
                            player[myPlayer].controlX = -1;
                        }
                        else if (GamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
                        {
                            player[myPlayer].controlX = 1;
                        }
                        else player[myPlayer].controlX = 0;
                        if (GamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
                        {
                            player[myPlayer].controlY = -1;
                        }
                        else if (GamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
                        {
                            player[myPlayer].controlY = 1;
                        }
                        else player[myPlayer].controlY = 0;

                        // Rotate target
                        float rot = (float) Math.Atan2(GamePadState.ThumbSticks.Right.Y, -GamePadState.ThumbSticks.Right.X);
                        if (GamePadState.ThumbSticks.Right.Length() > 0.1f)
                            player[myPlayer].pointerRot = rot;
                        Debug.constantText[3] = rot.ToString();

                        if (GamePadState.IsButtonDown(Buttons.RightShoulder))
                        {
                            player[myPlayer].pointerDist++;
                        }
                        else if (GamePadState.IsButtonDown(Buttons.LeftShoulder))
                        {
                            player[myPlayer].pointerDist--;
                        }
                        player[myPlayer].pointerDist = MathHelper.Clamp(player[myPlayer].pointerDist, 100, 500);
                    }
                }
            }
            if (inGame)
            {
                if (!isPaused)
                {
                    if (camOnPlayer)
                    {
                        screenPosition = player[myPlayer].position - new Vector2(screenWidth / 2, screenHeight / 2);
                        if (screenPosition.X < worldBoundsLeft) screenPosition.X = worldBoundsLeft;
                        if (screenPosition.Y < worldBoundsUp) screenPosition.Y = worldBoundsUp;
                        if (screenPosition.X + screenWidth > worldBoundsRight) screenPosition.X = worldBoundsRight - screenWidth;
                        if (screenPosition.Y + screenHeight > worldBoundsDown) screenPosition.Y = worldBoundsDown - screenHeight;
                    }
                }
            }
        }

        public static void spawnPlayer()
        {
            Player p = Player.newPlayer(playerSpawnPos);

            Weapon w = new Weapon();
            w.active = true;
            w.type = 0;
            w.owner = p.whoAmI;
            Defaults.setDefaults(w);
            p.weapon[0] = w;

            Weapon w2 = new Weapon();
            w2.active = true;
            w2.type = 2;
            w2.owner = p.whoAmI;
            Defaults.setDefaults(w2);
            p.weapon[1] = w2;

            p.fuel = p.maxFuel;

            myPlayer = p.whoAmI;

            Structure s = Structure.newStructure(1, (int) p.position.X);
        }

        public static void respawnPlayer(int p)
        {
            player[p].active = true;
            player[p].life = player[p].maxLife;
            player[p].fuel = player[p].maxFuel;
            player[p].position = playerSpawnPos;
            player[p].dead = false;
            player[p].rotationVel = 0f;
            player[p].rotation = 0f;
            player[p].velocity = Vector2.Zero;
        }

        public static void startGame()
        {
            playerSpawnPos = new Vector2(random.Next(100, worldBoundsRight-100), worldBoundsDown - (structureTexture[1].Height + playerTexture.Height));
            for (int i = 0; i < player.Length; i++) player[i].active = false;
            Dust.ClearDust();
            Projectile.ClearProjectiles();
            for (int i = 0; i < npc.Length; i++) npc[i].active = false;
            spawnPlayer();
            inGame = true;
        }

        public static void endGame()
        {
            inGame = false;
            isPaused = false;
            curMenu = 0;
            for (int i = 0; i < player.Length; i++) player[i].active = false;
            Dust.ClearDust();
            Projectile.ClearProjectiles();
            for (int i = 0; i < npc.Length; i++) npc[i].active = false;
        }

        public static void drawMenu()
        {
            if (curMenu == 0 || curMenu == 2)
            {
                foreach (Dust d in dust)
                {
                    if (d.active)
                    {
                        spriteBatch.Draw(
                            dustTexture[d.type],
                            new Rectangle((int) (d.position.X - screenPosition.X), (int) (d.position.Y - screenPosition.Y), (int) (d.width * d.scale), (int) (d.height * d.scale)),
                            null,
                            Color.White * d.alpha,
                            d.rotation,
                            new Vector2(d.width / 2, d.height / 2),
                            SpriteEffects.None,
                            0f);
                    }
                }
                foreach (Projectile p in projectile)
                {
                    if (p.active)
                    {
                        spriteBatch.Draw(
                            projectileTexture[p.type],
                            new Rectangle((int) (p.position.X - screenPosition.X), (int) (p.position.Y - screenPosition.Y), p.width, p.height),
                            null,
                            Color.White,
                            p.rotation,
                            new Vector2(p.width / 2, p.height / 2),
                            SpriteEffects.None,
                            0f);
                    }
                }
            }
            if (curMenu == 0)
            {
                spriteBatch.Draw(logoTexture, new Rectangle((screenWidth / 2) - 242, 50, 484, 414), Color.White);

                foreach (Button b in MainMenuButtons) b.draw();
            }
            else if (curMenu == 1)
            {
                foreach (Button b in customizeMenuButtons) b.draw();
            }
            else if (curMenu == 2)
            {
                if (usingGamepad)
                {
                    spriteBatch.Draw(checkTexture, new Rectangle(optionsButtons[0].rect.X-64, optionsButtons[0].rect.Y, 64, 67), null, Color.White);
                }
                else
                {
                    spriteBatch.Draw(checkTexture, new Rectangle(optionsButtons[1].rect.X-64, optionsButtons[1].rect.Y, 64, 67), null, Color.White);
                }
                foreach (Button b in optionsButtons) b.draw();
            }
        }

        public static void drawPauseMenu()
        {
            spriteBatch.Draw(nullTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * 0.5f);
            foreach (Button b in pauseMenuButtons) b.draw();
        }

        public static void drawGame()
        {
            Vector2 camPosMod = screenPosition + camShake;

            for (int x = -500; x < worldBoundsRight+500; x += 500)
            {
                if (x + 500 > (screenPosition.X+camShake.X) && x < (screenPosition.X+camShake.X) + screenWidth)
                {
                    spriteBatch.Draw(backgroundTexture[0], new Rectangle(x - (int) camPosMod.X, ((worldBoundsDown - groundHeight) - 500) - (int) camPosMod.Y, 500, 500), Color.White);
                }
            }

            int sX = (int) camPosMod.X;
            int sY = (int) camPosMod.Y;
            spriteBatch.Draw(groundTexture, new Rectangle((worldBoundsLeft - 100) - (int) camPosMod.X, worldBoundsDown-groundHeight - (int) camPosMod.Y, (worldBoundsRight - worldBoundsLeft) + 200, 228), new Rectangle(0, 0, 64, 64), Color.White);

            foreach (Structure s in structure)
            {
                if (s.active)
                {
                    spriteBatch.Draw(
                        structureTexture[s.type],
                        new Rectangle((int)s.position.X - sX, (int)s.position.Y - sY, s.width, s.height),
                        s.frame,
                        Color.White,
                        0f,
                        new Vector2(s.width / 2, s.height / 2),
                        SpriteEffects.None,
                        0f);
                }
            }

            foreach (NPC n in npc)
            {
                if (n.active)
                {
                    n.draw();
                }
            }

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].active)
                {
                    SpriteEffects effects = SpriteEffects.None;

                    spriteBatch.Draw(
                        playerTexture,
                        new Rectangle((int) player[i].position.X - sX, (int) player[i].position.Y - sY, player[i].width, player[i].height),
                        new Rectangle(32, 0, 165, 50),
                        Color.White,
                        player[i].rotation,
                        new Vector2(player[i].width / 2, player[i].height / 2),
                        effects,
                        0f);

                    double theta = Math.Atan2(23, -18);
                    int MainRotorX = (int) (player[i].position.X - (Math.Cos(player[i].rotation + theta) * 29));
                    int MainRotorY = (int) (player[i].position.Y - (Math.Sin(player[i].rotation + theta) * 29));

                    spriteBatch.Draw(
                        nullTexture,
                        new Rectangle(MainRotorX - sX, MainRotorY - sY, player[i].rotorSize, 1),
                        new Rectangle(31, 0, 165, 50),
                        Color.Black,
                        player[i].rotation,
                        new Vector2(player[i].rotorSize / 2, 0),
                        effects,
                        0f);

                    theta = Math.Atan2(16, 74);
                    int tailRotorX = (int) (player[i].position.X - (Math.Cos(player[i].rotation + theta) * 75));
                    int tailRotorY = (int) (player[i].position.Y - (Math.Sin(player[i].rotation + theta) * 75));

                    spriteBatch.Draw(
                        playerTexture,
                        new Rectangle(tailRotorX - sX, tailRotorY - sY, 30, 30),
                        new Rectangle(0, 0, 30, 30),
                        Color.Black,
                        player[i].tailRotorRotation,
                        new Vector2(15, 15),
                        effects,
                        0f);

                    for (int j = 0; j < player[i].weapon.Length; j++)
                    {
                        Weapon w = player[i].weapon[j];
                        if (w.active)
                        {
                            int x = (int) (player[i].position.X + w.posOnOwner.X) - sX;
                            int y = (int) (player[i].position.Y + w.posOnOwner.Y) - sY;
                            spriteBatch.Draw(
                                weaponTexture[w.type],
                                new Rectangle(x, y, w.width, w.height),
                                null,
                                Color.White,
                                w.rotation,
                                w.origin,
                                SpriteEffects.None,
                                0f);
                        }
                    }
                    if (i != myPlayer)
                    {
                        effects = SpriteEffects.None;

                        spriteBatch.Draw(
                            playerTexture,
                            new Rectangle((int) player[i].position.X - sX, (int) player[i].position.Y - sY, player[i].width, player[i].height),
                            new Rectangle(31, 0, 165, 50),
                            Color.White,
                            player[i].rotation,
                            new Vector2(player[i].width / 2, player[i].height / 2),
                            effects,
                            0f);

                        theta = Math.Atan2(23, -18);
                        MainRotorX = (int) (player[i].position.X - (Math.Cos(player[i].rotation + theta) * 29));
                        MainRotorY = (int) (player[i].position.Y - (Math.Sin(player[i].rotation + theta) * 29));

                        spriteBatch.Draw(
                            nullTexture,
                            new Rectangle(MainRotorX - sX, MainRotorY - sY, player[i].rotorSize, 1),
                            new Rectangle(31, 0, 165, 50),
                            Color.Black,
                            player[i].rotation,
                            new Vector2(player[i].rotorSize / 2, 0),
                            effects,
                            0f);

                        theta = Math.Atan2(16, 74);
                        tailRotorX = (int) (player[i].position.X - (Math.Cos(player[i].rotation + theta) * 75));
                        tailRotorY = (int) (player[i].position.Y - (Math.Sin(player[i].rotation + theta) * 75));

                        spriteBatch.Draw(
                            playerTexture,
                            new Rectangle(tailRotorX - sX, tailRotorY - sY, 30, 30),
                            new Rectangle(0, 0, 30, 30),
                            Color.Black,
                            player[i].tailRotorRotation,
                            new Vector2(15, 15),
                            effects,
                            0f);

                        for (int j = 0; j < player[i].weapon.Length; j++)
                        {
                            Weapon w = player[i].weapon[j];
                            if (w.active)
                            {
                                int x = (int) (player[i].position.X + w.posOnOwner.X) - sX;
                                int y = (int) (player[i].position.Y + w.posOnOwner.Y) - sY;
                                spriteBatch.Draw(
                                    weaponTexture[w.type],
                                    new Rectangle(x, y, w.width, w.height),
                                    null,
                                    Color.White,
                                    w.rotation,
                                    w.origin,
                                    SpriteEffects.None,
                                    0f);
                            }
                        }
                    }
                }
            }

            foreach (Dust d in dust)
            {
                if (d.active)
                {
                    spriteBatch.Draw(
                        dustTexture[d.type],
                        new Rectangle((int) (d.position.X - screenPosition.X), (int) (d.position.Y - screenPosition.Y), (int) (d.width * d.scale), (int) (d.height * d.scale)),
                        null,
                        Color.White * d.alpha,
                        d.rotation,
                        new Vector2(d.width / 2, d.height / 2),
                        SpriteEffects.None,
                        0f);
                }
            }
            foreach (Projectile p in projectile)
            {
                if (p.active)
                {
                    spriteBatch.Draw(
                        projectileTexture[p.type],
                        new Rectangle((int) (p.position.X - screenPosition.X), (int) (p.position.Y - screenPosition.Y), p.width, p.height),
                        null,
                        Color.White,
                        p.rotation,
                        new Vector2(p.width / 2, p.height / 2),
                        SpriteEffects.None,
                        0f);
                }
            }

            // HUD //
            int it = 0;
            int startY = 300;

            // life 

            string txt = "Life " + player[myPlayer].life + " of " + player[myPlayer].maxLife;
            spriteBatch.DrawString(font[1], txt, new Vector2(120, 50), Color.White);

            int size = (int) (((double) player[myPlayer].life / (double) player[myPlayer].maxLife) * 100);

            spriteBatch.Draw(lifeCrossTexture, new Rectangle(5, 20 + (100 - size), 100, size), new Rectangle(0, 100 - size, 100, size), Color.White);

            // fuel

            txt = "Fuel " + (int) player[myPlayer].fuel + " of " + (int) player[myPlayer].maxFuel;
            spriteBatch.DrawString(font[1], txt, new Vector2(120, 200), Color.White);

            size = (int) (((double)player[myPlayer].fuel/(double)player[myPlayer].maxFuel)*115);

            spriteBatch.Draw(gasCanTexture, new Rectangle(5, 150+(115-size), 88, size), new Rectangle(0, 115-size, 88, size), Color.White);

            // weapons & ammo
            foreach (Weapon w in player[myPlayer].weapon)
            {
                if (w.active)
                {
                    spriteBatch.DrawString(font[2], w.name, new Vector2(10, startY + (it * 80)), Color.Black);
                    string text = "RELOAD";
                    if (w.reloadTimer > 0)
                    {
                        spriteBatch.Draw(nullTexture, new Rectangle(10, startY + (it * 80) + 50, (int) ((1 - ((double) w.reloadTimer / (double) w.reloadTime)) * 250), 25), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(nullTexture, new Rectangle(10, startY + (it * 80) + 50, (int) (((double) w.ammo / (double) w.maxAmmo) * 250), 25), Color.Black);
                        text = w.ammo + "/" + w.maxAmmo;
                    }
                    spriteBatch.DrawString(font[0], text, new Vector2(10, startY + (it * 80) + 55), Color.White);
                    it++;
                }
            }
        }
    }
}
