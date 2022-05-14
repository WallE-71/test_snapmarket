import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { SellerService } from '../services/seller.service';

@Component({
  selector: 'app-seller',
  templateUrl: './seller.component.html',
  styleUrls: ['./seller.component.css']
})
export class SellerComponent implements OnInit {
  constructor(private sellerService: SellerService, private formBuilder: FormBuilder) { }

  form: FormGroup;
  message: string;

  RequestRegister() {
    this.sellerService.Request(this.form.value).subscribe((result: any) => {
      if (result.data != null) {
        localStorage.setItem("jkk955", result.data);
        Swal.fire({
          icon: 'success',
          title: 'ثبت نام انجام شد !!!',
          text: 'لطفا منتظر پاسخ مدیریت سایت بمانید.',
          confirmButtonText: 'بستن'
        });
      }
      else {
        Swal.fire({
          icon: 'error',
          title: 'ثبت نام انجام نشد !!!',
          text: 'در مراحل ثبت نام خطایی رخ داده, لطفا پس ازمدت کوتاهی مجددا تلاش نمایید.',
          confirmButtonText: 'بستن'
        });
      }
    })
  }

  ReciveAnswer(){
    const email = localStorage.getItem("jkk955")!;
    this.sellerService.Answer(email).subscribe((result: any) => {
      if(result.data != null){
        this.message = result.data;
        localStorage.removeItem("jkk955");
      }
    })
  }

  ngOnInit(): void {
    this.ReciveAnswer();
    this.form = this.formBuilder.group({
      imageFile: ['', {
        validators: [Validators.required]
      }],
      name: ['', {
        validators: [Validators.required]
      }],
      surName: ['', {
        validators: [Validators.required]
      }],
      nationalId: ['', {
        validators: [Validators.required]
      }],
      phoneNumber: ['', {
        validators: [Validators.required]
      }],
      email: ['', {
        validators: [Validators.required]
      }],
      webSite: ['', {
        validators: [Validators.required]
      }],
      brand: ['', {
        validators: [Validators.required]
      }],
      activityType: ['', {
        validators: [Validators.required]
      }],
      store: ['', {
        validators: [Validators.required]
      }],
      telNumber: ['', {
        validators: [Validators.required]
      }],
      address: ['', {
        validators: [Validators.required]
      }],
      postalCode: ['', {
        validators: [Validators.required]
      }],
      establishmentDate: ['', {
        validators: [Validators.required]
      }],
      sampleProduct: ['', {
        validators: [Validators.required]
      }],
      description: ['', {
        validators: [Validators.required]
      }],
      scanNationalIdDocument: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
