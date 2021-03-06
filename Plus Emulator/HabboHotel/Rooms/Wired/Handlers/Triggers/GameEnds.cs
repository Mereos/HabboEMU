using Plus.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace Plus.HabboHotel.Rooms.Wired.Handlers.Triggers
{
    internal class GameEnds : IWiredItem
    {
        public GameEnds(RoomItem item, Room room)
        {
            this.Item = item;
            Room = room;
        }

        public Interaction Type
        {
            get
            {
                return Interaction.TriggerGameEnd;
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

        public string OtherString
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string OtherExtraString
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public string OtherExtraString2
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public bool OtherBool
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public bool Execute(params object[] stuff)
        {
            List<IWiredItem> conditions = Room.GetWiredHandler().GetConditions(this);
            List<IWiredItem> effects = Room.GetWiredHandler().GetEffects(this);
            if (conditions.Any())
            {
                foreach (IWiredItem current in conditions)
                {
                    if (!current.Execute(null))
                    {
                        return false;
                    }
                    WiredHandler.OnEvent(current);
                }
            }
            if (effects.Any())
            {
                foreach (IWiredItem current2 in effects)
                {
                    foreach (RoomUser current3 in Room.GetRoomUserManager().UserList.Values)
                    {
                        current2.Execute(new object[]
                        {
                            current3,
                            this.Type
                        });
                    }
                    WiredHandler.OnEvent(current2);
                }
            }
            WiredHandler.OnEvent(this);
            return true;
        }
    }
}