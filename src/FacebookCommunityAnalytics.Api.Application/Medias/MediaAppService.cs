using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Medias
{
    [AllowAnonymous]
    [RemoteService(IsEnabled = false)]
    public class MediaAppService :
        BaseCrudApiAppService<
            Media,
            MediaDto,
            Guid, GetMediasInput,
            MediaCreateUpdateDto>,
        IMediaAppService
    {
        private readonly IBlobContainer _blobContainer;
        public MediaAppService(IRepository<Media, Guid> repository) : base(repository)
        {
            
        }

        public override async Task<PagedResultDto<MediaDto>> GetListAsync(GetMediasInput input)
        {
            if (input.FilterText.IsNotNullOrEmpty())
            {
                input.FilterText = input.FilterText.Trim().ToLowerInvariant();
            }

            var queryable = Repository.AsQueryable()
                .WhereIf
                (
                    input.FilterText.IsNotNullOrEmpty(),
                    x => x.FileName.ToLower().Contains(input.FilterText)
                         || x.Tags.Any(t => t.ToLower().Contains(input.FilterText))
                );
            var count = queryable.LongCount();
            var medias = queryable.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            var mediaDtos = ObjectMapper.Map<List<Media>, List<MediaDto>>(medias);

            return new PagedResultDto<MediaDto>()
            {
                TotalCount = count,
                Items = mediaDtos
            };
        }

        // public override async Task<MediaDto> CreateAsync(MediaCreateUpdateDto input)
        // {
        //     if (input.Path.IsNotNullOrEmpty())
        //     {
        //         input.FileName = $"{input.Path}/{input.FileName}";
        //     }
        //     await _blobContainer.SaveAsync(input.FileName, input.MediaFileInfo.FileBytes, overrideExisting: true);
        //
        //     var thumbnailBytes = ImageHelper.ResizeImage(input.MediaFileInfo.FileBytes, 150);
        //     
        //     if (thumbnailBytes.IsNotNullOrEmpty())
        //     {
        //         await _blobContainer.SaveAsync($"thumbs/{input.FileName}", thumbnailBytes, overrideExisting: true);
        //     }
        //     
        //     if (GlobalConfiguration.MediaBaseUrl.IsNotNullOrEmpty())
        //     {
        //         if (GlobalConfiguration.MediaBaseUrl.EndsWith("/"))
        //         {
        //             input.Url = $"{GlobalConfiguration.MediaBaseUrl}{input.FileName}";
        //             input.ThumbnailUrl = $"{GlobalConfiguration.MediaBaseUrl}thumbs/{input.FileName}";
        //         }
        //         else
        //         {
        //             input.Url = $"{GlobalConfiguration.MediaBaseUrl}/{input.FileName}";
        //             input.ThumbnailUrl = $"{GlobalConfiguration.MediaBaseUrl}/thumbs/{input.FileName}";
        //         }
        //     }
        //     
        //     return await base.CreateAsync(input);
        // }

        public override async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            if (File.Exists(Path.Combine(entity.Url)))    
            {
                File.Delete(Path.Combine(entity.Url));
            } 
            
            await base.DeleteAsync(id);
        }

        public async Task<MediaFileInfo> DownloadFileAsync(Guid id)
        {
            var mediaFile = await Repository.FindAsync(id);
            if (mediaFile is null || !File.Exists(mediaFile.Url))
            {
                return null;
            }
            
            var fileData = await File.ReadAllBytesAsync(mediaFile.Url);
            return new MediaFileInfo
            {
                FileBytes = fileData,
                FileName = mediaFile.FileName,
                ContentType = mediaFile.FileContentType
            };
        }

        public async Task<List<MediaDto>> GetMedias()
        {
            var medias = await Repository.GetListAsync();
            return ObjectMapper.Map<List<Media>, List<MediaDto>>(medias);
        }

        public async Task<PagedResultDto<MediaDto>> GetContentMediaAsync(GetMediasInput input)
        {
            var queryable = Repository.AsQueryable()
                    .Where(a => a.MediaEntityType == MediaEntityType.Content)
                    .WhereIf
                    (
                        input.FilterText.IsNotNullOrWhiteSpace(),
                        x => x.FileName.ToLower().Contains(input.FilterText.Trim().ToLower())
                             || x.Tags.Any(t => t.ToLower().Contains(input.FilterText.Trim().ToLower()))
                    )
                    .WhereIf(input.MediaCategory.HasValue, x => x.MediaCategory == input.MediaCategory);
                var count = queryable.LongCount();
                var medias =  queryable.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
                var mediaDtos = ObjectMapper.Map<List<Media>, List<MediaDto>>(medias);
                foreach (var item in mediaDtos)
                {
                    if (File.Exists(item.ThumbnailUrl))
                    {
                        var section = new Rectangle(new Point(0, 0), new Size(130, 130));

                        var croppedImage = CropImage(new Bitmap(item.ThumbnailUrl), section);
                        await using var ms           = new MemoryStream();
                        croppedImage.Save(ms, ImageFormat.Jpeg);
                        item.FileData = ms.ToArray();
                    }
                }
                return new PagedResultDto<MediaDto>()
                {
                    TotalCount = count,
                    Items = mediaDtos
                };
        }

        public async Task<List<KeyValuePair<string, string>>> GetContentMedias(List<string> paths)
        {
            List<KeyValuePair<string, string>> medias = new List<KeyValuePair<string, string>>();
            foreach (var item in paths)
            {
                if (File.Exists(item))
                {
                    var section = new Rectangle(new Point(0, 0), new Size(130, 130));
                    var croppedImage = CropImage(new Bitmap(item), section);
                    await using var ms           = new MemoryStream();
                    croppedImage.Save(ms, ImageFormat.Jpeg);
                    var byteStream = ms.ToArray();
                    var byte64 = $"data:image/gif;base64,{Convert.ToBase64String(byteStream, 0, byteStream.Length)}";
                    medias.Add(new KeyValuePair<string, string>(item,byte64));
                }
            }

            return medias;
        }


        public async Task<MediaDto> UploadThumpNailImage(MediaCreateUpdateDto input)
        {
            var thumbnailBytes = ImageHelper.ResizeImage(input.MediaFileInfo.FileBytes, 150);
            if (thumbnailBytes.IsNotNullOrEmpty())
            {
                var             filePath = Path.Combine(GlobalConfiguration.MediaPath.ThumpPath, input.FileName);
                await using var stream   = File.Create(filePath);
                stream.Write(thumbnailBytes, 0, thumbnailBytes.Length);
                input.ThumbnailUrl = filePath;
            }
            return await base.CreateAsync(input);
        }
        
        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }
    }
}