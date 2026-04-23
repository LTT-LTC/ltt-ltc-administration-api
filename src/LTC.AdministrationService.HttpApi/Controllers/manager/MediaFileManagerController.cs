using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager MediaFile operations: read-only access to files.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/media-files")]
    public class MediaFileManagerController : ManagerControllerBase
    {
        private readonly IMediaFileAppService _mediaFileAppService;

        public MediaFileManagerController(IMediaFileAppService mediaFileAppService)
        {
            _mediaFileAppService = mediaFileAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileByIdAsync(Guid id)
            => Ok(await _mediaFileAppService.GetFileByIdAsync(id));
    }
}
