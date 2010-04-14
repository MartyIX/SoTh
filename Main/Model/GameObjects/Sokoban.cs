#region using statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
#endregion


namespace Sokoban
{
    using Color = Microsoft.Xna.Framework.Graphics.Color;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;
    using Microsoft.Xna.Framework.Audio;

    public class Sokoban : GameObject
    {

        #region fields

        private bool isDeathAnimationStarted;

        public bool IsDeathAnimationStarted
        {
            get { return isDeathAnimationStarted; }
        }
        private int numberOfFrames = 30;
        private int currentFrame = 0;
        private SpriteSheet deathSpriteSheet = null;
        private SpriteSheet movementSpriteSheet = null;
        private int movementCurrentFrame = 0;
        private SokobanState sokobanState;
        private SpriteEffects spriteEffect;
        private int stepsNo;

        /// <summary>
        /// Actual number of steps Sokoban made so far
        /// </summary>
        public int StepsNo { get {return stepsNo;} }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Sokoban(View.GameDeskView form, Player player, int objectID, string description,
                          int pozX, int pozY, MovementDirection direction, EventType InitialEvent, int speed) :
               base(form, player, objectID, description, pozX, pozY, direction, InitialEvent, speed)
        {
            isDeathAnimationStarted = false;
            sokobanState = SokobanState.alive;
            stepsNo = 0;
        }
        #endregion

        #region Game logic
            /// <summary>
            /// Increment number of steps Sokoban did and show actual number in formular
            /// </summary>
            public void AddStep()
            {
                this.stepsNo++;
                //form.updateTimeAndSteps();
            }

            /// <summary>
            /// Set if Sokoban is oriented to the left or to the right; no gimmicks with pSokoban.smer!
            /// </summary>
            /// <param name="smer"></param>
            public void ChangeOrientation(MovementDirection direct)
            {
                if (this.direction != direct && (direct == MovementDirection.goLeft || direct == MovementDirection.goRight))
                {
                    this.direction = direct;
                }
            }

            /// <summary>
            /// Plays sound victory sound
            /// </summary>
            public void PlayHappySound()
            {
                // Play the sound.
                soundBank.PlayCue("SokobanHappySound");
            }

            /// <summary>
            /// Plays sound victory sound
            /// </summary>
            public void PlayScreamSound()
            {
                // Play the sound.
                soundBank.PlayCue("Scream");
            }

        #endregion

        #region Sokoban appearance

            public void StartKillingAnimation()
            {
                currentFrame = 0; // but first frame is 1! 
                movementCurrentFrame = 0;
                isDeathAnimationStarted = true;
            }

            public void ShowDeath()
            {
                sokobanState = SokobanState.dead;
            }

            /// <summary>
            /// XNA drawing
            /// </summary>
            public override void Draw()
            {
                if (isDeathAnimationStarted == true)
                {
                    currentFrame++;
                }
                else
                {
                    if (sokobanState == SokobanState.alive)
                    {
                        currentFrame = 1;
                    }
                    else
                    {
                        currentFrame = 30;
                    }
                }

                // stop animation
                if (currentFrame > numberOfFrames)
                {
                    isDeathAnimationStarted = false;
                    sokobanState = SokobanState.dead;
                }           

                SpriteElement frame = deathSpriteSheet.Sprites["dead_" + currentFrame.ToString().PadLeft(4, '0')];
                
                // looping frames
                //deathAnimationCurrentFrame = (deathAnimationCurrentFrame + 1 > deathAnimationFrames) ? 1 : deathAnimationCurrentFrame + 1;

                if (this.direction == MovementDirection.goLeft)
                {
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    spriteEffect = SpriteEffects.None;
                }

                if (movementInProgress == false)
                {

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                    spriteBatch.Draw(deathSpriteSheet.Texture,
                                     new Rectangle(1 + (this.posX - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Width) / 2),
                                                   1 + (this.posY - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Height) / 2),
                                                   frame.Rect.Width, frame.Rect.Height),
                                     frame.Rect, Color.White, 0f, new Vector2(0f, 0f), spriteEffect, 0f);
                    spriteBatch.End();
                }
                else
                {
                    if (movementCurrentFrame == 0)
                    {
                        movementCurrentFrame = 3;
                    }
                    else if (movementCurrentFrame == 3)
                    {
                        movementCurrentFrame = 5;
                    }
                    else if (movementCurrentFrame == 5)
                    {
                        movementCurrentFrame = 11;
                    }
                    else if (movementCurrentFrame == 11)
                    {
                        movementCurrentFrame = 3;
                    }

                    SpriteElement mov_frame = movementSpriteSheet.Sprites["sokobanMovement_" + (movementCurrentFrame % 6).ToString()];

                    double ratio = ((double)(this.model.time - movementStartTime) / (double)movementNoSteps);

                    if (ratio >= 1)
                    {
                        ratio = 1f;
                        movementInProgress = false;
                    }

                    int addDistance = Convert.ToInt32(Math.Round(form.squareSize * ratio));

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                    spriteBatch.Draw(movementSpriteSheet.Texture,
                                     new Rectangle(1 + (this.lastPosX - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Width) / 2) + (posX - lastPosX) * addDistance,
                                                   1 + (this.lastPosY - 1) * form.squareSize + Convert.ToInt32((form.squareSize - texture.Height) / 2) + (posY - lastPosY) * addDistance,
                                                   mov_frame.Rect.Width, mov_frame.Rect.Height),
                                     mov_frame.Rect, Color.White, 0f, new Vector2(0f, 0f), spriteEffect, 0f);

                    spriteBatch.End();
                }
            }

            public override void InitializeContent(ServiceContainer services, GraphicsDevice graphicsDevice, SoundBank soundBankParam)
            {
                base.InitializeContent(services, graphicsDevice, soundBankParam);            
                deathSpriteSheet = SpriteSheet.Load(@"Content/Images/SokobanDeathAnimation/SokobanDeath.xml", @"SokobanDeathAnimation/SokobanDeathDDS", this.content);
                movementSpriteSheet = SpriteSheet.Load(@"Content/Images/SokobanMovement/SokobanMovement.xml", @"Sokobanmovement/SokobanMovementDDS", this.content);
            }
        #endregion
    }
}