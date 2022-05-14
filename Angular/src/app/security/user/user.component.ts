import { Component, ViewChildren, QueryList, OnInit, ElementRef } from '@angular/core';
import { UserDto } from '../../models';
import { SecurityService } from '../../services/security.service';
import { Router } from '@angular/router';
import { Time } from '@angular/common';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { ShoppingCartComponent } from '../../cart/shoppingCart/shoppingCart.component';
import { CartService } from '../../services/cart.service';

declare var $: any;

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css'],
})
export class UserComponent implements OnInit {
  constructor(public securityService: SecurityService) { }

  duration: number;
  credit: number;
  user: string = '';
  errors: string[] = [];
  againCode: boolean = false;
  @ViewChildren('seconde') private _notificationsElements: QueryList<ElementRef>

  RegisterOrLogin() {
    Swal.fire({
      html: `<h3>برای ثبت سفارش وارد شوید</h3><br/>
      <small> ایمیل*</small>
      <input type="text" id="email" class="swal2-input" placeholder="ایمیل"/>
      <small>شماره موبایل*</small>
      <input type="text" id="phoneNumber" class="swal2-input" placeholder="شماره موبایل"/>
      <small>.ایمیل و شماره موبایل خود را وارد کنید تا کد تایید به ایمیلتان ارسال شود</small>`,
      confirmButtonText: 'ارسال کد تایید',
      showCloseButton: true,
      padding: '30px',
      width: '650px',
      inputValidator: (value) => {
        return new Promise((resolve) => {
          console.log(value)
          if (value.length < 11)
            resolve('کد تایید برای ایمیل شما ارسال گردید');
          else
            resolve('لطفا ایمیل و شماره همراه را صحیح وارد نمایید!')
        });
      }
    })
      .then((result) => {
        if (result.isConfirmed) {
          var email = $("#email").val();
          var phoneNumber = $("#phoneNumber").val();
          $.ajax({
            contentType: 'application/x-www-form-urlencoded',
            dataType: 'json',
            type: "get",
            url: this.SendCode(email, phoneNumber),
          })
        }
        else if (result.isDenied) {
          Swal.fire(
            'محدودیت دسترسی'
          )
        }
      })
  }

  lastCodeRecived: any[] = [];
  SendCode(email: string, phoneNumber: string, success: boolean = true) {
    if (success) {
      var codeRecived: string = "";
      this.securityService.ReciveCode(email).subscribe((result: any) => {
        codeRecived = result.data;
        this.lastCodeRecived.push(result.data);
      });
    }

    var lastcode = "";
    var user = new UserComponent(this.securityService);
    Swal.fire({
      html: success ? `<h4>کد ارسال شده به ایمیل خود را وارد نمایید</h4>
      <input type="text" id="code" style="max-width: 300px" class="swal2-input" placeholder="کد تایید"/><br/>
      <small> <a id="again" style="cursor: pointer;">ارسال مجدد کد</a></small><br/>` :
        `<p>کد تایید را مجددا وارد نمایید!</p><br/>
      <input type="text" id="code" style="max-width: 300px" class="swal2-input" placeholder="کد تایید"/>`,
      confirmButtonText: 'تایید',
      showCloseButton: true,
      padding: '30px',
      width: '650px',
      footer: `<small id="seconde">ثانیه {{this.duration}}دریافت مجدد</small>`,
      inputValidator: (value) => {
        return new Promise((resolve) => {
          if (value.length == 5)
            resolve('منتظر بمانید...');
          else {
            this.againCode = true;
            resolve('کد تایید صحیح نمی باشد!')
          }
        });
      }
    })
      .then((result) => {
        if (result.isConfirmed) {
          var code = $("#code").val();
          lastcode = code;
          $.ajax({
            contentType: 'application/x-www-form-urlencoded',
            dataType: 'json',
            type: "get",
            url: this.VerifyCode(code, codeRecived == undefined ? this.lastCodeRecived[0] : codeRecived, phoneNumber, email),
          }).done(function () {
            user.againCode = false;
          })
        }
      })

    $("#again").click(function () {
      if (user.againCode == true)
        user.SendCode(email, phoneNumber, true);
      else if (lastcode != "")
        alert('کد تایید ارسال شده است');
    });
  }

  VerifyCode(code: string, codeRecived: string, phoneNumber: string, email: string) {
    if (code == codeRecived)
      this.AccountUser(phoneNumber);
    else
      this.SendCode(email, phoneNumber, false);
  }

  AccountUser(phoneNumber: string) {
    this.securityService.RegisterOrSignIn(phoneNumber).subscribe((result: UserDto) => {
      this.securityService.SaveData(result.data);
      if (result.data != null) {
        var fName = this.securityService.GetFieldFromJWT('firstName')!;
        var lName = this.securityService.GetFieldFromJWT('lastName')!;
        this.user = fName + lName;
      }
      else
        this.user = 'کاربر عزیز';

      // this.fillCart();    
    });
  }

  GetUser() {
    const IsValid = this.securityService.IsAuthenticated();
    if (IsValid) {
      const lastName = this.securityService.GetFieldFromJWT('lastName')!;
      const firstName = this.securityService.GetFieldFromJWT('firstName')!;
      if (firstName != 'null')
        this.user = firstName + " " + lastName;
      else
        this.user = 'کاربر عزیز';
    }
    else
      this.user = '';
  }

  // fillCart(){
  //   const phoneNumber = localStorage.getItem('phoneNumber')!;
  //   var shoppingCart = new ShoppingCartComponent(this.cartService);
  //   shoppingCart.loadCart(phoneNumber);  
  // }


  ngAfterViewInit() {
    this._notificationsElements.forEach((element: any) => {
      const htmlElement = element.nativeElement as HTMLElement;
      setTimeout(htmlElement.style.display = 'none', this.duration);
    });
  }

  ngOnInit(): void {
    this.GetUser();
  }
}
