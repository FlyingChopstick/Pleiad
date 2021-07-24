using Silk.NET.Windowing;


namespace Pleiad.Render.Windows
{
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
        public PWindowResolution Resolution { get; init; } = new PWindowResolution(800, 600);
        public bool VSync { get; init; } = true;
    }
}
