using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public interface ICameraRepository
{
    public CameraModel? CreateCameraWithRoom(RoomModel room);
    public CameraModel GetCameraById(string id);
    public bool AssignNewRoom(string cameraName, RoomModel room);
}