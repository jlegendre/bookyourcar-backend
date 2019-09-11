using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using TestAuthentification.Models;

namespace TestAuthentification.Services
{
    public class LocationService
    {
        public BookYourCarContext _context;

        public LocationService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }

        public List<Vehicle> GetAvailableVehicleForLocation()
        {
            _context.Database.ExecuteSqlCommand("getAvailableVehicle @p0, @p1, @p2, @p3", parameters: new[] { DateTime.Now.ToString(), DateTime.Now.ToString(), 1.ToString(),1.ToString() });
            return new List<Vehicle>();
        }
    }
}


