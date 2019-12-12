using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Mandel
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Renderer renderer = new Renderer(360,240,100);
        bool RenderParalel = false;
        bool ready = false;
        double movement = 0.0;
        WriteableBitmap wb;
        public MainWindow()
        {
            InitializeComponent();            
            wb = new WriteableBitmap(360, 240,96,96,PixelFormats.Bgra32, null);            
            myImage.Source = wb;
            
            ready = true;
        }

        private void tlacitko_Click(object sender, RoutedEventArgs e)
        {
              
        }

        private void Render(int[,] output)
        {
            DateTime start = DateTime.Now;
            int stride = wb.PixelWidth * (wb.Format.BitsPerPixel / 8);
            byte[] data = new byte[stride * wb.PixelHeight];
            wb.CopyPixels(data, stride, 0);
            for (int y = 0; y < wb.PixelHeight; ++y)
            {
                for (int x = 0; x < wb.PixelWidth; ++x)
                {
                    if (output[x, y] != 0) //Nepatri
                    {
                        var index = (y * stride) + (x * 4);
                        data[index + 3] = 255;
                        data[index + 2] = (byte)(1 + (byte)output[x, y] * 1.5);
                        data[index + 1] = (byte)(1 + (byte)output[x, y] * 4);
                        data[index] = (byte)(1 + (byte)output[x, y] * 0.8);
                    }
                    else //Patri
                    {
                        var index = (y * stride) + (x * 4);
                        data[index + 3] = 255; //A
                        data[index + 2] = 0;
                        data[index + 1] = 0;
                        data[index] = 0;
                    }

                }
            }
            //wb.CopyPixels()
            wb.WritePixels(new Int32Rect(0, 0, 360, 240), data, stride, 0);
            Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
            Console.WriteLine("=====");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RenderParalel = true;
            ParalelRender();
        }

        private void CallRender()
        {
            if(ready)
            { 
                if (RenderParalel)
                    ParalelRender();
                else
                   StandartRender();
            }
        }

        private void StandartRender()
        {
            DateTime start = DateTime.Now;
            start = DateTime.Now;
            int[,] output = renderer.StandartCalc();
            Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
            Render(output); //Standart
        }

        private void ParalelRender()
        {
            DateTime start = DateTime.Now;
            start = DateTime.Now;
            int[,] output = renderer.ParalelCalc();
            Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
            Render(output); //Paralel
        }
        private void StandartButton_Click(object sender, RoutedEventArgs e)
        {            
            RenderParalel = false;
            StandartRender();
        }

        private void text_xpos_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
                renderer.xPosition = double.Parse(text_xpos.Text);
            }
            catch
            {

            }
            CallRender();
        }

        private void text_ypos_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
                renderer.yPosition = double.Parse(text_ypos.Text);
            }
            catch
            {

            }
            CallRender();
        }

        private void text_zoom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                renderer.zoom = double.Parse(text_zoom.Text);
            }
            catch
            {

            }
            CallRender();
        }        

        private void text_maxIter_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
                renderer.maxIter = int.Parse(text_maxIter.Text);
            }
            catch
            {

            }
            CallRender();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.A))
            {
                renderer.xPosition -= double.Parse(text_offset.Text);
            }
            if (Keyboard.IsKeyDown(Key.D))
            {
                renderer.xPosition += double.Parse(text_offset.Text);
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                renderer.yPosition += double.Parse(text_offset.Text);
            }
            if (Keyboard.IsKeyDown(Key.W))
            {
                renderer.yPosition -= double.Parse(text_offset.Text);
            }
            if(Keyboard.IsKeyDown(Key.Q))
            {
                renderer.zoom -= double.Parse(text_offset.Text);
            }
            if (Keyboard.IsKeyDown(Key.E))
            {
                renderer.zoom += double.Parse(text_offset.Text);
            }
            if(Keyboard.IsKeyDown(Key.R))
            {
                renderer.maxIter++;
            }
            if (Keyboard.IsKeyDown(Key.F))
            {
                renderer.maxIter--;                
            }
            CallRender();

            text_xpos.Text = renderer.xPosition.ToString();
            text_ypos.Text = renderer.yPosition.ToString();
            text_zoom.Text = renderer.zoom.ToString();
            text_maxIter.Text = renderer.maxIter.ToString();
        }

        private void text_offset_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_inc1_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.1;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy18_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.1;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.01;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy1_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy2_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.00001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy4_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy5_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy6_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.00000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy7_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy8_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy9_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.00000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy10_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy11_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy12_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.00000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy13_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy14_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy15_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.00000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy16_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.000000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy17_Click(object sender, RoutedEventArgs e)
        {
            movement += 0.0000000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy19_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.01;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy20_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy21_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy22_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.00001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy23_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy24_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy25_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.00000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy26_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy27_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy28_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.00000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy29_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy30_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy31_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.00000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy32_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy33_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy34_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.00000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy35_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.000000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void button_inc1_Copy36_Click(object sender, RoutedEventArgs e)
        {
            movement -= 0.0000000000000000001;
            text_offset.Text = movement.ToString();
        }

        private void Button_snapshot_Click(object sender, RoutedEventArgs e)
        {
            
            WriteableBitmap bigMap = new WriteableBitmap(1920, 1080, 96, 96, PixelFormats.Bgra32, null);
            Renderer snapshotRenderer = new Renderer(1920, 1080, renderer.maxIter);
            snapshotRenderer.xPosition = renderer.xPosition;
            snapshotRenderer.yPosition = renderer.yPosition;
            snapshotRenderer.zoom = renderer.zoom;

            int[,] output = snapshotRenderer.ParalelCalc();
            int stride = bigMap.PixelWidth * (bigMap.Format.BitsPerPixel / 8);
            byte[] data = new byte[stride * bigMap.PixelHeight];
            bigMap.CopyPixels(data, stride, 0);
            for (int y = 0; y < bigMap.PixelHeight; ++y)
            {
                for (int x = 0; x < bigMap.PixelWidth; ++x)
                {
                    if (output[x, y] != 0) //Nepatri
                    {
                        var index = (y * stride) + (x * 4);
                        data[index + 3] = 255;
                        data[index + 2] = (byte)(1 + (byte)output[x, y] * 1.5);
                        data[index + 1] = (byte)(1 + (byte)output[x, y] * 4);
                        data[index] = (byte)(1 + (byte)output[x, y] * 0.8);
                    }
                    else //Patri
                    {
                        var index = (y * stride) + (x * 4);
                        data[index + 3] = 255; //A
                        data[index + 2] = 0;
                        data[index + 1] = 0;
                        data[index] = 0;
                    }

                }
            }
            //wb.CopyPixels()
            bigMap.WritePixels(new Int32Rect(0, 0, 1920, 1080), data, stride, 0);
            
            SaveBitmap("test.png", bigMap);
            Process.Start("test.png");
        }


        void SaveBitmap(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
            }
        }
    }
}
