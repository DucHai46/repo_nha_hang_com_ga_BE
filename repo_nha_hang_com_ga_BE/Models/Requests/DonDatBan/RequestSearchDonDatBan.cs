using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DonDatBan;
public class RequestSearchDonDatBan : PagingParameterModel
{
    public string? banId { get; set; }
    public string? khachHangName { get; set; }
    public string? khungGio { get; set; }

}