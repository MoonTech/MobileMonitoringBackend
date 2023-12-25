using WatchTowerBackend.Domain.Models;

namespace WatchTowerBackend.BusinessLogical.Repositories.CameraRepository;

public interface ICameraRepository
{
    public CameraModel? CreateCameraWithRoom(string cameraName, RoomModel room);
    public CameraModel? GetCameraById(Guid id);
    public bool DeleteCamera(CameraModel camera);
    public bool AcceptCamera(CameraModel camera);
}