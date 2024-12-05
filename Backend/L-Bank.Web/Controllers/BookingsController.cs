using L_Bank_W_Backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IBookingRepository = L_Bank.EfCore.Repositories.IBookingRepository;

namespace L_Bank_W_Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController(IBookingRepository bookingRepository) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Booking booking)
    {
        return await Task.Run(() =>
        {
            IActionResult response;

            if (bookingRepository.Book(booking.SourceId, booking.DestinationId, booking.Amount))
                response = Ok();
            else
                response = Conflict();
            return response;
        });
    }
}