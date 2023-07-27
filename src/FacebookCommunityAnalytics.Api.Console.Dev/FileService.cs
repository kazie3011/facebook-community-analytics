using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Medias;
using IdentityServer4.Extensions;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.IO;
using Media = FacebookCommunityAnalytics.Api.Medias.Media;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class FileService : ITransientDependency
    {
        private GlobalConfiguration GlobalConfiguration { get; set; }
        private IRepository<Media, Guid> MediaRepository { get; set; }
        private IConfiguration _configuration { get; set; }

        public FileService(GlobalConfiguration globalConfiguration, IRepository<Media, Guid> mediaRepository)
        {
            GlobalConfiguration = globalConfiguration;
            MediaRepository = mediaRepository;
        }

        public async Task RemapMediaUrl(string sourceFolder)
        {
            var contractPath = GlobalConfiguration.MediaPath.FileContractPath;
            var files = GetFilesFromDirector(sourceFolder);
            var medias = await MediaRepository.GetListAsync();

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var mediaContents = medias.Where(x => x.FileName.Contains(name)).ToList();

                foreach (var media in mediaContents)
                {
                    if (media.Url.Contains(contractPath)) continue;
                    var mediaFileNames = media.FileName.Split('/');
                    var directory = mediaFileNames.FirstOrDefault()?.Trim();
                    var fileName = mediaFileNames.LastOrDefault()?.Trim();
                    if (directory.IsNullOrEmpty() && fileName.IsNullOrEmpty()) continue;

                    var folder = Path.Combine(contractPath, directory);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    var path = Path.Combine(contractPath, Path.Combine(directory, fileName));
                    media.Url = path;
                    if (File.Exists(path)) continue;
                    System.Console.WriteLine($"Tên File: {name}");
                    System.Console.WriteLine($"Đường dẫn mới: {path}");
                    File.Copy(file, path);
                    await MediaRepository.UpdateAsync(media);
                }
            }
        }

        private List<string> GetFilesFromDirector(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            return files.ToList();
        }

        public async Task ExportMedias(string path)
        {
            var medias = await MediaRepository.GetListAsync
            (
                x => x.Url.Contains("https://gdlblobstorage")
                     && (x.FileContentType.Contains("pdf")
                         || x.FileContentType.Contains("document"))
            );

            var rows = (from item in medias
                let mediaFileNames = item.FileName.Split('/')
                let contractCode = mediaFileNames.FirstOrDefault()?.Trim()
                let fileName = mediaFileNames.LastOrDefault()?.Trim()
                select new MediaRowExport()
                {
                    Url = item.Url,
                    ContractCode = contractCode,
                    FileName = fileName
                }).ToList();

            var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add("medias");
            worksheet.Cells.LoadFromCollection(rows);

            var i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<MediaRowExport>();
            foreach (var header in headers)
            {
                worksheet.Cells[1, i].Value = header;
                worksheet.Column(i).AutoFit();
                i++;
            }

            var data = excel.GetAsByteArray();
            File.WriteAllBytes(path, data);
            excel.Dispose();
        }
    }

    public class MediaRowExport
    {
        public string ContractCode { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}