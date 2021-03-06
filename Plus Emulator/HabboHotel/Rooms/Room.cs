using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Plus.Configuration;
using Plus.HabboHotel.Catalogs;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.RoomBots;
using Plus.HabboHotel.Rooms.Games;
using Plus.HabboHotel.Rooms.RoomInvokedItems;
using Plus.HabboHotel.Rooms.Wired;
using Plus.HabboHotel.SoundMachine;
using Plus.Messages;
using Plus.Messages.Parsers;
using Plus.Util;
using Plus.HabboHotel.Roleplay.Minigames;
//using Plus.HabboHotel.Roleplay.Jobs.Farming;

namespace Plus.HabboHotel.Rooms
{
    /// <summary>
    /// Class Room.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// The team banzai
        /// </summary>
        internal TeamManager TeamBanzai;

        /// <summary>
        /// The team freeze
        /// </summary>
        internal TeamManager TeamFreeze;

        internal SoloQueueRoom SoloQueue;

        /// <summary>
        /// The users with rights
        /// </summary>
        internal List<uint> UsersWithRights;

        /// <summary>
        /// The everyone got rights
        /// </summary>
        internal bool EveryoneGotRights, RoomMuted;

        /// <summary>
        /// The bans
        /// </summary>
        internal Dictionary<long, double> Bans;

        /// <summary>
        /// The loaded groups
        /// </summary>
        internal Dictionary<uint, string> LoadedGroups;

        /// <summary>
        /// The muted users
        /// </summary>
        internal Dictionary<uint, uint> MutedUsers;

        /// <summary>
        /// The muted bots
        /// </summary>
        internal bool MutedBots, DiscoMode, MutedPets;

        /// <summary>
        /// The moodlight data
        /// </summary>
        internal MoodlightData MoodlightData;

        /// <summary>
        /// The toner data
        /// </summary>
        internal TonerData TonerData;

        /// <summary>
        /// The active trades
        /// </summary>
        internal ArrayList ActiveTrades;

        /// <summary>
        /// The word filter
        /// </summary>
        internal List<string> WordFilter;

        /// <summary>
        /// The _m cycle ended
        /// </summary>
        private bool _mCycleEnded;

        /// <summary>
        /// The _idle time
        /// </summary>
        private int _idleTime;

        /// <summary>
        /// The _game
        /// </summary>
        private GameManager _game;

        /// <summary>
        /// The _game map
        /// </summary>
        private Gamemap _gameMap;

        /// <summary>
        /// The _room item handling
        /// </summary>
        private RoomItemHandling _roomItemHandling;

        /// <summary>
        /// The _room user manager
        /// </summary>
        private RoomUserManager _roomUserManager;

        /// <summary>
        /// The _soccer
        /// </summary>
        private Soccer _soccer;

        /// <summary>
        /// The _banzai
        /// </summary>
        private BattleBanzai _banzai;

        /// <summary>
        /// The _freeze
        /// </summary>
        private Freeze _freeze;

        /// <summary>
        /// The _game item handler
        /// </summary>
        private GameItemHandler _gameItemHandler;

        /// <summary>
        /// The _music controller
        /// </summary>
        private RoomMusicController _musicController;

        /// <summary>
        /// The _wired handler
        /// </summary>
        private WiredHandler _wiredHandler;

        /// <summary>
        /// The _is crashed
        /// </summary>
        private bool _isCrashed;

        /// <summary>
        /// The _m disposed
        /// </summary>
        public bool Disposed;

        /// <summary>
        /// The _room kick
        /// </summary>
        private Queue _roomKick;

        /// <summary>
        /// The _room thread
        /// </summary>
        private Thread _roomThread;

        /// <summary>
        /// The _process timer
        /// </summary>
        private Timer _processTimer;

        /// <summary>
        /// The LastTimerReset
        /// </summary>
        internal DateTime LastTimerReset;

        /// <summary>
        /// The farm instance of the room
        /// </summary>
        // internal Farm Farm;

        /// <summary>
        /// The just loaded
        /// </summary>
        public bool JustLoaded = true;

        internal void Start(RoomData data)
        {
            InitializeFromRoomData(data);
            GetRoomItemHandler().LoadFurniture();
            GetGameMap().GenerateMaps(true);
        }

        /// <summary>
        /// Gets the user count.
        /// </summary>
        /// <value>The user count.</value>
        internal int UserCount
        {
            get { return _roomUserManager != null ? _roomUserManager.GetRoomUserCount() : 0; }
        }

        /// <summary>
        /// Gets the tag count.
        /// </summary>
        /// <value>The tag count.</value>
        internal int TagCount
        {
            get { return RoomData.Tags.Count; }
        }

