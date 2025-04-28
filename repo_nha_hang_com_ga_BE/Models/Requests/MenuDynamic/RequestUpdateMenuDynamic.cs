using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
namespace repo_nha_hang_com_ga_BE.Models.Requests.MenuDynamic;

public class RequestUpdateMenuDynamic
{
    public string? routeLink { get; set; }
    public string? icon { get; set; }
    public string? label { get; set; }
    public string? parent { get; set; }
    public bool? isOpen { get; set; }
    public int? position { get; set; }
    public bool? isActive { get; set; }

}