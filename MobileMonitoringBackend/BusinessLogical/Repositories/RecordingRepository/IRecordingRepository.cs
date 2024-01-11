using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;

public interface IRecordingRepository
{
    public RecordingModel? AddRecording(string fileName, string fileUrl, RoomModel room);
    public ICollection<RecordingModel> GetRoomRecordings(RoomModel room);
    public RecordingModel? GetRecording(string fileName);
    public bool DeleteRecording(RecordingModel recording);
}