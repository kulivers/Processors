namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a unique id provider.
    /// </summary>
    public class MessageBrokerIdProvider
    {
        #region Fields

        private long id;
        public static readonly MessageBrokerIdProvider Instance = new MessageBrokerIdProvider((ulong)DateTime.UtcNow.Ticks);
        private readonly uint basis1;
        private readonly ushort basis2;
        private readonly ushort basis3;

        #endregion

        #region Ctors

        public MessageBrokerIdProvider(ulong basis)
        {
            this.basis1 = (uint)(basis >> 32); // 32-64
            this.basis2 = (ushort)(basis >> 16); // 16-32
            this.basis3 = (ushort)basis; // 0-16
        }

        #endregion

        #region Members

        /// <summary>
        /// Get the next unique identifier.
        /// </summary>
        /// <returns>The id</returns>
        public Guid GetId()
        {
            var current = System.Threading.Interlocked.Increment(ref this.id);

            return new Guid(
                this.basis1,
                this.basis2,
                this.basis3,
                (byte)(current >> 56), // 56-64 - might be reserved for server ID
                (byte)(current >> 48), // 48-56 - might be reserved for server ID
                (byte)(current >> 40), // 40-48
                (byte)(current >> 32), // 32-40
                (byte)(current >> 24), // 24-32
                (byte)(current >> 16), // 16-24
                (byte)(current >> 8),  // 8-16
                (byte)current);        // 0-8
        }

        /// <summary>
        /// Gets the next  unique identifier.
        /// </summary>
        /// <returns>The string id</returns>
        public static string GetStringId()
        {
            return Instance.GetId().ToString();
        }

        #endregion
    }
}