        /// <summary>
        /// Gets the room identifier.
        /// </summary>
        /// <value>The room identifier.</value>
        internal uint RoomId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance can trade in room.
        /// </summary>
        /// <value><c>true</c> if this instance can trade in room; otherwise, <c>false</c>.</value>
        internal bool CanTradeInRoom
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the room data.
        /// </summary>
        /// <value>The room data.</value>
        internal RoomData RoomData { get; private set; }

        /// <summary>
        /// Gets the wired handler.
        /// </summary>
        /// <returns>WiredHandler.</returns>
        public WiredHandler GetWiredHandler()
        {
            return _wiredHandler ?? (_wiredHandler = new WiredHandler(this));
        }

        /// <summary>
        /// Gets the game map.
        /// </summary>
        /// <returns>Gamemap.</returns>
        internal Gamemap GetGameMap()
        {
            return _gameMap;
        }

        /// <summary>
        /// Gets the room item handler.
        /// </summary>
        /// <returns>RoomItemHandling.</returns>
        internal RoomItemHandling GetRoomItemHandler()
        {
            return _roomItemHandling;
        }

        /// <summary>
        /// Gets the room user manager.
        /// </summary>
        /// <returns>RoomUserManager.</returns>
        internal RoomUserManager GetRoomUserManager()
        {
            return _roomUserManager;
        }

        /// <summary>
        /// Gets the soccer.
        /// </summary>
        /// <returns>Soccer.</returns>
        internal Soccer GetSoccer()
        {
            return _soccer ?? (_soccer = new Soccer(this));
        }

        /// <summary>
        /// Gets the team manager for banzai.
        /// </summary>
        /// <returns>TeamManager.</returns>
        internal TeamManager GetTeamManagerForBanzai()
        {
            return TeamBanzai ?? (TeamBanzai = TeamManager.CreateTeamforGame("banzai"));
        }

        /// <summary>
        /// Gets the team manager for freeze.
        /// </summary>
        /// <returns>TeamManager.</returns>
        internal TeamManager GetTeamManagerForFreeze()
        {
            return TeamFreeze ?? (TeamFreeze = TeamManager.CreateTeamforGame("freeze"));
        }

        /// <summary>
        /// Gets the banzai.
        /// </summary>
        /// <returns>BattleBanzai.</returns>
        internal BattleBanzai GetBanzai()
        {
            return _banzai ?? (_banzai = new BattleBanzai(this));
        }

        /// <summary>
        /// Gets the freeze.
        /// </summary>
        /// <returns>Freeze.</returns>
        internal Freeze GetFreeze()
        {
            return _freeze ?? (_freeze = new Freeze(this));
        }

        /// <summary>
        /// Gets the game manager.
        /// </summary>
        /// <returns>GameManager.</returns>
        internal GameManager GetGameManager()
        {
            return _game ?? (_game = new GameManager(this));
        }

        /// <summary>
        /// Gets the game item handler.
        /// </summary>
        /// <returns>GameItemHandler.</returns>
        internal GameItemHandler GetGameItemHandler()
        {
            return _gameItemHandler ?? (_gameItemHandler = new GameItemHandler(this));
        }

        /// <summary>
        /// Gets the room music controller.
        /// </summary>
        /// <returns>RoomMusicController.</returns>
        internal RoomMusicController GetRoomMusicController()
        {
            return _musicController ?? (_musicController = new RoomMusicController());
        }

        /// <summary>
        /// Gots the music controller.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotMusicController()
        {
            return _musicController != null;
        }

        /// <summary>
        /// Gots the soccer.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotSoccer()
        {
            return _soccer != null;
        }

        /// <summary>
        /// Gots the banzai.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotBanzai()
        {
            return _banzai != null;
        }

        /// <summary>
        /// Gots the freeze.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool GotFreeze()
        {
            return _freeze != null;
        }

        /// <summary>
        /// Starts the room processing.
        /// </summary>
        internal void StartRoomProcessing()
        {
            _processTimer = new Timer(ProcessRoom, null, 0, 440);
        }

