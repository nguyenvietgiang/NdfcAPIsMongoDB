using Syncfusion.XlsIO;
using NdfcAPIsMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NdfcAPIsMongoDB.FileService
{
    public class ExcelService
    {
        private readonly IMongoDatabase database;

        public ExcelService(IMongoDatabase database)
        {
            this.database = database;
        }

        public byte[] ExportExcel(string collectionType)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Create();
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.Name = collectionType;

                if (collectionType == "Player")
                {
                    var collection = database.GetCollection<Player>("Player");
                    var documents = collection.Find(new BsonDocument()).ToList();
                    int rowIndex = 1;
                    worksheet.Range["A1"].Text = "Họ Tên";
                    worksheet.Range["B1"].Text = "Chức vụ";
                    worksheet.Range["C1"].Text = "Vị trí";
                    foreach (var item in documents)
                    {
                        rowIndex++;
                        worksheet.Range[$"A{rowIndex}"].Text = item.Name;
                        worksheet.Range[$"B{rowIndex}"].Text = item.Role;
                        worksheet.Range[$"C{rowIndex}"].Text = item.Position;
                    }
                }
                else if (collectionType == "Match")
                {
                    var collection = database.GetCollection<Match>("Match");
                    var documents = collection.Find(new BsonDocument()).ToList();
                    int rowIndex = 1;
                    worksheet.Range["A1"].Text = "Đối thủ";
                    worksheet.Range["B1"].Text = "Sân đấu";
                    worksheet.Range["C1"].Text = "Thời gian";
                    foreach (var item in documents)
                    {
                        rowIndex++;
                        worksheet.Range[$"A{rowIndex}"].Text = item.Enemy;
                        worksheet.Range[$"B{rowIndex}"].Text = item.Stadium;
                        worksheet.Range[$"C{rowIndex}"].Text = item.Time;
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public void ImportExcelData(string collectionType, Stream fileStream)
        {
            using (var excelEngine = new ExcelEngine())
            {
                var workbook = excelEngine.Excel.Workbooks.Open(fileStream);
                var worksheet = workbook.Worksheets[0];

                if (collectionType == "Player")
                {
                    var collection = database.GetCollection<Player>("Player");
                    var rowIndex = 2; // Bắt đầu từ dòng thứ 2 (dòng tiêu đề là dòng đầu tiên)
                    var cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    while (cellValue != null)
                    {
                        var player = new Player
                        {
                            Name = worksheet.Range[$"A{rowIndex}"].Text,
                            Role = worksheet.Range[$"B{rowIndex}"].Text,
                            Position = worksheet.Range[$"C{rowIndex}"].Text
                        };

                        collection.InsertOne(player);

                        rowIndex++;
                        cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    }
                }
                else if (collectionType == "Match")
                {
                    var collection = database.GetCollection<Match>("Match");
                    var rowIndex = 2; // Bắt đầu từ dòng thứ 2 (dòng tiêu đề là dòng đầu tiên)
                    var cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    while (cellValue != null)
                    {
                        var match = new Match
                        {
                            Enemy = worksheet.Range[$"A{rowIndex}"].Text,
                            Stadium = worksheet.Range[$"B{rowIndex}"].Text,
                            Time = worksheet.Range[$"C{rowIndex}"].Text
                        };

                        collection.InsertOne(match);

                        rowIndex++;
                        cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    }
                }
            }
        }
    
}
}


