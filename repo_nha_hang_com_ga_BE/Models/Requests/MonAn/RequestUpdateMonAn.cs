using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Requests.MonAn;

public class RequestUpdateMonAn
{
    public string? tenMonAn { get; set; }

    public IdName? loaiMonAn { get; set; }

    public IdName? congThuc { get; set; }

    public GiamGiaMonAn? giamGia { get; set; }

    public string? moTa { get; set; }

    public string? hinhAnh { get; set; }

    public string? giaTien { get; set; }
}