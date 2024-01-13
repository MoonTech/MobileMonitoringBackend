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
    public const string WrongOwner = "you-are-not-an-owner";
    #endregion

    #region CameraErrorCodes
    public const string CameraAlreadyExists = "camera-already-exists";
    public const string CameraDoesNotExist = "camera-does-not-exist";
    #endregion

    #region OtherErrorCodes
    public const string CouldNotSaveChanges = "could-not-save-changes";
    #endregion

}