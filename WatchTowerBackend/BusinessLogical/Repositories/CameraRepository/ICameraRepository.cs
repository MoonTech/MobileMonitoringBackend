using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.CameraRepository;

public interface ICameraRepository
{
    public CameraModel? CreateCameraWithRoom(RoomModel room);
    public CameraModel? GetCameraById(Guid id);
    public bool DeleteCamera(CameraModel camera);
    public bool AcceptCamera(CameraModel camera);
    public bool RejectCamera(CameraModel camera);
    public bool AssignNewRoom(Guid id, RoomModel room);
}