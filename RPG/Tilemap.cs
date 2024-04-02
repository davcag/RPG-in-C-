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
using System.Xml;
using System.IO;

namespace RPG
{
    public class Tilemap
    {
        public List<Tile> tiles { get; private set; }
        public List<Sprite> enemies { get; private set; }
        public List<Sprite> npcs { get; private set; }
        public bool SaveNotFound { get; set; }
        //scrolling
        public int ScrollOffsetX { get; set; }
        public int ScrollOffsetY { get; set; }


        public Tilemap()
        {
            tiles = new List<Tile>();
            enemies = new List<Sprite>();
            npcs = new List<Sprite>();
            SaveNotFound = false;
            ScrollOffsetX = 0;
            ScrollOffsetY = 0;
        }



         /// <summary>
        /// Loads a tilemap from a xml document
        /// </summary>
        public void Load(string path)
        {

            //clear the old tiles to load the new ones
            tiles.Clear();
            enemies.Clear();
            npcs.Clear();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);

            //add tiles
            foreach (XmlNode node in xDoc.SelectNodes("Tilemap/Tiles/Tile"))
            {
                //read the xml stuff
                Rectangle TmapPosition = new Rectangle(
                    int.Parse(node.SelectSingleNode("TilemapPosition/X").InnerText),
                    int.Parse(node.SelectSingleNode("TilemapPosition/Y").InnerText), 64, 64);

                bool passable = bool.Parse(node.SelectSingleNode("Passable").InnerText);

                Rectangle position = new Rectangle(int.Parse(node.SelectSingleNode("Position/X").InnerText),
                    int.Parse(node.SelectSingleNode("Position/Y").InnerText),64,64);

                Color clr = new Color(int.Parse(node.SelectSingleNode("Color/R").InnerText),
                    int.Parse(node.SelectSingleNode("Color/G").InnerText),
                    int.Parse(node.SelectSingleNode("Color/B").InnerText),
                    int.Parse(node.SelectSingleNode("Color/A").InnerText));

                //add a new tile
                tiles.Add(new Tile(TmapPosition, passable, position, clr));
            }

            //add sprites
            foreach (XmlNode node in xDoc.SelectNodes("Tilemap/Sprites/Sprite"))
            {
                //read the xml stuff
                string name = node.SelectSingleNode("Name").InnerText;

                bool friendly = bool.Parse(node.SelectSingleNode("Friendly").InnerText);

                Vector2 position = new Vector2(float.Parse(node.SelectSingleNode("Position/X").InnerText), float.Parse(node.SelectSingleNode("Position/Y").InnerText));

                Color clr = new Color(int.Parse(node.SelectSingleNode("Color/R").InnerText),
                    int.Parse(node.SelectSingleNode("Color/G").InnerText),
                    int.Parse(node.SelectSingleNode("Color/B").InnerText),
                    int.Parse(node.SelectSingleNode("Color/A").InnerText));

                string Direction = node.SelectSingleNode("Direction").InnerText;
                Sprite.Direction dir = Sprite.Direction.RIGHT;
                switch(Direction)
                {
                    case "UP":
                        dir = Sprite.Direction.UP;
                        break;
                    case "DOWN":
                        dir = Sprite.Direction.DOWN;
                        break;
                    case "LEFT":
                        dir = Sprite.Direction.LEFT;
                        break;
                    case"RIGHT":
                        dir = Sprite.Direction.RIGHT;
                        break;
                    default:
                        break;
                }


                int dist = int.Parse(node.SelectSingleNode("Distance").InnerText);

                //add a new sprite
                if (!friendly)
                {
                    int health = int.Parse(node.SelectSingleNode("Health").InnerText);
                    int attack = int.Parse(node.SelectSingleNode("Attack").InnerText);
                    int defense = int.Parse(node.SelectSingleNode("Defense").InnerText);
                    enemies.Add(new Enemy(name,position, dir, 2, dist, health,attack,defense));
                }
                else if(friendly)
                {
                    string text = node.SelectSingleNode("Text").InnerText;
                    npcs.Add(new NPC(name,position, dir, 2, dist, text));
                }
            }
        }

