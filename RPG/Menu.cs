using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace RPG
{

    /// <summary>
    /// parent class
    /// </summary>
    public class Menu
    {
        public enum MenuState
        {
            MAIN,
            PLAY,
            OPTIONS,
            QUIT,
            GAMEMENU
        };

        protected MenuState state;
        protected SpriteFont font;
        protected List<Button> buttons;
        

        public virtual void Update(int mouseX, int mouseY, bool pressed, ref Player player, ref Tilemap tilemap)
        {
            foreach(Button button in buttons)
            {
                //if mouse is on the button 
                if(button.Position.Contains(mouseX, mouseY)  && button.Clr == Color.Yellow)
                {
                    button.Clr = Color.Red;
                    button.Text = button.Text.ToUpper();
                }
                // if mouse is not on the button
                else if (!button.Position.Contains(mouseX, mouseY)  && button.Clr == Color.Red)
                {
                    button.Clr = Color.Yellow;
                    button.Text = button.Text.ToLower();
                }
            }
        }

        public virtual void Draw(SpriteBatch sb, Texture2D buttonTexture)
        {
            foreach (Button button in buttons)
            {
                sb.Draw(buttonTexture, button.Position, button.Clr);
                sb.DrawString(font, button.Text, button.TextPosition, Color.Black);
            }
        }

        public MenuState GetMenuState()
        {
            return state;
        }

        //will need this for items menu
        public virtual void SetItems(ref Player player)
        { }
    }


    /// <summary>
    /// Main menu
    /// </summary>
    public class MainMenu : Menu
    {
        private bool SaveNotFound;

        public MainMenu(SpriteFont font)
        {
            this.font = font;
            state = MenuState.MAIN;

            buttons = new List<Button>();
            buttons.Add(new Button(new Rectangle(350, 200, 100, 30),
                 new Vector2(360,200), "play", Color.Yellow));
            buttons.Add(new Button(new Rectangle(350, 250, 100, 30),
                 new Vector2(360, 250), "load", Color.Yellow));
            buttons.Add(new Button(new Rectangle(350, 300, 100, 30),
                new Vector2(360, 300) ,"options", Color.Yellow));
            buttons.Add(new Button(new Rectangle(350, 350, 100, 30),
                new Vector2(360, 350), "quit", Color.Yellow));

            SaveNotFound = false;
        }

        public override void Update(int mouseX, int mouseY, bool pressed, ref Player player, ref Tilemap tilemap)
        {
            //if mouse button is pressed
            if (pressed)
            {
                foreach (Button button in buttons)
                {
                    //check if a button is pressed
                    if(button.Position.Contains(mouseX, mouseY))
                    {
                        //special cases for each button
                        switch(button.Text.ToLower())
                        {
                            case "play":
                                state = MenuState.PLAY;
                                if(SaveNotFound == true)
                                {
                                    SaveNotFound = false;
                                }
                                break;
                            case "load":
                                tilemap.LoadGame("Save.xml", ref player);
                                if (tilemap.SaveNotFound == false)
                                {
                                    state = MenuState.PLAY;
                                }
                                else
                                {
                                    SaveNotFound = true;
                                }
                                break;
                            case "options":
                                state = MenuState.OPTIONS;
                                if (SaveNotFound == true)
                                {
                                    SaveNotFound = false;
                                }
                                break;
                            case "quit":
                                state = MenuState.QUIT;
                                if (SaveNotFound == true)
                                {
                                    SaveNotFound = false;
                                }
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
            base.Update(mouseX, mouseY, pressed,ref player, ref tilemap);
        }

        public override void Draw(SpriteBatch sb, Texture2D buttonTexture)
        {
            sb.DrawString(font, "RPG", new Vector2(385,100), Color.Red);
            if(SaveNotFound == true)
            {

                sb.DrawString(font, "Save not found.", new Vector2(470, 250), Color.Red);
            }

            base.Draw(sb, buttonTexture);
        }
    }

    /// <summary>
    /// Options menu
    /// </summary>
    public class OptionsMenu : Menu
    {
        public OptionsMenu(SpriteFont font)
        {
            state = MenuState.OPTIONS;
            this.font = font;
            buttons = new List<Button>();
            buttons.Add(new Button(new Rectangle(350,200,100,30),
                new Vector2(360,200), "back",Color.Yellow));
        }


        public override void Update(int mouseX, int mouseY, bool pressed, ref Player player, ref Tilemap tilemap)
        {
            if(pressed)
            {
                foreach (Button button in buttons)
                {
                    if(button.Position.Contains(mouseX,mouseY))
                    {
                        switch(button.Text.ToLower())
                        {
                            case "back":
                                state = MenuState.MAIN;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            base.Update(mouseX, mouseY, pressed,ref player, ref tilemap);
        }

        public override void Draw(SpriteBatch sb, Texture2D buttonTexture)
        {
            sb.DrawString(font, "OPTIONS", new Vector2(385, 100), Color.Red);
            base.Draw(sb, buttonTexture);
        }

    }

    public class QuitMenu : Menu
    {
        public QuitMenu(SpriteFont font)
        {
            state = MenuState.QUIT;
            this.font = font;

            buttons = new List<Button>();
            buttons.Add(new Button(new Rectangle(270, 200, 100, 30),
                 new Vector2(305, 200), "yes", Color.Yellow));
            buttons.Add(new Button(new Rectangle(420, 200, 100, 30),
                new Vector2(460, 200), "no", Color.Yellow));
        }

        public override void Update(int mouseX, int mouseY, bool pressed, ref Player player, ref Tilemap tilemap)
        {
            if (pressed)
            {
                foreach (Button button in buttons)
                {
                    if (button.Position.Contains(mouseX, mouseY))
                    {
                        switch (button.Text.ToLower())
                        {
                            case "yes":
                                Game game = new Game();
                                game.Exit();
                                break;
                            case "no":
                                state = MenuState.MAIN;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            base.Update(mouseX, mouseY, pressed, ref player, ref tilemap);
        }

        public override void Draw(SpriteBatch sb, Texture2D buttonTexture)
        {
            sb.DrawString(font, "DO YOU REALLY WANT TO QUIT?", new Vector2(250, 100), Color.Red);
            sb.DrawString(font, "pls no", new Vector2(730,570), Color.Gray);
            base.Draw(sb, buttonTexture);
        }

    }





}