        /// <summary>
        /// Initializes the user bots.
        /// </summary>
        internal void InitUserBots()
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format("SELECT * FROM bots WHERE room_id = {0} AND ai_type != 'pet' AND ai_type != 'fightpet'",
                    RoomId));
                var table = queryReactor.GetTable();
                if (table == null)
                    return;
                foreach (var roomBot in from DataRow dataRow in table.Rows select BotManager.GenerateBotFromRow(dataRow)
                    )
                    _roomUserManager.DeployBot(roomBot, null);
            }
        }

        /// <summary>
        /// Clears the tags.
        /// </summary>
        internal void ClearTags()
        {
            RoomData.Tags.Clear();
        }

        /// <summary>
        /// Adds the tag range.
        /// </summary>
        /// <param name="tags">The tags.</param>
        internal void AddTagRange(List<string> tags)
        {
            RoomData.Tags.AddRange(tags);
        }

        /// <summary>
        /// Initializes the bots.
        /// </summary>
        internal void InitBots()
        {
            var botsForRoom = Plus.GetGame().GetBotManager().GetBotsForRoom(RoomId);
            foreach (var current in botsForRoom.Where(current => !current.IsPet))
                DeployBot(current);
        }

        /// <summary>
        /// Initializes the pets.
        /// </summary>
        internal void InitPets()
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format("SELECT * FROM bots WHERE room_id = {0} AND ai_type='pet'", RoomId));
                var table = queryReactor.GetTable();
                if (table == null)
                    return;
                foreach (DataRow dataRow in table.Rows)
                {
                    queryReactor.SetQuery(string.Format("SELECT * FROM pets_data WHERE id={0} LIMIT 1", dataRow[0]));
                    var row = queryReactor.GetRow();
                    if (row == null)
                        continue;
                    var pet = Catalog.GeneratePetFromRow(dataRow, row);

                    string AiType = Convert.ToString(dataRow["ai_type"]);
                    AIType Type = AIType.Pet;

                    switch (AiType.ToLower())
                    {
                        case "fightpet":
                        case "fight_pet":
                            Type = AIType.FightPet;
                            break;

                        case "pet":
                            Type = AIType.Pet;
                            break;
                    }

                    var bot = new RoomBot(pet.PetId, Convert.ToUInt32(RoomData.OwnerId), Type, false);
                    bot.Update(RoomId, "freeroam", pet.Name, "",
                        pet.Look, pet.X, pet.Y, ((int)pet.Z), 4, 0, 0, 0, 0, null, null, "", 0, 0, false, false);

                    _roomUserManager.DeployBot(bot, pet);
                }
            }
        }

        /// <summary>
        /// Deploys the bot.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <returns>RoomUser.</returns>
        internal RoomUser DeployBot(RoomBot bot)
        {
            return _roomUserManager.DeployBot(bot, null);
        }

        /// <summary>
        /// Queues the room kick.
        /// </summary>
        /// <param name="kick">The kick.</param>
        internal void QueueRoomKick(RoomKick kick)
        {
            lock (_roomKick.SyncRoot)
            {
                _roomKick.Enqueue(kick);
            }
        }

        /// <summary>
        /// Called when [room kick].
        /// </summary>
        internal void OnRoomKick()
        {
            var list = _roomUserManager.UserList.Values.Where(
                current => !current.IsBot && current.GetClient().GetHabbo().Rank < 4u).ToList();

            {
                foreach (var t in list)
                {
                    GetRoomUserManager().RemoveUserFromRoom(t.GetClient(), true, false);
                    t.GetClient().CurrentRoomUserId = -1;
                }
            }
        }

        /// <summary>
        /// Called when [user enter].
        /// </summary>
        /// <param name="user">The user.</param>
        internal void OnUserEnter(RoomUser user)
        {
            GetWiredHandler().ExecuteWired(Interaction.TriggerRoomEnter, user);

            var count = 0;

            foreach (var current in _roomUserManager.UserList.Values)
            {
                if (current.IsBot || current.IsPet)
                {
                    current.BotAI.OnUserEnterRoom(user);
                    count++;
                }

                if (count >= 3)
                    break;
            }
        }

        /// <summary>
        /// Called when [user say].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="message">The message.</param>
        /// <param name="shout">if set to <c>true</c> [shout].</param>
        internal void OnUserSay(RoomUser user, string message, bool shout)
        {
            foreach (var current in _roomUserManager.UserList.Values)
                try
                {
                    if (!current.IsBot && !current.IsPet)
                        continue;
                    if (!current.IsPet/* && message.ToLower().StartsWith(current.BotData.Name.ToLower())*/)
                    {
                        //message = message.Substring(1);
                        if (shout)
                            current.BotAI.OnUserShout(user, message);
                        else
                            current.BotAI.OnUserSay(user, message);
                    }
                    else if (current.IsPet/* && message.StartsWith(current.PetData.Name)*/ && current.PetData.Type != 16)
                    {
                        //message = message.Substring(current.PetData.Name.Length);
                        current.BotAI.OnUserSay(user, message);
                    }
                }
                catch (Exception)
                {
                    return;
                }
        }

        /// <summary>
        /// Loads the music.
        /// </summary>
        internal void LoadMusic()
        {
            DataTable table;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(
                    string.Format(
                        "SELECT items_songs.songid,items_rooms.id,items_rooms.base_item FROM items_songs LEFT JOIN items_rooms ON items_rooms.id = items_songs.itemid WHERE items_songs.roomid = {0}",
                        RoomId));
                table = queryReactor.GetTable();
            }

            if (table == null)
                return;
            foreach (DataRow dataRow in table.Rows)
            {
                var songId = (uint)dataRow[0];
                var num = Convert.ToUInt32(dataRow[1]);
                var baseItem = Convert.ToInt32(dataRow[2]);
                var songCode = string.Empty;
                var extraData = string.Empty;
                using (var queryreactor2 = Plus.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor2.SetQuery(string.Format("SELECT extra_data,songcode FROM items_rooms WHERE id = {0}",
                        num));
                    var row = queryreactor2.GetRow();
                    if (row != null)
                    {
                        extraData = (string)row["extra_data"];
                        songCode = (string)row["songcode"];
                    }
                }
                var diskItem = new SongItem(num, songId, baseItem, extraData, songCode);
                GetRoomMusicController().AddDisk(diskItem);
            }
        }

        /// <summary>
        /// Loads the rights.
        /// </summary>
        internal void LoadRights()
        {
            UsersWithRights = new List<uint>();
            DataTable dataTable;
            if (RoomData.Group != null)
                return;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format(
                    "SELECT rooms_rights.user_id FROM rooms_rights WHERE room_id = {0}", RoomId));
                dataTable = queryReactor.GetTable();
            }
            if (dataTable == null)
                return;
            foreach (DataRow dataRow in dataTable.Rows)
                UsersWithRights.Add(Convert.ToUInt32(dataRow["user_id"]));
        }

        /// <summary>
        /// Loads the bans.
        /// </summary>
        internal void LoadBans()
        {
            Bans = new Dictionary<long, double>();
            DataTable table;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format("SELECT user_id, expire FROM rooms_bans WHERE room_id = {0}", RoomId));
                table = queryReactor.GetTable();
            }
            if (table == null)
                return;
            foreach (DataRow dataRow in table.Rows)
                Bans.Add((uint)dataRow[0], Convert.ToDouble(dataRow[1]));
        }

        /// <summary>
        /// Gets the rights level.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>System.Int32.</returns>
        internal int GetRightsLevel(GameClient session)
        {
            try
            {
                if (session == null || session.GetHabbo() == null)
                    return 0;
                if (session.GetHabbo().UserName == RoomData.Owner || session.GetHabbo().HasFuse("fuse_any_room_controller") || (session.GetHabbo().HasFuse("fuse_builder") && !session.GetHabbo().HasFuse("fuse_mod")))
                    return 4;
                if (session.GetHabbo().HasFuse("fuse_any_rooms_rights") || (session.GetHabbo().HasFuse("fuse_builder") && !session.GetHabbo().HasFuse("fuse_mod")))
                    return 3;
                if (EveryoneGotRights || UsersWithRights.Contains(session.GetHabbo().Id))
                    return 1;
            }
            catch (Exception pException)
            {
                Logging.HandleException(pException, "GetRightsLevel");
            }
            return 0;
        }

        /// <summary>
        /// Checks the rights.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool CheckRights(GameClient session)
        {
            return CheckRights(session, false, false);
        }

        /// <summary>
        /// Checks the rights.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requireOwnerShip">if set to <c>true</c> [require ownership].</param>
        /// <param name="checkForGroups">if set to <c>true</c> [check for groups].</param>
        /// <param name="groupMembers"></param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool CheckRights(GameClient session, bool requireOwnerShip = false, bool checkForGroups = false,
                                  bool groupMembers = false)
        {
            try
            {
                if (session == null || session.GetHabbo() == null) return false;
                if (session.GetHabbo().UserName == RoomData.Owner && RoomData.Type == "private") return true;
                if (session.GetHabbo().HasFuse("fuse_owner") || session.GetHabbo().HasFuse("fuse_any_room_controller") || (session.GetHabbo().HasFuse("fuse_builder") && !session.GetHabbo().HasFuse("fuse_mod"))) return true;

                if (RoomData.Type != "private") return false;

                if (!requireOwnerShip)
                {
                    if (session.GetHabbo().HasFuse("fuse_any_rooms_rights") || (session.GetHabbo().HasFuse("fuse_builder") && !session.GetHabbo().HasFuse("fuse_mod"))) return true;
                    if (EveryoneGotRights || (UsersWithRights != null && UsersWithRights.Contains(session.GetHabbo().Id))) return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Room.CheckRights");
            }
            return false;
        }

        /// <summary>
        /// Processes the room.
        /// </summary>
        /// <param name="callItem">The call item.</param>
        internal void ProcessRoom(object callItem)
        {
            ProcessRoom();
        }

        /// <summary>
        /// Processes the room.
        /// </summary>
        internal void ProcessRoom()
        {
            try
            {
                if (_isCrashed || Disposed || Plus.ShutdownStarted)
                    return;
                try
                {
                    var idle = 0;
                    GetRoomItemHandler().OnCycle();
                    GetRoomUserManager().OnCycle(ref idle);

                    if (idle > 0)
                        _idleTime++;
                    else
                        _idleTime = 0;

                    if (!_mCycleEnded)
                    {
                        if ((_idleTime >= 60 && !JustLoaded) || (_idleTime >= 100 && JustLoaded))
                        {
                            Plus.GetGame().GetRoomManager().UnloadRoom(this, "No users");
                            return;
                        }
                        var serverMessage = GetRoomUserManager().SerializeStatusUpdates(false);

                        if (serverMessage != null)
                            SendMessage(serverMessage);
                    }

                    if (_gameItemHandler != null)
                        _gameItemHandler.OnCycle();
                    if (_game != null)
                        _game.OnCycle();
                    if (GotBanzai())
                        _banzai.OnCycle();
                    if (GotSoccer())
                        _soccer.OnCycle();
                    if (GetRoomMusicController() != null)
                        GetRoomMusicController().Update(this);

                    GetWiredHandler().OnCycle();
                    WorkRoomKickQueue();
                }
                catch (Exception e)
                {
                    Writer.Writer.LogException(e.ToString());
                    OnRoomCrash(e);
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(string.Format("Sub crash in room cycle: {0}", e));
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendMessage(byte[] message)
        {
            try
            {
                if (GetRoomUserManager() == null) return;
                var roomUsers = GetRoomUserManager().GetRoomUsers();

                if (roomUsers == null) return;
                foreach (var user in roomUsers)
                    user.SendMessage(message);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Broadcasts the chat message.
        /// </summary>
        /// <param name="chatMsg">The chat MSG.</param>
        /// <param name="roomUser">The room user.</param>
        /// <param name="p">The p.</param>
        internal void BroadcastChatMessage(ServerMessage chatMsg, RoomUser roomUser, uint p)
        {
            try
            {
                var roomUsers = GetRoomUserManager().GetRoomUsers();
                var msg = chatMsg.GetReversedBytes();
                foreach (
                    var user in
                        roomUsers.Where(user => user.OnCampingTent || !roomUser.OnCampingTent)
                            .Where(user => !user.GetClient().GetHabbo().MutedUsers.Contains(p)))
                    user.SendMessage(msg);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendMessage(ServerMessage message)
        {
            if (message != null)
                SendMessage(message.GetReversedBytes());
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="messages">The messages.</param>
        internal void SendMessage(List<ServerMessage> messages)
        {
            if (!messages.Any())
                return;

            try
            {
                var array = new byte[0];
                var num = 0;
                foreach (var current in messages)
                {
                    var bytes = current.GetReversedBytes();
                    var newSize = array.Length + bytes.Length;
                    Array.Resize(ref array, newSize);
                    foreach (byte t in bytes)
                    {
                        array[num] = t;
                        num++;
                    }
                }

                SendMessage(array);
            }
            catch (Exception pException)
            {
                Logging.HandleException(pException, "Room.SendMessage List<ServerMessage>");
            }
        }

        /// <summary>
        /// Sends the message to users with rights.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void SendMessageToUsersWithRights(ServerMessage message)
        {
            try
            {
                var bytes = message.GetReversedBytes();
                foreach (
                    var client in
                        _roomUserManager.UserList.Values.Where(current => !current.IsBot)
                            .Select(current => current.GetClient())
                            .Where(client => client != null && CheckRights(client)))
                    try
                    {
                        client.GetConnection().SendData(bytes);
                    }
                    catch (Exception pException)
                    {
                        Logging.HandleException(pException, "Room.SendMessageToUsersWithRights");
                    }
            }
            catch (Exception pException2)
            {
                Logging.HandleException(pException2, "Room.SendMessageToUsersWithRights");
            }
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        internal void Destroy()
        {
            SendMessage(new ServerMessage(LibraryParser.OutgoingRequest("OutOfRoomMessageComposer")));
            Dispose();
        }

        /// <summary>
        /// Users the is banned.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool UserIsBanned(uint pId)
        {
            return Bans.ContainsKey(pId);
        }

        /// <summary>
        /// Removes the ban.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        internal void RemoveBan(uint pId)
        {
            Bans.Remove(pId);
        }

        /// <summary>
        /// Adds the ban.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="time">The time.</param>
        internal void AddBan(int userId, long time)
        {
            if (!Bans.ContainsKey(Convert.ToInt32(userId)))
                Bans.Add(userId, ((Plus.GetUnixTimeStamp()) + time));

            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery("REPLACE INTO rooms_bans VALUES (" + userId + ", " + RoomId + ", '" + (Plus.GetUnixTimeStamp() + time) + "')");
        }

        /// <summary>
        /// Banneds the users.
        /// </summary>
        /// <returns>List&lt;System.UInt32&gt;.</returns>
        internal List<uint> BannedUsers()
        {
            var list = new List<uint>();
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(
                    string.Format("SELECT user_id FROM rooms_bans WHERE expire > UNIX_TIMESTAMP() AND room_id={0}",
                        RoomId));
                var table = queryReactor.GetTable();
                list.AddRange(from DataRow dataRow in table.Rows select (uint)dataRow[0]);
            }
            return list;
        }

        /// <summary>
        /// Determines whether [has ban expired] [the specified p identifier].
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns><c>true</c> if [has ban expired] [the specified p identifier]; otherwise, <c>false</c>.</returns>
        internal bool HasBanExpired(uint pId)
        {
            return !UserIsBanned(pId) || Bans[pId] < Plus.GetUnixTimeStamp();
        }

        /// <summary>
        /// Unbans the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        internal void Unban(uint userId)
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery("DELETE FROM rooms_bans WHERE user_id=" + userId + " AND room_id=" + RoomId + " LIMIT 1");
            Bans.Remove(userId);
        }

        /// <summary>
        /// Determines whether [has active trade] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if [has active trade] [the specified user]; otherwise, <c>false</c>.</returns>
        internal bool HasActiveTrade(RoomUser user)
        {
            return !user.IsBot && HasActiveTrade(user.GetClient().GetHabbo().Id);
        }

        /// <summary>
        /// Determines whether [has active trade] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if [has active trade] [the specified user identifier]; otherwise, <c>false</c>.</returns>
        internal bool HasActiveTrade(uint userId)
        {
            var array = ActiveTrades.ToArray();
            return array.Cast<Trade>().Any(trade => trade.ContainsUser(userId));
        }

        /// <summary>
        /// Gets the user trade.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Trade.</returns>
        internal Trade GetUserTrade(uint userId)
        {
            var array = ActiveTrades.ToArray();
            return array.Cast<Trade>().FirstOrDefault(trade => trade.ContainsUser(userId));
        }

        /// <summary>
        /// Tries the start trade.
        /// </summary>
        /// <param name="userOne">The user one.</param>
        /// <param name="userTwo">The user two.</param>
        internal void TryStartTrade(RoomUser userOne, RoomUser userTwo)
        {
            if (userOne == null || userTwo == null || userOne.IsBot || userTwo.IsBot || userOne.IsTrading ||
                userTwo.IsTrading || HasActiveTrade(userOne) || HasActiveTrade(userTwo))
                return;
            ActiveTrades.Add(new Trade(userOne.GetClient().GetHabbo().Id, userTwo.GetClient().GetHabbo().Id, RoomId));
        }

        /// <summary>
        /// Tries the stop trade.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        internal void TryStopTrade(uint userId)
        {
            var userTrade = GetUserTrade(userId);
            if (userTrade == null)
                return;
            userTrade.CloseTrade(userId);
            ActiveTrades.Remove(userTrade);
        }

        /// <summary>
        /// Sets the maximum users.
        /// </summary>
        /// <param name="maxUsers">The maximum users.</param>
        internal void SetMaxUsers(uint maxUsers)
        {
            RoomData.UsersMax = maxUsers;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery("UPDATE rooms_data SET users_max = " + maxUsers + " WHERE id = " + RoomId);
        }

        /// <summary>
        /// Flushes the settings.
        /// </summary>
        internal void FlushSettings()
        {
            _mCycleEnded = true;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                GetRoomItemHandler().SaveFurniture(queryReactor, null);
            RoomData.Tags.Clear();
            UsersWithRights.Clear();
            Bans.Clear();
            ActiveTrades.Clear();
            LoadedGroups.Clear();
            if (GotFreeze())
                _freeze = new Freeze(this);
            if (GotBanzai())
                _banzai = new BattleBanzai(this);
            if (GotSoccer())
                _soccer = new Soccer(this);
            if (_gameItemHandler != null)
                _gameItemHandler = new GameItemHandler(this);
        }

        /// <summary>
        /// Reloads the settings.
        /// </summary>
        internal void ReloadSettings()
        {
            var data = Plus.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            InitializeFromRoomData(data);
        }

        /// <summary>
        /// Updates the furniture.
        /// </summary>
        internal void UpdateFurniture()
        {
            var list = new List<ServerMessage>();
            var array = GetRoomItemHandler().FloorItems.Values.ToArray();
            var array2 = array;
            foreach (var roomItem in array2)
            {
                var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("UpdateRoomItemMessageComposer"));
                roomItem.Serialize(serverMessage);
                list.Add(serverMessage);
            }
            Array.Clear(array, 0, array.Length);
            var array3 = GetRoomItemHandler().WallItems.Values.ToArray();
            var array4 = array3;
            foreach (var roomItem2 in array4)
            {
                var serverMessage2 =
                    new ServerMessage(LibraryParser.OutgoingRequest("UpdateRoomWallItemMessageComposer"));
                roomItem2.Serialize(serverMessage2);
                list.Add(serverMessage2);
            }
            Array.Clear(array3, 0, array3.Length);
            SendMessage(list);
        }

        /// <summary>
        /// Checks the mute.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool CheckMute(GameClient session)
        {
            if (RoomMuted || session.GetHabbo().Muted)
                return true;
            if (!MutedUsers.ContainsKey(session.GetHabbo().Id))
                return false;
            if (MutedUsers[session.GetHabbo().Id] >= Plus.GetUnixTimeStamp())
                return true;
            MutedUsers.Remove(session.GetHabbo().Id);
            return false;
        }

        /// <summary>
        /// Adds the chatlog.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="globalMessage"></param>
        internal void AddChatlog(uint id, string message, bool globalMessage)
        {
            lock (RoomData.RoomChat)
            {
                RoomData.RoomChat.Add(new Chatlog(id, message, DateTime.Now, globalMessage));
            }
        }

        /// <summary>
        /// Resets the game map.
        /// </summary>
        /// <param name="newModelName">New name of the model.</param>
        /// <param name="wallHeight">Height of the wall.</param>
        /// <param name="wallThick">The wall thick.</param>
        /// <param name="floorThick">The floor thick.</param>
        internal void ResetGameMap(string newModelName, int wallHeight, int wallThick, int floorThick)
        {
            RoomData.ModelName = newModelName;
            RoomData.ModelName = newModelName;
            RoomData.ResetModel();
            RoomData.WallHeight = wallHeight;
            RoomData.WallThickness = wallThick;
            RoomData.FloorThickness = floorThick;
            _gameMap = new Gamemap(this);
        }

        /// <summary>
        /// Initializes from room data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void InitializeFromRoomData(RoomData data)
        {
            Initialize(data.Id, data, data.AllowRightsOverride, data.WordFilter);
        }

        /// <summary>
        /// Initializes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="roomData">The room data.</param>
        /// <param name="rightOverride">if set to <c>true</c> [right override].</param>
        /// <param name="wordFilter">The word filter.</param>
        private void Initialize(uint id, RoomData roomData, bool rightOverride, List<string> wordFilter)
        {
            RoomData = roomData;

            Disposed = false;
            RoomId = id;
            Bans = new Dictionary<long, double>();
            MutedUsers = new Dictionary<uint, uint>();
            ActiveTrades = new ArrayList();
            MutedBots = false;
            MutedPets = false;
            _mCycleEnded = false;
            EveryoneGotRights = rightOverride;
            LoadedGroups = new Dictionary<uint, string>();
            _roomKick = new Queue();
            _idleTime = 0;
            RoomMuted = false;
            _gameMap = new Gamemap(this);
            _roomItemHandling = new RoomItemHandling(this);
            _roomUserManager = new RoomUserManager(this);
            WordFilter = wordFilter;

            LoadRights();
            LoadMusic();
            LoadBans();
            InitUserBots();

            _roomThread = new Thread(StartRoomProcessing);
            _roomThread.Name = "Room Loader";
            _roomThread.Start();
            Plus.GetGame().GetRoomManager().QueueActiveRoomAdd(RoomData);
        }

        /// <summary>
        /// Works the room kick queue.
        /// </summary>
        private void WorkRoomKickQueue()
        {
            if (_roomKick.Count <= 0)
                return;
            lock (_roomKick.SyncRoot)
            {
                while (_roomKick.Count > 0)
                {
                    var roomKick = (RoomKick)_roomKick.Dequeue();
                    var list = new List<RoomUser>();
                    foreach (
                        var current in
                            _roomUserManager.UserList.Values.Where(
                                current =>
                                    !current.IsBot && current.GetClient().GetHabbo().Rank < (ulong)roomKick.MinRank))
                    {
                        if (roomKick.Alert.Length > 0)
                            current.GetClient().SendNotif(string.Format(Plus.GetLanguage().GetVar("kick_mod_room_message"), roomKick.Alert));
                        list.Add(current);
                    }
                    foreach (var current2 in list)
                    {
                        GetRoomUserManager().RemoveUserFromRoom(current2.GetClient(), true, false);
                        current2.GetClient().CurrentRoomUserId = -1;
                    }
                }
            }
        }

        internal List<RoomItem> ReturnRoleplaySpawn_Component(string component)
        {
            List<RoomItem> items = new List<RoomItem>();

            switch (component)
            {

                #region Colour wars
                case "anna_pill*4":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {
                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("anna_pill*4"))
                            continue;

                        items.Add(Item);
                    }
                    break;

                case "anna_pill*1":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {
                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("anna_pill*1"))
                            continue;

                        items.Add(Item);
                    }
                    break;

                case "anna_pill*3":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {
                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("anna_pill*3"))
                            continue;

                        items.Add(Item);
                    }
                    break;

                case "anna_pill*2":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {
                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("anna_pill*2"))
                            continue;

                        items.Add(Item);
                    }
                    break;
                #endregion

                #region Doormat
                case "doormat":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {
                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("doormat_love"))
                            continue;

                        items.Add(Item);
                    }
                    break;
                #endregion

                #region Hospital Bed
                case "hospital_bed":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {

                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("hosptl_bed"))
                            continue;

                        items.Add(Item);
                    }
                    break;
                #endregion

                #region Jail Bed
                case "jail_bed":
                    foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
                    {

                        if (Item == null)
                            continue;
                        if (Item.GetBaseItem() == null)
                            continue;
                        if (!Item.GetBaseItem().Name.Contains("army_c15_bed"))
                            continue;

                        items.Add(Item);
                    }
                    break;
                #endregion
            }

            return items;
        }

        internal RoomItem GetRandomItem(string item_name, bool cond2 = false, uint itemid = 0)
        {
            RoomItem Itemsearch = null;

            foreach (RoomItem Item in GetRoomItemHandler().FloorItems.Values)
            {
                if (cond2)
                {
                    if (itemid == Item.Id)
                        continue;
                }

                if (Item.GetBaseItem().Name.ToLower() == item_name.ToLower())
                {
                    Itemsearch = Item;
                }
            }

            return Itemsearch;
        }

        /// <summary>
        /// Called when [room crash].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnRoomCrash(Exception e)
        {
            var users = this.GetRoomUserManager().UserList.Values;
            Random errorcode = new Random();
            int EC = errorcode.Next(1, 999999999);

            Logging.LogThreadException(e.ToString(), string.Format("Room cycle task for room {0}", RoomId));
            Plus.GetGame().GetRoomManager().UnloadRoom(this, "Room crashed");

            // Puts users back to their room after crash
            Plus.GetGame().GetRoomManager().LoadRoom(RoomId);
            var roomFwd = new ServerMessage(LibraryParser.OutgoingRequest("RoomForwardMessageComposer"));
            roomFwd.AppendInteger(RoomId);

            var data = roomFwd.GetReversedBytes();

            foreach (var user in users.Where(user => user != null && user.GetClient() != null))
            {
                user.GetClient().SendMessage(data);
                user.GetClient().SendNotifWithScroll("The room you were currently in has unexpectedly crashed or corrupted. You have been transported back to the room you were currently in and the crash should be fixed!\n\nPlease report this error to one of the hotel administrator with the following: #" + EC + "^" + RoomId + "\n\nYou may report this error by going to http://forum.habflux.pw");
            }

            _isCrashed = true;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        private void Dispose()
        {
            _mCycleEnded = true;
            Plus.GetGame().GetRoomManager().QueueActiveRoomRemove(RoomData);
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                GetRoomItemHandler().SaveFurniture(queryReactor, null);
                queryReactor.RunFastQuery(string.Format("UPDATE rooms_data SET users_now=0 WHERE id = {0} LIMIT 1", RoomId));
            }

            if (_processTimer != null)
            {
                _processTimer.Dispose();
            }

            _processTimer = null;
            RoomData.Tags.Clear();
            _roomUserManager.UserList.Clear();
            UsersWithRights.Clear();
            Bans.Clear();
            LoadedGroups.Clear();

            RoomData.RoomChat.Clear();

            GetWiredHandler().Destroy();
            foreach (var current in GetRoomItemHandler().FloorItems.Values)
                current.Destroy();
            foreach (var current2 in GetRoomItemHandler().WallItems.Values)
                current2.Destroy();
            ActiveTrades.Clear();

            RoomData = null;
            Plus.GetGame().GetRoomManager().RemoveRoomData(RoomId);
        }
    }
}