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
    public class Button
    {
        public Rectangle Position { get; private set; }
        public Vector2 TextPosition{ get; private set; }
        public string Text{get; set;}
        public Color Clr { get; set;}

        public Button (Rectangle pos, Vector2 txtPos, string txt, Color clr)
        {
            Position = pos;
            TextPosition = txtPos;
            Text = txt;
            Clr = clr;
        }

    }
}
