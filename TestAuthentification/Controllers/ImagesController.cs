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
        private readonly AuthService _authService;

        public ImagesController(BookYourCarContext context)
        {
            _context = context;
            _authService = new AuthService(context);
        }

        /// <summary>
        /// Uplaods an image user to the server.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost, Route("UploadImageUser")]
        public async Task<IActionResult> UploadImageUser(IFormFile file)
        {
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            User user = _authService.GetUserConnected(token);

            if (CheckIfImageFile(file))
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
                        ImageUserId = user.UserId
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
            else
            {
                ModelState.AddModelError("Error", "Format du fichier non pris en charge.");
                return BadRequest(ModelState);
            }




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
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            if (CheckIfImageFile(file))
            {
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
            else
            {
                ModelState.AddModelError("Error", "Format du fichier non pris en charge.");
                return BadRequest(ModelState);
            }

        }


        /// <summary>
        /// retourne le lien d'une image en fonction d'un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetImageByUser")]
        public async Task<IActionResult> GetImageByUser()
        {
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();
            User user = _authService.GetUserConnected(token);


            // check si l'user a une image
            if (!checkIfUserAsPicture(user.UserId))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", "default-user-image.png");
                return new ObjectResult(path);
            }

            return new ObjectResult(_context.Images.FirstOrDefault(x => x.ImageUserId == user.UserId)?.ImageUri);
        }

        private bool checkIfUserAsPicture(int userId)
        {
            try
            {
                return _context.Images.FirstOrDefault(x => x.ImageUserId == userId)?.ImageUri == null;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private bool checkIfUserAsVehiculePicture(int vehiculeId)
        {
            try
            {
                return _context.Images.FirstOrDefault(x => x.ImageVehId == vehiculeId)?.ImageUri == null;
            }
            catch (Exception e)
            {
                return false;
            }


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
            string token = GetToken();
            if (!TokenService.ValidateToken(token) || !TokenService.VerifDateExpiration(token)) return Unauthorized();

            // check si l'user a une image
            if (!checkIfUserAsVehiculePicture(vehiculeId))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", "default-no-car-pic.png");
                return new ObjectResult(path);
            }

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

        /// <summary>
        /// Method to check if file is image file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return ImageService.GetImageFormat(fileBytes) != ImageService.ImageFormat.unknown;
        }



    }



}