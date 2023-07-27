using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using FacebookCommunityAnalytics.Api.Blazor.Pages;
using FacebookCommunityAnalytics.Api.Blazor.Pages.Contract;
using FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Reports;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Scriban.Syntax;

namespace FacebookCommunityAnalytics.Api.Blazor.Helpers
{
    public static class ExportHelper
    {
        private const string NumberFormat = "#,#0";

        public static byte[] GeneratePostsExcelBytes<T>(IStringLocalizer L, List<T> rows, string sheetName = "Sheet1") where T : class
        {
            // Creating an instance
            // of ExcelPackage
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(rows, true);

            int i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<T>();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{typeof(T).Name}:{header}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            // Link
            workSheet.Column(3).AutoFit(0, 80);
            // Like Comment Share Total
            workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // IsNotAvailable
            workSheet.Column(12).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(12).AutoFit(16, 20);
            workSheet.Column(13).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(13).AutoFit(16, 20);
            workSheet.Column(14).Style.Numberformat.Format = GlobalConsts.DateTimeFormat;
            workSheet.Column(14).AutoFit(16, 20);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateTransactionExcelBytes(IStringLocalizer L, List<TransactionExportRow> rows,TransactionStatsRow stats, string sheetName = "Sheet1")
        {
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right ;
            workSheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right ;
            workSheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right ;
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right ;

            workSheet.Cells.LoadFromCollection(rows, true);
            
            int i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<TransactionExportRow>();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{typeof(TransactionExportRow).Name}:{header}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            int row = rows.Count() + 2;
            workSheet.Cells[row,4].Value = stats.TotalTransactionVATAmount;
            workSheet.Cells[row, 4].Style.Font.Color.SetColor(Color.Red);
            
            workSheet.Cells[row,5].Value = stats.TotalTransactionAmount;
            workSheet.Cells[row, 5].Style.Font.Color.SetColor(Color.Red);

            var data = excel.GetAsByteArray();
            excel.Dispose();
            return data;
        }
    

    public static byte[] GenerateTiktokExcelBytes<T>(IStringLocalizer L, List<T> rows, string sheetName = "Sheet1") where T : class
        {
            // Creating an instance
            // of ExcelPackage
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Column(7).Style.Numberformat.Format = GlobalConsts.DateFormat;
            workSheet.Cells.LoadFromCollection(rows, true);

            int i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<T>();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{typeof(T).Name}:{header}"];
                workSheet.Column(i).AutoFit();
                i++;
            }
            

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateStaffEvalExcelBytes(IStringLocalizer L, List<StaffEvaluationExportRow> rows, string sheetName = "Sheet1", bool isSaleTeam = false)
        {
            var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add(sheetName);
            worksheet.DefaultRowHeight = 12;
            worksheet.Cells.LoadFromCollection(rows, true).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var headers = ObjectHelper.GetPropDescsOrNames<StaffEvaluationExportRow>();
            if (!isSaleTeam)
            {
                headers = headers.Where(x => x != nameof(StaffEvaluationExportRow.SaleKPIAmount)).ToList();
            }

            var i = 1;
            foreach (var header in headers)
            {
                worksheet.Cells[1, i].Value = L[$"{nameof(StaffEvaluationExportRow)}:{header}"];
                worksheet.Column(i).AutoFit();
                i++;
            }

            worksheet.Column(5).Style.Font.Color.SetColor(Color.Red);
            worksheet.Column(5).Style.Font.Bold = true;
            worksheet.Column(6).Style.Font.Color.SetColor(Color.Coral);
            worksheet.Column(6).Style.Font.Bold = true;
            worksheet.Column(7).Style.Font.Color.SetColor(Color.Green);
            worksheet.Column(7).Style.Font.Bold = true;
            worksheet.Column(8).Style.Font.Color.SetColor(Color.CornflowerBlue);
            worksheet.Column(8).Style.Font.Bold = true;
            worksheet.Column(9).Style.WrapText = true;
            worksheet.Column(11).Style.Font.Color.SetColor(Color.Coral);

            if (isSaleTeam) worksheet.Column(12).Style.Font.Color.SetColor(Color.Green);

            worksheet.Row(1).Height = 20;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Font.Color.SetColor(Color.Black);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateTiktokStaffEvalExcelBytes<T>(IStringLocalizer L, List<T> rows, string sheetName = "Sheet1") where T : class
        {
            // Creating an instance
            // of ExcelPackage
            var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add(sheetName);
            worksheet.DefaultRowHeight = 12;
            worksheet.Cells.LoadFromCollection(rows, true).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            var i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<T>();
            foreach (var header in headers)
            {
                worksheet.Cells[1, i].Value = L[$"{typeof(T).Name}:{header}"];
                worksheet.Column(i).AutoFit();
                i++;
            }

            worksheet.Column(6).Style.Font.Color.SetColor(Color.Red);
            worksheet.Column(6).Style.Font.Bold = true;
            worksheet.Column(7).Style.Font.Color.SetColor(Color.Coral);
            worksheet.Column(7).Style.Font.Bold = true;
            worksheet.Column(8).Style.Font.Color.SetColor(Color.Green);
            worksheet.Column(8).Style.Font.Bold = true;
            worksheet.Column(9).Style.Font.Color.SetColor(Color.CornflowerBlue);
            worksheet.Column(9).Style.Font.Bold = true;
            worksheet.Column(10).Style.WrapText = true;
            worksheet.Column(12).Style.Font.Color.SetColor(Color.Coral);
            worksheet.Column(13).Style.Font.Color.SetColor(Color.Green);

            worksheet.Row(1).Height = 20;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Font.Color.SetColor(Color.Black);

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateAccountExcelBytes<T>(IStringLocalizer L, List<T> rows, string sheetName = "Sheet1") where T : class
        {
            // Creating an instance
            // of ExcelPackage
            var excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells.LoadFromCollection(rows, true);

            int i = 1;
            var headers = ObjectHelper.GetPropDescsOrNames<T>();
            foreach (var header in headers)
            {
                workSheet.Cells[1, i].Value = L[$"{typeof(T).Name}:{header}"];
                workSheet.Column(i).AutoFit();
                i++;
            }

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateTiktokWeeklyReportExcelBytesMCN(IStringLocalizer L, List<TiktokWeeklyReportModelMCN> posts, string sheetName = "Sheet1")
        {
            if (posts.IsNullOrEmpty()) return null;
            var excel = new ExcelPackage();
            sheetName = "Tiktok Weekly Report MCN";
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(2).Style.Font.Bold = true;

            //Header Excel
            //Format Chanel Name Header
            workSheet.Cells[1, 1, 2, 1].Merge = true;
            workSheet.Cells[1, 1].Value = L["TiktokReports.Index"];
            workSheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Column(1).Width = 5;

            workSheet.Cells[1, 2, 2, 2].Merge = true;
            workSheet.Cells[1, 2].Value = L["Chanel Name"];
            workSheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(2).Width = 50;


            workSheet.Cells[1, 3, 2, 3].Merge = true;
            workSheet.Cells[1, 3].Value = L["TiktokReports.TiktokCategoryType"];

            workSheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

            var weeklyHeaderIndex = 4;
            var propertyHeaderIndex = 4;
            foreach (var item in posts.GroupBy(_ => _.TimeTitle))
            {
                //Format Weekly Header
                workSheet.Cells[1, weeklyHeaderIndex, 1, weeklyHeaderIndex + 2].Merge = true;
                workSheet.Cells[1, weeklyHeaderIndex].Value = item.Key;
                workSheet.Cells[1, weeklyHeaderIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, weeklyHeaderIndex].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["Follower"]);
                propertyHeaderIndex++;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["View"]);
                propertyHeaderIndex++;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["Video"]);
                propertyHeaderIndex++;

                workSheet.Column(weeklyHeaderIndex).AutoFit();
                weeklyHeaderIndex += 3;
            }

            //Body Excel

            var categoryTyperow = 3;
            var categoryTypeColumn = 3;

            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[categoryTyperow, categoryTypeColumn].Value = items.FirstOrDefault().TiktokCategoryType.ToString();
                categoryTyperow++;
                workSheet.Column(categoryTypeColumn).AutoFit();
            }

            var row = 3;
            var column = 2;
            int index = 1;

            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[row, column - 1].Value = index;
                workSheet.Cells[row, column - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row, column - 1].Style.Fill.BackgroundColor.SetColor(Color.Gold);

                workSheet.Cells[row, column].Value = items.Key;
                workSheet.Column(column).AutoFit();

                foreach (var item in items)
                {
                    column++;
                    workSheet = FormatPropertyBodyWorksheet
                    (
                        workSheet,
                        row,
                        column + 1,
                        item.Followers,
                        item.IncreasedFollowers
                    );
                    column++;
                    workSheet = FormatPropertyBodyWorksheet(workSheet, row, column + 1, item.Views);
                    column++;
                    workSheet = FormatPropertyBodyWorksheet(workSheet, row, column + 1, item.Videos);
                }

                row++;
                column = 2;
                index++;
            }

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateTiktokWeeklyReportExcelBytesBoD(IStringLocalizer L, List<TiktokWeeklyReportModelBoD> posts, string sheetName = "Sheet1")
        {
            if (posts.IsNullOrEmpty()) return null;
            var excel = new ExcelPackage();
            sheetName = "Tiktok Weekly Report - BoD";
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(2).Style.Font.Bold = true;

            //Header Excel
            //Format Chanel Name Header
            workSheet.Cells[1, 1, 2, 1].Merge = true;
            workSheet.Cells[1, 1].Value = L["TiktokReports.Index"];
            workSheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Column(1).Width = 5;

            workSheet.Cells[1, 2, 2, 2].Merge = true;
            workSheet.Cells[1, 2].Value = L["Chanel Name"];
            workSheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(2).Width = 40;

            workSheet.Cells[1, 3, 2, 3].Merge = true;
            workSheet.Cells[1, 3].Value = L["TiktokReports.TiktokCategoryType"];
            workSheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

            var weeklyHeaderIndex = 4;
            var propertyHeaderIndex = 4;
            foreach (var item in posts.GroupBy(_ => _.TimeTitle))
            {
                //Format Weekly Header
                workSheet.Cells[1, weeklyHeaderIndex].Value = item.Key;
                workSheet.Cells[1, weeklyHeaderIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, weeklyHeaderIndex].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                workSheet.Column(weeklyHeaderIndex).Width = 40;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, $"{L["Follower"]}: {StringHelper.FormatNumberInExcel(item.Sum(_ => _.Followers))}",Color.Red);
                propertyHeaderIndex++;
                weeklyHeaderIndex++;
            }

