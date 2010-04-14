//#define DEBUG_MOVEMENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using Sokoban.Lib;

namespace Sokoban.Model.GameDesk
{

    public class Sokoban : GameObject
    {
        public EventType heldKeyEvent;

        public bool IsDeathAnimationStarted
        {
            get { return isDeathAnimationStarted; }
        }

        /// <summary>
        /// Actual number of steps Sokoban made so far
        /// </summary>
        public int StepsNo { get { return stepsNo; } }

        private bool isDeathAnimationStarted;
        private int numberOfFrames = 30;
        private int currentFrame = 0;
        //private SpriteSheet deathSpriteSheet = null;
        //private SpriteSheet movementSpriteSheet = null;
        private int movementCurrentFrame = 0;
        private SokobanState sokobanState;
        //private SpriteEffects spriteEffect;
        private int stepsNo;


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Sokoban(int objectID, string description,
                          int pozX, int pozY, MovementDirection direction, int speed) :
            base(objectID, description, pozX, pozY, direction, speed)
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
        }

        /// <summary>
        /// Plays sound victory sound
        /// </summary>
        public void PlayHappySound()
        {
            // Play the sound.
            //soundBank.PlayCue("SokobanHappySound");
        }

        /// <summary>
        /// Plays sound victory sound
        /// </summary>
        public void PlayScreamSound()
        {
            // Play the sound.
            //soundBank.PlayCue("Scream");
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
        /*
        public override void Draw(SpriteBatch spriteBatch, int squareSize, List<GameObject> gameObjects, Int64 time, double phase, int redrawsPerTimeUnit)
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

            if (UI.Direction == MovementDirection.goLeft)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                spriteEffect = SpriteEffects.None;
            }


            Vector2 position;
            Texture2D texture;
            SpriteElement frame;
            double scaleConst = 0.7;

            double centerXPos = squareSize * (1 - scaleConst) / 2;
            double centerYPos = squareSize * (1 - scaleConst) / 2;

#if DEBUG_MOVEMENT
            Debug.WriteLine(MovementInProgress(time).ToString() + "; Time: " + time.ToString() + "; Phase: " + phase.ToString());
            Debug.WriteLine("Start time: " + movementStartTime.ToString() + "; " + "End time: " + movementEndTime.ToString() + "; ");
            Debug.WriteLine("X,Y: " + posX.ToString() + "x" + posY.ToString() + "; last X,Y: " + lastPosX.ToString() + "x" + lastPosY.ToString());
#endif

            if (MovementInProgress(time) == false)
            {
                frame = deathSpriteSheet.Sprites["dead_" + currentFrame.ToString().PadLeft(4, '0')];
                texture = deathSpriteSheet.Texture;
                position = new Vector2(
                    (float)((posX - 1) * squareSize + centerXPos),
                    (float)((posY - 1) * squareSize + centerYPos)
                );
#if DEBUG_MOVEMENT
                Debug.WriteLine("x = " + position.X.ToString() + "; y = " + position.Y.ToString());
                Debug.WriteLine("Difference: " + Math.Abs(position.X - lastDrawedX).ToString());
#endif
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

                double step = squareSize / (double)(Speed * redrawsPerTimeUnit);
                double addDist = step * ((time - UI.MovementStartTime) * redrawsPerTimeUnit + phase);

                float x = (float)((lastPosX - 1) * squareSize + addDist * (posX - lastPosX) + centerXPos);
                float y = (float)((lastPosY - 1) * squareSize + addDist * (posY - lastPosY) + centerYPos);

#if DEBUG_MOVEMENT
                Debug.WriteLine("x = " + x.ToString() + "; y = " + y.ToString());
                Debug.WriteLine("Difference: " + Math.Abs(x - lastDrawedX).ToString());
#endif

                lastDrawedX = x;

                frame = movementSpriteSheet.Sprites["sokobanMovement_" + (movementCurrentFrame % 6).ToString()];
                texture = movementSpriteSheet.Texture;
                position = new Vector2(x, y);
            }

            // DRAW
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            double scale = (double)((double)squareSize * scaleConst / (double)frame.Rect.Width);

            spriteBatch.Draw(texture, position,
                frame.Rect, Color.White, 0f, new Vector2(0f, 0f), (float)scale, spriteEffect, 0f);
            spriteBatch.End();

#if DEBUG_MOVEMENT
            Debug.WriteLine("==");
#endif

        }

        public override void InitializeMultimedia(SoundBank soundBankParam)
        {
            
            //base.InitializeMultimedia(services, graphicsDevice, soundBankParam);
            //deathSpriteSheet = SpriteSheet.Load(ApplicationRepository.GetAppPath() + @"/Content/Images/SokobanDeathAnimation/SokobanDeath.xml", @"SokobanDeathAnimation/SokobanDeathDDS", this.content);
            //movementSpriteSheet = SpriteSheet.Load(ApplicationRepository.GetAppPath() + @"/Content/Images/SokobanMovement/SokobanMovement.xml", @"Sokobanmovement/SokobanMovementDDS", this.content);
            
        }
       
       */
       #endregion
       
    }
}
