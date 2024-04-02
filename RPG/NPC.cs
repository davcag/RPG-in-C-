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
    public class NPC:Sprite
    {
        private Vector2 startPos;
        private int size = 60;
        private bool changedDir;
        private int speed;
        public int Distance { get; private set; }
        public string Text { get; private set; }
        private int textTimer;
        public string Name { get; private set; }

        public bool Talking { get; set; }

        public NPC(string name,Vector2 pos, Sprite.Direction dir, int sp, int dist, string txt)
        {
            this.Name = name;
            startPos = pos;
            Position = new Rectangle((int)pos.X, (int)pos.Y, size, size);
            clr = Color.White;
            MoveDir = dir;
            if (dist > 0)
            {
                Moving = true;
            }
            else
            {
                Moving = false;
            }
            rows = 4;
            cols = 3;
            lastFrame = 3;
            frameTime = 0;
            curFrame = 1;
            changedDir = false;
            speed = sp;
            Distance = dist;
            Text = txt;
            textTimer = 0;
            Talking = false;
        }

        /// <summary>
        /// Uses the parent function to update the animation
        /// </summary>
        public void Update(GameTime time, Tilemap tm)
        {
            //Updates the position
            if (Moving == true)
            {
                Move(tm);
            }
            //handles talking
            if(Talking)
            {
                if (textTimer<2000)
                {
                    textTimer += time.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    textTimer = 0;
                    Talking = false;
                }
            }
            //Updates the animation
            base.Update(time);
        }


        /// <summary>
        /// Uses parent function to draw the sprite
        /// and draws the talk bog with the text
        /// </summary>
        public void Draw(SpriteBatch sb, Texture2D spriteTexture, Texture2D textBG, SpriteFont font, int scrollX , int scrollY)
        {
            if(Talking)
            {
                //need to make it write around 20 characters in 1 row and then move to another
                string[] textArray = Text.Split(' ');
                List<string> newText = new List<string>();
                int stringLen = 0;
                int strRow = 0;
                newText.Add("");


                foreach (string str in textArray)
                {

                    stringLen += str.Length+1;

                    if (stringLen < 20)
                    {
                        newText[strRow] += (str + " ");
                    }
                    else
                    {
                        stringLen = str.Length;
                        strRow++;
                        newText.Add("");
                        newText[strRow] += (str + " ");
                    }
                }


                sb.Draw(textBG, new Rectangle(Position.X+scrollX, Position.Y + scrollY - 20- ( 30*strRow), (19*12)+10, 30*(strRow+1)), Color.White);

                int index = 0;
                foreach (string txt in newText)
                {
                    sb.DrawString(font, txt, new Vector2(Position.X + scrollX + 10, Position.Y + scrollY - 20-(30*(strRow-index))), Color.Black);
                    index++;
                }
            }
            base.Draw(sb, spriteTexture, scrollX, scrollY);
        }


        /// <summary>
        /// ai 2 way movement
        /// </summary>
        private void Move(Tilemap tm)
        {
            //check which direction 
            if(Moving==true)
            switch(MoveDir)
            {
                case Sprite.Direction.DOWN:
                    if(!changedDir)
                    {
                        if(Position.Y < startPos.Y+Distance)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else //if the enemy passed the distance change the direction
                        {
                            changedDir = true;
                            MoveDir = Sprite.Direction.UP;
                        }
                    }
                    else if (changedDir)
                    {
                        if (Position.Y < startPos.Y)
                        { 
                            base.Move(MoveDir, tm, speed);
                        }
                        else //if the enemy passed the start postion change direction
                        {
                            changedDir = false;
                            MoveDir = Sprite.Direction.UP;
                        }
                    }
                    break;
                case Sprite.Direction.UP:
                    if(!changedDir)
                    {
                        if(Position.Y>startPos.Y-Distance)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = true;
                            MoveDir = Sprite.Direction.DOWN;
                        }
                    }
                    else if (changedDir)
                    {
                        if (Position.Y > startPos.Y)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = false;
                            MoveDir = Sprite.Direction.DOWN;
                        }
                    }
                    break;
                case Sprite.Direction.LEFT:
                    if(!changedDir)
                    {
                        if(Position.X>startPos.X-Distance)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = true;
                            MoveDir = Sprite.Direction.RIGHT;
                        }
                    }
                    else if (changedDir)
                    {
                        if (Position.X > startPos.X)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = false;
                            MoveDir = Sprite.Direction.RIGHT;
                        }
                    }
                    break;
                case Sprite.Direction.RIGHT:
                    if (!changedDir)
                    {
                        if (Position.X < startPos.X + Distance)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = true;
                            MoveDir = Sprite.Direction.LEFT;
                        }
                    }
                    else if (changedDir)
                    {
                        if (Position.X < startPos.X)
                        {
                            base.Move(MoveDir, tm, speed);
                        }
                        else
                        {
                            changedDir = false;
                            MoveDir = Sprite.Direction.LEFT;
                        }
                    }
                    break;
                default:
                    break;
            }   
        }


    }
}
