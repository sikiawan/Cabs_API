using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using Model.Models;
using Model.ViewModel;
using Newtonsoft.Json;
using CabsAPI.Helpers;
using System.Net;
using System.Net.Http.Headers;

namespace CabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ValuesController : ControllerBase
    {
        IHubContext<TestHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private const string FacebookGraphApiUrl = "https://graph.facebook.com/v17.0/187226267809043/messages";
        private const string AccessToken = "EAAQEyNRH3bIBOZBVS9ZBLBxRh50px6hr1mH1fgU8yX9YOQiIPUFQDX4t8oLp4qngTFepbopL9mB1XoRCuahFj9vDY6awrUuhAd9YZBB52qcvPWkyibYplZCl2ZCrHo6lk6K4JcB4lcgT5xHIPMBoyvgFmp4P7HdG70lVn6PN4uUO7ZCu0mA9QCUryHad4MsoHVm64AQTZAxtViK5lbWyHJhzNhnRsUU";

        public ValuesController(IHubContext<TestHub> hubcontext, IUnitOfWork unitOfWork)
        {
            _hubContext = hubcontext;
            _unitOfWork = unitOfWork;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _hubContext.Clients.All.SendAsync("Posted", value);

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpPost]
        [Route("api/sendwhatsappmessage")]
        public async Task<ActionResult<string>> SendMessage([FromBody] FacebookMessageRequest request)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");

                var messageContent = new
                {
                    messaging_product = "whatsapp",
                    to = request.To,
                    type = "template",
                    template = new
                    {
                        name = "hello_world",
                        language = new { code = "en_US" }
                    }
                };

                var response = await client.PostAsJsonAsync(FacebookGraphApiUrl, messageContent);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Message sent successfully");
                }
                else
                {
                    return BadRequest($"Failed to send message. Response: {await response.Content.ReadAsStringAsync()}");
                }
            }
        }
    }
    public class FacebookMessageRequest
    {
        public string To { get; set; }
    }
}
