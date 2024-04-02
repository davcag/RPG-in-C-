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
    public class GameMenu:Menu
    {
        enum GameMenuState
        {
            DEFAULT,
            ITEMS,
            STATS, 
            SAVE
        }

        private GameMenuState oldMenuState;
        private GameMenuState menuState;
        private int itemPage;

        

        private int health;
        private int maxHealth;
        private int attack;
        private int defense;
        private int level;
        private int exp;
        private int nextLevelExp;
        

        private List<Item> items;

        public GameMenu(SpriteFont font, List<Item> items)
        {
            this.font = font;
            state = MenuState.GAMEMENU;
            menuState = GameMenuState.DEFAULT;
            menuState = GameMenuState.DEFAULT;

            buttons = new List<Button>();

            buttons.Add(new Button(new Rectangle(300,150,200,30),new Vector2(380,150),"back",Color.Yellow));
            buttons.Add(new Button(new Rectangle(300, 200, 200, 30), new Vector2(380, 200), "stats", Color.Yellow));
            buttons.Add(new Button(new Rectangle(300, 250, 200, 30), new Vector2(380, 250), "items", Color.Yellow));
            buttons.Add(new Button(new Rectangle(300, 300, 200, 30), new Vector2(380, 300), "save", Color.Yellow));
            buttons.Add(new Button(new Rectangle(300, 350, 200, 30), new Vector2(380, 350), "exit", Color.Yellow));

            //items
            this.items = items;
            itemPage = 0;

        }

        public override void Update(int mouseX, int mouseY, bool pressed, ref Player player, ref Tilemap tilemap)
        {
            MenuChange();

            if (pressed)
            {
                if (menuState == GameMenuState.DEFAULT)
                {
                    foreach (Button button in buttons)
                    {

                        if (button.Position.Contains(mouseX, mouseY))
                        {
                            switch (button.Text.ToLower())
                            {
                                case "back":
                                    state = MenuState.PLAY;
                                    break;
                                case "stats":
                                    menuState = GameMenuState.STATS;
                                    level = player.Level;
                                    exp = player.Exp;
                                    nextLevelExp = player.NextLvlExp;
                                    health = player.Health;
                                    maxHealth = player.MaxHealth;
                                    attack = player.Attack;
                                    defense = player.Defense;
                                    break;
                                case "items":
                                    menuState = GameMenuState.ITEMS;
                                    break;
                                case "save":
                                    tilemap.SaveGame("Save.xml", player);
                                    menuState = GameMenuState.SAVE;
                                    break;
                                case "exit":
                                    state = MenuState.MAIN;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (menuState == GameMenuState.ITEMS)
                {
                    foreach (Button button in buttons)
                    {

                        if (button.Position.Contains(mouseX, mouseY))
                        {
                            switch (button.Text.ToLower())
                            {
                                case "leave":
                                    itemPage = 0;
                                    menuState = GameMenuState.DEFAULT;
                                    break;
                                case "next":
                                    itemPage++;
                                    break;
                                case "back":
                                    itemPage--;
                                    break;
                                default:
                                    if(Item.Use(button.Text, ref player))
                                    {
                                        items.Remove(items[itemPage*5+((button.Position.Y-200)/50)]);
                                    }
                                    break;
                            }
                        }
                    }

                }
                else if (menuState == GameMenuState.STATS)
                {
                    foreach (Button button in buttons)
                    {

                        if (button.Position.Contains(mouseX, mouseY))
                        {
                            switch (button.Text.ToLower())
                            {
                                case "back":
                                    menuState = GameMenuState.DEFAULT;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else if (menuState == GameMenuState.SAVE)
                {
                    foreach (Button button in buttons)
                    {

                        if (button.Position.Contains(mouseX, mouseY))
                        {
                            switch (button.Text.ToLower())
                            {
                                case "back":
                                    menuState = GameMenuState.DEFAULT;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }


            base.Update(mouseX, mouseY, pressed, ref player, ref tilemap);
        }

        public override void Draw(SpriteBatch sb, Texture2D buttonTexture)
        {
            sb.Draw(buttonTexture, new Rectangle(250, 100, 300, 400), new Color(Color.RosyBrown,0.85f));

          
            base.Draw(sb, buttonTexture);
            
        }

        public override void SetItems (ref Player player)
        {
            player.Items = this.items;
        }

        private void MenuChange()
        {
            if(menuState == GameMenuState.DEFAULT)
            {
                buttons.Clear();
                buttons.Add(new Button(new Rectangle(300, 150, 200, 30), new Vector2(380, 150), "back", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 200, 200, 30), new Vector2(380, 200), "stats", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 250, 200, 30), new Vector2(380, 250), "items", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 300, 200, 30), new Vector2(380, 300), "save", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 350, 200, 30), new Vector2(380, 350), "exit", Color.Yellow));
            }
            else if (menuState == GameMenuState.ITEMS)
            {
                buttons.Clear();
                buttons.Add(new Button(new Rectangle(300, 150, 200, 30), new Vector2(380, 150), "leave", Color.Yellow));
                int index = 0;
                //if there are more then 5 items
                if(items.Count>5)
                {
                    if(items.Count / 5 > itemPage)
                    {
                        for (int i = 5 * itemPage; i < 5 * (itemPage + 1); i++ )
                        {
                            buttons.Add(new Button(new Rectangle(300, 200 + (50 * (i%5)), 200, 30), new Vector2(380, 200 + (50 * (i%5))), items[i].Name, Color.Yellow));
                        }
                        //next buton
                        buttons.Add(new Button(new Rectangle(410, 450, 100, 30), new Vector2(430, 450), "next", Color.Yellow));
                    }
                    //if it's the last page
                    else if(items.Count/5 == itemPage)
                    {
                        int itemCount = items.Count - (itemPage * 5);
                        for (int i = 5 * itemPage; i < 5 * itemPage + itemCount; i++)
                        {
                            buttons.Add(new Button(new Rectangle(300, 200 + (50 * (i % 5)), 200, 30), new Vector2(380, 200 + (50 * (i % 5))), items[i].Name, Color.Yellow));
                        }

                    }
                    //back button
                    if (itemPage >0)
                    {
                        buttons.Add(new Button(new Rectangle(290, 450, 100, 30), new Vector2(310, 450), "back", Color.Yellow));
                    }
                }
                else
                {
                    foreach(Item item in items)
                    {
                        buttons.Add(new Button(new Rectangle(300, 200 + (50 * index), 200, 30), new Vector2(380, 200 + (50 * index)), item.Name, Color.Yellow));
                        index++;
                    }
                }
            }
            else if (menuState == GameMenuState.STATS)
            {
                buttons.Clear();
                buttons.Add(new Button(new Rectangle(300, 150, 200, 30), new Vector2(380, 150), "back", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 200, 200, 30), new Vector2(310, 200), "Level: " + level.ToString(), Color.RosyBrown));
                buttons.Add(new Button(new Rectangle(300, 250, 200, 30), new Vector2(310, 250), "Exp: " + exp.ToString() + " / " + nextLevelExp.ToString(), Color.RosyBrown));
                buttons.Add(new Button(new Rectangle(300, 300, 200, 30), new Vector2(310, 300), "HP: " + health.ToString() + " / " + maxHealth.ToString(), Color.RosyBrown));
                buttons.Add(new Button(new Rectangle(300, 350, 200, 30), new Vector2(310, 350), "Attack: " + attack.ToString(), Color.RosyBrown));
                buttons.Add(new Button(new Rectangle(300, 400, 200, 30), new Vector2(310, 400), "Defense: " + defense.ToString(), Color.RosyBrown));

            }
            else if (menuState == GameMenuState.SAVE)
            {
                buttons.Clear();
                buttons.Add(new Button(new Rectangle(300, 150, 200, 30), new Vector2(380, 150), "back", Color.Yellow));
                buttons.Add(new Button(new Rectangle(300, 200, 200, 30), new Vector2(310, 200), "Game saved.", Color.RosyBrown));
            }
            oldMenuState = menuState;
        }



    }
}
