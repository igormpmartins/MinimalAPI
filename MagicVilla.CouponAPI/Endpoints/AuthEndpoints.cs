using AutoMapper;
using FluentValidation;
using MagicVilla.CouponAPI.Models;
using MagicVilla.CouponAPI.Models.DTO;
using MagicVilla.CouponAPI.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.CouponAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/login", Login)
                .WithName("Login").Accepts<LoginRequestDTO>("application/json")
                .Produces<APIResponse>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

            app.MapPost("/api/register", Register)
                .WithName("Register").Accepts<RegistrationRequestDTO>("application/json")
                .Produces<APIResponse>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> Login(IAuthRepository repository,
            [FromBody] LoginRequestDTO loginRequestDTO)
        {
            var response = new APIResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
            };

            var loginResponse = await repository.Login(loginRequestDTO);
            if (loginResponse == null) 
            {
                response.ErrorMessages.Add("Invalid username or password");
                return Results.BadRequest(response);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = loginResponse;

            return Results.Ok(response);
        }

        private static async Task<IResult> Register(IAuthRepository repository,
            [FromBody] RegistrationRequestDTO registrationDTO)
        {
            var response = new APIResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
            };

            var isUnique = repository.IsUniqueUser(registrationDTO.UserName);
            if (!isUnique)
            {
                response.ErrorMessages.Add("User already exists!");
                return Results.BadRequest(response);
            }

            var registerResponse = await repository.Register(registrationDTO);
            if (registerResponse == null || string.IsNullOrEmpty(registerResponse.UserName))
            {
                response.ErrorMessages.Add("Invalid operation");
                return Results.BadRequest(response);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = registerResponse;

            return Results.Ok(response);
        }
    }
}
