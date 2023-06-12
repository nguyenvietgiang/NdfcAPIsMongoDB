namespace NdfcAPIsMongoDB.Models.DTO
{
    public class PlayerDto
    {
        public string sName { get; set; }
        public int iAge { get; set; }
        public string sRole { get; set; }
        public string sPosition { get; set; }
        public IFormFile Image { get; set; } // Thêm trường để đính kèm tệp ảnh
    }
}
