namespace PleiadInput
{
    /// <summary>
    /// Required to add input checks to the system
    /// </summary>
    public interface IRegisterInput
    {
        /// <summary>
        /// Register keys for listening
        /// </summary>
        /// <param name="listener">Input Listener</param>
        void InputRegistration(ref InputListener listener);
    }
}
