using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using System.Diagnostics.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/coupon", () => Results.Ok(CouponStore.couponList)).WithName("GetCoupons");

app.MapGet("/api/coupon/{id:int}", (int id) => {

    if (CouponStore.couponList.Any(coupon => coupon.Id == id))
    { 
        return Results.Ok(CouponStore.couponList.FirstOrDefault(coupon => coupon.Id == id)); 
    }
    else 
    {
        return Results.NotFound("Invalid Coupon"); 
    }

}).WithName("GetCoupon"); ;

app.MapPost("/api/coupon", (Coupon coupon) => {

    if ( string.IsNullOrEmpty(coupon.Name))
    {
        return Results.BadRequest("Invalid Coupon Name!");
    }
    else if ( CouponStore.couponList.Any( cpn => cpn.Name.Equals(coupon.Name, StringComparison.InvariantCultureIgnoreCase) || cpn.Id == coupon.Id ))      
    {
        return Results.BadRequest("Coupon already exists with same Id or name!");
    }
    else
    {
        CouponStore.couponList.Add(coupon);
        // return Results.Created($"api/coupon/{coupon.Id}",coupon);
        return Results.CreatedAtRoute("GetCoupon",new { id = coupon.Id }, coupon);
    }
}).WithName("CreateCoupon"); ;

app.MapPut("/api/coupon", () => { });

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
});

app.UseHttpsRedirection();



app.Run();