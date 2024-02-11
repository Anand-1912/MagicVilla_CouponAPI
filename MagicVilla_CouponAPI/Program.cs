using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Diagnostics.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.
    MapGet("/api/coupon", (ILogger<Program> _logger) => {

        _logger.LogInformation("Listing all Coupons");
        return Results.Ok(CouponStore.couponList);
        })
    .WithName("GetCoupons")
    .Produces<IEnumerable<Coupon>>(200);

app.MapGet("/api/coupon/{id:int}", (int id) => {

    if (CouponStore.couponList.Any(coupon => coupon.Id == id))
    { 
        return Results.Ok(CouponStore.couponList.FirstOrDefault(coupon => coupon.Id == id)); 
    }
    else 
    {
        return Results.NotFound("Invalid Coupon"); 
    }

})
    .WithName("GetCoupon")
    .Produces<Coupon>(200); ;

app.MapPost("/api/coupon", async (CouponCreateDto coupon, IMapper _mapper, IValidator<CouponCreateDto> _couponCreateValidator, ILogger<Program> _logger) => {

    _logger.LogInformation("Creating new Coupon!");

    await Task.Run(() => Task.Delay(10000));

    var validationResult = await _couponCreateValidator.ValidateAsync(coupon);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }
    else if ( CouponStore.couponList.Any( cpn => cpn.Name.Equals(coupon.Name, StringComparison.InvariantCultureIgnoreCase)))      
    {
        return Results.BadRequest("Coupon already exists with same Name!");
    }
    else
    {
        var newCoupon = _mapper.Map<Coupon>(coupon);

        newCoupon.Id = CouponStore.couponList.Max(coupon => coupon.Id) + 1;

        CouponStore.couponList.Add(newCoupon);

        CouponDto couponDto = _mapper.Map<CouponDto>(newCoupon);
        
        return Results.CreatedAtRoute("GetCoupon",new { id = newCoupon.Id }, couponDto);
    }
})
    .WithName("CreateCoupon")
    .Produces<CouponDto>(201)
    .Produces(400)
    .Accepts<CouponCreateDto>("application/json");

app.MapPut("/api/coupon", () => { })
    .WithName("UpdateCoupon")
    .Produces(200);

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
   
    var coupon = CouponStore.couponList.Find(coupon => coupon.Id == id);
    if (coupon == null)
    {
        return Results.NotFound("Invalid Coupon");
    }
    else
    {
        CouponStore.couponList.Remove(coupon);
        return Results.Ok();
    }
}).WithName("DeleteCoupon")
    .Produces(200);

app.UseHttpsRedirection();



app.Run();