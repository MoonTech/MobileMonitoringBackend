using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public interface ICameraRepository
{
    public bool CreateCameraWithRoom(string cameraId, RoomModel room);
    public CameraModel GetCameraById(string id);
    public bool AssignNewRoom(string cameraId, RoomModel room);
}