using Microsoft.AspNetCore.Mvc;
using SleepPvtTracker.Web.Models;
using SleepPvtTracker.Core.DTOs;
using SleepPvtTracker.Core.Services;
using SleepPvtTracker.Core.Exceptions;

namespace SleepPvtTracker.Web.Controllers;

public class SleepRecordController(ISleepRecordService service) : Controller
{
    private readonly ISleepRecordService _service = service;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var records = await _service.GetAllSleepRecordsAsync();

        var viewModels = records.Select(SleepRecordListViewModel.ConvertFromEntity).ToList();

        return View(viewModels);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PvtTest(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("無効なIDです");

        var viewModel = new SubmitPvtViewModel { SleepRecordId = id };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] CreateSleepRecordViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var dto = viewModel.ConvertToDto();

        try
        {
            await _service.CreateSleepRecordAsync(dto, DateTime.Now);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(viewModel);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "サーバエラーが発生しました" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitPvt([FromBody] SubmitPvtViewModel viewModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var dto = viewModel.ConvertToDto();
            await _service.SubmitPvtAsync(dto);
            return Ok();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "サーバエラーが発生しました" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest();

        try
        {
            await _service.DeleteSleepRecordAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (EntityNotFoundException)
        {
            return NotFound();
        }
    }
}