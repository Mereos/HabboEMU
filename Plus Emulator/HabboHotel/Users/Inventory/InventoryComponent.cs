using Plus.Configuration;
using Plus.HabboHotel.Catalogs;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Pets;
using Plus.HabboHotel.RoomBots;
using Plus.HabboHotel.Users.UserDataManagement;
using Plus.Messages;
using Plus.Messages.Parsers;
using Plus.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;//

namespace Plus.HabboHotel.Users.Inventory
{
    /// <summary>
    /// Class InventoryComponent.
    /// </summary>
    internal class InventoryComponent
    {
        /// <summary>
        /// The user identifier
        /// </summary>
        internal uint UserId;

        /// <summary>
        /// The _floor items
        /// </summary>
        private readonly HybridDictionary _floorItems;

        /// <summary>
        /// The _wall items
        /// </summary>
        private readonly HybridDictionary _wallItems;

        /// <summary>
        /// The _inventory pets
        /// </summary>
        private readonly HybridDictionary _inventoryPets;

        /// <summary>
        /// The _m added items
        /// </summary>
        private readonly HybridDictionary _mAddedItems;

        /// <summary>
        /// The _m removed items
        /// </summary>
        private readonly HybridDictionary _mRemovedItems;

        /// <summary>
        /// The _inventory bots
        /// </summary>
        private readonly HybridDictionary _inventoryBots;

        /// <summary>
        /// The _m client
        /// </summary>
        private GameClient _mClient;

        /// <summary>
        /// The _is updated
        /// </summary>
        private bool _isUpdated;

        /// <summary>
        /// The _user attatched
        /// </summary>
        private bool _userAttatched;

