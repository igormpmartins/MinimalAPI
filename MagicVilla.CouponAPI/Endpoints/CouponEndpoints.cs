using AutoMapper;
using FluentValidation;
using MagicVilla.CouponAPI.Models;
using MagicVilla.CouponAPI.Models.DTO;
using MagicVilla.CouponAPI.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace MagicVilla.CouponAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", GetAllCoupons)
                .WithName("GetCoupons").Produces<APIResponse>(StatusCodes.Status200OK)
                .RequireAuthorization("AdminOnly");

            app.MapGet("/api/coupon/special", GetSpecial)
                .WithName("GetCouponSpecial").Produces<APIResponse>(StatusCodes.Status200OK);

            app.MapGet("/api/coupon/{id:int}", GetCoupon)
                .WithName("GetCoupon").Produces<APIResponse>(StatusCodes.Status200OK);

            app.MapPost("/api/coupon", CreateCoupon)
                .WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json")
                .Produces<APIResponse>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

            app.MapPut("/api/coupon", UpdateCoupon)
                .WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json")
                .Produces<APIResponse>(StatusCodes.Status200OK).Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/coupon/{id:int}", DeleteCoupon)
                .WithName("DeleteCoupon")
                .Produces<APIResponse>(StatusCodes.Status204NoContent).Produces(StatusCodes.Status400BadRequest);
        }

        [Authorize]
        private static async Task<IResult> DeleteCoupon(ICouponRepository repository, int id)
        {
            var response = new APIResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
            };

            var coupon = await repository.GetAsync(id);
            if (coupon == null)
            {
                response.ErrorMessages.Add("Coupon not found");
                return Results.BadRequest(response);
            }

            await repository.RemoveAsync(coupon);
            await repository.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }

        [Authorize]
        private static async Task<IResult> UpdateCoupon(ICouponRepository repository,
            IValidator<CouponUpdateDTO> validator, IMapper mapper, CouponUpdateDTO couponDTO)
        {
            var response = new APIResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
            };

            var result = await validator.ValidateAsync(couponDTO);
            if (!result.IsValid)
            {
                response.ErrorMessages = result.Errors.Select(q => q.ErrorMessage).ToList();
                return Results.BadRequest(response);
            }

            var coupon = await repository.GetAsync(couponDTO.Id);
            if (coupon == null)
            {
                response.ErrorMessages.Add("Invalid coupon");
                return Results.BadRequest(response);
            }

            mapper.Map(couponDTO, coupon);
            await repository.UpdateAsync(coupon);
            await repository.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = mapper.Map<CouponDTO>(coupon);

            return Results.Ok(response);
        }

        [Authorize]
        private static async Task<IResult> CreateCoupon(ICouponRepository repository,
            IValidator<CouponCreateDTO> validator, IMapper mapper, CouponCreateDTO couponCreatedDTO)
        {
            var response = new APIResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
            };

            var result = await validator.ValidateAsync(couponCreatedDTO);

            if (!result.IsValid)
            {
                response.ErrorMessages = result.Errors.Select(q => q.ErrorMessage).ToList();
                return Results.BadRequest(response);
            }

            if (await repository.ExistsAsync(couponCreatedDTO.Name))
            {
                response.ErrorMessages.Add("Coupon name already exists");
                return Results.BadRequest(response);
            }

            var coupon = mapper.Map<Coupon>(couponCreatedDTO);
            await repository.CreateAsync(coupon);
            await repository.SaveAsync();

            var couponDTO = mapper.Map<CouponDTO>(coupon);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            response.Result = couponDTO;

            return Results.Ok(response);
        }

        [Authorize]
        private static async Task<IResult> GetCoupon(ICouponRepository repository, int id)
        {
            var response = new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = await repository.GetAsync(id)
            };
            return Results.Ok(response);
        }

        private static async Task<IResult> GetAllCoupons(ICouponRepository repository, ILogger<Program> logger)
        {
            logger.LogInformation("Getting all coupons");

            var response = new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = await repository.GetAllAsync()
            };

            return Results.Ok(response);
        }

        private static async Task<IResult> GetSpecial([AsParameters] CouponRequest request, ICouponRepository repository)
        {
            request.Logger.LogInformation("Getting coupons special");

            var response = new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = await repository.GetSpecialAsync(request.Text, request.PageSize, request.CurrentPage)
            };

            return Results.Ok(response);
        }
    }

    public class CouponRequest
    {
        public string Text { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public ILogger<CouponRequest> Logger { get; set; }
    }

}
