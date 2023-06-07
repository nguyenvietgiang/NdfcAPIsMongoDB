using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NdfcAPIsMongoDB.FileService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class FileExtendController : ControllerBase
    {
        private readonly ExcelService excelService;
        private readonly IMongoDatabase database;

        public FileExtendController(IMongoDatabase database)
        {
            this.database = database;
            this.excelService = new ExcelService(database);
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
                    var excelService = new ExcelService(database);
                    excelService.ImportExcelData(collectionType, stream);
                }

                return Ok("Đã thêm dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không thêm được dữ liệu: {ex.Message}");
            }
        }


    }
}



