﻿using Plus.HabboHotel.Items;
using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms.Wired.Handlers.Effects
{
    public class ResetTimers : IWiredItem
    {
        //private List<InteractionType> mBanned;
        public ResetTimers(RoomItem item, Room room)
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
                return Interaction.ActionResetTimer;
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

        public int Delay
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public string OtherString { get; set; }

        public string OtherExtraString { get; set; }

        public string OtherExtraString2 { get; set; }

        public bool OtherBool { get; set; }

        public bool Execute(params object[] stuff)
        {
            //var roomUser = (RoomUser)stuff[0];
            //var item = (InteractionType)stuff[1];
            foreach (RoomItem aitem in this.Items)
            {
                switch (aitem.GetBaseItem().InteractionType)
                {
                    case Interaction.TriggerRepeater:
                    case Interaction.TriggerTimer:
                        IWiredItem trigger = this.Room.GetWiredHandler().GetWired(aitem);

                        trigger.Delay = 5000;
                        this.Room.GetWiredHandler().ReloadWired(trigger);
                        break;
                }
            }

            return true;
        }
    }
}