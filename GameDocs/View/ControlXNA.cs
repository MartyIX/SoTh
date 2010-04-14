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
using Sokoban.View;
using Sokoban.Model;
#endregion

namespace Sokoban
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to use ContentManager
    /// inside a WinForms application. It loads a SpriteFont object through the
    /// ContentManager, then uses a SpriteBatch to draw text. The control is not
    /// animated, so it only redraws itself in response to WinForms paint messages.
    /// </summary>
    public partial class ControlXNA : GraphicsDeviceControl
    {
		public SpriteFont ArialFont;
        public ContentManager content;
        public AudioEngine engine;
        public Texture2D part;
        public SoundBank soundBank;
        public SpriteBatch spriteBatch;
        public WaveBank waveBank;
        public event EmptyEventHandler OnDraw;

        public ControlXNA()
        {

        }
		
        public GraphicsDevice GetGraphicsDevice
        {
            get { return GraphicsDevice; }
        }

        public SpriteBatch GetSpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// Draws the control, using SpriteBatch and SpriteFont.
        /// </summary>
        public override void Draw()
        {
            engine.Update(); // for audio

            if (OnDraw != null) OnDraw();
        }

        public void HookRedrawing()
        {
            System.Windows.Forms.Application.Idle += delegate { 
                this.Invalidate();
                this.Update();
            };
        }

        /// <summary>
        /// Disposes the control, unloading the ContentManager.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Unload();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialization of playing desk. 
        /// Function set the desk from: maxX and maxY and resize the window accordingly.
        /// </summary>
        protected override void InitializeGraphics()
        {
            content = new ContentManager(Services, "Content");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize audio objects.

            string file = "Sounds/SokobanSounds.xgs";

            if (ApplicationRepository.ContentFileExists(file))
            {
                engine = new AudioEngine(ApplicationRepository.GetAppPath() + "/Content/" + file);
            }
            else
            {
                MessageBox.Show("File " + file + " does not exists.");
            }


            file = "Sounds/Sound Bank.xsb";

            if (ApplicationRepository.ContentFileExists(file))
            {
                soundBank = new SoundBank(engine, ApplicationRepository.GetAppPath() + "/Content/" + file);
            }
            else
            {
                MessageBox.Show("File " + file + " does not exists.");
            }


            file = "Sounds/Wave Bank.xwb";

            if (ApplicationRepository.ContentFileExists(file))
            {
                waveBank = new WaveBank(engine, ApplicationRepository.GetAppPath() + "/Content/" + file, 0, (short)16);
            }
            else
            {
                MessageBox.Show("File " + file + " does not exists.");
            }

            file = "Images/obj_A_tmave";

            if (ApplicationRepository.ContentFileExists(file + ".xnb"))
            {
                part = content.Load<Texture2D>(@"Images/obj_A_tmave");
            }
            else
            {
                MessageBox.Show("File " + file + ".xnb does not exists.");
            }                       
            
            ArialFont = content.Load<SpriteFont>("Arial");           
        }
    }
}