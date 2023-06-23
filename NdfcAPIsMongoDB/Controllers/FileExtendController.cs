using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NdfcAPIsMongoDB.FileService;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using Syncfusion.Drawing;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class FileExtendController : ControllerBase
    {
        private readonly ExcelService excelService;
        private readonly IMongoDatabase database;

        public FileExtendController(IMongoDatabase database, IWebHostEnvironment env)
        {
            this.database = database;
            this.excelService = new ExcelService(database, env);
        }

        [HttpGet("{collectionType}")]
        [Authorize]
        public IActionResult ExportExcel(string collectionType)
        {
            var fileData = excelService.ExportExcel(collectionType);

            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NDFC.xlsx");
        }

        [HttpPost("import-excel/{collectionType}")]
        [Authorize]
        public IActionResult ImportExcel(string collectionType, IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    excelService.ImportExcelData(collectionType, stream);
                }

                return Ok("Đã thêm dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không thêm được dữ liệu: {ex.Message}");
            }
        }

        [HttpGet("dowload-template/{templateName}")]
        public IActionResult GetExcelTemplate(string templateName)
        {
            byte[] templateBytes = excelService.GetExcelTemplate(templateName);
            if (templateBytes == null)
            {
                return NotFound();
            }

            return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", templateName + ".xlsx");
        }

        [HttpGet("pdf-invitation")]
        public IActionResult GenerateInvitation(string name, DateTime time, string reason)
        {
            // Tạo một đối tượng PDF mới
            using (PdfDocument document = new PdfDocument())
            {
                // Tạo một trang mới
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
                float lineSpacing = font.Height * 1.2f;

                string text = "Meeting invitation";
                SizeF textSize = font.MeasureString(text);
                float textX = (page.GetClientSize().Width - textSize.Width) / 2;
                float textY = 80;
                graphics.DrawString(text, font, PdfBrushes.Black, textX, textY);

                float bodyY = page.GetClientSize().Height - lineSpacing * 2;
                graphics.DrawString("Dear: " + name, new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, 50, bodyY);
                bodyY -= lineSpacing;
                graphics.DrawString("At " + time, new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, 50, bodyY);
                bodyY -= lineSpacing;
                graphics.DrawString("To: " + reason, new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, 50, bodyY);

                PdfFont namDinhTodayFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                string namDinhTodayText = "NamDinh / " + DateTime.Today.ToString("yyyy-MM-dd");
                SizeF namDinhTodaySize = namDinhTodayFont.MeasureString(namDinhTodayText);
                float namDinhTodayX = (page.GetClientSize().Width - namDinhTodaySize.Width) / 2;
                bodyY -= lineSpacing;
                graphics.DrawString(namDinhTodayText, namDinhTodayFont, PdfBrushes.Black, namDinhTodayX, bodyY);

                // Lưu file PDF vào một MemoryStream
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;
                // Trả về file PDF như là một phản hồi HTTP
                return File(stream, "application/pdf", "invitation.pdf");
            }
        }
    }
}



