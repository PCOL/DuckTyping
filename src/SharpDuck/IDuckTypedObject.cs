namespace SharpDuck
{
    /// <summary>
    /// Defines members of the duck typed interface.
    /// </summary>
    public interface IDuckTypedObject
    {
        /// <summary>
        /// Gets the instance of the duck typed object.
        /// </summary>
        object DuckTypedObject { get; }
    }
}
