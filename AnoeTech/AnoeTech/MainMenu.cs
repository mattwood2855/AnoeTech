using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace AnoeTech
{
    public class MainMenu
    {
        Texture2D mainBackground, optionsBackground, colorBlast;
        Texture2D[] selectionGraphics, arrowGraphics, optionsGraphics, audioGraphics, controlsGraphics, videoGraphics;
        Song[] playlist;
        int currentSong = 0;
        int currentSelection = (int)MenuItems.NEW_GAME;
        SoundEffect clickSound, gunShotSound;
        public SoundEffectInstance menuClick, menuGunShot;
        float faderAlpha = 0;
        MenuPages currentMenu = MenuPages.MAIN;
        bool runningOnGame = false;

        enum MenuPages { MAIN, TRANSTOOPTIONS, TRANSTOMAIN, OPTIONS, AUDIO, VIDEO, CONTROLS };
        enum MenuItems { RESUME, NEW_GAME, OPTIONS, QUIT}
        enum OptionsItems { AUDIO, VIDEO, CONTROLS, BACK };

        public void Draw()
        {
            switch( currentMenu )
            {
                case MenuPages.MAIN:
                    GraphicsEngine.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    GraphicsEngine.spriteBatch.Draw(mainBackground, new Rectangle(0, 0, GraphicsEngine.viewport.Width, GraphicsEngine.viewport.Height), Color.White);
                    GraphicsEngine.spriteBatch.Draw(selectionGraphics[currentSelection], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.68f), (int)(GraphicsEngine.viewport.Height * 0.4f), (320 * GraphicsEngine.viewport.Width/1200), (50 * GraphicsEngine.viewport.Height/800)), Color.White);

                        if (!runningOnGame)
                        {
                            if (currentSelection > 1)
                                GraphicsEngine.spriteBatch.Draw(arrowGraphics[0], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.68f) + 70, (int)(GraphicsEngine.viewport.Height * 0.4f) - 30, 50, 30), Color.White * (float)Math.Sin(faderAlpha));
                        }
                        else
                        {
                            if (currentSelection > 0)
                                GraphicsEngine.spriteBatch.Draw(arrowGraphics[0], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.68f) + 70, (int)(GraphicsEngine.viewport.Height * 0.4f) - 30, 50, 30), Color.White * (float)Math.Sin(faderAlpha));
                        }
                        if (currentSelection < selectionGraphics.Length - 1)
                            GraphicsEngine.spriteBatch.Draw(arrowGraphics[1], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.68f) + 70, (int)(GraphicsEngine.viewport.Height * 0.4f) + 30, 50, 30), Color.White * (float)Math.Sin(faderAlpha));
                        GraphicsEngine.spriteBatch.End();
                    break;


                case MenuPages.OPTIONS:

                    GraphicsEngine.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    GraphicsEngine.spriteBatch.Draw(optionsBackground, new Rectangle(0, 0, GraphicsEngine.viewport.Width, GraphicsEngine.viewport.Height), Color.White);
                    GraphicsEngine.spriteBatch.Draw(colorBlast, new Rectangle((int)(GraphicsEngine.viewport.Width * 0.3f), (int)(GraphicsEngine.viewport.Height * 0.31f), 420, 250), Color.White);
                        

                        if(currentSelection == (int)OptionsItems.AUDIO)
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.BACK + 1], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f), 320, 50), Color.White);
                        else
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.AUDIO], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f), 320, 50), Color.White);
                        
                        if (currentSelection == (int)OptionsItems.VIDEO)
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.BACK + 2], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f) + 60, 320, 50), Color.White);
                        else
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.VIDEO], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f) + 60, 320, 50), Color.White);

                        if (currentSelection == (int)OptionsItems.CONTROLS)
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.BACK + 3], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f) + 120, 320, 50), Color.White);
                        else
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.CONTROLS], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.60f), (int)(GraphicsEngine.viewport.Height * 0.4f) + 120, 320, 50), Color.White);

                        if (currentSelection == (int)OptionsItems.BACK)
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.BACK + 4], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.80f), (int)(GraphicsEngine.viewport.Height * 0.9f), 320, 50), Color.White);
                        else
                            GraphicsEngine.spriteBatch.Draw(optionsGraphics[(int)OptionsItems.BACK], new Rectangle((int)(GraphicsEngine.viewport.Width * 0.80f), (int)(GraphicsEngine.viewport.Height * 0.9f), 320, 50), Color.White);

                        GraphicsEngine.spriteBatch.End();
                    break;
            }
        }

        public void Initiate()
        {
            mainBackground = GameState.anoetech.Content.Load<Texture2D>("Menus/MainMenu");
            selectionGraphics = new Texture2D[4];
            //selectionGraphics[(int)MenuItems.PLAY_GAME] = _engine.Content.Load<Texture2D>("Menus/PlayGame");
            selectionGraphics[(int)MenuItems.RESUME] = GameState.contentManager.Load<Texture2D>("Menus/Resume");
            selectionGraphics[(int)MenuItems.NEW_GAME] = GameState.contentManager.Load<Texture2D>("Menus/NewGame");
            selectionGraphics[(int)MenuItems.OPTIONS] = GameState.contentManager.Load<Texture2D>("Menus/Options");
            selectionGraphics[(int)MenuItems.QUIT] = GameState.contentManager.Load<Texture2D>("Menus/Quit");

            optionsBackground = GameState.anoetech.Content.Load<Texture2D>("Menus/OptionsMenu2");
            colorBlast = GameState.anoetech.Content.Load<Texture2D>("Menus/colorBlast");
            
            optionsGraphics = new Texture2D[8];
            optionsGraphics[(int)OptionsItems.AUDIO] = GameState.contentManager.Load<Texture2D>("Menus/Audio");
            optionsGraphics[(int)OptionsItems.VIDEO] = GameState.contentManager.Load<Texture2D>("Menus/Video");
            optionsGraphics[(int)OptionsItems.CONTROLS] = GameState.contentManager.Load<Texture2D>("Menus/Controls");
            optionsGraphics[(int)OptionsItems.BACK] = GameState.contentManager.Load<Texture2D>("Menus/Back");
            optionsGraphics[(int)OptionsItems.BACK + 1] = GameState.contentManager.Load<Texture2D>("Menus/AudioS");
            optionsGraphics[(int)OptionsItems.BACK + 2] = GameState.contentManager.Load<Texture2D>("Menus/VideoS");
            optionsGraphics[(int)OptionsItems.BACK + 3] = GameState.contentManager.Load<Texture2D>("Menus/ControlsS");
            optionsGraphics[(int)OptionsItems.BACK + 4] = GameState.contentManager.Load<Texture2D>("Menus/BackS");

            arrowGraphics = new Texture2D[2];
            arrowGraphics[0] = GameState.contentManager.Load<Texture2D>("Menus/upArrow");
            arrowGraphics[1] = GameState.contentManager.Load<Texture2D>("Menus/downArrow");

            clickSound = GameState.contentManager.Load<SoundEffect>("SFX/menuClick");
            menuClick = clickSound.CreateInstance();
            gunShotSound = GameState.contentManager.Load<SoundEffect>("SFX/gunShot2");
            menuGunShot = gunShotSound.CreateInstance();

            playlist = new Song[2];
            playlist[0] = GameState.contentManager.Load<Song>("Music/iceCreamMusic1");
            playlist[1] = GameState.contentManager.Load<Song>("Music/iceCreamMusic2");
        }

        private void TransitionToOptions()
        {

        }

        public void Update()
        {

            faderAlpha += 0.05f;
            if (faderAlpha > MathHelper.Pi)
                faderAlpha = 0;

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Volume = 0.3f;
                MediaPlayer.Play(playlist[currentSong]);
                currentSong++;
                if (currentSong > playlist.Length - 1)
                    currentSong = 0;
            }

            

            switch (currentMenu)
            {
                case MenuPages.MAIN:

                    if (InputHandler.keyboardState.IsKeyDown(Keys.Up))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Up))
                            if (currentSelection > 0)
                            {
                                menuClick.Play();
                                currentSelection--;
                                if (currentSelection == 0 && !runningOnGame)
                                    currentSelection = 1;
                            }
                    if (InputHandler.keyboardState.IsKeyDown(Keys.Down))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Down))
                            if (currentSelection < selectionGraphics.Length - 1)
                            {
                                menuClick.Play();
                                currentSelection++;
                            }
                    if (InputHandler.keyboardState.IsKeyDown(Keys.Enter))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        if (currentSelection == (int)MenuItems.RESUME)
                        {
                            MediaPlayer.Stop();
                            menuGunShot.Play();
                            GameState.ChangeEngineState(GameState.EngineStates.RUNNING);
                            
                        }
                        if (currentSelection == (int)MenuItems.NEW_GAME)
                        {
                            MediaPlayer.Stop();
                            menuGunShot.Play();
                            if (runningOnGame)
                                GameState.anoetech.DestroyCurrentGame();
                            else
                                runningOnGame = true;

                            currentSelection = (int)MenuItems.RESUME;
                            GameState.ChangeEngineState(GameState.EngineStates.LOADING);
                        }

                        if (currentSelection == (int)MenuItems.OPTIONS)
                        {
                            //Transition to options
                            menuGunShot.Play();
                            currentMenu = MenuPages.OPTIONS;
                            currentSelection = (int)OptionsItems.AUDIO;
                        }

                        if (currentSelection == (int)MenuItems.QUIT)
                            GameState.anoetech.Exit();

                    }
                    break;

                case MenuPages.OPTIONS:
                    if (InputHandler.keyboardState.IsKeyDown(Keys.Up))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Up))
                            if (currentSelection > 0)
                            {
                                menuClick.Play();
                                currentSelection--;
                            }
                    if (InputHandler.keyboardState.IsKeyDown(Keys.Down))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Down))
                            if (currentSelection < optionsGraphics.Length/2 - 1)
                            {
                                menuClick.Play();
                                currentSelection++;
                            }

                    if (InputHandler.keyboardState.IsKeyDown(Keys.Enter))
                        if (!InputHandler.oldKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        if (currentSelection == (int)OptionsItems.BACK)
                        {
                            menuGunShot.Play();
                            currentMenu = MenuPages.MAIN;
                            currentSelection = (int)MenuItems.OPTIONS;
                        }
                    }
                    break;

            }
        }
    }
}