        public void SaveGame(string path, Player player)
        {
            XmlTextWriter xWriter = new XmlTextWriter(path, Encoding.UTF8);
            xWriter.Formatting = Formatting.Indented;

            xWriter.WriteStartElement("Tilemap");//<Tilemap>

            xWriter.WriteStartElement("Sprites");//<Sprites>

            foreach (Enemy sprite in enemies)
            {
                xWriter.WriteStartElement("Sprite"); //<Sprite>
                xWriter.WriteStartElement("Name"); //<Name>
                xWriter.WriteString(sprite.Name);
                xWriter.WriteEndElement();//</Name>
                xWriter.WriteStartElement("Friendly"); //<Friendly>
                xWriter.WriteString("false");
                xWriter.WriteEndElement();//</Friendly>
                xWriter.WriteStartElement("Position"); //<Position>
                xWriter.WriteStartElement("X"); //<X>
                xWriter.WriteString(sprite.Position.X.ToString());
                xWriter.WriteEndElement();//</X>
                xWriter.WriteStartElement("Y"); //<Y>
                xWriter.WriteString(sprite.Position.Y.ToString());
                xWriter.WriteEndElement();//</Y>
                xWriter.WriteEndElement();//</Position>
                xWriter.WriteStartElement("Color"); //<Color>
                xWriter.WriteStartElement("R"); //<R>
                xWriter.WriteString(sprite.clr.R.ToString());
                xWriter.WriteEndElement();//</R>
                xWriter.WriteStartElement("G"); //<G>
                xWriter.WriteString(sprite.clr.G.ToString());
                xWriter.WriteEndElement();//</G>
                xWriter.WriteStartElement("B"); //<B>
                xWriter.WriteString(sprite.clr.B.ToString());
                xWriter.WriteEndElement();//</B>
                xWriter.WriteStartElement("A"); //<A>
                xWriter.WriteString(sprite.clr.A.ToString());
                xWriter.WriteEndElement();//</A>
                xWriter.WriteEndElement();//</Color>
                xWriter.WriteStartElement("Direction"); //<Direction>
                xWriter.WriteString(sprite.MoveDir.ToString());
                xWriter.WriteEndElement();//</Direction>
                xWriter.WriteStartElement("Distance"); //<Distance>
                xWriter.WriteString(sprite.Distance.ToString());
                xWriter.WriteEndElement();//</Distance>
                xWriter.WriteStartElement("Health"); //<Health>
                xWriter.WriteString(sprite.Health.ToString());
                xWriter.WriteEndElement();//</Health>
                xWriter.WriteStartElement("Attack"); //<Attack>
                xWriter.WriteString(sprite.Attack.ToString());
                xWriter.WriteEndElement();//</Attack>
                xWriter.WriteStartElement("Defense"); //<Defense>
                xWriter.WriteString(sprite.Defense.ToString());
                xWriter.WriteEndElement();//</Defense>
                xWriter.WriteEndElement();//</Sprite>
            }

            foreach (NPC sprite in npcs)
            {
                xWriter.WriteStartElement("Sprite"); //<Sprite>
                xWriter.WriteStartElement("Name"); //<Name>
                xWriter.WriteString(sprite.Name);
                xWriter.WriteEndElement();//</Name>
                xWriter.WriteStartElement("Friendly"); //<Friendly>
                xWriter.WriteString("true");
                xWriter.WriteEndElement();//</Friendly>
                xWriter.WriteStartElement("Position"); //<Position>
                xWriter.WriteStartElement("X"); //<X>
                xWriter.WriteString(sprite.Position.X.ToString());
                xWriter.WriteEndElement();//</X>
                xWriter.WriteStartElement("Y"); //<Y>
                xWriter.WriteString(sprite.Position.Y.ToString());
                xWriter.WriteEndElement();//</Y>
                xWriter.WriteEndElement();//</Position>
                xWriter.WriteStartElement("Color"); //<Color>
                xWriter.WriteStartElement("R"); //<R>
                xWriter.WriteString(sprite.clr.R.ToString());
                xWriter.WriteEndElement();//</R>
                xWriter.WriteStartElement("G"); //<G>
                xWriter.WriteString(sprite.clr.G.ToString());
                xWriter.WriteEndElement();//</G>
                xWriter.WriteStartElement("B"); //<B>
                xWriter.WriteString(sprite.clr.B.ToString());
                xWriter.WriteEndElement();//</B>
                xWriter.WriteStartElement("A"); //<A>
                xWriter.WriteString(sprite.clr.A.ToString());
                xWriter.WriteEndElement();//</A>
                xWriter.WriteEndElement();//</Color>
                xWriter.WriteStartElement("Direction"); //<Direction>
                xWriter.WriteString(sprite.MoveDir.ToString());
                xWriter.WriteEndElement();//</Direction>
                xWriter.WriteStartElement("Distance"); //<Distance>
                xWriter.WriteString(sprite.Distance.ToString());
                xWriter.WriteEndElement();//</Distance>
                xWriter.WriteStartElement("Text"); //<Text>
                xWriter.WriteString(sprite.Text);
                xWriter.WriteEndElement();//</Text>                             
                xWriter.WriteEndElement();//</Sprite>
            
            }
            xWriter.WriteEndElement();//</Sprites>


            xWriter.WriteStartElement("Player"); //<Player>
                xWriter.WriteStartElement("Level"); //<Level>
                xWriter.WriteString(player.Level.ToString());
                xWriter.WriteEndElement();//</Level>
                xWriter.WriteStartElement("Exp"); //<Exo>
                xWriter.WriteString(player.Exp.ToString());
                xWriter.WriteEndElement();//</Exp>
                xWriter.WriteStartElement("NextLevelExp"); //<NextLevelExp>
                xWriter.WriteString(player.NextLvlExp.ToString());
                xWriter.WriteEndElement();//</NextLevelExp>
                xWriter.WriteStartElement("Health"); //<Health>
                xWriter.WriteString(player.Health.ToString());
                xWriter.WriteEndElement();//</Health>
                xWriter.WriteStartElement("MaxHealth"); //<MaxHealth>
                xWriter.WriteString(player.MaxHealth.ToString());
                xWriter.WriteEndElement();//</MaxHealth>
                xWriter.WriteStartElement("Attack"); //<Attack>
                xWriter.WriteString(player.Attack.ToString());
                xWriter.WriteEndElement();//</Attack>
                xWriter.WriteStartElement("Defense"); //<Defense>
                xWriter.WriteString(player.Defense.ToString());
                xWriter.WriteEndElement();//</Defense>
                xWriter.WriteStartElement("Position"); //<Position>
                    xWriter.WriteStartElement("X"); //<X>
                    xWriter.WriteString(player.Position.X.ToString());
                    xWriter.WriteEndElement();//</X>
                    xWriter.WriteStartElement("Y"); //<Y>
                    xWriter.WriteString(player.Position.Y.ToString());
                    xWriter.WriteEndElement();//</Y>
                xWriter.WriteEndElement();//</Position>
            xWriter.WriteEndElement();//</Player>
            xWriter.WriteStartElement("Map"); //<Map>
                xWriter.WriteStartElement("ScrollOffsetX"); //<ScrollOffsetX>
                xWriter.WriteString(ScrollOffsetX.ToString());
                xWriter.WriteEndElement();//</ScrollOffsetX>
                xWriter.WriteStartElement("ScrollOffsetY"); //<ScrollOffsetY>
                xWriter.WriteString(ScrollOffsetY.ToString());
                xWriter.WriteEndElement();//</ScrollOffsetY>
            xWriter.WriteEndElement();//</Map>

            xWriter.WriteEndElement();//</Tilemap>

            xWriter.Close();
        }

