using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace brick_break_karen
{
    public class BlockManager : DrawableGameComponent
    {
        public List<MonogameBlock> Blocks { get; private set; } //List of Blocks the are managed by Block Manager
        List<MonogameBlock> blocksToRemove; //list of block to remove probably because they were hit

        //Dependancy on Ball
        Ball ball;

        public BlockManager(Game game, Ball b) : base(game)
        {
            this.ball = b;
            this.Blocks = new List<MonogameBlock>();
            this.blocksToRemove = new List<MonogameBlock>();
        }
        public override void Initialize()
        {
            LoadLevel();
            base.Initialize();
        }

        private void LoadLevel()
        {
            CreateBlockArrayByWidthAndHeight(24, ScoreManager.Level, 1);
        }

        const int hardLeftMargin = 5;
        const int hardTopMargin = 50;

        private void CreateBlockArrayByWidthAndHeight(int width, int height, int margin)
        {
            MonogameBlock b;
            //Create Block Array based on with and hieght
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    b = new MonogameBlock(this.Game);
                    b.Initialize(); //outside of game load and init
                    b.Location = new Vector2(hardLeftMargin + (w * b.SpriteTexture.Width + (w * margin)), hardTopMargin + (h * b.SpriteTexture.Height + (h * margin)));
                    b.SetTranformAndRect();
                    Blocks.Add(b);
                }
            }
        }

        //Bool switch
        bool Ballreflected; //the ball should onlyl reflect once even if it hits two bricks
        public override void Update(GameTime gameTime)
        {
            this.Ballreflected = false; //only reflect once per update
            UpdateCheckBlocksForCollision(gameTime);
            //UpdateBlocks(gameTime);
            base.Update(gameTime);
        }
        private void UpdateCheckBlocksForCollision(GameTime gameTime)
        {
            foreach (MonogameBlock b in Blocks)
            {
                if (b.Enabled) //Only chack active blocks
                {
                    b.Update(gameTime); //Update Block
                    //Ball Collision
                    if (b.Intersects(ball)) //check rectagle collision between ball and current block 
                    {
                        //hit
                        if(ball.State == BallState.Playing)
                            b.HitByBall(ball);
                        //update score
                        ScoreManager.Score++;
                        if (b.BlockState == BlockState.Broken)
                            blocksToRemove.Add(b);  //Ball is hit add it to remove list
                        if (!Ballreflected) //only reflect once
                        {
                            ball.Reflect(b);
                            this.Ballreflected = true;
                        }
                    }
                }
            }
        }
            private void UpdateBlocks(GameTime gameTime)
        {
            foreach (var block in Blocks)
            {
                if(block.Enabled == true)
                    block.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var block in this.Blocks)
            {
                if (block.Visible)   //respect block visible property
                    block.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
