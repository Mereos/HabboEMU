﻿using Plus.Configuration;
using Plus.Connection.ServerManager;
using Plus.Database.Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups.Structs;
using Plus.HabboHotel.Navigators;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Inventory;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Users.Subscriptions;
using Plus.HabboHotel.Users.UserDataManagement;
using Plus.HabboHotel.Roleplay.Minigames.Colour_Wars;
using Plus.Messages;
using Plus.Messages.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;

namespace Plus.HabboHotel.Users
{
    /// <summary>
    /// Class Habbo.
    /// </summary>
    public class Habbo
    {
        /// <summary>
        /// The guide other user
        /// </summary>
        public GameClient GuideOtherUser;

        /// <summary>
        /// The identifier
        /// </summary>
        internal uint Id;

        /// <summary>
        /// The user name
        /// </summary>
        internal string UserName, RealName, Motto, Look, Gender;

        /// <summary>
        /// The create date
        /// </summary>
        internal double CreateDate;

        /// <summary>
        /// The rank
        /// </summary>
        internal uint Rank;

        /// <summary>
        /// The last change
        /// </summary>
        internal int LastChange;

        /// <summary>
        /// The credits
        /// </summary>
        internal int Credits, AchievementPoints, ActivityPoints, BelCredits;

        /// <summary>
        /// The muted
        /// </summary>
        internal bool Muted;

        /// <summary>
        /// The respect
        /// </summary>
        internal int Respect, DailyRespectPoints, DailyPetRespectPoints, DailyCompetitionVotes;

        /// <summary>
        /// The loading room
        /// </summary>
        internal uint LoadingRoom;

        /// <summary>
        /// The loading checks passed
        /// </summary>
        internal bool LoadingChecksPassed;

        /// <summary>
        /// The current room identifier
        /// </summary>
        public uint CurrentRoomId;

        /// <summary>
        /// The home room
        /// </summary>
        internal uint HomeRoom;

        /// <summary>
        /// The last online
        /// </summary>
        internal int LastOnline;

        /// <summary>
        /// The previous online
        /// </summary>
        internal int PreviousOnline;

        /// <summary>
        /// The is teleporting
        /// </summary>
        internal bool IsTeleporting;

        /// <summary>
        /// The is hopping
        /// </summary>
        internal bool IsHopping;

        /// <summary>
        /// The teleporting room identifier
        /// </summary>
        internal uint TeleportingRoomId;

        /// <summary>
        /// The teleporter identifier
        /// </summary>
        internal uint TeleporterId;

        /// <summary>
        /// The hopper identifier
        /// </summary>
        internal uint HopperId;

        /// <summary>
        /// The favorite rooms
        /// </summary>
        internal List<uint> FavoriteRooms;

        /// <summary>
        /// The muted users
        /// </summary>
        internal List<uint> MutedUsers;

        /// <summary>
        /// The tags
        /// </summary>
        internal List<string> Tags;

        /// <summary>
        /// The achievements
        /// </summary>
        internal Dictionary<string, UserAchievement> Achievements;

        /// <summary>
        /// The talents
        /// </summary>
        internal Dictionary<int, UserTalent> Talents;

        /// <summary>
        /// The rated rooms
        /// </summary>
        internal HashSet<uint> RatedRooms;

        /// <summary>
        /// The recently visited rooms
        /// </summary>
        internal LinkedList<uint> RecentlyVisitedRooms;

        /// <summary>
        /// The spectator mode
        /// </summary>
        internal bool SpectatorMode;

        /// <summary>
        /// The disconnected
        /// </summary>
        internal bool Disconnected;

        /// <summary>
        /// The has friend requests disabled
        /// </summary>
        internal bool HasFriendRequestsDisabled;

        /// <summary>
        /// The users rooms
        /// </summary>
        internal HashSet<RoomData> UsersRooms;

        /// <summary>
        /// The user groups
        /// </summary>
        internal HashSet<GroupUser> UserGroups;

        /// <summary>
        /// The favourite group
        /// </summary>
        internal uint FavouriteGroup;

        /// <summary>
        /// The spam protection bol
        /// </summary>
        internal bool SpamProtectionBol;

        /// <summary>
        /// The spam protection count
        /// </summary>
        internal int SpamProtectionCount = 1, SpamProtectionTime, SpamProtectionAbuse;

        /// <summary>
        /// The friend count
        /// </summary>
        internal uint FriendCount;

        /// <summary>
        /// The spam flood time
        /// </summary>
        internal DateTime SpamFloodTime;

        /// <summary>
        /// The quests
        /// </summary>
        internal Dictionary<uint, int> Quests;

        /// <summary>
        /// The current quest identifier
        /// </summary>
        internal uint CurrentQuestId;

        /// <summary>
        /// The last quest completed
        /// </summary>
        internal uint LastQuestCompleted;

        /// <summary>
        /// The flood time
        /// </summary>
        internal int FloodTime;

