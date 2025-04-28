using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class MenuDynamic : BaseMongoDb
{
        public string? RouteLink { get; set; }
        public string? Icon { get; set; }
        public string? Label { get; set; }
        public string? Parent { get; set; }
        public bool? IsOpen { get; set; } = false;
        public int? Position { get; set; }
        // public List<MenuDynamic>? Children { get; set; }
        public bool? IsActive { get; set; } = true;
}