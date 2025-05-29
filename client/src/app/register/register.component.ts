import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  router = inject(Router);
  cancelRegister = output<boolean>();
  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        console.log('Registration successful:', response);
        this.cancel();
      },
      // TODO: Add to service
      error: (error) => {
        // Validation errors
        if (error.error?.errors) {
          const errorMessages = [];
          for (const key in error.error.errors) {
            if (error.error.errors[key]) {
              errorMessages.push(...error.error.errors[key]);
            }
          }
          this.toastr.error(errorMessages.join('\n'));
        }
        // Direct string error
        else if (typeof error.error === 'string') {
          this.toastr.error(error.error);
        }
        // Error object with a message property
        else if (error.error?.message) {
          this.toastr.error(error.error.message);
        }
        // Fallback for unexpected formats
        else {
          this.toastr.error('Registration failed. Please try again');
        }
        console.error('Registration failed:', error);
      },
      complete: () => {
        console.log('Registration request completed');
      },
    });
  }

  cancel() {
    console.log('Registration cancelled');
    this.router.navigate(['/']);
  }
}
