﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Sprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace brick_break_karen
{
    public class MonogameBlock : DrawableSprite
    {
        Block block;
        string NormalTextureName, HitTextureName;
        Texture2D NormalTexture, HitTexture;

        public MonogameBlock(Game game) : base(game)
        {
            this.NormalTextureName = "block_blue";
            this.HitTextureName = "block_bubble";
            block = new Block();
        }
        protected override void LoadContent()
        {
            NormalTexture = this.Game.Content.Load<Texture2D>(NormalTextureName);
            HitTexture = this.Game.Content.Load<Texture2D>(HitTextureName);
            this.UpdateTexture();
            base.LoadContent();
            this.Location = new Vector2(100, 100);
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateTexture();
            base.Update(gameTime);
        }

        private void UpdateTexture()
        {
            switch (block.BlockState)
            {
                case BlockState.Normal:
                    this.spriteTexture = NormalTexture;
                    break;
                    case BlockState.Hit:
                    this.spriteTexture = HitTexture;
                    break;
            }
        }
        public void HitByBall()
        {
            this.block.Hit();
        }
    }
}
