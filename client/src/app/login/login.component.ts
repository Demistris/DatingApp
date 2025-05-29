import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private router = inject(Router);
  private toastr = inject(ToastrService);
  accountService = inject(AccountService);
  model: any = {};

  onLoginClick() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      // TODO: Handle errors in service
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
          this.toastr.error('Login failed. Please try again');
        }
        console.error('Login error:', error);
      },
    });
  }

  onRegisterClick() {
    this.router.navigateByUrl('/register');
  }

  onForgotPasswordClick() {
    this.toastr.info('In progress');
    // TODO: Make functionality for this method
  }

  onRememberMeToggle(isChecked: boolean) {
    this.toastr.info('In progress');
    // TODO: Make functionality for this methods
  }
}
