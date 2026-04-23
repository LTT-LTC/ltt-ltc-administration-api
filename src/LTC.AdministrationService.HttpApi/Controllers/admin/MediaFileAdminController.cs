using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only MediaFile operations: upload and delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/media-files")]
    public class MediaFileAdminController : AdminControllerBase
    {
        private readonly IMediaFileAppService _mediaFileAppService;

        public MediaFileAdminController(IMediaFileAppService mediaFileAppService)
        {
            _mediaFileAppService = mediaFileAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileByIdAsync(Guid id)
            => Ok(await _mediaFileAppService.GetFileByIdAsync(id));

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] List<UploadFileInputDto> lstFile)
            => Ok(await _mediaFileAppService.UploadFileAsync(lstFile));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileByIdAsync(Guid id)
            => Ok(await _mediaFileAppService.DeleteFileByIdAsync(id));
    }
}
