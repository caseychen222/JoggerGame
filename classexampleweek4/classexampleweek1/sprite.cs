using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;

// "hit" and "win" sound effects from SoundBible.com
// "loss" sound effects from "The Price Is Right"

// primarily working with keyUp and keyDown, not so much keyPress
// get sprite size - take dimension (ex. megaman 1280x754, how many sprites across and down (4x2), so 1280/4 and 754/2 are the individual sprite's dimensions)

namespace classexampleweek1
{
    public class sprite
    {
       public SoundPlayer soundEffects;
       public Rectangle r;
       public bool hasBeenHit, beenWon = false;
       public int x, y, width, height, xPos, iRow, iCol, currentFrame, frames, numHits = 0;
       public int livesLeft = 2;
       public int drivingAlong = 3;
       public int steps = 20;
       public double dx, dy = 0.0;

       public Bitmap cars, trucks, player, faceimage, picture;
       private Bitmap spriteMap;

       public Timer spriteTimer;

       public sprite(Point location, int size, double dx, double dy)
       {
           this.x = location.X;
           this.y = location.Y;
           this.dx = dx;
           this.dy = dy;
           this.width = size;
           this.height = size;

           r = new Rectangle(x, y, size, size * 2);
       }

       public void loadimagemap()
       {
           spriteMap = (Bitmap)Bitmap.FromFile("spritemap_half.png");
           iRow = 0; // Y
           iCol = 0; // X

           spriteTimer = new Timer();
           spriteTimer.Tick += new EventHandler(spriteTimer_Tick);
           spriteTimer.Interval = 1;
           spriteTimer.Enabled = true;

           player = (Bitmap)cropImage(spriteMap, new Rectangle(iCol * 56, iRow * 100, 56, 100));
           player.MakeTransparent(player.GetPixel(1, 1));
       }

       private static Image cropImage(Image img, Rectangle cropArea)
       {
           Bitmap bmpImage = new Bitmap(img);
           Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);

           return (Image)bmpCrop;
       }

       void spriteTimer_Tick(object sender, EventArgs e)
       {
           if (currentFrame > 2)
           {
               currentFrame = 0;
           }

           player = (Bitmap)cropImage(spriteMap, new Rectangle(iCol * 56, iRow * 100, 56, 100));
           player.MakeTransparent(player.GetPixel(1, 1));

           iCol = currentFrame;
       }

       public int movePlayer(ref bool[] keys, ref int score)
       {
           // while the player is still in the game (numHits is not 3), and the player is out of the starting zone then add 50 points to the score
           if (numHits != 3 && (r.Y + r.Height) > 110)
           {
               score += 50;
           }
           
           // reset the score to 0 if the player has been hit three times, and set livesLeft to 0
           if (hasBeenHit)
           {
               score = 0;
               livesLeft = 0;
           }

           if (keys[3]) // moving right
           {
               dx = steps;
               dy = 0;
               currentFrame++;
               iRow = 0;
           }

           else if (keys[2]) // moving left
           {
               dx = steps * -1;
               dy = 0;
               currentFrame++;
               iRow = 1;
           }

           else if (keys[1]) // moving down
           {
               dy = steps;
               dx = 0;
               currentFrame++;
               iRow = 2;
           }

           else if (keys[0]) // moving up
           {
               dy = steps * -1;
               dx = 0;
               currentFrame++;
               iRow = 3;
           }

           else {
               dx = 0;
           }

           x += (int)Math.Round(dx);
           y += (int)Math.Round(dy);

           r.X = x;
           r.Y = y;

           return score;
       }

       // checks to see if the player is hit by checking if the player collided with the truck or car
       public void isHit(vehicle carVeh, vehicle truckVeh, ref int score)
       {
           if (carVeh.r.IntersectsWith(this.r) || truckVeh.r.IntersectsWith(this.r))
           {
               playEffect("hit"); // play the hit sound effect

               livesLeft--; // subtract one from the number of lives left
               numHits++; // add one to the number of hits

               // if the player is hit three times, they are out of chances, and the hasBeenHit is set to true, thereby triggering game over
               if (numHits == 3)
               {
                   hasBeenHit = true;
                   livesLeft = 0;

                   playEffect("loss"); // play the losing sound effect
               }

                if ((score - 200) <= 0)
                {
                    score = 0;
                }
                else
                {
                    score -= 200; // subtract 200 points each time the player is hit
                }

               // reset the player to the starting position
               x = (Form1.ActiveForm.Width) / 2 - 62;
               y = 5;

               r.X = x;
               r.Y = y;
           }
       }

       public void isSafe(Rectangle safeZone)
       {
           // checks to see if the player has collided with (made it to) the safe zone, thereby winning the game
           if (this.r.IntersectsWith(safeZone) && !hasBeenHit)
           {
               beenWon = true;

               playEffect("win"); // play the winning sound effect from the playEffect() function
           }
       }

       // function to play a sound effect depending on what happened
       public void playEffect(string whatEffect)
       {
           soundEffects = new SoundPlayer(); // set up the sound player

           // play a sound effect based on the event that occured
           switch (whatEffect)
           {
               case "traffic": // the traffic background noise
                   soundEffects.SoundLocation = "traffic.wav";
                   soundEffects.Play(); // loop the traffic background noise
                   break;
               case "hit": // if the player is hit
                   soundEffects.SoundLocation = "splat.wav";
                   soundEffects.Play();
                   playEffect("traffic");
                   break;
               case "win": // if the player wins
                   soundEffects.Stop(); // stop the playing of the background traffic noise
                   soundEffects.SoundLocation = "win.wav";
                   break;
               case "loss": // if the player loses (runs out of lives / gets hit three times)
                   soundEffects.Stop(); // stop the playing of the background traffic noise
                   soundEffects.SoundLocation = "loss.wav";
                   break;
           }

           soundEffects.Play(); // play the sound effect
       }

       public void update()
       {
           //currentFrame++;
           r.X = x;
           r.Y = y;
       }
    }
}