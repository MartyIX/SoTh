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
    public class Scene
    {
        protected View.GameDesk gameDesk;
        public event d_SceneEndedCallback callback;
        
        #region Properties (2)

        public GraphicsDevice graphicsDevice
        {
            get
            {
                return (gameDesk != null) ? gameDesk.GetGraphicsDevice : null;
            }
        }

        public SpriteBatch spriteBatch
        {
            get
            {
                return (gameDesk != null) ? gameDesk.GetSpriteBatch : null;
            }
        }

        public virtual void Draw()
        {
        }

        public Scene(View.GameDesk gameDesk)
        {
            this.gameDesk = gameDesk;
        }

        protected void CallCallback()
        {
            gameDesk.UnRegisterScene(this, this.Draw);
            if (callback != null) callback();
        }

        #endregion Properties 
    }
}
