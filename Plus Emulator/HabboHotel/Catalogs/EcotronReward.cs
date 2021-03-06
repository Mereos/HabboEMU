using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Catalogs
{
    /// <summary>
    /// Class EcotronReward.
    /// </summary>
    internal class EcotronReward
    {
        /// <summary>
        /// The display identifier
        /// </summary>
        internal uint DisplayId;

        /// <summary>
        /// The base identifier
        /// </summary>
        internal uint BaseId;

        /// <summary>
        /// The reward level
        /// </summary>
        internal uint RewardLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcotronReward"/> class.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <param name="baseId">The base identifier.</param>
        /// <param name="rewardLevel">The reward level.</param>
        internal EcotronReward(uint displayId, uint baseId, uint rewardLevel)
        {
            DisplayId = displayId;
            BaseId = baseId;
            RewardLevel = rewardLevel;
        }

        /// <summary>
        /// Gets the base item.
        /// </summary>
        /// <returns>Item.</returns>
        internal Item GetBaseItem()
        {
            return Plus.GetGame().GetItemManager().GetItem(BaseId);
        }
    }
}