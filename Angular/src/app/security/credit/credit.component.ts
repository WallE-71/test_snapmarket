import { Component, OnInit } from '@angular/core';
import { SecurityService } from '../../services/security.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-credit',
  templateUrl: './credit.component.html',
  styleUrls: ['./credit.component.css']
})
export class CreditComponent implements OnInit {
  constructor(public securityService: SecurityService, private formBuilder: FormBuilder) { }

  form: FormGroup;
  credit: any;

  InsertCreditCart(){
    var phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    if(phoneNumber != null && phoneNumber != 'undefined' && phoneNumber != ''){
      debugger
      this.securityService.IncreaseCredit(phoneNumber, this.form.value.credit, this.form.value.nationalId, this.form.value.bankCode).subscribe((result: any) => {
        if(result.isSuccess)
          this.credit = result.data;
          debugger       
      });
    }
  }

  ngOnInit(): void {
    var phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    if(phoneNumber != null && phoneNumber != 'undefined' && phoneNumber != ''){
      this.securityService.GetCreditCart(phoneNumber).subscribe((result: any) => {
        if(result.isSuccess)
          this.credit = result.data; 
      });
    }

    this.form = this.formBuilder.group({
      credit: ['', {
        validators: [Validators.required]
      }],
      nationalId: ['', {
        validators: [Validators.required]
      }],
      bankCode: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
