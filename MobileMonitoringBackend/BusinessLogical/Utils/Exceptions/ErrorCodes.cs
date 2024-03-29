namespace MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;

public static class ErrorCodes
{
    #region UserErrorCodes
    public const string UserAlreadyExists = "user-already-exists";
    public const string UserPasswordWrong = "user-password-wrong";
    public const string UserDoesNotExist = "user-does-not-exist";
    #endregion

    #region RoomErrorCodes
    public const string RoomAlreadyExists = "room-already-exists";
    public const string RoomPasswordWrong = "room-password-wrong";
    public const string RoomDoesNotExist = "room-does-not-exist";
    public const string RoomWrongOwner = "you-are-not-an-owner";
    public const string RoomAccessDenied = "access-to-room-denied";
    public const string RoomAccessRefresh = "different-access-refresh-token";
    #endregion

    #region CameraErrorCodes
    public const string CameraAlreadyExists = "camera-already-exists";
    public const string CameraDoesNotExist = "camera-does-not-exist";
    public const string CameraAlreadyAccepted = "camera-already-accepted";
    #endregion

    #region RecordingErrorCodes
    public const string RecordingAlreadyExists = "recording-already-exists";
    public const string RecordingDoesNotExist = "recording-does-not-exist";
    public const string RecordingCamerasCache = "recording-cameras-cache-exception";
    #endregion

    #region VideoServerErrorCodes
    public const string VideoServerCommunication = "video-server-comminication-exception";
    #endregion

    #region OtherErrorCodes
    public const string CouldNotSaveChanges = "could-not-save-changes";
    public const string InvalidRefreshToken = "invalid-refresh-token";
    #endregion

}