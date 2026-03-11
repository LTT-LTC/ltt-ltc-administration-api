using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Controllers.MediaFile
{
    [Route($"{AdministrationServiceSettingNames.DefaultRoute}/media-file")]
    public class MediaFileController(
        IMediaFileAppService mediaFileAppService
        ) : AppControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] List<UploadFileInputDto> lstFile)
                => Success(await mediaFileAppService.UploadFileAsync(lstFile));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileByIdAsync(Guid id)
            => Success(await mediaFileAppService.DeleteFileByIdAsync(id));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileByIdAsync(Guid id)
            => Success(await mediaFileAppService.GetFileByIdAsync(id));
    }
}
