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
    public class Item
    {
        public string Name { get; private set; }

        public Item(string name)
        {
            Name = name;
        }

        /// <summary>
        /// for use in battle
        /// </summary>
        public static void Use(string name, ref Player player, ref Enemy enemy)
        {
            switch(name)
            {
                case "Potion":
                    player.Health += 10;
                    if(player.Health>player.MaxHealth)
                    {
                        player.Health = player.MaxHealth;
                    }
                    break;
                case "Bomb":
                    enemy.Health -= 10;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// for use in the items menu
        /// </summary>
        public static bool Use(string name, ref Player player)
        {
            switch (name)
            {
                case "Potion":
                    if (player.Health != player.MaxHealth)
                    {
                        player.Health += 10;
                        if (player.Health > player.MaxHealth)
                        {
                            player.Health = player.MaxHealth;
                        }
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}
