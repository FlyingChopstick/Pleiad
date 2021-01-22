namespace PleiadMisc
{
    /// <summary>
    /// Wrapper struct providing a way to check if result is found and to protect from accidental <see langword="null"/> values
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Use that if result is not found
        /// </summary>
        public static Result<T> NotFound => new Result<T>();
        /// <summary>
        /// Pack the result
        /// </summary>
        /// <param name="data"></param>
        public static Result<T> Found(T data) => new Result<T>(data);

        private Result()
        {
            IsFound = false;
            Data = default;
        }
        private Result(T data)
        {
            IsFound = true;
            Data = data;
        }

        /// <summary>
        /// Was result found
        /// </summary>
        public bool IsFound { get; private set; } = false;
        /// <summary>
        /// Result
        /// </summary>
        public T Data { get; private set; } = default;
    }
}
