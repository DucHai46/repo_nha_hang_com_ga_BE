using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
namespace repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;

public class RequestSearchKhuyenMai : PagingParameterModel
{
    public string? tenKhuyenMai { get; set; }
    public DateTimeOffset? ngayBatDau { get; set; }
    public DateTimeOffset? ngayKetThuc { get; set; }
    public double? giaTri { get; set; }
    public TrangThaiGiamGia? trangThai { get; set; }
}