using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Sokoban.View.GameDocsComponents;
using System.Windows.Data;
using System.Windows.Documents;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl
    {
        private Fader fader = null;

        private void applyBlurEffect()
        {
            // Initialize a new BlurBitmapEffect that will be applied
            // to the Button.
            BlurBitmapEffect myBlurEffect = new BlurBitmapEffect();

            // Set the Radius property of the blur. This determines how 
            // blurry the effect will be. The larger the radius, the more
            // blurring. 
            myBlurEffect.Radius = 15;

            // Set the KernelType property of the blur. A KernalType of "Box"
            // creates less blur than the Gaussian kernal type.
            myBlurEffect.KernelType = KernelType.Box;

            // Apply the bitmap effect to the Button.
            gamedeskCanvas.BitmapEffect = myBlurEffect;
            gamedeskOpponentCanvas.BitmapEffect = myBlurEffect;
            //gameDeskControlParent.BitmapEffect = myBlurEffect;
        }

        private void removeBlurEffect()
        {
            gamedeskCanvas.BitmapEffect = null;
            gamedeskOpponentCanvas.BitmapEffect = null;
            //gameDeskControlParent.BitmapEffect = null;
        }

        private void applyShadeEffect()
        {
            this.fader = new Fader(gameDeskControlParent, Color.FromArgb(128,0,0,0), "Game Over");
            AdornerLayer.GetAdornerLayer(gameDeskControlParent).Add(fader);
            
        }

        private void removeShadeEffect()
        {
            if (fader != null)
            {
                AdornerLayer.GetAdornerLayer(gameDeskControlParent).Remove(this.fader);
            }            
        }
    }
}
