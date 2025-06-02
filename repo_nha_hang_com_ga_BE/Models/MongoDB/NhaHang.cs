using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class NhaHang : BaseMongoDb
{
    public string? tenNhaHang { get; set; }
    public string? diaChi { get; set; }
    public string? soDienThoai { get; set; }
    public string? email { get; set; }
    public string? website { get; set; }
    public string? logo { get; set; }
    public string? banner { get; set; }
    public string? moTa { get; set; }
    public bool isActive { get; set; }
    public GiaoDienNhaHang? giaoDien { get; set; }
}
public class GiaoDienNhaHang
{
    public HeaderNhaHang? header { get; set; }
    public HomeNhaHang? home { get; set; }
    public AboutNhaHang? about { get; set; }
    public FooterNhaHang? footer { get; set; }
}

public class HeaderNhaHang
{
    public string? logo { get; set; }
    public string? backgroundColor { get; set; }

    public List<string>? ImageSlider { get; set; }
}

public class HomeNhaHang
{
    public string? title { get; set; }
    public string? content { get; set; }
    public string? image { get; set; }

    public List<Content1>? content1 { get; set; }

    public List<Content2>? content2 { get; set; }
}

public class Content1
{
    public string? title { get; set; }
    public string? content { get; set; }
    public string? image { get; set; }
}

public class Content2
{
    public string? title { get; set; }
    public string? content { get; set; }
    public string? image { get; set; }
}

public class AboutNhaHang
{
    public List<ContentAbout>? content { get; set; }
}

public class ContentAbout
{
    public string? title { get; set; }
    public string? content { get; set; }
    public string? image { get; set; }
}


public class FooterNhaHang
{
    public string? title { get; set; }
    public string? content { get; set; }
    public string? logo { get; set; }
    public string? backgroundColor { get; set; }
    public List<string>? address { get; set; }
    public List<string>? phone { get; set; }
    public List<string>? email { get; set; }
    public List<string>? social { get; set; }
}