import { Component, OnInit, Input, EventEmitter } from '@angular/core';
// import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserDto } from '../models';
import { SecurityService } from '../services/security.service';
import Swal from 'sweetalert2/dist/sweetalert2.js';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  constructor(public securityService: SecurityService, private router: Router, private formBuilder: FormBuilder) {}

  email: string;
  address: string;
  lastName: string;
  firstName: string;
  phoneNumber: string;
  form: FormGroup;

  profile(){
    this.email = this.securityService.GetFieldFromJWT('email')!;
    this.address = this.securityService.GetFieldFromJWT('address')!;
    this.lastName = this.securityService.GetFieldFromJWT('lastName')!;
    this.firstName = this.securityService.GetFieldFromJWT('firstName')!;
    this.phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    if(this.phoneNumber != null || this.phoneNumber != undefined){
      this.router.navigate(['/profile'], {
        queryParams: { phoneNumber: this.phoneNumber}
      });
    }
  }

  Edit(){
    if(this.form.value.email == "")
      this.form.value.email = this.email;
    if(this.form.value.firstName == "")
      this.form.value.firstName = this.firstName;
    if(this.form.value.lastName == "")
      this.form.value.lastName = this.lastName;
    if(this.form.value.address == "")
      this.form.value.address = this.address;
    this.securityService.EditProfile(this.form.value).subscribe((result: any) => {
      this.securityService.SaveData(result.data);
    })
  }

  Order() {
    return this.router.navigate(['/order']);
  }

  ngOnInit(): void {
    this.profile();
    this.form = this.formBuilder.group({
      email: ['', {
        validators: [Validators.required]
      }],
      firstName: ['', {
        validators: [Validators.required]
      }],
      lastName: ['', {
        validators: [Validators.required]
      }],
      address: ['', {
        validators: [Validators.required]
      }],
    })
  }
}
