using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Repository;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _repo;
        private readonly ApplicationDbContext _context; // For Transaction

        public FacilityService(IFacilityRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        private FacilityResponse MapToResponse(Facility f)
        {
            return new FacilityResponse
            {
                Id = f.Id,
                OwnerId = f.OwnerId,
                Name = f.Name,
                Address = f.Address,
                Latitude = f.Location?.Y,
                Longitude = f.Location?.X,
                Status = f.Status,
                Images = f.Images.Select(i => i.ImageUrl).ToList(),
                Amenities = f.FacilityAmenities.Select(fa => new AmenityResponse { Id = fa.AmenityId, Name = fa.Amenity?.Name ?? "", IconCode = fa.Amenity?.IconCode }).ToList(),
                Courts = f.Courts.Select(c => new CourtResponse { Id = c.Id, FacilityId = c.FacilityId, CategoryId = c.CategoryId, CategoryName = c.Category?.Name ?? "", Name = c.Name, Status = c.Status, DefaultPrice = c.DefaultPrice }).ToList()
            };
        }

        public async Task<ApiResponse<IEnumerable<FacilityResponse>>> SearchFacilitiesAsync(double? lat, double? lng, double? radius, Guid? categoryId, Guid? amenityId, string? status)
        {
            IEnumerable<Facility> facilities;
            if (lat.HasValue && lng.HasValue && radius.HasValue)
            {
                facilities = await _repo.GetFacilitiesNearMeAsync(lat.Value, lng.Value, radius.Value);
                // Also apply other filters manually
                if (categoryId.HasValue) facilities = facilities.Where(f => f.Courts.Any(c => c.CategoryId == categoryId.Value));
                if (amenityId.HasValue) facilities = facilities.Where(f => f.FacilityAmenities.Any(fa => fa.AmenityId == amenityId.Value));
                if (!string.IsNullOrEmpty(status)) facilities = facilities.Where(f => f.Status == status);
            }
            else
            {
                facilities = await _repo.SearchFacilitiesAsync(categoryId, amenityId, status);
            }

            return new ApiResponse<IEnumerable<FacilityResponse>> { StatusCode = 200, Message = "Success", Data = facilities.Select(MapToResponse) };
        }

        public async Task<ApiResponse<FacilityResponse?>> GetFacilityDetailsAsync(Guid id)
        {
            var f = await _repo.GetFacilityDetailsAsync(id);
            if (f == null) return new ApiResponse<FacilityResponse?> { StatusCode = 404, Message = "Facility not found", Data = null };
            return new ApiResponse<FacilityResponse?> { StatusCode = 200, Message = "Success", Data = MapToResponse(f) };
        }

        public async Task<ApiResponse<FacilityResponse?>> CreateFacilityAsync(Guid ownerId, CreateFacilityRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var facility = new Facility
                {
                    Id = Guid.NewGuid(),
                    OwnerId = ownerId,
                    Name = request.Name,
                    Address = request.Address,
                    Location = request.Latitude.HasValue && request.Longitude.HasValue ? new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 } : null
                };

                _context.Facilities.Add(facility);

                foreach (var img in request.ImageUrls)
                {
                    _context.FacilityImages.Add(new FacilityImage { Id = Guid.NewGuid(), FacilityId = facility.Id, ImageUrl = img });
                }

                foreach (var aid in request.AmenityIds)
                {
                    _context.FacilityAmenities.Add(new FacilityAmenity { FacilityId = facility.Id, AmenityId = aid });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetFacilityDetailsAsync(facility.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<FacilityResponse?> { StatusCode = 500, Message = ex.Message, Data = null };
            }
        }

        public async Task<ApiResponse<FacilityResponse?>> UpdateFacilityAsync(Guid facilityId, Guid ownerId, UpdateFacilityRequest request)
        {
            var facility = await _repo.GetFacilityDetailsAsync(facilityId);
            if (facility == null) return new ApiResponse<FacilityResponse?> { StatusCode = 404, Message = "Not found", Data = null };
            if (facility.OwnerId != ownerId) return new ApiResponse<FacilityResponse?> { StatusCode = 403, Message = "Forbidden", Data = null };

            if (!string.IsNullOrEmpty(request.Name)) facility.Name = request.Name;
            if (request.Address != null) facility.Address = request.Address;
            if (request.Status != null) facility.Status = request.Status;
            if (request.Latitude.HasValue && request.Longitude.HasValue) facility.Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };

            if (request.ImageUrls != null)
            {
                _context.FacilityImages.RemoveRange(facility.Images);
                foreach (var img in request.ImageUrls)
                {
                    _context.FacilityImages.Add(new FacilityImage { Id = Guid.NewGuid(), FacilityId = facility.Id, ImageUrl = img });
                }
            }

            if (request.AmenityIds != null)
            {
                _context.FacilityAmenities.RemoveRange(facility.FacilityAmenities);
                foreach (var aid in request.AmenityIds)
                {
                    _context.FacilityAmenities.Add(new FacilityAmenity { FacilityId = facility.Id, AmenityId = aid });
                }
            }

            facility.ModifiedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();
            return await GetFacilityDetailsAsync(facility.Id);
        }
    }
}
