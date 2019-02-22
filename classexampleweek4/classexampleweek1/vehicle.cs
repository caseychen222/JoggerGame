using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace classexampleweek1
{
    public class vehicle
    {
        public Bitmap cars, trucks;
        public Rectangle r;
        public List<Bitmap> carsList, trucksList;
        public int frames, x, y, width, height, currentFrame = 0;
        public int drivingAlong = 3;
        public double dx, dy;

        public void loadimages()
        {
            cars = (Bitmap)Bitmap.FromFile("car1.png");
            trucks = (Bitmap)Bitmap.FromFile("truck1.png");

            carsList = new List<Bitmap>();
            trucksList = new List<Bitmap>();

            //Bitmap car, truck;

            cars.MakeTransparent(cars.GetPixel(1, 1));
            trucks.MakeTransparent(trucks.GetPixel(1, 1));

            /*for (int i = 1; i < 4; i++)
            {
                car = (Bitmap)Bitmap.FromFile("car" + i.ToString() + ".png");
                car.MakeTransparent(car.GetPixel(1, 1));

                truck = (Bitmap)Bitmap.FromFile("truck" + i.ToString() + ".png");
                truck.MakeTransparent(truck.GetPixel(1, 1));

                carsList.Add(car);
                trucksList.Add(truck);

                frames++;
            }*/
        }

       public vehicle(Point location, int size, double dx, double dy)
       {
           this.x = location.X;
           this.y = location.Y;
           this.dx = dx;
           this.dy = dy;
           this.width = size;
           this.height = size;

           r = new Rectangle(x, y, size + 87, size);
       }

       public void update(int w, char v)
       {
           if (v == 't') // for cars - moving right to left
           {
               // if the leftmost point of the car is past the form's left edge, then reset the car's leftmost point x to the width of the form
               if (r.X < (0 - trucks.Width))
               {
                   x = w;
               }

               // this causes the car to appear to be travelling slightly faster, adding a bit of difficulty
               dx = drivingAlong + 2;

               x -= (int)Math.Round(dx);
               y -= (int)Math.Round(dy);
           }
           else // for trucks - moving left to right
           {
               // if the rightmost point of the truck is past the form's right edge, then reset the truck's rightmost point x to the width of the form
               if (r.X > w)
               {
                   x = 0 - cars.Width;
               }

               // cause the truck to "drive" along at the rate of 3 pixels to the right
               dx = drivingAlong;

               x += (int)Math.Round(dx);
               y += (int)Math.Round(dy);
           }

           //currentFrame++;
           r.X = x;
           r.Y = y;
       }
    }
}
