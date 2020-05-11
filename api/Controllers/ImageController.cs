
using dp.business.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dp.api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : BaseController
    {

        public ImageController()
        {
            // _userService = userService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string guid, bool thumb = false)
        {
            if (guid == null)
            {
                return BadRequest("No GUID provided");
            }
            byte[] imageBytes = null;

            //   TODO
            //   [ImageGet](imageId1,0)

            imageBytes = await AdoNetDao.ImageDao.GetImageAsync(guid.ToString(), thumb);
            return File(imageBytes, "image/jpeg");


        }


        [HttpPost]
        [Route("Save")]
        [Authorize]
        public async Task<IActionResult> Post(IFormFile form)
        {
            var currentUserId = GetClaimedUser().UserId;
            using (var st = new MemoryStream())
            {

                await form.CopyToAsync(st);
                var fileBytes = st.ToArray();
                string s = Convert.ToBase64String(fileBytes);



                if (fileBytes != null)
                {


                    if (fileBytes != null && ImageBinaryValidator.GetImageFormat(fileBytes) != ImageBinaryValidator.ImageFormat.Unknown)
                    {
                        string imageGuid = await AdoNetDao.ImageDao.AddImageAsync(fileBytes, currentUserId);
                        return Ok(imageGuid);

                    }
                }
            }


            return BadRequest("Failed to save image");
        }






    }
}