        /// <summary>
        /// TimeLoggedOn
        /// </summary>
        internal DateTime TimeLoggedOn;
        internal int TimeLoggedOn1;

        /// <summary>
        /// The hide in room
        /// </summary>
        internal bool HideInRoom;

        /// <summary>
        /// The appear offline
        /// </summary>
        internal bool AppearOffline;

        internal RoomUser GetRoomUser()
        {
            RoomUser RUser = null;
            try
            {
                if (CurrentRoom != null)
                {
                    RUser = CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(UserName);
                }
            }
            catch
            {
                return RUser;
            }

            return RUser;
        }

        /// <summary>
        /// The vip
        /// </summary>
        internal bool VIP;
        internal bool vipAlertsOff = false;

        /// <summary>
        /// The last gift purchase time
        /// </summary>
        internal DateTime LastGiftPurchaseTime;

        /// <summary>
        /// The last gift open time
        /// </summary>
        internal DateTime LastGiftOpenTime;

        /// <summary>
        /// The trade lock expire
        /// </summary>
        internal int TradeLockExpire;

        /// <summary>
        /// The trade locked
        /// </summary>
        internal bool TradeLocked;

        /// <summary>
        /// The talent status
        /// </summary>
        internal string TalentStatus;

        /// <summary>
        /// The current talent level
        /// </summary>
        internal int CurrentTalentLevel;

        /// <summary>
        /// The relationships
        /// </summary>
        internal Dictionary<int, Relationship> Relationships;

        /// <summary>
        /// The answered polls
        /// </summary>
        internal HashSet<uint> AnsweredPolls;

        /// <summary>
        /// The nux passed
        /// </summary>
        internal bool NuxPassed, NewNavigator;

        /// <summary>
        /// The minimail unread messages
        /// </summary>
        internal uint MinimailUnreadMessages;

        /// <summary>
        /// The last SQL query
        /// </summary>
        internal int LastSqlQuery;

        /// <summary>
        /// The builders expire
        /// </summary>
        internal int BuildersExpire;

        /// <summary>
        /// The builders items maximum
        /// </summary>
        internal int BuildersItemsMax;

        /// <summary>
        /// The builders items used
        /// </summary>
        internal int BuildersItemsUsed;

        /// <summary>
        /// The on duty
        /// </summary>
        internal bool OnDuty;

        /// <summary>
        /// The release name
        /// </summary>
        internal string ReleaseName;

        /// <summary>
        /// The navigator logs
        /// </summary>
        internal Dictionary<int, NaviLogs> NavigatorLogs;

        /// <summary>
        /// The _clothing manager
        /// </summary>
        internal UserClothing _clothingManager;

        internal UserPreferences Preferences;

        internal YoutubeManager _youtubeManager;

        /// <summary>
        /// The _my groups
        /// </summary>
        private readonly List<UInt32> _myGroups;

        /// <summary>
        /// The _subscription manager
        /// </summary>
        private SubscriptionManager _subscriptionManager;

        /// <summary>
        /// The _messenger
        /// </summary>
        private HabboMessenger _messenger;

        /// <summary>
        /// The _badge component
        /// </summary>
        private BadgeComponent _badgeComponent;

        /// <summary>
        /// The _inventory component
        /// </summary>
        private InventoryComponent _inventoryComponent;

        /// <summary>
        /// The _avatar effects inventory component
        /// </summary>
        private AvatarEffectsInventoryComponent _avatarEffectsInventoryComponent;

        /// <summary>
        /// The _m client
        /// </summary>
        private GameClient _mClient;

        /// <summary>
        /// The _habboinfo saved
        /// </summary>
        private bool _habboinfoSaved;

        public bool RPDisconnected = false;

        /// <summary>
        /// The _loaded my groups
        /// </summary>
        private bool _loadedMyGroups;

        /// <summary>
        /// The own rooms serialized
        /// </summary>
        internal bool OwnRoomsSerialized = false;

        /// <summary>
        /// The timer_ elapsed
        /// </summary>
        public bool timer_Elapsed = false;

        public uint LastSelectedUser = 0;

        public DateTime LastUsed = DateTime.Now;

        public int RollRig;

