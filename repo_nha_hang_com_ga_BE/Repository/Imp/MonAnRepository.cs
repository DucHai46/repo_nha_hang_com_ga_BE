using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.MonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.MonAn;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class MonAnRepository : IMonAnRepository
{
    private readonly IMongoCollection<MonAn> _collection;
    private readonly IMapper _mapper;

    public MonAnRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<MonAn>("MonAn");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<MonAnRespond>>> GetAllMonAns(RequestSearchMonAn request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<MonAn>.Filter.Empty;
            filter &= Builders<MonAn>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenMonAn))
            {
                filter &= Builders<MonAn>.Filter.Regex(x => x.tenMonAn, new BsonRegularExpression($".*{request.tenMonAn}.*"));
            }

            if (!string.IsNullOrEmpty(request.tenLoaiMonAn))
            {
                filter &= Builders<MonAn>.Filter.Regex(x => x.loaiMonAn!.Name, new BsonRegularExpression($".*{request.tenLoaiMonAn}.*"));
            }

            if (!string.IsNullOrEmpty(request.tenCongThuc))
            {
                filter &= Builders<MonAn>.Filter.Regex(x => x.congThuc!.Name, new BsonRegularExpression($".*{request.tenCongThuc}.*"));
            }

            var projection = Builders<MonAn>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenMonAn)
                .Include(x => x.loaiMonAn)
                .Include(x => x.congThuc)
                .Include(x => x.giamGia)
                .Include(x => x.moTa)
                .Include(x => x.hinhAnh)
                .Include(x => x.giaTien);

            var findOptions = new FindOptions<MonAn, MonAnRespond>
            {
                Projection = projection
            };

            if (request.IsPaging)
            {
                long totalRecords = await collection.CountDocumentsAsync(filter);

                int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                int currentPage = request.PageNumber;
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages) currentPage = totalPages;

                findOptions.Skip = (currentPage - 1) * request.PageSize;
                findOptions.Limit = request.PageSize;

                var cursor = await collection.FindAsync(filter, findOptions);
                var monAns = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<MonAnRespond>>
                {
                    Paging = pagingDetail,
                    Data = monAns
                };

                return new RespondAPIPaging<List<MonAnRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var monAns = await cursor.ToListAsync();

                return new RespondAPIPaging<List<MonAnRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<MonAnRespond>>
                    {
                        Data = monAns,
                        Paging = new PagingDetail(1, monAns.Count, monAns.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<MonAnRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<MonAnRespond>> GetMonAnById(string id)
    {
        try
        {
            var monAn = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (monAn == null)
            {
                return new RespondAPI<MonAnRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy món ăn với ID đã cung cấp."
                );
            }

            var monAnRespond = _mapper.Map<MonAnRespond>(monAn);

            return new RespondAPI<MonAnRespond>(
                ResultRespond.Succeeded,
                "Lấy món ăn thành công.",
                monAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<MonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<MonAnRespond>> CreateMonAn(RequestAddMonAn request)
    {
        try
        {
            MonAn newMonAn = _mapper.Map<MonAn>(request);

            newMonAn.createdDate = DateTimeOffset.UtcNow;
            newMonAn.updatedDate = DateTimeOffset.UtcNow;
            newMonAn.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newLoaiNguyenLieu.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newMonAn);

            var monAnRespond = _mapper.Map<MonAnRespond>(newMonAn);

            return new RespondAPI<MonAnRespond>(
                ResultRespond.Succeeded,
                "Tạo món ăn thành công.",
                monAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<MonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo món ăn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<MonAnRespond>> UpdateMonAn(string id, RequestUpdateMonAn request)
    {
        try
        {
            var filter = Builders<MonAn>.Filter.Eq(x => x.Id, id);
            filter &= Builders<MonAn>.Filter.Eq(x => x.isDelete, false);
            var monAn = await _collection.Find(filter).FirstOrDefaultAsync();

            if (monAn == null)
            {
                return new RespondAPI<MonAnRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy món ăn với ID đã cung cấp."
                );
            }

            _mapper.Map(request, monAn);

            monAn.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, monAn);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<MonAnRespond>(
                    ResultRespond.Error,
                    "Cập nhật món ăn không thành công."
                );
            }

            var monAnRespond = _mapper.Map<MonAnRespond>(monAn);

            return new RespondAPI<MonAnRespond>(
                ResultRespond.Succeeded,
                "Cập nhật món ăn thành công.",
                monAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<MonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật món ăn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteMonAn(string id)
    {
        try
        {
            var existingMonAn = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingMonAn == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy món ăn để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa món ăn không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa món ăn thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa món ăn: {ex.Message}"
            );
        }
    }
}