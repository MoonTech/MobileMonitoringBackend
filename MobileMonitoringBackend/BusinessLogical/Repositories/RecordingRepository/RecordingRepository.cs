using Microsoft.EntityFrameworkCore;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Recording;
using MobileMonitoringBackend.BusinessLogical.Utils.Exceptions.Room;
using MobileMonitoringBackend.DataAccess.DbContexts;
using MobileMonitoringBackend.Domain.Models;

namespace MobileMonitoringBackend.BusinessLogical.Repositories.RecordingRepository;

public class RecordingRepository : BaseRepository, IRecordingRepository
{
    public RecordingRepository(MobileMonitoringDbContext context) : base(context) {}

    public RecordingModel AddRecording(string fileName, string fileUrl, RoomModel room)
    {
        try
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
            throw new CouldNotSaveChangesException();
        }
        catch (Exception)
        {
            throw new RecordingAlreadyExistsException(fileName);
        }
    }

    public ICollection<RecordingModel> GetRoomRecordings(RoomModel room)
    {
        try
        {
            return context.Recordings.Where(r => r.RoomName == room.RoomName).ToList();
        }
        catch
        {
            throw new RoomDoesNotExistException(room.RoomName);
        }
    }

    public RecordingModel GetRecording(string fileName)
    {
        var result = context.Recordings
                .Include(recording => recording.Room)
                .SingleOrDefault(r => r.Name == fileName);
        if (result is not null)
        {
            return result;
        }
        throw new RecordingDoesNotExistException(fileName);
    }

    public bool DeleteRecording(RecordingModel recording)
    {
        context.Recordings.Remove(recording);
        if (SaveChanges())
        {
            return true;
        }
        throw new RecordingDoesNotExistException(recording.Name);
    }
}