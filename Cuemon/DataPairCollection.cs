using System.Collections.ObjectModel;

namespace Cuemon
{
    /// <summary>
    /// Provides a collection of <see cref="DataPair"/>. This class cannot be inherited.
    /// </summary>
    public sealed class DataPairCollection : Collection<DataPair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPairCollection"/> class.
        /// </summary>
        public DataPairCollection() : base()
        {
        }

        /// <summary>
        /// Adds a new <see cref="DataPair{T}"/> to the end of this <see cref="DataPairCollection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data being added to this instance.</typeparam>
        /// <param name="name">The name of the data pair.</param>
        /// <param name="value">The value of the data pair.</param>
        public void Add<T>(string name, T value)
        {
            this.Add(new DataPair<T>(name, value));
        }
    }
}
