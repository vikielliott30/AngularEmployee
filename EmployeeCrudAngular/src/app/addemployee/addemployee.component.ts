import { Component, OnInit } from '@angular/core';
import { Employee } from '../employee.model';
import { EmployeeService } from '../employee.service';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-addemployee',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './addemployee.component.html',
  styleUrls: ['./addemployee.component.css']
})
export class AddemployeeComponent implements OnInit {
  newEmployee: Employee = new Employee(0, '', '');
  submitBtnText: string = "Create";
  imgLoadingDisplay: string = 'none';

  constructor(
    private employeeService: EmployeeService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toastr: ToastrService // Inyecta el servicio de Toastr
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      const employeeId = params['id'];
      if (employeeId) {
        this.editEmployee(employeeId);
      }
    });
  }

  async addEmployee(employee: Employee) {
    if (!employee.name) {
      this.toastr.error("El nombre es obligatorio.");
      return;
    }

    // Verifica que el nombre no este vacio
    if (employee.name == "") {
      this.toastr.error("El nombre del empleado no puede estar vacio.");
      return;
    }

    // Longitud máxima del nombre y apellido (10 caracteres)
    if (employee.name.length > 10) {
      this.toastr.error("El nombre no puede superar los 10 caracteres.");
      return;
    }

    // Longitud mínima del nombre (2 caracteres)
    if (employee.name.length < 2) {
      this.toastr.error("El nombre debe tener al menos 2 caracteres.");
      return;
    }

    // Verificar que el nombre no contenga números
    if (/\d/.test(employee.name)) {
      this.toastr.error("El nombre no puede contener números.");
      return;
    }

    // Evitar caracteres repetidos de forma excesiva
    if (this.hasExcessiveRepeatingCharacters(employee.name)) {
      this.toastr.error("El nombre contiene caracteres repetidos de forma excesiva.");
      return;
    }

    // Guardar el empleado
    employee.createdDate = new Date().toISOString();
    if (employee.id === 0) {
      await this.employeeService.createEmployee(employee).toPromise();
    } else {
      await this.employeeService.updateEmployee(employee).toPromise();
    }

    this.router.navigate(['/']);
  }

  editEmployee(employeeId: number) {
    this.employeeService.getEmployeeById(employeeId).subscribe(res => {
      this.newEmployee.id = res.id;
      this.newEmployee.name = res.name;
      this.submitBtnText = "Edit";
    });
  }

  // Función para verificar caracteres repetidos
  private hasExcessiveRepeatingCharacters(name: string): boolean {
    const characterCount: { [key: string]: number } = {};
    for (const char of name) {
      characterCount[char] = (characterCount[char] || 0) + 1;
      if (characterCount[char] > 2) {
        return true;
      }
    }
    return false;
  }
}
