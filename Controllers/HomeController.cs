using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RoomFInder.Data;
using RoomFInder.Models;
using RoomFInder.Repository;

namespace RoomFInder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly IGenericRepository<Room> _genericRepository;

    public HomeController(ILogger<HomeController> logger, AppDbContext context )
    {
        _logger = logger;
        _context = context;
    }


    public IActionResult Index(string searchString, string SortOrder, PropertyType? propertyType)
    {
        ViewData["MinPriceSortparm"] = String.IsNullOrEmpty(SortOrder) ? "minprice_desc" : "";
        ViewData["MaxPriceSortparm"] = SortOrder == "MaxPrice" ? "maxprice_desc" : "maxPrice";


        var rooms = _context.Rooms
            .Include(r => r.Owner)
            .AsQueryable();

        if (propertyType.HasValue)
        {
            rooms = rooms.Where(r => r.PropertyType == propertyType.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            rooms = rooms.Where(s => s.Location.Contains(searchString));
        }

        switch (SortOrder)
        {
            case "minprice_desc":
                rooms = rooms.OrderByDescending(r => r.Price);
                break;
            case "MaxPrice":
                rooms = rooms.OrderBy(r => r.Price);
                break;
            case "maxprice_desc":
                rooms = rooms.OrderByDescending(r => r.Price);
                break;
            default:
                rooms = rooms.OrderBy(r => r.Price);
                break;
        }

        return View(rooms);

    }


public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}