            //Body Excel
            var categoryTypeRow = 3;
            var categoryTypeColumn = 3;

            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[categoryTypeRow, categoryTypeColumn].Value = items.FirstOrDefault()?.TiktokCategoryType;
                categoryTypeRow++;
                workSheet.Column(categoryTypeColumn).AutoFit();
            }

            var row = 3;
            var column = 2;
            int index = 1;
            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[row, column - 1].Value = index;
                workSheet.Cells[row, column - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row, column - 1].Style.Fill.BackgroundColor.SetColor(Color.Gold);

                workSheet.Cells[row, column].Value = items.Key;
                column += 2;
                foreach (var item in items)
                {
                    workSheet = FormatPropertyBodyWorksheet
                    (
                        workSheet,
                        row,
                        column,
                        item.Followers,
                        item.IncreasedFollowers
                    );
                    column++;
                }

                row++;
                column = 2;
                index++;
            }

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        public static byte[] GenerateTiktokMonthlyReportExcelBytes(IStringLocalizer L, List<TiktokMonthlyReportModel> posts, string sheetName = "Sheet1")
        {
            if (posts.IsNullOrEmpty()) return null;
            var excel = new ExcelPackage();
            sheetName = "Tiktok Monthly Report";
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(2).Style.Font.Bold = true;

            //Header Excel
            //Format Chanel Name Header
            workSheet.Cells[1, 1, 2, 1].Merge = true;
            workSheet.Cells[1, 1].Value = L["TiktokReports.Index"];
            workSheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Column(1).Width = 5;


            workSheet.Cells[1, 2, 2, 2].Merge = true;
            workSheet.Cells[1, 2].Value = L["Chanel Name"];
            workSheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            workSheet.Column(2).Width = 40;

            workSheet.Cells[1, 3, 2, 3].Merge = true;
            workSheet.Cells[1, 3].Value = L["TiktokReports.TiktokCategoryType"];
            workSheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

            var categoryTyperow = 3;
            var categoryTypeColumn = 3;

            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[categoryTyperow, categoryTypeColumn].Value = items.FirstOrDefault().TiktokCategoryType.ToString();
                categoryTyperow++;
                workSheet.Column(categoryTypeColumn).AutoFit();
            }

            var monthlyHeaderIndex = 4;
            var propertyHeaderIndex = 4;

            foreach (var item in posts.GroupBy(_ => _.TimeTitle))
            {
                //Format Weekly Header
                workSheet.Cells[1, monthlyHeaderIndex, 1, monthlyHeaderIndex + 3].Merge = true;
                workSheet.Cells[1, monthlyHeaderIndex].Value = item.Key;
                workSheet.Cells[1, monthlyHeaderIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, monthlyHeaderIndex].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["Follower"]);
                propertyHeaderIndex++;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["View"]);
                propertyHeaderIndex++;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["Video"]);
                propertyHeaderIndex++;

                workSheet = FormatPropertyHeaderWorksheet(workSheet, propertyHeaderIndex, L["Average"]);
                propertyHeaderIndex++;

                workSheet.Column(monthlyHeaderIndex).AutoFit();
                monthlyHeaderIndex += 4;
            }

            //Body Excel
            var row = 3;
            var column = 2;
            int index = 1;
            foreach (var items in posts.GroupBy(_ => _.ChannelName))
            {
                workSheet.Cells[row, column - 1].Value = index;
                workSheet.Cells[row, column - 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[row, column - 1].Style.Fill.BackgroundColor.SetColor(Color.Gold);

                workSheet.Cells[row, column].Value = items.Key;
                workSheet.Column(column).AutoFit();
                foreach (var item in items)
                {
                    column++;
                    workSheet = FormatPropertyBodyWorksheet
                    (
                        workSheet,
                        row,
                        column + 1,
                        item.Followers,
                        item.IncreasedFollowers
                    );
                    column++;
                    workSheet = FormatPropertyBodyWorksheet(workSheet, row, column + 1, item.Views);
                    column++;
                    workSheet = FormatPropertyBodyWorksheet(workSheet, row, column + 1, item.Videos);
                    column++;
                    workSheet = FormatPropertyBodyWorksheet(workSheet, row, column + 1, item.Average);
                }

                row++;
                column = 2;
                index++;
            }

            var data = excel.GetAsByteArray();
            excel.Dispose();

            return data;
        }

        private static ExcelWorksheet FormatPropertyHeaderWorksheet(ExcelWorksheet worksheet, int index, string name,Color color = default)
        {
            worksheet.Cells[2, index].Value = name;
            worksheet.Cells[2, index].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, index].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
            worksheet.Cells[2, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[2, index].Style.Font.Color.SetColor(color);
            

            return worksheet;
        }

        private static ExcelWorksheet FormatPropertyBodyWorksheet(ExcelWorksheet worksheet, int row, int column, double value)
        {
            worksheet.Cells[row, column].Value = value;
            worksheet = NumberFormatWorksheet(worksheet, row, column);
            return worksheet;
        }

        private static ExcelWorksheet FormatPropertyBodyWorksheet(
            ExcelWorksheet worksheet,
            int row,
            int column,
            int value,
            bool? isIncreased)
        {
            if (isIncreased.HasValue) worksheet.Cells[row, column].Style.Font.Color.SetColor(StringHelper.FormatGrowthColorBla(isIncreased));

            worksheet.Cells[row, column].Value = value;
            worksheet = NumberFormatWorksheet(worksheet, row, column);
            return worksheet;
        }

        private static ExcelWorksheet NumberFormatWorksheet(ExcelWorksheet worksheet, int row, int column)
        {
            worksheet.Cells[row, column].Style.Numberformat.Format = NumberFormat;
            worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Column(column).AutoFit();
            return worksheet;
        }
    }
}