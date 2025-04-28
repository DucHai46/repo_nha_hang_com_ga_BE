using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB
{
    public class MonAn : BaseMongoDb
    {
        public IdName? loaiMonAn { get; set; }
        public IdName? congThuc { get; set; }
        public GiamGiaMonAn? giamGia { get; set; }
        public string? tenMonAn { get; set; }
        public string? hinhAnh { get; set; }
        public string? giaTien { get; set; }
        public string? moTa { get; set; }
    }
}

public class GiamGiaMonAn : IdName
{
    public string? giaTri { get; set; }
}