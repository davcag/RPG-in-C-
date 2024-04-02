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
    public class Player: Sprite
    {

        //stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int NextLvlExp { get; set; }
        public List<Item> Items;

        private int size = 60;
        
        public Player (Vector2 pos)
        {
            Position = new Rectangle((int)pos.X, (int)pos.Y,size,size );
            clr = Color.White;
            MoveDir = Direction.RIGHT;
            Moving = false;
            rows = 4;
            cols = 3;
            lastFrame = 3;
            frameTime = 0;
            curFrame = 1;

            //stats

            Health = 50;
            MaxHealth = 50;
            Attack = 5;
            Defense = 5;
            Level = 1;
            Exp = 0;
            NextLvlExp = 10;
            
            Items = new List<Item>();

        }


        public void UpdateExp(int exp)
        {
            Exp += exp;
            if (Exp >= NextLvlExp)
            {
                Attack += Level;
                Defense += Level;
                MaxHealth += (Level * 10);
                Level++;
                Exp = Exp - NextLvlExp;
                NextLvlExp += (10 * Level);

            }
        }

        /// <summary>
        /// Uses the parent function to move the sprite
        /// </summary>
        public override void Move(Sprite.Direction dir, Tilemap tm, int speed)
        {
            base.Move(dir, tm, speed);
        }

        /// <summary>
        /// Uses the parent function to update the animation
        /// </summary>
        public override void Update(GameTime time)
        {
            base.Update(time);
        }
      
        /// <summary>
        /// Uses parent function to draw the sprite
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, int scrollX, int scrollY)
        {
            base.Draw(spriteBatch, texture, scrollX, scrollY);
        }

        public void SetPosition(int x, int y)
        {
            Position = new Rectangle(x, y, size, size);
        }


    }
}
