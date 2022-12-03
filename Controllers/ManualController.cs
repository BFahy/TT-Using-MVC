using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TypicalTools.Controllers
{
    public class ManualController : Controller
    {
        private readonly DBContext context;
        private readonly ILogger<ManualController> _logger;
        private readonly FileLoaderService _loader;
        private readonly IWebHostEnvironment _environment;

        public ManualController(DBContext context, ILogger<ManualController> logger, FileLoaderService loader, IWebHostEnvironment environment)
        {
            this.context = context;
            _logger = logger;
            _loader = loader;
            _environment = environment;
        }

        /**
        * Retrieving files from database to be displayed
        * ** Can only be accessed by logged in / authorised users ** 
        */
        [HttpGet]        
        public IActionResult Index()
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, "Manuals");
            DirectoryInfo d = new DirectoryInfo(uploadPath);
            FileInfo[] files = d.GetFiles("*.*");
            string[] str = new string[10];
            for (int i = 0; i < files.Length; i++)
            {
                str[i] = files[i].Name;
            }
            ViewBag.ManualList = str;
            return View();
        }


        /**
         * Get request for 'AddManual' page to upload new manual
         * ** Cannot be accessed without user role set to administrator and authenticated **
         */
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddManual()
        {
            return View();
        }

        /**
         * Retrieving files from database to be displayed
         * Saving encrypted file to wwwroot
         * ** Cannot be accessed without admin privilege **
         */
        [HttpPost]
        public async Task<IActionResult> ManualUpload(IFormFile file)
        {
            string uploadPath = Path.Combine(_environment.WebRootPath, "Manuals");
            DirectoryInfo d = new DirectoryInfo(uploadPath);
            FileInfo[] files = d.GetFiles("*.*");
            string[] str = new string[10];
            for (int i = 0; i < files.Length; i++)
            {
                str[i] = files[i].Name;
            }
            ViewBag.ManualList = str;
            await _loader.SaveFile(file);
            return View("Index");
        }

        /**
         * Downloads decrypted file using FileLoaderService.cs
         */
        [HttpPost]
        public async Task<IActionResult> DownloadManual(string filename)
        {
            byte[] fileBytes = await _loader.LoadEncryptedFile(filename);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}
