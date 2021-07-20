namespace Pleiad.Systems.Interfaces
{
    public interface IPleiadSystem
    {
        /// <summary>
        /// Function to execute on every update
        /// </summary>
        /// <param name="dTime">Time since the last update</param>
        void Cycle(double dTime);
    }
}
