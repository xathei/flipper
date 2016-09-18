using FFACETools;

namespace Flipper.Classes
{
    /// <summary>
    /// POCO class that holds player information for party members.
    /// </summary>
    public class PartyMember
    {
        /// <summary>
        /// The name of the member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The job of the member.
        /// </summary>
        public Job Job { get; set; }

        /// <summary>
        /// The current HP of the member.
        /// </summary>
        public int HpCurrent { get; set; }

        /// <summary>
        /// The current Max HP of the member.
        /// </summary>
        public int HpCurrentMax{ get; set; }

        /// <summary>
        /// The current amount of HP the member is missing.
        /// </summary>
        public int HpMissing { get; set; }
    }
}
