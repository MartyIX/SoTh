#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System;
#endregion

namespace Sokoban
{
    public class Scene_EndOfRound : Scene
    {
		#region Fields (7) 

        private Vector2 center;
        private Vector2 fontOrigin;
        private int frameNo = 0;
        private int framesCount;
        private string message;
        private Texture2D picture;
        private Texture2D rectangle;

		#endregion Fields 

		#region Constructors (1) 

        public Scene_EndOfRound(int framesCount, View.GameDesk gameDesk) : base(gameDesk)
        {
            this.framesCount = framesCount;
            picture = gameDesk.content.Load<Texture2D>(@"Images/Scene_EndOfRound");
            rectangle = new Texture2D(graphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            rectangle.SetData<Color>(new Color[] {Color.CornflowerBlue});
            center = new Vector2(graphicsDevice.Viewport.Width / 2 - picture.Width / 2, graphicsDevice.Viewport.Height / 2 - picture.Height / 2);
            message = "You've made it!";
            fontOrigin = gameDesk.ArialFont.MeasureString(message) / 2;            
        }

		#endregion Constructors 

		#region Methods (1) 

		// Public Methods (1) 

        public override void Draw()
        {
            frameNo++;

            spriteBatch.Begin();
            int height = (int)Math.Ceiling((float)((float)frameNo / (float)framesCount) * graphicsDevice.Viewport.Height);
            spriteBatch.Draw(rectangle, new Rectangle(0, 0, graphicsDevice.Viewport.Width, height), Color.White);

            if (height > center.Y)
            {
                //spriteBatch.DrawString(gameDesk.ArialFont, "You've made it!", center, Color.Black, 0f, fontOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(picture, center, new Rectangle(0,0, picture.Width, height - (int)center.Y), Color.White);
            }

            spriteBatch.End();

            if (frameNo >= framesCount)
            {
                CallCallback();
            }
        }

		#endregion Methods 
    }
}