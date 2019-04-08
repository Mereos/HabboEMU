using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
    internal class InteractorAlert : IFurniInteractor
    {
        public void OnPlace(GameClient session, RoomItem item)
        {
            item.ExtraData = "0";
            item.UpdateNeeded = true;
        }

        public void OnRemove(GameClient session, RoomItem item)
        {
            item.ExtraData = "0";
        }

        public void OnTrigger(GameClient session, RoomItem item, int request, bool hasRights)
        {
            if (!hasRights)
            {
                return;
            }
            if (item.ExtraData != "0")
            {
                return;
            }
            item.ExtraData = "1";
            item.UpdateState(false, true);
            item.ReqUpdate(4, true);
        }

        public void OnUserWalk(GameClient session, RoomItem item, RoomUser user)
        {
        }

        public void OnWiredTrigger(RoomItem item)
        {
            if (item.ExtraData != "0")
            {
                return;
            }
            item.ExtraData = "1";
            item.UpdateState(false, true);
            item.ReqUpdate(4, true);
        }
    }
}