using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Medias
{
    public interface IMediaAppService : IBaseApiCrudAppService<MediaDto,Guid, GetMediasInput, MediaCreateUpdateDto>
    {
        Task<MediaFileInfo>            DownloadFileAsync(Guid id);
        Task<List<MediaDto>>           GetMedias();
        Task<PagedResultDto<MediaDto>> GetContentMediaAsync(GetMediasInput        input);
        Task<List<KeyValuePair<string,string>>> GetContentMedias(List<string> paths);
        Task<MediaDto>                 UploadThumpNailImage(MediaCreateUpdateDto  input);
    }
}