using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApp.Model;

namespace TestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly AppDbContext _db;
        public PersonController(AppDbContext db) {
        
            _db = db;

        }


        [HttpGet]
        public async Task<ActionResult> ViewPersons()
        {
            var result = await _db.Persons.ToListAsync();

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePerson([FromBody] Person person)
        {
            var editedPerson = await _db.Persons.Where(r => r.Id == person.Id).FirstOrDefaultAsync();

            if (editedPerson != null)
            {
                editedPerson.Name = person.Name;
                editedPerson.DateOfBirth = person.DateOfBirth;
                editedPerson.Maried = person.Maried;
                editedPerson.Phone = person.Phone;
                editedPerson.Salary = person.Salary;

                _db.Update(editedPerson);
                _db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpDelete]
        public IActionResult DeletePerson([FromQuery] int id)
        {
            var person = _db.Persons.Find(id);
            if (person != null)
            {
                _db.Persons.Remove(person);
                _db.SaveChanges();
            }
            return Json(new { success = true });
        }


        [HttpGet("upload")]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!file.FileName.EndsWith(".csv"))
            {
                return BadRequest("Please upload a CSV file.");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var csvData = await reader.ReadToEndAsync();
                var persons = ParseCsv(csvData);
                await _db.Persons.AddRangeAsync(persons);
                await _db.SaveChangesAsync();
            }

            return Ok("File uploaded and data saved.");
        }

        private List<Person> ParseCsv(string csvData)
        {
            var persons = new List<Person>();
            var lines = csvData.Split("\n");

            // Assuming the first line is the header
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(",");
                if (columns.Length >= 5) // Adjust based on CSV structure
                {
                    var person = new Person
                    {
                        Name = columns[0],
                        DateOfBirth = DateTime.Parse(columns[1]),
                        Maried = bool.Parse(columns[2]),
                        Phone = columns[3],
                        Salary = decimal.TryParse(columns[4], out var salary) ? salary : 0
                    };
                    persons.Add(person);
                }
            }

            return persons;
        }
    }
}
