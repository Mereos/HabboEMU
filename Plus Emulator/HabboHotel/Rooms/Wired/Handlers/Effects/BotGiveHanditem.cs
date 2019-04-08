﻿using Plus.HabboHotel.Items;
using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms.Wired.Handlers.Effects
{
    public class BotGiveHanditem : IWiredItem
    {
        //private List<InteractionType> mBanned;
        public BotGiveHanditem(RoomItem item, Room room)
        {
            this.Item = item;
            Room = room;
            this.OtherString = string.Empty;
            this.OtherExtraString = string.Empty;
            this.OtherExtraString2 = string.Empty;
            //this.mBanned = new List<InteractionType>();
        }

        public Interaction Type
        {
            get
            {
                return Interaction.ActionBotGiveHanditem;
            }
        }

        public RoomItem Item { get; set; }

        public Room Room { get; set; }

        public List<RoomItem> Items
        {
            get
            {
                return new List<RoomItem>();
            }
            set
            {
            }
        }

        public int Delay { get; set; }

        public string OtherString { get; set; }

        public string OtherExtraString { get; set; }

        public string OtherExtraString2 { get; set; }

        public bool OtherBool { get; set; }

        public bool Execute(params object[] stuff)
        {
            RoomUser roomUser = (RoomUser)stuff[0];
            //InteractionType item = (InteractionType)stuff[1];
            int handitem = Delay / 500;
            if (handitem < 0) return false;
            roomUser.CarryItem(handitem);
            RoomUser bot = Room.GetRoomUserManager().GetBotByName(OtherString);
            if (bot == null) return true;
            bot.Chat(null, Plus.GetLanguage().GetVar("bot_give_handitem"), false, 0);
            return true;
        }
    }
}