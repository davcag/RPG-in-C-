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
    public class Battle
    {
        enum Turn
        {
            PLAYER,
            ATTACK,
            ENEMY
        }

        enum Menu
        {
            MAIN,
            ITEMS
        }

        enum ArrowDir
        {
            UPARROW = 0,
            DOWNARROW =1,
            RIGHTARROW =2,
            LEFTARROW =3
        }


        /// attacking
        bool attacking;
        int attackTimer;
        ///
        private Turn turn;
        private List<Button> buttons;
        private List<Button> itemButtons;
        private int buttonIndex;
        private int itemButtonIndex;
        private bool keyPressed;
        private Menu menu;
        private int itemsPage;


        //player
        private Player player;
        private float defModifier;
        private float attModifier;
        //enemy
        private Enemy enemy;

        //arrows
        class Arrow
        {
            public Arrow(ArrowDir dir)
            {
                Direction = dir;
                hitRect = new Rectangle(100+(64 * (int)Direction),400,64,64);
            }

            public Rectangle hitRect { get; set; }
            public ArrowDir Direction { get; set; }
        }
        int nHits;
        List<Rectangle> HitRect;
        List<Arrow> MovingArrows;
        bool missed;
        int arrowTime;

        //
        public bool BattleOver { get; private set; }
        public bool BattleWon { get; private set; }
        public bool BattleLost { get; private set; }
        
        
        public Battle(Player player, Enemy enemy)
        {
            this.player = player;
            this.enemy = enemy;
            defModifier = 1;
            attModifier = 1;

            menu = Menu.MAIN;
            turn = Turn.PLAYER;
            buttons = new List<Button>();
            itemButtons = new List<Button>();
            keyPressed = false;
            buttonIndex = 0;
            itemButtonIndex = 0;
            itemsPage = 0;
            buttons.Add(new Button(new Rectangle(50, 400, 100, 30),new Vector2(60,400),"Attack",Color.White));
            buttons.Add(new Button(new Rectangle(50, 450, 100, 30), new Vector2(60, 450), "Defend", Color.White));
            buttons.Add(new Button(new Rectangle(50, 500, 100, 30), new Vector2(60, 500), "Items", Color.White));
            buttons.Add(new Button(new Rectangle(50, 550, 100, 30), new Vector2(60, 550), "Run", Color.White));
            buttons[buttonIndex].Clr = Color.RosyBrown;
            UpdateItemButtons();

            //combo arrows
            nHits = 0;
            HitRect = new List<Rectangle>();
            for (int i = 0; i < 4; i++ )
            {
                HitRect.Add(new Rectangle(100 +(64*i), 130, 64, 4));
            }
            missed = false;
            arrowTime = 0;
            MovingArrows = new List<Arrow>();

            //attacking
            attacking = false;
            attackTimer = 0;
        }

        //have to return the player so the hp stays removed and items used
        public Player ReturnPlayer()
        {
            return player;
        }


        public void Update(GameTime time)
        {

            if (turn == Turn.PLAYER)
            {
                UpdatePlayer();
                if (enemy.Health<=0)
                {
                    BattleWon = true;
                    //will have to change it when i make enemies have exp
                    player.UpdateExp(10);
                    return;
                }
            }
            else if (turn == Turn.ENEMY)
            {
                UpdateEnemy();
                if (attacking == true)
                {
                    AttackTimeCountdown(time);
                }
                if (player.Health <= 0)
                {
                    BattleLost = true;
                    return;
                }
            }
            else if (turn == Turn.ATTACK)
            {
                if(!missed)
                {
                    if (MovingArrows.Count == 0)
                    {
                        CreateArrow();
                        arrowTime++;
                    }
                    else if (MovingArrows.Count >0)
                    {
                        foreach(Arrow arrow in MovingArrows)
                        {
                            //checks colission
                            int index = (int)arrow.Direction;
                            if (arrow.hitRect.Intersects(HitRect[index]) &&
                                (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.A) ||
                                Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.D)))
                            {
                                if(CheckArrowCollision(arrow))
                                {
                                    return;
                                }
                            }

                            //updates aroow position
                            if (arrow.hitRect.Y > 70)
                            {
                                float accel = nHits / 10.0f;
                                int y = (int)(arrow.hitRect.Y - time.ElapsedGameTime.Milliseconds/(2 - accel));
                                arrow.hitRect = new Rectangle(arrow.hitRect.X,
                                    (int)y, 64, 64);
                            }
                            else
                            {
                                MovingArrows.Clear();
                                missed = true;
                                Attack();
                                return;
                            }
                            
                        }
                    }
                }

                if (attacking == true)
                {
                    AttackTimeCountdown(time);
                }
            }
            
        }

        public void Draw(SpriteBatch sb, SpriteFont font, Texture2D buttonTexture, Texture2D playerImg, Texture2D enemyImg, Texture2D battleArrows)
        {
            //battle arrows
            sb.Draw(battleArrows, new Rectangle(100, 100, 256, 64), new Rectangle(0, 0, 256, 64), Color.White);
            sb.DrawString(font, "x" + nHits.ToString(), new Vector2(50, 100), Color.Red);
            foreach(Arrow arrow in MovingArrows)
            {
                sb.Draw(battleArrows, arrow.hitRect, new Rectangle(64 * (int)arrow.Direction, 64, 64, 64), Color.White);
            }


            //sprites
            if (turn == Turn.ENEMY && attacking == true)
            {
                Random rand = new Random();
                int displacement = rand.Next(25);

                sb.Draw(playerImg, new Vector2(250+displacement, 400+displacement), Color.White);
            }
            else
            {
                sb.Draw(playerImg, new Vector2(250, 400), Color.White);
            }
            if (turn == Turn.PLAYER && attacking == true)
            {
                Random rand = new Random();
                int displacement = rand.Next(25);

                sb.Draw(enemyImg, new Vector2(500+displacement, 150+displacement), Color.White);
            }
            else
            {
                sb.Draw(enemyImg, new Vector2(500, 150), Color.White);
            }

            //health bar
            //player
            sb.Draw(buttonTexture, new Rectangle(230, 370, 106, 16), Color.Black);
            sb.Draw(buttonTexture, new Rectangle(233, 373, (100/player.MaxHealth)*player.Health, 10), Color.Red);
            //enemy
            sb.Draw(buttonTexture, new Rectangle(470, 120, 106, 16), Color.Black);
            sb.Draw(buttonTexture, new Rectangle(473, 123, (100 / enemy.MaxHealth) * enemy.Health, 10), Color.Red);

            //buttons
            foreach (Button button in buttons)
            {
                sb.Draw(buttonTexture, button.Position, button.Clr);
                sb.DrawString(font, button.Text, button.TextPosition, Color.Black);
            }

            if(menu == Menu.ITEMS)
            {
                foreach (Button item in itemButtons)
                {
                     sb.Draw(buttonTexture, item.Position, item.Clr);
                     sb.DrawString(font, item.Text, item.TextPosition, Color.Black);
                } 
                if(itemsPage >0)
                {
                    sb.Draw(buttonTexture, new Rectangle(170,350,40,30), Color.Yellow);
                    sb.DrawString(font,"<-",new Vector2(180,350),Color.Black);
                }
                if (player.Items.Count % 4 > 0)
                {
                    if (itemsPage <= (itemButtons.Count / 4) + 1)
                    {
                        sb.Draw(buttonTexture, new Rectangle(220, 350, 40, 30), Color.Yellow);
                        sb.DrawString(font, "->", new Vector2(230, 350), Color.Black);
                    }
                        
                }
                else if(itemsPage<=itemButtons.Count/4)
                {
                    sb.Draw(buttonTexture, new Rectangle(220,350,40,30), Color.Yellow);
                    sb.DrawString(font, "->", new Vector2(230,350), Color.Black);
                }
            }
        } 

        private void UpdatePlayer()
        {
            if (menu == Menu.MAIN)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.S) && keyPressed == false)
                {
                    if (buttonIndex < buttons.Count - 1)
                    {
                        buttons[buttonIndex].Clr = Color.White;
                        buttonIndex++;
                        buttons[buttonIndex].Clr = Color.RosyBrown;
                        keyPressed = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.W) && keyPressed == false)
                {
                    if (buttonIndex > 0)
                    {
                        buttons[buttonIndex].Clr = Color.White;
                        buttonIndex--;
                        buttons[buttonIndex].Clr = Color.RosyBrown;
                        keyPressed = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.E) && keyPressed == false)
                {
                    keyPressed = true;
                    switch (buttons[buttonIndex].Text)
                    {
                        case "Attack":
                            turn = Turn.ATTACK;
                            break;
                        case "Defend":
                            Defend();
                            break;
                        case "Items":
                            Items();
                            break;
                        case "Run":
                            Run();
                            break;
                        default:
                            break;
                    }
                }
                else if (keyPressed == true &&
                    Keyboard.GetState().IsKeyUp(Keys.S) &&
                    Keyboard.GetState().IsKeyUp(Keys.W) &&
                    Keyboard.GetState().IsKeyUp(Keys.E))
                {
                    keyPressed = false;
                }
            }
            else if(menu == Menu.ITEMS)
            {


                if(Keyboard.GetState().IsKeyDown(Keys.F) && keyPressed == false)
                {
                    menu = Menu.MAIN;
                    keyPressed = true;
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.W) && keyPressed == false)
                {
                    if(itemButtonIndex > 0)
                    {
                        itemButtons[itemButtonIndex].Clr = Color.White;
                        itemButtonIndex--;
                        itemButtons[itemButtonIndex].Clr = Color.RosyBrown;
                        keyPressed = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S) && keyPressed == false)
                {
                    if (itemButtonIndex < itemButtons.Count-1)
                    {
                        itemButtons[itemButtonIndex].Clr = Color.White;
                        itemButtonIndex++;
                        itemButtons[itemButtonIndex].Clr = Color.RosyBrown;
                        keyPressed = true;
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A) && keyPressed == false)
                {
                    if (itemsPage >0)
                    {
                        keyPressed = true;
                        itemsPage--;
                        UpdateItemButtons();
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D) && keyPressed == false)
                {
                    if(player.Items.Count%4 >0)
                    {
                        if (itemsPage < player.Items.Count/4)
                        {
                            keyPressed = true;
                            itemsPage++;
                            UpdateItemButtons();
                        }
                    }
                    else if (itemsPage < (player.Items.Count/4)-1)
                    {
                        keyPressed = true;
                        itemsPage++;
                        UpdateItemButtons();
                    }
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.E) && keyPressed == false)
                {
                    keyPressed = true;
                    // use an item then remove it
                    Item.Use(itemButtons[itemButtonIndex].Text,ref player,ref enemy);
                    player.Items.Remove(player.Items[itemButtonIndex]);
                    UpdateItemButtons();
                    itemButtons[itemButtonIndex].Clr = Color.White;
                    itemButtonIndex = 0;
                    itemButtons[itemButtonIndex].Clr = Color.RosyBrown;
                    if (itemButtons.Count > 0)
                    {
                        itemButtons[itemButtonIndex].Clr = Color.RosyBrown;
                    }
                    menu = Menu.MAIN;
                    turn = Turn.ENEMY;
                }
                else if (keyPressed == true &&
                    Keyboard.GetState().IsKeyUp(Keys.F) &&
                    Keyboard.GetState().IsKeyUp(Keys.W) &&
                    Keyboard.GetState().IsKeyUp(Keys.S) &&
                    Keyboard.GetState().IsKeyUp(Keys.A) &&
                    Keyboard.GetState().IsKeyUp(Keys.D) &&
                    Keyboard.GetState().IsKeyUp(Keys.E)) 
                {
                    keyPressed = false;
                }
            }
        }

        private void UpdateEnemy()
        {
            if (attacking == false)
            {
                int dmg = (int)(enemy.Attack - ((player.Defense * defModifier) / 2));
                if (dmg < 1)
                {
                    dmg = 1;
                }
                player.Health -= dmg;
            }
            attacking = true;
        }

        private void Attack()
        {
            int dmg = 0;
            if (turn == Turn.ATTACK)
            {
                dmg = (int)((player.Attack * attModifier * (0.25 * nHits)) - (enemy.Defense / 2));
            }
            else if (turn == Turn.ENEMY)
            {
                dmg = (int)((player.Attack * attModifier - (enemy.Defense / 2)));
            }
            
            if (dmg < 1)
            {
                dmg = 1;
            }
            enemy.Health-= dmg;
            nHits = 0;
            attacking = true;
        }

        private void Defend()
        {
            defModifier += 0.25f;
            turn = Turn.ENEMY;
        }

        private void Items()
        {
            if (itemButtons.Count > 0)
            {
                menu = Menu.ITEMS;
            }
        }

        private void Run()
        {
            Random rand = new Random();
            int index = rand.Next(10);
            if(index >= 5)
            {
                BattleOver = true;
                return;
            }
            turn = Turn.ENEMY;
        }

        private void UpdateItemButtons()
        {
            itemButtons.Clear();

            if (player.Items.Count > 4)
            {
                if (player.Items.Count/4 == itemsPage)
                {
                    int itemCount = player.Items.Count - (itemsPage * 4);
                    for (int i = 4 * itemsPage; i < 4 * itemsPage + itemCount; i++)
                    {
                        itemButtons.Add(new Button(new Rectangle(170, 400 + 50 * (i % 4), 100, 30),
                            new Vector2(180, 400 + 50 * (i%4)), player.Items[i].Name, Color.White));
                    }

                }
                else
                {
                    for (int i = 0 + (4 * itemsPage); i < 4 + (4 * itemsPage); i++)
                    {
                        itemButtons.Add(new Button(new Rectangle(170, 400 + 50 * (i % 4), 100, 30),
                            new Vector2(180, 400 + 50 * (i%4)), player.Items[i].Name, Color.White));
                    }
                }
            }
            else
            {
                for (int i = 0; i < player.Items.Count; i++)
                {
                    itemButtons.Add(new Button(new Rectangle(170, 400 + 50 * (i % 4), 100, 30),
                        new Vector2(180, 400 + 50 * (i%4)), player.Items[i].Name, Color.White));
                }
            }

            if (itemButtons.Count > 0)
            {
                itemButtons[itemButtonIndex].Clr = Color.RosyBrown;
            }
        }

        private void CreateArrow()
        {
            Random rand = new Random();
            int number = rand.Next(4);
            MovingArrows.Add(new Arrow((ArrowDir)number));

        }

        private bool CheckArrowCollision(Arrow arrow)
        {
            switch (arrow.Direction)
            {
                case ArrowDir.UPARROW:
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        nHits++;
                        MovingArrows.Remove(arrow);
                        return true;
                    }
                    break;
                case ArrowDir.DOWNARROW:
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        nHits++;
                        MovingArrows.Remove(arrow);
                        return true;
                    }
                    break;
                case ArrowDir.RIGHTARROW:
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        nHits++;
                        MovingArrows.Remove(arrow);
                        return true;
                    }
                    break;
                case ArrowDir.LEFTARROW:
                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        nHits++;
                        MovingArrows.Remove(arrow);
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        private void AttackTimeCountdown(GameTime time)
        {
            attackTimer += time.ElapsedGameTime.Milliseconds;
            if (attackTimer > 250)
            {
                attackTimer = 0;
                attacking = false;
                if (turn == Turn.ATTACK)
                {
                    turn = Turn.ENEMY;
                    missed = false;
                }
                else if (turn == Turn.ENEMY)
                {
                    turn = Turn.PLAYER;
                }
            }
        }
    }

}
