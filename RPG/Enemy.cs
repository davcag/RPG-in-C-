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
    public class Enemy : Sprite
    {
        //stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public string Name { get; private set; }
        public Vector2 StartPos;
        private int size = 60;
        private bool changedDir;
        private int speed;
        public int Distance { get; private set; }

        public Enemy(string name, Vector2 pos, Sprite.Direction dir, int sp, int dist, int hp, int att, int def)
        {
            this.Name = name;
            StartPos = pos;
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

            // stats
            Health = hp;
            MaxHealth = hp;
            Attack = att;
            Defense = def;
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
            //Updates the animation
            base.Update(time);
        }


        /// <summary>
        /// Uses parent function to draw the sprite
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, int scrollX, int scrollY)
        {
            base.Draw(spriteBatch, texture, scrollX, scrollY);
        }



        /// <summary>
        /// ai 2 way movement
        /// </summary>
        private void Move(Tilemap tm)
        {
            //check which direction 
            switch(MoveDir)
            {
                case Sprite.Direction.DOWN:
                    if(!changedDir)
                    {
                        if(Position.Y < StartPos.Y+Distance)
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
                        if (Position.Y < StartPos.Y)
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
                        if(Position.Y>StartPos.Y-Distance)
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
                        if (Position.Y > StartPos.Y)
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
                        if(Position.X>StartPos.X-Distance)
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
                        if (Position.X > StartPos.X)
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
                        if (Position.X < StartPos.X + Distance)
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
                        if (Position.X < StartPos.X)
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
