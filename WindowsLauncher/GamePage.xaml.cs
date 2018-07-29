using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

using Microsoft.Graphics.Canvas.UI.Xaml;

using GridEngine;
using GridEngine.Engine;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private Engine engine;

        public CanvasSize CanvasSize { get { return new CanvasSize { Width = this.ActualWidth, Height = this.ActualHeight }; } }

        public CanvasBitmap[,]  DisplayGrid { get; private set; }




        Random rnd = new Random();
        private Vector2 RndPosition()
        {
            double x = rnd.NextDouble() * 500f;
            double y = rnd.NextDouble() * 500f;
            return new Vector2((float)x, (float)y);
        }

        private float RndRadius()
        {
            return (float)rnd.NextDouble() * 150f;
        }

        private byte RndByte()
        {
            return (byte)rnd.Next(256);
        }



        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            XmlDocument gameData = (XmlDocument)e.Parameter;

            // Create the engine
            engine = new Engine(gameData);
            //types.GetType(types.GetName().Name + ".Game").GetField("engine").SetValue(null, new Engine(gameData, types.GetType(types.GetName().Name + ".Game").GetNestedType("Methods"), types.GetType(types.GetName().Name + ".Game").GetNestedType("Entities")));

            //engine = (Engine)types.GetType(types.GetName().Name + ".Game").GetField("engine").GetValue(null);

            // Subscribe to closing the engine
            //((Engine)types.GetType(types.GetName().Name + ".Game").GetField("engine").GetValue(null)).EngineStop += ReturnToMenu;
            engine.EngineStop += ReturnToMenu;

            // Start the engine
            //((Engine)types.GetType(types.GetName().Name + ".Game").GetField("engine").GetValue(null)).Start();
            engine.Start();
        }

        public GamePage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        private void canvas_AnimatedDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            double imageWidth = this.CanvasSize.Width / DisplayGrid.GetLength(0);
            double imageHeight = this.CanvasSize.Height / DisplayGrid.GetLength(1);
            
            for (int x = 0; x < DisplayGrid.GetLength(0); x++)
            {
                for (int y = 0; y < DisplayGrid.GetLength(1); y++)
                {
                    args.DrawingSession.DrawImage(DisplayGrid[x, y], new Rect { X = x * imageWidth, Y = y * imageHeight, Width = imageWidth, Height = imageHeight });
                }
            }

            //float radius = (float)(1 + Math.Sin(args.Timing.TotalTime.TotalSeconds)) * 10f;
            //blur.BlurAmount = radius;
            //args.DrawingSession.DrawImage(blur);
        }

        void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        GaussianBlurEffect blur;
        private void canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                for (int i = 0; i < 100; i++)
                {
                    clds.DrawText("Hello, World!", RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                    clds.DrawCircle(RndPosition(), RndRadius(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                    clds.DrawLine(RndPosition(), RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                }
            }

            blur = new GaussianBlurEffect();
            blur.Source = cl;
            blur.BlurAmount = 10.0f;
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            string[,] entityGrid = engine.ActiveArea.GetEntityGrid();

            double imageWidth = this.CanvasSize.Width / DisplayGrid.GetLength(0);
            double imageHeight = this.CanvasSize.Height / DisplayGrid.GetLength(1);

            int[][] updates = engine.ActiveArea.GetUpdates();// method should clear cashe

            foreach (int[] location in updates)
            {
                if (entityGrid[location[0], location[1]] != null)
                {
                    DisplayGrid[location[0], location[1]] = engine.ActiveArea.Entities[entityGrid[location[0], location[1]]].Image;
                }
                else
                {
                    DisplayGrid[location[0], location[1]] = null;
                }
                
            }
        }

        public void ReturnToMenu(object sender, EngineStopEventArgs e)
        {
            // Do stuf here e.g. save

            Frame.Navigate(typeof(HomePage));
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            engine.ActiveArea.OnRaiseKeyPress(new GridEngine.Areas.KeyPressEventArgs(GridEngine.Enums.InputKeys.FromValue((int)e.Key)));
        }
    }

    public struct CanvasSize
    {
        public double Width;

        public double Height;
    }
}
