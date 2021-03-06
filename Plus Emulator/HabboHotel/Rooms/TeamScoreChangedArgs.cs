using Plus.HabboHotel.Rooms.Games;
using System;

namespace Plus.HabboHotel.Rooms
{
    /// <summary>
    /// Class TeamScoreChangedArgs.
    /// </summary>
    public class TeamScoreChangedArgs : EventArgs
    {
        /// <summary>
        /// The points
        /// </summary>
        internal readonly int Points;

        /// <summary>
        /// The team
        /// </summary>
        internal readonly Team Team;

        /// <summary>
        /// The user
        /// </summary>
        internal readonly RoomUser User;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamScoreChangedArgs"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="team">The team.</param>
        /// <param name="user">The user.</param>
        public TeamScoreChangedArgs(int points, Team team, RoomUser user)
        {
            this.Points = points;
            this.Team = team;
            this.User = user;
        }
    }
}