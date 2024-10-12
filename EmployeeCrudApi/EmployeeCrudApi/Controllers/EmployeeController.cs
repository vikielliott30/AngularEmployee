using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // Importa Regex
using System.Threading.Tasks;

namespace EmployeeCrudApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet]
        public async Task<Employee> GetById(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Employee employee)
        {
            var employeeToUpdate = await _context.Employees.FindAsync(employee.Id);
            if (employeeToUpdate == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            employeeToUpdate.Name = employee.Name;
            // Otras actualizaciones...
            await _context.SaveChangesAsync();

            return Ok("Empleado actualizado exitosamente.");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var employeeToDelete = await _context.Employees.FindAsync(id);
            if (employeeToDelete == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            _context.Employees.Remove(employeeToDelete);
            await _context.SaveChangesAsync();

            return Ok("Empleado eliminado exitosamente.");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            // 1. Nombre del empleado no repetido
            if (await _context.Employees.AnyAsync(e => e.Name == employee.Name))
            {
                return BadRequest("El nombre del empleado ya está registrado.");
            }

            // 2. Longitud máxima del nombre y apellido (10 caracteres)
            if (employee.Name.Length > 10)
            {
                return BadRequest("El nombre no puede superar los 10 caracteres.");
            }

            // 3. Longitud mínima del nombre (2 caracteres)
            if (employee.Name.Length < 2)
            {
                return BadRequest("El nombre debe tener al menos 2 caracteres.");
            }

            // 4. Verificar que el nombre no contenga números
            if (Regex.IsMatch(employee.Name, @"\d"))
            {
                return BadRequest("El nombre no puede contener números.");
            }

            // 5. Evitar caracteres repetidos de forma excesiva
            if (HasExcessiveRepeatingCharacters(employee.Name))
            {
                return BadRequest("El nombre contiene caracteres repetidos de forma excesiva.");
            }

            // Si todo está bien, agregar el empleado
            employee.CreatedDate = DateTime.Now;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return Ok("Empleado agregado exitosamente.");
        }

        private bool HasExcessiveRepeatingCharacters(string input)
        {
            // Verifica si hay 3 o más caracteres consecutivos repetidos
            return Regex.IsMatch(input, @"(.)\1{2,}");
        }
    }
}