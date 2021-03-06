using Plus.Messages;
using Plus.Messages.Parsers;

namespace Plus.HabboHotel.Achievements.Composer
{
    /// <summary>
    /// Class AchievementScoreUpdateComposer.
    /// </summary>
    internal class AchievementScoreUpdateComposer
    {
        /// <summary>
        /// Composes the specified score.
        /// </summary>
        /// <param name="Score">The score.</param>
        /// <returns>ServerMessage.</returns>
        internal static ServerMessage Compose(int Score)
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("AchievementPointsMessageComposer"));
            serverMessage.AppendInteger(Score);
            return serverMessage;
        }
    }
}