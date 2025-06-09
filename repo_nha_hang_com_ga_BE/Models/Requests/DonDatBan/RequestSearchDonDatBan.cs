using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DonDatBan;

public class RequestSearchDonDatBan : PagingParameterModel
{
    public string? ban { get; set; }
    public List<string>? khachHang { get; set; }
    public DateTimeOffset? tuNgay { get; set; }
    public DateTimeOffset? denNgay { get; set; }
    public TimeSpan? tuGio { get; set; }
    public TimeSpan? denGio { get; set; }

}