using LTC.Shared.CrossCuttingConcerns.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public interface IMediaFileAppService
    {
        Task<List<Guid>> UploadFileAsync(List<UploadFileInputDto> lstFile);
        Task<bool> DeleteFileByIdAsync(Guid id);
        Task<FileOutputDto> GetFileByIdAsync(Guid id);
    }
}
