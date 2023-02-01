namespace GameClient.network
{
    public enum ReceiveHandler
    {
        Null = -2,
        ClientConnected = 0,
        LoginResult = 1,
        LoginPrevResult = 2,
        PlayerListResult = 3,
        EnterSceneResult = 4,
        LeaveSceneResult = 5,
        WarpSceneResult = 6,
        UserMoveResult = 7,
        UserChatResult = 8,
        GetVideoStatusResult = 9,
        VideoIndexUrlResult = 10,
        UpdateVideoPauseResult = 11,
        UpdateVideoTimeResult = 12,
        UserSitResult = 13,
        UserSitCancelResult = 14,
        UserSwimResult = 15,
        UserEmotionResult = 16,

    }

    public enum SendHandler
    {
        Null = -2,
        ClientClosed = 0,
        LoginRequest = 1,
        LoginPrevRequest = 2,
        EnterSceneRequest = 3,
        LeaveSceneRequest = 4,
        WarpSceneRequest = 5,
        UserMoveRequest = 6,
        UserChatRequest = 7,
        GetVideoStatusRequest = 8,
        VideoIndexUrlRequest = 9,
        UpdateVideoPauseRequest = 10,
        UpdateVideoTimeRequest = 11,
        UserSitRequest = 12,
        UserSitCancelRequest = 13,
        UserSwimRequest = 14,
        UserEmotionRequest = 15,
        Ping = 16,
    }
}