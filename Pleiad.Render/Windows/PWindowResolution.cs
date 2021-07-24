namespace Pleiad.Render.Windows
{
    public struct PWindowResolution
    {
        //options.Size = new Vector2D<int>(800, 600);                 
        //options.Title = "Test Window";
        //options.VSync = true;
        public PWindowResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
