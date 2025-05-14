using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuNhap;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuNhap;
namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhieuNhapRepository : IPhieuNhapRepository
{
    private readonly IMongoCollection<PhieuNhap> _collection;
    private readonly IMongoCollection<NguyenLieu> _collectionNguyenLieu;
    private readonly IMongoCollection<LoaiNguyenLieu> _collectionLoaiNguyenLieu;
    private readonly IMongoCollection<DonViTinh> _collectionDonViTinh;
    private readonly IMongoCollection<TuDo> _collectionTuDo;
    private readonly IMongoCollection<NhaCungCap> _collectionNhaCungCap;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;
    private readonly IMapper _mapper;

    public PhieuNhapRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhieuNhap>("PhieuNhap");
        _collectionNguyenLieu = database.GetCollection<NguyenLieu>("NguyenLieu");
        _collectionLoaiNguyenLieu = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _collectionDonViTinh = database.GetCollection<DonViTinh>("DonViTinh");
        _collectionTuDo = database.GetCollection<TuDo>("TuDo");
        _collectionNhaCungCap = database.GetCollection<NhaCungCap>("NhaCungCap");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");

        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<PhieuNhapRespond>>> GetAllPhieuNhaps(RequestSearchPhieuNhap request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<PhieuNhap>.Filter.Empty;
            filter &= Builders<PhieuNhap>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhieu))
            {
                filter &= Builders<PhieuNhap>.Filter.Regex(x => x.tenPhieu, new BsonRegularExpression($".*{request.tenPhieu}.*"));
            }

            var projection = Builders<PhieuNhap>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenPhieu)
                .Include(x => x.tenNguoiGiao)
                .Include(x => x.nhaCungCap)
                .Include(x => x.dienGiai)
                .Include(x => x.diaDiem)
                .Include(x => x.tongTien)
                .Include(x => x.ghiChu)
                .Include(x => x.nhanVien)
                .Include(x => x.nguyenLieus);

            var findOptions = new FindOptions<PhieuNhap, PhieuNhap>
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
                var PhieuNhaps = await cursor.ToListAsync();
                var nhaCungCapDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();
                var tuDoDict = new Dictionary<string, string>();


                var tuDoIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.tuDo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.loaiNguyenLieu)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var donViTinhIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.donViTinh)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhaCungCapIds = PhieuNhaps.Select(x => x.nhaCungCap).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhanVienIds = PhieuNhaps.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
                var nhaCungCapFilter = Builders<NhaCungCap>.Filter.In(x => x.Id, nhaCungCapIds);
                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var donViTinhProjection = Builders<DonViTinh>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenDonViTinh);
                var tuDoProjection = Builders<TuDo>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenTuDo);
                var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhaCungCap);
                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);

                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

                var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                    .Project<DonViTinh>(donViTinhProjection)
                    .ToListAsync();
                donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                var tuDos = await _collectionTuDo.Find(tuDoFilter)
                    .Project<TuDo>(tuDoProjection)
                    .ToListAsync();
                tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);
                var nhaCungCaps = await _collectionNhaCungCap.Find(nhaCungCapFilter)
                    .Project<NhaCungCap>(nhaCungCapProjection)
                    .ToListAsync();
                nhaCungCapDict = nhaCungCaps.ToDictionary(x => x.Id, x => x.tenNhaCungCap);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);

                var phieuNhapResponds = PhieuNhaps.Select(x => new PhieuNhapRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    tenNguoiGiao = x.tenNguoiGiao,
                    nhaCungCap = new IdName
                    {
                        Id = x.nhaCungCap,
                        Name = nhaCungCapDict.ContainsKey(x.nhaCungCap) ? nhaCungCapDict[x.nhaCungCap] : null
                    },
                    dienGiai = x.dienGiai,
                    diaDiem = x.diaDiem,
                    tongTien = x.tongTien,
                    ghiChu = x.ghiChu,
                    nhanVien = new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    },
                    nguyenLieus = x.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                    {
                        tenNguyenLieu = y.tenNguyenLieu,
                        moTa = y.moTa,
                        soLuong = y.soLuong,
                        hanSuDung = y.hanSuDung,
                        donGia = y.donGia != null ? y.donGia : null,
                        thanhTien = y.thanhTien != null ? y.thanhTien : null,
                        loaiNguyenLieu = new IdName
                        {
                            Id = y.loaiNguyenLieu,
                            Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                        },
                        donViTinh = new IdName
                        {
                            Id = y.donViTinh,
                            Name = donViTinhDict.GetValueOrDefault(y.donViTinh)
                        },
                        tuDo = new IdName
                        {
                            Id = y.tuDo,
                            Name = tuDoDict.GetValueOrDefault(y.tuDo)
                        },
                        trangThai = y.trangThai
                    }).ToList()
                }).ToList();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<PhieuNhapRespond>>
                {
                    Paging = pagingDetail,
                    Data = phieuNhapResponds
                };

                return new RespondAPIPaging<List<PhieuNhapRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var PhieuNhaps = await cursor.ToListAsync();
                var nhaCungCapDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();
                var tuDoDict = new Dictionary<string, string>();


                var tuDoIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.tuDo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.loaiNguyenLieu)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var donViTinhIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.donViTinh)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhaCungCapIds = PhieuNhaps.Select(x => x.nhaCungCap).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhanVienIds = PhieuNhaps.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
                var nhaCungCapFilter = Builders<NhaCungCap>.Filter.In(x => x.Id, nhaCungCapIds);
                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var donViTinhProjection = Builders<DonViTinh>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenDonViTinh);
                var tuDoProjection = Builders<TuDo>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenTuDo);
                var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhaCungCap);
                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);

                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

                var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                    .Project<DonViTinh>(donViTinhProjection)
                    .ToListAsync();
                donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                var tuDos = await _collectionTuDo.Find(tuDoFilter)
                    .Project<TuDo>(tuDoProjection)
                    .ToListAsync();
                tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);
                var nhaCungCaps = await _collectionNhaCungCap.Find(nhaCungCapFilter)
                    .Project<NhaCungCap>(nhaCungCapProjection)
                    .ToListAsync();
                nhaCungCapDict = nhaCungCaps.ToDictionary(x => x.Id, x => x.tenNhaCungCap);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);

                var phieuNhapResponds = PhieuNhaps.Select(x => new PhieuNhapRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    tenNguoiGiao = x.tenNguoiGiao,
                    nhaCungCap = new IdName
                    {
                        Id = x.nhaCungCap,
                        Name = nhaCungCapDict.ContainsKey(x.nhaCungCap) ? nhaCungCapDict[x.nhaCungCap] : null
                    },
                    dienGiai = x.dienGiai,
                    diaDiem = x.diaDiem,
                    tongTien = x.tongTien,
                    ghiChu = x.ghiChu,
                    nhanVien = new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    },
                    nguyenLieus = x.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                    {
                        tenNguyenLieu = y.tenNguyenLieu,
                        moTa = y.moTa,
                        soLuong = y.soLuong,
                        hanSuDung = y.hanSuDung,
                        donGia = y.donGia != null ? y.donGia : null,
                        thanhTien = y.thanhTien != null ? y.thanhTien : null,
                        loaiNguyenLieu = new IdName
                        {
                            Id = y.loaiNguyenLieu,
                            Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                        },
                        donViTinh = new IdName
                        {
                            Id = y.donViTinh,
                            Name = donViTinhDict.GetValueOrDefault(y.donViTinh)
                        },
                        tuDo = new IdName
                        {
                            Id = y.tuDo,
                            Name = tuDoDict.GetValueOrDefault(y.tuDo)
                        },
                        trangThai = y.trangThai
                    }).ToList()
                }).ToList();

                return new RespondAPIPaging<List<PhieuNhapRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhieuNhapRespond>>
                    {
                        Data = phieuNhapResponds,
                        Paging = new PagingDetail(1, phieuNhapResponds.Count, phieuNhapResponds.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhieuNhapRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhieuNhapRespond>> GetPhieuNhapById(string id)
    {
        try
        {

            var phieuNhap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (phieuNhap == null)
            {
                return new RespondAPI<PhieuNhapRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phieu nhập với ID đã cung cấp."
                );
            }

            var phieuNhapResponds = new PhieuNhapRespond
            {
                id = phieuNhap.Id,
                tenPhieu = phieuNhap.tenPhieu,
                tenNguoiGiao = phieuNhap.tenNguoiGiao,
                nhaCungCap = new IdName
                {
                    Id = phieuNhap.nhaCungCap,
                    Name = _collectionNhaCungCap.Find(x => x.Id == phieuNhap.nhaCungCap).FirstOrDefault()?.tenNhaCungCap,
                },
                dienGiai = phieuNhap.dienGiai,
                diaDiem = phieuNhap.diaDiem,
                tongTien = phieuNhap.tongTien,
                ghiChu = phieuNhap.ghiChu,
                nhanVien = new IdName
                {
                    Id = phieuNhap.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == phieuNhap.nhanVien).FirstOrDefault()?.tenNhanVien,
                },
                nguyenLieus = phieuNhap.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                {
                    tenNguyenLieu = y.tenNguyenLieu,
                    moTa = y.moTa,
                    soLuong = y.soLuong,
                    hanSuDung = y.hanSuDung,
                    donGia = y.donGia != null ? y.donGia : null,
                    thanhTien = y.thanhTien != null ? y.thanhTien : null,
                    loaiNguyenLieu = new IdName
                    {
                        Id = y.loaiNguyenLieu,
                        Name = _collectionLoaiNguyenLieu.Find(x => x.Id == y.loaiNguyenLieu).FirstOrDefault()?.tenLoai
                    },
                    donViTinh = new IdName
                    {
                        Id = y.donViTinh,
                        Name = _collectionDonViTinh.Find(x => x.Id == y.donViTinh).FirstOrDefault()?.tenDonViTinh
                    },
                    tuDo = new IdName
                    {
                        Id = y.tuDo,
                        Name = _collectionTuDo.Find(x => x.Id == y.tuDo).FirstOrDefault()?.tenTuDo
                    },
                    trangThai = y.trangThai
                }).ToList()
            };

            return new RespondAPI<PhieuNhapRespond>(
            ResultRespond.Succeeded,
            "Lấy phiếu nhập thành công.",
            phieuNhapResponds
        );


        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhieuNhapRespond>> CreatePhieuNhap(RequestAddPhieuNhap request)
    {
        try
        {
            PhieuNhap newPhieuNhap = _mapper.Map<PhieuNhap>(request);
            newPhieuNhap.createdDate = DateTimeOffset.UtcNow;
            newPhieuNhap.updatedDate = DateTimeOffset.UtcNow;
            newPhieuNhap.isDelete = false;

            // 1. Mapping từ request → entity và set mặc định
            var nguyenLieuEntities = request.nguyenLieus.Select(nl =>
            {
                var entity = new NguyenLieu();
                entity.tenNguyenLieu = nl.tenNguyenLieu;
                entity.moTa = nl.moTa;
                entity.hanSuDung = nl.hanSuDung;
                entity.soLuong = nl.soLuong;
                entity.loaiNguyenLieu = nl.loaiNguyenLieu;
                entity.donViTinh = nl.donViTinh;
                entity.tuDo = nl.tuDo;
                entity.trangThai = nl.trangThai;
                entity.createdDate = DateTimeOffset.UtcNow;
                entity.updatedDate = DateTimeOffset.UtcNow;
                entity.isDelete = false;
                return entity;
            }).ToList();

            // 2. Insert tất cả vào MongoDB
            await _collectionNguyenLieu.InsertManyAsync(nguyenLieuEntities);


            await _collection.InsertOneAsync(newPhieuNhap);

            var nhaCungCapDict = new Dictionary<string, string>();
            var nhanVienDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var nguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();
            var tuDoDict = new Dictionary<string, string>();


            var tuDoIds = nguyenLieuEntities.Select(x => x.tuDo).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var loaiNguyenLieuIds = nguyenLieuEntities.Select(x => x.loaiNguyenLieu).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var donViTinhIds = nguyenLieuEntities.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var nhaCungCapIds = newPhieuNhap.nhaCungCap;
            var nhanVienIds = newPhieuNhap.nhanVien;
            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
            var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var donViTinhProjection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh);
            var tuDoProjection = Builders<TuDo>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenTuDo);
            var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNhaCungCap);
            var nhanVienProjection = Builders<NhanVien>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNhanVien);

            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                .Project<DonViTinh>(donViTinhProjection)
                .ToListAsync();
            donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
            var tuDos = await _collectionTuDo.Find(tuDoFilter)
                .Project<TuDo>(tuDoProjection)
                .ToListAsync();
            tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);

            var phieuNhapResponds = new PhieuNhapRespond
            {
                id = newPhieuNhap.Id,
                tenPhieu = newPhieuNhap.tenPhieu,
                tenNguoiGiao = newPhieuNhap.tenNguoiGiao,
                nhaCungCap = new IdName
                {
                    Id = newPhieuNhap.nhaCungCap,
                    Name = _collectionNhaCungCap.Find(x => x.Id == newPhieuNhap.nhaCungCap).FirstOrDefault()?.tenNhaCungCap,
                },
                dienGiai = newPhieuNhap.dienGiai,
                diaDiem = newPhieuNhap.diaDiem,
                tongTien = newPhieuNhap.tongTien,
                ghiChu = newPhieuNhap.ghiChu,
                nhanVien = new IdName
                {
                    Id = newPhieuNhap.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == newPhieuNhap.nhanVien).FirstOrDefault()?.tenNhanVien,
                },
                nguyenLieus = newPhieuNhap.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                {
                    tenNguyenLieu = y.tenNguyenLieu,
                    moTa = y.moTa,
                    soLuong = y.soLuong,
                    hanSuDung = y.hanSuDung,
                    donGia = y.donGia != null ? y.donGia : null,
                    thanhTien = y.thanhTien != null ? y.thanhTien : null,
                    loaiNguyenLieu = new IdName
                    {
                        Id = y.loaiNguyenLieu,
                        Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                    },
                    donViTinh = new IdName
                    {
                        Id = y.donViTinh,
                        Name = donViTinhDict.GetValueOrDefault(y.donViTinh) ?? ""
                    },
                    tuDo = new IdName
                    {
                        Id = y.tuDo,
                        Name = tuDoDict.GetValueOrDefault(y.tuDo) ?? ""
                    },
                    trangThai = y.trangThai
                }).ToList()
            };

            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Succeeded,
                "Tạo phiếu nhập thành công.",
                phieuNhapResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo phiếu nhập: {ex.Message}"
            );
        }

    }

    // public async Task<RespondAPI<PhieuNhapRespond>> UpdatePhieuNhap(string id, RequestUpdatePhieuNhap request)
    // {


    // }

    public async Task<RespondAPI<string>> DeletePhieuNhap(string id)
    {
        try
        {
            var existingPhieuNhap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingPhieuNhap == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu nhập để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa phiếu nhập không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa phiếu thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa phiếu nhập: {ex.Message}"
            );
        }
    }



}