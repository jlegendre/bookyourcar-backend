using System;
using System.IO;
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
    public class ImagesController : Controller
    {
        private readonly BookYourCarContext _context;

        public ImagesController(BookYourCarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Uplaods an image user to the server.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadImageUser")]
        public async Task<IActionResult> UploadImageUser(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0) return Content("file not selected");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", file.FileName);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                Images newImage = new Images()
                {
                    ImageUri = path,
                    ImageUserId = userId
                };
                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", "Une erreur est survenue." + e.Message);
                Console.WriteLine(e);
                return BadRequest(ModelState);
            }

            return Ok("Files upload");


        }

        /// <summary>
        /// Uplaods an image vehicule to the server.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="vehiculeId"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadImageVehicule")]
        public async Task<IActionResult> UploadImageVehicule(IFormFile file, int vehiculeId)
        {
            var token = GetToken();

            if (!TokenService.ValidateTokenWhereIsAdmin(token) || !TokenService.VerifDateExpiration(token))
                return Unauthorized();
            
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("Error", "Veuillez choisir un fichier.");
                return BadRequest(ModelState);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", file.FileName);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                Images newImage = new Images()
                {
                    ImageUri = path,
                    ImageVehId = vehiculeId
                };
                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", "Une erreur est survenue." + e.Message);
                Console.WriteLine(e);
                return BadRequest(ModelState);
            }

            return Ok("Files upload");


        }


        /// <summary>
        /// retourne le lien d'une image en fonction d'un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetImageByUser")]

        public async Task<IActionResult> GetImageByUser(int userId)
        {
            return new ObjectResult(_context.Images.FirstOrDefault(x => x.ImageUserId == userId)?.ImageUri);
        }

        /// <summary>
        /// retourne le lien d'une image en fonction d'un vehicule
        /// </summary>
        /// <param name="vehiculeId"></param>
        /// <returns></returns>
        [

        HttpGet, Route("GetImageByVehicule")]

        public async Task<IActionResult> GetImageByVehicule(int vehiculeId)
        {
            return new ObjectResult(_context.Images.FirstOrDefault(x => x.ImageUserId == vehiculeId)?.ImageUri);
        }

        private string GetToken()
        {
            var token = Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer"))
            {
                var tab = token.Split(" ");
                token = tab[1];
            }

            return token;
        }


    }



}