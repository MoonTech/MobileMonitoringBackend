using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;

public class RecordingRepository : BaseRepository, IRecordingRepository
{
    public RecordingRepository(MobileMonitoringDbContext context) : base(context) {}

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
        return context.Recordings.Include(recording => recording.Room).SingleOrDefault(r => r.Name == fileName);
    }

    public bool DeleteRecording(RecordingModel recording)
    {
        context.Recordings.Remove(recording);
        return SaveChanges();
    }
}