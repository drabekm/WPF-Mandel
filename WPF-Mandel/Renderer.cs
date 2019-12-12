using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media;
using System.Diagnostics;
using System.Data;
using System.Threading;

namespace WPF_Mandel
{
    class Renderer
    {
        public double xPosition, yPosition, zoom;
        double width, height;
        public int maxIter;
        object zamek = new object();
        int[,] output;
        public Renderer(int width, int height, int maxIter)
        {
            this.maxIter = maxIter;
            this.width = width;
            this.height = height;
            output = new int[width, height];
        }

        /// <summary>
        /// This is used when we want to zoom/move in the mandelbrot set. 
        /// Normally corners of the image would be in a range from -2 to + 2. If we want to move around and zoom into the image
        /// we need to use different corners.
        /// 
        /// Kurva snad to píšu dobře. Anglicky umím akorát tak nadávat
        /// </summary>
        /// <returns>
        /// Returns an array with new corners of our image. First number is xMin then xMax, yMin, yMax
        /// </returns>
        private double[] CalcZoom()
        {
            double[] output = new double[4];
            output[0] = xPosition - 2.0 * zoom; //xMin
            output[1] = xPosition + 2.0 * zoom; //xMax
            output[2] = yPosition - 2.0 * zoom; //yMin
            output[3] = yPosition + 2.0 * zoom; //yMax
            return output;
        }

        /// <summary>
        /// Calculate mandelbrot using a single thread
        /// </summary>
        public int[,] StandartCalc()
        {
            double[] corners = CalcZoom();
            //Array of points of the set. 1 == isn't part of mandelbrot. 0 == is part of mandelbrot

            Complex c, z, z2; // c == a point which we are trying to determine if it belongs to the set or not
                              // z = z^2 + c    |z| > 2 => not in the set
            double xmax = 2, xmin = -2, ymax = 2, ymin = -2, x , y;
            xmin = corners[0];
            xmax = corners[1];
            ymin = corners[2];
            ymax = corners[3];

            y = ymin;
            for(int j = 0; j < height; j++)
            {
                x = xmin;
                for(int i = 0; i <width; i++)
                {
                    c = new Complex(x, y);
                    z = new Complex(0, 0);
                    int iter = 0;
                    do
                    {
                        z2 = new Complex(z.Real * z.Real, z.Imaginary * z.Imaginary);
                        z = new Complex(z2.Real - z2.Imaginary + c.Real, 2.0*z.Real*z.Imaginary+c.Imaginary);
                        iter++;
                    } while (iter < maxIter && (z2.Real + z2.Imaginary) < 4.0);

                    if(iter == maxIter)
                    {
                        output[i, j] = 0;
                    }
                    else
                    {
                        output[i, j] = iter;
                    }
                    x += (xmax - xmin) / width;
                }
                y += (ymax - ymin) / height;
            }
            return output;
        }

        public void RenderMandel()
        {
            
        }

        /// <summary>
        /// TODO: Calculating mandelbrot using multiple threads
        /// </summary>
        public int[,] ParalelCalc()
        {
            double[] corners = CalcZoom();
            double xmax = 2, xmin = -2, ymax = 2, ymin = -2, x, y;
            xmin = corners[0];
            xmax = corners[1];
            ymin = corners[2];
            ymax = corners[3];
            //Array of points of the set. 1 == isn't part of mandelbrot. 0 == is part of mandelbrot
            Task task1 = Task.Factory.StartNew(() => ParalelCalcHelper(xmin, xmax, ymin, (ymax + ymin) / 2, 0, (int)width, 0, (int)height/2, ymin, ymax));
            Task task2 = Task.Factory.StartNew(() => ParalelCalcHelper(xmin, xmax, (ymax + ymin) / 2, ymax, 0, (int)width, (int)height/2, (int)height, ymin, ymax));
            Task.WaitAll(task1, task2);
            return output;
        }

        private void ParalelCalcHelper(double xmin, double xmax, double ymin, double ymax, int widthStart, int widthEnd, int heightStart, int heightEnd, double originalYposMin, double originalYposMax)
        {            
            Complex c, z, z2; // c == a point which we are trying to determine if it belongs to the set or not
                              // z = z^2 + c    |z| > 2 => not in the set
            double x, y;



            y = ymin;
            for (int j = heightStart; j < heightEnd; j++)
            {
               
                x = xmin;
                for (int i = widthStart; i < widthEnd; i++)
                {
                    c = new Complex(x, y);
                    z = new Complex(0, 0);
                    int iter = 0;
                    do
                    {
                        z2 = new Complex(z.Real * z.Real, z.Imaginary * z.Imaginary);
                        z = new Complex(z2.Real - z2.Imaginary + c.Real, 2.0 * z.Real * z.Imaginary + c.Imaginary);
                        iter++;
                    } while (iter < maxIter && (z2.Real + z2.Imaginary) < 4.0);

                    if (iter == maxIter)
                    {
                        //lock (zamek)
                        //{
                            output[i, j] = 0;
                        //}

                    }
                    else
                    {
                        //lock (zamek)
                        //{
                            output[i, j] = iter;
                        //}
                    }
                    x += (xmax - xmin) / width;
                }
                y += (originalYposMax - originalYposMin) / height;
            }            
        }

    }
}
