using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.Messages;
using Plus.Messages.Parsers;
using Plus.Security;
using System;
using System.Collections.Generic;
using Plus.HabboHotel.Roleplay.Misc;
using System.Data;
using System.Linq;

namespace Plus.HabboHotel.Users.Messenger
{
    /// <summary>
    /// Class HabboMessenger.
    /// </summary>
    internal class HabboMessenger
    {
        /// <summary>
        /// The requests
        /// </summary>
        internal Dictionary<uint, MessengerRequest> Requests;

        /// <summary>
        /// The friends
        /// </summary>
        internal Dictionary<uint, MessengerBuddy> Friends;

        /// <summary>
        /// The appear offline
        /// </summary>
        internal bool AppearOffline;

        /// <summary>
        /// The _user identifier
        /// </summary>
        private readonly uint _userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="HabboMessenger"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        internal HabboMessenger(uint userId)
        {
            Requests = new Dictionary<uint, MessengerRequest>();
            Friends = new Dictionary<uint, MessengerBuddy>();
            _userId = userId;
        }

        /// <summary>
        /// Initializes the specified friends.
        /// </summary>
        /// <param name="friends">The friends.</param>
        /// <param name="requests">The requests.</param>
        internal void Init(Dictionary<uint, MessengerBuddy> friends, Dictionary<uint, MessengerRequest> requests)
        {
            Requests = new Dictionary<uint, MessengerRequest>(requests);
            Friends = new Dictionary<uint, MessengerBuddy>(friends);
        }

