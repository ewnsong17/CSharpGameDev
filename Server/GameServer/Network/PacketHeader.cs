using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Network
{
    public enum ReceiveHandler
    {
        Null = -2,
        ClientClosed = 0,
        RequestPlayerExist = 1,
    }

    public enum SendHandler
    {
        Null = -2,
        ClientConnected = 0,
        ResultPlayerExist = 1,
    }
}