        public int TotalItems { get { return _floorItems.Count + _wallItems.Count + SongDisks.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryComponent"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="client">The client.</param>
        /// <param name="userData">The user data.</param>
        internal InventoryComponent(uint userId, GameClient client, UserData userData)
        {
            _mClient = client;
            UserId = userId;
            _floorItems = new HybridDictionary();
            _wallItems = new HybridDictionary();
            SongDisks = new HybridDictionary();
            foreach (var current in userData.Inventory)
            {
                if (current.BaseItem.InteractionType == Interaction.MusicDisc)
                    SongDisks.Add(current.Id, current);
                if (current.IsWallItem)
                    _wallItems.Add(current.Id, current);
                else
                    _floorItems.Add(current.Id, current);
            }
            _inventoryPets = new HybridDictionary();
            _inventoryBots = new HybridDictionary();
            _mAddedItems = new HybridDictionary();
            _mRemovedItems = new HybridDictionary();
            _isUpdated = false;

            foreach (var bot in userData.Bots)
                AddBot(bot.Value);

            foreach (var pet in userData.Pets)
                AddPet(pet.Value);       
        }

        public void RefreshPets()
        {
            using (var queryReactor2 = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor2.SetQuery(
                    string.Format("SELECT * FROM bots WHERE user_id = {0} AND room_id = 0 OR ai_type = 'fightpet' AND user_id = {0}",
                        UserId));
                var table2 = queryReactor2.GetTable();
                if (table2 == null)
                    return;
                foreach (DataRow botRow in table2.Rows)
                    switch ((string)botRow["ai_type"])
                    {
                        case "pet":
                            {
                                queryReactor2.SetQuery(string.Format("SELECT * FROM pets_data WHERE id={0} LIMIT 1",
                                    botRow[0]));
                                var row = queryReactor2.GetRow();
                                if (row == null)
                                    continue;
                                var pet = Catalog.GeneratePetFromRow(botRow, row);
                                AddPet(pet);
                            }
                            break;

                        case "fightpet":
                            {
                                queryReactor2.SetQuery(string.Format("SELECT * FROM pets_data WHERE id={0} LIMIT 1",
                                    botRow[0]));
                                var row = queryReactor2.GetRow();
                                if (row == null)
                                    continue;
                                var pet = Catalog.GeneratePetFromRow(botRow, row);
                                AddPet(pet);
                            }
                            break;

                        case "generic":
                            AddBot(BotManager.GenerateBotFromRow(botRow));
                            break;
                    }
            }

            SerializePetInventory();
        }


        /// <summary>
        /// Gets a value indicating whether this instance is inactive.
        /// </summary>
        /// <value><c>true</c> if this instance is inactive; otherwise, <c>false</c>.</value>
        public bool IsInactive
        {
            get { return !_userAttatched; }
        }

        /// <summary>
        /// Gets a value indicating whether [needs update].
        /// </summary>
        /// <value><c>true</c> if [needs update]; otherwise, <c>false</c>.</value>
        internal bool NeedsUpdate
        {
            get { return !_userAttatched && !_isUpdated; }
        }

        /// <summary>
        /// Gets the song disks.
        /// </summary>
        /// <value>The song disks.</value>
        internal HybridDictionary SongDisks { get; private set; }

        /// <summary>
        /// Clears the items.
        /// </summary>
        internal void ClearItems()
        {
            UpdateItems(true);
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                queryReactor.RunFastQuery(string.Format("DELETE FROM items_rooms WHERE room_id='0' AND user_id = {0}",
                    UserId));
            _mAddedItems.Clear();
            _mRemovedItems.Clear();
            _floorItems.Clear();
            _wallItems.Clear();
            SongDisks.Clear();
            _inventoryPets.Clear();
            _isUpdated = true;
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("UpdateInventoryMessageComposer"));
            GetClient().GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Redeemcreditses the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        internal void Redeemcredits(GameClient session)
        {
            var currentRoom = session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;
            DataTable table;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(string.Format("SELECT id FROM items_rooms WHERE user_id={0} AND room_id='0'",
                    session.GetHabbo().Id));
                table = queryReactor.GetTable();
            }

            {
                foreach (DataRow dataRow in table.Rows)
                {
                    var item = GetItem(Convert.ToUInt32(dataRow[0]));
                    if (item == null ||
                        (!item.BaseItem.Name.StartsWith("CF_") && !item.BaseItem.Name.StartsWith("CFC_"))) continue;
                    var array = item.BaseItem.Name.Split('_');
                    var num = int.Parse(array[1]);
                    using (var queryreactor2 = Plus.GetDatabaseManager().GetQueryReactor())
                        queryreactor2.RunFastQuery(string.Format("DELETE FROM items_rooms WHERE id={0} LIMIT 1",
                            item.Id));

                    currentRoom.GetRoomItemHandler().RemoveItem(item.Id);
                    RemoveItem(item.Id, false);
                    if (num <= 0) continue;
                    session.GetHabbo().Credits += num;
                    session.GetHabbo().UpdateCreditsBalance();
                }
            }
        }

        /// <summary>
        /// Sets the state of the active.
        /// </summary>
        /// <param name="client">The client.</param>
        internal void SetActiveState(GameClient client)
        {
            _mClient = client;
            _userAttatched = true;
        }

        /// <summary>
        /// Sets the state of the idle.
        /// </summary>
        internal void SetIdleState()
        {
            _userAttatched = false;
            _mClient = null;
        }

        /// <summary>
        /// Gets the pet.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Pet.</returns>
        internal Pet GetPet(uint id)
        {
            return _inventoryPets.Contains(id) ? _inventoryPets[id] as Pet : null;
        }

        internal Pet GetFirstPet()
        {
            lock (_inventoryPets.Values)
            {
                foreach (Pet Pet in _inventoryPets.Values)
                {
                    if (Pet != null)
                        return Pet;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes the pet.
        /// </summary>
        /// <param name="petId">The pet identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool RemovePet(uint petId)
        {
            _isUpdated = false;
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("604)"));
            serverMessage.AppendInteger(petId);
            GetClient().SendMessage(serverMessage);
            _inventoryPets.Remove(petId);
            return true;
        }

        /// <summary>
        /// Moves the pet to room.
        /// </summary>
        /// <param name="petId">The pet identifier.</param>
        internal void MovePetToRoom(uint petId)
        {
            _isUpdated = false;
            RemovePet(petId);
        }

        /// <summary>
        /// Adds the pet.
        /// </summary>
        /// <param name="pet">The pet.</param>
        internal void AddPet(Pet pet)
        {
            _isUpdated = false;
            if (pet == null || _inventoryPets.Contains(pet.PetId))
                return;
            pet.PlacedInRoom = false;
            pet.RoomId = 0u;
            _inventoryPets.Add(pet.PetId, pet);

            SerializePetInventory();
        }

        internal HybridDictionary getRocks()
        {
            HybridDictionary rocks = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 1943)
                    {
                        rocks.Add(item.Id, item);
                    }
                }

                return rocks;
            }
            catch { return rocks; }
        }

