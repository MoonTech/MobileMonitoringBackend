using Microsoft.AspNetCore.Routing.Constraints;
using WatchTowerAPI.DataAccess.DbContexts;
using WatchTowerAPI.Domain.Models;

namespace WatchTowerAPI.BusinessLogical.Repositories.RecordingRepository;

public class RecordingRepository : BaseRepository, IRecordingRepository
{
    public RecordingRepository(WatchTowerDbContext context) : base(context) {}

    public RecordingModel? AddRecording(string fileName, string fileUrl, RoomModel room)
    {
        var recordingEntity = context.Recordings.Add(new RecordingModel()
        {
            Name = fileName,
            Url = fileUrl,
            Room = room
        });
        var recording = recordingEntity.Entity;
        if (SaveChanges())
        {
            return recording;
        }
        return null;
    }

    public ICollection<RecordingModel> GetRoomRecordings(RoomModel room)
    {
        return context.Recordings.Where(r => r.RoomName == room.RoomName).ToList();
    }

    public RecordingModel? GetRecording(string fileName)
    {
        return context.Recordings.SingleOrDefault(r => r.Name == fileName);
    }
}