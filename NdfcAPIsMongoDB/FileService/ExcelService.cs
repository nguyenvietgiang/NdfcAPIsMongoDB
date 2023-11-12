using Syncfusion.XlsIO;
using NdfcAPIsMongoDB.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;
using static NdfcAPIsMongoDB.FileService.ExcelService;
using static System.Net.Mime.MediaTypeNames;

namespace NdfcAPIsMongoDB.FileService
{
    public class ExcelService
    {
        private readonly IMongoDatabase database;
        private readonly string _commonFolderPath;

        public ExcelService(IMongoDatabase database, IWebHostEnvironment env)
        {
            _commonFolderPath = Path.Combine(env.ContentRootPath, "Common");
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
                    worksheet.Range["D1"].Text = "Tình trạng";
                    worksheet.Range["E1"].Text = "Số bàn thắng";
                    foreach (var item in documents)
                    {
                        rowIndex++;
                        worksheet.Range[$"A{rowIndex}"].Text = item.Name;
                        worksheet.Range[$"B{rowIndex}"].Text = item.Role;
                        worksheet.Range[$"C{rowIndex}"].Text = item.Position;
                        worksheet.Range[$"D{rowIndex}"].Text = item.Status;
                        worksheet.Range[$"E{rowIndex}"].Number = item.Scrored;
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
                        worksheet.Range[$"C{rowIndex}"].DateTime = item.Time;
                    }
                }

                else if (collectionType == "Account")
                {
                    var collection = database.GetCollection<Account>("Account");
                    var documents = collection.Find(new BsonDocument()).ToList();
                    int rowIndex = 1;
                    worksheet.Range["A1"].Text = "Tên tài khoản";
                    worksheet.Range["B1"].Text = "Email";
                    worksheet.Range["C1"].Text = "Loại tài khoản";
                    foreach (var item in documents)
                    {
                        rowIndex++;
                        worksheet.Range[$"A{rowIndex}"].Text = item.Username;
                        worksheet.Range[$"B{rowIndex}"].Text = item.Email;
                        worksheet.Range[$"C{rowIndex}"].Text = item.Role;
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
                            Time = worksheet.Range[$"C{rowIndex}"].DateTime
                        };

                        collection.InsertOne(match);

                        rowIndex++;
                        cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    }
                }
                else if (collectionType == "Account")
                {
                    var collection = database.GetCollection<Account>("Account");
                    var rowIndex = 2; // Bắt đầu từ dòng thứ 2 (dòng tiêu đề là dòng đầu tiên)
                    var cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    while (cellValue != null)
                    {
                        var account = new Account
                        {
                            Username = worksheet.Range[$"A{rowIndex}"].Text,
                            Email = worksheet.Range[$"B{rowIndex}"].Text,
                            Password = worksheet.Range[$"D{rowIndex}"].Text,
                            Role = "User"
                        };

                        collection.InsertOne(account);

                        rowIndex++;
                        cellValue = worksheet.Range[$"A{rowIndex}"].Value;
                    }
                }
            }
        }

        public byte[] GetExcelTemplate(string templateName)
        {
            string templateFilePath = Path.Combine(_commonFolderPath, "ExcelTemplate", templateName + ".xlsx");

            using (FileStream fileStream = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    fileStream.CopyTo(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GetWordTemplate(string templateName)
        {
            string templateFilePath = Path.Combine(_commonFolderPath, "WordTemplate", templateName + ".docx");
            using (FileStream fileStream = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream stream = new MemoryStream()) 
                {
                    fileStream.CopyTo(stream);
                    return stream.ToArray();
                }
            }
        }
        
    }
}