        internal HybridDictionary getTrees()
        {
            HybridDictionary trees = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 2694)
                    {
                        trees.Add(item.Id, item);
                    }
                }

                return trees;
            }
            catch { return trees; }
        }

        internal HybridDictionary getMedi()
        {
            HybridDictionary medi = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 8039)
                    {
                        medi.Add(item.Id, item);
                    }
                }

                return medi;
            }
            catch { return medi; }
        }

        internal HybridDictionary getBrotein()
        {
            HybridDictionary brotein = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 4495)
                    {
                        brotein.Add(item.Id, item);
                    }
                }

                return brotein;
            }
            catch { return brotein; }
        }

        internal HybridDictionary getEnergyd()
        {
            HybridDictionary edrink = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 3514)
                    {
                        edrink.Add(item.Id, item);
                    }
                }

                return edrink;
            }
            catch { return edrink; }
        }

        internal HybridDictionary getBagsLMFAO()
        {
            HybridDictionary rocks = new HybridDictionary();

            try
            {
                foreach (UserItem item in _floorItems.Values)
                {
                    if (item.BaseItemId == 1943)
                    {
                        rocks.Add(item.Id, item);
                    }
                }

                return rocks;
            }
            catch { return rocks; }
        }

        internal UserItem GetBag()
        {
            lock (_floorItems.Values)
            {

                List<uint> Bags = new List<uint>();

                foreach (UserItem Item in _floorItems.Values)
                {
                    if (Item.BaseItem.Name.ToLower().Contains("sleepingbag*"))
                    {
                        return Item;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Loads the inventory.
        /// </summary>
        internal void LoadInventory()
        {
            _floorItems.Clear();
            _wallItems.Clear();
            DataTable table;
            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT * FROM items_rooms WHERE user_id=@userid AND room_id='0' LIMIT 8000;");
                queryReactor.AddParameter("userid", ((int)UserId));

                table = queryReactor.GetTable();
            }
            foreach (DataRow dataRow in table.Rows)
            {
                var id = Convert.ToUInt32(dataRow[0]);
                var itemId = Convert.ToUInt32(dataRow[3]);

                if (!Plus.GetGame().GetItemManager().ContainsItem(itemId))
                    continue;

                string extraData;
                if (!DBNull.Value.Equals(dataRow[4]))
                    extraData = (string)dataRow[4];
                else
                    extraData = string.Empty;
                var group = Convert.ToUInt32(dataRow["group_id"]);
                string songCode;
                if (!DBNull.Value.Equals(dataRow["songcode"]))
                    songCode = (string)dataRow["songcode"];
                else
                    songCode = string.Empty;
                var userItem = new UserItem(id, itemId, extraData, group, songCode);

                if (userItem.BaseItem.InteractionType == Interaction.MusicDisc && !SongDisks.Contains(id))
                    SongDisks.Add(id, userItem);
                if (userItem.IsWallItem)
                {
                    if (!_wallItems.Contains(id))
                        _wallItems.Add(id, userItem);
                }
                else if (!_floorItems.Contains(id))
                    _floorItems.Add(id, userItem);
            }
            SongDisks.Clear();
            _inventoryPets.Clear();
            _inventoryBots.Clear();

            using (var queryReactor2 = Plus.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor2.SetQuery(
                    string.Format("SELECT * FROM bots WHERE user_id = {0} AND room_id = 0",
                        UserId));
                var table2 = queryReactor2.GetTable();
                if (table2 == null)
                    return;
                foreach (DataRow botRow in table2.Rows)
                    switch ((string)botRow["ai_type"])
                    {
                        case "pet":
                            {
                                queryReactor2.SetQuery(string.Format("SELECT * FROM pets_data WHERE id={0} LIMIT 1",
                                    botRow[0]));
                                var row = queryReactor2.GetRow();
                                if (row == null)
                                    continue;
                                var pet = Catalog.GeneratePetFromRow(botRow, row);
                                AddPet(pet);
                            }
                            break;

                        case "fightpet":
                            {
                                queryReactor2.SetQuery(string.Format("SELECT * FROM pets_data WHERE id={0} LIMIT 1",
                                    botRow[0]));
                                var row = queryReactor2.GetRow();
                                if (row == null)
                                    continue;
                                var pet = Catalog.GeneratePetFromRow(botRow, row);
                                AddPet(pet);
                            }
                            break;

                        case "generic":
                            AddBot(BotManager.GenerateBotFromRow(botRow));
                            break;
                    }
            }
        }

        /// <summary>
        /// Updates the items.
        /// </summary>
        /// <param name="fromDatabase">if set to <c>true</c> [from database].</param>
        internal void UpdateItems(bool fromDatabase)
        {
            if (fromDatabase)
            {
                RunDbUpdate();
                LoadInventory();
            }
            _mClient.GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("UpdateInventoryMessageComposer"));
            _mClient.GetMessageHandler().SendResponse();
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>UserItem.</returns>
        internal UserItem GetItem(uint id)
        {
            _isUpdated = false;
            if (_floorItems.Contains(id))
                return (UserItem)_floorItems[id];
            if (_wallItems.Contains(id))
                return (UserItem)_wallItems[id];
            return null;
        }

        internal bool HasBaseItem(uint id)
        {
            if (
                _floorItems.Values.Cast<UserItem>()
                    .Any(item => item != null && item.BaseItem != null && item.BaseItem.ItemId == id)) return true;
            return _wallItems.Values.Cast<UserItem>()
                .Any(item => item != null && item.BaseItem != null && item.BaseItem.ItemId == id);
        }

        /// <summary>
        /// Adds the bot.
        /// </summary>
        /// <param name="bot">The bot.</param>
        internal void AddBot(RoomBot bot)
        {
            _isUpdated = false;
            if (bot == null || _inventoryBots.Contains(bot.BotId))
                return;

            bot.RoomId = 0u;
            _inventoryBots.Add(bot.BotId, bot);
        }

        /// <summary>
        /// Gets the bot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>RoomBot.</returns>
        internal RoomBot GetBot(uint id)
        {
            return _inventoryBots.Contains(id) ? _inventoryBots[id] as RoomBot : null;
        }

        /// <summary>
        /// Removes the bot.
        /// </summary>
        /// <param name="petId">The pet identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal bool RemoveBot(uint petId)
        {
            _isUpdated = false;
            if (_inventoryBots.Contains(petId))
                _inventoryBots.Remove(petId);
            return true;
        }

        /// <summary>
        /// Moves the bot to room.
        /// </summary>
        /// <param name="petId">The pet identifier.</param>
        internal void MoveBotToRoom(uint petId)
        {
            _isUpdated = false;
            RemoveBot(petId);
        }

        /// <summary>
        /// Adds the new item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="baseItem">The base item.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="group">The group.</param>
        /// <param name="insert">if set to <c>true</c> [insert].</param>
        /// <param name="fromRoom">if set to <c>true</c> [from room].</param>
        /// <param name="limno">The limno.</param>
        /// <param name="limtot">The limtot.</param>
        /// <param name="songCode">The song code.</param>
        /// <returns>UserItem.</returns>
        internal UserItem AddNewItem(uint id, uint baseItem, string extraData, uint group, bool insert, bool fromRoom,
                                     int limno, int limtot, string songCode = "")
        {
            _isUpdated = false;
            if (insert)
            {
                if (fromRoom)
                {
                    using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.RunFastQuery("UPDATE items_rooms SET room_id = '0' WHERE id = " + id);
                    }
                }
                else
                {

                    using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery(
                            string.Format(
                                "INSERT INTO items_rooms (base_item, user_id, group_id) VALUES ('{0}', '{1}', '{2}');",
                                baseItem, UserId, group));
                        if (id == 0) id = ((uint) queryReactor.InsertQuery());

                        SendNewItems(id);

                        if (!string.IsNullOrEmpty(extraData))
                        {
                            queryReactor.SetQuery("UPDATE items_rooms SET extra_data = @extraData WHERE id = " + id);
                            queryReactor.AddParameter("extraData", extraData);
                            queryReactor.RunQuery();
                        }
                        if (limno > 0)
                            queryReactor.RunFastQuery(
                                string.Format("INSERT INTO items_limited VALUES ('{0}', '{1}', '{2}');", id, limno,
                                    limtot));
                        if (!string.IsNullOrEmpty(songCode))
                        {
                            queryReactor.SetQuery(
                                string.Format("UPDATE items_rooms SET songcode='{0}' WHERE id='{1}' LIMIT 1", songCode,
                                    id));
                            queryReactor.RunQuery();
                        }
                    }
                }
            }
            if (id == 0) return null;

            var userItem = new UserItem(id, baseItem, extraData, @group, songCode);
            if (UserHoldsItem(id)) RemoveItem(id, false);
            if (userItem.BaseItem.InteractionType == Interaction.MusicDisc) SongDisks.Add(userItem.Id, userItem);
            if (userItem.IsWallItem) _wallItems.Add(userItem.Id, userItem);
            else _floorItems.Add(userItem.Id, userItem);
            if (_mRemovedItems.Contains(id)) _mRemovedItems.Remove(id);
            if (!_mAddedItems.Contains(id)) _mAddedItems.Add(id, userItem);
            return userItem;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="placedInroom">if set to <c>true</c> [placed inroom].</param>
        internal void RemoveItem(uint id, bool placedInroom)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null ||
                GetClient().GetHabbo().GetInventoryComponent() == null)
                GetClient().Disconnect("user null RemoveItem");
            _isUpdated = false;
            GetClient()
                .GetMessageHandler()
                .GetResponse()
                .Init(LibraryParser.OutgoingRequest("RemoveInventoryObjectMessageComposer"));
            GetClient().GetMessageHandler().GetResponse().AppendInteger(id);
            //this.GetClient().GetMessageHandler().GetResponse().AppendInt32(Convert.ToInt32(this.GetClient().GetHabbo().Id));
            GetClient().GetMessageHandler().SendResponse();
            if (_mAddedItems.Contains(id))
                _mAddedItems.Remove(id);
            if (_mRemovedItems.Contains(id))
                return;
            var item = GetClient().GetHabbo().GetInventoryComponent().GetItem(id);
            SongDisks.Remove(id);
            _floorItems.Remove(id);
            _wallItems.Remove(id);
            _mRemovedItems.Add(id, item);
        }

        /// <summary>
        /// Serializes the floor item inventory.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeFloorItemInventory()
        {
            var i = (_floorItems.Count + SongDisks.Count + _wallItems.Count);

            if (i > 2800)
            {
                _mClient.SendMessage(StaticMessage.AdviceMaxItems);
            }
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("LoadInventoryMessageComposer"));
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(0);
            serverMessage.AppendInteger(i > 2800 ? 2800 : i);

            var inc = 0;
            foreach (UserItem userItem in _floorItems.Values)
            {
                if (inc == 2800) return serverMessage;
                inc++;
                userItem.SerializeFloor(serverMessage, true);
            }

            foreach (UserItem userItem in _wallItems.Values)
            {
                if (inc == 2800) return serverMessage;
                inc++;
                userItem.SerializeWall(serverMessage, true);
            }

            foreach (UserItem userItem in SongDisks.Values)
            {
                if (inc == 2800) return serverMessage;
                inc++;
                userItem.SerializeFloor(serverMessage, true);
            }

            return serverMessage;
        }

        /// <summary>
        /// Serializes the wall item inventory.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeWallItemInventory()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("LoadInventoryMessageComposer"));
            serverMessage.AppendString("I");
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(_wallItems.Count);
            foreach (UserItem userItem in _wallItems.Values)
                userItem.SerializeWall(serverMessage, true);
            return serverMessage;
        }

        /// <summary>
        /// Serializes the pet inventory.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializePetInventory()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("PetInventoryMessageComposer"));
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(_inventoryPets.Count);
            foreach (Pet current in _inventoryPets.Values)
                current.SerializeInventory(serverMessage);
            return serverMessage;
        }

        /// <summary>
        /// Serializes the bot inventory.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeBotInventory()
        {
            var serverMessage = new ServerMessage();
            serverMessage.Init(LibraryParser.OutgoingRequest("BotInventoryMessageComposer"));

            serverMessage.AppendInteger(_inventoryBots.Count);
            foreach (RoomBot current in _inventoryBots.Values)
            {
                serverMessage.AppendInteger(current.BotId);
                serverMessage.AppendString(current.Name);
                serverMessage.AppendString(current.Motto);
                serverMessage.AppendString("m");
                serverMessage.AppendString(current.Look);
            }
            return serverMessage;
        }

        /// <summary>
        /// Adds the item array.
        /// </summary>
        /// <param name="roomItemList">The room item list.</param>
        internal void AddItemArray(List<RoomItem> roomItemList)
        {
            foreach (var current in roomItemList)
                AddItem(current);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void AddItem(RoomItem item)
        {
            AddNewItem(item.Id, item.BaseItem, item.ExtraData, item.GroupId, true, true, 0, 0, item.SongCode);
        }

        /// <summary>
        /// Runs the cycle update.
        /// </summary>
        internal void RunCycleUpdate()
        {
            _isUpdated = true;
            RunDbUpdate();
        }

        /// <summary>
        /// Runs the database update.
        /// </summary>
        internal void RunDbUpdate()
        {
            try
            {
                if (_mRemovedItems.Count <= 0 && _mAddedItems.Count <= 0 && _inventoryPets.Count <= 0)
                    return;
                var queryChunk = new QueryChunk();
                if (_mAddedItems.Count > 0)
                {
                    foreach (UserItem userItem in _mAddedItems.Values)
                        queryChunk.AddQuery(string.Format("UPDATE items_rooms SET user_id='{0}', room_id='0' WHERE id='{1}'", UserId, userItem.Id));
                    _mAddedItems.Clear();
                }
                if (_mRemovedItems.Count > 0)
                {
                    try
                    {
                        foreach (UserItem userItem2 in _mRemovedItems.Values)
                        {
                            using (var queryReactor = Plus.GetDatabaseManager().GetQueryReactor())
                                GetClient()
                                    .GetHabbo()
                                    .CurrentRoom.GetRoomItemHandler()
                                    .SaveFurniture(queryReactor);
                            if (SongDisks.Contains(userItem2.Id))
                                SongDisks.Remove(userItem2.Id);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    _mRemovedItems.Clear();
                }
                foreach (Pet current in _inventoryPets.Values)
                {
                    if (current.DbState == DatabaseUpdateState.NeedsUpdate)
                    {
                        queryChunk.AddParameter(string.Format("{0}name", current.PetId), current.Name);
                        queryChunk.AddParameter(string.Format("{0}race", current.PetId), current.Race);
                        queryChunk.AddParameter(string.Format("{0}color", current.PetId), current.Color);
                        queryChunk.AddQuery(string.Concat(new object[]
                        {
                            "UPDATE bots SET room_id = ",
                            current.RoomId,
                            ", name = @",
                            current.PetId,
                            "name, x = ",
                            current.X,
                            ", Y = ",
                            current.Y,
                            ", Z = ",
                            current.Z,
                            " WHERE id = ",
                            current.PetId
                        }));
                        queryChunk.AddQuery(string.Concat(new object[]
                        {
                            "UPDATE pets_data SET race = @",
                            current.PetId,
                            "race, color = @",
                            current.PetId,
                            "color, type = ",
                            current.Type,
                            ", experience = ",
                            current.Experience,
                            ", energy = ",
                            current.Energy,
                            ", nutrition = ",
                            current.Nutrition,
                            ", respect = ",
                            current.Respect,
                            ", createstamp = '",
                            current.CreationStamp,
                            "', lasthealth_stamp = ",
                            Plus.DateTimeToUnix(current.LastHealth),
                            ", untilgrown_stamp = ",
                            Plus.DateTimeToUnix(current.UntilGrown),
                            " WHERE id = ",
                            current.PetId
                        }));
                    }
                    current.DbState = DatabaseUpdateState.Updated;
                }
                using (var queryreactor2 = Plus.GetDatabaseManager().GetQueryReactor())
                    queryChunk.Execute(queryreactor2);
            }
            catch (Exception ex)
            {
                Logging.LogCacheError(string.Format("FATAL ERROR DURING USER INVENTORY DB UPDATE: {0}", ex));
            }
        }

        /// <summary>
        /// Serializes the music discs.
        /// </summary>
        /// <returns>ServerMessage.</returns>
        internal ServerMessage SerializeMusicDiscs()
        {
            var serverMessage = new ServerMessage(LibraryParser.OutgoingRequest("SongsLibraryMessageComposer"));
            serverMessage.AppendInteger(SongDisks.Count);
            foreach (var current in
                from x in _floorItems.Values.OfType<UserItem>()
                where x.BaseItem.InteractionType == Interaction.MusicDisc
                select x)
            {
                uint i;
                uint.TryParse(current.ExtraData, out i);
                serverMessage.AppendInteger(current.Id);
                serverMessage.AppendInteger(i);
            }
            return serverMessage;
        }

        /// <summary>
        /// Gets the pets.
        /// </summary>
        /// <returns>List&lt;Pet&gt;.</returns>
        internal List<Pet> GetPets()
        {
            return _inventoryPets.Values.Cast<Pet>().ToList();
        }

        /// <summary>
        /// Sends the floor inventory update.
        /// </summary>
        internal void SendFloorInventoryUpdate()
        {
            _mClient.SendMessage(SerializeFloorItemInventory());
        }

        /// <summary>
        /// Sends the new items.
        /// </summary>
        /// <param name="id">The identifier.</param>
        internal void SendNewItems(uint id)
        {
            var serverMessage = new ServerMessage();
            serverMessage.Init(LibraryParser.OutgoingRequest("NewInventoryObjectMessageComposer"));
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(1);
            serverMessage.AppendInteger(id);
            _mClient.SendMessage(serverMessage);
        }

        /// <summary>
        /// Users the holds item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool UserHoldsItem(uint itemId)
        {
            return SongDisks.Contains(itemId) || _floorItems.Contains(itemId) || _wallItems.Contains(itemId);
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns>GameClient.</returns>
        private GameClient GetClient()
        {
            return Plus.GetGame().GetClientManager().GetClientByUserId(UserId);
        }
    }
}