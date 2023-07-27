using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace FacebookCommunityAnalytics.Api.Exports
{
    public interface IExportDomainService
    {
        byte[] GeneratePostExcelBytes(List<PostWithNavigationProperties> posts, string sheetName);
        byte[] GeneratePostDetailExcelBytes(List<PostWithNavigationProperties> posts, string sheetName);
        byte[] GenerateUserAffiliateExcelBytes(List<UserAffiliateWithNavigationProperties> affiliates, string sheetName);
        byte[] GenerateCampaignDetailExcelBytes(GetCampaignDetailExportRequest request, string sheetName);
        byte[] GenerateTiktokExcelBytes(List<TiktokExportRow> posts, string sheetName);
        byte[] GenerateUserCompensationsExcelBytes(List<UserCompensationExportRow> userCompensations);
        byte[] GenerateAffiliateEvaluationExcelBytes(List<PostWithNavigationProperties> posts, List<CompensationAffiliateExport> exportAffiliates, string sheetName = "Sheet1");
    }
    
    public class ExportDomainService : BaseDomainService, IExportDomainService
    {
        private const int Limit = 10000;
        public byte[] GeneratePostExcelBytes(List<PostWithNavigationProperties> posts, string sheetName = "Sheet1")
        {
            // Creating an instance
            // of ExcelPackage
            if (posts.IsNullOrEmpty()) return null;
            
            posts = posts.Take(Limit).ToList();
            var excel = new ExcelPackage();
            var exportPosts = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostExportRow>>(posts);
            var postContentTypeGroups = exportPosts.GroupBy(_ => _.PostContentType);
            foreach (var item in postContentTypeGroups)
            {
                sheetName = L[$"Export.CampaignPost:{item.Key}"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                PostSheetFormat(workSheet, item);
            }
            
            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }
        
        public byte[] GeneratePostDetailExcelBytes(List<PostWithNavigationProperties> posts, string sheetName = "Sheet1")
        {
            // Creating an instance
            // of ExcelPackage
            if (posts.IsNullOrEmpty()) return null;
            posts = posts.Take(Limit).ToList();
            var excel = new ExcelPackage();
            var exportPosts = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostDetailExportRow>>(posts);
            foreach (var post in exportPosts)
            {
                // post.Type = L[$"Enum:PostType:{(int)post.Type.ToEnumerable<PostType>()}"];
                // post.PostCopyrightType = L[$"Enum:PostCopyrightType:{(int)post.PostCopyrightType.ToEnumerable<PostCopyrightType>()}"];
                post.IsNotAvailable = post.IsNotAvailable == PostConsts.True ? L["Post.IsNotAvailable"] : L["Post.IsAvailable"];
            }
            var postContentTypeGroups = exportPosts.GroupBy(_ => _.PostContentType);
            foreach (var item in postContentTypeGroups)
            {
                sheetName = L[$"Export.CampaignPost:{item.Key}"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                PostDetailSheetFormat(workSheet, item);
            }
            
            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }
        
        public byte[] GenerateUserAffiliateExcelBytes(List<UserAffiliateWithNavigationProperties> affiliates, string sheetName = "Sheet1")
        {
            // Creating an instance
            // of ExcelPackage
            if (affiliates.IsNullOrEmpty()) return null;
            affiliates = affiliates.Take(Limit).ToList();
            var excel = new ExcelPackage();
            var exportAffiliates = ObjectMapper.Map<List<UserAffiliateWithNavigationProperties>, List<UserAffiliateExportRow>>(affiliates);
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            AffiliateSheetFormat(workSheet, exportAffiliates);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }
        
        public byte[] GenerateCampaignDetailExcelBytes(GetCampaignDetailExportRequest request, string sheetName = "Sheet1")
        {
            // Creating an instance
            // of ExcelPackage
            if (request.Posts.IsNullOrEmpty()) return null;
            var exportPosts = request.Posts.Take(Limit).ToList();
            var excel = new ExcelPackage();
            var postContentTypeGroups = exportPosts.GroupBy(_ => _.PostContentType);
            foreach (var item in postContentTypeGroups.OrderByDescending(_ => _.Key))
            {
                sheetName = L[$"Export.CampaignPost:{item.Key}"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                PostSheetFormat(workSheet, item);
            }

            if (request.Affiliates.IsNotNullOrEmpty())
            {
                sheetName = "Short Link";
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                AffiliateSheetFormat(workSheet, request.Affiliates);
            }
            
            if (request.TikToks.IsNotNullOrEmpty())
            {
                sheetName = "TikTok";
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                TikTokSheetFormat(workSheet, request.TikToks);
            }
            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public byte[] GenerateTiktokExcelBytes(List<TiktokExportRow> tiktoks, string sheetName)
        {
            if (tiktoks.Count <= 0) return null;
            tiktoks = tiktoks.Select
                (
                    _ =>
                    {
                        _.Index = tiktoks.FindIndex(t => t == _) + 1;
                        return _;
                    }
                )
                .ToList();
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(tiktoks, true);

            int i = 1;
            var headers = typeof(TiktokExportRow).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            workSheet.Column(5).Style.Numberformat.Format = GlobalConsts.DateFormat;
            workSheet.Column(5).AutoFit(16, 20);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public byte[] GenerateUserCompensationsExcelBytes(List<UserCompensationExportRow> userCompensations)
        {
            if (userCompensations.Count <= 0) return null;
            var excel = new ExcelPackage();
            foreach (var group in userCompensations.GroupBy(x=>x.Team))
            {
                var list = group.ToList().OrderBy(x=>x.Username);
                
                var workSheet = excel.Workbook.Worksheets.Add("Team: " + group.Key);
                workSheet.DefaultRowHeight = 12;
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                workSheet.Cells.LoadFromCollection(list, true);

                int i = 1;
                var headers = typeof(UserCompensationExportRow).GetProperties();
                foreach (var header in headers)
                {
                    workSheet.Cells[1, i].Value = L[$"{nameof(UserCompensationExportRow)}:{header.Name}"];
                    workSheet.Column(i).AutoFit();
                    i++;
                }

                workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                workSheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }
            
            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        private void AffiliateSheetFormat(ExcelWorksheet workSheet, List<UserAffiliateExportRow> exportAffiliates)
        {
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(exportAffiliates, true);

            int i = 1;
            var headers = typeof(UserAffiliateExportRow).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{nameof(UserAffiliateExportRow)}:{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            // Link
            // var t = 2;
            // foreach (var affiliate in exportAffiliates)
            // {
            //     using (var Rng = workSheet.Cells[t, 4])
            //     {
            //         var siteLink = affiliate.Url;
            //         var disTxt = affiliate.Url;
            //         Rng.Formula = "=HYPERLINK(\"" + siteLink + "\", \"" + disTxt + "\")";
            //         Rng.Style.Font.UnderLine = true;
            //         Rng.Style.Font.Color.SetColor(Color.Blue);
            //     }
            //
            //     using (var Rng = workSheet.Cells[t, 5])
            //     {
            //         var siteLink = affiliate.AffiliateUrl;
            //         var disTxt = affiliate.AffiliateUrl;
            //         Rng.Formula = "=HYPERLINK(\"" + siteLink + "\", \"" + disTxt + "\")";
            //         Rng.Style.Font.UnderLine = true;
            //         Rng.Style.Font.Color.SetColor(Color.Blue);
            //     }
            //
            //     t++;
            // }

            workSheet.Column(4).AutoFit(0, 80);
            workSheet.Column(5).AutoFit(0, 30);
            // Conversion
            workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            // Date 
            workSheet.Column(10).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(10).AutoFit(16, 20);
        }

        private void PostSheetFormat(ExcelWorksheet workSheet, IGrouping<string, PostExportRow> item)
        {
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(item, true);

            int i = 1;
            var headers = typeof(PostExportRow).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{nameof(PostExportRow)}:{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            // Link
            // var t = 2;
            // foreach (var post in item)
            // {
            //     using (var Rng = workSheet.Cells[t, 2])
            //     {
            //         var siteLink = post.Url;
            //         var disTxt = post.Url;
            //         Rng.Formula = "=HYPERLINK(\"" + siteLink + "\", \"" + disTxt + "\")";
            //         Rng.Style.Font.UnderLine = true;
            //         Rng.Style.Font.Color.SetColor(Color.Blue);
            //     }
            //
            //     t++;
            // }

            workSheet.Column(3).AutoFit(0, 80);
            // Like Comment Share Total
            workSheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }
        
        private void TikTokSheetFormat(ExcelWorksheet workSheet, List<TiktokExportRow> items)
        {
            items = items.Select
                (
                    _ =>
                    {
                        _.Index = items.FindIndex(t => t == _) + 1;
                        return _;
                    }
                )
                .ToList();
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(items, true);

            var i = 1;
            var headers = typeof(TiktokExportRow).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            workSheet.Column(5).Style.Numberformat.Format = GlobalConsts.DateFormat;
            workSheet.Column(5).AutoFit(16, 20);
        }
        
        private void PostDetailSheetFormat(ExcelWorksheet workSheet, IGrouping<string, PostDetailExportRow> item)
        {
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(item, true);

            int i = 1;
            var headers = typeof(PostDetailExportRow).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{nameof(PostDetailExportRow)}:{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            // Link
            workSheet.Column(3).AutoFit(0, 80);
            // Like Comment Share Total
            workSheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // IsNotAvailable

            workSheet.Column(17).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(17).AutoFit(16, 20);
            workSheet.Column(18).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(18).AutoFit(16, 20);
            workSheet.Column(19).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(19).AutoFit(16, 20);
        }
        
        public byte[] GenerateAffiliateEvaluationExcelBytes(List<PostWithNavigationProperties> posts, List<CompensationAffiliateExport> exportAffiliates, string sheetName = "Sheet1")
        {
            // Creating an instance
            // of ExcelPackage
            if (posts.IsNullOrEmpty()) return null;
            posts = posts.Take(Limit).ToList();
            var excel = new ExcelPackage();
            var exportPosts = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostExportRow>>(posts);
            var postContentTypeGroups = exportPosts.GroupBy(_ => _.PostContentType);
            foreach (var item in postContentTypeGroups.OrderByDescending(_ => _.Key))
            {
                sheetName = L[$"Export.CampaignPost:{item.Key}"];
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                PostSheetFormat(workSheet, item);
            }

            if (exportAffiliates.IsNotNullOrEmpty())
            {
                sheetName = "Short Link";
                var workSheet = excel.Workbook.Worksheets.Add(sheetName);
                AffiliateEvaluationSheetFormat(workSheet, exportAffiliates);
            }
            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        private void AffiliateEvaluationSheetFormat(ExcelWorksheet workSheet, List<CompensationAffiliateExport> exportAffiliates)
        {
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(exportAffiliates, true);

            var i = 1;
            var headers = typeof(CompensationAffiliateExport).GetProperties();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{nameof(CompensationAffiliateExport)}:{header.Name}"];
                workSheet.Column(i).AutoFit();
                i++;
            }
        }
    }
}