        /// <summary>
        /// Handles the ElapsedEvent event of the timer control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        public void timer_ElapsedEvent(object source, ElapsedEventArgs e)
        {
            timer_Elapsed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Habbo"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="realName">Name of the real.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="motto">The motto.</param>
        /// <param name="look">The look.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="credits">The credits.</param>
        /// <param name="activityPoints">The activity points.</param>
        /// <param name="lastActivityPointsUpdate">The last activity points update.</param>
        /// <param name="muted">if set to <c>true</c> [muted].</param>
        /// <param name="homeRoom">The home room.</param>
        /// <param name="respect">The respect.</param>
        /// <param name="dailyRespectPoints">The daily respect points.</param>
        /// <param name="dailyPetRespectPoints">The daily pet respect points.</param>
        /// <param name="hasFriendRequestsDisabled">if set to <c>true</c> [has friend requests disabled].</param>
        /// <param name="currentQuestId">The current quest identifier.</param>
        /// <param name="currentQuestProgress">The current quest progress.</param>
        /// <param name="achievementPoints">The achievement points.</param>
        /// <param name="regTimestamp">The reg timestamp.</param>
        /// <param name="lastOnline">The last online.</param>
        /// <param name="appearOffline">if set to <c>true</c> [appear offline].</param>
        /// <param name="hideInRoom">if set to <c>true</c> [hide in room].</param>
        /// <param name="vip">if set to <c>true</c> [vip].</param>
        /// <param name="createDate">The create date.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="citizenShip">The citizen ship.</param>
        /// <param name="belCredits">The bel credits.</param>
        /// <param name="groups">The groups.</param>
        /// <param name="favId">The fav identifier.</param>
        /// <param name="lastChange">The last change.</param>
        /// <param name="tradeLocked">if set to <c>true</c> [trade locked].</param>
        /// <param name="tradeLockExpire">The trade lock expire.</param>
        /// <param name="nuxPassed">if set to <c>true</c> [nux passed].</param>
        /// <param name="buildersExpire">The builders expire.</param>
        /// <param name="buildersItemsMax">The builders items maximum.</param>
        /// <param name="buildersItemsUsed">The builders items used.</param>
        /// <param name="releaseVersion">The release version.</param>
        /// <param name="onDuty">if set to <c>true</c> [on duty].</param>
        /// <param name="naviLogs">The navi logs.</param>
        /// <param name="newNavigator"></param>
        internal Habbo(uint id, string userName, string realName, uint rank, string motto, string look, string gender,
            int credits, int activityPoints, double lastActivityPointsUpdate, bool muted, uint homeRoom, int respect,
            int dailyRespectPoints, int dailyPetRespectPoints, bool hasFriendRequestsDisabled, uint currentQuestId,
            int currentQuestProgress, int achievementPoints, int regTimestamp, int lastOnline, bool appearOffline,
            bool hideInRoom, bool vip, double createDate, bool online, string citizenShip, int belCredits,
            HashSet<GroupUser> groups, uint favId, int lastChange, bool tradeLocked, int tradeLockExpire, bool nuxPassed,
            int buildersExpire, int buildersItemsMax, int buildersItemsUsed, int releaseVersion, bool onDuty,
            Dictionary<int, NaviLogs> naviLogs, bool newNavigator, int dailyCompetitionVotes)
        {
            Id = id;
            UserName = userName;
            RealName = realName;
            _myGroups = new List<uint>();
            BuildersExpire = buildersExpire;
            BuildersItemsMax = buildersItemsMax;
            BuildersItemsUsed = buildersItemsUsed;
            if (rank < 1u)
                rank = 1u;
            ReleaseName = string.Empty;
            OnDuty = onDuty;
            Rank = rank;
            Motto = motto;
            Look = look.ToLower();
            VIP = rank > 5 || vip;
            LastChange = lastChange;
            TradeLocked = tradeLocked;
            NavigatorLogs = naviLogs;
            TradeLockExpire = tradeLockExpire;
            Gender = gender.ToLower();
            Credits = credits;
            ActivityPoints = activityPoints;
            BelCredits = belCredits;
            AchievementPoints = achievementPoints;
            Muted = muted;
            LoadingRoom = 0u;
            CreateDate = createDate;
            LoadingChecksPassed = false;
            FloodTime = 0;
            NuxPassed = nuxPassed;
            CurrentRoomId = 0u;
            TimeLoggedOn = DateTime.Now;
            TimeLoggedOn1 = Plus.GetUnixTimeStamp();
            HomeRoom = homeRoom;
            HideInRoom = hideInRoom;
            AppearOffline = appearOffline;
            FavoriteRooms = new List<uint>();
            MutedUsers = new List<uint>();
            Tags = new List<string>();
            Achievements = new Dictionary<string, UserAchievement>();
            Talents = new Dictionary<int, UserTalent>();
            Relationships = new Dictionary<int, Relationship>();
            RatedRooms = new HashSet<uint>();
            Respect = respect;
            DailyRespectPoints = dailyRespectPoints;
            DailyPetRespectPoints = dailyPetRespectPoints;
            IsTeleporting = false;
            TeleporterId = 0u;
            UsersRooms = new HashSet<RoomData>();
            HasFriendRequestsDisabled = hasFriendRequestsDisabled;
            LastOnline = Plus.GetUnixTimeStamp();
            PreviousOnline = lastOnline;
            RecentlyVisitedRooms = new LinkedList<uint>();
            CurrentQuestId = currentQuestId;
            IsHopping = false;

            FavouriteGroup = Plus.GetGame().GetGroupManager().GetGroup(favId) != null ? favId : 1u;
            UserGroups = groups;
            if (DailyPetRespectPoints > 99)
                DailyPetRespectPoints = 99;
            if (DailyRespectPoints > 99)
                DailyRespectPoints = 99;
            LastGiftPurchaseTime = DateTime.Now;
            LastGiftOpenTime = DateTime.Now;
            TalentStatus = citizenShip;
            CurrentTalentLevel = GetCurrentTalentLevel();
            NewNavigator = newNavigator;
            DailyCompetitionVotes = dailyCompetitionVotes;

            RollRig = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can change name.
        /// </summary>
        /// <value><c>true</c> if this instance can change name; otherwise, <c>false</c>.</value>
        public bool CanChangeName
        {
            get
            {
                return (ExtraSettings.CHANGE_NAME_STAFF && HasFuse("fuse_can_change_name")) ||
                       (ExtraSettings.CHANGE_NAME_VIP && VIP) ||
                       (ExtraSettings.CHANGE_NAME_EVERYONE &&
                        Plus.GetUnixTimeStamp() > (LastChange + 604800));
            }
        }

        /// <summary>
        /// Gets the head part.
        /// </summary>
        /// <value>The head part.</value>
        internal string HeadPart
        {
            get
            {
                var strtmp = Look.Split('.');
                var tmp2 = strtmp.FirstOrDefault(x => x.Contains("hd-"));
                var lookToReturn = tmp2 ?? "";

                if (Look.Contains("ha-"))
                    lookToReturn += string.Format(".{0}", strtmp.FirstOrDefault(x => x.Contains("ha-")));
                if (Look.Contains("ea-"))
                    lookToReturn += string.Format(".{0}", strtmp.FirstOrDefault(x => x.Contains("ea-")));
                if (Look.Contains("hr-"))
                    lookToReturn += string.Format(".{0}", strtmp.FirstOrDefault(x => x.Contains("hr-")));
                if (Look.Contains("he-"))
                    lookToReturn += string.Format(".{0}", strtmp.FirstOrDefault(x => x.Contains("he-")));
                if (Look.Contains("fa-"))
                    lookToReturn += string.Format(".{0}", strtmp.FirstOrDefault(x => x.Contains("fa-")));

                return lookToReturn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [in room].
        /// </summary>
        /// <value><c>true</c> if [in room]; otherwise, <c>false</c>.</value>
        internal bool InRoom
        {
            get { return CurrentRoomId >= 1 && CurrentRoom != null; }
        }

        /// <summary>
        /// Gets the current room.
        /// </summary>
        /// <value>The current room.</value>
        internal Room CurrentRoom
        {
            get
            {
                return CurrentRoomId <= 0u ? null : Plus.GetGame().GetRoomManager().GetRoom(CurrentRoomId);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is helper.
        /// </summary>
        /// <value><c>true</c> if this instance is helper; otherwise, <c>false</c>.</value>
        internal bool IsHelper
        {
            get { return TalentStatus == "helper" || Rank >= 4; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is citizen.
        /// </summary>
        /// <value><c>true</c> if this instance is citizen; otherwise, <c>false</c>.</value>
        internal bool IsCitizen
        {
            get { return CurrentTalentLevel > 4; }
        }

        /// <summary>
        /// Gets the get query string.
        /// </summary>
        /// <value>The get query string.</value>
        internal string GetQueryString
        {
            get
            {
                _habboinfoSaved = true;
                return string.Concat(new object[]
                {
                    "UPDATE users SET online='0', last_online = '",
                    Plus.GetUnixTimeStamp(),
                    "', activity_points = '",
                    ActivityPoints,
                    "', seasonal_currency = '",
                    BelCredits,
                    "', credits = '",
                    Credits,
                    "' WHERE id = '",
                    Id,
                    "'; UPDATE users_stats SET achievement_score = ",
                    AchievementPoints,
                    " WHERE id=",
                    Id,
                    " LIMIT 1; "
                });
            }
        }

        /// <summary>
        /// Gets my groups.
        /// </summary>
        /// <value>My groups.</value>
        internal List<uint> MyGroups
        {
            get
            {
                if (!_loadedMyGroups)
                    _LoadMyGroups();

                return _myGroups;
            }
        }

        /// <summary>
        /// Initializes the information.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void InitInformation(UserData data)
        {
            _subscriptionManager = new SubscriptionManager(Id, data);
            _badgeComponent = new BadgeComponent(Id, data);
            Quests = data.Quests;
            _messenger = new HabboMessenger(Id);
            _messenger.Init(data.Friends, data.Requests);
            SpectatorMode = false;
            Disconnected = false;
            UsersRooms = data.Rooms;
            Relationships = data.Relations;
            AnsweredPolls = data.SuggestedPolls;
        }

        /// <summary>
        /// Initializes the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="data">The data.</param>
        internal void Init(GameClient client, UserData data)
        {
            _mClient = client;
            _subscriptionManager = new SubscriptionManager(Id, data);
            _badgeComponent = new BadgeComponent(Id, data);
            _inventoryComponent = InventoryGlobal.GetInventory(Id, client, data);
            _inventoryComponent.SetActiveState(client);
            _avatarEffectsInventoryComponent = new AvatarEffectsInventoryComponent(Id, client, data);
            Quests = data.Quests;
            _messenger = new HabboMessenger(Id);
            _messenger.Init(data.Friends, data.Requests);
            FriendCount = Convert.ToUInt32(data.Friends.Count);
            SpectatorMode = false;
            Disconnected = false;
            UsersRooms = data.Rooms;
            MinimailUnreadMessages = data.MiniMailCount;
            Relationships = data.Relations;
            AnsweredPolls = data.SuggestedPolls;
            _clothingManager = new UserClothing(Id);
            Preferences = new UserPreferences(Id);
            _youtubeManager = new YoutubeManager(Id);
        }

        /// <summary>
        /// Updates the rooms.
        /// </summary>
        internal void UpdateRooms()
        {
            using (var dbClient = Plus.GetDatabaseManager().GetQueryReactor())
            {
                UsersRooms.Clear();
                dbClient.SetQuery("SELECT * FROM rooms_data WHERE owner = @name ORDER BY id ASC LIMIT 50");
                dbClient.AddParameter("name", UserName);
                var table = dbClient.GetTable();
                foreach (DataRow dataRow in table.Rows)
                    UsersRooms.Add(
                        Plus.GetGame()
                            .GetRoomManager()
                            .FetchRoomData(Convert.ToUInt32(dataRow["id"]), dataRow));
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void LoadData(UserData data)
        {
            LoadAchievements(data.Achievements);
            LoadTalents(data.Talents);
            LoadFavorites(data.FavouritedRooms);
            LoadMutedUsers(data.Ignores);
            LoadTags(data.Tags);
        }

        /// <summary>
        /// Serializes the quests.
        /// </summary>
        /// <param name="response">The response.</param>
        internal void SerializeQuests(ref QueuedServerMessage response)
        {
            Plus.GetGame().GetQuestManager().GetList(_mClient, null);
        }

        /// <summary>
        /// Gots the command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotCommand(string cmd)
        {
            return Plus.GetGame().GetRoleManager().RankGotCommand(Rank, cmd);
        }

        /// <summary>
        /// Determines whether the specified fuse has fuse.
        /// </summary>
        /// <param name="fuse">The fuse.</param>
        /// <returns><c>true</c> if the specified fuse has fuse; otherwise, <c>false</c>.</returns>
        internal bool HasFuse(string fuse)
        {
            return Plus.GetGame().GetRoleManager().RankHasRight(Rank, fuse) ||
                   (GetSubscriptionManager().HasSubscription &&
                    Plus.GetGame()
                        .GetRoleManager()
                        .HasVip(GetSubscriptionManager().GetSubscription().SubscriptionId, fuse));
        }

        /// <summary>
        /// Loads the favorites.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        internal void LoadFavorites(List<uint> roomId)
        {
            FavoriteRooms = roomId;
        }

        /// <summary>
        /// Loads the muted users.
        /// </summary>
        /// <param name="usersMuted">The users muted.</param>
        internal void LoadMutedUsers(List<uint> usersMuted)
        {
            MutedUsers = usersMuted;
        }

        /// <summary>
        /// Loads the tags.
        /// </summary>
        /// <param name="tags">The tags.</param>
        internal void LoadTags(List<string> tags)
        {
            Tags = tags;
        }

        /// <summary>
        /// Serializes the club.
        /// </summary>
        internal void SerializeClub()
        {
            var client = GetClient();
            var serverMessage = new ServerMessage();
            serverMessage.Init(LibraryParser.OutgoingRequest("SubscriptionStatusMessageComposer"));
            serverMessage.AppendString("club_habbo");

            if (client == null)
                return;

            if (client.GetHabbo() == null)
                return;

            if (client.GetHabbo().GetSubscriptionManager() == null)
                return;

            if (client.GetHabbo().GetSubscriptionManager().HasSubscription)
            {
                double num = client.GetHabbo().GetSubscriptionManager().GetSubscription().ExpireTime;
                var num2 = num - Plus.GetUnixTimeStamp();

                {
                    var num3 = (int)Math.Ceiling(num2 / 86400.0);
                    var i =
                        (int)
                            Math.Ceiling(
                                (

                                    Plus.GetUnixTimeStamp() -
                                    (double)client.GetHabbo().GetSubscriptionManager().GetSubscription().ActivateTime) /
                                86400.0);
                    var num4 = num3 / 31;
                    if (num4 >= 1)
                        num4--;
                    serverMessage.AppendInteger(num3 - num4 * 31);
                    serverMessage.AppendInteger(1);
                    serverMessage.AppendInteger(num4);
                    serverMessage.AppendInteger(1);
                    serverMessage.AppendBool(true);
                    serverMessage.AppendBool(true);
                    serverMessage.AppendInteger(i);
                    serverMessage.AppendInteger(i);
                    serverMessage.AppendInteger(10);
                }
            }
            else
            {
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(0);
                serverMessage.AppendBool(false);
                serverMessage.AppendBool(false);
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(0);
                serverMessage.AppendInteger(0);
            }
            client.SendMessage(serverMessage);
            var serverMessage2 = new ServerMessage(LibraryParser.OutgoingRequest("UserClubRightsMessageComposer"));
            serverMessage2.AppendInteger(GetSubscriptionManager().HasSubscription ? 2 : 0);
            serverMessage2.AppendInteger(Rank);
            serverMessage2.AppendBool((Rank == 4) ? false : Rank >= Convert.ToUInt32(Plus.GetDbConfig().DbData["ambassador.minrank"]));
            client.SendMessage(serverMessage2);
        }

        /// <summary>
        /// Loads the achievements.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        internal void LoadAchievements(Dictionary<string, UserAchievement> achievements)
        {
            Achievements = achievements;
        }

        /// <summary>
        /// Loads the talents.
        /// </summary>
        /// <param name="talents">The talents.</param>
        internal void LoadTalents(Dictionary<int, UserTalent> talents)
        {
            Talents = talents;
        }

        /// <summary>
        /// Called when [disconnect].
        /// </summary>
        /// <param name="reason">The reason.</param>
        internal void OnDisconnect(string reason)
        {
            if (Disconnected)
                return;

            Disconnected = true;

            if (_inventoryComponent != null)
            {
                if (_mClient.GetRoleplay().UsingPet && GetRoomUser().MyPet != null)
                {
                    using (var Adapt = Plus.GetDatabaseManager().GetQueryReactor())
                    {
                        Adapt.RunFastQuery("UPDATE bots SET room_id = '0' WHERE id = ' " + GetRoomUser().MyPet.PetData.PetId + "'");
                    }
                    _mClient.GetMessageHandler().PickUpPet(_mClient, GetRoomUser().MyPet.PetData.PetId, true);
                }
                _inventoryComponent.RunDbUpdate();
                _inventoryComponent.SetIdleState();
            }

            var navilogs = string.Empty;
            if (NavigatorLogs.Any())
            {
                navilogs = NavigatorLogs.Values.Aggregate(navilogs, (current, navi) => current + string.Format("{0},{1},{2};", navi.Id, navi.Value1, navi.Value2));
                navilogs = navilogs.Remove(navilogs.Length - 1);
            }

            Plus.GetGame().GetClientManager().UnregisterClient(Id, UserName);
            SessionManagement.IncreaseDisconnection();

            Out.WriteLine("[" + UserName + "] has logged out! Reason: " + reason, "", ConsoleColor.DarkYellow);

            TimeSpan GetOnlineSeconds = DateTime.Now - TimeLoggedOn;
            int SecondsToGive = GetOnlineSeconds.Seconds;
            int GiveOnlineTime = Plus.GetUnixTimeStamp() - TimeLoggedOn1;

            if (_mClient.GetRoleplay().IsBoxing && _mClient.GetRoleplay().BoxingRoom != null && _mClient.GetRoleplay().BoxingRoom.SoloQueue != null)
            {
                if (_mClient.GetRoleplay().BoxingRoom.SoloQueue.PlayerQueue.Contains(_mClient))
                {
                    _mClient.GetRoleplay().BoxingRoom.SoloQueue.PlayerQueue.Remove(_mClient);
                }

                _mClient.GetRoleplay().BoxingRoom.SoloQueue.RemoveBoxer(_mClient, true);
            }

            // If the user is in colour wars
            if (_mClient.GetRoleplay().inColourWars)
            {
                // Remove the user from team
                string team;
                team = _mClient.GetRoleplay().ColourWarTeam.Colour.ToLower().ToString();
                _mClient.GetRoleplay().SaveQuickStat("last_cw_team", team);
               
                ColourManager.RemovePlayerFromTeam(_mClient, _mClient.GetRoleplay().ColourWarTeam, true, "eat", true);
            }

            if (_mClient.GetRoleplay().InMafiaWars)
            {
                Plus.GetGame().MafiaWars.RemoveUserFromGame(_mClient, _mClient.GetRoleplay().TeamString, true, true);
            }

            if (_mClient.GetRoleplay().Bag != null)
            {
                HabboHotel.Roleplay.Misc.RoleplayManager.PickRock(_mClient.GetRoleplay().Bag, _mClient.GetRoleplay().Bag.RoomId);
            }

            if (!_habboinfoSaved)
            {
                _habboinfoSaved = true;
                using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                {
                    if (this != null)
                    {
                        if (_mClient != null)
                        {
                            if (_mClient.GetRoleplay() != null)
                            {
                                _mClient.GetRoleplay().SaveStats();
                                _mClient.GetRoleplay().Dispose(); // Dispose of roleplay
                            }
                        }
                    }

                    queryReactor.SetQuery("UPDATE users SET activity_points = " + ActivityPoints + ", credits = " + Credits + ", seasonal_currency = " + BelCredits + ", online='0', last_online = '" + Plus.GetUnixTimeStamp() + "', builders_items_used = " + BuildersItemsUsed + ", navigator = '" + Plus.BoolToEnum(NewNavigator) + "', navilogs = @navilogs  WHERE id = " + Id + " LIMIT 1;UPDATE users_stats SET achievement_score=" + AchievementPoints + " WHERE id=" + Id + " LIMIT 1;");
                    queryReactor.AddParameter("navilogs", navilogs);
                    queryReactor.RunQuery();

                    queryReactor.RunFastQuery("UPDATE users_stats SET online_seconds = online_seconds + " + GiveOnlineTime + " WHERE id = " + Id);

                    if (Rank >= 4u)
                        queryReactor.RunFastQuery(
                            string.Format(
                                "UPDATE moderation_tickets SET status='open', moderator_id=0 WHERE status='picked' AND moderator_id={0}",
                                Id));
                }
            }

            if (InRoom && CurrentRoom != null)
                CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(_mClient, false, false);
            if (_messenger != null)
            {
                _messenger.AppearOffline = true;
                _messenger.Destroy();
            }

            if (_avatarEffectsInventoryComponent != null)
                _avatarEffectsInventoryComponent.Dispose();
            _mClient = null;
        }

        /// <summary>
        /// Initializes the messenger.
        /// </summary>
        internal void InitMessenger()
        {
            var client = GetClient();
            if (client == null)
                return;
            client.SendMessage(_messenger.SerializeCategories());
            client.SendMessage(_messenger.SerializeFriends());

            client.SendMessage(_messenger.SerializeRequests());
            if (Plus.OfflineMessages.ContainsKey(Id))
            {
                var list = Plus.OfflineMessages[Id];
                foreach (var current in list)
                    client.SendMessage(_messenger.SerializeOfflineMessages(current));
                Plus.OfflineMessages.Remove(Id);
                OfflineMessage.RemoveAllMessages(Plus.GetDatabaseManager().GetQueryReactor(), Id);
            }
            if (_messenger.Requests.Count > Plus.FriendRequestLimit)
            {
                client.SendNotif(Plus.GetLanguage().GetVar("user_friend_request_max"));
            }

            _messenger.OnStatusChanged(false);
        }

        /// <summary>
        /// Updates the credits balance.
        /// </summary>
        internal void UpdateCreditsBalance()
        {
            if (_mClient == null || _mClient.GetMessageHandler() == null ||
                _mClient.GetMessageHandler().GetResponse() == null)
                return;
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("CreditsBalanceMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendString(string.Format("{0}.0", Credits));
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Updates the activity points balance.
        /// </summary>
        internal void UpdateActivityPointsBalance()
        {
            if (_mClient == null || _mClient.GetMessageHandler() == null ||
                _mClient.GetMessageHandler().GetResponse() == null)
                return;
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("ActivityPointsMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendInteger(3);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(0);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(ActivityPoints);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(5);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(BelCredits);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(105);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(BelCredits);
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Updates the seasonal currency balance.
        /// </summary>
        internal void UpdateSeasonalCurrencyBalance()
        {
            if (_mClient == null || _mClient.GetMessageHandler() == null ||
                _mClient.GetMessageHandler().GetResponse() == null)
                return;
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("ActivityPointsMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendInteger(3);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(0);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(ActivityPoints);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(5);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(BelCredits);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(105);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(BelCredits);
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Notifies the new pixels.
        /// </summary>
        /// <param name="change">The change.</param>
        internal void NotifyNewPixels(int change)
        {
            if (_mClient == null || _mClient.GetMessageHandler() == null ||
                _mClient.GetMessageHandler().GetResponse() == null)
                return;
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("ActivityPointsNotificationMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendInteger(ActivityPoints);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(change);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(0);
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Notifies the new diamonds.
        /// </summary>
        /// <param name="change">The change.</param>
        internal void NotifyNewDiamonds(int change)
        {
            if (_mClient == null || _mClient.GetMessageHandler() == null ||
                _mClient.GetMessageHandler().GetResponse() == null)
                return;

            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("ActivityPointsNotificationMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendInteger(BelCredits);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(change);
            _mClient.GetMessageHandler().GetResponse().AppendInteger(5);
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Notifies the voucher.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="productDescription">The product description.</param>
        internal void NotifyVoucher(bool isValid, string productName, string productDescription)
        {
            if (isValid)
            {
                _mClient.GetMessageHandler()
                    .GetResponse()
                    .Init(LibraryParser.OutgoingRequest("VoucherValidMessageComposer"));
                _mClient.GetMessageHandler().GetResponse().AppendString(productName);
                _mClient.GetMessageHandler().GetResponse().AppendString(productDescription);
                _mClient.GetMessageHandler().SendResponse();
                return;
            }
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("VoucherErrorMessageComposer"));
            _mClient.GetMessageHandler().GetResponse().AppendString("1");
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Mutes this instance.
        /// </summary>
        internal void Mute()
        {
            if (!Muted)
                Muted = true;
        }

        /// <summary>
        /// Uns the mute.
        /// </summary>
        internal void UnMute()
        {
            if (Muted)
                GetClient().SendNotif("You were unmuted.");

            Muted = false;
            if (CurrentRoom != null && CurrentRoom.MutedUsers.ContainsKey(Id))
                CurrentRoom.MutedUsers.Remove(Id);
        }

        /// <summary>
        /// Gets the subscription manager.
        /// </summary>
        /// <returns>SubscriptionManager.</returns>
        internal SubscriptionManager GetSubscriptionManager()
        {
            return _subscriptionManager;
        }
        internal YoutubeManager GetYoutubeManager()
        {
            return _youtubeManager;
        }

        /// <summary>
        /// Gets the messenger.
        /// </summary>
        /// <returns>HabboMessenger.</returns>
        internal HabboMessenger GetMessenger()
        {
            return _messenger;
        }

        /// <summary>
        /// Gets the badge component.
        /// </summary>
        /// <returns>BadgeComponent.</returns>
        internal BadgeComponent GetBadgeComponent()
        {
            return _badgeComponent;
        }

        /// <summary>
        /// Gets the inventory component.
        /// </summary>
        /// <returns>InventoryComponent.</returns>
        internal InventoryComponent GetInventoryComponent()
        {
            return _inventoryComponent;
        }

        /// <summary>
        /// Gets the avatar effects inventory component.
        /// </summary>
        /// <returns>AvatarEffectsInventoryComponent.</returns>
        internal AvatarEffectsInventoryComponent GetAvatarEffectsInventoryComponent()
        {
            return _avatarEffectsInventoryComponent;
        }

        /// <summary>
        /// Runs the database update.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        internal void RunDbUpdate(IQueryAdapter dbClient)
        {
            dbClient.RunFastQuery(string.Concat(new object[]
            {
                "UPDATE users SET last_online = '",
                Plus.GetUnixTimeStamp(),
                "', activity_points = '",
                ActivityPoints,
                "', credits = '",
                Credits,
                "', seasonal_currency = '",
                BelCredits,
                "' WHERE id = '",
                Id,
                "' LIMIT 1; "
            }));
        }

        /// <summary>
        /// Gets the quest progress.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>System.Int32.</returns>
        internal int GetQuestProgress(uint p)
        {
            int result;
            Quests.TryGetValue(p, out result);
            return result;
        }

        /// <summary>
        /// Gets the achievement data.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>UserAchievement.</returns>
        internal UserAchievement GetAchievementData(string p)
        {
            UserAchievement result;
            Achievements.TryGetValue(p, out result);
            return result;
        }

        /// <summary>
        /// Gets the talent data.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>UserTalent.</returns>
        internal UserTalent GetTalentData(int t)
        {
            UserTalent result;
            Talents.TryGetValue(t, out result);
            return result;
        }

        /// <summary>
        /// Gets the current talent level.
        /// </summary>
        /// <returns>System.Int32.</returns>
        internal int GetCurrentTalentLevel()
        {
            return Talents.Values.Select(current => Plus.GetGame().GetTalentManager().GetTalent(current.TalentId).Level).Concat(new[] { 1 }).Max();
        }

        /// <summary>
        /// _s the load my groups.
        /// </summary>
        internal void _LoadMyGroups()
        {
            DataTable dTable;
            using (var dbClient = Plus.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery(string.Format("SELECT id FROM groups_data WHERE owner_id = {0}", Id));
                dTable = dbClient.GetTable();
            }

            foreach (DataRow dRow in dTable.Rows)
                _myGroups.Add(Convert.ToUInt32(dRow["id"]));

            _loadedMyGroups = true;
        }

        /// <summary>
        /// Gots the poll data.
        /// </summary>
        /// <param name="pollId">The poll identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotPollData(uint pollId)
        {
            return (AnsweredPolls.Contains(pollId));
        }

        /// <summary>
        /// Checks the trading.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool CheckTrading()
        {
            if (!TradeLocked)
                return true;
            if (TradeLockExpire - Plus.GetUnixTimeStamp() > 0)
                return false;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery(string.Format("UPDATE users SET trade_lock = '0' WHERE id = {0}", Id));
            TradeLocked = false;
            return true;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns>GameClient.</returns>
        private GameClient GetClient()
        {
            return Plus.GetGame().GetClientManager().GetClientByUserId(Id);
        }
    }
}