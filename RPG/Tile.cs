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
    public class Tile
    {
        public bool Passable { get; private set; }
        public Color Clr { get; private set; }


        public Rectangle Position { get; set; }
        public Rectangle TilempPosition { get; private set; }


        private int tileSize = 64;

        public Tile(Rectangle TmapPosition, bool passable)
        {
            TilempPosition = TmapPosition;
            Passable = passable;
            Clr = Color.White; 
        }

        public Tile(Rectangle TmapPosition, bool passable, Rectangle position)
        {
            TilempPosition = TmapPosition;
            Passable = passable;
            Clr = Color.White;
            Position = position;
        }

        public Tile(Rectangle TmapPosition, bool passable, Rectangle position, Color clr)
        {
            TilempPosition = TmapPosition;
            Passable = passable;
            Clr = clr;
            Position = position;
        }


    }
}
