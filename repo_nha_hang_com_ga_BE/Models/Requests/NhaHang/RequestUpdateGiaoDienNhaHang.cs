using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.NhaHang;

public class RequestUpdateGiaoDienNhaHang
{
    public HeaderNhaHang? header { get; set; }
    public HomeNhaHang? home { get; set; }
    public AboutNhaHang? about { get; set; }
    public FooterNhaHang? footer { get; set; }
}
