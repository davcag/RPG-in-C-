#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace RPG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //screen size
        int screenWidth = 800;
        int screenHeight = 600;

        //mouse
        MouseState ms;
        bool leftPresed;

        //Game states
        public enum GameState
        {
            MENU,
            PLAY,
            BATTLE
        };

        public GameState gameState;
        
        //menu
        public Menu menu;
        SpriteFont font;
        bool gameMenuOn;

        //battle
        public Battle battle;

        //textures and tilemap
        Dictionary<string, Texture2D> textures;
        Tilemap tileMap;

        

        //Player
        Player player;

        //Music
        Song song;


        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //mouse
            

            //GameState
            gameState = GameState.MENU;

            //
            tileMap = new Tilemap();
            textures = new Dictionary<string, Texture2D>();
            player = new Player(new Vector2(300, 300));
            leftPresed = false;
            gameMenuOn = false;


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load tiles
            textures.Add("Tilemap", Content.Load<Texture2D>("Tiles/Tilemap"));
            //load player and add his starting items
            textures.Add("Player", Content.Load<Texture2D>("Player/Player"));
            textures.Add("PlayerSheet", Content.Load<Texture2D>("Player/SpriteSheet"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Potion"));
            player.Items.Add(new Item("Bomb"));
            player.Items.Add(new Item("Bomb"));
            player.Items.Add(new Item("Bomb"));
            player.Items.Add(new Item("Bomb"));
            player.Items.Add(new Item("Bomb"));
            player.Items.Add(new Item("Bomb"));
            //load enemies
            textures.Add("Enemy", Content.Load<Texture2D>("Enemies/Enemy"));
            textures.Add("EnemySheet", Content.Load<Texture2D>("Enemies/EnemySheet"));
            //load NPC's
            textures.Add("NpcSheet", Content.Load<Texture2D>("NPC/NpcSheet"));
            //load a map
            tileMap.Load("Map1.xml");
            //button textures
            textures.Add("Button", Content.Load<Texture2D>("Buttons/White"));
            //battle arrows
            textures.Add("BattleArrows", Content.Load<Texture2D>("BattleArt/Arrows"));
            
            
            //menu stuff
            font = Content.Load<SpriteFont>("Font");
            menu = new MainMenu(font);

            //music
            //song = Content.Load<Song>("Music/Song1");
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(song);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here




            ms = Mouse.GetState();

            //if in play state
            if (gameState == GameState.PLAY)
            {
                //hide mouse
                if (this.IsMouseVisible == true && menu.GetMenuState() != Menu.MenuState.GAMEMENU)
                {
                    this.IsMouseVisible = false;
                }
                KeyboardUpdate();

                foreach(Enemy enemy in tileMap.enemies)
                {
                    enemy.Update(gameTime, tileMap);
                    //i'll make the player rect smaller so it doesnt
                    //react if a hand slightly touches the enemy
                    if(new Rectangle(player.Position.X+5,player.Position.Y+5,50,50).Intersects(enemy.Position))
                    {
                        battle = new Battle(player, enemy);//just random numbers for now
                        gameState = GameState.BATTLE;
                    }
                }

                foreach (NPC npc in tileMap.npcs)
                {
                    npc.Update(gameTime, tileMap);
                }

                player.Update(gameTime);

                if (menu.GetMenuState() == Menu.MenuState.GAMEMENU)
                {
                    UpdateMenu();
                    //if the player exited the menu update item list
                    if(menu.GetMenuState() != Menu.MenuState.GAMEMENU)
                    {
                        menu.SetItems(ref player);
                    }
                }
            }
            else if(gameState == GameState.BATTLE)
            {
                battle.Update(gameTime);

                //in case the player ran away
                if (battle.BattleOver)
                {
                    gameState = GameState.PLAY;
                    player = battle.ReturnPlayer();
                    //this is just a temp solution so i dont start a new battle as soon as i end it
                    player.SetPosition(300, 300);
                    tileMap.ScrollOffsetX = 0;
                    tileMap.ScrollOffsetY = 0;
                }
                else if (battle.BattleWon)
                {
                    gameState = GameState.PLAY;
                    player = battle.ReturnPlayer();
                    //check which enemy was in battle and remove it
                    //i might change it later because it will remove all enemies the player colided with
                    foreach (Enemy enemy in tileMap.enemies)
                    {
                        if(new Rectangle(player.Position.X+5,player.Position.Y+5,50,50).Intersects(enemy.Position))
                        {
                            tileMap.enemies.Remove(enemy);
                            return;
                        }
                    }
                }
                else if (battle.BattleLost)
                {
                    //needs to be done
                }
            }
            else if (gameState == GameState.MENU)
            {
                UpdateMenu();

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Turquoise);
           
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.NonPremultiplied);
            
            // TODO: Add your drawing code here



            if(gameState == GameState.PLAY)
            {
                DrawMap();

                //draw sprites
                foreach(Enemy enemy in tileMap.enemies)
                {
                    enemy.Draw(spriteBatch, textures["EnemySheet"], tileMap.ScrollOffsetX, tileMap.ScrollOffsetY);
                }

                foreach (NPC npc in tileMap.npcs)
                {
                    npc.Draw(spriteBatch, textures["NpcSheet"], textures["Button"], font, tileMap.ScrollOffsetX, tileMap.ScrollOffsetY);
                }

                //draw player
                player.Draw(spriteBatch, textures["PlayerSheet"], tileMap.ScrollOffsetX, tileMap.ScrollOffsetY);


                if(gameMenuOn ==true)
                {
                    menu.Draw(spriteBatch, textures["Button"]);

                }
            }
            else if (gameState == GameState.BATTLE)
            {
                battle.Draw(spriteBatch, font, textures["Button"], textures["Player"], textures["Enemy"], textures["BattleArrows"]);
            }
            else if(gameState == GameState.MENU)
            {
                //draw menu
                menu.Draw(spriteBatch, textures["Button"]); //will need to change texture later, just testing
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }

        

        /// <summary>
        /// handles keyboard input
        /// </summary>
        public void KeyboardUpdate()
        {
            if (gameState == GameState.PLAY)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    player.Move(Player.Direction.RIGHT, tileMap, 4);
                    if (!(player.Position.X + player.Position.Height + tileMap.ScrollOffsetX < 550) && 
                        tileMap.isPassable(new Rectangle(player.Position.X+4,player.Position.Y,60,60)))
                    {
                        tileMap.ScrollOffsetX -= 4;
                    }
                    player.Moving = true;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A) && player.Position.X > 0)
                {
                    player.Move(Player.Direction.LEFT, tileMap, 4);
                    if (!(player.Position.X + tileMap.ScrollOffsetX > 250) &&
                        tileMap.isPassable(new Rectangle(player.Position.X - 4, player.Position.Y, 60, 60)))
                    {
                        tileMap.ScrollOffsetX += 4;
                    }
                    
                    player.Moving = true;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.W) && player.Position.Y > 0)
                {
                    player.Move(Player.Direction.UP, tileMap, 4);
                    if (!(player.Position.Y + tileMap.ScrollOffsetY > 150) &&
                        tileMap.isPassable(new Rectangle(player.Position.X, player.Position.Y-4, 60, 60)))
                    {
                        tileMap.ScrollOffsetY += 4;
                    }
                    player.Moving = true;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    player.Move(Player.Direction.DOWN, tileMap, 4);
                    if (!(player.Position.Y + player.Position.Height + tileMap.ScrollOffsetY < 450) &&
                        tileMap.isPassable(new Rectangle(player.Position.X, player.Position.Y + 4, 60, 60)))
                    {
                        tileMap.ScrollOffsetY -= 4;
                    }
                    player.Moving = true;
                }
                else
                {
                    player.Moving = false;
                }

                if (gameMenuOn == false && Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    gameMenuOn = true;
                    menu = new GameMenu(font, player.Items);
                    this.IsMouseVisible = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    foreach (NPC npc in tileMap.npcs)
                    {
                        if (npc.Talking == false)
                        {
                            if (player.Position.Intersects(npc.Position))
                            {
                                npc.Talking = true;
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Draws the map
        /// </summary>
        public void DrawMap()
        {
            foreach (Tile tile in tileMap.tiles)
            {
                spriteBatch.Draw(textures["Tilemap"],
                    new Rectangle(tile.Position.X + tileMap.ScrollOffsetX, tile.Position.Y + tileMap.ScrollOffsetY, 64, 64),
                    tile.TilempPosition, tile.Clr);
            }
        }

        /// <summary>
        /// handles changes in the menu state
        /// </summary>
        public void MenuChange()
        {
            switch (menu.GetMenuState())
            {
                case Menu.MenuState.PLAY:
                    gameState = GameState.PLAY;
                    gameMenuOn = false;
                    break;
                case Menu.MenuState.OPTIONS:
                    menu = new OptionsMenu(font);
                    break;
                case Menu.MenuState.QUIT:
                    menu = new QuitMenu(font);
                    break;
                case Menu.MenuState.MAIN:
                    menu = new MainMenu(font);
                    if(gameState == GameState.PLAY)
                    {
                        gameState = GameState.MENU;
                        gameMenuOn = false;
                    }
                    break;
                default:
                    break;
            }
        }

        public void UpdateMenu()
        {
            Menu.MenuState startState = menu.GetMenuState();

            //show mouse
            if (this.IsMouseVisible == false)
            {
                this.IsMouseVisible = true;
            }


            if (ms.LeftButton == ButtonState.Pressed && leftPresed == false)
            {
                menu.Update(ms.X, ms.Y, true, ref player, ref tileMap);
                leftPresed = true;
            }
            else
            {
                menu.Update(ms.X, ms.Y, false, ref player, ref tileMap);
                if (leftPresed == true && ms.LeftButton == ButtonState.Released)
                {
                    leftPresed = false;
                }
            }

            //check if menu state switched
            if (startState != menu.GetMenuState())
            {
                //handle state changes
                MenuChange();
            }
  
        
        }
    }
}