        public void LoadGame(string path, ref Player player)
        { 
            if (File.Exists(path))
            {
                //clear the old tiles to load the new ones
                enemies.Clear();
                npcs.Clear();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(path);

                //add sprites
                foreach (XmlNode node in xDoc.SelectNodes("Tilemap/Sprites/Sprite"))
                {
                    //read the xml stuff
                    string name = node.SelectSingleNode("Name").InnerText;

                    bool friendly = bool.Parse(node.SelectSingleNode("Friendly").InnerText);

                    Vector2 position = new Vector2(float.Parse(node.SelectSingleNode("Position/X").InnerText), float.Parse(node.SelectSingleNode("Position/Y").InnerText));

                    Color clr = new Color(int.Parse(node.SelectSingleNode("Color/R").InnerText),
                        int.Parse(node.SelectSingleNode("Color/G").InnerText),
                        int.Parse(node.SelectSingleNode("Color/B").InnerText),
                        int.Parse(node.SelectSingleNode("Color/A").InnerText));

                    string Direction = node.SelectSingleNode("Direction").InnerText;
                    Sprite.Direction dir = Sprite.Direction.RIGHT;
                    switch (Direction)
                    {
                        case "UP":
                            dir = Sprite.Direction.UP;
                            break;
                        case "DOWN":
                            dir = Sprite.Direction.DOWN;
                            break;
                        case "LEFT":
                            dir = Sprite.Direction.LEFT;
                            break;
                        case "RIGHT":
                            dir = Sprite.Direction.RIGHT;
                            break;
                        default:
                            break;
                    }


                    int dist = int.Parse(node.SelectSingleNode("Distance").InnerText);

                    //add a new sprite
                    if (!friendly)
                    {
                        int health = int.Parse(node.SelectSingleNode("Health").InnerText);
                        int attack = int.Parse(node.SelectSingleNode("Attack").InnerText);
                        int defense = int.Parse(node.SelectSingleNode("Defense").InnerText);
                        enemies.Add(new Enemy(name, position, dir, 2, dist, health, attack, defense));
                    }
                    else if (friendly)
                    {
                        string text = node.SelectSingleNode("Text").InnerText;
                        npcs.Add(new NPC(name, position, dir, 2, dist, text));
                    }
                }
                foreach (XmlNode node in xDoc.SelectNodes("Tilemap/Player"))
                {
                    player.Level = int.Parse(node.SelectSingleNode("Level").InnerText);
                    player.Exp = int.Parse(node.SelectSingleNode("Exp").InnerText);
                    player.NextLvlExp = int.Parse(node.SelectSingleNode("NextLevelExp").InnerText);
                    player.Health = int.Parse(node.SelectSingleNode("Health").InnerText);
                    player.MaxHealth = int.Parse(node.SelectSingleNode("MaxHealth").InnerText);
                    player.Attack = int.Parse(node.SelectSingleNode("Attack").InnerText);
                    player.Defense = int.Parse(node.SelectSingleNode("Defense").InnerText);
                    player.Position = new Rectangle(
                        int.Parse(node.SelectSingleNode("Position/X").InnerText),
                        int.Parse(node.SelectSingleNode("Position/Y").InnerText), 64, 64);
                }
                foreach (XmlNode node in xDoc.SelectNodes("Tilemap/Map"))
                {
                    ScrollOffsetX = int.Parse(node.SelectSingleNode("ScrollOffsetX").InnerText);
                    ScrollOffsetY = int.Parse(node.SelectSingleNode("ScrollOffsetY").InnerText);
                }
            }
            else
            {
                //no saved games found
                SaveNotFound = true;
            }
        }

        public bool isPassable (Rectangle playerRect)
        {
            foreach(Tile tile in tiles)
            {
                if(tile.Position.Intersects(playerRect))
                {
                    if (tile.Passable == false)
                    {
                        return false;
                    }
                }

            } 

            //if it's something that's not in the map return false;
            return true;   
        }

            

    }
}
