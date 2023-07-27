using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Medias;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FacebookCommunityAnalytics.Api.Blazor.Controllers.Upload
{
    [ApiController]
    [Route("[controller]")]
    [DisableRequestSizeLimit]
    [IgnoreAntiforgeryToken]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string              _uploadFilesRootPath;
        private readonly IMediaAppService    _mediaAppService;
        private readonly IContractAppService _contractAppService;
        private const string IMAGE = "image";
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public UploadController(IWebHostEnvironment environment,
                                IMediaAppService mediaAppService, 
                                IContractAppService contractAppService)
        {
            this._environment = environment;
            _mediaAppService = mediaAppService;
            _contractAppService = contractAppService;
            _uploadFilesRootPath = $"{environment.WebRootPath}/uploadfiles/";
        }
        
        [HttpPost]
        [Route("UploadFiles")]
        public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> files)
        {
            var result = new List<MediaDto>();
            foreach (var file in files)
            {
                var fileNameOrigin = file.FileName.Trim('"');
                var extension = Path.GetExtension(fileNameOrigin);
                var newFileName = $"{DateTime.Now.Ticks}_{fileNameOrigin}";
                new FileExtensionContentTypeProvider().TryGetContentType(newFileName, out string contentType);
                var uploads  = GlobalConfiguration.MediaPath.FilePath;
                var filePath = Path.Combine(uploads, newFileName);
                await using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(fileStream);
                }
                var input = new MediaCreateUpdateDto { Extension = extension,
                    FileName = newFileName,
                    FileContentType = contentType,
                    MediaEntityType = MediaEntityType.Unknown,
                    MediaFileInfo = new MediaFileInfo
                    {
                        FileBytes = await file.GetAllBytesAsync(),
                        Length = file.Length,
                        FileName = newFileName
                    }
                };
              result.Add(await _mediaAppService.CreateAsync(input)); 
            }
            
            return Json(result);
        }
        
        [HttpPost]
        [Route("UploadContractFiles/{contractId}")]
        public async Task<IActionResult> UploadContractFiles(Guid contractId,IEnumerable<IFormFile> files)
        {
            var contractDto = await _contractAppService.GetAsync(contractId);
            var path = contractDto.ContractCode.ReplaceInvalidChars();
            var result = new List<MediaDto>();
            foreach (var file in files)
            {
                var fileNameOrigin = file.FileName.Trim('"');
                var extension      = Path.GetExtension(fileNameOrigin);
                var newFileName    = $"{DateTime.Now:yyyyMMddHHmmss}_{fileNameOrigin}";
                var fileName       = $"{path}/{newFileName}";
                var uploads        = GlobalConfiguration.MediaPath.FileContractPath;
                var filePath       = Path.Combine(uploads, fileName);
                var directorPath   = Path.Combine(uploads, path);
                if(!Directory.Exists(directorPath))
                    Directory.CreateDirectory(directorPath);
                await using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(fileStream);
                }
                
                new FileExtensionContentTypeProvider().TryGetContentType(newFileName, out string contentType);
                var input = new MediaCreateUpdateDto {
                    Extension       = extension,
                    FileName        = fileName,
                    FileContentType = contentType,
                    Url             = filePath,
                    MediaEntityType = MediaEntityType.Contract,
                    MediaFileInfo = new MediaFileInfo
                    {
                        FileBytes = await file.GetAllBytesAsync(),
                        Length = file.Length,
                        FileName = newFileName
                    }
                };
              result.Add(await _mediaAppService.CreateAsync(input));

              if (input.FileContentType.Contains(IMAGE))
              {
                  input.FileName = newFileName;
                  result.Add(await _mediaAppService.UploadThumpNailImage(input)); 
              }
            }
            
            return Json(result);
        }
        [HttpPost]
        [Route("UploadContentFiles")]
        public async Task<IActionResult> UploadContentFiles(IEnumerable<IFormFile> files)
        {
            var result = new List<MediaDto>();
            foreach (var file in files)
            {
                var fileNameOrigin = file.FileName.Trim('"');
                var extension = Path.GetExtension(fileNameOrigin);
                var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileNameOrigin}";
                new FileExtensionContentTypeProvider().TryGetContentType(newFileName, out string contentType);
                var uploads  = GlobalConfiguration.MediaPath.ContentPath;
                var filePath = Path.Combine(uploads, newFileName);
                
                await using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    await file.CopyToAsync(fileStream);
                }
                var input = new MediaCreateUpdateDto {
                    Extension = extension,
                    FileName = newFileName,
                    FileContentType = contentType,
                    MediaEntityType = MediaEntityType.Content,
                    MediaFileInfo = new MediaFileInfo
                    {
                        FileBytes = await file.GetAllBytesAsync(),
                        Length = file.Length,
                        FileName = newFileName
                    }
                };
              result.Add(await _mediaAppService.UploadThumpNailImage(input)); 
            }
            
            return Json(result);
        }
        
        [HttpPost]
        [Route("UploadAvatarUser")]
        public async Task<IActionResult> UploadAvatarUser(IFormFile file)
        {
            var fileNameOrigin = file.FileName.Trim('"');
            var extension = Path.GetExtension(fileNameOrigin);
            var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileNameOrigin}";
            new FileExtensionContentTypeProvider().TryGetContentType(newFileName, out string contentType);
            var uploads  = GlobalConfiguration.MediaPath.AvatarPath;
            var filePath = Path.Combine(uploads, newFileName);
            var input = new MediaCreateUpdateDto {
                Extension = extension,
                FileName = newFileName,
                FileContentType = contentType,
                MediaEntityType = MediaEntityType.UserInfo,
                MediaFileInfo = new MediaFileInfo
                {
                    FileBytes = await file.GetAllBytesAsync(),
                    Length = file.Length,
                    FileName = newFileName
                }
            };
            return Json(await _mediaAppService.UploadThumpNailImage(input));
        }
    }
}