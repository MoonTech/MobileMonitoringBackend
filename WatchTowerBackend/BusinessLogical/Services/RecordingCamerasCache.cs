namespace WatchTowerBackend.BusinessLogical.Services;

public class RecordingCamerasCache
{
    private readonly ICollection<Guid> _recordingCameras;

    public RecordingCamerasCache()
    {
        _recordingCameras = new HashSet<Guid>();
    }

    public void Add(Guid id)
    {
        _recordingCameras.Add(id);
    }

    public void Remove(Guid id)
    {
        _recordingCameras.Remove(id);
    }

    public bool Contains(Guid id)
    {
        return _recordingCameras.Contains(id);
    }
}