        /// <summary>
        /// Clears the requests.
        /// </summary>
        internal void ClearRequests()
        {
            Requests.Clear();
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <param name="senderId">The sender identifier.</param>
        /// <returns>MessengerRequest.</returns>
        internal MessengerRequest GetRequest(uint senderId)
        {
            return Requests.ContainsKey(senderId) ? Requests[senderId] : null;
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        internal void Destroy()
        {
            var clientsById = Plus.GetGame().GetClientManager().GetClientsById(Friends.Keys);
            foreach (var current in clientsById.Where(current => current.GetHabbo() != null && current.GetHabbo().GetMessenger() != null))
                current.GetHabbo().GetMessenger().UpdateFriend(_userId, null, true);

            Friends.Clear();
            Requests.Clear();
            Friends = null;
            Requests = null;
        }

        /// <summary>
        /// Called when [status changed].
        /// </summary>
        /// <param name="notification">if set to <c>true</c> [notification].</param>
        internal void OnStatusChanged(bool notification)
        {
            if (Friends == null)
                return;

            IEnumerable<GameClient> onlineUsers = Plus.GetGame().GetClientManager().GetClientsById(Friends.Keys);

            foreach (GameClient client in onlineUsers)
            {
                if (client == null || client.GetHabbo() == null || client.GetHabbo().GetMessenger() == null)
                    continue;

                client.GetHabbo().GetMessenger().UpdateFriend(_userId, client, true);
                if (client.GetHabbo() != null && client != null && notification != null)
                {
                    if (client.GetHabbo().Id != null && client.GetHabbo().Id != 0)
                    {
                        UpdateFriend(client.GetHabbo().Id, client, notification);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the friend.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="client">The client.</param>
        /// <param name="notification">if set to <c>true</c> [notification].</param>
        internal void UpdateFriend(uint userid, GameClient client, bool notification)
        {
            if (!Friends.ContainsKey(userid))
                return;
            Friends[userid].UpdateUser();
            if (!notification)
                return;
            var client2 = GetClient();
            if (client2 != null)
                client2.SendMessage(SerializeUpdate(Friends[userid]));
        }

        /// <summary>
        /// Serializes the messenger action.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        internal void SerializeMessengerAction(int type, string name)
        {
            if (GetClient() == null)
                return;
            var serverMessage = new ServerMessage();
            serverMessage.Init(LibraryParser.OutgoingRequest("ConsoleMessengerActionMessageComposer"));
            serverMessage.AppendString(GetClient().GetHabbo().Id.ToString());
            serverMessage.AppendInteger(type);
            serverMessage.AppendString(name);
            foreach (var current in Friends.Values.Where(current => current.Client != null))
                current.Client.SendMessage(serverMessage);
        }

        /// <summary>
        /// Handles all requests.
        /// </summary>
        internal void HandleAllRequests()
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery("DELETE FROM messenger_requests WHERE from_id = " + _userId + " OR to_id = " + _userId);
            ClearRequests();
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        internal void HandleRequest(uint sender)
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery(string.Concat(new object[]
                {
                    "DELETE FROM messenger_requests WHERE (from_id = ",
                    _userId,
                    " AND to_id = ",
                    sender,
                    ") OR (to_id = ",
                    _userId,
                    " AND from_id = ",
                    sender,
                    ")"
                }));
            Requests.Remove(sender);
        }

        /// <summary>
        /// Creates the friendship.
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        internal void CreateFriendship(uint friendId)
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery(string.Concat(new object[]
                {
                    "REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES (",
                    _userId,
                    ",",
                    friendId,
                    ")"
                }));
            OnNewFriendship(friendId);
            var clientByUserId = Plus.GetGame().GetClientManager().GetClientByUserId(friendId);
            Plus.GetGame().GetAchievementManager().ProgressUserAchievement(clientByUserId, "ACH_FriendListSize", 1, false);
            if (clientByUserId != null && clientByUserId.GetHabbo().GetMessenger() != null)
                clientByUserId.GetHabbo().GetMessenger().OnNewFriendship(_userId);
        }

        /// <summary>
        /// Destroys the friendship.
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        internal void DestroyFriendship(uint friendId)
        {
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery(string.Concat(new object[]
                {
                    "DELETE FROM messenger_friendships WHERE (user_one_id = ",
                    _userId,
                    " AND user_two_id = ",
                    friendId,
                    ") OR (user_two_id = ",
                    _userId,
                    " AND user_one_id = ",
                    friendId,
                    ")"
                }));
            OnDestroyFriendship(friendId);
            var clientByUserId = Plus.GetGame().GetClientManager().GetClientByUserId(friendId);
            if (clientByUserId != null && clientByUserId.GetHabbo().GetMessenger() != null)
                clientByUserId.GetHabbo().GetMessenger().OnDestroyFriendship(_userId);
        }

        /// <summary>
        /// Called when [new friendship].
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        internal void OnNewFriendship(uint friendId)
        {
            var clientByUserId = Plus.GetGame().GetClientManager().GetClientByUserId(friendId);
            MessengerBuddy messengerBuddy;
            if (clientByUserId == null || clientByUserId.GetHabbo() == null)
            {
                DataRow row;
                using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery(
                        string.Format(
                            "SELECT id,username,motto,look,last_online,hide_inroom,hide_online FROM users WHERE id = {0}",
                            friendId));
                    row = queryReactor.GetRow();
                }
                messengerBuddy = new MessengerBuddy(friendId, (string)row["Username"], (string)row["look"],
                    (string)row["motto"], (int)row["last_online"], Plus.EnumToBool(row["hide_online"].ToString()),
                    Plus.EnumToBool(row["hide_inroom"].ToString()));
            }
            else
            {
                var habbo = clientByUserId.GetHabbo();
                messengerBuddy = new MessengerBuddy(friendId, habbo.UserName, habbo.Look, habbo.Motto, 0,
                    habbo.AppearOffline, habbo.HideInRoom);
                messengerBuddy.UpdateUser();
            }
            if (!Friends.ContainsKey(friendId))
                Friends.Add(friendId, messengerBuddy);
            GetClient().SendMessage(SerializeUpdate(messengerBuddy));
        }

        /// <summary>
        /// Requests the exists.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool RequestExists(uint requestId)
        {
            if (Requests.ContainsKey(requestId))
                return true;

            {
                bool result;
                using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery(
                        "SELECT user_one_id FROM messenger_friendships WHERE user_one_id = @myID AND user_two_id = @friendID");
                    queryReactor.AddParameter("myID", (int)_userId);
                    queryReactor.AddParameter("friendID", (int)requestId);
                    result = queryReactor.FindsResult();
                }
                return result;
            }
        }

        /// <summary>
        /// Friendships the exists.
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool FriendshipExists(uint friendId)
        {
            return Friends.ContainsKey(friendId);
        }

        /// <summary>
        /// Called when [destroy friendship].
        /// </summary>
        /// <param name="friend">The friend.</param>
        internal void OnDestroyFriendship(uint friend)
        {
            Friends.Remove(friend);
            GetClient()
                .GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("FriendUpdateMessageComposer"));
            GetClient().GetMessageHandler().GetResponse().AppendInteger(0);
            GetClient().GetMessageHandler().GetResponse().AppendInteger(1);
            GetClient().GetMessageHandler().GetResponse().AppendInteger(-1);
            GetClient().GetMessageHandler().GetResponse().AppendInteger(friend);
            GetClient().GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Requests the buddy.
        /// </summary>
        /// <param name="userQuery">The user query.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool RequestBuddy(string userQuery)
        {
            var clientByUsername = Plus.GetGame().GetClientManager().GetClientByUserName(userQuery);
            uint num;
            bool flag;

            if (GetClient().GetRoleplay().Phone == 0)
            {
                GetClient().SendNotif("You do not have a phone to add contacts!");
                return true;
            }

            if (clientByUsername == null)
            {
                DataRow dataRow;
                using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT id,block_newfriends FROM users WHERE Username = @query");
                    queryReactor.AddParameter("query", userQuery.ToLower());
                    dataRow = queryReactor.GetRow();
                }
                if (dataRow == null)
                    return false;
                num = Convert.ToUInt32(dataRow["id"]);
                flag = Plus.EnumToBool(dataRow["block_newfriends"].ToString());
            }
            else
            {
                num = clientByUsername.GetHabbo().Id;
                flag = clientByUsername.GetHabbo().HasFriendRequestsDisabled;
            }
            if (flag && GetClient().GetHabbo().Rank < 4u)
            {
                GetClient()
                    .GetMessageHandler()
                    .GetResponse()
                    .Init(LibraryParser.OutgoingRequest("NotAcceptingRequestsMessageComposer"));
                GetClient().GetMessageHandler().GetResponse().AppendInteger(39);
                GetClient().GetMessageHandler().GetResponse().AppendInteger(3);
                GetClient().GetMessageHandler().SendResponse();
                return false;
            }
            var num2 = num;
            if (RequestExists(num2))
            {
                GetClient().SendNotif("Ya le has enviado una petici�n anteriormente.");
                return true;
            }
            using (var queryreactor2 = Plus.GetDatabaseManager().GetQueryReactor())
                queryreactor2.RunFastQuery(string.Concat(new object[]
                {
                    "REPLACE INTO messenger_requests (from_id,to_id) VALUES (",
                    _userId,
                    ",",
                    num2,
                    ")"
                }));
            Plus.GetGame().GetQuestManager().ProgressUserQuest(GetClient(), QuestType.AddFriends, 0u);
            var clientByUserId = Plus.GetGame().GetClientManager().GetClientByUserId(num2);
            if (clientByUserId == null || clientByUserId.GetHabbo() == null)
                return true;
            var messengerRequest = new MessengerRequest(num2, _userId,
                Plus.GetGame().GetClientManager().GetNameById(_userId));
            clientByUserId.GetHabbo().GetMessenger().OnNewRequest(_userId);
            var serverMessage =
                new ServerMessage(LibraryParser.OutgoingRequest("ConsoleSendFriendRequestMessageComposer"));
            messengerRequest.Serialize(serverMessage);
            clientByUserId.SendMessage(serverMessage);
            Requests.Add(num2, messengerRequest);
            return true;
        }

        /// <summary>
        /// Called when [new request].
        /// </summary>
        /// <param name="friendId">The friend identifier.</param>
        internal void OnNewRequest(uint friendId)
        {
            if (!Requests.ContainsKey(friendId))
                Requests.Add(friendId,
                    new MessengerRequest(_userId, friendId, Plus.GetGame().GetClientManager().GetNameById(friendId)));
        }

        /// <summary>
        /// Sends the instant message.
        /// </summary>
        /// <param name="toId">To identifier.</param>
        /// <param name="message">The message.</param>
        internal void SendInstantMessage(uint toId, string message)
        {
            int credit = new Random().Next(1, 15);

            if (!GetClient().GetHabbo().HasFuse("fuse_owner") && AntiPublicistas.CheckPublicistas(message))
            {
                GetClient().PublicistCount++;
                GetClient().HandlePublicista(message);
                return;
            }
            if (!FriendshipExists(toId))
            {
                DeliverInstantMessageError(6, toId);
                return;
            }

            if (GetClient().GetRoleplay().Phone == 0)
            {
                GetClient().SendWhisperBubble("Voc� n�o tem telefone! Voc� pode comprar um na loja de telefone! [Room ID: 5]", 1);
                return;
            }
            if (GetClient().GetHabbo().ActivityPoints < credit)
            {
                GetClient().SendWhisperBubble("Voc� tem fundos de cr�dito insuficientes! Voc� pode comprar mais na loja de telefone! [Room ID: 5]", 1);
                return;
            }

            if (toId == 0) // Staff Chat
            {
                var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("ConsoleChatMessageComposer"));
                serverMessage.AppendInteger(0); //userid
                serverMessage.AppendString(GetClient().GetHabbo().UserName + " : " + message);
                serverMessage.AppendInteger(0);
                Plus.GetGame().GetClientManager().StaffAlert(serverMessage, GetClient().GetHabbo().Id);
            }
            else
            {
                var clientByUserId = Plus.GetGame().GetClientManager().GetClientByUserId(toId);
                if (clientByUserId == null || clientByUserId.GetHabbo().GetMessenger() == null)
                {
                    if (Plus.OfflineMessages.ContainsKey(toId))
                        Plus.OfflineMessages[toId].Add(new OfflineMessage(GetClient().GetHabbo().Id, message,
                            Plus.GetUnixTimeStamp()));
                    else
                    {
                        Plus.OfflineMessages.Add(toId, new List<OfflineMessage>());
                        Plus.OfflineMessages[toId].Add(new OfflineMessage(GetClient().GetHabbo().Id, message,
                            Plus.GetUnixTimeStamp()));
                    }
                    OfflineMessage.SaveMessage(Plus.GetDatabaseManager().GetQueryReactor(), toId,
                        GetClient().GetHabbo().Id, message);
                    return;
                }
                if (GetClient().GetHabbo().Muted)
                {
                    DeliverInstantMessageError(4, toId);
                    return;
                }
                if (clientByUserId.GetHabbo().Muted) DeliverInstantMessageError(3, toId);
                if (message == "") return;
                clientByUserId.GetHabbo().GetMessenger().DeliverInstantMessage(message, _userId);

                #region Whisper
                if (GetClient().GetHabbo().CurrentRoomId != 0)
                {
                    var roomUserByRank = GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByRank(2);
                    RoomUser u = GetClient().GetHabbo().GetRoomUser();
                    RoomUser u2 = clientByUserId.GetHabbo().GetRoomUser();

                    foreach (var current2 in roomUserByRank)
                        if (current2 != null && current2.HabboId != u2.HabboId &&
                            current2.HabboId != u.HabboId && current2.GetClient() != null)
                        {
                            if (RoleplayManager.BypassRights(current2.GetClient()))
                            {
                                var whispStaff = new ServerMessage(LibraryParser.OutgoingRequest("WhisperMessageComposer"));
                                whispStaff.AppendInteger(u.VirtualId);
                                whispStaff.AppendString(string.Format("PM to {0}: {1}", clientByUserId.GetHabbo().UserName, message));
                                whispStaff.AppendInteger(0);
                                whispStaff.AppendInteger(0);
                                whispStaff.AppendInteger(0);
                                whispStaff.AppendInteger(-1);
                                current2.GetClient().SendMessage(whispStaff);
                            }
                        }
                }
                #endregion

                RoleplayManager.GiveCredit(GetClient(), -credit);
            }
            LogPM(_userId, toId, message);
        }

        
        internal void LogPM(uint From_Id, uint ToId, string Message)
        {
            var arg_10_0 = GetClient().GetHabbo().Id;
            var arg_16_0 = DateTime.Now;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Concat(new object[]
                {
                    "INSERT INTO users_chatlogs_console VALUES (NULL, ",
                    From_Id,
                    ", ",
                    ToId,
                    ", @Message, UNIX_TIMESTAMP())"
                }));
                queryReactor.AddParameter("message", Message);
                queryReactor.RunQuery();
            }
        }
         

        /// <summary>
        /// Delivers the instant message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="convoId">The convo identifier.</param>
        internal void DeliverInstantMessage(string message, uint convoId)
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("ConsoleChatMessageComposer"));
            serverMessage.AppendInteger(convoId);
            serverMessage.AppendString(message);
            serverMessage.AppendInteger(0);
            GetClient().SendMessage(serverMessage);
        }

        /// <summary>
        /// Delivers the instant message error.
        /// </summary>
        /// <param name="errorId">The error identifier.</param>
        /// <param name="conversationId">The conversation identifier.</param>
        internal void DeliverInstantMessageError(int errorId, uint conversationId)
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("ConsoleChatErrorMessageComposer"));
            serverMessage.AppendInteger(errorId);
            serverMessage.AppendInteger(conversationId);
            serverMessage.AppendString("");
            GetClient().SendMessage(serverMessage);
        }

        /// <summary>
        /// Serializes the categories.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeCategories()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("LoadFriendsCategories"));
            serverMessage.AppendInteger(20000);
            serverMessage.AppendInteger(300);
            serverMessage.AppendInteger(800);
            serverMessage.AppendInteger(2000);
            serverMessage.AppendInteger(0); // categories
            // int id
            // str name
            return serverMessage;
        }

        /// <summary>
        /// Serializes the friends.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeFriends()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("LoadFriendsMessageComposer"));
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(0);
            serverMessage.AppendInteger(Friends.Count);
            foreach (var current in Friends.Values)
            {
                current.UpdateUser();
                current.Serialize(serverMessage, GetClient());
            }
            return serverMessage;
        }

        /// <summary>
        /// Serializes the offline messages.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeOfflineMessages(OfflineMessage message)
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("ConsoleChatMessageComposer"));
            serverMessage.AppendInteger(message.FromId);
            serverMessage.AppendString(message.Message);
            serverMessage.AppendInteger(((int)(Plus.GetUnixTimeStamp() - message.Timestamp)));

            return serverMessage;
        }

        /// <summary>
        /// Serializes the update.
        /// </summary>
        /// <param name="friend">The friend.</param>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeUpdate(MessengerBuddy friend)
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("FriendUpdateMessageComposer"));
            serverMessage.AppendInteger(0);
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(0);
            friend.Serialize(serverMessage, GetClient());
            serverMessage.AppendBool(false);
            return serverMessage;
        }

        /// <summary>
        /// Serializes the requests.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeRequests()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("FriendRequestsMessageComposer"));
            serverMessage.AppendInteger(((long)Requests.Count > (long)((ulong)Plus.FriendRequestLimit))
                ? ((int)Plus.FriendRequestLimit)

                : Requests.Count);
            serverMessage.AppendInteger(((long)Requests.Count > (long)((ulong)Plus.FriendRequestLimit))
                ? ((int)Plus.FriendRequestLimit)

                : Requests.Count);
            var num = 0;
            foreach (var current in Requests.Values)
            {
                {
                    num++;
                }
                if (num > Plus.FriendRequestLimit)
                    break;
                current.Serialize(serverMessage);
            }
            return serverMessage;
        }

        /// <summary>
        /// Performs the search.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage PerformSearch(string query)
        {
            var searchResult = SearchResultFactory.GetSearchResult(query);
            var list = new List<SearchResult>();
            var list2 = new List<SearchResult>();
            foreach (var current in searchResult.Where(current => current.UserId != GetClient().GetHabbo().Id))
                if (FriendshipExists(current.UserId))
                    list.Add(current);
                else
                    list2.Add(current);
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("ConsoleSearchFriendMessageComposer"));
            serverMessage.AppendInteger(list.Count);
            foreach (var current2 in list)
                current2.Searialize(serverMessage);
            serverMessage.AppendInteger(list2.Count);
            foreach (var current3 in list2)
                current3.Searialize(serverMessage);
            return serverMessage;
        }

        /// <summary>
        /// Gets the active friends rooms.
        /// </summary>
        /// <returns>HashSet&lt;RoomData&gt;.</returns>
        internal HashSet<RoomData> GetActiveFriendsRooms()
        {
            var toReturn = new HashSet<RoomData>();
            foreach (var current in
                from p in Friends.Values
                where p != null && p.InRoom && p.CurrentRoom != null && p.CurrentRoom.RoomData != null
                select p)
                toReturn.Add(current.CurrentRoom.RoomData);
            return toReturn;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns>GameClient.</returns>
        private GameClient GetClient()
        {
            return Plus.GetGame().GetClientManager().GetClientByUserId(_userId);
        }
    }
}