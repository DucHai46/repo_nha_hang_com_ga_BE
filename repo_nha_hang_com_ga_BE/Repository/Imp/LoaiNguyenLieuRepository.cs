using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiNguyenLieu;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class LoaiNguyenLieuRepository : ILoaiNguyenLieuRepository
{
    private readonly IMongoCollection<LoaiNguyenLieu> _collection;
    private readonly IMapper _mapper;

    public LoaiNguyenLieuRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<LoaiNguyenLieuRespond>>> GetAllLoaiNguyenLieus(RequestSearchLoaiNguyenLieu request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<LoaiNguyenLieu>.Filter.Empty;
            filter &= Builders<LoaiNguyenLieu>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.danhMucNguyenLieuId))
            {
                filter &= Builders<LoaiNguyenLieu>.Filter.Eq(x => x.danhMucNguyenLieu.Id, request.danhMucNguyenLieuId);
            }

            if (!string.IsNullOrEmpty(request.tenLoai))
            {
                filter &= Builders<LoaiNguyenLieu>.Filter.Regex(x => x.tenLoai, new BsonRegularExpression($".*{request.tenLoai}.*"));
            }

            var projection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai)
                .Include(x => x.danhMucNguyenLieu.Id)
                .Include(x => x.danhMucNguyenLieu.Name)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<LoaiNguyenLieu, LoaiNguyenLieuRespond>
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
                var loaiNguyenLieus = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<LoaiNguyenLieuRespond>>
                {
                    Paging = pagingDetail,
                    Data = loaiNguyenLieus
                };

                return new RespondAPIPaging<List<LoaiNguyenLieuRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var loaiNguyenLieus = await cursor.ToListAsync();

                return new RespondAPIPaging<List<LoaiNguyenLieuRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<LoaiNguyenLieuRespond>>
                    {
                        Data = loaiNguyenLieus,
                        Paging = new PagingDetail(1, loaiNguyenLieus.Count, loaiNguyenLieus.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<LoaiNguyenLieuRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<LoaiNguyenLieuRespond>> GetLoaiNguyenLieuById(string id)
    {
        try
        {
            var loaiNguyenLieu = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (loaiNguyenLieu == null)
            {
                return new RespondAPI<LoaiNguyenLieuRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại nguyên liệu với ID đã cung cấp."
                );
            }

            var loaiNguyenLieuRespond = _mapper.Map<LoaiNguyenLieuRespond>(loaiNguyenLieu);

            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Lấy loại nguyên liệu thành công.",
                loaiNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiNguyenLieuRespond>> CreateLoaiNguyenLieu(RequestAddLoaiNguyenLieu request)
    {
        try
        {
            LoaiNguyenLieu newLoaiNguyenLieu = _mapper.Map<LoaiNguyenLieu>(request);

            newLoaiNguyenLieu.createdDate = DateTimeOffset.UtcNow;
            newLoaiNguyenLieu.updatedDate = DateTimeOffset.UtcNow;
            newLoaiNguyenLieu.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newLoaiNguyenLieu.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newLoaiNguyenLieu);

            var loaiNguyenLieuRespond = _mapper.Map<LoaiNguyenLieuRespond>(newLoaiNguyenLieu);

            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Tạo loại nguyên liệu thành công.",
                loaiNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo loại nguyên liệu: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiNguyenLieuRespond>> UpdateLoaiNguyenLieu(string id, RequestUpdateLoaiNguyenLieu request)
    {
        try
        {
            var filter = Builders<LoaiNguyenLieu>.Filter.Eq(x => x.Id, id);
            filter &= Builders<LoaiNguyenLieu>.Filter.Eq(x => x.isDelete, false);
            var loaiNguyenLieu = await _collection.Find(filter).FirstOrDefaultAsync();

            if (loaiNguyenLieu == null)
            {
                return new RespondAPI<LoaiNguyenLieuRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại nguyên liệu với ID đã cung cấp."
                );
            }

            _mapper.Map(request, loaiNguyenLieu);

            loaiNguyenLieu.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, loaiNguyenLieu);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<LoaiNguyenLieuRespond>(
                    ResultRespond.Error,
                    "Cập nhật loại nguyên liệu không thành công."
                );
            }

            var loaiNguyenLieuRespond = _mapper.Map<LoaiNguyenLieuRespond>(loaiNguyenLieu);

            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Cập nhật loại nguyên liệu thành công.",
                loaiNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật loại nguyên liệu: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteLoaiNguyenLieu(string id)
    {
        try
        {
            var existingLoaiNguyenLieu = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingLoaiNguyenLieu == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại nguyên liệu để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa loại nguyên liệu không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa loại nguyên liệu thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa loại nguyên liệu: {ex.Message}"
            );
        }
    }
}