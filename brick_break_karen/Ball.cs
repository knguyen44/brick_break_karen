using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using MonoGameLibrary.Util;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace brick_break_karen
{
    public enum BallState { OnPaddleStart, Playing }

    public class Ball : DrawableSprite
    {
        SpriteBatch sb;
        SpriteFont font;

        GameConsole console;
        public BallState State;

        public Ball(Game game) : base(game)
        {
            this.Direction = new Vector2(1, 1);
            this.Speed = 200;
            this.State = BallState.OnPaddleStart;

            //Lazy load game console
            console = (GameConsole)this.Game.Services.GetService<IGameConsole>();
            if (console == null)
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);
            }
        }
        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Arial");

            this.spriteTexture = this.Game.Content.Load<Texture2D>("ballSmall");
            base.LoadContent();

            //TODO probably need to put this on the paddle as soon as I have paddle
            this.Location = new Vector2(200, 200);
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        //private time between frames
        float time;

        public override void Update(GameTime gameTime)
        {
            time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            this.keepBallOnScreen();

            //Balls hits bottom, then respawns
            if ((this.Location.Y > this.GraphicsDevice.Viewport.Height - this.spriteTexture.Height))
                this.respawnBall();

            this.UpdateBall();
            this.Location += this.Direction * (this.Speed * time);
            base.Update(gameTime);

            //Log Ball location to console
            console.Log("Ball Location", this.Location.ToString());
        }

        private void respawnBall()
        {
            this.State = BallState.OnPaddleStart;
            ScoreManager.Lives--;
            if(ScoreManager.Lives == 0)
            {
                using var game = new brick_break_karen.Game1();
                game.Exit();
            }

        }

        private void UpdateBall()
        {
            switch (this.State)
            {
                case BallState.OnPaddleStart:
                    this.Speed = 0;
                    break;
                case BallState.Playing:
                    this.Speed = 400;
                    break;
            }
        }

        private void keepBallOnScreen()
        {
            //Left and Right
            if ((this.Location.X < 0) ||
                (this.Location.X > this.GraphicsDevice.Viewport.Width - this.spriteTexture.Width))
                this.Direction.X *= -1;
            //Up abd Down
            if ((this.Location.Y < 0) ||
                (this.Location.Y > this.GraphicsDevice.Viewport.Height - this.spriteTexture.Height))
                this.Direction.Y *= -1;
        }

        internal void LaunchBall(GameTime gametime)
        {
            this.State = BallState.Playing;
            this.Direction = new Vector2(1, -1); //launch left and up TODO check this
        }

        internal void Reflect(MonogameBlock b)
        {
            //simple
            this.Direction *= -1;
        }
    }
}
