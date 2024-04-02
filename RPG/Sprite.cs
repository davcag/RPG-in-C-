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
    /// This will be a parent class for the player
    /// and enemy sprites
    /// </summary>
    public class Sprite
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        };


        public Rectangle Position { get; set; }
        public Color clr { get; protected set; }
        public bool Moving { get; set; }
        public Direction MoveDir { get; protected set; }

        //animation stuff
        protected int rows;
        protected int cols;
        protected int curFrame;
        protected int lastFrame;
        protected int frameTime;



        /// <summary>
        /// Moves the sprite a certain distance in the chosen idrection if the tile is passable
        /// </summary>
        public virtual void Move (Direction dir, Tilemap tm, int speed)
        {
            MoveDir = dir; //need to do this for the animation

            if (dir == Direction.RIGHT)
            {

                Rectangle newRect = new Rectangle(Position.X + speed, Position.Y, Position.Width, Position.Height);
               
                if (tm.isPassable(newRect) == true)
                {
                    Position = newRect;
                }

            }
            if (dir == Direction.LEFT)
            {
                Rectangle newRect = new Rectangle(Position.X - speed, Position.Y, Position.Width, Position.Height);

                if (tm.isPassable(newRect) == true)
                {
                    Position = newRect;
                }
            }
            if (dir == Direction.UP)
            {
                Rectangle newRect = new Rectangle(Position.X, Position.Y - speed, Position.Width, Position.Height);

                if (tm.isPassable(newRect) == true)
                {
                    Position = newRect;
                }
            }
            if (dir == Direction.DOWN)
            {
                Rectangle newRect = new Rectangle(Position.X, Position.Y + speed, Position.Width, Position.Height);

                if (tm.isPassable(newRect) == true)
                {
                    Position = newRect;
                }
            }
        }

        /// <summary>
        /// Updates the animation of the sprite,
        /// the 1st frame is the stand still frame and it's not in the move animation
        /// </summary>
        public virtual void Update (GameTime time)
        {
            if (Moving == true)
            {
                frameTime += time.ElapsedGameTime.Milliseconds;
                if (frameTime > 250)
                {
                    curFrame++;
                    frameTime = 0;
                    if (curFrame == lastFrame)
                    {
                        curFrame = 1;
                    }
                }
            }
        }



        /// <summary>
        ///  Draws the sprite
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture, int scrollX , int scrollY)
        {
            int width = texture.Width / cols;
            int height = texture.Height / rows;


            if (Moving == false)
            {
                switch (MoveDir)
                {
                    case Direction.RIGHT:
                        spriteBatch.Draw(texture, new Rectangle(Position.X+scrollX,Position.Y+scrollY,60,60), new Rectangle(0, 0, width, height), clr);
                        break;
                    case Direction.LEFT:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(0, 60, width, height), clr);
                        break;
                    case Direction.DOWN:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(0, 120, width, height), clr);
                        break;
                    case Direction.UP:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(0, 180, width, height), clr);
                        break;
                }

            }
            else if (Moving == true)
            {
                switch (MoveDir)
                {
                    case Direction.RIGHT:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(curFrame * width, 0, width, height), clr);
                        break;
                    case Direction.LEFT:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(curFrame * width, 60, width, height), clr);
                        break;
                    case Direction.DOWN:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(curFrame * width, 120, width, height), clr);
                        break;
                    case Direction.UP:
                        spriteBatch.Draw(texture, new Rectangle(Position.X + scrollX, Position.Y + scrollY, 60, 60), new Rectangle(curFrame * width, 180, width, height), clr);
                        break;
                }
            }
        }




    }
}
