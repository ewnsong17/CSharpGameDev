﻿namespace GameClient.network
{
    public enum ReceiveHandler
    {
        Null = -2,
        ClientConnected = 0,
        ResultPlayerExist = 1,
        ResultGameInit = 2,
    }

    public enum SendHandler
    {
        Null = -2,
        ClientClosed = 0,
        RequestPlayerExist = 1,
    }
}