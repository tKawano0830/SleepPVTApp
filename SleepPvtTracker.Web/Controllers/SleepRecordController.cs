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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] CreateSleepRecordViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var dto = viewModel.ConvertToDto();

        try
        {

            await _service.CreateSleepRecordAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }
}