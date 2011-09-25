using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using _3DAPI;
using WPhoneLib;
namespace WPSampleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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
            System.Threading.Thread mthread = new System.Threading.Thread(thetar);
            mthread.Start();
        }
        void thetar()
        {
            renderer = new PhoneRenderer(this, Content, GraphicsDevice);
            // TODO: use this.Content to load your game content here
          
            RealProg mprog = new RealProg(renderer);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            TouchCollection mlection = TouchPanel.GetState();

            foreach (TouchLocation et in mlection)
            {
                if (et.State == TouchLocationState.Moved)
                {
                    renderer.ntfyTouchMoved(et.Id, (int)et.Position.X, (int)et.Position.Y);
                }
                if (et.State == TouchLocationState.Pressed)
                {
                    renderer.ntfyTouchDown((int)et.Position.X, (int)et.Position.Y, et.Id);
                }
                if (et.State == TouchLocationState.Released)
                {
                    renderer.ntfyTouchUp((int)et.Position.X, (int)et.Position.Y, et.Id);
                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        internal PhoneRenderer renderer;
        bool red = true;
        bool drawnred = false;
        bool drawnwhite = false;
        bool drawnblue = false;
        RenderTarget2D target;
        RenderTarget2D secondtarget;
        bool enable3D = true;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderer.matrix = Matrix.CreateTranslation(0, 0, 0);
            GraphicsDevice.Clear(Color.Black);
            Vector3 cameraPosition = new Vector3(0, 0, 2.5f);
            cameraPosition.Y -= .01f;
            if (target == null)
            {
                //Uncomment for depth correction; but blurry pictures
                //target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight,false,SurfaceFormat.Rgba64,DepthFormat.Depth24);
                target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            } RasterizerState tstate = new RasterizerState();

            tstate.CullMode = CullMode.None;
            DepthStencilState lstate = new DepthStencilState();
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            lstate.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = lstate;
            GraphicsDevice.RasterizerState = tstate;
         
                if (!renderer.NtfyReadyRender(GraphicsDevice))
                {
              
                    renderer.NtfyReadyRender(GraphicsDevice);
                }
            
            base.Draw(gameTime);
        }
    }
}
