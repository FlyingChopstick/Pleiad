namespace PleiadEntities
{
    /// <summary>
    /// Wrapper struct providing a way to check if result is found and to protect from accidental <see langword="null"/> values
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public struct SearchResult<T>
    {
        /// <summary>
        /// Use that if result is not found
        /// </summary>
        /// <param name="isFound">The value is not used - set whatever</param>
        public SearchResult(bool isFound)
        {
            IsFound = false;
            Result = default;
        }
        /// <summary>
        /// Pack the result
        /// </summary>
        /// <param name="data"></param>
        public SearchResult(T data)
        {
            IsFound = true;
            Result = data;
        }

        /// <summary>
        /// Was result found
        /// </summary>
        public bool IsFound { get; private set; }
        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; private set; }
    }
}
