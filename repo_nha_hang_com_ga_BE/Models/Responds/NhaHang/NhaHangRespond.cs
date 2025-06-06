﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Responds.NhaHang;

public class NhaHangRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

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