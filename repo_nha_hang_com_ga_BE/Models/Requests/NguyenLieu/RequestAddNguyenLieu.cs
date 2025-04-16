using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

public class RequestAddNguyenLieu
{
    public string? tenNguyenLieu { get; set; }

    public string? moTa { get; set; }

    // public DateTimeOffset? hanSuDung { get; set; }
    public int? soLuong { get; set; }

    public IdName? loaiNguyenLieu { get; set; }

    public IdName? donViTinh { get; set; }

    public IdName? tuDo { get; set; }

    public TrangThaiNguyenLieu? trangThai { get; set; }

}
