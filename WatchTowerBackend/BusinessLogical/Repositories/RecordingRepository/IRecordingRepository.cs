using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RecordingRepository;

public interface IRecordingRepository
{
    public RecordingModel? AddRecording(string fileName, string fileUrl, RoomModel room);
    public ICollection<RecordingModel> GetRoomRecordings(RoomModel room);
    public RecordingModel? GetRecording(string fileName);
}