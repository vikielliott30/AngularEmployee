using EmployeeCrudApi.Controllers;
using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeCrudApi.Tests
{
    public class EmployeeControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Crear una nueva base de datos en memoria para cada prueba
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfEmployees()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Employees.AddRange(
                new Employee { Id = 1, Name = "John Doe" },
                new Employee { Id = 2, Name = "Jane Doe" }
            );
            context.SaveChanges();

            var controller = new EmployeeController(context);

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].Name);
            Assert.Equal("Jane Doe", result[1].Name);
        }

        [Fact]
        public async Task GetById_ReturnsEmployeeById()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Employees.Add(new Employee { Id = 1, Name = "John Doe" });
            context.SaveChanges();

            var controller = new EmployeeController(context);

            // Act
            var result = await controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public async Task Create_AddsEmployee()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);

            var newEmployee = new Employee { Id = 3, Name = "Ignacio" };

            // Act
            await controller.Create(newEmployee);

            // Assert
            var employee = await context.Employees.FindAsync(3);
            Assert.NotNull(employee);
            Assert.Equal("Ignacio", employee.Name);
        }

        [Fact]
        public async Task Update_UpdatesEmployee()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var existingEmployee = new Employee { Id = 1, Name = "Old Name" };
            context.Employees.Add(existingEmployee);
            context.SaveChanges();

            var controller = new EmployeeController(context);

            var updatedEmployee = new Employee { Id = 1, Name = "Updated Name" };

            // Act
            await controller.Update(updatedEmployee);

            // Assert
            var employee = await context.Employees.FindAsync(1);
            Assert.NotNull(employee);
            Assert.Equal("Updated Name", employee.Name);
        }

        [Fact]
        public async Task Delete_RemovesEmployee()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var employeeToDelete = new Employee { Id = 1, Name = "John Doe" };
            context.Employees.Add(employeeToDelete);
            context.SaveChanges();

            var controller = new EmployeeController(context);

            // Act
            await controller.Delete(1);

            // Assert
            var employee = await context.Employees.FindAsync(1);
            Assert.Null(employee); // Verifica que el empleado fue eliminado
        }

        //Pruebas Unitarias de los 5 escenarios elegidos

        
        [Fact]
public async Task AddEmployee_NameAlreadyExists_ReturnsBadRequest()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new EmployeeController(context);

    var employee1 = new Employee { Name = "Juan" };
    var employee2 = new Employee { Name = "Juan" };
    await controller.Create(employee1); // Agrega el primer empleado

    // Act
    var result = await controller.Create(employee2); // Intenta agregar el segundo con el mismo nombre

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
    var badRequestResult = result as BadRequestObjectResult;
    Assert.Equal("El nombre del empleado ya está registrado.", badRequestResult.Value);
}

[Fact]
public async Task AddEmployee_NameTooLong_ReturnsBadRequest()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new EmployeeController(context);
    var employee = new Employee { Name = "NombreLargo" };

    // Act
    var result = await controller.Create(employee);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
    var badRequestResult = result as BadRequestObjectResult;
    Assert.Equal("El nombre no puede superar los 10 caracteres.", badRequestResult.Value);
}

[Fact]
public async Task AddEmployee_NameTooShort_ReturnsBadRequest()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new EmployeeController(context);
    var employee = new Employee { Name = "A" };

    // Act
    var result = await controller.Create(employee);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
    var badRequestResult = result as BadRequestObjectResult;
    Assert.Equal("El nombre debe tener al menos 2 caracteres.", badRequestResult.Value);
}

[Fact]
public async Task AddEmployee_NameContainsNumbers_ReturnsBadRequest()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new EmployeeController(context);
    var employee = new Employee { Name = "Juan123" };

    // Act
    var result = await controller.Create(employee);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
    var badRequestResult = result as BadRequestObjectResult;
    Assert.Equal("El nombre no puede contener números.", badRequestResult.Value);
}

[Fact]
public async Task AddEmployee_NameHasExcessiveRepeatingCharacters_ReturnsBadRequest()
{
    // Arrange
    var context = GetInMemoryDbContext();
    var controller = new EmployeeController(context);
    var employee = new Employee { Name = "Juuuannnn" };

    // Act
    var result = await controller.Create(employee);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
    var badRequestResult = result as BadRequestObjectResult;
    Assert.Equal("El nombre contiene caracteres repetidos de forma excesiva.", badRequestResult.Value);
}

}
}