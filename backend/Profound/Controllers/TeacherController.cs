﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public TeacherController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        // GET: api/Teacher
        [HttpGet("courses")]

        public IEnumerable<Course> GetCourses(int teacherId)
        {
            return _dataRepository.GetCourses().Where(c => c.CreatorId == teacherId);
        }

        [HttpGet("courses/{courseId}")]
        public IEnumerable<Module> GetModules(int courseId)
        {
            return _dataRepository.GetCourseModules(courseId);
        }

        [HttpGet("courses/{courseId}/{moduleId}")]
        public IEnumerable<Lesson> GetLessons(int moduleId)
        {
            return _dataRepository.GetModuleLessons(moduleId);
        }

        [HttpGet("courses/{courseId}/{moduleId}/{lessonId}")]
        public IEnumerable<Component> GetComponent(int lessonId)
        {
            return _dataRepository.GetLessonComponents(lessonId);
        }

        [HttpPost("createCourse")]
        public Course CreateCourse(Course course)
        {
            return _dataRepository.CreateCourse(course);
        }

        [HttpPost("courses/{courseId}/createModule")]
        public Module CreateModule(int courseId, Module module)
        {
            module.CourseId = courseId;
            return _dataRepository.CreateModule(module);
        }

        [HttpPost("courses/{courseId}/{moduleId}/createLesson")]
        public Lesson CreateLesson(int moduleId, Lesson lesson)
        {
            lesson.ModuleId = moduleId;
            return _dataRepository.CreateLesson(lesson);
        }

        [HttpPost("courses/{courseId}/{moduleId}/{lessonId}/createComponent")]
        public Component CreateComponent(int lessonId, Component component)
        {
            component.LessonId = lessonId;
            return _dataRepository.CreateComponent(component);
        }

        [HttpPost("requestToModeration")]
        public void RequestToPublic(int course_id)
        {
            _dataRepository.RequestToModeration(course_id);
        }

        public IEnumerable<string> GetCourses()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Teacher/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Teacher
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Teacher/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
