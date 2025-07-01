namespace FinalProject.BL
{
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Drive.v3.Data;
    using Google.Apis.Services;
    using Google.Apis.Upload;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public static class GoogleDriveHelper
    {
        public static DriveService GetDriveService()
        {
            GoogleCredential credential;
            var path = "C:\\Users\\zichr\\Source\\Repos\\The-Secretary\\the-secretary-464517-129fb2c748d0.json";
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.Scope.DriveFile);
            }

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "The-Secretary"
            });
        }

        public static async Task<Google.Apis.Drive.v3.Data.File> UploadFileAsync(DriveService service, IFormFile file, string spaceName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"{spaceName}_{file.FileName}",
                Parents = new List<string> { "1mJl2lcksbPfXu82i5Rp4HPqaz97p20-O" }  // ID של התיקייה ב-Drive
            };

            using (var stream = file.OpenReadStream())
            {
                var request = service.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink";
                var result = await request.UploadAsync();

                if (result.Status == UploadStatus.Completed)
                {
                    return request.ResponseBody;
                }
                else
                {
                    return null;
                }
            }
        }
    }

}
