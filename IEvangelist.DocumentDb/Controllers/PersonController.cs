using IEvangelist.DocumentDb.Models;
using IEvangelist.DocumentDb.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEvangelist.DocumentDb.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        private readonly IRepository<Person> _repository;

        public PersonController(IRepository<Person> repository)
        {
            _repository = 
                repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        [HttpGet]
        public Task<IEnumerable<Person>> Get() => _repository.GetAsync(p => p != null);
        
        [HttpGet("{id}")]
        public Task<Person> Get(string id) => _repository.GetAsync(id);
        
        [HttpPost]
        public Task Post([FromBody] Person person) => _repository.CreateAsync(person);
        
        [HttpPut("{id}")]
        public Task Put(string id, [FromBody] Person person) => _repository.UpdateAsync(id, person);
        
        [HttpDelete("{id}")]
        public Task Delete(string id) => _repository.DeleteAsync(id);
    }
}