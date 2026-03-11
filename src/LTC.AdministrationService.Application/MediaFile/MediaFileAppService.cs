using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LTC.AdministrationService.Entities;
using LTC.Shared.CrossCuttingConcerns.Enums;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace LTC.AdministrationService
{
    public class MediaFileAppService : AdministrationServiceAppService, IMediaFileAppService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICurrentTenant _currentTenant;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<MediaFile, Guid> _fileRepository;

        public MediaFileAppService(
            Cloudinary cloudinary,
            ICurrentTenant currentTenant,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MediaFile, Guid> fileRepository
            )
        {
            _cloudinary = cloudinary;
            _currentTenant = currentTenant;
            _unitOfWorkManager = unitOfWorkManager;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// validate tổng kích thước của tất cả file được upload không vượt quá 100MB
        /// </summary>
        /// <param name="lstFile"></param>
        /// <exception cref="UserFriendlyException"></exception>
        public bool IsOutOfSize(List<IFormFile> lstFile)
        {
            const long maxTotalSize = MediaFileConsts.MaxFileSize;
            var totalSize = lstFile.Sum(f => f.Length);
            return totalSize > maxTotalSize;
        }

        /// <summary>
        /// Tải file lên Cloudinary
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<Guid>> UploadFileAsync(List<UploadFileInputDto> lstFile)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var isOutOfSize = IsOutOfSize(lstFile.Select(x => x.File).ToList());
                if (isOutOfSize)
                    throw new UserFriendlyException(L["OutOfSize", "100MB"]);

                var lstMediaFile = new List<MediaFile>();
                var deletedFileIds = new List<DeleteFileInputDto>();
                try
                {
                    foreach (var item in lstFile)
                    {
                        var fileType = item.FileType;
                        var file = item.File;

                        if (file == null || file.Length == 0)
                            throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["InvalidValue"], L["File"]));

                        var newFileId = Guid.CreateVersion7();
                        var tenantFolder = _currentTenant.Id.HasValue ? $"{MediaFileConsts.ParentFolder}/{_currentTenant.Id.Value}" : $"{MediaFileConsts.ParentFolder}/{Guid.Empty}";
                        var publicId = $"{_currentTenant.Id ?? Guid.Empty}_{newFileId.ToString()}";
                        var tags = $"tenant-{_currentTenant.Id ?? Guid.Empty},file-{newFileId.ToString()}";
                        var fileName = file.FileName;

                        // upload to Cloudinary
                        await using var stream = file.OpenReadStream();
                        var fileDescription = new FileDescription(fileName, stream);

                        var uploadResult = new FileUploadedOutputDto();
                        uploadResult.StatusCode = HttpStatusCode.BadRequest;

                        if (fileType == FileTypeEnum.Image)
                            uploadResult = await UploadImageAsync(fileDescription, publicId, tenantFolder, tags);
                        else if (fileType == FileTypeEnum.Video)
                            uploadResult = await UploadVideoAsync(fileDescription, publicId, tenantFolder, tags);
                        else if (fileType == FileTypeEnum.Document)
                            uploadResult = await UploadDocumentAsync(fileDescription, publicId, tenantFolder, tags);

                        if (uploadResult.StatusCode != HttpStatusCode.OK)
                            throw new UserFriendlyException(L["UploadFailed"]);

                        MediaFile mediaFile = new MediaFile(newFileId)
                        {
                            DisplayName = uploadResult.DisplayName,
                            ResourceType = uploadResult.ResourceType,
                            Type = uploadResult.Type,
                            Format = uploadResult.Format,
                            AssetId = uploadResult.AssetId,
                            PublicId = uploadResult.PublicId,
                            SecureUrl = uploadResult.SecureUrl,
                            Size = uploadResult.Size,
                        };
                        DeleteFileInputDto deleteFileInput = new DeleteFileInputDto
                        {
                            PublicId = uploadResult.PublicId,
                            ResourceType = uploadResult.ResourceType
                        };
                        lstMediaFile.Add(mediaFile);
                        deletedFileIds.Add(deleteFileInput);
                    }

                    // cập nhật DB khi tất cả file đã được upload thành công
                    await _fileRepository.InsertManyAsync(lstMediaFile);
                    await uow.CompleteAsync();
                    return lstMediaFile.Select(x => x.Id).ToList();
                }
                catch
                {
                    // nếu có lỗi xảy ra khi upload file, xóa tất cả file đã upload thành công trước đó để tránh rác trên Cloudinary
                    await DeleteFileFromCloudinaryAsync(deletedFileIds);
                    throw new UserFriendlyException(L["UploadFailed"]);
                }
            }
        }

        /// <summary>
        /// Tải file ảnh
        /// </summary>
        /// <param name="fileDescription"></param>
        /// <param name="publicId"></param>
        /// <param name="folder"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task<FileUploadedOutputDto> UploadImageAsync(
            FileDescription fileDescription,
            string publicId,
            string folder,
            string tags
            )
        {
            var uploadParams = new ImageUploadParams()
            {
                File = fileDescription,
                Folder = folder,
                PublicId = publicId,
                Overwrite = true,
                Tags = tags,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new FileUploadedOutputDto
            {
                ResourceType = uploadResult.ResourceType,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                DisplayName = uploadResult.OriginalFilename,
                Type = uploadResult.Type,
                AssetId = uploadResult.AssetId,
                Format = uploadResult.Format,
                StatusCode = uploadResult.StatusCode,
                Size = uploadResult.Bytes
            };
        }

        /// <summary>
        /// Tải file video
        /// </summary>
        /// <param name="fileDescription"></param>
        /// <param name="publicId"></param>
        /// <param name="folder"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task<FileUploadedOutputDto> UploadVideoAsync(
            FileDescription fileDescription,
            string publicId,
            string folder,
            string tags
            )
        {
            var uploadParams = new VideoUploadParams()
            {
                File = fileDescription,
                Folder = folder,
                PublicId = publicId,
                Overwrite = true,
                Tags = tags,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new FileUploadedOutputDto
            {
                ResourceType = uploadResult.ResourceType,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                DisplayName = uploadResult.OriginalFilename,
                Type = uploadResult.Type,
                AssetId = uploadResult.AssetId,
                Format = uploadResult.Format,
                StatusCode = uploadResult.StatusCode,
                Size = uploadResult.Bytes
            };
        }

        /// <summary>
        /// Tải file tài liệu
        /// </summary>
        /// <param name="fileDescription"></param>
        /// <param name="publicId"></param>
        /// <param name="folder"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task<FileUploadedOutputDto> UploadDocumentAsync(
            FileDescription fileDescription,
            string publicId,
            string folder,
            string tags
            )
        {
            var uploadParams = new RawUploadParams()
            {
                File = fileDescription,
                Folder = folder,
                PublicId = publicId,
                Overwrite = true,
                Tags = tags,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new FileUploadedOutputDto
            {
                ResourceType = uploadResult.ResourceType,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                DisplayName = uploadResult.OriginalFilename,
                Type = uploadResult.Type,
                AssetId = uploadResult.AssetId,
                Format = uploadResult.Format,
                StatusCode = uploadResult.StatusCode,
                Size = uploadResult.Bytes
            };
        }

        /// <summary>
        /// Xóa danh sách file trên Cloudinary
        /// </summary>
        /// <param name="lstFileId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task<bool> DeleteFileFromCloudinaryAsync(List<DeleteFileInputDto> input)
        {
            foreach (var item in input)
            {
                var deletionParams = new DeletionParams(item.PublicId)
                {
                    ResourceType = ConvertStringToResourceType(item.ResourceType)
                };
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                if (deletionResult.Result != "ok")
                    throw new UserFriendlyException(L["DeletedFailed"]);
            }
            return true;
        }

        /// <summary>
        /// Xóa file theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> DeleteFileByIdAsync(Guid id)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var mediaFile = await _fileRepository.GetAsync(id)
                    ?? throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["NotFound"], L["File"]));

                // Soft delete không cần xóa ở thời điểm hiện tại (khi data phình to sẽ cân nhắc xóa file đã có IsDeleted = 1 trên Cloudinary)
                //var deleteFileInput = new DeleteFileInputDto
                //{
                //    PublicId = mediaFile.PublicId,
                //    ResourceType = mediaFile.ResourceType
                //};
                //var isDeletedFromCloudinary = await DeleteFileFromCloudinaryAsync(new List<DeleteFileInputDto> { deleteFileInput });
                //if (!isDeletedFromCloudinary)
                //    throw new UserFriendlyException(L["DeletedFailed"]);

                await _fileRepository.DeleteAsync(mediaFile);
                await uow.CompleteAsync();
                return true;
            }
        }

        /// <summary>
        /// Chi tiết file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<FileOutputDto> GetFileByIdAsync(Guid id)
        {
            var mediaFileQueryable = await _fileRepository.GetQueryableAsync();
            var mediaFile = await mediaFileQueryable
                .Where(x => x.Id == id)
                .Select(x => new FileOutputDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Format = x.Format,
                    ResourceType = x.ResourceType,
                    SecureUrl = x.SecureUrl,
                    Size = x.Size
                }).FirstOrDefaultAsync() ?? throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["NotFound"], L["File"]));

            return mediaFile;
        }

        /// <summary>
        /// convert string sang Resource Type của Cloudinary
        /// </summary>
        /// <param name="resourceTypeString"></param>
        /// <returns></returns>
        private ResourceType ConvertStringToResourceType(string resourceTypeString)
        {
            return resourceTypeString.ToLower() switch
            {
                "image" => ResourceType.Image,
                "video" => ResourceType.Video,
                "raw" => ResourceType.Raw,
                _ => ResourceType.Auto,
            };
        }
    }
}
