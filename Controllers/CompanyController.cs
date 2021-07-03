using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using conversion_api.Models;
using conversion_api.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace conversion_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CompanyController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CompanyController> _logger;
        private readonly IOptions<MailGunSmtpEmailSettings> _smtpEmailConfig;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(ILogger<CompanyController> logger, 
            DBContext context, 
            IConfiguration configuration, 
            IOptions<MailGunSmtpEmailSettings> smtpEmailConfig,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _smtpEmailConfig = smtpEmailConfig;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Company
        [HttpGet]
        public IEnumerable<Company> GetCompany()
        {
            return _context
                    .Companies
                    .Include(c=> c.Stores.Where(s=>s.IsDelete != 1))
                    .Include(c => c.Campaigns.Where(c => c.IsDelete != 1))
                    .Where(c => c.IsDelete != 1).ToList();
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        // PUT: api/Company/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany([FromRoute] int id, [FromBody] Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != company.Id)
            {
                return BadRequest();
            }

            Company companyObj = _context.Companies.FirstOrDefault(e => e.Id == id);
            if (companyObj != null)
            {
                company.CreatedDate = companyObj.CreatedDate;
                company.ModifiedDate = DateTime.Now;
                _context.Entry(company).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NoContent();
        }

        // POST: api/Company
        [HttpPost]
        public async Task<IActionResult> PostCompany([FromBody] Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            company.CreatedDate = DateTime.Now;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            company.IsDelete = 1;
            _context.Entry(company).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(company);
        }

        // POST: api/Company/sendContact
        [HttpPost("sendContact")]
        public async Task<IActionResult> SendContact([FromBody] dynamic contactUs)
        {
            try
            {
                string email = (string)contactUs.storeEmail;
                string message = "<br/>Contact Us Message<br/>";
                message += "<br/>FirstName: " + (string)contactUs.firstname;
                message += "<br/>LastName: " + (string)contactUs.lastname;
                message += "<br/>Email: " + (string)contactUs.email;
                message += "<br/>Message: " + (string)contactUs.message;

                string subject = "New Contact Us Message";
                MailGunSmtpEmailSender emailSender = new MailGunSmtpEmailSender(_smtpEmailConfig, _webHostEnvironment);
                await emailSender.SendEmailAsync(email, subject, message);

                dynamic response = new JObject();
                response.result = "sent";

                return Ok(response);
            }
            catch
            {
                return Ok();
            }
        }

        // POST: api/Company/sendCalendar
        [HttpPost("sendCalendar")]
        public async Task<IActionResult> SendCalendar([FromBody] dynamic calendar)
        {
            try
            {
                string storeEmail = (string)calendar.storeEmail;
                string email = (string)calendar.email;
                string message = "<br/>Booking Details<br/>";
                message += "<br/>FirstName: " + (string)calendar.firstname;
                message += "<br/>LastName: " + (string)calendar.lastname;
                message += "<br/>Email: " + email;
                message += "<br/>Date: " + (((string)calendar.date != null) ? ((DateTime)calendar.date).ToShortDateString() : "");
                message += "<br/>Time: " + (string)calendar.time;

                string subject = "New Booking Request";
                MailGunSmtpEmailSender emailSender = new MailGunSmtpEmailSender(_smtpEmailConfig, _webHostEnvironment);
                await emailSender.SendEmailAsync(storeEmail, subject, message);
                await emailSender.SendEmailAsync(email, subject, message);

                dynamic response = new JObject();
                response.result = "sent";

                return Ok(response);
            }
            catch
            {
                return Ok();
            }
        }

        private bool CompanyExists(long id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
