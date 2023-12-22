using WatchTowerAPI.Domain.Models;
using WatchTowerBackend.Contracts.DTOs.ModelsToDTOs;

namespace WatchTowerBackend.BusinessLogical.Converters;

public static class WithoutPasswordConverter
{
    public static ICollection<UserDTO> UserCollectionConverter(ICollection<UserModel> users)
    {
        if (users is null)
        {
            return null;
        }
        ICollection<UserDTO> result = new List<UserDTO>();
        foreach (var user in users)
        {
            result.Add(new()
            {
                Login = user.Login,
                Rooms = RoomCollectionConverter(user.Rooms)
            });
        }
        return result;
    }

    public static ICollection<RoomDTO> RoomCollectionConverter(ICollection<RoomModel> rooms)
    {
        if (rooms is null)
        {
            return null;
        }
        ICollection<RoomDTO> result = new List<RoomDTO>();
        foreach (var room in rooms)
        {
            result.Add(new()
            {
                RoomName = room.RoomName,
                Cameras = CameraCollectionConverter(room.Cameras)
            });
        }
        return result;
    }

    public static ICollection<CameraDTO> CameraCollectionConverter(ICollection<CameraModel> cameras)
    {
        if (cameras is null)
        {
            return null;
        }
        ICollection<CameraDTO> result = new List<CameraDTO>();
        foreach (var camera in cameras)
        {
            result.Add(new()
            {
                Id = camera.Id,
                CameraName = camera.CameraName,
                CameraToken = camera.CameraToken,
                AcceptationState = camera.AcceptationState
            });
        }
        return result;
    }
    
    public static ICollection<RecordingDTO> RecordingCollectionConverter(ICollection<RecordingModel> recordings)
    {
        if (recordings is null)
        {
            return null;
        }
        ICollection<RecordingDTO> result = new List<RecordingDTO>();
        foreach (var recording in recordings)
        {
            result.Add(recording);
        }
        return result;
    }
}