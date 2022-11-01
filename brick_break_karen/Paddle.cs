using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using MonoGameLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace brick_break_karen
{
    public class Paddle : DrawableSprite
    {
        GameConsole console;
        Random r;

        Ball ball;
        PaddleController controller;

        public Paddle(Game game, Ball b) : base(game)
        {
            this.Speed = 210;
            ball = b;
            this.controller = new PaddleController(this.Game, ball);

            r = new Random();
        }

        Rectangle collisionRectangle;  //Rectangle for paddle collision uses just the top of the paddle instead of the whole sprite

        public override void Update(GameTime gameTime)
        {
            //Update Collision Rect
            collisionRectangle = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.spriteTexture.Width, 1);

            //Deal with ball state
            switch (ball.State)
            {
                case BallState.OnPaddleStart:
                    //Move the ball with the paddle until launch
                    UpdateMoveBallWithPaddle();
                    break;
                case BallState.Playing:
                    UpdateCheckBallCollision();
                    break;
            }
   
            //Movement from controller
            controller.HandleInput(gameTime);

            this.Direction = controller.Direction;
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            KeepPaddleOnScreen();
            base.Update(gameTime);
        }
        private void UpdateMoveBallWithPaddle()
        {
            ball.Speed = 0;
            ball.Direction = Vector2.Zero;
            ball.Location = new Vector2(this.Location.X + (this.LocationRect.Width / 2 - ball.SpriteTexture.Width / 2), this.Location.Y - ball.SpriteTexture.Height);
        }

        //TODO make the paddle reflect ball
        private void UpdateCheckBallCollision()
        {
            //Ball Collsion
            //Very simple collision with ball only uses rectangles
            if (collisionRectangle.Intersects(ball.LocationRect))
            {
                //TODO Change angle based on location of collision or direction of paddle
                ball.Direction.Y *= -1;
                UpdateBallCollisionBasedOnPaddleImpactLocation();
                UpdateBallCollisionRandomFuness();
                console.GameConsoleWrite("Paddle collision ballLoc:" + ball.Location + " paddleLoc:" + this.Location.ToString());
            }
        }
        private void UpdateBallCollisionBasedOnPaddleImpactLocation()
        {
            //Change angle based on paddle movement
            if (this.Direction.X > 0)
            {
                ball.Direction.X += .1f;
            }
            if (this.Direction.X < 0)
            {
                ball.Direction.X -= .1f;
            }
            //Change anlge based on side of paddle
            //First Third

            if ((ball.Location.X > this.Location.X) && (ball.Location.X < this.Location.X + this.spriteTexture.Width / 3))
            {
                console.GameConsoleWrite("1st Third");
                ball.Direction.X += .1f;
            }
            if ((ball.Location.X > this.Location.X + (this.spriteTexture.Width / 3)) && (ball.Location.X < this.Location.X + (this.spriteTexture.Width / 3) * 2))
            {
                console.GameConsoleWrite("2nd third");
            }
            if ((ball.Location.X > (this.Location.X + (this.spriteTexture.Width / 3) * 2)) && (ball.Location.X < this.Location.X + (this.spriteTexture.Width)))
            {
                console.GameConsoleWrite("3rd third");
                ball.Direction.X -= .1f;
            }
        }
        private void UpdateBallCollisionRandomFuness()
        {
            /// 
            /// Adds a bit of entropy to bounce nothing should be perfect
            /// 
            /// 
            ball.Direction.Y = GetReflectEntropy();
        }


        private float GetReflectEntropy()
        {
            return -1 + ((r.Next(0, 3) - 1) * 0.1f); //return -.9, -1 or -1.1
        }
        private void KeepPaddleOnScreen()
        {
            this.Location.X = MathHelper.Clamp(this.Location.X, 0, this.Game.GraphicsDevice.Viewport.Width - this.spriteTexture.Width);
        }


        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("paddleSmall");
            base.LoadContent();
            this.Location = new Vector2(100, 400);
        }
    }
}
