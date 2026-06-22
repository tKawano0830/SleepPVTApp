using Microsoft.AspNetCore.Mvc;
using SleepPvtTracker.Web.Models;
using SleepPvtTracker.Core.UseCases;

namespace SleepPvtTracker.Web.Controllers;

public class SleepRecordController(ISleepRecordUseCase service, TimeProvider timeProvider) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var records = await service.GetAllSleepRecordsAsync();
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
        var currentTime = timeProvider.GetLocalNow().DateTime;

        var result = await service.CreateSleepRecordAsync(dto, currentTime);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitPvt([FromBody] SubmitPvtViewModel viewModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var dto = viewModel.ConvertToDto();
        var result = await service.SubmitPvtAsync(dto);
        if (result.IsFailure) return BadRequest(new { message = result.ErrorMessage });

        return Ok();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest();
        var result = await service.DeleteSleepRecordAsync(id);
        if (result.IsFailure) return NotFound();

        return RedirectToAction(nameof(Index));
    }
}