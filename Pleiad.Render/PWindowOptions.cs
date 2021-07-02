using Silk.NET.Windowing;


namespace Pleiad.Render
{
    public struct WindowResolution
    {
        //options.Size = new Vector2D<int>(800, 600);                 
        //options.Title = "Test Window";
        //options.VSync = true;
        public WindowResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public int Width { get; init; }
        public int Height { get; init; }
    }
    public class PWindowOptions : IPWindowOptions
    {
        public WindowOptions SilkOptions
        {
            get
            {
                WindowOptions options = WindowOptions.Default;
                options.Size = new Silk.NET.Maths.Vector2D<int>(Resolution.Width, Resolution.Height);
                options.Title = Title;
                options.VSync = VSync;
                return options;
            }
        }
        public string Title { get; init; } = "Default Title";
        public WindowResolution Resolution { get; init; } = new WindowResolution(800, 600);
        public bool VSync { get; init; } = true;
    }
}
