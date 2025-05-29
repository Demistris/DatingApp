import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-nav',
  imports: [
    FormsModule,
    BsDropdownModule,
    RouterLink,
    RouterLinkActive,
    TitleCasePipe,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  private router = inject(Router);
  private toastr = inject(ToastrService);
  accountService = inject(AccountService);
  model: any = {};

  get user() {
    return this.accountService.currentUser();
  }

  login() {
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

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
