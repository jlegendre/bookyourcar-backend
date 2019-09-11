using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TestAuthentification.Models;
using TestAuthentification.Services;
using TestAuthentification.ViewModels.Planning;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanningController : ControllerBase
    {
        private readonly BookYourCarContext _context;
        private readonly AuthService _authService;

        private ILogger _logger;

        public PlanningController(BookYourCarContext context)
        {
            _context = context;
            _authService = new AuthService(context);
        }

        // GET: api/Planning/5
        [HttpGet("{date}")]
        public async Task<IActionResult> GetPlanning([FromRoute] DateTime date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DayOfWeek today = date.DayOfWeek;

                PlanningViewModel planningVM = new PlanningViewModel();

                PlanningService planningService = new PlanningService(_context);

                //Get first and last day of current week
                Tuple<DateTime, DateTime> weeksDay = GetFirstAndLastDaysOfCurrentWeek(date);
                planningVM.StartWeek = weeksDay.Item1;
                planningVM.EndWeek = weeksDay.Item2;

                //Get count reservations which end or strat in the current week
                planningVM.StartReservationCount = planningService.GetStartReservationCountThisWeek(weeksDay);
                planningVM.EndReservationCount = planningService.GetEndReservationCountThisWeek(weeksDay);

                //Get all vehicle count and used vehicle count for today
                planningVM.TotalVehiclesCount = planningService.GetCountTotalVehicles();
                planningVM.UsedVehiclesCount = planningService.GetUsedCarToday();

                //Get List of vehicule and with reservations for the calendar, on each line, if there is a reservation, display on tooltip the name of the driver
                planningVM.ListOfReservationsByVehicule = planningService.GetReservationsByVehicule(GetFirstAndLastDaysOfCurrentWeek(date));

                return Ok(planningVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Error", "Erreur durant la récupération du planning.");
                return BadRequest(ModelState);
            }       
        }

        private Tuple<DateTime, DateTime> GetFirstAndLastDaysOfCurrentWeek(DateTime date)
        {
            DayOfWeek dayOfWeek = date.DayOfWeek;

            DateTime fisrtDay = date.AddDays(-(int)dayOfWeek + 1);
            DateTime lastDay = date.AddDays(7 - (int)dayOfWeek + 1);

            return new Tuple<DateTime,DateTime>(fisrtDay, lastDay);
        }
    }
}