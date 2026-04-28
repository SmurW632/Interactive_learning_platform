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
        [FromQuery] int pageSize = 4)
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
    /// Получить полную информацию о курсе для страницы курса
    /// </summary>
    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetCourseFullDetail(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdGuid = userId != null ? Guid.Parse(userId) : (Guid?)null;

            var course = await _courseService.GetCourseFullDetailAsync(id, userIdGuid);
            if (course == null)
                return NotFound(new { message = "Курс не найден" });

            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course full detail {CourseId}", id);
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

    /// <summary>
    /// Начать курс (записаться)
    /// </summary>
    [Authorize]
    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartCourse(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _courseService.StartCourseAsync(userId, id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message ?? "Не удалось записаться на курс" });

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting course {CourseId}", id);
            return StatusCode(500, new { message = "Ошибка при записи на курс" });
        }
    }

    /// <summary>
    /// Получить содержимое урока
    /// </summary>
    [HttpGet("{courseId}/lessons/{lessonId}")]
    public async Task<IActionResult> GetLessonContent(Guid courseId, Guid lessonId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdGuid = userId != null ? Guid.Parse(userId) : (Guid?)null;

            var lesson = await _courseService.GetLessonContentAsync(courseId, lessonId, userIdGuid);
            if (lesson == null)
                return NotFound(new { message = "Урок не найден" });

            return Ok(lesson);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson content {LessonId}", lessonId);
            return StatusCode(500, new { message = "Ошибка при получении урока" });
        }
    }

    /// <summary>
    /// Обновить прогресс урока
    /// </summary>
    [Authorize]
    [HttpPost("lessons/{lessonId}/progress")]
    public async Task<IActionResult> UpdateLessonProgress(Guid lessonId, [FromBody] UpdateProgressRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _courseService.UpdateLessonProgressAsync(userId, lessonId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson progress {LessonId}", lessonId);
            return StatusCode(500, new { message = "Ошибка при обновлении прогресса" });
        }
    }

    //[Authorize]
    [HttpGet("test")]
    public IActionResult TestAuth()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new
        {
            message = "Вы авторизованы!",
            userId,
            userName,
            email,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
