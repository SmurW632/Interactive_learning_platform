// server/Controllers/CoursesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Data.DTOs;
using server.Services;
using System.Security.Claims;

namespace server.Controllers;

public class CoursesController : BaseApiV1Controller
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Получить список курсов с фильтрацией и пагинацией
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? level,
        [FromQuery] string? priceRange,
        [FromQuery] string? duration,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        try
        {
            var filter = new CourseFilterDto
            {
                Search = search,
                Category = category,
                Level = level,
                PriceRange = priceRange ?? "all",
                Duration = duration ?? "all",
                SortBy = sortBy ?? "popular",
                Page = page,
                PageSize = pageSize
            };

            var result = await _courseService.GetCoursesAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses");
            return StatusCode(500, new { message = "Ошибка при получении списка курсов" });
        }
    }

    /// <summary>
    /// Получить детальную информацию о курсе
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdGuid = userId != null ? Guid.Parse(userId) : (Guid?)null;

            var course = await _courseService.GetCourseByIdAsync(id, userIdGuid);
            if (course == null)
                return NotFound(new { message = "Курс не найден" });

            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course {CourseId}", id);
            return StatusCode(500, new { message = "Ошибка при получении курса" });
        }
    }

    /// <summary>
    /// Получить все категории
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _courseService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "Ошибка при получении категорий" });
        }
    }
}
