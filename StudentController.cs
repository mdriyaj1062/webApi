using CollageApp.Data;
using CollageApp.Models;
using CollageApp.MyLogging;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System.Linq;
using System.Net; 
using System.Xml.Linq;

namespace CollageApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController:ControllerBase
    {
       private readonly ILogger<StudentController> _logger;
        private readonly CollegeDBContext _dbContext;
        public StudentController(ILogger<StudentController> logger, CollegeDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("All",Name ="GetAllStudents()")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetStudents()
        {
            _logger.LogInformation("GetStudents methods started");

            // prepare DTO :-
            //var students = new List<StudentDTO>();
            //foreach (var item in CollageReository.Students)
            //{
            //    StudentDTO obj = new StudentDTO()
            //    {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Email = item.Email,
            //        Adress = item.Address,

            //    };
            //    students.Add(obj);
            //}
            // we can also build it using a link you query : - using link you we can convert students list into students doto list
            var students = _dbContext.Students.Select(s => new StudentDTO()
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Adress=s.Address

            }).ToList();
            // ok-200
            return Ok(students);
            
        }
        // for single Student 
        [HttpGet]
        [Route("{id:int}",Name ="GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetStudentById(int id)
        {
            if (id <=0)
            {
                _logger.LogWarning("Bad request");
                return BadRequest();
            }
            // ok-200 for Success
            var student= _dbContext.Students.Where(n => n.Id == id).FirstOrDefault();
            if (student == null) {
                _logger.LogError("student not found with given Id");
                return NotFound();
            }
            var studentDTO = new StudentDTO()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Adress = student.Address
            };
            return Ok(studentDTO);
        }

        [HttpGet("{name:alpha}",Name ="GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name)) { 
                return BadRequest();
            }
           var student= _dbContext.Students.Where(n => n.Name == name).FirstOrDefault();
            if (student == null) 
                return NotFound($"The student with name {name} not found !");
            var studentDTO = new StudentDTO()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Adress = student.Address
            };
            return Ok(studentDTO);
            
        }
        // create HttpPost
        [HttpPost]
        [Route("Create")]
        //api/student/create
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public IActionResult CreateStudent([FromBody]StudentDTO model)
        {
            if(model == null)
                return BadRequest();
            ////Custom Attributes :-
            //if(model.AdmissionDate < DateTime.Now)
            //{
            //    //1. Directly adding error messgae to modelstate
            //    //2. Using custom attribute
            //    ModelState.AddModelError("AdmissionDate Error", "Admission date must be greater than or equal to todays date");
            //    return BadRequest(ModelState);
            //}

            //int newId = _dbContext.Students.LastOrDefault().Id + 1;
            Student student = new Student
            {
                //Id=newId,
                Name = model.Name,
                Email = model.Email,
                Address=model.Adress 
            };
            _dbContext.Students.Add(student);
            // after adding, we need to call dbContext.saveChanges
            _dbContext.SaveChanges();
            model.Id = student.Id;
            //CreateAtRoute
            return CreatedAtRoute("GetStudentById", new {id=model.Id },model);    
            //return Ok(model);
        }
        [HttpPut]
        [Route("Update")]
        // api/student/update
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpDateStudent([FromBody] StudentDTO model)
        {
            if(model == null || model.Id<=0)
                return BadRequest();
            var existingStudent = _dbContext.Students.Where(s => s.Id == model.Id).FirstOrDefault();
            if (existingStudent == null)
           return NotFound();
            existingStudent.Name = model.Name;
            existingStudent.Email = model.Email;
            existingStudent.Address=model.Adress;
            // after updating we need to call DbContext.SaveChanges
            _dbContext.SaveChanges();
            return NoContent();
        }
        // HttpPatch
        [HttpPatch]
        [Route("{id:int}/UpdatepPartial")]
        // api/student/1/UpdatePartial
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpDateStudentPartial(int id,[FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
                return BadRequest();

            var existingStudent = _dbContext.Students.Where(s => s.Id == id).FirstOrDefault();
            if (existingStudent == null)
                return NotFound();

            // Create a new student DTO based on the existing student
            var studentDTO = new StudentDTO()
            {
                Id = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email,
                Adress = existingStudent.Address
            };

            // Apply the patch document to the student DTO
            patchDocument.ApplyTo(studentDTO, ModelState);

            // After applying the patch, update the existing student's properties
            existingStudent.Name = studentDTO.Name;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Adress;
            // after updating we need to call DbContext.SaveChanges
            _dbContext.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id:int}",Name ="DeleteStudentById")]
        public IActionResult DeleteStudent(int id)
        {
            
           var stu= _dbContext.Students.Where(n=>n.Id==id).FirstOrDefault();
            _dbContext.Students.Remove(stu);
            // after deleting we need to call DbContext.SaveChanges
            _dbContext.SaveChanges();
            return Ok(true);
        }
    }
}
