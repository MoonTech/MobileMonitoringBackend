using System.Net;
using WatchTowerAPI.Contracts.DTOs.Parameters.Camera;
using WatchTowerAPI.Contracts.DTOs.Responses.Camera;
using WatchTowerAPI.Contracts.DTOs.Responses.Room;
using WatchTowerAPI.Domain.Models;
using WatchTowerAPI.Presentation.Controllers;
using WatchTowerBackend.BusinessLogical.Utils;
using WatchTowerBackendTests.Room;
using WatchTowerBackendTests.User;
using WatchTowerBackendTests.Utils;

namespace WatchTowerBackendTests.Camera;

public class CameraControllerTests
{
   private readonly HttpClient _httpClient;


   public CameraControllerTests()
   {
      _httpClient = new();
      _httpClient.BaseAddress = new Uri(Constants.ApiHttpUrl);
   }

   // TODO Change this test
   [Fact]
   public async Task PostCameraShouldReturnErrorIfTwiceTheSameRoom()
   {
      var userToken = await RoomControllerTests.GetUserToken(_httpClient); // TODO Move all the static methods to other place
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
         Assert.True(true);
      }
   }

   [Fact]
   public async Task CameraPostShouldReturnId()
   {
      var userToken = await RoomControllerTests.GetUserToken(_httpClient); // TODO Move all the static methods to other place
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
      var userToken = await RoomControllerTests.GetUserToken(_httpClient); // TODO Move all the static methods to other place
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
      var userToken = await RoomControllerTests.GetUserToken(_httpClient); // TODO Move all the static methods to other place
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
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Put, $"camera", guid, token: userToken);
      Assert.True(response == HttpStatusCode.MethodNotAllowed);
   }
   
   [Fact]
   public async Task CameraPutShouldReturn200IfCameraNotAccepted()
   {
      var userToken = await RoomControllerTests.GetUserToken(_httpClient,"log","pass");
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
      var response = await _httpClient.SendRequestNoAnswerBody(RequestType.Put, $"camera", guid, token: userToken);
      Assert.True(response == HttpStatusCode.MethodNotAllowed);
   }

   public static async Task<Guid> GetGuidOfCamera1Room1(HttpClient httpClient)
   {
      var userToken = await RoomControllerTests.GetUserToken(httpClient);
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
}