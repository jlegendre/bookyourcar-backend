using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAuthentification.Models;

namespace TestAuthentification.Services
{
    public class PoleService
    {
        public BookYourCarContext _context;
        public PoleService _poleservice;

        public PoleService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }


        public Pole GetPole(int? id)
        {
            var pole = _context.Pole.Find(id);

            return pole;
        }
    }
}


