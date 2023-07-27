using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Medias;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Medias
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Media")]
    [Route("api/app/library-media")]
    public class MediaController : AbpController, IMediaAppService
    {
        private readonly IMediaAppService _mediaAppService;

        public MediaController(IMediaAppService mediaAppService)
        {
            _mediaAppService = mediaAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<MediaDto> GetAsync(Guid id)
        {
            return _mediaAppService.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<MediaDto>> GetListAsync(GetMediasInput input)
        {
            return _mediaAppService.GetListAsync(input);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("upload-files")]
        public async Task<MediaDto> CreateAsync(MediaEntityType type = MediaEntityType.Unknown)
        {
            var file = Request.Form.Files[0];

            if (file == null || file.Length == 0) { throw new ApiException(MediaConst.NotFileUpload); }

            byte[] fileBytes;
            await using (var stream = file.OpenReadStream()) { fileBytes = await stream.GetAllBytesAsync(); }

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            if (fileName == null) return null;
            
            var fileNameOrigin = fileName.Trim('"');
            var extension = Path.GetExtension(fileNameOrigin);
            var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileNameOrigin}";
            new FileExtensionContentTypeProvider().TryGetContentType(newFileName, out string contentType);
            var input = new MediaCreateUpdateDto { Extension = extension,
                FileName = newFileName,
                FileContentType = contentType,
                MediaEntityType = type,
                MediaFileInfo = new MediaFileInfo
                {
                    FileBytes = fileBytes,
                    Length = file.Length,
                    FileName = newFileName
                }
            };

            return await _mediaAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<MediaDto> UpdateAsync(Guid id, MediaCreateUpdateDto input)
        {
            return _mediaAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _mediaAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("get-file/{id}")]
        public Task<MediaFileInfo> DownloadFileAsync(Guid id)
        {
            return _mediaAppService.DownloadFileAsync(id);
        }

        [HttpGet]
        [Route("get-librabry-medias")]
        public Task<List<MediaDto>> GetMedias()
        {
            return _mediaAppService.GetMedias();
        }

        [HttpGet]
        [Route("get-content-media")]
        public Task<PagedResultDto<MediaDto>> GetContentMediaAsync(GetMediasInput input)
        {
            return _mediaAppService.GetContentMediaAsync(input);
        }

        [HttpGet]
        [Route("get-content-medias")]
        public async Task<List<KeyValuePair<string, string>>> GetContentMedias(List<string> paths)
        {
            return await _mediaAppService.GetContentMedias(paths);
        }
        
        [HttpPost, DisableRequestSizeLimit]
        [Route("upload-thump-nail-image")]
        public async Task<MediaDto> UploadThumpNailImage(MediaCreateUpdateDto input)
        {
            return await _mediaAppService.UploadThumpNailImage(input);
        }

        [HttpGet]
        [Route("loadfile/{id}")]
        public async Task<ActionResult> LoadFile(Guid id)
        {
            var mediaInfo = await _mediaAppService.DownloadFileAsync(id);
            return File(mediaInfo.FileBytes, mediaInfo.ContentType, mediaInfo.FileName);
        }

        [HttpPost, DisableRequestSizeLimit]
        public Task<MediaDto> CreateAsync(MediaCreateUpdateDto input)
        {
            return _mediaAppService.CreateAsync(input);
        }
    }
}