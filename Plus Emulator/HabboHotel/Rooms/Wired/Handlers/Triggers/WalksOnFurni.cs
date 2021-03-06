using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Rooms.Wired.Handlers.Triggers
{
    internal class WalksOnFurni : IWiredItem, IWiredCycler
    {
        private long _mNext;

        public WalksOnFurni(RoomItem item, Room room)
        {
            Item = item;
            Room = room;
            ToWork = new Queue();
            Items = new List<RoomItem>();
        }

        public Interaction Type
        {
            get { return Interaction.TriggerWalkOnFurni; }
        }

        public RoomItem Item { get; set; }

        public Room Room { get; set; }

        public List<RoomItem> Items { get; set; }

        public int Delay { get; set; }

        public string OtherString
        {
            get { return ""; }
            set { }
        }

        public string OtherExtraString
        {
            get { return ""; }
            set { }
        }

        public string OtherExtraString2
        {
            get { return ""; }
            set { }
        }

        public bool OtherBool
        {
            get { return true; }
            set { }
        }

        public Queue ToWork { get; set; }

        public ConcurrentQueue<RoomUser> ToWorkConcurrentQueue { get; set; }

        public bool Execute(params object[] stuff)
        {
            var roomUser = (RoomUser) stuff[0];
            var roomItem = (RoomItem) stuff[1];
            if (!Items.Contains(roomItem) || (roomUser.LastItem != 0 && roomUser.LastItem == roomItem.Id)) return false;
            ToWork.Enqueue(roomUser);
            if (Delay == 0) OnCycle();
            else
            {
                _mNext = (Plus.Now() + (Delay));

                Room.GetWiredHandler().EnqueueCycle(this);
            }
            return true;
        }

        public bool OnCycle()
        {
            var num = Plus.Now();
            if (num <= _mNext) return false;
            lock (ToWork.SyncRoot)
            {
                while (ToWork.Count > 0)
                {
                    var roomUser = (RoomUser) ToWork.Dequeue();
                    var conditions = Room.GetWiredHandler().GetConditions(this);
                    var effects = Room.GetWiredHandler().GetEffects(this);
                    if (conditions.Any())
                    {
                        foreach (var current in conditions)
                        {
                            if (!current.Execute(roomUser)) return false;
                            WiredHandler.OnEvent(current);
                        }
                    }
                    if (!effects.Any()) continue;
                    foreach (var current2 in effects.Where(current2 => current2.Execute(roomUser, Type))) WiredHandler.OnEvent(current2);
                }
            }
            _mNext = 0L;
            WiredHandler.OnEvent(this);
            return true;
        }
    }
}