using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Sokoban
{
    /// <summary>
    /// Object on game desk
    /// </summary>
    public partial class GameObject
    {       
        /// <summary>
        /// For XNA
        /// </summary>
        public virtual void Draw()
        {
            if (movementInProgress == false)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(texture, new Rectangle(1 + (this.posX - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Width) / 2),
                                                       1 + (this.posY - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Height) / 2),
                                                       texture.Width, texture.Height), Color.White);
                spriteBatch.End();
            }
            else
            {
                double ratio = ((double)(this.model.time - movementStartTime) / (double)movementNoSteps);

                if (ratio >= 1)
                {
                    ratio = 1f;
                    movementStopWatch = null; // stop stopwatch
                    movementInProgress = false;
                }

                int addDistance = Convert.ToInt32(Math.Round(form.squareSize * ratio));


                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(texture,
                                 new Rectangle(1 + (this.lastPosX - 1) * form.squareSize + (posX - lastPosX) * addDistance +
                                               Convert.ToInt32((form.squareSize - texture.Width) / 2),
                                               1 + (this.lastPosY - 1) * form.squareSize + (posY - lastPosY) * addDistance +
                                               Convert.ToInt32((form.squareSize - texture.Height) / 2),
                                               texture.Width, texture.Height), Color.White);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Method is called by GameDesk.AddGraphicComponent
        /// </summary>
        /// <param name="services"></param>
        /// <param name="graphicsDevice"></param>
        public virtual void InitializeContent(ServiceContainer services, GraphicsDevice graphicsDevice, SoundBank soundBankParam)
        {
            Services = services;
            GraphicsDevice = graphicsDevice;
            soundBank = soundBankParam;
            content = new ContentManager(Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //spriteBatch = (SpriteBatch)Services.GetService(typeof(SpriteBatch));
            texture = content.Load<Texture2D>(@"Images\obj_" + this.Description);
        }
    }
}