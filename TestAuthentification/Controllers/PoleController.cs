using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAuthentification.Models;
using TestAuthentification.Services;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoleController : ControllerBase
    {
        public A5dContext _context;
        public PoleService _poleservice;

        public PoleController(A5dContext context)
        {
            _context = context;
            _poleservice = new PoleService(context);
        }

        // GET api/pole
        [HttpGet]
        public IEnumerable<Pole> getPoles()
        {
            return _context.Pole.ToList();

        }
    }
}