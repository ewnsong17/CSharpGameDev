namespace GameClient.network
{
    public enum ReceiveHandler
    {
        Null = -2,
        ClientConnected = 0,
        ResultPlayerExist = 1,
        ResultGameInit = 2,
        ResultOtherDisconnect = 3,
        ResultHit = 4,
        ResultOppositeHit = 5,
        ResultStand = 6,
        ResultOppositeStand = 7,
    }

    public enum SendHandler
    {
        Null = -2,
        ClientClosed = 0,
        RequestPlayerExist = 1,
        RequestHit = 2,
        RequestStand = 3,
    }
}