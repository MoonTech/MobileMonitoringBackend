using System.Net;
using MobileMonitoringBackend.Contracts.DTOs.Parameters.Camera;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Camera;
using MobileMonitoringBackend.Contracts.DTOs.Responses.Room;
using MobileMonitoringBackend.BusinessLogical.Utils;
using MobileMonitoringBackendTests.Room;
using MobileMonitoringBackendTests.Utils;

namespace MobileMonitoringBackendTests.Camera;

public class CameraControllerTests
{
   private readonly HttpClient _httpClient;


   public CameraControllerTests()
   {
      _httpClient = new();
      _httpClient.BaseAddress = new Uri("http://localhost:5000/");
   }

   [Fact]
   public async Task PostCameraShouldReturn400IfTwiceTheSameCamera()
   {
      var userToken = await _httpClient.GetUserToken();
      await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
      await _httpClient.SendRequestNoAnswerBody(RequestType.Post, "camera",
         new PostCameraParameter()
         {
            CameraName = "Camera1",
            RoomName = "Room1",
            Password = ""
         });
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Post, "camera",
         new PostCameraParameter()
         {
            CameraName = "Camera1",
            RoomName = "Room1",
            Password = ""
         });
      Assert.True(response == HttpStatusCode.BadRequest);
   }

   [Fact]
   public async Task CameraPostShouldReturnId()
   {
      var userToken = await _httpClient.GetUserToken();
      await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
      try
      {
         var guid = await GetGuidOfCamera1Room1(_httpClient);
         await _httpClient.SendRequestNoAnswerBody(RequestType.Delete, $"camera/{guid}", token: userToken);
      }
      catch
      {
         
      }
      var response = await _httpClient.SendRequest<PostCameraResponse>(RequestType.Post, "camera",
         new PostCameraParameter()
         {
            CameraName = "Camera1",
            RoomName = "Room1",
            Password = ""
         });
      Assert.True(response.Id != Guid.Empty);
   }

   [Fact]
   public async Task DeleteCameraShouldReturn200()
   {
      var userToken = await _httpClient.GetUserToken();
      await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
      try
      {
         await _httpClient.SendRequest<PostCameraResponse>(RequestType.Post, "camera",
            new PostCameraParameter()
            {
               CameraName = "Camera1",
               RoomName = "Room1",
               Password = ""
            });
      }
      catch
      {
         
      }
      var guid = await GetGuidOfCamera1Room1(_httpClient);
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Delete, $"camera/{guid}", token: userToken);
      Assert.True(response == HttpStatusCode.OK);
   }

   [Fact]
   public async Task CameraPutShouldReturn405IfCameraAccepted()
   {
      var userToken = await _httpClient.GetUserToken();
      await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
      await PopulateDbWithCamera(_httpClient, "Camera1", "Room1", "RoomPassword",userToken);
      try
      {
         await _httpClient.SendRequest<PostCameraResponse>(RequestType.Post, "camera",
            new PostCameraParameter()
            {
               CameraName = "Camera1",
               RoomName = "Room1",
               Password = ""
            });
      }
      catch
      {
         
      }
      var guid = await GetGuidOfCamera1Room1(_httpClient);
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Put, $"camera", guid, token: userToken);
      Assert.True(response == HttpStatusCode.MethodNotAllowed);
   }
   
   [Fact]
   public async Task CameraPutShouldReturn200IfCameraNotAccepted()
   {
      var userToken = await _httpClient.GetUserToken("log","pass");
      await RoomControllerTests.PopulateDbWithRoom(_httpClient, "Room1", "RoomPassword", userToken);
      await PopulateDbWithCamera(_httpClient, "Camera1", "Room1", "RoomPassword", userToken);
      var guid = await GetGuidOfCamera1Room1(_httpClient);
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Put, $"camera", guid, token: userToken);
      Assert.True(response == HttpStatusCode.MethodNotAllowed);
   }

   public static async Task<Guid> GetGuidOfCamera1Room1(HttpClient httpClient)
   {
      var userToken = await httpClient.GetUserToken();
      var listOfRooms = await httpClient.SendRequest<GetAllRoomsResponse>(RequestType.Get, "room", token:userToken);
      var guid = listOfRooms.Rooms
         .Where(r => r.RoomName == "Room1")
         .Select(r => r.Cameras)
         .First()
         .Where(c => c.CameraName=="Camera1")
         .Select(c=>c.Id)
         .FirstOrDefault();
      return guid;
   }

   public static async Task PopulateDbWithCamera(HttpClient httpClient, string cameraName,
      string roomName, string roomPassword, string token)
   {
      try
      {
         var postCameraResponse = await httpClient.SendRequest<PostRoomResponse>(RequestType.Post, "camera",
            new PostCameraParameter()
            {
               CameraName = cameraName,
               RoomName = roomName,
               Password = roomPassword
            }, token);
      }
      catch
      {

      }
   }
}